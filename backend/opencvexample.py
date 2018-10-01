import cv2, numpy
from collections import Counter
from scipy.spatial import distance


circle_store_list = [(0,0)]
def circle_checker(circle):
    Xcircle = []
    Ycircle = []
    circle_store_list.append(circle)
    for i in range(0,(len(circle_store_list))-1):
        for j in range(1,len(circle_store_list)):
            Xcircle.append(abs(circle_store_list[i][0]-circle_store_list[j][0]))
            Ycircle.append(abs(circle_store_list[i][1]-circle_store_list[j][1]))

    if Xcircle        
    print(Xcircle)
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


color_blue = (255,0,0)
color_red = (0,0,128)

RowCheckerList=[]
ColumnCheckerList=[]

image = cv2.imread("example.jpg")
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
cv2.imwrite("gray.jpg", gray)
edged = cv2.Canny(gray, 10, 250)
cv2.imwrite("edged.jpg", edged)
cnts = cv2.findContours(edged.copy(), cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)[1]

#Едем по контурам
global_counter = 0
old_center = (0, 0)
for c in cnts:

    rect = cv2.minAreaRect(c) # пытаемся вписать прямоугольник
    box = cv2.boxPoints(rect)
    box = numpy.int0(box)
    center = (int(rect[0][0]),int(rect[0][1]))
    area = int(rect[1][0]*rect[1][1])
    if (area > 10000) and (area < 60000) and (rect[1][0] > 75) and (rect[1][1] > 75) and (centers_checker(center,old_center) == False):
        circle_checker(center)
        old_center = center
        cv2.drawContours(image,[box],0,color_blue,2)
        cv2.circle(image, center, 5, color_red, 2)
        print("center: "+str(center))
        #Закидываем center на проверку
        ColumnCheckerList.append(center[0])
        RowCheckerList.append(center[1])

        smallimg = cv2.resize(image, (0,0), fx=0.5, fy=0.5)
        cv2.imshow("CHECK",smallimg)
        cv2.waitKey(100000)
        global_counter +=1

#Считаем кол-во повторений
RowCounter = Counter(RowCheckerList)
ColumnCounter = Counter(ColumnCheckerList)

#Ищем максимальные элементы
MaxRow = 0
MaxColumn = 0
for item in list(RowCounter):
    if (RowCounter[item]>MaxRow):
        MaxRow=RowCounter[item]

for item in list(ColumnCounter):
    if (ColumnCounter[item]>MaxColumn):
        MaxColumn=ColumnCounter[item]

print(MaxRow)
print(MaxColumn)

#Итоги
print("Я нашёл {0} ужс".format(global_counter))
cv2.imwrite("output.jpg", image)