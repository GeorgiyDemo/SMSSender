#импортируем нужные библиотеки
from PIL import Image
import pytesseract
#проверяем запущен ли скрипт из консоли
if __name__ == '__main__':
    #открываем или создаём файл для записи
    #флаг 'w' - говорит, что файл открывается для записи а если не его нет, то создаётся
    f= open('text_photo.txt','w')
    #получаем текст из фото
    text = pytesseract.image_to_string(Image.open('JPG.png'), lang='rus')
    print(text)
    #записываем его
    f.write(text)