# processstrategycards.py
#
# Reads the raw text data relating to strategy cards
# and saves it in the format that will be read by the game.
#
# Current output format options: .tistrats (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processstrategycards.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import StrategyCard
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".tistrats"
tabSize = 4
firstValueCol = 5
secondValueCol = firstValueCol + 2
thirdValueCol = secondValueCol + 1
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
		print("UsageError: processstrategycards.py <language=english || ...>")
		
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
	fileName = path + "strategycards.txt"
	
	strats = {}
	
	with open(fileName, 'r') as stratsFile:
		lineNumber = 3
		sections = stratsFile.read().split("--------------------")
		sections = [section for section in sections if section] #Remove empty sections
		for section in sections:
			# Each section is a strategy card set
			blocks = section.split("\n\n")
			blocks = [block for block in blocks if block] #Remove empty blocks
				
			sectionHeader = blocks[0]; # Designates which set
			set = sectionHeader.strip()
			blocks.remove(blocks[0]);
			
			
			for block in blocks:
				# Each block is a strategy card
				
				specialText = primaryName = secondaryName = primaryText = secondaryText = ""
				
				lines = block.split("\n")
				lines = [line for line in lines if line] #Remove empty lines
				
				for i in range(0,len(lines)):
					line = lines[i]
					
					if i == 0:
						#Name
						initiative, name = line.strip().split(" : ")
						assertNotEmpty(initiative, fileName, lineNumber)
						assertNotSeparator(initiative, fileName, lineNumber)
						assertNotEmpty(name, fileName, lineNumber)
						assertNotSeparator(name, fileName, lineNumber)
					else:
						#Ability
						tag, abilityText = line.split(" : ")
						tagParts = tag.split(": ")
						if len(tagParts) == 2:
							abilityType = tagParts[0]	#"Primary Ability" or "Secondary Ability"
							abilityName = tagParts[1]	#"Diplomatic Envoy", etc.
						else:
							abilityType = tag.strip()	#"Special"
							abilityName = ""
						if abilityType == "Special":
							specialText = abilityText.strip()
						elif abilityType == "Primary Ability":
							primaryName = abilityName.strip()
							primaryText = abilityText.strip()
						elif abilityType == "Secondary Ability":
							secondaryName = abilityName.strip()
							secondaryText = abilityText.strip()
					lineNumber += 1
				lineNumber += 1
				strat = StrategyCard(name, initiative, set, primaryName, primaryText, secondaryName, secondaryText, specialText)
				strats[strat.getInitiative() + strat.getSet()] = strat
			lineNumber += 1
	return strats

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

def writeName(strat, file):
	file.write(getStringAtCol(strat.getName()+lineEnd, firstValueCol, "Name: ", file))
	
def writeInitiative(strat, file):
	file.write(getStringAtCol(strat.getInitiative()+lineEnd, firstValueCol, "Initiative: ", file))
	
def writeSet(strat, file):
	file.write(getStringAtCol(strat.getSet()+lineEnd, firstValueCol, "Set: ", file))
	
def writePrimaryAbility(strat, file):
	writeAbility("Primary Ability: ", strat.getPrimaryName(), strat.getPrimaryText(), file)
	
def writeSecondaryAbility(strat, file):
	writeAbility("Secondary Ability: ", strat.getSecondaryName(), strat.getSecondaryText(), file)
	
def writeAbility(tag, name, text, file):
	file.write(getStringAtCol(blockStart, firstValueCol, tag, file))
	pre = getStringAtCol("Name:", firstValueCol, "", file)
	file.write(getStringAtCol(name+lineEnd, secondValueCol, pre, file))
	pre = getStringAtCol("Text:", firstValueCol, "", file)
	file.write(getStringAtCol(text+lineEnd, secondValueCol, pre, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", file))
	
def writeSpecial(strat, file):
	file.write(getStringAtCol(strat.getSpecial()+lineEnd, firstValueCol, "Special: ", file))
	
def writeOne(strat, file):
	if outFormat == ".tistrats":
		file.write(blockStart)
		writeName(strat, file)
		writeInitiative(strat, file)
		writeSet(strat, file)
		writePrimaryAbility(strat, file)
		writeSecondaryAbility(strat, file)
		writeSpecial(strat, file)
		file.write(blockEnd)
	#No other formats currently implemented
	
def writeAll(strats, outFolder):
	#Takes in a dictionary of Action objects and writes them to file
	
	#Make sure directories exist
	path = outFolder + sys.argv[1].lower().capitalize() + '/'
	if not os.path.exists(path):
		os.makedirs(path)
	
	outFileName = "strategyCards" + outFormat
	outFileLoc = path + outFileName
	tempFileName = "strategyCardstemp.tmp"
	fileMode = 'w'
	first = True
	with open(tempFileName, fileMode) as outFile:
		for stratName in sorted(strats):
			strat = strats[stratName]
			writeOne(strat, outFile)
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
	strats = readAll(inFolder)
	writeAll(strats, outFolder)
	
main()