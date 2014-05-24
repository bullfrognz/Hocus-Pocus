using UnityEngine;
using System.Collections;
using System;

public class CSonicWave : CSpell
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	override public bool Cast(Vector3 _vTargetPoint)
	{
		Network.Instantiate(Resources.Load("Prefabs/SonicWave", typeof(GameObject)), _vTargetPoint, Quaternion.identity, 0);
		
		GetComponent<CWarlockAnimator>().NotifyCastSpell(CWarlockAnimator.ECastAnimation.RANDOM_RASE_STAFF);
		
		
		return (true);
	}
	
	
	// Private:	
	
	
	override protected void Initialise(ref string _rsDescription, ref EType _reSpellType, ref float _rfCooldownLength, ref float _rfDamageLevelIncrement,
									   ref float _rfPushbackLevelIncrement, ref float _rfRange, ref uint _ruiCurrancyUpgradeCost, ref float _rfCastDelay)
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.SONIC_WAVE);
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
	
	bool WithinRadius(GameObject _Object)
	{
		Vector3 Distance = transform.position - _Object.transform.position;
	
	    float sumRadius = m_fRange + 2.0f;
	    float sqrRadius = sumRadius * sumRadius;
	
	    float distSqr = (Distance.x * Distance.x) + (Distance.z * Distance.z);
	
	    if (distSqr <= sqrRadius)
	    {
	        return true;
	    }
	
	    return false;
	}
	
	protected override void ProcessCastInput()
	{
		if (m_bSelected)
		{
			m_bSelected = false;
			m_bCast = true;
			m_fNoInputDelayTimer = m_fCastDelay;
			//Debug.Log (m_fCastDelay);
			//Debug.Log (m_fNoInputDelayTimer);
			//transform.GetComponent<CSpellbook>().m_bIgnoreInput = true;
			transform.GetComponent<CWarlockMotor>().m_bIgnoreInput = true;
			transform.GetComponent<CWarlockMotor>().m_bReachedPath = true;
			GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetWarlock().GetComponent<CSpellbook>().m_bCanCast = false;
			GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetWarlock().GetComponent<CSpellbook>().m_bSpellSelected = false;
		}
	}
	
	override public void CustomSpellCode()
	{
		
	}
	
	protected override void ProcessCast()
	{
		if(m_bCast)
		{
			m_fNoInputDelayTimer -= Time.deltaTime;
			
			if(m_fNoInputDelayTimer <= 0.0f)
			{				
				m_bCast = false;
				m_fNoInputDelayTimer = m_fCastDelay;
				
				Cast(new Vector3(transform.position.x, 0.5f, transform.position.z));				
				
				ExecutePostCast();			
			}
		}
	}
	
	override public string GetShopUpgradeDetails()
	{
		return ("Cast Tine:		" + m_fCastDelay + "		-> " + (m_fCastDelay + m_fCastDelayLevelIncrement) + Environment.NewLine + 
				"Push:			" + m_fPushbackAmount + "	-> " + (m_fPushbackAmount + m_fPushbackLevelIncrement) + Environment.NewLine + 
				"Cool Down: 	" + m_fCooldownLength + "		-> " + (m_fCooldownLength + m_fCooldownTimerLevelIncrement) + Environment.NewLine);
	}
	// Events:
	
	
// Member Variables
	
	
	// Public:
	
	// Private:
	private float m_fNoInputDelayTimer;
	private bool  m_bCasted;
	
}
