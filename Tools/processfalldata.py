# processfalldata.py
#
# Reads the raw text data relating to race-specific information
# and saves it in the format that will be read by the game.
#
# Current output format options: multiformat : 
#		1x .tipols, 1x .tiobjs, 1x .titrts, 1x .tirace 
#		(basically just .txt files with specific formats)
#
# Written by Kaleb Williams as part of Twilight Imperium Online project
#
# Usage: processfalldata.py <language=english || ...>

import shutil
import os
import sys
import math
import string
from libs.inputClasses import Race,System,Technology,Planet,PoliticalCard,Objective,Treaty
from libs.exceptions import UsageError
from libs.syntaxAssertions import *

acceptedLanguages = ["english"]
outFormat = "multiformat"
tabSize = 4
firstValueCol = 4
secondValueCol = firstValueCol + 2
thirdValueCol = secondValueCol + 3
fourthValueCol = thirdValueCol + 3
fifthValueCol = fourthValueCol + 6
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
		print("UsageError: processfalldata.py <language=english || ...>")

################################################################
#	File read section
################################################################

def readFolders():
	#Reads in the locations of the In and Out folders

	with open("folders.txt", 'r') as foldersFile:
		lines = foldersFile.read().split("\n")
		return lines[0].strip(),lines[1].strip()

def readRaceNames(section, fileName, lineNumber):
	lines = section.split("\n")
	assertNumLines(lines, 3, fileName, lineNumber)
	assertNotEmpty(lines[0], fileName, lineNumber)
	assertNotSeparator(lines[0], fileName, lineNumber)
	lineNumber+=1
	assertNotEmpty(lines[1], fileName, lineNumber)
	assertNotSeparator(lines[1], fileName, lineNumber)
	lineNumber+=1
	assertNotEmpty(lines[2], fileName, lineNumber)
	assertNotSeparator(lines[2], fileName, lineNumber)
	lineNumber+=1
	
	return lines[0].strip(), lines[1].strip(), lines[2].strip(), lineNumber
		
def readRaceSheet(section, fileName, lineNumber):
	blocks = section.split("\n\n")
	i = 0
	startingUnits = {}
	startingTechs = []
	specialAbilities = []
	history = []
	for block in blocks:
		lines = block.split("\n")
		if i == 0:
			#Section header ("Race Sheet")
			assertNumLines(lines, 1, fileName, lineNumber)
			assertNotEmpty(lines[0], fileName, lineNumber)
			assertNotSeparator(lines[0], fileName, lineNumber)
			lineNumber+=1
		elif i == 1:
			#Starting units
			assertString(lines[0], "Starting Units", fileName, lineNumber)
			for line in lines:
				if line != lines[0]:
					number,split,unit = line.partition(" ")
					assertNumber(number, fileName, lineNumber)
					assertUnitType(unit, fileName, lineNumber)
					startingUnits[unit.strip()] = number.strip()
				lineNumber+=1
		elif i == 2:
			#Starting technology
			assertString(lines[0], "Starting Technology", fileName, lineNumber)
			for line in lines:
				if line != lines[0]:
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					startingTechs.append(line.strip())
				lineNumber+=1
		elif i == 3:
			#Special abilities
			assertString(lines[0], "Special Abilities", fileName, lineNumber)
			for line in lines:
				if line != lines[0]:
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					specialAbilities.append(line.strip())
				lineNumber+=1
		elif i == 4:
			#History
			assertString(lines[0], "History", fileName, lineNumber)
			for line in lines:
				if line == "":
					continue
				if line != lines[0]:
					assertNotEmpty(line, fileName, lineNumber)
					assertNotSeparator(line, fileName, lineNumber)
					history.append(line.strip())
				lineNumber+=1
		if block != "":
			lineNumber+=1
		i += 1
			
	return startingUnits, startingTechs, specialAbilities, history, lineNumber

def readTradeContracts(section, fileName, lineNumber):
	tag, contracts = section.split(":")
	c1, c2 = contracts.split(",")
	assertNumber(c1, fileName, lineNumber)
	assertNumber(c2, fileName, lineNumber)
	contracts = [c1.strip(), c2.strip()]
	lineNumber += 1
	
	return contracts, lineNumber
			
