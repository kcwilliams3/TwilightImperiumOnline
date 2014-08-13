# processsystemdata.py
#
# Reads the raw text data relating to systems and planets
# and saves it in the format that will be read by the game.
#
# Current output format options: .tisysts (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processsystemdata.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import System,Planet
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".tisysts"
multiFileOut = True
tabSize = 4
firstValueCol = 0
secondValueCol = firstValueCol + 2
thirdValueCol = secondValueCol + 1
fourthValueCol = thirdValueCol + 5
fifthValueCol = fourthValueCol + 1
sixthValueCol = fifthValueCol + 1
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
		print("UsageError: processsystemdata.py <language=english || ...>")

################################################################
#	File read section
################################################################

def readFolders():
	#Reads in the locations of the In and Out folders

	with open("folders.txt", 'r') as foldersFile:
		lines = foldersFile.read().split("\n")
		return lines[0].strip(),lines[1].strip()
			
def readSystems(fileName, systems):
	with open(fileName, 'r') as systemsFile:
		lineNumber = 1
		sections = systemsFile.read().split("--------------------\n")
		for section in sections:
			if section != "":
				blocks = section.split("\n\n")
				for block in blocks:
					lines = block.split("\n")
					if block == blocks[0]:
						#System type/shape
						assertNumLines(lines, 1, fileName, lineNumber)
						line = lines[0]
						assertNotEmpty(line, fileName, lineNumber)
						assertNotSeparator(line, fileName, lineNumber)
						type = line.split(' ')[0]
						lineNumber += 1
					else:
						#System
						name = ""
						planets = []
						for line in lines:
							line = line.strip()
							if line == "":
								continue
							if line[-1] == ':':
								#System name
								assertNotEmpty(line, fileName, lineNumber)
								assertNotSeparator(line, fileName, lineNumber)
								name = line[:-1]
							elif (line == lines[0] and name == "") or (line == lines[1] and name != ""):
								#Expansion
								expansion = line
								assertValidExpansion(expansion, fileName, lineNumber)
							else:
								#Planet
								assertNotEmpty(line, fileName, lineNumber)
								assertNotSeparator(line, fileName, lineNumber)
								if line != "(None)":
									planets.append(line)
							lineNumber+=1
						system = System(name, type, expansion)
						id = system.getName()
						for planet in planets:
							system.addPlanet(planet)
							id += " " + planet
						id += expansion + type
						systems[id] = system
					if block != "":
						lineNumber+=1
			

def validateWormholes(systems):
	for system in systems.values():
		system.addWormholeDetails()
						
def readPlanets(fileName):
	with open(fileName, 'r') as planetsFile:
		lineNumber = 1
		blocks = planetsFile.read().split("\n\n")
		planets = []
		for block in blocks:
			lines = block.split("\n")
			extras = []
			for line in lines:
				line = line.strip()
				part1 = line.split(' ')[0]
				if part1 == "Green" or part1 == "Red" or part1 == "Yellow" or part1 == "Blue" or part1 == "Refresh":
					#Extras (refresh abilities, tech specialties, etc.)
					extras = line.strip().split(", ")
					for extra in extras:
						assertNotEmpty(extra, fileName, lineNumber)
						assertNotSeparator(extra, fileName, lineNumber)
				elif line == lines[0]:
					#Planet name
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					planetName = line.strip()
				elif (len(lines) > 1 and line == lines[1]):
					#Expansion
					assertValidExpansion(line, fileName, lineNumber)
					expansion = line.strip()
				elif (len(lines) > 2 and line == lines[2]):
					#Text
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					text = line.strip()
				elif (len(lines) > 3 and line == lines[3]):
					#Resources, influence
					resources, influence = line.split(',')
					assertNumber(resources, fileName, lineNumber)
					assertNumber(influence, fileName, lineNumber)
					
				lineNumber+=1
			planets.append(Planet(planetName, text, resources, influence, extras))
			lineNumber+=1
	return planets
				
def readAll(inFolder):
	#Reads in the data from the input files and returns a
	# dict of Race objects
	
	#Set file names
	path = inFolder + sys.argv[1].lower().capitalize() + "/"
	systemsFileName = path + "systems.txt"
	planetsFileName = path + "planets.txt"
	
	systems = {}
	
	readSystems(systemsFileName, systems)
	validateWormholes(systems)
	planets = readPlanets(planetsFileName)
	for system in systems.values():
		for planet in planets:
			system.addPlanetDetails(planet.getName(), planet.getFlavorText(), planet.getResources(), planet.getInfluence(), planet.getExtras())
	
	return systems

