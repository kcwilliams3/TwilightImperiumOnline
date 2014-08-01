# syntaxAssertions.py
#
# Contains file syntax checks used by scripts
#
# Written by Kaleb Williams as part of Twilight Imperium project
#
# Used in scripts

import sys

from libs.exceptions import FileSyntaxError

def _checkEmpty(line, assertion, fileName, lineNumber):
	empty = (line == "\n" or line == "")
	if empty != assertion:
		try:
			raise FileSyntaxError
		except FileSyntaxError as e:
			print("File \"{0}\", line {1}".format(fileName,lineNumber))
			if assertion:
				print("FileSyntaxError: Expected empty line, got \"{0}\"".format(line.strip()))
			else:
				print("FileSyntaxError: Unexpected empty line")
			sys.exit()

def assertEmpty(line, fileName, lineNumber):
	_checkEmpty(line, True, fileName, lineNumber)

def assertNotEmpty(line, fileName, lineNumber):
	_checkEmpty(line, False, fileName, lineNumber)
	
def _checkSeparator(line, assertion, fileName, lineNumber):
	separator = True
	for char in line.strip():
		if char != "-":
			separator = False
	try:
		if separator != assertion:
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		if assertion:
			print("FileSyntaxError: Expected separator, got \"{0}\"".format(line.strip()))
		else:
			print("FileSyntaxError: Unexpected separator")
		sys.exit()
			
def assertSeparator(line, fileName, lineNumber):
	_checkSeparator(line, True, fileName, lineNumber)
	
def assertNotSeparator(line, fileName, lineNumber):
	_checkSeparator(line, False, fileName, lineNumber)
	
def assertValidRace(line, races, fileName, lineNumber):
	try:
		if not line.strip() in races and not "The " + line.strip():
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		print("FileSyntaxError: Expected a full race name, got \"{0}\"".format(line.strip()))
		sys.exit()
		
def assertValidExpansion(line, fileName, lineNumber):
	expansions = ["Vanilla","Shattered Empire","Shards of the Throne"]
	try:
		if not line.strip() in expansions:
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		print("FileSyntaxError: Expected an expansion name or \"Vanilla\", got \"{0}\"".format(line.strip()))
		sys.exit()
		
def assertString(line, assertion, fileName, lineNumber):
	try:
		if line.strip() != assertion:
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		print("FileSyntaxError: Expected \"{0}\", got \"{1}\"".format(assertion, line.strip()))
		sys.exit()
		
def assertNumber(token, fileName, lineNumber):
	try:
		if not token.strip().isdigit():
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		print("FileSyntaxError: Expected a number, got \"{0}\"".format(token.strip()))
		sys.exit()

def assertUnitType(token, fileName, lineNumber):
	unitTypes = ["Ground Force", "Destroyer", "Carrier", "Fighter", "Space Dock", "Dreadnought", "Mechanized Unit", "Cruiser", "Shock Troop", "War Sun", "PDS"]
	try:
		if not token.strip() in unitTypes and not token.strip()[:-1] in unitTypes:
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		print("FileSyntaxError: Expected a unit type, got \"{0}\"".format(token.strip()))
		sys.exit()
		
def assertNumLines(lineList, number, fileName, lineNumber):
	try:
		if len(lineList) != number:
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		print("FileSyntaxError: Expected block of length {0}, got block of length {1}.".format(number, len(lineList)))
		sys.exit()
		
def assertSystemType(token, fileName, lineNumber):
	systemTypes = ["Unattached", "Standard", "Special"]
	try:
		if not token.strip() in systemTypes:
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		print("FileSyntaxError: Expected a system type, got {0}.".format(token))
		sys.exit()
		
def assertValidRepType(token, fileName, lineNumber):
	repTypes = ["Bodyguard","Spy","Councilor"]
	try:
		if not token.strip() in repTypes:
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		print("FileSyntaxError: Expected a representative type, got {0}.".format(token))
		sys.exit()
		
def assertValidLeaderType(token, fileName, lineNumber):
	leaderTypes = ["Diplomat", "Agent", "Scientist", "General", "Admiral"]
	try:
		if not token.strip() in leaderTypes:
			raise FileSyntaxError
	except FileSyntaxError as e:
		print("File \"{0}\", line {1}".format(fileName,lineNumber))
		print("FileSyntaxError: Expected a leader type, got {0}.".format(token))
		sys.exit()