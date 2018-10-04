from tika import parser
import re
raw = parser.from_file("./PDF/5.pdf")
text = raw["content"]

for line in text.splitlines():
    print(line)