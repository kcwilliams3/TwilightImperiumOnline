using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class LocalizationException : System.Exception, ISerializable
{
	public LocalizationException()
		: base() { }
	public LocalizationException(string message)
		: base(message) { }
	public LocalizationException (string message, System.Exception inner)
		: base(message, inner) { }
	protected LocalizationException (SerializationInfo info, StreamingContext context)
		: base(info, context) { }
	public LocalizationException(string languageString, string englishString) 
		: base("LocalizationException: {0}: Unrecognized identifier {1}",languageString,englishString) { }
}


public class LanguageManager : TIOMonoBehaviour {

	public string Language;

	//String to _____ dicts
	private Dictionary<string,int> toNumber = new Dictionary<string,int>();
	private Dictionary<string,bool> toBoolean = new Dictionary<string,bool>();
	private Dictionary<string,TPrereqMode> toTPrereqMode = new Dictionary<string,TPrereqMode>();
	private Dictionary<string,string> toDataType = new Dictionary<string,string>();
	private Dictionary<string,Expansion> toExpansion = new Dictionary<string,Expansion>();
	private Dictionary<string,UType> toUType = new Dictionary<string,UType>();
	private Dictionary<string,UAbility> toUAbility = new Dictionary<string,UAbility>();
	private Dictionary<string,TType> toTType = new Dictionary<string,TType>();
	private Dictionary<string,string> toSTag = new Dictionary<string,string>();
	private Dictionary<string,SType> toSType = new Dictionary<string,SType>();
	private Dictionary<string,OType> toOType = new Dictionary<string,OType>();
	private Dictionary<string,OReward> toOReward = new Dictionary<string,OReward>();
	private Dictionary<string,StrategySet> toStrategySet = new Dictionary<string,StrategySet>();
	private Dictionary<string,string> toElect = new Dictionary<string,string>();
	private Dictionary<string,RType> toRType = new Dictionary<string,RType>();
	private Dictionary<string,Option> toOption = new Dictionary<string,Option>();
	private Dictionary<string,LType> toLType = new Dictionary<string,LType> ();
	
	//______ to string dicts
	private Dictionary<int,string> numberTo = new Dictionary<int,string>();
	private Dictionary<bool,string> booleanTo = new Dictionary<bool,string>();
	private Dictionary<TPrereqMode,string> tPrereqModeTo = new Dictionary<TPrereqMode,string>();
	private Dictionary<Expansion,string> expansionTo = new Dictionary<Expansion,string>();
	private Dictionary<UType,string> uTypeTo = new Dictionary<UType,string>();
	private Dictionary<UAbility,string> uAbilityTo = new Dictionary<UAbility,string>();
	private Dictionary<TType,string> tTypeTo = new Dictionary<TType,string>();
	private Dictionary<string,string> sTagTo = new Dictionary<string,string>();
	private Dictionary<SType,string> sTypeTo = new Dictionary<SType, string>();
	private Dictionary<OType,string> oTypeTo = new Dictionary<OType, string>();
	private Dictionary<OReward,string> oRewardTo = new Dictionary<OReward,string>();
	private Dictionary<StrategySet,string> strategySetTo = new Dictionary<StrategySet, string>();
	private Dictionary<string,string> electTo = new Dictionary<string,string>();
	private Dictionary<RType,string> rTypeTo = new Dictionary<RType,string>();
	private Dictionary<Option,string> optionTo = new Dictionary<Option,string>();
	private Dictionary<LType,string> lTypeTo = new Dictionary<LType,string>();


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	//StringTo_____ functions
	public int StringToNumber(string numberString) { return toNumber [numberString]; }
	public bool StringToBoolean(string boolString) { return toBoolean [boolString]; }
	public TPrereqMode StringToTPrereqMode(string prereqString) { return toTPrereqMode [prereqString]; }
	public string StringToDataType(string dataTypeString) { return toDataType [dataTypeString]; }
	public Expansion StringToExpansion(string expansionString) { return toExpansion [expansionString]; }
	public UType StringToUType(string uTypeString) { return toUType [uTypeString]; }
	public UAbility StringToUAbility(string uAbilityString) { return toUAbility [uAbilityString]; }
	public TType StringToTType(string tTypeString) { return toTType [tTypeString]; }
	public string StringToSTag(string sTagString) { return toSTag [sTagString]; }
	public SType StringToSType(string sTypeString) { return toSType [sTypeString]; } 
	public OType StringToOType(string oTypeString) { return toOType [oTypeString]; }
	public OReward StringToOReward(string oRewardString) { return toOReward [oRewardString]; }
	public EType StringToEType(string eTypeString) { return toElect[eTypeString]; }
	public RType StringToRType(string rTypeString) { return toRType [rTypeString]; }
	public Option StringToOption(string optionString) { return toOption [optionString]; }
	public LType StringToLType(string lTypeString) { return toLType [lTypeString]; }

