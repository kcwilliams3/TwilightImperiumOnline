# processpoliticalcards.py
#
# Reads the raw text data relating to political cards
# and saves it in the format that will be read by the game.
#
# Current output format options: .tipols (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processpoliticalcards.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import PoliticalCard
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".tipols"
tabSize = 4
firstValueCol = 3
secondValueCol = firstValueCol + 1
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
		print("UsageError: processpoliticalcards.py <language=english || ...>")
		
################################################################
#	File read section
################################################################

def readFolders():
	#Reads in the locations of the In and Out folders

	with open("folders.txt", 'r') as foldersFile:
		lines = foldersFile.read().split("\n")
		return lines[0].strip(),lines[1].strip()

def readAll(inFolder):
	#Reads in the data from the input file and returns a
	# dictionary of PoliticalCard objects
	
	#Set file name
	path = inFolder + sys.argv[1].lower().capitalize() + '/'
	fileName = path + "politicalcards.txt"
	
	politicalCards = {}
	with open(fileName, 'r') as politicalCardsFile:
		lineNumber = 1
		blocks = politicalCardsFile.read().split("\n\n")
		for block in blocks:
			# Each block is a political card
			lines = block.split("\n")
			for i in range(0,len(lines)):
				line = lines[i]
				if i == 0:
					#Political name    (LAW)?
					if line.strip()[-5:] == "(LAW)":
						law = True
						politicalName = line[:-6].strip()
					else:
						law = False
						politicalName = line.strip()
					assertNotEmpty(politicalName, fileName, lineNumber)
					assertNotSeparator(politicalName, fileName, lineNumber)
				elif i == 1:
					#Expansion
					expansion = line.strip()
					assertNotEmpty(expansion, fileName, lineNumber)
					assertNotSeparator(expansion, fileName, lineNumber)
				elif i == 2:
					#Flavor text
					flavor = line.strip()
					assertNotEmpty(flavor, fileName, lineNumber)
					assertNotSeparator(flavor, fileName, lineNumber)
					politicalCard = PoliticalCard(politicalName, law, expansion, flavor)
				else:
					#Rule text line
					ruleLine = line.strip()
					assertNotEmpty(ruleLine, fileName, lineNumber)
					assertNotSeparator(ruleLine, fileName, lineNumber)	
					politicalCard.addRuleText(ruleLine)
				lineNumber += 1
			lineNumber += 1
			politicalCards[politicalCard.getName() + politicalCard.getExpansion()] = politicalCard
	return politicalCards

################################################################
#	File write section
################################################################

def getTabLevel(occupied):
	#returns the current indentation level
	return math.floor(occupied / tabSize)

def getSpaceBeforeCol(col,preString,file):
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
	
def getStringAtCol(string,col,preString,file):
	return preString + getSpaceBeforeCol(col, preString,file) + string

def writeName(political, file):
	file.write(getStringAtCol(political.getName()+lineEnd, firstValueCol, "Name: ", file))

def writeLaw(political, file):
	if (political.getIsLaw()):
		lawString = "true"
	else:
		lawString = "false"
	file.write(getStringAtCol(lawString+lineEnd, firstValueCol, "Law: ", file))
	
def writeExpansion(political, file):
	file.write(getStringAtCol(political.getExpansion()+lineEnd, firstValueCol, "Expansion: ", file))
	
def writeFlavor(political, file):
	file.write(getStringAtCol(political.getFlavorText()+lineEnd, firstValueCol, "Flavor Text: ", file))
	
def writeRuleText(political, file):
	type = "For/Against"
	discard = ""
	for rule in political.getRuleText():
		if rule.lower().startswith("elect"):
			type = "Elect"
		if (rule.lower().startswith("discard") and "if" in rule.lower()) or (rule.lower().startswith("if") and len(rule.split(', ')) > 1 and rule.split(', ')[1].lower().startswith("discard")):
			discard = rule
	if discard != "":
		political.getRuleText().remove(discard)
	if type == "Elect":
		writeElectText(political, file)
	else:
		writeForAgainstText(political, file)
	if discard != "":
		writeDiscardText(discard, file)

def writeElectText(political, file):
	electLine, text = political.getRuleText()
	candidates = electLine[6:]
	file.write(getStringAtCol(candidates+lineEnd, firstValueCol, "Elect: ", file))
	file.write(getStringAtCol(text+lineEnd, firstValueCol, "Effect: ", file))
	
def writeForAgainstText(political, file):
	forText, againstText = political.getRuleText()
	file.write(getStringAtCol(forText[5:]+lineEnd, firstValueCol, "For: ", file))
	file.write(getStringAtCol(againstText[9:]+lineEnd, firstValueCol, "Against: ", file))
	
def writeDiscardText(discard, file):
	file.write(getStringAtCol(discard+lineEnd, firstValueCol, "Discard: ", file))
	
def writeOne(political, file):
	if outFormat == ".tipols":
		file.write(blockStart)
		writeName(political, file)
		writeLaw(political, file)
		writeExpansion(political, file)
		writeFlavor(political, file)
		writeRuleText(political, file)
		file.write(blockEnd)
	#No other formats currently implemented
	
def writeAll(politicalCards, outFolder):
	#Takes in a dictionary of PoliticalCard objects and writes them to file
	
	#Make sure directories exist
	path = outFolder + sys.argv[1].lower().capitalize() + '/'
	if not os.path.exists(path):
		os.makedirs(path)
	
	outFileName = "politicalCards" + outFormat
	outFileLoc = path + outFileName
	tempFileName = "politicalCardsstemp.tmp"
	fileMode = 'w'
	first = True
	with open(tempFileName, fileMode) as outFile:
		for politicalName in sorted(politicalCards):
			politicalCard = politicalCards[politicalName]
			writeOne(politicalCard, outFile)
	if not os.path.exists("./Backups/"+path[3:]):
		os.makedirs("./Backups/"+path[3:])
	#Save old file as a backup
	if os.path.exists(path+"/"+outFileName):
		shutil.move(path+"/"+outFileName,"./Backups/"+path[3:]+outFileName+".bak")
	#Replace old file with temp file
	shutil.move(tempFileName,path+"/"+outFileName)
	
################################################################
#	Main program flow section
################################################################

def main():
	checkUsage()
	inFolder, outFolder = readFolders()
	politicalCards = readAll(inFolder)
	writeAll(politicalCards, outFolder)
	
main()