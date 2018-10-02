import cv2, numpy, random, requests
from collections import Counter
from pdf2image import convert_from_path

circle_store_list = []

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
        return new[1][0]<(old[1][0]+(old[1][0]/100)*80)

#Процедура визаулизации матрицы
def matrix(rows,columns):
    matrix = []

    counter = 0
    print(len(circle_store_list))
    for i in range(columns):
        matrix.append([])
        for j in range(rows):
            matrix[i].append(0)
            print(counter)
            counter +=1
    
    print(matrix)
            


#Процедура дополнительной дорисовки кругов
def circle_checker(rows,columns):

    Xcircle = []
    Ycircle = []
    nparr = numpy.array([])
    print("Врубаем дорисовку центров на основе текущих..")
    print("------")
    for i in range(1,len(circle_store_list)):
        bufcheck = abs(circle_store_list[i-1][0]-circle_store_list[i][0])

        if bufcheck < 250:
            Xcircle.append(bufcheck)
        if (i%rows==0):
            #print(Xcircle) #здесь проверяем на макс длинну все Xcircle (как лучше??)
            nparr = numpy.append(nparr, Xcircle)
            Xcircle = []
            #Ycircle.append(abs(circle_store_list[i][1]-circle_store_list[j][1]))
    print("-----")
    print(nparr)

    print("\nКол-во строк: "+str(rows))
    print("Кол-во колонок: "+str(columns))
    #print(Ycircle)


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


def main():

    get_document()
    RowCheckerList=[]
    ColumnCheckerList=[]
    firstflag = True

    image = cv2.imread("JPG.jpg")
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    cv2.imwrite("gray.jpg", gray)
    gray = cv2.GaussianBlur(gray, (3, 3), 0)
    edged = cv2.Canny(gray, 10, 250)
    cv2.imwrite("edged.jpg", edged)
    cnts = cv2.findContours(edged.copy(), cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)[1]

    #Едем по контурам
    global_counter = 0
    old_center = (0, 0)
    old_rect = ((0.0, 0.0), (0.0, 0.0), -0.0)
    for c in cnts:

        rect = cv2.minAreaRect(c) # пытаемся вписать прямоугольник
        box = cv2.boxPoints(rect)
        box = numpy.int0(box)
        center = (int(rect[0][0]),int(rect[0][1]))
        area = int(rect[1][0]*rect[1][1])
        
        if (check_res(firstflag,old_rect,rect) == True) and (area > 10000) and (area < 60000) and (rect[1][0] > 75) and (rect[1][1] > 75) and (centers_checker(center,old_center) == False):
            
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

    #if global_counter !=(MaxRow*MaxColumn):
    circle_checker(MaxRow,MaxColumn)
    matrix(MaxRow,MaxColumn)
    #Итоги
    print("{0} предметов".format(global_counter))
    cv2.imwrite("output.jpg", image)

main()