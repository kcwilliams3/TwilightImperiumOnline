# processracedata.py
#
# Reads the raw text data relating to race-specific information
# and saves it in the format that will be read by the game.
#
# Current output format options: .tirace (basically just a .txt file with a specific format)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processracedata.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import Race,System,Technology,Planet,Flagship,Representative,Leader
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = ".tirace"
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

def readRaceNames(fileName, races):
	with open(fileName, 'r') as namesFile:
		lineNumber = 1
		blocks = namesFile.read().split("\n\n")
		for block in blocks:
			lines = block.split("\n")
			assertNumLines(lines, 3, fileName, lineNumber)
			assertNotEmpty(lines[0], fileName, lineNumber)
			assertNotSeparator(lines[0], fileName, lineNumber)
			race = Race()
			race.setNameFull(lines[0].strip())
			lineNumber+=1
			assertNotEmpty(lines[1], fileName, lineNumber)
			assertNotSeparator(lines[1], fileName, lineNumber)
			race.setNameShort(lines[1].strip())
			lineNumber+=1
			assertNotEmpty(lines[2], fileName, lineNumber)
			assertNotSeparator(lines[2], fileName, lineNumber)
			race.setNameSpecies(lines[2].strip())
			races[race.getNameFull()] = race
			lineNumber+=2
		
def readRaceSheets(fileName, races):
	with open(fileName, 'r') as namesFile:
		lineNumber = 1
		sections = namesFile.read().split("--------------------------\n")
		for section in sections:
			if section != "":
				blocks = section.split("\n\n")
				for block in blocks:
					lines = block.split("\n")
					if block == blocks[0]:
						#Race header
						assertNumLines(lines, 2, fileName, lineNumber)
						assertValidRace(lines[0], races, fileName, lineNumber)
						if lines[0].strip()[0:4] != "The ":
							name = "The " + lines[0].strip()
						else:
							name = lines[0].strip()
						race = races[name]
						lineNumber+=1
						assertNotEmpty(lines[1], fileName, lineNumber)
						assertNotSeparator(lines[1], fileName, lineNumber)
						race.setExpansion(lines[1].strip())
						lineNumber+=1
					elif block == blocks[1]:
						#Starting units
						assertString(lines[0], "Starting Units", fileName, lineNumber)
						for line in lines:
							if line != lines[0]:
								number,split,unit = line.partition(" ")
								assertNumber(number, fileName, lineNumber)
								assertUnitType(unit, fileName, lineNumber)
								race.addStartingUnit(unit.strip(), number.strip())
							lineNumber+=1
					elif block == blocks[2]:
						#Starting technology
						assertString(lines[0], "Starting Technology", fileName, lineNumber)
						for line in lines:
							if line != lines[0]:
								assertNotEmpty(line, fileName, lineNumber)
								assertNotSeparator(line, fileName, lineNumber)
								race.addStartingTech(line.strip())
							lineNumber+=1
					elif block == blocks[3]:
						#Special abilities
						assertString(lines[0], "Special Abilities", fileName, lineNumber)
						for line in lines:
							if line != lines[0]:
								assertNotEmpty(line, fileName, lineNumber)
								assertNotSeparator(line, fileName, lineNumber)
								race.addSpecialAbility(line.strip())
							lineNumber+=1
					elif block == blocks[4]:
						#History
						assertString(lines[0], "History", fileName, lineNumber)
						for line in lines:
							if line == "":
								continue
							if line != lines[0]:
								assertNotEmpty(line, fileName, lineNumber)
								assertNotSeparator(line, fileName, lineNumber)
								race.addHistoryParagraph(line.strip())
							lineNumber+=1
					if block != "":
						lineNumber+=1
			
