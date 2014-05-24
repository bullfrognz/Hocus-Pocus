using UnityEngine;
using System.Collections;
using System;


public class CSpellSwap : CSpell
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	// Protected:
	
	
	override protected void Initialise(ref string _rsDescription, ref EType _reSpellType, ref float _rfCooldownLength, ref float _rfDamageLevelIncrement,
									   ref float _rfPushbackLevelIncrement, ref float _rfRange, ref uint _ruiCurrancyUpgradeCost, ref float _rfCastDelay)
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.SWAP);
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
	
	protected override void ProcessCastInput()
	{
		if (m_bSelected &&
			Input.GetMouseButtonDown(0))
		{
			Vector3 m_vCastPosition = GameApp.GetInstance().GetCursor().Get3DPosition();
			Ray rayInfo = Camera.main.ScreenPointToRay(m_vCastPosition);
			if(rayInfo.direction.y < 0)
			{
				Vector3 hitPos = rayInfo.origin + (rayInfo.direction * ((-rayInfo.origin.y) / rayInfo.direction.y));
				Vector3 vTempMyPos = transform.GetComponent<CWarlockMotor>().transform.position;
				Vector3 vDistance = vTempMyPos - hitPos;
				float _fDistance = vDistance.magnitude;
				
				if(_fDistance < 0.0f) 
				{
					_fDistance *= -1.0f;	
				}
				
				if(_fDistance <= m_fRange)
				{
					BeginCast();
				}
			}
		}
	}
	
	
	override public bool Cast(Vector3 _vTargetPoint)
	{
		Debug.Log ("rogfl1");
		GameObject[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
	    foreach(GameObject thisObject in allObjects)
		{
	    	if (thisObject.activeInHierarchy)
			{
	    		if(thisObject.CompareTag("Wizard"))
				{
					Debug.Log ("rogfl2");
					if (thisObject.networkView.isMine)
					{
						continue;
					}
					Debug.Log ("rogfl3");
					Debug.Log(_vTargetPoint);
					//Add Code here
					if(FindWarlock (_vTargetPoint, thisObject))
					{	
						Debug.Log ("rogfl4");
						Vector3 vTempMyPos = transform.GetComponent<CWarlockMotor>().transform.position;
						transform.GetComponent<CWarlockMotor>().transform.position = thisObject.transform.position;
						thisObject.GetComponent<CWarlockController>().MoveWarlock(vTempMyPos);
						thisObject.GetComponent<CWarlockController>().AnimateWarlock((int)CWarlockAnimator.ECastAnimation.RANDOM_SPIN);						
					}
				}
			}
		}		
		
		GetComponent<CWarlockAnimator>().NotifyCastSpell(CWarlockAnimator.ECastAnimation.RANDOM_CAST);
		
		return (true);
	}
	
	bool FindWarlock(Vector3 _vCastPoint, GameObject _Wizard)
	{
		Vector3 Distance = _vCastPoint - _Wizard.transform.position;
		Debug.Log (Distance);
		Debug.Log (_vCastPoint);
		Debug.Log (_Wizard.transform.position);
	
	    float sumRadius = 4.0f;
	    float sqrRadius = sumRadius * sumRadius;
		Debug.Log(sqrRadius);
	
	    float distSqr = (Distance.x * Distance.x) + (Distance.z * Distance.z);
		Debug.Log (distSqr);
	
	    if (distSqr <= sqrRadius)
	    {
	        return true;
	    }
	
	    return false;
	}
	
	override public void CustomSpellCode()
	{
		
	}

	override  public string GetShopUpgradeDetails()
	{
		return ("Range:		" + m_fRange + "	-> " + (m_fRange + m_fRangeLevelIncrement) + Environment.NewLine +  
				"Cool Down: 	" + m_fCooldownLength + "	-> " + (m_fCooldownLength + m_fCooldownTimerLevelIncrement) + Environment.NewLine);
	}
	
	// Events:
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
}