def readHomeSystem(section, fileName, lineNumber):
	blocks = section.split("\n\n")
	for block in blocks:
		lines = block.split("\n")
		for line in lines:
			if line == "":
				continue
			if line == lines[0]:
				#Section header ("Home System")
				assertNotEmpty(line, fileName, lineNumber)
				assertNotSeparator(line, fileName, lineNumber)
			elif line == lines[1]:
				#System name
				assertNotEmpty(line, fileName, lineNumber)
				assertNotSeparator(line, fileName, lineNumber)
				systemName = line.strip().rstrip(":")
				system = System(systemName, "Fixed")
			else:
				#Planet
				assertNotEmpty(line, fileName, lineNumber)
				assertNotSeparator(line, fileName, lineNumber)
				system.addPlanet(line.strip())
			lineNumber+=1
	if block != "":
		lineNumber+=1
	
	return system, lineNumber
						
def readHomePlanet(section, fileName, lineNumber):
	lines = section.split("\n")
	extras = ""
	for i in range(0,len(lines)):
		line = lines[i]
		part1 = line.split(' ')[0]
		if i==0:
			#Section header ("Home Planet")
			assertNotEmpty(line, fileName, lineNumber)
			assertNotSeparator(line, fileName, lineNumber)
		elif part1 == "Green" or part1 == "Red" or part1 == "Yellow" or part1 == "Blue" or part1 == "Refresh":
			#Extras (refresh abilities, tech specialties, etc.)
			extras = line.strip().split(", ")
			for extra in extras:
				assertNotEmpty(extra, fileName, lineNumber)
				assertNotSeparator(extra, fileName, lineNumber)
			planet = Planet(planetName, flavorText, resources, influence, extras)
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
		lineNumber+=1
	lineNumber+=1
	
	return Planet(planetName, flavorText, resources, influence, extras), lineNumber

def readAgenda(block, fileName, lineNumber):
	lines = block.split("\n")
	for i in range(0,len(lines)):
		line = lines[i]
		if line.strip()=="":
			continue
		elif i == 0:
			#Political name    (LAW) || (EVENT) ?
			if line.strip()[-5:] == "(LAW)":
				law = True
				event = False
				politicalName = line[:-6].strip()
			elif line.strip()[-7:] == "(EVENT)":
				law = False
				event = True
				politicalName = line[:-8].strip()
			else:
				law = False
				event = False
				politicalName = line.strip()
			assertNotEmpty(politicalName, fileName, lineNumber)
			assertNotSeparator(politicalName, fileName, lineNumber)
		elif i == 1:
			#Flavor text
			flavor = line.strip()
			assertNotEmpty(flavor, fileName, lineNumber)
			assertNotSeparator(flavor, fileName, lineNumber)
			agendaCard = PoliticalCard(politicalName, law, "Shards of the Throne", flavor, event)
		else:
			#Rule text line
			ruleLine = line.strip()
			assertNotEmpty(ruleLine, fileName, lineNumber)
			assertNotSeparator(ruleLine, fileName, lineNumber)	
			agendaCard.addRuleText(ruleLine)
		lineNumber += 1
	lineNumber += 1
	
	return agendaCard, lineNumber
	
def readObjective(section, fileName, lineNumber, isLazax=False):
	lines = section.split("\n")
	if not isLazax:
		start = 1
	else:
		start = 0
	rewardText = ""
	for i in range(start,len(lines)+start):
		line = lines[i-start]
		part1 = line.split(' ')[0]
		if i==0:
			#Section header ("Objective")
			assertNotEmpty(line, fileName, lineNumber)
			assertNotSeparator(line, fileName, lineNumber)
		elif i==1:
			#Name
			assertNotEmpty(line, fileName, lineNumber)
			assertNotSeparator(line, fileName, lineNumber)
			name = line.strip()
		elif i==2:
			#Flavor text
			assertNotEmpty(line, fileName, lineNumber)
			assertNotSeparator(line, fileName, lineNumber)
			flavorText = line.strip()
		elif i==3:
			#Rule text
			assertNotEmpty(line, fileName, lineNumber)
			assertNotSeparator(line, fileName, lineNumber)
			ruleText = line.strip()
		elif i==4:
			#Reward text
			assertNotEmpty(line, fileName, lineNumber)
			assertNotSeparator(line, fileName, lineNumber)
			rewardText = line.strip()
		lineNumber+=1
	lineNumber+=1
	
	if isLazax:
		type = "Lazax"
	else:
		type = "Scenario"
	
	return Objective(name, ["Shards of the Throne"], type, ruleText, rewardText, flavorText), lineNumber
	
