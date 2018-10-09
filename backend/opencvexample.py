import cv2, numpy, requests, time
from collections import Counter
from pdf2image import convert_from_path

all_circles_xyz = []
group_cell_list = []
circle_store_list = []
finalmatrix = []

def get_document():

    url = 'http://178.128.225.114/PDF/4.pdf'
    r = requests.get(url, stream=True)
    with open('PDF.pdf', 'wb') as fd:
        for chunk in r.iter_content(2000):
            fd.write(chunk)
    pages = convert_from_path('PDF.pdf', 150)
    for page in pages:
        page.save('JPG.jpg', 'JPEG')

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
        if ((item[0] == center[0]) and (abs(item[1]-center[1])<4)) or ((item[1] == center[1]) and (abs(item[0]-center[0])<4)):
            return False
    return True

def titlechecker(box):
    for item in group_cell_list:
        for boxitem in box:
            for p in range(1,len(item)-1):
                    pt = (item[p][0],item[p][1])
                    pm = (boxitem[0], boxitem[1])
                    if pt == pm:
                        return False
    return True

#((403.5, 307.0), (180.0, 147.0), -90.0)
#center: (403, 307)
#[[477 397]
#[330 397]
#[330 217]
#[477 217]]

#[[330 238]
#[330 217]
#[477 217]
#[477 238]]

        #print(res[1][0]>item[1][0])
        #print(res[0][0]>item[0][0])
        #print(res[0][1]>item[0][1])
        #if res[1][0]>item[1][0] and ((res[0][0]>item[0][0]) or (res[0][1]>item[0][1])):
        #    return True
    return False

#Функция для погрешности центров кругов
def centers_checker(a,b):
    boolflag0 = False
    boolflag1 = False
    if (abs(a[0]-b[0]) < 5):
        boolflag0 = True
    if (abs(a[1]-b[1]) < 5):
        boolflag1 = True
    if boolflag1 == True and boolflag0 == True:
        return True
    return False

def get_null_values(timg,image):

    outchecklist = []
    kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (90, 90))
    closed = cv2.morphologyEx(timg, cv2.MORPH_CLOSE, kernel)
    cnts = cv2.findContours(closed.copy(), cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)[1]
    cv2.imwrite("CHECK.jpg",closed)
    firstflag = True
    old_center = (0, 0)
    old_rect = ((0.0, 0.0), (0.0, 0.0), -0.0)

    for c in cnts:

        rect = cv2.minAreaRect(c)
        box = cv2.boxPoints(rect)
        box = numpy.int0(box)
        center = (int(rect[0][0]),int(rect[0][1]))
        area = int(rect[1][0]*rect[1][1])

        #Проверка на основные ячейки
        if (check_res(firstflag,old_rect,rect) == True) and (area > 10000) and (area < 60000) and (rect[1][0] > 75) and (rect[1][1] > 75) and (centers_checker(center,old_center) == False):
            firstflag = False
            outchecklist.append(center)
            old_rect = rect
            old_center = center

            cv2.drawContours(image,[box],0,(0,0,0),2)
            cv2.circle(image, center, 5, (0,0,0), 2)

    #Смещение на 1 пиксель
    print(outchecklist)
    for i in range(len(outchecklist)):
        buftuple = outchecklist[i]
        outchecklist.remove(outchecklist[i])
        outchecklist.insert(i,(buftuple[0]-1, buftuple[1]-1))

    #Добавляем 0 там, где есть совпадение
    for nullitem in outchecklist:
        for item in finalmatrix:
            for i in range(len(item)):
                if nullitem == item[i]:
                    item.remove(item[i])
                    item.insert(i,0)
    
    #Где нет нуля, пишем 1
    for item in finalmatrix:
        for i in range(len(item)):
            if item[i] != 0:
                item.remove(item[i])
                item.insert(i,1)


    print(finalmatrix)

def main():

    get_document()
    RowCheckerList=[]
    ColumnCheckerList=[]
    firstflag = True

    image = cv2.imread("JPG.jpg")
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    gray = cv2.GaussianBlur(gray, (3, 3), 0)
    edged = cv2.Canny(gray, 10, 250)
    cnts = cv2.findContours(edged.copy(), cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)[1]

    #Едем по контурам
    global_counter = 0
    old_center = (0, 0)
    old_rect = ((0.0, 0.0), (0.0, 0.0), -0.0)
    
    #Проверка на ячейки названий групп
    for c in cnts:
    
        rect = cv2.minAreaRect(c)
        box = cv2.boxPoints(rect)
        box = numpy.int0(box)
        center = (int(rect[0][0]),int(rect[0][1]))
        area = int(rect[1][0]*rect[1][1])
        
        if (rect[1][1] < 50) and (rect[1][1] > 17) and (rect[1][0] > 100):

            for p in box:
                pt = (p[0],p[1])
                cv2.circle(image, pt, 5, (0,0,255), 2)
                smallimg = cv2.resize(image, (0,0), fx=0.5, fy=0.5)
                cv2.imshow("CHECK",smallimg)
                cv2.waitKey(100000)
                
            
            cv2.drawContours(image,[box],0,(0,0,255),2)
            cv2.circle(image, center, 5, (0,0,255), 2)
            group_cell_list.append(box)
            print(rect)
            print("center: "+str(center))
            smallimg = cv2.resize(image, (0,0), fx=0.5, fy=0.5)
            cv2.imshow("CHECK",smallimg)
            cv2.waitKey(100000)

    for c in cnts:

        rect = cv2.minAreaRect(c)
        box = cv2.boxPoints(rect)
        box = numpy.int0(box)
        center = (int(rect[0][0]),int(rect[0][1]))
        area = int(rect[1][0]*rect[1][1])

        if (check_res(firstflag,old_rect,rect) == True) and (area > 10000) and (area < 60000) and (rect[1][0] > 75) and (rect[1][1] > 75) and (centers_checker(center,old_center) == False) and (allcenters_checker(center) == True) and (titlechecker(box) == True):
            firstflag = False
            circle_store_list.append(center)
            old_rect = rect
            old_center = center

            cv2.drawContours(image,[box],0,(0,0,128),2)
            cv2.circle(image, center, 5, (0,0,128), 2)
            print(rect)
            print("center: "+str(center))
            #Закидываем центры на проверку для подсчета кол-ва повторений
            ColumnCheckerList.append(center[0])
            RowCheckerList.append(center[1])

            smallimg = cv2.resize(image, (0,0), fx=0.5, fy=0.5)
            cv2.imshow("CHECK",smallimg)
            cv2.waitKey(100000)
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
    
    print(MaxRow*MaxColumn)
    print(global_counter)
    if global_counter == MaxRow*MaxColumn:
        matrix(MaxRow,MaxColumn)
        get_null_values(edged.copy(),image)

    cv2.imwrite("output.jpg", image)

main()