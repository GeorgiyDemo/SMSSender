from tika import parser
import re
raw = parser.from_file("sample.pdf")
text = raw["content"]

groups_list = []
output = {}

groupflag = False
thisgroupflag = False
thisgroupnumper = 0

for line in text.splitlines():
    if (line!="") and (groupflag == False):
        groups_list = line.split(" ")
        groupflag = True
    elif (line.isdigit() == True):
        thisgroupnumper = int(line)
        thisgroupflag = True
        output[thisgroupnumper] = ""
    elif (line!="") and (thisgroupflag == True):
        output[thisgroupnumper] += line

print(groups_list)
print(output[1])