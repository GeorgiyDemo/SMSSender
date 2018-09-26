from tika import parser
import re
raw = parser.from_file("sample.pdf")
text = raw["content"]

groups_list = []
output = {}

teacher_list = ["Записной Д.В.", "Володин С.М.", "Юрченкова И.А.", "Сельченков Д.А.", "Семенихина А.В.", "Разин А.Л.", "Князева К.М.", "Филатов В.А.", "Поколодина Е.В.", "Леглер А.О.", "Перхункова Е.Д.", "Морозова М.В."]
subjects_list = [""]

groupflag = False
thisgroupflag = False
thisgroupnumper = 0
grouplesoncounter = 1
for line in text.splitlines():
    if (line!="") and (groupflag == False):
        groups_list = line.split(" ")
        groupflag = True
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
                                   "6": {},
                                   "7": {},
                                   "8": {},
                                   "9": {},

            }
    elif (line!="") and (thisgroupflag == True) and (line in teacher_list):
        output[thisgroupnumper][str(grouplesoncounter)]["teacher"] = line+","     
    elif (line!="") and (thisgroupflag == True):
        output[thisgroupnumper][str(grouplesoncounter)]["subject"] = line.replace("  ","")+","
        grouplesoncounter += 1
    
    print(repr(line))

print(groups_list)
print(output[2])
print(output[1])