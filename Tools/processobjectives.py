# processobjectives.py
#
# Reads the raw text data relating to objectives
# and saves it in the format that will be read by the game.
#
# Current output format options: .tiobjs (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processobjectives.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import Objective
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".tiobjs"
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
		print("UsageError: processobjectives.py <language=english || ...>")
		
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
	# dictionary of Objective objects
	
	#Set file name
	path = inFolder + sys.argv[1].lower().capitalize() + '/'
	fileName = path + "objectives.txt"
	
	objectives = {}
	
	with open(fileName, 'r') as objectivesFile:
		lineNumber = 3
		sections = objectivesFile.read().split(26*"-")
		sections = [section for section in sections if section] #Remove empty sections
		for section in sections:
			# Each section is an objective type
			blocks = section.split("\n\n")
			blocks = [block for block in blocks if block] #Remove empty blocks
				
			sectionHeader = blocks[0]; # Designates which objective type the group is
			typeParts = sectionHeader.split(', ')
			type = typeParts[0].split(' ')[0].strip()
			if len(typeParts) > 1:
				type += " " + typeParts[1]
			blocks.remove(blocks[0]);
			
			
			objName = '' 
			expansions = []
			text = ''
			reward = ''
			for block in blocks:
				# Each block is an objective card
				
				lines = block.split("\n")
				lines = [line for line in lines if line] #Remove empty lines
				
				for i in range(0,len(lines)):
					line = lines[i]

					if i == 0 and len(lines) == 4:
						#Objective Name
						objName = line.strip()
						assertNotEmpty(objName, fileName, lineNumber)
						assertNotSeparator(objName, fileName, lineNumber)
					elif (i == 1 and len(lines) == 4) or (i == 0 and len(lines) == 3):
						#Text
						text = line.strip()
						assertNotEmpty(text, fileName, lineNumber)
						assertNotSeparator(text, fileName, lineNumber)
					elif (i == 2 and len(lines) == 4) or (i == 1 and len(lines) == 3):
						#Expansions
						expansions = line.strip().split(", ")
						for expansion in expansions:
							assertNotEmpty(expansion, fileName, lineNumber)
							assertNotSeparator(expansion, fileName, lineNumber)
					else:
						#Reward
						reward = line.strip()
						assertNotEmpty(reward, fileName, lineNumber)
						assertNotSeparator(reward, fileName, lineNumber)	
					lineNumber += 1
				lineNumber += 1
				objective = Objective(objName, expansions, type, text, reward)
				objectives[objective.getType() + objective.getExpansions()[0] + objective.getName() + objective.getText()] = objective
			lineNumber += 1
	return objectives

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

def writeName(obj, file):
	file.write(getStringAtCol(obj.getName()+lineEnd, firstValueCol, "Name: ", file))
	
def writeExpansions(obj, file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Expansions: ", file))
	for expansion in obj.getExpansions():
		file.write(getStringAtCol(expansion+lineEnd, secondValueCol, '', file))
	file.write(getStringAtCol(blockEnd, firstValueCol, '', file))
	
def writeType(obj, file):
	file.write(getStringAtCol(obj.getType()+lineEnd, firstValueCol, "Type: ", file))
	
def writeReward(obj, file):
	file.write(getStringAtCol(obj.getReward()+lineEnd, firstValueCol, "Reward: ", file))
	
def writeText(obj, file):
	file.write(getStringAtCol(obj.getText()+lineEnd, firstValueCol, "Text: ", file))
	
def writeOne(obj, file):
	if outFormat == ".tiobjs":
		file.write(blockStart)
		writeName(obj, file)
		writeType(obj, file)
		writeExpansions(obj, file)
		writeText(obj, file)
		writeReward(obj, file)
		file.write(blockEnd)
	#No other formats currently implemented
	
def writeAll(objectives, outFolder):
	#Takes in a dictionary of Action objects and writes them to file
	
	#Make sure directories exist
	path = outFolder + sys.argv[1].lower().capitalize() + '/'
	if not os.path.exists(path):
		os.makedirs(path)
	
	outFileName = "objectives" + outFormat
	outFileLoc = path + outFileName
	tempFileName = "objectivestemp.tmp"
	fileMode = 'w'
	first = True
	with open(tempFileName, fileMode) as outFile:
		for objTNameText in sorted(objectives):
			obj = objectives[objTNameText]
			writeOne(obj, outFile)
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
	objectives = readAll(inFolder)
	writeAll(objectives, outFolder)
	
main()