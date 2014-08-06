# processactioncards.py
#
# Reads the raw text data relating to action cards
# and saves it in the format that will be read by the game.
#
# Current output format options: .tiacts (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium project
#
# Usage: processactioncards.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import Action
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".tiacts"
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
		print("UsageError: processactioncards.py <language=english || ...>")
		
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
	# dictionary of Action objects
	
	#Set file name
	path = inFolder + sys.argv[1].lower().capitalize() + '/'
	fileName = path + "actioncards.txt"
	
	actionCards = {}
	
	with open(fileName, 'r') as actionCardsFile:
		lineNumber = 1
		blocks = actionCardsFile.read().split("\n\n")
		for block in blocks:
			# Each block is an action
			lines = block.split("\n")
			play = '' #Initialize Play so we can safely check it later
			textA = []
			textB = []
			for i in range(0,len(lines)):
				line = lines[i]
				if i == 0:
					#Action name : quantity
					actionName, quantity = line.strip().split(" : ")
					assertNotEmpty(actionName, fileName, lineNumber)
					assertNotSeparator(actionName, fileName, lineNumber)
					assertNumber(quantity, fileName, lineNumber)
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
				elif line[0:5] == "Play:":
					#Play: statement
					play = line[5:].strip()
					assertNotEmpty(play, fileName, lineNumber)
					assertNotSeparator(play, fileName, lineNumber)	
				elif play == '':
					entry = line.strip()
					assertNotEmpty(entry, fileName, lineNumber)
					assertNotSeparator(entry, fileName, lineNumber)	
					textA.append(entry)
				else:
					#Text below Play: statement
					entry = line.strip()
					assertNotEmpty(entry, fileName, lineNumber)
					assertNotSeparator(entry, fileName, lineNumber)	
					textB.append(entry)
				lineNumber += 1
			lineNumber += 1
			actionCard = Action(actionName, expansion, flavor, textA, play, textB, quantity)
			actionCards[actionCard.getName()] = actionCard
	return actionCards

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

def writeName(action, file):
	file.write(getStringAtCol(action.getName()+lineEnd, firstValueCol, "Name: ", file))
	
def writeExpansion(action, file):
	file.write(getStringAtCol(action.getExpansion()+lineEnd, firstValueCol, "Expansion: ", file))
	
def writeFlavor(action, file):
	file.write(getStringAtCol(action.getFlavor()+lineEnd, firstValueCol, "Flavor Text: ", file))
	
def writeTextA(action, file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Rule Text A: ", file))
	for block in action.getTextA():
		file.write(getStringAtCol(block+lineEnd, secondValueCol, '', file))
	file.write(getStringAtCol(blockEnd, firstValueCol, '', file))
	
def writePlay(action, file):
	file.write(getStringAtCol(action.getPlay()+lineEnd, firstValueCol, "Play Text: ", file))
	
def writeTextB(action, file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Rule Text B: ", file))
	for block in action.getTextB():
		file.write(getStringAtCol(block+lineEnd, secondValueCol, '', file))
	file.write(getStringAtCol(blockEnd, firstValueCol, '', file))
	
def writeQuantity(action, file):
	file.write(getStringAtCol(action.getQuantity()+lineEnd, firstValueCol, "Quantity: ", file))
	
def writeOne(action, file):
	if outFormat == ".tiacts":
		file.write(blockStart)
		writeName(action, file)
		writeQuantity(action, file)
		writeExpansion(action, file)
		writeFlavor(action, file)
		writeTextA(action, file)
		writePlay(action, file)
		writeTextB(action, file)
		file.write(blockEnd)
	#No other formats currently implemented
	
def writeAll(actionCards, outFolder):
	#Takes in a dictionary of Action objects and writes them to file
	
	#Make sure directories exist
	path = outFolder + sys.argv[1].lower().capitalize() + '/'
	if not os.path.exists(path):
		os.makedirs(path)
	
	outFileName = "actions" + outFormat
	outFileLoc = path + outFileName
	tempFileName = "actionstemp.tmp"
	fileMode = 'w'
	first = True
	with open(tempFileName, fileMode) as outFile:
		for actionName in sorted(actionCards):
			if not first:
				outFile.write("\n")
			else:
				first = False
			action = actionCards[actionName]
			writeOne(action, outFile)
	if not os.path.exists("./Backups/"+path[3:]):
		os.makedirs("./Backups/"+path[3:])
	#Save old file as a backup
	if os.path.exists(path+"/"+outFileName):
		shutil.move(path+"/"+outFileName,"./Backups/"+path[3:]+outFileName+".bak")
	#Replace old file with temp file
	shutil.move("actionstemp.tmp",path+"/"+outFileName)
	
################################################################
#	Main program flow section
################################################################

def main():
	checkUsage()
	inFolder, outFolder = readFolders()
	actionCards = readAll(inFolder)
	writeAll(actionCards, outFolder)
	
main()