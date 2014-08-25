# processdomaincounters.py
#
# Reads the raw text data relating to domain counters
# and saves it in the format that will be read by the game.
#
# Current output format options: .tidomain (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processdomaincounters.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import Domain
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".tidomain"
multiFileOut = True
tabSize = 4
firstValueCol = 4
secondValueCol = firstValueCol + 2
thirdValueCol = secondValueCol + 3
fourthValueCol = thirdValueCol + 3
fifthValueCol = fourthValueCol + 3
lineEnd = "<;>\n"
blockStart = "<{>\n"
blockEnd = "<}>\n"

def checkUsage():
	#check for proper usage
	try:
		if (len(sys.argv) != 2):
			raise UsageError
		if sys.argv[1] not in acceptedLanguages:
			raise UsageError
	except UsageError as e:
		print("UsageError: processracedata.py <language=english || ...>")

################################################################
#	File read section
################################################################

def readFolders():
	#Reads in the locations of the In and Out folders

	with open("folders.txt", 'r') as foldersFile:
		lines = foldersFile.read().split("\n")
		return lines[0].strip(),lines[1].strip()

def readDomainCounters(fileName, domainCounters):
	with open(fileName, 'r') as domainsFile:
		lineNumber = 1
		blocks = domainsFile.read().split("\n\n")
		for block in blocks:
			expansion = ""
			option = ""
			lines = block.split("\n")
			assertNumLines(lines, 3, fileName, lineNumber)
			assertNotEmpty(lines[0], fileName, lineNumber)
			assertNotSeparator(lines[0], fileName, lineNumber)
			line0parts = lines[0].strip().split("::")
			name = line0parts[0]
			quantities = []
			qualifiers = []
			try:
				float(line0parts[1])
				number = True
			except ValueError:
				number = False
			if (number):
				quantities.append(line0parts[1].strip())
				qualifiers.append("")
			else:
				line0parts = line0parts[1].strip().split(", ")
				for part in line0parts:
					qualifier, quantity = part.split(':')
					quantities.append(quantity.strip())
					qualifiers.append(qualifier.strip())
			lineNumber+=1
			assertNotEmpty(lines[1], fileName, lineNumber)
			assertNotSeparator(lines[1], fileName, lineNumber)
			tags = lines[1].strip().split(", ")
			for tag in tags:
				if tag == "Shattered Empire" or tag == "Shards of the Throne":
					expansion = tag.strip()
				else:
					option = tag.strip()
			lineNumber+=1
			assertNotEmpty(lines[2], fileName, lineNumber)
			assertNotSeparator(lines[2], fileName, lineNumber)
			text = lines[2].strip()
			for i in range(0,len(qualifiers)):
				domainCounters[name + qualifiers[i] + expansion] = Domain(name, qualifiers[i], quantities[i], expansion, option, text) 
			lineNumber+=2
				
def readAll(inFolder):
	#Reads in the data from the input files and returns a
	# dict of Race objects
	
	#Set file names
	path = inFolder + sys.argv[1].lower().capitalize() + "/"
	domainsFileName = path + "domain.txt"
	
	domainCounters = {}
	
	readDomainCounters(domainsFileName, domainCounters)
	
	return domainCounters

################################################################
#	File write section
################################################################

def getTabLevel(occupied):
	#returns the current indentation level
	return math.floor(occupied / tabSize)

def getSpaceBeforeCol(col,preString,domain,file):
	#Args:
	# col:			the column we want the resulting space to get to
	# preString:	the  string of "occupied" space before current checking point
	initialTabLevel = getTabLevel(len(preString.expandtabs(tabSize)))
	space = ""
	tabLevel = initialTabLevel
	while tabLevel <= col:
		tabLevel += 1
		space += '\t'
	return space
	
def getStringAtCol(string,col,preString,domain,file):
	return preString + getSpaceBeforeCol(col, preString,domain,file) + string
	
def writeName(domain,file):
	file.write(getStringAtCol(domain.getName()+lineEnd, firstValueCol, "Name: ", domain, file))

def writeQualifier(domain,file):
	file.write(getStringAtCol(domain.getQualifier()+lineEnd, firstValueCol, "Qualifier: ", domain, file))
	
def writeQuantity(domain,file):
	file.write(getStringAtCol(domain.getQuantity()+lineEnd, firstValueCol, "Quantity: ", domain, file))

def writeExpansion(domain,file):
	if domain.getExpansion() == "":
		expoString = "Vanilla";
	else:
		expoString = domain.getExpansion();
	file.write(getStringAtCol(expoString+lineEnd, firstValueCol, "Expansion: ", domain, file))
	
def writeOption(domain,file):
	file.write(getStringAtCol(domain.getOption()+lineEnd, firstValueCol, "Option: ", domain, file))
	
def writeText(domain,file):
	file.write(getStringAtCol(domain.getText()+lineEnd, firstValueCol, "Text: ", domain, file))
	
def writeOne(domain,file):
	#Takes in a single race object and writes it to the given output file location
	if outFormat == ".tidomain":
		file.write(blockStart)
		writeName(domain,file)
		writeQualifier(domain,file)
		writeQuantity(domain,file)
		writeExpansion(domain,file)
		writeOption(domain,file)
		writeText(domain,file)
		file.write(blockEnd)
	#No other formats currently implemented

def writeAll(domains, outFolder):
	#Takes in a dict of Domain objects and writes them to file(s)
	
	#Make sure directories exist
	outPath = path = outFolder + sys.argv[1].lower().capitalize() + '/'
	if not os.path.exists(path):
		os.makedirs(path)

	outFileName = "domainCounters" + outFormat
	outFileLoc = path + outFileName
	tempFileName = "domainCounters" + "temp.tmp"
	
	with open(tempFileName, 'a') as outFile:
		for domainName in sorted(domains):
			domain = domains[domainName]
			writeOne(domain,outFile)
			
	if not os.path.exists("./Backups/"+path[3:]):
		os.makedirs("./Backups/"+path[3:])
	#Save old file as a backup
	if os.path.exists(path+outFileName):
		shutil.move(path+outFileName,"./Backups/"+path[3:]+outFileName+".bak")
	#Replace old file with temp file
	shutil.move("domainCounterstemp.tmp",path+"/"+outFileName)

################################################################
#	Main program flow section
################################################################
			
def main():
	checkUsage()
	inFolder, outFolder = readFolders()
	domains = readAll(inFolder)
	writeAll(domains, outFolder)

main()