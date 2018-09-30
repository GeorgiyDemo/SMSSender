import numpy as np
import cv2

color_blue = (255,0,0)
color_red = (0,0,128)
# загрузите изображение, смените цвет на оттенки серого и уменьшите резкость
image = cv2.imread("example.jpg")
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
gray = cv2.GaussianBlur(gray, (3, 3), 0)
cv2.imwrite("gray.jpg", gray)
# распознавание контуров
edged = cv2.Canny(gray, 10, 250)
cv2.imwrite("edged.jpg", edged)
# создайте и примените закрытие
#65 - где нет надписей
kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (3, 3))
closed = cv2.morphologyEx(edged, cv2.MORPH_CLOSE, kernel)
cv2.imwrite("closed.jpg", closed)
# найдите контуры в изображении и подсчитайте количество книг
cnts = cv2.findContours(closed.copy(), cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)[1]
total = 0
# цикл по контурам
for c in cnts:
    # аппроксимируем (сглаживаем) контур
    rect = cv2.minAreaRect(c) # пытаемся вписать прямоугольник
    box = cv2.boxPoints(rect)
    box = np.int0(box)
    center = (int(rect[0][0]),int(rect[0][1]))
    area = int(rect[1][0]*rect[1][1])

    edge1 = np.int0((box[1][0] - box[0][0],box[1][1] - box[0][1]))
    edge2 = np.int0((box[2][0] - box[1][0], box[2][1] - box[1][1]))

    usedEdge = edge1
    if cv2.norm(edge2) > cv2.norm(edge1):
        usedEdge = edge2

    reference = (1,0) # horizontal edge

    if (area > 14000) and (area < 60000) and (rect[1][0] > 100):
        cv2.drawContours(image,[box],0,color_blue,2)
        cv2.circle(image, center, 5, color_red, 2)

# показываем результирующее изображение
print("Я нашёл {0} книг на этой картинке".format(total))
cv2.imwrite("output.jpg", image)