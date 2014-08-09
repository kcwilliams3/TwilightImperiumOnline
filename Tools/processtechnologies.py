# processtechnologies.py
#
# Reads the raw text data relating to non-racial technologies
# and saves it in the format that will be read by the game.
#
# Current output format options: .titechs (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processactioncards.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import Technology
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".titechs"
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
		print("UsageError: processtechnologies.py <language=english || ...>")
		
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
	fileName = path + "technologies.txt"
	
	techCards = {}
	
	with open(fileName, 'r') as techsFile:
		lineNumber = 3
		sections = techsFile.read().split(26*"-")
		sections = [section for section in sections if section] #Remove empty sections
		for section in sections:
			# Each section is a technology group (color)
			blocks = section.split("\n\n")
			blocks = [block for block in blocks if block] #Remove empty blocks
				
			sectionHeader = blocks[0]; # Designates which color the tech group is
			color = sectionHeader.split(' ')[0].strip()
			blocks.remove(blocks[0]);
			
			
			techName = '' 
			expansion = ''
			prerequisites = []
			text = ''
			for block in blocks:
				# Each block is a technology card
				
				lines = block.split("\n")
				lines = [line for line in lines if line] #Remove empty lines
				
				for i in range(0,len(lines)):
					line = lines[i]

					if i == 0:
						#Tech Name
						techName = line.strip()
						assertNotEmpty(techName, fileName, lineNumber)
						assertNotSeparator(techName, fileName, lineNumber)
					elif i == 1:
						#Expansion
						expansion = line.strip()
						assertNotEmpty(expansion, fileName, lineNumber)
						assertNotSeparator(expansion, fileName, lineNumber)
					elif i == 2:
						#Prerequisites
						line = line.strip().split(":")[1].strip()
						prerequisites = line.strip().split(" AND ")
						if len(prerequisites) > 1:
							prerequisites.insert(1,"AND");
						else:
							prerequisites = line.strip().split(" OR ")
							if len(prerequisites) > 1:
								prerequisites.insert(1,"OR");
						if "None" in prerequisites:
							prerequisites.remove("None")
						for element in prerequisites:
							assertNotEmpty(element, fileName, lineNumber)
							assertNotSeparator(element, fileName, lineNumber)
					else:
						#Text
						text = line.strip()
						assertNotEmpty(text, fileName, lineNumber)
						assertNotSeparator(text, fileName, lineNumber)	
					lineNumber += 1
				lineNumber += 1
				techCard = Technology(techName, color, expansion, prerequisites, text, False)
				techCards[techCard.getName()] = techCard
			lineNumber += 1
	return techCards

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

def writeName(tech, file):
	file.write(getStringAtCol(tech.getName()+lineEnd, firstValueCol, "Name: ", file))
	
def writeExpansion(tech, file):
	file.write(getStringAtCol(tech.getExpansion()+lineEnd, firstValueCol, "Expansion: ", file))
	
def writeColor(tech, file):
	file.write(getStringAtCol(tech.getColor()+lineEnd, firstValueCol, "Color: ", file))
	
def writePrerequisites(tech, file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Requires: ", file))
	for element in tech.getRequirements():
		file.write(getStringAtCol(element+lineEnd, secondValueCol, '', file))
	file.write(getStringAtCol(blockEnd, firstValueCol, '', file))
	
def writeText(tech, file):
	file.write(getStringAtCol(tech.getText()+lineEnd, firstValueCol, "Text: ", file))
	
def writeOne(tech, file):
	if outFormat == ".titechs":
		file.write(blockStart)
		writeName(tech, file)
		writeColor(tech, file)
		writeExpansion(tech, file)
		writePrerequisites(tech, file)
		writeText(tech, file)
		file.write(blockEnd)
	#No other formats currently implemented
	
def writeAll(techCards, outFolder):
	#Takes in a dictionary of Action objects and writes them to file
	
	#Make sure directories exist
	path = outFolder + sys.argv[1].lower().capitalize() + '/'
	if not os.path.exists(path):
		os.makedirs(path)
	
	outFileName = "technologies" + outFormat
	outFileLoc = path + outFileName
	tempFileName = "technologiestemp.tmp"
	fileMode = 'w'
	first = True
	with open(tempFileName, fileMode) as outFile:
		for techName in sorted(techCards):
			if not first:
				outFile.write("\n")
			else:
				first = False
			tech = techCards[techName]
			writeOne(tech, outFile)
	if not os.path.exists("./Backups/"+path[3:]):
		os.makedirs("./Backups/"+path[3:])
	#Save old file as a backup
	if os.path.exists(path+"/"+outFileName):
		shutil.move(path+"/"+outFileName,"./Backups/"+path[3:]+outFileName+".bak")
	#Replace old file with temp file
	shutil.move("technologiestemp.tmp",path+"/"+outFileName)
	
################################################################
#	Main program flow section
################################################################

def main():
	checkUsage()
	inFolder, outFolder = readFolders()
	techCards = readAll(inFolder)
	writeAll(techCards, outFolder)
	
main()