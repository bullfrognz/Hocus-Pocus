using UnityEngine;
using System.Collections;
using System;

abstract public class CSpell : MonoBehaviour
{
	
// Member Types
	
	
	public enum EType
	{
		INVALID = -1,
		
		FIREBALL,
		HOMING,
		TELEPORT,
		METEOR,
		
		SONIC_WAVE, // Col 2 starts here
		SWAP, 
		TETHER,
		BLACKHOLE,
		
		SHEILD,
		SPELL_MAX
	}
	
	
// Member Functions
	
	
	// Public:
	
	
	public bool Select()
	{
		if (!IsOnCooldown())
		{
			m_bSelected = true;
		}		
		
		return (IsSelected());
	}
	
	public void Deselect()
	{
		m_bSelected = false;
	}
	
	
	public void Upgrade()
	{
		
		m_uiUpgradedLevel += 1;
		
		m_fPushbackAmount += m_fPushbackLevelIncrement;
		m_fDamageAmount += m_fDamageLevelIncrement;
		m_fCooldownLength += m_fCooldownTimerLevelIncrement;
		m_fCastDelay += m_fCastDelayLevelIncrement;
		m_fRange += m_fRangeLevelIncrement;
		m_fRadius += m_fRadiusLevelIncrement;
		
		m_fPushbackAmount = (float)Math.Round(m_fPushbackAmount, 2);
		m_fDamageAmount = (float)Math.Round(m_fDamageAmount, 2);
		m_fCooldownLength = (float)Math.Round(m_fCooldownLength, 2);
		m_fCastDelay = (float)Math.Round(m_fCastDelay, 2);
		m_fRange = (float)Math.Round(m_fRange, 2);
		m_fRadius = (float)Math.Round(m_fRadius, 2);
		
	}
	
	
	public EType GetSpellType()
	{
		return (m_eType);
	}
	
	
	public string GetDescription()
	{
		return (m_sDescription);
	}
	
	
	public float GetCooldownLength()
	{
		return (m_fCooldownLength);
	}
	
	
	public float GetCooldownTimer()
	{
		return (m_fCooldownTimer);
	}
	
	
	public uint GetUpgradedLevel()
	{
		return (m_uiUpgradedLevel);
	}
	
	
	public float GetDamageAmount()
	{
		return (m_fDamageAmount);
	}
	
	
	public float GetPushbackAmount()
	{
		return (m_fPushbackAmount);
	}
	
	
	public uint GetCurrencyBuyCost()
	{
		return (m_uiCurrencyBuyCost);
	}
	
	
	public uint GetCurrencyUpgradeCost()
	{
		return (m_uiCurrencyUpgradeCost);
	}
	
	public float GetRange()
	{
		return (m_fRange);
	}
	
	public float GetRadius()
	{
		return (m_fRadius);
	}
	
	public bool IsOnCooldown()
	{
		//return true;
		return (m_fCooldownTimer > 0);
	}
	
	public bool IsRangeInfanit()
	{
		return (m_fRange == 0.0f);
	}
	
	
	public bool IsSelected()
	{
		return (m_bSelected);
	}
	

	
	
	// Protected:
	

	abstract protected void Initialise(ref string _rsDescription, ref EType _reSpellType, ref float _rfCooldownLength, ref float _rfDamageLevelIncrement,
									   ref float _rfPushbackLevelIncrement, ref float _rfRange, ref uint _ruiCurrencyUpgradeCost, ref float _rfCastDelay);
	
	
	protected virtual void ProcessCastInput()
	{
		if (m_bSelected &&
			Input.GetMouseButtonDown(0))
		{
			BeginCast();
			//Debug.Log("casting..." + m_fCastDelay.ToString());
		}
	}
	
	
	protected void ProcessCooldown()
	{
		if (IsOnCooldown())
		{
			m_fCooldownTimer -= Time.deltaTime;
		}
		else 
		{
			m_fCooldownTimer = 0.0f;
		}
	}
	
	
	protected void ExecutePostCast()
	{
		m_fCooldownTimer = m_fCooldownLength;
		m_bSelected = false;
		transform.GetComponent<CWarlockMotor>().m_bIgnoreInput = false;
		m_bCast = false;
		GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetWarlock().GetComponent<CSpellbook>().m_bCanCast = true;
		GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetWarlock().GetComponent<CSpellbook>().m_bSpellSelected = false;
		//Debug.Log("Can cast");
	}
	
