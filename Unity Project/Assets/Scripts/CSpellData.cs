using UnityEngine;
using System.Collections;
using System;

public class CSpellData : MonoBehaviour
{
	// Initial spell data is stored here so that it is accessable by the shop, and spellbook
	// This method was chosen as it was easier to iterate through
	// the spells in the shop and it is easier to update by changing the CSpell enum. 
	
	public struct TSpellData
	{
		public CSpell.EType eType;
		public string sTitle; 
		public string sDescription;
		public string sShopDetails; // Shown in shop if spell is not owned
		public float fCooldown; 
		public float fCooldownLevelIncrement;
		public float fCastDelay;
		public float fCastDelayLevelIncrement;
		public float fDamage; 
		public float fDamageLevelInrement; 	
		public float fPushback; 
		public float fPushbackLevelIncrement;
		public float fRange; 
		public float fRangeLevelIncrement;
		public float fRadius;	
		public float fRadiusLevelIncrement;
		public float fSpeed;	
		public float fSpeedLevelIncrement;
		public uint uiBuyCost; 	
		public uint uiUpgradeCost;
		public Texture tTexture;
		public Texture tNameTexture;
	}
	
	TSpellData[] m_atSpells = new TSpellData[(int)CSpell.EType.SPELL_MAX];
	
	void Start () 
	{
		// Populate spells with default data
		
		// Fireball
		int iSpell = (int)CSpell.EType.FIREBALL;
		m_atSpells[iSpell].eType = CSpell.EType.FIREBALL;
		m_atSpells[iSpell].sTitle 					= "Fire Ball";
		m_atSpells[iSpell].sDescription				= "Unleash a fiery projectile that explodes on contact and knock opponents back.";
		m_atSpells[iSpell].fCooldown 				= 3.5f;
		m_atSpells[iSpell].fCooldownLevelIncrement	= -0.2f;
		m_atSpells[iSpell].fCastDelay 				= 0.05f;
		m_atSpells[iSpell].fCastDelayLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fDamage 					= 2.0f;
		m_atSpells[iSpell].fDamageLevelInrement 	= 1.0f;
		m_atSpells[iSpell].fPushback 				= 1.5f;
		m_atSpells[iSpell].fPushbackLevelIncrement	= 0.15f;
		m_atSpells[iSpell].fRange 					= 22.0f;
		m_atSpells[iSpell].fRangeLevelIncrement		= 2.0f;
		m_atSpells[iSpell].fRadius 					= 1.5f;
		m_atSpells[iSpell].fRadiusLevelIncrement 	= 0.1f;
		m_atSpells[iSpell].fSpeed 					= 0.0f;
		m_atSpells[iSpell].fSpeedLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].uiBuyCost 				= 5;
		m_atSpells[iSpell].uiUpgradeCost 			= 4;
		m_atSpells[iSpell].tTexture	 				= Resources.Load("UI/Spell Icons/Spell_Fireball", typeof(Texture)) as Texture;
		m_atSpells[iSpell].tNameTexture	 			= Resources.Load("UI/Spell Titles/Text_Fireball", typeof(Texture)) as Texture;
		m_atSpells[iSpell].sShopDetails	 			=   "Damage: " + m_atSpells[iSpell].fDamage + Environment.NewLine +
														"Push: " + m_atSpells[iSpell].fPushback + Environment.NewLine +
														"Cool Down: " + m_atSpells[iSpell].fCooldown;