def readHomeSystems(fileName, races):
	with open(fileName, 'r') as homeSystemsFile:
		lineNumber = 1
		sections = homeSystemsFile.read().split("--------------------\n")
		for section in sections:
			if section != "":
				blocks = section.split("\n\n")
				for block in blocks:
					lines = block.split("\n")
					if block == blocks[0]:
						#System type/shape
						assertNumLines(lines, 1, fileName, lineNumber)
						line = lines[0]
						assertSystemType(line.strip()[:-13], fileName, lineNumber)
						assertString(line.strip()[-12:], "Home Systems", fileName, lineNumber)
						type = line.strip()[:-13]
						lineNumber += 1
					else:
						#Race block
						for line in lines:
							if line == "":
								continue
							if line == lines[0]:
								#Race name
								assertNotEmpty(line, fileName, lineNumber)
								assertNotSeparator(line, fileName, lineNumber)
								if (line.strip() == "The Creuss Gate:"):
									raceName = "The Ghosts of Creuss"
									systemName = line.strip().rstrip(":")
								else:
									assertString(line.strip()[-12:], "Home System:", fileName, lineNumber)
									systemName = line.strip().rstrip(":")
									if line.strip()[0:4] != "The ":
										raceName = "The " + line.strip()[:-13]
									else:
										raceName = line.strip()[:-13]
								assertValidRace(line.strip(), races, fileName, lineNumber)
								race = races[raceName]
								system = System(systemName, type)
								race.addHomeSystem(system)
							else:
								#Planet
								assertNotEmpty(line, fileName, lineNumber)
								assertNotSeparator(line, fileName, lineNumber)
								system.addPlanet(line.strip())
							lineNumber+=1
					if block != "":
						lineNumber+=1

def validateWormholes(races):
	for race in races.values():
		for system in race.getHomeSystems():
			system.addWormholeDetails()
						
def readHomePlanets(fileName, races):
	with open(fileName, 'r') as homePlanetsFile:
		lineNumber = 1
		blocks = homePlanetsFile.read().split("\n\n")
		for block in blocks:
			lines = block.split("\n")
			for i in range(0,len(lines)):
				line = lines[i]
				if i==0:
					#Race name
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					if line.strip()[0:4] != "The ":
						raceName = "The " + line.strip()
					else:
						raceName = line.strip()
					assertValidRace(line.strip(), races, fileName, lineNumber)
					race = races[raceName]
					for system in race.getHomeSystems():
						if system.getName()[-11:] == "Home System":
							homeSystem = system
				elif i%3==1:
					#Planet name
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					planetName = line.strip()
				elif i%3==2:
					#Flavor text
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					flavorText = line.strip()
				elif i%3==0:
					#Resources, influence
					resources, influence = line.split(',')
					assertNumber(resources, fileName, lineNumber)
					assertNumber(influence, fileName, lineNumber)
					homeSystem.addPlanetDetails(planetName, flavorText, resources, influence)
				lineNumber+=1
			lineNumber+=1

def readTradeContracts(fileName, races):
	with open(fileName, 'r') as tradeContractsFile:
		lineNumber = 1
		lines = tradeContractsFile.read().split('\n')
		for line in lines:
			name, contracts = line.split(":")
			if line.strip()[0:4] != "The ":
				raceName = "The " + name.strip()
			else:
				raceName = name.strip()
			assertValidRace(raceName, races, fileName, lineNumber)
			c1, c2 = contracts.split(",")
			assertNumber(c1, fileName, lineNumber)
			assertNumber(c2, fileName, lineNumber)
			race = races[raceName]
			race.addTradeContract(c1.strip())
			race.addTradeContract(c2.strip())
			lineNumber += 1
			
def readRacialTechs(fileName, races):
	with open(fileName, 'r') as racialTechsFile:
		lineNumber = 1
		blocks = racialTechsFile.read().split("\n\n")
		for block in blocks:
			lines = block.split('\n')
			for i in range(0,len(lines)):
				line = lines[i]
				if i==0:
					#Race name
					if line.strip()[0:4] != "The ":
						raceName = "The " + line.strip()
					else:
						raceName = line.strip()
					assertValidRace(raceName, races, fileName, lineNumber)
					race = races[raceName]
				elif i%4==1:
					#Tech name
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					techName = line.strip()
				elif i%4==2:
					#Expansion
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					expansion = line.strip()
				elif i%4==3:
					#Cost
					tag, cost = line.strip().split(": ")
					assertString(tag, "Cost", fileName, lineNumber)
					if cost != "*":
						assertNumber(cost, fileName, lineNumber)
				elif i%4==0:
					#Text
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					text = line.strip()
					race.addRacialTech(Technology(techName, expansion, cost, text, True))
				lineNumber += 1
			lineNumber += 1