################################################################
#	File write section
################################################################

def getTabLevel(occupied):
	#returns the current indentation level
	return math.floor(occupied / tabSize)

def getSpaceBeforeCol(col,preString,race,file):
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
	
def getStringAtCol(string,col,preString,race,file):
	return preString + getSpaceBeforeCol(col, preString,race,file) + string
	
def writeOne(system,file):
	#Takes in a single system object and writes it to the given output file location
	file.write(blockStart)
	#Name
	file.write(getStringAtCol(system.getName()+lineEnd, secondValueCol, "Name: ", system, file))
	#Expansion
	file.write(getStringAtCol(system.getExpansion()+lineEnd, secondValueCol, "Expansion: ", system, file))
	#Type
	file.write(getStringAtCol(system.getType()+lineEnd, secondValueCol, "Type: ", system, file))
	#Planets
	file.write(getStringAtCol(blockStart, secondValueCol, "Planets: ", system, file))
	for planet in system.getPlanets():
		file.write(getStringAtCol(blockStart, thirdValueCol, "", system, file))
		#Name
		tag = getStringAtCol("Name:", thirdValueCol, "", system, file)
		file.write(getStringAtCol(planet.getName()+lineEnd, fourthValueCol, tag, system, file))
		#Text
		tag = getStringAtCol("Text:", thirdValueCol, "", system, file)
		file.write(getStringAtCol(planet.getFlavorText()+lineEnd, fourthValueCol, tag, system, file))
		#Resources
		tag = getStringAtCol("Resources:", thirdValueCol, "", system, file)
		file.write(getStringAtCol(planet.getResources()+lineEnd, fourthValueCol, tag, system, file))
		#Influence
		tag = getStringAtCol("Influence:", thirdValueCol, "", system, file)
		file.write(getStringAtCol(planet.getInfluence()+lineEnd, fourthValueCol, tag, system, file))
		#Extras
		refresh = ''
		techSpecialties = []
		for extra in planet.getExtras():
			extraParts = extra.split(' ')
			if extraParts[0] == "Refresh":
				refresh = extra.split(": ")[1]
			elif extraParts[2] == "Specialty":
				techSpecialties.append(extraParts[0])
		tag = getStringAtCol("Refresh Ability:", thirdValueCol, "", system, file)
		file.write(getStringAtCol(refresh+lineEnd, fourthValueCol, tag, system, file))
		tag = getStringAtCol("Tech Specialties:", thirdValueCol, "", system, file)
		file.write(getStringAtCol(blockStart, fourthValueCol, tag, system, file))
		for spec in techSpecialties:
			file.write(getStringAtCol(spec+lineEnd, fifthValueCol, "", system, file))
		file.write(getStringAtCol(blockEnd, fourthValueCol, "", system, file))
		file.write(getStringAtCol(blockEnd, thirdValueCol, "", system, file))
	file.write(getStringAtCol(blockEnd, secondValueCol, "", system, file))
	file.write(blockEnd)
	#No other formats currently implemented

def writeAll(systems, outFolder):
	#Takes in a dict of System objects and writes them to file(s)
	
	#Make sure directories exist
	outPath = path = outFolder + sys.argv[1].lower().capitalize() + "/"
	if not os.path.exists(path):
		os.makedirs(path)

	outFileName = "systems" + outFormat
	outFileLoc = path + outFileName
	tempFileName = "systemstemp.tmp"
	fileMode = 'w'
	with open(tempFileName, fileMode) as outFile:
		for systemName in sorted(systems):
			system = systems[systemName]
			writeOne(system, outFile)
	if not os.path.exists("./Backups/"+path[3:]):
		os.makedirs("./Backups/"+path[3:])
	#Save old file as a backup
	if os.path.exists(path+"/"+outFileName):
		shutil.move(path+"/"+outFileName,"./Backups/"+path[3:]+outFileName+".bak")
	#Replace old file with temp file
	shutil.move("systemstemp.tmp",path+"/"+outFileName)
################################################################
#	Main program flow section
################################################################
			
def main():
	checkUsage()
	inFolder, outFolder = readFolders()
	systems = readAll(inFolder)
	writeAll(systems, outFolder)

main()