	//______ToString functions
	public string NumberToString(int number) { return numberTo [number]; }
	public string BooleanToString(bool boolean) { return booleanTo [boolean]; }
	public string TPrereqModeToString(TPrereqMode prereq) { return tPrereqModeTo [prereq]; }
	public string ExpansionToString(Expansion expo) { return expansionTo [expo]; }
	public string UTypeToString(UType unitType) { return uTypeTo [unitType]; }
	public string UAbilityToString(UAbility unitAbility) { return uAbilityTo [unitAbility]; }
	public string TTypeToString(TType techType) { return tTypeTo [techType]; }
	public string STagToString(string systemTag) { return sTagTo [systemTag]; }
	public string STypeToString(SType systemType) { return sTypeTo [systemType]; }
	public string OTypeToString(OType objectiveType) { return oTypeTo [objectiveType]; }
	public string ORewardToString(OReward objectiveReward) { return oRewardTo [objectiveReward]; }
	public string ETypeToString(EType electionType) { return electTo [electionType]; }
	public string RTypeToString(RType repType) { return rTypeTo [repType]; }
	public string OptionToString(Option opt) { return optionTo [opt]; }
	public string LTypeToString(LType leaderType) { return lTypeTo [leaderType]; }

	//Dictionary-filling functions
	public void AddNumber (string languageString) {
		AddNumber (languageString, languageString);
	}
	public void AddNumber (string languageString, string englishString) {
		int value;
		if (englishString.ToLower() == "two") {
			value = 2;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		numberTo [value] = languageString;
		toNumber [languageString] = value;
	}

	public void AddBoolean(string languageString) {
		AddBoolean (languageString, languageString);
	}
	public void AddBoolean(string languageString, string englishString) {
		bool value;
		if (englishString.ToLower () == "true") {
			value = true;
		} else  if (englishString.ToLower () == "false") {
			value = false;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		booleanTo [value] = languageString;
		toBoolean [languageString] = value;
	}

	public void AddTPrereqMode(string languageString) {
		AddTPrereqMode (languageString, languageString);
	}
	public void AddTPrereqMode(string languageString, string englishString) {
		TPrereqMode value;
		if (englishString.ToLower () == "and") {
			value = TPrereqMode.AND;
		} else  if (englishString.ToLower () == "or") {
			value = TPrereqMode.OR;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		tPrereqModeTo [value] = languageString;
		toTPrereqMode [languageString] = value;
	}

	public void AddDataType(string languageString) {
		AddDataType (languageString, languageString);
	}
	public void AddDataType(string languageString, string englishString) {
		int value;
		toDataType [languageString] = englishString;
	}

	public void AddExpansion(string languageString) {
		AddExpansion (languageString, languageString);
	}
	public void AddExpansion(string languageString, string englishString) {
		Expansion value;
		if (englishString.ToLower () == "vanilla") {
			value = Expansion.Vanilla;
		} else  if (englishString.ToLower () == "shattered empire") {
			value = Expansion.ShatteredEmpire;
		} else if (englishString.ToLower () == "shards of the throne") {
			value = Expansion.ShardsOfTheThrone;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		expansionTo [value] = languageString;
		toExpansion [languageString] = value;
	}

	public void AddUType(string languageString) {
		AddUType (languageString, languageString);
	}
	public void AddUType(string languageString, string englishString) {
		UType value;
		if (englishString.ToLower () == "carrier") {
			value = UType.Carrier;
		} else  if (englishString.ToLower () == "cruiser") {
			value = UType.Cruiser;
		} else if (englishString.ToLower () == "destroyer") {
			value = UType.Destroyer;
		} else if (englishString.ToLower () == "dreadnought") {
			value = UType.Dreadnought;
		} else if (englishString.ToLower () == "fighter") {
			value = UType.Fighter;
		} else if (englishString.ToLower () == "flagship") {
			value = UType.Flagship;
		} else if (englishString.ToLower() == "ground force") {
			value = UType.GroundForce;
		} else if (englishString.ToLower () == "mechanized unit") {
			value = UType.MechanizedUnit;
		} else if (englishString.ToLower () == "pds") {
			value = UType.PDS;
		} else if (englishString.ToLower () == "space dock") {
			value = UType.SpaceDock;
		} else if (englishString.ToLower () == "war sun") {
			value = UType.WarSun;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		uTypeTo [value] = languageString;
		toUType [languageString] = value;
	}

	public void AddUAbility(string languageString) {
		AddUAbility (languageString, languageString);
	}
	public void AddUAbility(string languageString, string englishString) {
		UAbility value;
		if (englishString.ToLower () == "anti-fighter barrage") {
			value = UAbility.AntiFighterBarrage;
		} else  if (englishString.ToLower () == "bombardment") {
			value = UAbility.Bombardment;
		} else if (englishString.ToLower () == "planetary shield") {
			value = UAbility.PlanetaryShield;
		} else if (englishString.ToLower () == "production") {
			value = UAbility.Production;
		} else if (englishString.ToLower () == "sustain damage") {
			value = UAbility.SustainDamage;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		uAbilityTo [value] = languageString;
		toUAbility [languageString] = value;
	}

	public void AddTType(string languageString) {
		AddTType (languageString, languageString);
	}
	public void AddTType(string languageString, string englishString) {
		TType value;
		if (englishString.ToLower () == "blue") {
			value = TType.Blue;
		} else  if (englishString.ToLower () == "green") {
			value = TType.Green;
		} else if (englishString.ToLower () == "racial") {
			value = TType.Racial;
		} else if (englishString.ToLower () == "red") {
			value = TType.Red;
		} else if (englishString.ToLower () == "yellow") {
			value = TType.Yellow;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		tTypeTo [value] = languageString;
		toTType [languageString] = value;
	}

	public void AddSTag(string languageString) {
		AddSTag (languageString, languageString);
	}
	public void AddSTag(string languageString, string englishString) {
		sTagTo [englishString] = languageString;
		toSTag [languageString] = englishString;
	}

	public void AddSType(string languageString) {
		AddSType (languageString, languageString);
	}
	public void AddSType(string languageString, string englishString) {
		SType value;
		if (englishString.ToLower () == "special") {
			value = SType.Special;
		} else  if (englishString.ToLower () == "home") {
			value = SType.Home;
		} else if (englishString.ToLower () == "unattached") {
			value = SType.Unattached;
		} else if (englishString.ToLower () == "fixed") {
			value = SType.Fixed;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		sTypeTo [value] = languageString;
		toSType [languageString] = value;
	}

	public void AddOType(string languageString) {
		AddOType (languageString, languageString);
	}
	public void AddOType(string languageString, string englishString) {
		OType value;
		if (englishString.ToLower () == "public stage i") {
			value = OType.PublicStageI;
		} else  if (englishString.ToLower () == "public stage ii") {
			value = OType.PublicStageII;
		} else if (englishString.ToLower () == "preliminary") {
			value = OType.Preliminary;
		} else if (englishString.ToLower () == "secret") {
			value = OType.Secret;
		} else if (englishString.ToLower () == "special") {
			value = OType.Special;
		} else if (englishString.ToLower () == "lazax") {
			value = OType.Lazax;
		} else if (englishString.ToLower () == "scenario") {
			value = OType.Scenario;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		oTypeTo [value] = languageString;
		toOType [languageString] = value;
	}

	public void AddOReward(string languageString) {
		AddOReward (languageString, languageString);
	}
	public void AddOReward(string languageString, string englishString) {
		OReward value;
		if (englishString.ToLower () == "i win the game") {
			value = OReward.WIN;
		} else  if (englishString.ToLower () == "immediate victory") {
			value = OReward.INSTANTWIN;
		} else if (englishString.ToLower () == "game over") {
			value = OReward.GAMEOVER;
		} else if (englishString.ToLower () == "vp") {
			value = OReward.VP;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		oRewardTo [value] = languageString;
		toOReward [languageString] = value;
	}

	public void AddStrategySet(string languageString) {
		AddStrategySet (languageString, languageString);
	}
	public void AddStrategySet(string languageString, string englishString) {
		StrategySet value;
		if (englishString.ToLower () == "vanilla set") {
			value = StrategySet.Vanilla;
		} else  if (englishString.ToLower () == "shattered empire set") {
			value = StrategySet.ShatteredEmpire;
		} else if (englishString.ToLower () == "fall of the empire variant") {
			value = StrategySet.FallOfTheEmpire;
		} else if (englishString.ToLower () == "none") {
			value = StrategySet.None;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		strategySetTo [value] = languageString;
		toStrategySet [languageString] = value;
	}

	public void AddElect(string languageString) {
		AddElect (languageString, languageString);
	}
	public void AddElect(string languageString, string englishString) {
		string value;
		electTo [englishString] = languageString;
		toElect [languageString] = englishString;
	}

	public void AddRType(string languageString) {
		AddRType (languageString, languageString);
	}
	public void AddRType(string languageString, string englishString) {
		RType value;
		if (englishString.ToLower () == "bodyguard") {
			value = RType.Bodyguard;
		} else  if (englishString.ToLower () == "councilor") {
			value = RType.Councilor;
		} else if (englishString.ToLower () == "spy") {
			value = RType.Spy;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		rTypeTo [value] = languageString;
		toRType [languageString] = value;
	}

	public void AddOption(string languageString) {
		AddOption (languageString, languageString);
	}
	public void AddOption(string languageString, string englishString) {
		Option value;
		if (englishString.ToLower () == "distant suns") {
			value = Option.DistantSuns;
		} else  if (englishString.ToLower () == "the final frontier") {
			value = Option.TheFinalFrontier;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		optionTo [englishString] = languageString;
		toOption [languageString] = englishString;
	}

	public void AddLType(string languageString) {
		AddLType (languageString, languageString);
	}
	public void AddLType(string languageString, string englishString) {
		Leader value;
		if (englishString.ToLower () == "admiral") {
			value = LType.Admiral;
		} else  if (englishString.ToLower () == "agent") {
			value = LType.Agent;
		} else if (englishString.ToLower () == "diplomat") {
			value = LType.Diplomat;
		} else if (englishString.ToLower () == "general") {
			value = LType.General;
		} else if (englishString.ToLower () == "scientist") {
			value = LType.Scientist;
		} else {
			throw LocalizationException(languageString, englishString);
		}
		lTypeTo [value] = languageString;
		toLType [languageString] = value;
	}
}