def readTreaties(section, fileName, lineNumber, isLazax=False):
	blocks = section.split("\n\n")
	treaties = []
	i=0
	for block in blocks:
		lines = block.split("\n")
		if i == 0 and isLazax:
			#Section header ("Treaties")
			assertNumLines(lines, 1, fileName, lineNumber)
			assertNotEmpty(lines[0], fileName, lineNumber)
			assertNotSeparator(lines[0], fileName, lineNumber)
			lineNumber+=1
		else:
			#Treaty
			name = lines[0].strip()
			flavor = lines[1].strip()
			rule = lines[2].strip()
			suggestion = lines[3].strip()
			rank = lines[4].strip()
			assertNotEmpty(name, fileName, lineNumber)
			assertNotSeparator(name, fileName, lineNumber)
			assertNotEmpty(flavor, fileName, lineNumber)
			assertNotSeparator(flavor, fileName, lineNumber)
			assertNotEmpty(rule, fileName, lineNumber)
			assertNotSeparator(rule, fileName, lineNumber)
			assertNotEmpty(suggestion, fileName, lineNumber)
			assertNotSeparator(suggestion, fileName, lineNumber)
			assertNumber(rank, fileName, lineNumber)
			if (isLazax):
				race = "Lazax"
			else:
				race = ""
			treaties.append(Treaty(name,flavor,rule,suggestion,rank,race))
			lineNumber+=1
		if block != "":
			lineNumber+=1
		i += 1
			
	return treaties, lineNumber

def readRaceFile(fileName):
	race = Race()
	
	#Read race file
	with open(fileName, 'r') as raceFile:
		lineNumber = 1
		sections = raceFile.read().split("--------------------------\n")
		i = 0
		for section in sections:
			section = section.strip()
			if section != "":
				if i==0:
					#Names section
					nameFull, nameShort, nameSpecies, lineNumber = readRaceNames(section, fileName, lineNumber)
				elif i==1:
					#Race Sheet section
					startingUnits, startingTechs, specialAbilities, history, lineNumber =  readRaceSheet(section, fileName, lineNumber)
				elif i==2:
					#Trade Contracts section
					tradeContracts, lineNumber = readTradeContracts(section, fileName, lineNumber)
				elif i==3:
					#Home System section
					homeSystem, lineNumber = readHomeSystem(section, fileName, lineNumber)
				elif i==4:
					#Home Planet section
					homePlanet, lineNumber = readHomePlanet(section, fileName, lineNumber)
					homeSystem.addPlanetDetails(homePlanet.getName(), homePlanet.getFlavorText(), homePlanet.getResources(), homePlanet.getInfluence())
				elif i==5:
					#Objective section
					objective, lineNumber = readObjective(section, fileName, lineNumber, True)
				elif i==6:
					#Treaties section
					treaties, lineNumber = readTreaties(section, fileName, lineNumber, True)
			i += 1
	
	#Set race attributes
	race.setNameFull(nameFull)
	race.setNameShort(nameShort)
	race.setNameSpecies(nameSpecies)
	race.setExpansion("Shards of the Throne")
	for unit in startingUnits.keys():
		race.addStartingUnit(unit, startingUnits[unit])
	for tech in startingTechs:
		race.addStartingTech(tech)
	for ability in specialAbilities:
		race.addSpecialAbility(ability)
	for paragraph in history:
		race.addHistoryParagraph(paragraph)
	for contract in tradeContracts:
		race.addTradeContract(contract)
	race.addHomeSystem(homeSystem)
	
	return race, objective, treaties

def readTreatiesFile(fileName):
	with open(fileName, 'r') as treatiesFile:
		lineNumber = 1
		treaties, lineNumber = readTreaties(treatiesFile.read(), fileName, lineNumber)
	return treaties
	
def readObjectivesFile(fileName):
	objectives = []
	with open(fileName, 'r') as objectivesFile:
		lineNumber = 1
		blocks = objectivesFile.read().split("\n\n")
		blocks = [block for block in blocks if block] #Remove empty blocks
		for block in blocks:
			# Each block is an objective card
			objective, lineNumber = readObjective(block, fileName, lineNumber)
			objectives.append(objective)
			lineNumber += 1
	return objectives
	
def readAgendasFile(fileName):
	agendas = []
	with open(fileName, 'r') as agendasFile:
		lineNumber = 1
		blocks = agendasFile.read().split("\n\n")
		blocks = [block for block in blocks if block] #Remove empty blocks
		for block in blocks:
			# Each block is an agenda card
			agenda, lineNumber = readAgenda(block, fileName, lineNumber)
			agendas.append(agenda)
			lineNumber += 1
	return agendas
			
