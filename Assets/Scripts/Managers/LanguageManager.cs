using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LanguageManager : TIOMonoBehaviour {

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
	private Dictionary<string,string> toObjectiveName = new Dictionary<string,string>();
	private Dictionary<string,OReward> toOReward = new Dictionary<string,OReward>();
	private Dictionary<string,EType> toEType = new Dictionary<string,EType>();
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
	private Dictionary<string,string> objectiveNameTo = new Dictionary<string,string>();
	private Dictionary<OReward,string> oRewardTo = new Dictionary<OReward,string>();
	private Dictionary<EType,string> eTypeTo = new Dictionary<EType,string>();
	private Dictionary<RType,string> rTypeTo = new Dictionary<RType,string>();
	private Dictionary<Option,string> optionTo = new Dictionary<Option,string>();
	private Dictionary<LType,string> lTypeTo = new Dictionary<LType,string>();


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
