# inputClasses.py
#
# Contains the classes used when reading in data 
#
# Written by Kaleb Williams as part of Twilight Imperium project
#
# Used in process______data.py scripts

class Race:
	def __init__(self):
		self._nameFull = None
		self._nameShort = None
		self._nameSpecies = None
		self._expansion = None
		self._startingUnits = {}
		self._startingTechs = []
		self._specialAbilities = []
		self._history = []
		self._homeSystems = []
		self._flagship = []
		self._representatives = []
		self._racialTechs = []
		self._tradeContracts = []
		self._leaders = []
	
	def __repr__(self):
		return "Race Object: {0}".format(self._nameShort)
	
	def getNameFull(self):
		return self._nameFull
	
	def setNameFull(self, name):
		self._nameFull = name
		
	def getNameShort(self):
		return self._nameShort
		
	def setNameShort(self, name):
		self._nameShort = name
	
	def getNameSpecies(self):
		return self._nameSpecies
		
	def setNameSpecies(self, name):
		self._nameSpecies = name
		
	def getExpansion(self):
		return self._expansion
		
	def setExpansion(self, expansion):
		self._expansion = expansion
		
	def addStartingUnit(self, unit, quantity):
		self._startingUnits[unit] = quantity
		
	def getStartingUnits(self):
		units = []
		for unit in self._startingUnits.keys():
			units.append((unit,self._startingUnits[unit]))
		return tuple(units)
		
	def addStartingTech(self, tech):
		self._startingTechs.append(tech)
		
	def getStartingTechs(self):
		return tuple(self._startingTechs)
	
	def addSpecialAbility(self, ability):
		self._specialAbilities.append(ability)
		
	def getSpecialAbilities(self):
		return tuple(self._specialAbilities)
		
	def addHistoryParagraph(self, paragraph):
		self._history.append(paragraph)
	
	def getHistory(self):
		return tuple(self._history)
		
	def addHomeSystem(self, system):
		self._homeSystems.append(system)
		
	def getHomeSystems(self):
		return tuple(self._homeSystems)
		
	def addTradeContract(self, contract):
		self._tradeContracts.append(contract)
		
	def getTradeContracts(self):
		return self._tradeContracts
		
	def addRacialTech(self, tech):
		self._racialTechs.append(tech)
		
	def getRacialTechs(self):
		return self._racialTechs
		
	def setFlagship(self, ship):
		self._flagship = ship
		
	def getFlagship(self):
		return self._flagship
		
	def addRepresentative(self, rep):
		self._representatives.append(rep)
		
	def getRepresentatives(self):
		return tuple(self._representatives)
		
	def addLeader(self, leader):
		self._leaders.append(leader)
	
	def getLeaders(self):
		return tuple(self._leaders)
		
class System:
	def __init__(self, name, type):
		self._name = name
		self._planets = []
		self._type = type
	
	def __repr__(self):
		return "System Object: {0}".format(self._name)
		
	def getName(self):
		return self._name
		
	def addPlanet(self, planet):
		self._planets.append(planet)
	
	def getPlanets(self):
		return tuple(self._planets)
		
	def addPlanetDetails(self, name, flavor, resources, influence):
		for planet in self._planets:
			if planet == name:
				self._planets.remove(planet)
				self._planets.append(Planet(name, flavor, resources, influence))
				
	def addWormholeDetails(self):
		for planet in self._planets:
			if "Wormhole" in planet:
				self._planets.remove(planet)
				self._planets.append(Planet(planet, "", 0, 0))
		
	def getType(self):
		return self._type
		
class Planet:
	def __init__(self, name, flavor, resources, influence):
		self._name = name
		self._flavorText = flavor
		self._resources = resources
		self._influence = influence
	
	def __repr__(self):
		return "Planet Object: {0}".format(self._name)
	
	def getName(self):
		return self._name
		
	def getFlavorText(self):
		return self._flavorText
		
	def getResources(self):
		return str(self._resources)
		
	def getInfluence(self):
		return str(self._influence)
	
class Technology:
	def __init__(self, name, color, expansion, requirementsOrCost, text, isRacial=False):
		self._name = name
		self._color = color
		self._expansion = expansion
		self._text = text
		if isRacial:
			self._requirements = []
			self._cost = requirementsOrCost
		else:
			self._requirements = requirementsOrCost
			self._cost = None
		
	def __repr__(self):
		return "Technology Object: {0}".format(self._name)
	
	def getName(self):
		return self._name
		
	def getColor(self):
		return self._color
		
	def getExpansion(self):
		return self._expansion
		
	def getRequirements(self):
		return tuple(self._requirements)
		
	def getCost(self):
		return str(self._cost)
		
	def getText(self):
		return self._text
		
class Flagship:
	def __init__(self, name, abilities, text, cost, battle, multiplier, move, capacity):
		self._name = name
		self._abilities = abilities
		self._text = text
		self._cost = cost
		self._battle = battle
		self._multiplier = multiplier
		self._move = move
		self._capacity = capacity
		
	def __repr__(self):
		return "Flagship Object {0}".format(self._name)
		
	def getName(self):
		return self._name
	
	def getAbilities(self):
		return tuple(self._abilities)
		
	def getText(self):
		return self._text
		
	def getCost(self):
		return str(self._cost)
		
	def getBattle(self):
		return str(self._battle)
	
	def getMultiplier(self):
		return str(self._multiplier)
		
	def getMove(self):
		return str(self._move)
		
	def getCapacity(self):
		return str(self._capacity)
		
class Representative:
	def __init__(self, name, votes, types, text):
		self._name = name
		self._votes = votes
		self._types = types
		self._text = text
		
	def getName(self):
		return self._name
		
	def getVotes(self):
		return str(self._votes)
		
	def getTypes(self):
		return tuple(self._types)
		
	def getText(self):
		return self._text
		
class Leader:
	def __init__(self, name, type):
		self._name = name
		self._type = type
		
	def getName(self):
		return self._name
		
	def getType(self):
		return self._type
		
class Action:
	def __init__(self, name, expansion, flavor, textA, play, textB, quantity):
		self._name = name
		self._expansion = expansion
		self._flavor = flavor
		self._textA = textA
		self._play = play
		self._textB = textB
		self._quantity = quantity
		
	def __repr__(self):
		return "Action Object {0}".format(self._name)
		
	def getName(self):
		return self._name
		
	def getExpansion(self):
		return self._expansion
		
	def getFlavor(self):
		return self._flavor
		
	def getTextA(self):
		return tuple(self._textA)
		
	def getTextB(self):
		return tuple(self._textB)
	
	def getPlay(self):
		return self._play
		
	def getQuantity(self):
		return self._quantity
		
class Component:
	def __init__(self, name, qualifier, vanillaCount, shatteredEmpireCount, shardsOfTheThroneCount):
		self._name = name
		self._vanillaCount = vanillaCount
		self._SECount = shatteredEmpireCount
		self._SotTCount = shardsOfTheTrhoneCount
		self._qualifier = qualifier