def readAll(inFolder):
	#Reads in the data from the input files and returns a
	# dict of Race objects
	
	#Set file names
	path = inFolder + sys.argv[1].lower().capitalize() + "/Fall of the Empire/"
	raceFileName = path + "lazax.txt"
	objFileName = path + "objectives.txt"
	treatyFileName = path + "treaties.txt"
	agendaFileName = path + "agendas.txt"

	race, objective, treaties = readRaceFile(raceFileName)
	objectives = [objective]
	objectives.extend(readObjectivesFile(objFileName))
	treaties.extend(readTreatiesFile(treatyFileName))
	agendas = readAgendasFile(agendaFileName)
	
	return race, objectives, treaties, agendas

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

def writeAName(political, file):
	file.write(getStringAtCol(political.getName()+lineEnd, firstValueCol, "Name: ", file))

def writeALaw(political, file):
	if (political.getIsLaw()):
		lawString = "true"
	else:
		lawString = "false"
	file.write(getStringAtCol(lawString+lineEnd, firstValueCol, "Law: ", file))
	
def writeAEvent(political, file):
	if (political.isEvent()):
		eventString = "true"
	else:
		eventString = "false"
	file.write(getStringAtCol(eventString+lineEnd, firstValueCol, "Event: ", file))
	
def writeAExpansion(political, file):
	file.write(getStringAtCol(political.getExpansion()+lineEnd, firstValueCol, "Expansion: ", file))
	
def writeAFlavor(political, file):
	file.write(getStringAtCol(political.getFlavorText()+lineEnd, firstValueCol, "Flavor Text: ", file))
	
def writeARuleText(political, file):
	if political.isEvent():
		type = "Event"
	else:
		type = "For/Against"
	discard = ""
	for rule in political.getRuleText():
		if rule.lower().startswith("elect"):
			type = "Elect"
		if (not political.isEvent()) and ((rule.lower().startswith("discard") and "if" in rule.lower()) or (rule.lower().startswith("if") and len(rule.split(', ')) > 1 and rule.split(', ')[1].lower().startswith("discard"))):
			discard = rule
	if discard != "":
		political.getRuleText().remove(discard)
	if type == "Elect":
		writeElectText(political, file)
	elif type == "Event":
		writeEventText(political, file)
	else:
		writeForAgainstText(political, file)
	if discard != "":
		writeDiscardText(discard, file)

def writeEventText(political, file):
	file.write(getStringAtCol(political.getRuleText()[0]+lineEnd, firstValueCol, "Effect: ", file))
		
def writeElectText(political, file):
	electLine, text = political.getRuleText()
	candidates = electLine[6:]
	file.write(getStringAtCol(candidates+lineEnd, firstValueCol, "Elect: ", file))
	file.write(getStringAtCol(text+lineEnd, firstValueCol, "Effect: ", file))
	
def writeForAgainstText(political, file):
	forText, againstText = political.getRuleText()
	file.write(getStringAtCol(forText[5:]+lineEnd, firstValueCol, "For: ", file))
	file.write(getStringAtCol(againstText[9:]+lineEnd, firstValueCol, "Against: ", file))
	
def writeADiscardText(discard, file):
	file.write(getStringAtCol(discard+lineEnd, firstValueCol, "Discard: ", file))
	
def writeAgendas(agendas, file):
	for agenda in agendas:
		file.write(blockStart)
		writeAName(agenda, file)
		writeALaw(agenda, file)
		writeAEvent(agenda, file)
		writeAExpansion(agenda, file)
		writeAFlavor(agenda, file)
		writeARuleText(agenda, file)
		file.write(blockEnd)
	

def writeTName(treaty, file):
	file.write(getStringAtCol(treaty.getName()+lineEnd, firstValueCol, "Name: ", file))

def writeTFlavor(treaty, file):
	file.write(getStringAtCol(treaty.getFlavor()+lineEnd, firstValueCol, "Flavor: ", file))
	
def writeTRule(treaty, file):
	file.write(getStringAtCol(treaty.getRule()+lineEnd, firstValueCol, "Rule: ", file))
	
def writeTSuggestion(treaty, file):
	file.write(getStringAtCol(treaty.getSuggestion()+lineEnd, firstValueCol, "Suggestion: ", file))

