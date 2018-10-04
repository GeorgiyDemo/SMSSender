import cv2, numpy, random, requests
from collections import Counter
from pdf2image import convert_from_path

circle_store_list = []
finalmatrix = []

def get_document():

    url = 'http://178.128.225.114/PDF/3.pdf'
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
    circle_store_list.sort(key=lambda x: (x[1]),reverse=True)
    print(len(circle_store_list))
    for i in range(columns):
        matrix.append([])
        for j in range(rows):
            matrix[i].append(circle_store_list[counter])
            counter +=1

    print("COUNTER: "+str(counter))
    print(len(circle_store_list))
    
    print(matrix)

    #[
    # [(1487, 970), (1319, 970), (1146, 970), (966, 970), (783, 970), (603, 970), (431, 970), (267, 970)],
    # [(1486, 827), (1319, 827), (1146, 827), (966, 827), (783, 827), (603, 827), (431, 827), (267, 827)],
    # [(1487, 681), (1319, 681), (1146, 681), (966, 681), (783, 681), (603, 681), (431, 681), (267, 681)],
    # [(1487, 519), (1319, 519), (1146, 519), (966, 519), (783, 519), (603, 519), (431, 519), (267, 519)],
    # [(966, 342), (1487, 341), (1319, 341), (1146, 341), (783, 341), (603, 341), (431, 341), (267, 341)]
    # ]

    for item in matrix:
        item.sort(key=lambda x: (x[0]))

    #[
    # [(267, 970), (431, 970), (603, 970), (783, 970), (966, 970), (1146, 970), (1319, 970), (1487, 970)],
    # [(267, 827), (431, 827), (603, 827), (783, 827), (966, 827), (1146, 827), (1319, 827), (1486, 827)],
    # [(267, 681), (431, 681), (603, 681), (783, 681), (966, 681), (1146, 681), (1319, 681), (1487, 681)],
    # [(267, 519), (431, 519), (603, 519), (783, 519), (966, 519), (1146, 519), (1319, 519), (1487, 519)],
    # [(267, 341), (431, 341), (603, 341), (783, 341), (966, 342), (1146, 341), (1319, 341), (1487, 341)]
    # ]

    i = len(matrix)-1
    while i != -1:
        finalmatrix.append(matrix[i])
        i -=1
    
    print(finalmatrix)

    # [
    # [(267, 341), (431, 341), (603, 341), (783, 341), (966, 342), (1146, 341), (1319, 341), (1487, 341)],
    # [(267, 519), (431, 519), (603, 519), (783, 519), (966, 519), (1146, 519), (1319, 519), (1487, 519)],
    # [(267, 681), (431, 681), (603, 681), (783, 681), (966, 681), (1146, 681), (1319, 681), (1487, 681)],
    # [(267, 827), (431, 827), (603, 827), (783, 827), (966, 827), (1146, 827), (1319, 827), (1486, 827)],
    # [(267, 970), (431, 970), (603, 970), (783, 970), (966, 970), (1146, 970), (1319, 970), (1487, 970)]
    # ]

    #[
    # [(259, 309), (411, 309), (552, 309), (692, 309), (834, 309)],
    # [(259, 461), (411, 461), (552, 461), (692, 461), (834, 461)],
    # [(259, 599), (410, 599), (552, 599), (691, 599), (834, 599)],
    # [(259, 724), (410, 724), (552, 724), (691, 724), (834, 724)],
    # [(259, 846), (411, 846), (552, 846), (692, 846), (834, 846)]
    # ]

#Процедура дополнительной дорисовки кругов (а надо ли?)
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
            print(Xcircle) #здесь проверяем на макс длинну все Xcircle (как лучше??)
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

def get_null_values(timg,image):

    kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (90, 90))
    closed = cv2.morphologyEx(timg, cv2.MORPH_CLOSE, kernel)
    cv2.imwrite("CHECKER.jpg", closed)
    cnts = cv2.findContours(closed.copy(), cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)[1]
    
    firstflag = True
    old_center = (0, 0)
    old_rect = ((0.0, 0.0), (0.0, 0.0), -0.0)

    for c in cnts:

        rect = cv2.minAreaRect(c)
        box = cv2.boxPoints(rect)
        box = numpy.int0(box)
        center = (int(rect[0][0]),int(rect[0][1]))
        area = int(rect[1][0]*rect[1][1])
        
        if (check_res(firstflag,old_rect,rect) == True) and (area > 10000) and (area < 60000) and (rect[1][0] > 75) and (rect[1][1] > 75) and (centers_checker(center,old_center) == False):
            firstflag = False
            circle_store_list.append(center)
            old_rect = rect
            old_center = center

            cv2.drawContours(image,[box],0,(0,0,0),2)
            cv2.circle(image, center, 5, (0,0,0), 2)

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
    #circle_checker(MaxRow,MaxColumn)
    matrix(MaxRow,MaxColumn)
    get_null_values(edged.copy(),image)
    #Итоги
    print("{0} предметов".format(global_counter))
    cv2.imwrite("output.jpg", image)

main()