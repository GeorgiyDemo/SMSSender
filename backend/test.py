output = {}
output[0] = {"1": {"teacher" : "КОТ КОТ", "projects" : "MEOW MEOW" },
             "2" : {"teacher" : "КОТ КОТ", "projects" : "MEOW MEOW" }
            }
output[1] = {"teacher" : "КОТ", "projects" : "MEOW" }

output[0]["1"]["teacher"] = "СОБАКАА"
print(output[0])

from tika import parser
import re
raw = parser.from_file("sample.pdf")
text = raw["content"]
print(text)