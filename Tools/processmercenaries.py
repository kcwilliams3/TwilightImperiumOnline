# processmercenaries.py
#
# Reads the raw text data relating to mercenaries
# and saves it in the format that will be read by the game.
#
# Current output format options: .timercs (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processmercenaries.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import Merc
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".timercs"
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
		print("UsageError: processmercenaries.py <language=english || ...>")
		
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
	fileName = path + "mercenaries.txt"
	
	mercs = {}
	
	with open(fileName, 'r') as mercsFile:
		lineNumber = 1
		blocks = mercsFile.read().split("\n\n")
		for block in blocks:
			# Each block is an action
			lines = block.split("\n")
			text = ''
			for i in range(0,len(lines)):
				line = lines[i]
				if i == 0:
					#Merc name
					mercName = line.strip()
					assertNotEmpty(mercName, fileName, lineNumber)
					assertNotSeparator(mercName, fileName, lineNumber)
				elif i == 1:
					#Abilities
					abilities = line.strip().split(", ")
					for ability in abilities:
						assertNotEmpty(ability, fileName, lineNumber)
						assertNotSeparator(ability, fileName, lineNumber)
				elif i == 2 and len(line.split(',')) != 3:
					#Text
					text = line.strip()
					assertNotEmpty(text, fileName, lineNumber)
					assertNotSeparator(text, fileName, lineNumber)
				else:
					#Space Battle, Ground Battle, Movement
					spaceBattle, groundBattle, movement = line.strip().split(',')
					assertNotEmpty(spaceBattle, fileName, lineNumber)
					assertNotEmpty(groundBattle, fileName, lineNumber)
					assertNotEmpty(movement, fileName, lineNumber)	
					assertNotSeparator(spaceBattle, fileName, lineNumber)
					assertNotSeparator(groundBattle, fileName, lineNumber)
					assertNotSeparator(movement, fileName, lineNumber)						
				lineNumber += 1
			lineNumber += 1
			merc = Merc(mercName, abilities, text, spaceBattle, groundBattle, movement)
			mercs[merc.getName()] = merc
	return mercs

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

def writeName(merc, file):
	file.write(getStringAtCol(merc.getName()+lineEnd, firstValueCol, "Name: ", file))
	
def writeAbilities(merc, file):
	for ability in merc.getAbilities():
		if ability == "Sustain Damage":
			file.write(getStringAtCol("true"+lineEnd, firstValueCol, "Sustain Damage: ", file))
		else:
			ability, value = ability.strip(')').split(" (")
			file.write(getStringAtCol(value+lineEnd, firstValueCol, ability+": ", file))
	
def writeText(merc, file):
	file.write(getStringAtCol(merc.getText()+lineEnd, firstValueCol, "Text: ", file))
	
def writeSpaceBattle(merc, file):
	spaceBattleParts = merc.getSpaceBattle().strip().split('x')
	file.write(getStringAtCol(spaceBattleParts[0]+lineEnd, firstValueCol, "Space Battle: ", file))
	if len(spaceBattleParts) == 2:
		file.write(getStringAtCol(spaceBattleParts[1]+lineEnd, firstValueCol, "Space Shots: ", file))
		
def writeGroundBattle(merc, file):
	groundBattleParts = merc.getGroundBattle().strip().split('x')
	file.write(getStringAtCol(groundBattleParts[0]+lineEnd, firstValueCol, "Ground Battle: ", file))
	if len(groundBattleParts) == 2:
		file.write(getStringAtCol(groundBattleParts[1]+lineEnd, firstValueCol, "Ground Shots: ", file))
	
def writeMovement(merc, file):
	file.write(getStringAtCol(merc.getMovement()+lineEnd, firstValueCol, "Movement: ", file))
	
def writeOne(action, file):
	if outFormat == ".timercs":
		file.write(blockStart)
		writeName(action, file)
		writeAbilities(action, file)
		writeText(action, file)
		writeSpaceBattle(action, file)
		writeGroundBattle(action, file)
		writeMovement(action, file)
		file.write(blockEnd)
	#No other formats currently implemented
	
def writeAll(mercs, outFolder):
	#Takes in a dictionary of Merc objects and writes them to file
	
	#Make sure directories exist
	path = outFolder + sys.argv[1].lower().capitalize() + '/'
	if not os.path.exists(path):
		os.makedirs(path)
	
	outFileName = "mercenaries" + outFormat
	outFileLoc = path + outFileName
	tempFileName = "mercenariestemp.tmp"
	fileMode = 'w'
	first = True
	with open(tempFileName, fileMode) as outFile:
		for mercName in sorted(mercs):
			merc = mercs[mercName]
			writeOne(merc, outFile)
	if not os.path.exists("./Backups/"+path[3:]):
		os.makedirs("./Backups/"+path[3:])
	#Save old file as a backup
	if os.path.exists(path+outFileName):
		shutil.move(path+outFileName,"./Backups/"+path[3:]+outFileName+".bak")
	#Replace old file with temp file
	shutil.move("mercenariestemp.tmp",path+"/"+outFileName)
	
################################################################
#	Main program flow section
################################################################

def main():
	checkUsage()
	inFolder, outFolder = readFolders()
	mercs = readAll(inFolder)
	writeAll(mercs, outFolder)
	
main()