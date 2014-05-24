using UnityEngine;
using System.Collections;
using System;


public class CSpellHoming : CSpell
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	// Protected:
	
	
	override protected void Initialise(ref string _rsDescription, ref EType _reSpellType, ref float _rfCooldownLength, ref float _rfDamageLevelIncrement,
									   ref float _rfPushbackLevelIncrement, ref float _rfRange, ref uint _ruiCurrancyUpgradeCost, ref float _rfCastDelay)
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.HOMING);
		_rsDescription = tData.sDescription;
		_reSpellType = tData.eType;
		_rfCooldownLength = tData.fCooldown;
		_rfDamageLevelIncrement = tData.fDamageLevelInrement;
		_rfPushbackLevelIncrement = tData.fPushbackLevelIncrement;
		_rfRange = tData.fRange;
		_ruiCurrancyUpgradeCost = tData.uiUpgradeCost;
		_rfCastDelay = tData.fCastDelay;
		m_fRadius = tData.fRadius;
		
				m_fCooldownLength				= tData.fCooldown;
		m_fCooldownTimerLevelIncrement	= tData.fCooldownLevelIncrement;
		m_fCastDelay					= tData.fCastDelay;
		m_fCastDelayLevelIncrement		= tData.fCastDelayLevelIncrement;
		m_fDamageAmount					= tData.fDamage;
		m_fDamageLevelIncrement			= tData.fDamageLevelInrement;	
		m_fPushbackAmount				= tData.fPushback;
		m_fPushbackLevelIncrement		= tData.fPushbackLevelIncrement;
		m_fRange						= tData.fRange;
		m_fRangeLevelIncrement			= tData.fRangeLevelIncrement;
		m_fRadius						= tData.fRadius;
		m_fRadiusLevelIncrement			= tData.fRadiusLevelIncrement;
	}
	
	
	// Private:
	
	
	override public bool Cast(Vector3 _vTargetPoint)
	{
		Vector3 TempLook = _vTargetPoint - transform.FindChild("TheWizard").transform.position;		
		GameObject oHomingBall = Network.Instantiate(Resources.Load("Prefabs/Homing", typeof(GameObject)), transform.FindChild("TheWizard").transform.position, Quaternion.LookRotation(TempLook), 0) as GameObject;
		
		oHomingBall.GetComponent<CProjectileHoming>().m_fPushBack = m_fPushbackAmount;
		oHomingBall.GetComponent<CProjectileHoming>().m_fRange = m_fRange;
		
		
		GetComponent<CWarlockAnimator>().NotifyCastSpell(CWarlockAnimator.ECastAnimation.RANDOM_CAST);
		
		return (true);
	}
	
	override public void CustomSpellCode()
	{
		
	}
	
	override public string GetShopUpgradeDetails()
	{
		return ("Damage:		" + m_fDamageAmount + "	-> " + (m_fDamageAmount + m_fDamageLevelIncrement) + Environment.NewLine + 
				"Push:			" + m_fPushbackAmount + "		-> " + (m_fPushbackAmount + m_fPushbackLevelIncrement) + Environment.NewLine + 
				"Cool Down: 	" + m_fCooldownLength + "		-> " + (m_fCooldownLength + m_fCooldownTimerLevelIncrement) + Environment.NewLine);
	}
	// Events:
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
}
