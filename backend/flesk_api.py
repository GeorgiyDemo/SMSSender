import cv2, numpy, requests, time, pytesseract, flask, os, json, threading
from datetime import datetime
from collections import Counter
from pdf2image import convert_from_bytes
from flask import request, jsonify
app = flask.Flask(__name__)
app.config["DEBUG"] = True
GLOBAL_RESULT = {}

def ParseKIPTT():
    global GLOBAL_RESULT
    leftnumber_cell_list = []
    group_cell_list = []
    circle_store_list = []
    finalmatrix = []
    group_text_association = {}
    center_and_text = {}

    #Процедура получения PDF и конвертации в JPG
    def get_document():

        #url = 'http://178.128.225.114/PDF/2.pdf'
        #r = requests.get(url, stream=True)
        #with open('PDF.pdf', 'wb') as fd:
        #    for chunk in r.iter_content(2000):
        #        fd.write(chunk)
        
        pages = convert_from_bytes(open('PDF.pdf', 'rb').read(),300)
        for page in pages:
            page.save('PNG.png', 'PNG')

    #Процедура для того, чтоб контур не был огромным
    def check_res(firstflag, old, new):
        if firstflag == True:
            return True
        else:
            return new[1][0]<(old[1][0]+(old[1][0]/100)*80) and new[1][1]<(old[1][1]+(old[1][1]/100)*80)

    #Процедура визаулизации матрицы
    def matrix(rows,columns):

        matrix = []
        counter = 0
        circle_store_list.sort(key=lambda x: (x[1]),reverse=True)
        for i in range(columns):
            matrix.append([])
            for j in range(rows):
                matrix[i].append(circle_store_list[counter])
                counter +=1
        for item in matrix:
            item.sort(key=lambda x: (x[0]))
        i = len(matrix)-1
        while i != -1:
            finalmatrix.append(matrix[i])
            i -=1 

    #Функция для определения всех погрешностей кругов
    def allcenters_checker(center):
        for item in circle_store_list:
            if ((item[0] == center[0]) and (abs(item[1]-center[1])<8)) or ((item[1] == center[1]) and (abs(item[0]-center[0])<8)):
                return False
        return True

    #Функция для проверки на то, чтоб контур не включал в себя номера пар
    def leftnumberchecker(box):
        for item in leftnumber_cell_list:
            for boxitem in box:
                for p in range(0,len(item)-2):
                    if (item[p][0],item[p][1]) == (boxitem[0], boxitem[1]):
                        return False
        return True

    #Функция для проверки на то, чтоб контур не включал в себя названия групп
    def titlechecker(box):
        for item in group_cell_list:
            for boxitem in box:
                for p in range(1,len(item)-1):
                    if (item[p][0],item[p][1]) == (boxitem[0], boxitem[1]):
                        return False
        return True

    #Функция для погрешности центров кругов
    def centers_checker(a,b):
        boolflag0 = False
        boolflag1 = False
        if (abs(a[0]-b[0]) < 10):
            boolflag0 = True
        if (abs(a[1]-b[1]) < 10):
            boolflag1 = True
        if boolflag1 == True and boolflag0 == True:
            return True
        return False

    #Определение null-пар
    def get_null_values(timg,image):

        outchecklist = []
        kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (180, 180))
        closed = cv2.morphologyEx(timg, cv2.MORPH_CLOSE, kernel)
        cnts = cv2.findContours(closed.copy(), cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)[1]
        firstflag = True
        old_center = (0, 0)
        old_rect = ((0.0, 0.0), (0.0, 0.0), -0.0)

        for c in cnts:

            rect = cv2.minAreaRect(c)
            box = cv2.boxPoints(rect)
            box = numpy.int0(box)
            center = (int(rect[0][0]),int(rect[0][1]))

            if (check_res(firstflag,old_rect,rect) == True) and (AreaChecker(rect) == True) and (centers_checker(center,old_center) == False) and (titlechecker(box) == True):
                firstflag = False
                outchecklist.append(center)
                old_rect = rect
                old_center = center

                cv2.drawContours(image,[box],0,(0,0,255),5)
                cv2.circle(image, center, 5, (0,0,255), 5)

        for nullitem in outchecklist:
            for item in finalmatrix:
                for i in range(len(item)):
                    if (abs(nullitem[0]-item[i][0])<10) and (abs(nullitem[1]-item[i][1])<10):
                        item.remove(item[i])
                        item.insert(i,(0,0))
        
        for item in finalmatrix:
            for i in range(len(item)):
                if item[i] == (0,0):
                    item.remove(item[i])
                    item.insert(i,0)
                else:
                    bufitem = item[i]
                    item.remove(bufitem)
                    item.insert(i,center_and_text[bufitem])

    def finalmatrix_to_json(groupcheck):
        global GLOBAL_RESULT
        allgroups = []
        outjson = {}
        for item in groupcheck:
            allgroups.append(item[1])
        
        for i in range(len(finalmatrix[0])):
            outjson[allgroups[i]] = []
            for j in range(len(finalmatrix)):
                outjson[allgroups[i]].append(finalmatrix[j][i])
        
        print(finalmatrix)
        print(allgroups)
        GLOBAL_RESULT = outjson

    #Фильтрация периметра
    def AreaChecker(res):
        area = int(res[1][0]*res[1][1])
        if (area > 20000) and (area < 500000) and (res[1][0] > 150) and (res[1][1] > 150):
            return True
        return False

    def grouptextchecker(text):
        formatedtext = "".join(text.split())
        while formatedtext[-1:].isnumeric() == False:
            formatedtext = formatedtext[:-1]
        return formatedtext
    
    def cropimager(image, box):
        TopLeftCoords = (box[0][0], box[0][1])
        BottomRightCoords = TopLeftCoords
        
        for p in box:
            if p[0]<=TopLeftCoords[0] and p[1]<=TopLeftCoords[1]:
                TopLeftCoords = (p[0],p[1])
            
            if p[0]>=BottomRightCoords[0] and p[1]>=BottomRightCoords[1]:
                BottomRightCoords = (p[0],p[1])

        return image[TopLeftCoords[1]:BottomRightCoords[1],TopLeftCoords[0]:BottomRightCoords[0]]

    get_document()
    RowCheckerList=[]
    ColumnCheckerList=[]
    firstflag = True

    image = cv2.imread("PNG.png")
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    gray = cv2.GaussianBlur(gray, (3, 3), 0)
    edged = cv2.Canny(gray, 10, 250)
    cnts = cv2.findContours(edged.copy(), cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)[1]
    old_center = (0, 0)
        
    for c in cnts:
        
        rect = cv2.minAreaRect(c)
        box = cv2.boxPoints(rect)
        box = numpy.int0(box)
        center = (int(rect[0][0]),int(rect[0][1]))
            
        #Заголовки групп
        if (rect[1][1] < 100) and (rect[1][1] > 30) and (rect[1][0] > 120) and (rect[1][0] > rect[1][1]) and (rect[0][0] > rect[0][1]) and (centers_checker(center,old_center) == False) and (rect[1][0] < 500):
            crop_img = image[box[1][1]:box[0][1], box[1][0]:box[2][0]]
            text = pytesseract.image_to_string(crop_img, lang='rus')
            group_text_association[center] = grouptextchecker(text)

            old_center = center
            for p in box:
                cv2.circle(image, (p[0],p[1]), 5, (0,255,0), 5)
            cv2.drawContours(image,[box],0,(0,255,0),5)
            cv2.circle(image, center, 5, (0,255,0), 5)
            group_cell_list.append(box)

            
        #Цифры пар (слева)
        if (rect[1][0] < 200) and (rect[1][0] > 34) and (rect[1][1] < 600) and (rect[1][0] < rect[1][1]) and (rect[0][0] < rect[0][1]):
            for p in box:
                cv2.circle(image, (p[0],p[1]), 5, (255,0,255), 5)
                
            cv2.drawContours(image,[box],0,(255,0,255),5)
            cv2.circle(image, center, 5, (255,0,255), 5)
            leftnumber_cell_list.append(box)

    #Едем по контурам
    sorted_by_value = sorted(group_text_association.items(), key=lambda kv: kv[0])
    global_counter = 0
    old_center = (0, 0)
    old_rect = ((0.0, 0.0), (0.0, 0.0), -0.0)

    for c in cnts:

        rect = cv2.minAreaRect(c)
        box = cv2.boxPoints(rect)
        box = numpy.int0(box)
        center = (int(rect[0][0]),int(rect[0][1]))

        if (check_res(firstflag,old_rect,rect) == True) and (AreaChecker(rect) == True) and (centers_checker(center,old_center) == False) and (allcenters_checker(center) == True) and (titlechecker(box) == True) and (leftnumberchecker(box) == True):

            firstflag = False
            circle_store_list.append(center)
            old_rect = rect
            old_center = center

            cv2.drawContours(image,[box],0,(128,0,0),5)

            crop_img = cropimager(image, box)
            text = pytesseract.image_to_string(crop_img, lang='rus')
            center_and_text[center] = text.replace("\n"," ").replace("  "," ").replace("\n\n"," ")
            #Закидываем центры на проверку для подсчета кол-ва повторений
            ColumnCheckerList.append(center[0])
            RowCheckerList.append(center[1])

            global_counter +=1

    #Считаем кол-во повторений 
    RowCounter = Counter(RowCheckerList)
    ColumnCounter = Counter(ColumnCheckerList)

    #Ищем максимальные элементы в структурированных объектах
    MaxRow = 0
    MaxColumn = 0
    for item in list(RowCounter):
        if (RowCounter[item]>MaxRow):
            MaxRow=RowCounter[item]
    for item in list(ColumnCounter):
        if (ColumnCounter[item]>MaxColumn):
            MaxColumn=ColumnCounter[item]

    if not os.path.exists("outputs"):
            os.makedirs("outputs")
    if global_counter == MaxRow*MaxColumn:
        matrix(MaxRow,MaxColumn)
        get_null_values(edged.copy(),image)   
        cv2.imwrite("outputs/output"+str(datetime.now())+"_GOOD.png", image)
        finalmatrix_to_json(sorted_by_value)
    
    else:
        cv2.imwrite("outputs/output"+str(datetime.now())+"_BAD.png", image)
        GLOBAL_RESULT = {"False"}

@app.route('/api/v1/parse_json/', methods=['GET'])
def parse_json():
    threading.Thread(target=ParseKIPTT).start()
    return "TRUE"

@app.route('/api/v1/getjson/', methods=['GET'])
def get_json():
    global GLOBAL_RESULT
    return json.dumps(GLOBAL_RESULT, ensure_ascii=False)

app.run(host='127.0.0.1',port=500, threaded=False)