def writeTRank(treaty, file):
	file.write(getStringAtCol(treaty.getRank()+lineEnd, firstValueCol, "Rank: ", file))
	
def writeTRace(treaty, file):
	if treaty.getRace() != "":
		file.write(getStringAtCol(treaty.getRace()+lineEnd, firstValueCol, "Race: ", file))
	
def writeTreaties(treaties, file):
	for treaty in treaties:
		file.write(blockStart)
		writeTName(treaty, file)
		writeTFlavor(treaty, file)
		writeTRule(treaty, file)
		writeTSuggestion(treaty, file)
		writeTRank(treaty, file)
		writeTRace(treaty, file)
		file.write(blockEnd)	
	
	
def writeOName(obj, file):
	file.write(getStringAtCol(obj.getName()+lineEnd, firstValueCol, "Name: ", file))
	
def writeOExpansions(obj, file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Expansions: ", file))
	for expansion in obj.getExpansions():
		file.write(getStringAtCol(expansion+lineEnd, secondValueCol, '', file))
	file.write(getStringAtCol(blockEnd, firstValueCol, '', file))
	
def writeOType(obj, file):
	file.write(getStringAtCol(obj.getType()+lineEnd, firstValueCol, "Type: ", file))
	
def writeOReward(obj, file):
	if obj.getReward() != "":
		file.write(getStringAtCol(obj.getReward()+lineEnd, firstValueCol, "Reward: ", file))

def writeOFlavor(obj, file):
	if obj.getFlavor() != "":
		file.write(getStringAtCol(obj.getFlavor()+lineEnd, firstValueCol, "Flavor: ", file))
	
def writeOText(obj, file):
	file.write(getStringAtCol(obj.getText()+lineEnd, firstValueCol, "Text: ", file))
	
def writeObjectives(objs, file):
	for obj in objs:
		file.write(blockStart)
		writeOName(obj, file)
		writeOType(obj, file)
		writeOExpansions(obj, file)
		writeOFlavor(obj, file)
		writeOText(obj, file)
		writeOReward(obj, file)
		file.write(blockEnd)
	
def writeFullName(race,file):
	file.write(getStringAtCol(race.getNameFull()+lineEnd, firstValueCol, "Full Name: ", file))

def writeShortName(race,file):
	file.write(getStringAtCol(race.getNameShort()+lineEnd, firstValueCol, "Short Name: ", file))
	
def writeSpeciesName(race,file):
	file.write(getStringAtCol(race.getNameSpecies()+lineEnd, firstValueCol, "Species Name: ", file))

def writeExpansion(race,file):
	file.write(getStringAtCol(race.getExpansion()+lineEnd, firstValueCol, "Expansion: ", file))
	
def writeHistory(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "History:", file))
	for paragraph in race.getHistory():
		file.write(getStringAtCol(paragraph+lineEnd, firstValueCol, "", file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", file))
	
def writeSpecialAbilities(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Special Abilities:", file))
	for ability in race.getSpecialAbilities():
		file.write(getStringAtCol(ability+lineEnd, firstValueCol, "", file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", file))
	
def writeTradeContracts(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Trade Contracts:", file))
	for contract in race.getTradeContracts():
		file.write(getStringAtCol(contract+lineEnd, firstValueCol, "", file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", file))

def writeHomeSystems(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Home Systems:", file))
	for system in race.getHomeSystems():
		file.write(getStringAtCol(blockStart, secondValueCol, "", file))
		#Name
		tag = getStringAtCol("Name:", secondValueCol, "", file)
		file.write(getStringAtCol(system.getName()+lineEnd, thirdValueCol, tag, file))
		#Type
		tag = getStringAtCol("Type:", secondValueCol, "", file)
		file.write(getStringAtCol(system.getType()+lineEnd, thirdValueCol, tag, file))
		#Planets
		tag = getStringAtCol("Planets:", secondValueCol, "", file)
		file.write(getStringAtCol(blockStart, thirdValueCol, tag, file))
		for planet in system.getPlanets():
			file.write(getStringAtCol(blockStart, fourthValueCol, "", file))
			#Name
			tag = getStringAtCol("Name:", fourthValueCol, "", file)
			file.write(getStringAtCol(planet.getName()+lineEnd, fifthValueCol, tag, file))
			#Text
			tag = getStringAtCol("Text:", fourthValueCol, "", file)
			file.write(getStringAtCol(planet.getFlavorText()+lineEnd, fifthValueCol, tag, file))
			#Resources
			tag = getStringAtCol("Resources:", fourthValueCol, "", file)
			file.write(getStringAtCol(planet.getResources()+lineEnd, fifthValueCol, tag, file))
			#Influence
			tag = getStringAtCol("Influence:", fourthValueCol, "", file)
			file.write(getStringAtCol(planet.getInfluence()+lineEnd, fifthValueCol, tag, file))
			#Extras
			refresh = ''
			techSpecialties = []
			for extra in planet.getExtras():
				extraParts = extra.split(' ')
				if extraParts[0] == "Refresh":
					refresh = extra.split(": ")[1]
				elif extraParts[2] == "Specialty":
					techSpecialties.append(extraParts[0])
			tag = getStringAtCol("Refresh Ability:", fourthValueCol, "", file)
			file.write(getStringAtCol(refresh+lineEnd, fifthValueCol, tag, file))
			tag = getStringAtCol("Tech Specialties:", fourthValueCol, "", file)
			file.write(getStringAtCol(blockStart, fifthValueCol, tag, file))
			for spec in techSpecialties:
				file.write(getStringAtCol(spec+lineEnd, sixthValueCol, "", file))
			file.write(getStringAtCol(blockEnd, fifthValueCol, "", file))
			file.write(getStringAtCol(blockEnd, fourthValueCol, "", file))
				
					
		file.write(getStringAtCol(blockEnd, thirdValueCol, "", file))
		file.write(getStringAtCol(blockEnd, secondValueCol, "", file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", file))
	
def writeStartingUnits(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Starting Units:", file))
	for unit in race.getStartingUnits():
		file.write(getStringAtCol(blockStart, secondValueCol, "", file))
		name, quantity = unit
		#Unit name
		tag = getStringAtCol("Unit:", thirdValueCol, "", file)
		file.write(getStringAtCol(name+lineEnd, fourthValueCol, tag, file))
		#Quantity
		tag = getStringAtCol("Quantity:", thirdValueCol, "", file)
		file.write(getStringAtCol(quantity+lineEnd, fourthValueCol, tag, file))
		file.write(getStringAtCol(blockEnd, secondValueCol, "", file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", file))	
	
def writeStartingTechs(race,file):
	file.write(getStringAtCol(blockStart, firstValueCol, "Starting Techs:", file))
	for tech in race.getStartingTechs():
		file.write(getStringAtCol(tech+lineEnd, secondValueCol, "", file))
	file.write(getStringAtCol(blockEnd, firstValueCol, "", file))	
	
def writeRace(race,file):
	#Takes in a single race object and writes it to the given output file location
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
	file.write(blockEnd)

def writeAll(race, objectives, treaties, agendas, outFolder):
	#Takes in relevant objects and writes them to file(s)
	
	#Make sure directories exist
	path = outFolder + sys.argv[1].lower().capitalize() + "/Fall of the Empire/"
	if not os.path.exists(path):
		os.makedirs(path)

	fileMode = 'w'
		
	fileNames = ["lazax.tirace","scenario.tiobjs","treaties.titrts","agendas.tipols"]
	tempFiles = ["lazaxtemp.tmp","scenariotemp.tmp","treatiestemp.tmp","agendastemp.tmp"]
	
	with open(tempFiles[0], fileMode) as raceFile:
		writeRace(race,raceFile)
	with open(tempFiles[1], fileMode) as objsFile:
		writeObjectives(objectives,objsFile)
	with open(tempFiles[2], fileMode) as treatiesFile:
		writeTreaties(treaties,treatiesFile)
	with open(tempFiles[3], fileMode) as agendasFile:
		writeAgendas(agendas,agendasFile)
	
	if not os.path.exists("./Backups/"+path[3:]):
		os.makedirs("./Backups/"+path[3:])
	for i in range(0,len(fileNames)):
		if os.path.isfile(path+fileNames[i]):
			#Save old file as a backup
			shutil.move(path+fileNames[i],"./Backups/"+path[3:]+fileNames[i])
		#Replace old file with temp file
		shutil.move(fileNames[i].split('.')[0]+"temp.tmp",path+'/'+fileNames[i])

################################################################
#	Main program flow section
################################################################
			
def main():
	checkUsage()
	inFolder, outFolder = readFolders()
	race, objectives, treaties, agendas = readAll(inFolder)
	
	writeAll(race, objectives, treaties, agendas, outFolder)

main()