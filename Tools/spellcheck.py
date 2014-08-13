# spellcheck.py
#
# Spell checks a given file using a given dictionary.
# When a word is found that isn't in the dictionary, it allows the 
# user to (via the console) either add the word to the dictionary
# or correct the spelling & recheck. In the latter case, the spelling
# will be altered in the file being spell checked.
#
# Written by Kaleb Williams as part of Twilight Imperium project
#
# Usage: spellcheck.py <file> <dictionary (*.dict)>

import sys
import shutil
import os
from libs.exceptions import UsageError

#check for proper usage
try:
	if (len(sys.argv) != 3):
		raise UsageError
	extension2 = sys.argv[2].split(".")[-1]
	if (extension2 != "dict"):
		raise UsageError
except UsageError as e:
	print("UsageError: spellcheck.py <file> <dictionary (*.dict)>")

filename = os.path.basename(sys.argv[1])
filepath = os.path.dirname(sys.argv[1])
dictname = sys.argv[2]
	
#load in the existing dictionary
dict = []
#	open the dictionary file, if it exists
if os.path.isfile(dictname):
	with open(dictname, 'r') as d:
		#store each line as an entry in the list data structure
		for line in d:
			dict.append(line.strip())

#	now open the dictionary file for appending
with open(dictname, 'a+') as d:
	#check the file
	#	Open file for reading
	with open(filepath+"/"+filename, 'r+') as f:
		#Open temporary file that will act as an updated version of the old file
		with open("temp.tmp", 'w') as o:
			#compare each token with list
			lineNumber = 1
			for line in f:
				writeline = line
				tokens = line.replace("-"," ").replace(":"," ").replace("/"," ").replace(","," ").replace("."," ").replace("<"," ").replace(">"," ").replace("%"," ").split()
				for token in tokens:
					token = token.strip()
					token = token.rstrip(",.:;!\"'?+-")
					token = token.lstrip("[(\"'+-")
					if token.endswith("(s)"):
						token = token[:-3]
					token = token.rstrip(")].")
					oldtoken = token
					#skip token if it's numeric or only consisted of punctuation (is now empty)
					if token.isdigit() or token == "":
						continue
					remaining = ""
					while token.lower() not in dict:
						if token.replace("x","").isdigit():
							#token is of the form NUMBERxNUMBER, so it should be skipped
							break
						print("\n",writeline)
						if " " in token:
							#happens if a correction involved a split
							token,split,remaining = token.partition(" ")
						if token in dict:
							token = remaining
							continue
						response = input("Line {0}: \"{1}\" not found in dictionary. Add to dictionary? (Y/n) ".format(lineNumber,token.lower()))
						if (response.lower()=="y"):
							#add token to list and dict file
							dict.append(token.lower())
							d.write(token.lower()+"\n")
						elif (response.lower()=="abort"):
							sys.exit()
						else:
							token = input("Correction: ")
						if remaining:
							#happens if a correction involved a split
							token = remaining
						#token has now been accepted (possibly with corrections). If it has been modified, update the line to be written
						if (oldtoken != token):
							writeline = writeline.replace(oldtoken,token)
				#Write the corrected line to temp file
				o.write(writeline)
				lineNumber +=1

#Save old file as a backup
if not os.path.exists("./Backups/"):
	os.makedirs("./Backups/")
shutil.move(filepath+"/"+filename,"./Backups/"+filename+".bak")

#Replace old file with temp file
shutil.move("temp.tmp",filepath+"/"+filename)