def readFlagships(fileName, races):
	with open(fileName, 'r') as flagshipsFile:
		lineNumber = 1
		blocks = flagshipsFile.read().split("\n\n")
		for block in blocks:
			lines = block.split('\n')
			for i in range(0,len(lines)):
				line = lines[i]
				if i==0:
					#Race name
					if line.strip()[0:4] != "The ":
						raceName = "The " + line.strip()
					else:
						raceName = line.strip()
					assertValidRace(raceName, races, fileName, lineNumber)
					race = races[raceName]
				elif i%7==1:
					#Tech name
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					flagshipName = line.strip()
				elif i%7==2:
					#Abilities
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					if ',' in line.strip():
						abilities = line.strip().split(",")
					else:
						abilities = [line.strip()]
				elif i%7==3:
					#Text
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					text = line.strip()
				elif i%7==4:
					#Cost
					assertString(line[:5], "Cost:", fileName, lineNumber)
					cost = line.lstrip("Cost: ").strip()
				elif i%7==5:
					#Battle
					assertString(line.strip()[:7], "Battle:", fileName, lineNumber)
					battleString = line.lstrip("Battle: ").strip()
					if 'x' in battleString:
						battle, multiplier = battleString.split('x')
					else:
						battle = battleString.strip()
						multiplier = 1
				elif i%7==6:
					#Move
					assertString(line.strip()[:5], "Move:", fileName, lineNumber)
					move = line.lstrip("Move: ").strip()
				elif i%7==0:
					#Capacity
					assertString(line.strip()[:9], "Capacity:", fileName, lineNumber)
					capacity = line.lstrip("Capacity: ").strip()
					races[raceName].setFlagship(Flagship(flagshipName, abilities, text, cost, battle, multiplier, move, capacity))
				lineNumber += 1
			lineNumber += 1
	
def readRepresentatives(fileName, races):
	with open(fileName, 'r') as repsFile:
		lineNumber = 1
		blocks = repsFile.read().split("\n\n")
		for block in blocks:
			lines = block.split('\n')
			for i in range(0,len(lines)):
				line = lines[i]
				if i==0:
					#Race name
					if line.strip()[0:4] != "The ":
						raceName = "The " + line.strip()
					else:
						raceName = line.strip()
					assertValidRace(raceName, races, fileName, lineNumber)
					race = races[raceName]
				elif i%4==1:
					#Rep name
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					repName = line.strip()
				elif i%4==2:
					#Votes
					assertString(line.strip()[:1], '+', fileName, lineNumber)
					votes = line.strip().lstrip('+')
					assertNumber(votes, fileName, lineNumber)
				elif i%4==3:
					#Types
					if ',' in line.strip():
						reps = line.strip().split(',')
					else:
						reps = [line.strip()]
					for rep in reps:
						assertValidRepType(rep, fileName, lineNumber)
				elif i%4==0:
					#Text
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					text = line.strip()
					race.addRepresentative(Representative(repName, votes, reps, text))
				lineNumber += 1
			lineNumber += 1
	
def readLeaders(fileName, races):
	with open(fileName, 'r') as leadersFile:
		lineNumber = 1
		blocks = leadersFile.read().split("\n\n")
		for block in blocks:
			lines = block.split('\n')
			for i in range(0,len(lines)):
				line = lines[i]
				if i==0:
					#Race name
					if line.strip()[0:4] != "The ":
						raceName = "The " + line.strip()
					else:
						raceName = line.strip()
					assertValidRace(raceName, races, fileName, lineNumber)
					race = races[raceName]
				else:
					#Leader
					name, type = line.split(", ")
					assertNotEmpty(name, fileName, lineNumber)
					assertNotSeparator(name, fileName, lineNumber)
					assertValidLeaderType(type, fileName, lineNumber)
					race.addLeader(Leader(name, type))
				lineNumber += 1
			lineNumber += 1
				
