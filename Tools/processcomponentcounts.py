# processcomponentcounts.py
#
# Reads the raw text data relating to component quantities
# and saves it in the format that will be read by the game.
#
# Current output format options: .ticounts (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processcomponentcounts.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import Action
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".ticounts"
tabSize = 4
firstValueCol = 3
secondValueCol = firstValueCol + 3
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
		print("UsageError: processcomponentcounts.py <language=english || ...>")
		
################################################################
#	File read section
################################################################

def readFolders():
	#Reads in the locations of the In and Out folders

	with open("folders.txt", 'r') as foldersFile:
		lines = foldersFile.read().split("\n")
		return lines[0].strip(),lines[1].strip()

def readAll(inFolder):
	#Reads in the data from the input file and returns two
	# dictionaries of Component objects
	
	#Set file name
	path = inFolder + sys.argv[1].lower().capitalize() + '/'
	fileName = path + "componentcounts.txt"
	
	playerCounts = {}
	sharedCounts = {}
	
	with open(fileName, 'r') as componentCountsFile:
		lineNumber = 1
		sections = componentCountsFile.read().split("--------------------\n")
		for i in range(0,len(sections)):
			section = sections[i]
			if section == '':
				continue
			blocks = section.split("\n\n")
			vanillaCount = ''
			SECount = ''
			SotTCount = ''
			for j in range(0,len(blocks)):
				block = blocks[j]
				if j == 0:
					#Section header
					if block.strip == "Per-Player":
						#Per-Player Section
						dict = playerCounts
					elif block.strip == "Shared":
						#Shared Section
						dict = sharedCounts
					lineNumber += 1
				else:
					#Each non-header block is a component type
					lines = block.split("\n")
					for k in range(0,len(lines)):
						line = lines[k]
						if line == '' or line == "--------------------":
							continue
						if k == 0:
							#Name( : Qualifier)
							if " : " in line:
								name, qualifier = line.strip().split(" : ")
							else:
								name = line.strip()
								qualifier = ''
						else:
							#Quantity for one of the sets
							set, quantity = line.strip().split(" : ")
							assertValidExpansion(set, componentCountsFile, lineNumber)
							if set == "Vanilla":
								vanillaCount = quantity
							elif set == "Shattered Empire":
								SECount = quantity
							elif set == "Shards of the Throne":
								SotTCount = quantity
						lineNumber += 1
				lineNumber += 1
	
	return playerCounts, sharedCounts

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

# def writeName(action, file):
	# file.write(getStringAtCol(action.getName()+lineEnd, firstValueCol, "Name: ", file))
	
# def writeExpansion(action, file):
	# file.write(getStringAtCol(action.getExpansion()+lineEnd, firstValueCol, "Expansion: ", file))
	
# def writeFlavor(action, file):
	# file.write(getStringAtCol(action.getFlavor()+lineEnd, firstValueCol, "Flavor Text: ", file))
	
# def writeTextA(action, file):
	# file00.write(getStringAtCol(blockStart, firstValueCol, "Rule Text A: ", file))
	# for block in action.getTextA():
		# file.write(getStringAtCol(block+lineEnd, secondValueCol, '', file))
	# file.write(getStringAtCol(blockEnd, firstValueCol, '', file))
	
# def writePlay(action, file):
	# file.write(getStringAtCol(action.getPlay()+lineEnd, firstValueCol, "Play Text: ", file))
	
# def writeTextB(action, file):
	# file.write(getStringAtCol(blockStart, firstValueCol, "Rule Text B: ", file))
	# for block in action.getTextB():
		# file.write(getStringAtCol(block+lineEnd, secondValueCol, '', file))
	# file.write(getStringAtCol(blockEnd, firstValueCol, '', file))
	
# def writeQuantity(action, file):
	# file.write(getStringAtCol(action.getQuantity()+lineEnd, firstValueCol, "Quantity: ", file))
	
def writeOne(action, file):
	pass
	# if outFormat == ".tiacts":
		# file.write(blockStart)
		# writeName(action, file)
		# writeQuantity(action, file)
		# writeExpansion(action, file)
		# writeFlavor(action, file)
		# writeTextA(action, file)
		# writePlay(action, file)
		# writeTextB(action, file)
		# file.write(blockEnd)
	#No other formats currently implemented
	
def writeAll(actionCards, outFile):
	#Takes in a dictionary of Action objects and writes them to file
	
	#Make sure directories exist
	path = outFile + sys.argv[1].lower().capitalize() + '/'
	if not os.path.exists(path):
		os.makedirs(path)
	
	# outFileName = "actions" + outFormat
	# outFileLoc = path + outFileName
	# tempFileName = "actionstemp.tmp"
	# fileMode = 'w'
	# first = True
	# with open(tempFileName, fileMode) as outFile:
		# for actionName in sorted(actionCards):
			# if not first:
				# outFile.write("\n")
			# else:
				# first = False
			# action = actionCards[actionName]
			# writeOne(action, outFile)
	# if not os.path.exists("./Backups/"+path[3:]):
		# os.makedirs("./Backups/"+path[3:])
	# #Save old file as a backup
	# if os.path.exists(path+"/"+outFileName):
		# shutil.move(path+"/"+outFileName,"./Backups/"+path[3:]+outFileName+".bak")
	# #Replace old file with temp file
	# shutil.move("actionstemp.tmp",path+"/"+outFileName)
	
################################################################
#	Main program flow section
################################################################

def main():
	checkUsage()
	inFoler, outFolder = readFolders()
	playerCounts, sharedCounts = readAll(inFolder)
	# writeAll(actionCards)
	
main()