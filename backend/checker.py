from tika import parser
import re
raw = parser.from_file("sample.pdf")
text = raw["content"]

groups_list = []
output = {}

teacher_list = ["Записной Д.В.", "Володин С.М.", "Юрченкова И.А.", "Сельченков Д.А.", "Семенихина А.В.", "Разин А.Л.", "Князева К.М.", "Филатов В.А.", "Поколодина Е.В.", "Леглер А.О.", "Перхункова Е.Д.", "Морозова М.В."]
subjects_list = ["Русскийязык", "Русскийязык", "Теориявероятностейиматематическаястатистика", "Иностранныйязык", "Иностранныйязык", "Иностранныиязык","Математика", "Инженернаяикомпьютернаяграфика","Теориявероятностейиматематическаястатистика","Эксплуатациякомпьютерныхсетей","Основыэкономики","Бухгалтерскийучет","Технологияразработкиизащитыбазданных","Эксплуатациякомпьютерныхсетей","ЭкономическиеосновыПОДФТ"]

groupflag = False
thisgroupflag = False
thisgroupnumper = 0
grouplesoncounter = 1

newthisgroupline = ""
for line in text.splitlines():

    #Список групп с изменениями 
    if (line!="") and (groupflag == False):
        groups_list = line.split(" ")
        groupflag = True
    
    #Получаем индекс группы
    elif (line.isdigit() == True):
        thisgroupflag = True
        thisgroupnumper = int(line)
        grouplesoncounter = 0
        output[thisgroupnumper] = {"0": {},
                                   "1": {},
                                   "2": {},
                                   "3": {},
                                   "4": {},
                                   "5": {},
                                   

            }

    #Если предмет в одну линию
    elif (line!="") and (thisgroupflag == True) and (line in subjects_list): 
        output[thisgroupnumper][str(grouplesoncounter)]["subject"] = line.replace("  ","")
        newthisgroupline = ""

    #Проверка на сопоставление созданной строки с предметами/если предмет в несколько линий
    elif newthisgroupline in subjects_list:
            output[thisgroupnumper][str(grouplesoncounter)]["subject"] = line.replace("  ","")
            newthisgroupline = ""

    #Генерация строки/если предмет в несколько линий
    elif (line!="") and (thisgroupflag == True) and not (line in teacher_list):
            newthisgroupline += line.replace(" ","").replace("     ","")
    
    #Проверка на учителей
    elif (line!="") and (thisgroupflag == True) and (line in teacher_list):
        output[thisgroupnumper][str(grouplesoncounter)]["teacher"] = line
        grouplesoncounter += 1

    print(newthisgroupline)


print(groups_list)
print(output)
print(output)