def readAll(inFolder):
	#Reads in the data from the input files and returns a
	# dict of Race objects
	
	#Set file names
	path = inFolder + sys.argv[1].lower().capitalize() + "/Race-Specific/"
	namesFileName = path + "racenames.txt"
	raceSheetsFileName = path + "racesheets.txt"
	homeSystemsFileName = path + "homesystems.txt"
	homePlanetsFileName = path + "homeplanets.txt"
	tradeFileName = path + "tradecontracts.txt"
	techsFileName = path + "racialtechs.txt"
	flagshipsFileName = path + "flagships.txt"
	repsFileName = path + "representatives.txt"
	leadersFileName = path + "leaders.txt"
	
	races = {}
	
	readRaceNames(namesFileName, races)
	readRaceSheets(raceSheetsFileName, races)
	readHomeSystems(homeSystemsFileName, races)
	validateWormholes(races)
	readHomePlanets(homePlanetsFileName, races)
	readTradeContracts(tradeFileName, races)
	readRacialTechs(techsFileName, races)
	readFlagships(flagshipsFileName, races)
	readRepresentatives(repsFileName, races)
	readLeaders(leadersFileName, races)
	# DEBUG:
	# for key in races.keys():
		# if key == "The Universities of Jol Nar":
			# print("Full Name:",races[key].getNameFull())
			# print("Short Name:",races[key].getNameShort())
			# print("Species Name:",races[key].getNameSpecies())
			# print("Expansion:",races[key].getExpansion())
			# print("Starting Units:",races[key].getStartingUnits())
			# print("Starting Techs:",races[key].getStartingTechs())
			# print("Special Abilities:",races[key].getSpecialAbilities())
			# print("History:",races[key].getHistory())
			# for system in races[key].getHomeSystems():
				# print(system)
				# for planet in system.getPlanets():
					# print("\tName:",planet.getName())
					# print("\tFlavor Text:",planet.getFlavorText())
					# print("\tResources:",planet.getResources())
					# print("\tInfluence:",planet.getInfluence())
			# print("Trade Contracts:",races[key].getTradeContracts())
			# print("Racial Techs:")
			# for tech in races[key].getRacialTechs():
				# print("\tName:",tech.getName())
				# print("\tExpansion:",tech.getExpansion())
				# print("\tCost:",tech.getCost())
				# print("\tText:",tech.getText())
			# print("Flagship:")
			# flagship = races[key].getFlagship()
			# print("\tName:",flagship.getName())
			# print("\tAbilities:",flagship.getAbilities())
			# print("\tText:",flagship.getText())
			# print("\tCost:",flagship.getCost())
			# print("\tBattle:",flagship.getBattle())
			# print("\tMultiplier:",flagship.getMultiplier())
			# print("\tMove:",flagship.getMove())
			# print("\tCapacity:",flagship.getCapacity())
			# print("Representatives:")
			# for rep in races[key].getRepresentatives():
				# print("\tName:",rep.getName())
				# print("\tVotes:",rep.getVotes())
				# print("\tTypes:",rep.getTypes())
				# print("\tText:",rep.getText())
			# print("Leaders:")
			# for leader in races[key].getLeaders():
				# print("\tName:",leader.getName())
				# print("\tType:",leader.getType())
	return races

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
	
def writeFullName(race,file):
	file.write(getStringAtCol(race.getNameFull()+lineEnd, firstValueCol, "Full Name: ", race, file))

def writeShortName(race,file):
	file.write(getStringAtCol(race.getNameShort()+lineEnd, firstValueCol, "Short Name: ", race, file))
	
def writeSpeciesName(race,file):
	file.write(getStringAtCol(race.getNameSpecies()+lineEnd, firstValueCol, "Species Name: ", race, file))

def writeExpansion(race,file):
	file.write(getStringAtCol(race.getExpansion()+lineEnd, firstValueCol, "Expansion: ", race, file))
	
def writeHistory(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "History:", race, file))
	for paragraph in race.getHistory():
		file.write(getStringAtCol(paragraph+lineEnd, firstValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))
	
