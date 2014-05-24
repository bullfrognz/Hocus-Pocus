using UnityEngine;
using System.Collections;
using System;


public class CTeleport : CSpell
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:	
	override public bool Cast(Vector3 _vTargetPoint)
	{
		Vector3 vDirection = _vTargetPoint - transform.position;
//		Vector3 vTempDirection = vDirection;
		//vTempDirection.Normalize();
		
		float ActualDistance = vDirection.magnitude;
		
		if(ActualDistance <= m_fRange)
		{
			transform.position = _vTargetPoint;	
		}
		else
		{
			//vDirection.Normalize();
			transform.position = transform.position + ((_vTargetPoint - transform.position).normalized * m_fRange);
		}
				
		GetComponent<CWarlockAnimator>().NotifyCastSpell(CWarlockAnimator.ECastAnimation.RANDOM_SPIN);
		
		return (true);
	}
	
	
	// Private:
	
	
	override protected void Initialise(ref string _rsDescription, ref EType _reSpellType, ref float _rfCooldownLength, ref float _rfDamageLevelIncrement,
									   ref float _rfPushbackLevelIncrement, ref float _rfRange, ref uint _ruiCurrancyUpgradeCost, ref float _rfCastDelay)
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.TELEPORT);
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
	
	override public void CustomSpellCode()
	{
		
	}
	
	override public string GetShopUpgradeDetails()
	{
		return ("Range:		" + m_fRange + "	-> " + (m_fRange + m_fRangeLevelIncrement) + Environment.NewLine +  
				"Cool Down: 	" + m_fCooldownLength + "	-> " + (m_fCooldownLength + m_fCooldownTimerLevelIncrement) + Environment.NewLine);
	}
	
	// Events:
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
}