		// Sonic Wave
		iSpell = (int)CSpell.EType.SONIC_WAVE;
		m_atSpells[iSpell].eType = CSpell.EType.SONIC_WAVE;
		m_atSpells[iSpell].sTitle 					= "Sonic Wave";
		m_atSpells[iSpell].sDescription				= "After a short cast time, release a localised wave to knock back opponents.";
		m_atSpells[iSpell].fCooldown 				= 10.0f;
		m_atSpells[iSpell].fCooldownLevelIncrement	= -0.25f;
		m_atSpells[iSpell].fCastDelay 				= 1.0f;
		m_atSpells[iSpell].fCastDelayLevelIncrement	= -0.05f;
		m_atSpells[iSpell].fDamage 					= 10.0f;
		m_atSpells[iSpell].fDamageLevelInrement 	= 2.0f;
		m_atSpells[iSpell].fPushback 				= 1.75f;
		m_atSpells[iSpell].fPushbackLevelIncrement	= 0.25f;
		m_atSpells[iSpell].fRange 					= 5.0f;
		m_atSpells[iSpell].fRangeLevelIncrement		= 1.0f;
		m_atSpells[iSpell].fRadius 					= 4.0f;
		m_atSpells[iSpell].fRadiusLevelIncrement 	= 0.5f;
		m_atSpells[iSpell].fSpeed 					= 0.0f;
		m_atSpells[iSpell].fSpeedLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].uiBuyCost 				= 5;
		m_atSpells[iSpell].uiUpgradeCost 			= 4;
		m_atSpells[iSpell].tTexture	 				= Resources.Load("UI/Spell Icons/Spell_Wave", typeof(Texture)) as Texture;
		m_atSpells[iSpell].tNameTexture	 			= Resources.Load("UI/Spell Titles/Text_Wave", typeof(Texture)) as Texture;
		m_atSpells[iSpell].sShopDetails	 			=   "Cast Time: " + m_atSpells[iSpell].fCastDelay + Environment.NewLine +
														"Push: " + m_atSpells[iSpell].fPushback + Environment.NewLine +
														"Cool Down: " + m_atSpells[iSpell].fCooldown;
		// Teleport
		iSpell = (int)CSpell.EType.TELEPORT;
		m_atSpells[iSpell].eType = CSpell.EType.TELEPORT;
		m_atSpells[iSpell].sTitle 					= "Teleport";
		m_atSpells[iSpell].sDescription				= "Teleport to desired a location.";
		m_atSpells[iSpell].fCooldown 				= 13.0f;
		m_atSpells[iSpell].fCooldownLevelIncrement	= -0.5f;
		m_atSpells[iSpell].fCastDelay 				= 0.003f;
		m_atSpells[iSpell].fCastDelayLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fDamage 					= 0.0f;
		m_atSpells[iSpell].fDamageLevelInrement 	= 0.0f;
		m_atSpells[iSpell].fPushback 				= 0.0f;
		m_atSpells[iSpell].fPushbackLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fRange 					= 10.0f;
		m_atSpells[iSpell].fRangeLevelIncrement		= 3.0f;
		m_atSpells[iSpell].fRadius 					= 0.0f;
		m_atSpells[iSpell].fRadiusLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].fSpeed 					= 0.0f;
		m_atSpells[iSpell].fSpeedLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].uiBuyCost 				= 8;
		m_atSpells[iSpell].uiUpgradeCost 			= 7;
		m_atSpells[iSpell].tTexture	 				= Resources.Load("UI/Spell Icons/Spell_Teleport", typeof(Texture)) as Texture;
		m_atSpells[iSpell].tNameTexture	 			= Resources.Load("UI/Spell Titles/Text_Teleport", typeof(Texture)) as Texture;
		m_atSpells[iSpell].sShopDetails	 			=   "Range: " + m_atSpells[iSpell].fRange + Environment.NewLine +
														"Cool Down: " + m_atSpells[iSpell].fCooldown;
		
		// Swap
		iSpell = (int)CSpell.EType.SWAP;
		m_atSpells[iSpell].eType = CSpell.EType.SWAP;
		m_atSpells[iSpell].sTitle 					= "Swap";
		m_atSpells[iSpell].sDescription				= "Switch places with an opponent.";
		m_atSpells[iSpell].fCooldown 				= 9.0f;
		m_atSpells[iSpell].fCooldownLevelIncrement	= -0.5f;
		m_atSpells[iSpell].fCastDelay 				= 0.05f;
		m_atSpells[iSpell].fCastDelayLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fDamage 					= 0.0f;
		m_atSpells[iSpell].fDamageLevelInrement 	= 0.0f;
		m_atSpells[iSpell].fPushback 				= 0.0f;
		m_atSpells[iSpell].fPushbackLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fRange 					= 14.0f;
		m_atSpells[iSpell].fRangeLevelIncrement		= 2.5f;
		m_atSpells[iSpell].fRadius 					= 0.0f;
		m_atSpells[iSpell].fRadiusLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].fSpeed 					= 0.0f;
		m_atSpells[iSpell].fSpeedLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].uiBuyCost 				= 10;
		m_atSpells[iSpell].uiUpgradeCost 			= 5;
		m_atSpells[iSpell].tTexture	 				= Resources.Load("UI/Spell Icons/Spell_Swap", typeof(Texture)) as Texture;
		m_atSpells[iSpell].tNameTexture	 			= Resources.Load("UI/Spell Titles/Text_Swap", typeof(Texture)) as Texture;
		m_atSpells[iSpell].sShopDetails	 			=   "Range: " + m_atSpells[iSpell].fRange + Environment.NewLine +
														"Cool Down: " + m_atSpells[iSpell].fCooldown;
		
		// Homing
		iSpell = (int)CSpell.EType.HOMING;
		m_atSpells[iSpell].eType = CSpell.EType.HOMING;
		m_atSpells[iSpell].sTitle 					= "Homing";
		m_atSpells[iSpell].sDescription				= "Cast a projectile that will chase the closest target and knock back opponents.";
		m_atSpells[iSpell].fCooldown 				= 10.0f;
		m_atSpells[iSpell].fCooldownLevelIncrement	= -0.5f;
		m_atSpells[iSpell].fCastDelay 				= 0.05f;
		m_atSpells[iSpell].fCastDelayLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fDamage 					= 2.5f;
		m_atSpells[iSpell].fDamageLevelInrement 	= 0.5f;
		m_atSpells[iSpell].fPushback 				= 1.3f;
		m_atSpells[iSpell].fPushbackLevelIncrement	= 0.2f;
		m_atSpells[iSpell].fRange 					= 60.0f;
		m_atSpells[iSpell].fRangeLevelIncrement		= 10.0f;
		m_atSpells[iSpell].fRadius 					= 1.5f;
		m_atSpells[iSpell].fRadiusLevelIncrement 	= 0.1f;
		m_atSpells[iSpell].fSpeed 					= 0.0f;
		m_atSpells[iSpell].fSpeedLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].uiBuyCost 				= 7;
		m_atSpells[iSpell].uiUpgradeCost 			= 8;
		m_atSpells[iSpell].tTexture	 				= Resources.Load("UI/Spell Icons/Spell_Homing", typeof(Texture)) as Texture;
		m_atSpells[iSpell].tNameTexture	 			= Resources.Load("UI/Spell Titles/Text_Homing", typeof(Texture)) as Texture;
		m_atSpells[iSpell].sShopDetails	 			=   "Damage: " + m_atSpells[iSpell].fDamage + Environment.NewLine +
														"Push: " + m_atSpells[iSpell].fPushback + Environment.NewLine +
														"Cool Down: " + m_atSpells[iSpell].fCooldown;
		
		// Meteor
		iSpell = (int)CSpell.EType.METEOR;
		m_atSpells[iSpell].eType = CSpell.EType.METEOR;
		m_atSpells[iSpell].sTitle 					= "Meteor";
		m_atSpells[iSpell].sDescription				= "Call a huge ball of fire to fall from the sky causing huge damange and knock back.";
		m_atSpells[iSpell].fCooldown 				= 13.0f;
		m_atSpells[iSpell].fCooldownLevelIncrement	= -0.5f;
		m_atSpells[iSpell].fCastDelay 				= 0.5f;
		m_atSpells[iSpell].fCastDelayLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fDamage 					= 10.5f;
		m_atSpells[iSpell].fDamageLevelInrement 	= 2.2f;
		m_atSpells[iSpell].fPushback 				= 2.2f;
		m_atSpells[iSpell].fPushbackLevelIncrement	= 0.35f;
		m_atSpells[iSpell].fRange 					= 22.0f;
		m_atSpells[iSpell].fRangeLevelIncrement		= 1.0f;
		m_atSpells[iSpell].fRadius 					= 3.5f;
		m_atSpells[iSpell].fRadiusLevelIncrement 	= 0.15f;
		m_atSpells[iSpell].fSpeed 					= 0.0f;
		m_atSpells[iSpell].fSpeedLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].uiBuyCost 				= 9;
		m_atSpells[iSpell].uiUpgradeCost 			= 8;
		m_atSpells[iSpell].tTexture	 				= Resources.Load("UI/Spell Icons/Spell_Meteor", typeof(Texture)) as Texture;
		m_atSpells[iSpell].tNameTexture	 			= Resources.Load("UI/Spell Titles/Text_Meteor", typeof(Texture)) as Texture;
		m_atSpells[iSpell].sShopDetails	 			=   "Damage: " + m_atSpells[iSpell].fDamage + Environment.NewLine +
														"Push: " + m_atSpells[iSpell].fPushback + Environment.NewLine +
														"Cool Down: " + m_atSpells[iSpell].fCooldown;
		
		// Tether
		iSpell = (int)CSpell.EType.TETHER;
		m_atSpells[iSpell].eType = CSpell.EType.TETHER;
		m_atSpells[iSpell].sTitle 					= "Tether";
		m_atSpells[iSpell].sDescription				= "Force opponents to move toward you.";
		m_atSpells[iSpell].fCooldown 				= 15.0f;
		m_atSpells[iSpell].fCooldownLevelIncrement	= -0.0f;
		m_atSpells[iSpell].fCastDelay 				= 0.05f;
		m_atSpells[iSpell].fCastDelayLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fDamage 					= 0.1f; //Damage is done 10 times per second
		m_atSpells[iSpell].fDamageLevelInrement 	= 0.15f;
		m_atSpells[iSpell].fPushback 				= 6.0f; // Used for pull amount (speed of warlock 5.0f is default)
		m_atSpells[iSpell].fPushbackLevelIncrement	= -0.5f;
		m_atSpells[iSpell].fRange 					= 12.0f;
		m_atSpells[iSpell].fRangeLevelIncrement		= 1.0f;
		m_atSpells[iSpell].fRadius 					= 2.5f;
		m_atSpells[iSpell].fRadiusLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].fSpeed 					= 0.0f;
		m_atSpells[iSpell].fSpeedLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].uiBuyCost 				= 6;
		m_atSpells[iSpell].uiUpgradeCost 			= 4;
		m_atSpells[iSpell].tTexture	 				= Resources.Load("UI/Spell Icons/Spell_Tether", typeof(Texture)) as Texture;
		m_atSpells[iSpell].tNameTexture	 			= Resources.Load("UI/Spell Titles/Text_Tether", typeof(Texture)) as Texture;
		m_atSpells[iSpell].sShopDetails	 			=   "Pull: " + m_atSpells[iSpell].fPushback + Environment.NewLine +
														"Duration: " + m_atSpells[iSpell].fDamage + Environment.NewLine +
														"Cool Down: " + m_atSpells[iSpell].fCooldown;
		
		// Blackhole
		iSpell = (int)CSpell.EType.BLACKHOLE;
		m_atSpells[iSpell].eType = CSpell.EType.BLACKHOLE;
		m_atSpells[iSpell].sTitle 					= "Black Hole";
		m_atSpells[iSpell].sDescription				= "Opponents within the radius of the Back Hole will be forced toward it's centre.";
		m_atSpells[iSpell].fCooldown 				= 12.0f;
		m_atSpells[iSpell].fCooldownLevelIncrement	= -0.5f;
		m_atSpells[iSpell].fCastDelay 				= 1.0f;
		m_atSpells[iSpell].fCastDelayLevelIncrement	= -0.05f;
		m_atSpells[iSpell].fDamage 					= 0.15f;
		m_atSpells[iSpell].fDamageLevelInrement 	= 0.05f; 
		m_atSpells[iSpell].fPushback 				= 1.5f;
		m_atSpells[iSpell].fPushbackLevelIncrement	= 0.025f; // Used for pull amount
		m_atSpells[iSpell].fRange 					= 3.015f;// Duration of the spell
		m_atSpells[iSpell].fRangeLevelIncrement		= 0.51f;
		m_atSpells[iSpell].fRadius 					= 5.005f;
		m_atSpells[iSpell].fRadiusLevelIncrement 	= 0.5f;
		m_atSpells[iSpell].fSpeed 					= 0.0f;
		m_atSpells[iSpell].fSpeedLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].uiBuyCost 				= 9;
		m_atSpells[iSpell].uiUpgradeCost 			= 8;
		m_atSpells[iSpell].tTexture	 				= Resources.Load("UI/Spell Icons/Spell_Blackhole", typeof(Texture)) as Texture;
		m_atSpells[iSpell].tNameTexture	 			= Resources.Load("UI/Spell Titles/Text_Blackhole", typeof(Texture)) as Texture;
		m_atSpells[iSpell].sShopDetails	 			=   "Pull: " + m_atSpells[iSpell].fPushback + Environment.NewLine +
														"Duration: " + m_atSpells[iSpell].fDamage + Environment.NewLine +
														"Cool Down: " + m_atSpells[iSpell].fCooldown;
		
		// Spell 8
		iSpell = (int)CSpell.EType.SHEILD;
		m_atSpells[iSpell].eType = CSpell.EType.SHEILD;
		m_atSpells[iSpell].sTitle 					= "SPELL8";
		m_atSpells[iSpell].sDescription				= "Unleash a fiery projectile that explodes on contact";
		m_atSpells[iSpell].sShopDetails	 			= "Please Fill";
		m_atSpells[iSpell].fCooldown 				= 2.5f;
		m_atSpells[iSpell].fCooldownLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fCastDelay 				= 0.05f;
		m_atSpells[iSpell].fCastDelayLevelIncrement	= 0.0f;
		m_atSpells[iSpell].fDamage 					= 2.0f;
		m_atSpells[iSpell].fDamageLevelInrement 	= 2.0f;
		m_atSpells[iSpell].fPushback 				= 5.0f;
		m_atSpells[iSpell].fPushbackLevelIncrement	= 5.0f;
		m_atSpells[iSpell].fRange 					= 22.0f;
		m_atSpells[iSpell].fRangeLevelIncrement		= 1.0f;
		m_atSpells[iSpell].fRadius 					= 1.5f;
		m_atSpells[iSpell].fRadiusLevelIncrement 	= 0.1f;
		m_atSpells[iSpell].fSpeed 					= 0.0f;
		m_atSpells[iSpell].fSpeedLevelIncrement 	= 0.0f;
		m_atSpells[iSpell].uiBuyCost 				= 5;
		m_atSpells[iSpell].uiUpgradeCost 			= 4;
		m_atSpells[iSpell].tTexture	 				= Resources.Load("UI/Spell Icons/Spell_Swap", typeof(Texture)) as Texture;
		m_atSpells[iSpell].tNameTexture	 			= Resources.Load("UI/Spell Titles/Spell_Swap", typeof(Texture)) as Texture;
		m_atSpells[iSpell].sShopDetails	 			=   "Pull: " + m_atSpells[iSpell].fPushback + Environment.NewLine +
														"Duration: " + m_atSpells[iSpell].fDamage + Environment.NewLine +
														"Cool Down: " + m_atSpells[iSpell].fCooldown;
	}
	
	public TSpellData GetSpellData(CSpell.EType _eSpellType)
	{
		return(m_atSpells[(int)_eSpellType]);
	}
	
	public TSpellData[] GetAllSpellData()
	{
		return(m_atSpells);
	}

	void Update () 
	{
	
	}
}