def writeSpecialAbilities(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Special Abilities:", race, file))
	for ability in race.getSpecialAbilities():
		file.write(getStringAtCol(ability+lineEnd, firstValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))
	
def writeTradeContracts(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Trade Contracts:", race, file))
	for contract in race.getTradeContracts():
		file.write(getStringAtCol(contract+lineEnd, firstValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))

def writeHomeSystems(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Home Systems:", race, file))
	for system in race.getHomeSystems():
		file.write(getStringAtCol(blockStart, secondValueCol, "", race, file))
		#Name
		tag = getStringAtCol("Name:", secondValueCol, "", race, file)
		file.write(getStringAtCol(system.getName()+lineEnd, thirdValueCol, tag, race, file))
		#Type
		tag = getStringAtCol("Type:", secondValueCol, "", race, file)
		file.write(getStringAtCol(system.getType()+lineEnd, thirdValueCol, tag, race, file))
		#Planets
		tag = getStringAtCol("Planets:", secondValueCol, "", race, file)
		file.write(getStringAtCol(blockStart, thirdValueCol, tag, race, file))
		for planet in system.getPlanets():
			file.write(getStringAtCol(blockStart, fourthValueCol, "", race, file))
			#Name
			tag = getStringAtCol("Name:", fourthValueCol, "", race, file)
			file.write(getStringAtCol(planet.getName()+lineEnd, fifthValueCol, tag, race, file))
			#Text
			tag = getStringAtCol("Text:", fourthValueCol, "", race, file)
			file.write(getStringAtCol(planet.getFlavorText()+lineEnd, fifthValueCol, tag, race, file))
			#Resources
			tag = getStringAtCol("Resources:", fourthValueCol, "", race, file)
			file.write(getStringAtCol(planet.getResources()+lineEnd, fifthValueCol, tag, race, file))
			#Influence
			tag = getStringAtCol("Influence:", fourthValueCol, "", race, file)
			file.write(getStringAtCol(planet.getInfluence()+lineEnd, fifthValueCol, tag, race, file))
			file.write(getStringAtCol(blockEnd, fourthValueCol, "", race, file))
		file.write(getStringAtCol(blockEnd, thirdValueCol, "", race, file))
		file.write(getStringAtCol(blockEnd, secondValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))
	
def writeStartingUnits(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Starting Units:", race, file))
	for unit in race.getStartingUnits():
		file.write(getStringAtCol(blockStart, secondValueCol, "", race, file))
		name, quantity = unit
		#Unit name
		tag = getStringAtCol("Unit:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(name+lineEnd, fourthValueCol, tag, race, file))
		#Quantity
		tag = getStringAtCol("Quantity:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(quantity+lineEnd, fourthValueCol, tag, race, file))
		file.write(getStringAtCol(blockEnd, secondValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))	
	
def writeStartingTechs(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Starting Techs:", race, file))
	for tech in race.getStartingTechs():
		file.write(getStringAtCol(tech+lineEnd, secondValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))	
	
def writeLeaders(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Leaders:", race, file))
	for leader in race.getLeaders():
		file.write(getStringAtCol(blockStart, secondValueCol, "", race, file))
		#Name
		tag = getStringAtCol("Name:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(leader.getName()+lineEnd, fourthValueCol, tag, race, file))
		#Type
		tag = getStringAtCol("Type", thirdValueCol, "", race, file)
		file.write(getStringAtCol(leader.getType()+lineEnd, fourthValueCol, tag, race, file))
		file.write(getStringAtCol(blockEnd, secondValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))	
	
def writeRacialTechs(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Racial Techs:", race, file))
	for tech in race.getRacialTechs():
		file.write(getStringAtCol(blockStart, secondValueCol, "", race, file))
		#Name
		tag = getStringAtCol("Name:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(tech.getName()+lineEnd, fourthValueCol, tag, race, file))
		#Expansion
		tag = getStringAtCol("Expansion:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(tech.getExpansion()+lineEnd, fourthValueCol, tag, race, file))
		#Cost
		tag = getStringAtCol("Cost:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(tech.getCost()+lineEnd, fourthValueCol, tag, race, file))
		#Text
		tag = getStringAtCol("Text:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(tech.getText()+lineEnd, fourthValueCol, tag, race, file))
		file.write(getStringAtCol(blockEnd, secondValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))
	
def writeFlagship(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Flagship:", race, file))
	flagship = race.getFlagship()
	#Name
	tag = getStringAtCol("Name:", secondValueCol, "", race, file)
	file.write(getStringAtCol(flagship.getName()+lineEnd, fourthValueCol, tag, race, file))
	#Abilities
	tag = getStringAtCol("Abilities:", secondValueCol, "", race, file)
	file.write(getStringAtCol(blockStart, thirdValueCol, tag, race, file))
	abilities = flagship.getAbilities()
	for ability in abilities:
		file.write(getStringAtCol(ability+lineEnd, fourthValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, thirdValueCol, "", race, file))
	#Text
	tag = getStringAtCol("Text:", secondValueCol, "", race, file)
	file.write(getStringAtCol(flagship.getText()+lineEnd, thirdValueCol, tag, race, file))
	#Cost
	tag = getStringAtCol("Cost:", secondValueCol, "", race, file)
	file.write(getStringAtCol(flagship.getCost()+lineEnd, thirdValueCol, tag, race, file))
	#Battle value
	tag = getStringAtCol("Battle:", secondValueCol, "", race, file)
	file.write(getStringAtCol(flagship.getBattle()+lineEnd, thirdValueCol, tag, race, file))
	#Battle multiplier
	tag = getStringAtCol("Multiplier:", secondValueCol, "", race, file)
	file.write(getStringAtCol(flagship.getMultiplier()+lineEnd, thirdValueCol, tag, race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))
	
def writeRepresentatives(race, file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Representatives:", race, file))
	for rep in race.getRepresentatives():
		file.write(getStringAtCol(blockStart, secondValueCol, "", race, file))
		#Name
		tag = getStringAtCol("Name:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(rep.getName()+lineEnd, fourthValueCol, tag, race, file))
		#Votes
		tag = getStringAtCol("Votes:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(rep.getVotes()+lineEnd, fourthValueCol, tag, race, file))
		#Types
		tag = getStringAtCol("Types:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(blockStart, fourthValueCol, tag, race, file))
		types = rep.getTypes()
		for type in types:
			file.write(getStringAtCol(type+lineEnd, fifthValueCol, "", race, file))
		file.write(getStringAtCol(blockEnd, fourthValueCol, "", race, file))
		#Text
		tag = getStringAtCol("Text:", thirdValueCol, "", race, file)
		file.write(getStringAtCol(rep.getText()+lineEnd, fourthValueCol, tag, race, file))
		file.write(getStringAtCol(blockEnd, secondValueCol, "", race, file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", race, file))
	
def writeOne(race,file):
	#Takes in a single race object and writes it to the given output file location
	if outFormat == ".tirace":
		file.write(blockStart)
		writeFullName(race,file)
		writeShortName(race,file)
		writeSpeciesName(race,file)
		writeExpansion(race,file)
		writeHistory(race,file)
		writeSpecialAbilities(race,file)
		writeTradeContracts(race,file)
		writeHomeSystems(race,file)
		writeStartingUnits(race,file)
		writeStartingTechs(race,file)
		writeLeaders(race,file)
		writeRacialTechs(race,file)
		writeFlagship(race,file)
		writeRepresentatives(race,file)
		file.write(blockEnd)
	#No other formats currently implemented

def writeAll(races, outFolder):
	#Takes in a dict of Race objects and writes them to file(s)
	
	#Make sure directories exist
	outPath = path = outFolder + sys.argv[1].lower().capitalize() + "/Races/"
	if not os.path.exists(path):
		os.makedirs(path)

	if not multiFileOut:
		outFileName = "races" + outFormat
		outFileLoc = path + outFileName
		tempFileName = "racestemp.tmp"
		fileMode = 'a'
	else:
		fileMode = 'w'
	for race in races.values():
		if multiFileOut:
			outFileName = race.getNameShort() + outFormat
			outFileLoc = path + outFileName
			tempFileName = race.getNameShort() + "temp.tmp"
		with open(tempFileName, fileMode) as outFile:
			writeOne(race,outFile)
			
	if not os.path.exists("./Backups/"+path[3:]):
		os.makedirs("./Backups/"+path[3:])
	if not multiFileOut:
		#Save old file as a backup
		shutil.move(path+"/"+outFileName,"./Backups/"+path[3:]+outFileName+".bak")
		#Replace old file with temp file
		shutil.move("racestemp.tmp",path+"/"+outFileName)
	else:
		for race in races.values():
			#Save old file as a backup
			shutil.move(path+"/"+race.getNameShort()+outFormat,"./Backups/"+path[3:]+race.getNameShort()+outFormat)
			#Replace old file with temp file
			shutil.move(race.getNameShort()+"temp.tmp",path+'/'+race.getNameShort()+outFormat)

################################################################
#	Main program flow section
################################################################
			
def main():
	checkUsage()
	inFolder, outFolder = readFolders()
	races = readAll(inFolder)
	writeAll(races, outFolder)

main()