	protected void BeginCast()
	{
		m_bCast = true;
		transform.GetComponent<CWarlockMotor>().m_bReachedPath = true;
		transform.GetComponent<CWarlockMotor>().m_bIgnoreInput = true;
		
		m_fCastDelayTimer = m_fCastDelay;
		GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetWarlock().GetComponent<CSpellbook>().m_bCanCast = false;
		m_vCastPosition = GameApp.GetInstance().GetCursor().Get3DPosition();
		//Debug.Log("Cant cast");
	}
	
	
	// Private:
	
	
	void Start()
	{
		Initialise(ref m_sDescription, ref m_eType, ref m_fCooldownLength, ref m_fDamageLevelIncrement, ref m_fPushbackLevelIncrement, ref m_fRange, ref m_uiCurrencyUpgradeCost, ref m_fCastDelay);

		//Upgrade();
	}
	
	protected virtual void ProcessCast()
	{
		if(m_bCast)
		{
			m_fCastDelayTimer -= Time.deltaTime;
			if(m_fCastDelayTimer <= 0.0f)
			{
				Ray rayInfo = Camera.main.ScreenPointToRay(m_vCastPosition);
				if(rayInfo.direction.y < 0)
				{
					Vector3 hitPos = rayInfo.origin + (rayInfo.direction * ((-rayInfo.origin.y) / rayInfo.direction.y));
					transform.GetComponent<CWarlockMotor>().m_bReachedPath = true;
					Cast(hitPos);
					ExecutePostCast();
					
					
					GetComponent<CWarlockMotor>().LookAt(new Vector2(hitPos.x, hitPos.z));
				}
				
				// OLD CODE
//				RaycastHit tHitInfo;
//				if (GameObject.Find("Floor").collider.Raycast(Camera.main.ScreenPointToRay(m_vCastPosition), out tHitInfo, 10000.0f))
//				{
//					transform.GetComponent<CWarlockMotor>().m_bReachedPath = true;
//					Cast(tHitInfo.point);
//					
//					ExecutePostCast();
//				}
			}			
		}		
	}
	

	void Update()
	{
		if(GameApp.GetInstance().GetServer().GetGameMode() == Server.EGameMode.WTF)
		{
			m_fCooldownTimer = 0.0f;
		}
		ProcessCastInput();
		ProcessCooldown();
		CustomSpellCode();
		
		ProcessCast();
	}	
	
	abstract public void CustomSpellCode();	
	abstract public bool Cast(Vector3 _vTargetPoint);
	abstract public string GetShopUpgradeDetails();
			
	
	// Events:
	
	
// Member Variables
	
	
	// Protected:
	
	
	protected string m_sDescription;
	
	
	protected EType m_eType = EType.INVALID;
	
	protected Vector3 m_vCastPosition;
	
	protected float m_fCooldownLength;
	protected float m_fCooldownTimer;
	protected float m_fCooldownTimerLevelIncrement;
	protected float m_fCastDelay;
	protected float m_fCastDelayLevelIncrement;
	protected float m_fDamageAmount;
	protected float m_fDamageLevelIncrement;	
	protected float m_fPushbackAmount;
	protected float m_fPushbackLevelIncrement;
	protected float m_fRange;
	protected float m_fRangeLevelIncrement;
	public float m_fRadius;
	public float m_fRadiusLevelIncrement;
	
	protected float m_fCastDelayTimer;
	
	protected uint m_uiUpgradedLevel;
	protected uint m_uiCurrencyBuyCost;
	protected uint m_uiCurrencyUpgradeCost;
	
	protected bool m_bSelected;
	protected bool m_bCast;
	
	
	// Private:
	
	
}
