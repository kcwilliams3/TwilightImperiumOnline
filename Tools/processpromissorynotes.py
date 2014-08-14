# processpromissorynotes.py
#
# Reads the raw text data relating to promissory notes
# and saves it in the format that will be read by the game.
#
# Current output format options: .tiproms (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processpromissorynotes.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import PromissoryNote
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".tiproms"
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
		print("UsageError: processpromissorynotes.py <language=english || ...>")
		
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
	# dictionary of PromissoryNote objects
	
	#Set file name
	path = inFolder + sys.argv[1].lower().capitalize() + '/'
	fileName = path + "promissorynotes.txt"
	
	promissoryNotes = {}
	with open(fileName, 'r') as promissoryNotesFile:
		lineNumber = 1
		blocks = promissoryNotesFile.read().split("\n\n")
		for block in blocks:
			# Each block is a promissory note
			lines = block.split("\n")
			play = ""
			for i in range(0,len(lines)):
				line = lines[i]
				if i == 0:
					#Promissory note name
					promNoteName = line.strip()
					assertNotEmpty(promNoteName, fileName, lineNumber)
					assertNotSeparator(promNoteName, fileName, lineNumber)
				elif i == 1:
					#Flavor text
					flavor = line.strip()
					assertNotEmpty(flavor, fileName, lineNumber)
					assertNotSeparator(flavor, fileName, lineNumber)
				elif len(lines) == 4 and i == 2:
					#Play text
					play = line.strip()
					assertNotEmpty(play, fileName, lineNumber)
					assertNotSeparator(play, fileName, lineNumber)	
				else:
					#Rule text
					rule = line.strip()
					assertNotEmpty(rule, fileName, lineNumber)
					assertNotSeparator(rule, fileName, lineNumber)
				lineNumber += 1
			lineNumber += 1
			promissoryNote = PromissoryNote(promNoteName, flavor, play, rule)
			promissoryNotes[promissoryNote.getName()] = promissoryNote
	return promissoryNotes

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

def writeName(promissoryNote, file):
	file.write(getStringAtCol(promissoryNote.getName()+lineEnd, firstValueCol, "Name: ", file))
	
def writeFlavor(promissoryNote, file):
	file.write(getStringAtCol(promissoryNote.getFlavorText()+lineEnd, firstValueCol, "Flavor Text: ", file))
	
def writePlayText(promissoryNote, file):
	text = promissoryNote.getPlayText().rstrip(':')
	if ((len(text) > 4) and (text[0:4] == "Play")):
		text = text[5:]
		text = text[0].upper() + text[1:]
	if text != "":
		text += "."
	file.write(getStringAtCol(text+lineEnd, firstValueCol, "Play Text: ", file))

def writeRuleText(promissoryNote, file):
	file.write(getStringAtCol(promissoryNote.getRuleText()+lineEnd, firstValueCol, "Rule Text: ", file))
	
def writeOne(promissoryNote, file):
	if outFormat == ".tiproms":
		file.write(blockStart)
		writeName(promissoryNote, file)
		writeFlavor(promissoryNote, file)
		writePlayText(promissoryNote, file)
		writeRuleText(promissoryNote, file)
		file.write(blockEnd)
	#No other formats currently implemented
	
def writeAll(promissoryNotes, outFolder):
	#Takes in a dictionary of PoliticalCard objects and writes them to file
	
	#Make sure directories exist
	path = outFolder + sys.argv[1].lower().capitalize() + '/'
	if not os.path.exists(path):
		os.makedirs(path)
	
	outFileName = "promissoryNotes" + outFormat
	outFileLoc = path + outFileName
	tempFileName = "promissoryNotestemp.tmp"
	fileMode = 'w'
	with open(tempFileName, fileMode) as outFile:
		for promissoryNoteName in sorted(promissoryNotes):
			promissoryNote = promissoryNotes[promissoryNoteName]
			writeOne(promissoryNote, outFile)
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
	promissoryNotes = readAll(inFolder)
	writeAll(promissoryNotes, outFolder)
	
main()