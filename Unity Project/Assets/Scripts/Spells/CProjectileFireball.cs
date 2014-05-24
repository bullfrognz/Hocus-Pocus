using UnityEngine;
using System.Collections;


public class CProjectileFireball : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	// Private:
	
	
	void Start()
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.FIREBALL);
		
		CSpell oSpell = GameApp.GetInstance().GetWarlock().GetComponent<CSpellbook>().GetSpell(CSpell.EType.FIREBALL);
		
		m_fDamage = oSpell.GetDamageAmount();
		m_fVelocity = 10.0f;
		m_fPushBack = oSpell.GetPushbackAmount();
		m_bExplode = false;
		
		m_fRange = oSpell.GetRange();
		m_fDuration = 0.0f;
		m_fMaxDuration = 0.2f;
		m_fMaxSize = 2.0f;
		m_fRadius = oSpell.GetRadius();
		
		m_fDistanceTraveled = 0.0f;	
	}
	

	void Update()
	{
		if(networkView.isMine)
		{
			if(!m_bExplode)
			{				
				Vector3 vTranslation = transform.forward * m_fVelocity * Time.deltaTime;
				Vector3 vNewPosition = new Vector3(transform.position.x + vTranslation.x,
												   m_fRadius / 2.0f,
												   transform.position.z + vTranslation.z);
				transform.position = vNewPosition;
				
				CheckDistance(vTranslation);
				CheckCollision();				
			}		
			else
			{
				Explode();
			}
		}
	}
	
	void CheckDistance(Vector3 _vDeltaDistance)
	{
		m_fDistanceTraveled += _vDeltaDistance.magnitude;
		if(m_fDistanceTraveled >= m_fRange)
		{
			m_bExplode = true;
		}
	}
	
	void CheckCollision()
	{
		if(!m_bUpdate)
		{
			return;	
		}
		
		m_bUpdate = true;
		
		GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Wizard");		

	    foreach(GameObject thisObject in allObjects)
		{
			// Don't Collide with self
			if (thisObject.networkView.isMine)
			{
				continue;
			}
			
			// Check against connected wizards
			if(CollideWizard(thisObject))
			{
				m_bExplode = true;
				
				
				Vector2 vSelfPosition = new Vector2(transform.position.x, transform.position.z);
				Vector2 vTargetPosition = new Vector2(thisObject.transform.position.x, thisObject.transform.position.z);
				Vector2 vecPushBack2D = vTargetPosition - vSelfPosition;
				vecPushBack2D.Normalize();
				vecPushBack2D *= m_fPushBack;
				
				thisObject.GetComponent<CWarlockController>().WarlockController_ApplyPushback(vecPushBack2D.x, vecPushBack2D.y);	
				thisObject.GetComponent<CWarlockController>().TakeDamage(m_fDamage);	
				
				// This works but only with the host vs. anyone else.
				//thisObject.GetComponent<CWarlockController>().SetLastAttacker(thisObject.networkView.owner);
				
				// This does not work
				// Tell this warlock that the owner of the fireball has hit them.
				//Debug.LogError ("Collison, Fireball Owner: " + networkView.owner.ToString());
				thisObject.GetComponent<CWarlockController>().WarlockController_SetLastAttacker(GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId());
			}
		}
		
		GameObject[] allSpells = GameObject.FindGameObjectsWithTag("Spell");		

	    foreach(GameObject thisObject in allSpells)
		{
			if (thisObject.networkView.isMine)
			{
				continue;
			}
			
			if(CollideSpell((GameObject)thisObject))
			{
				m_bExplode = true;
			}			
		}
    }	

	
	bool CollideWizard(GameObject _CurrentObject)
	{		
		bool bCollided = false;		
		
		Vector3 vDisplacement = transform.position - _CurrentObject.transform.position;
		float fDistance = vDisplacement.magnitude;		
		
		float TotalRadii = m_fRadius;
		fDistance -= TotalRadii;	
		
		if (fDistance < 0.0f)
		{
			bCollided = true;
		}		
		
		return (bCollided);		
	}
	
	bool CollideSpell(GameObject _CurrentObject)
	{
		bool bCollided = false;		
		
		Vector3 vDisplacement = transform.position - _CurrentObject.transform.position;
		float fDistance = vDisplacement.magnitude;		
		
		float TotalRadii = m_fRadius;
		fDistance -= TotalRadii;
		
		if (fDistance < 0.0f)
		{
			bCollided = true;
		}		
		
		return (bCollided);
	}
	

	void Explode()
	{
		m_fDuration += Time.deltaTime;
		
		if(m_fDuration < m_fMaxDuration)
		{
			Vector3 TempScale = new Vector3(1.0f, 1.0f, 1.0f);
			TempScale *= ((1 / (m_fMaxDuration / m_fDuration)) * m_fMaxSize);
			transform.localScale = TempScale;
		}
		else
		{
			networkView.RPC("Kill", RPCMode.AllBuffered);
		}		
	}
	
	[RPC]
	void Kill()
	{
		Destroy(this.gameObject);
	}
	
	
	// Events:
	

	
	// Update is called once per frame
	
	
	
// Member Variables
	
	
	// Public:
	
	private float m_fDamage;
	private float m_fVelocity = 10.0f;
	private float m_fPushBack;
	private bool  m_bExplode = false;
	protected bool m_bUpdate = true;
		
	private float m_fRange;
	private float m_fDuration;
	private float m_fMaxDuration;
	private float m_fMaxSize;
	private float m_fRadius = 2.0f;
	
	protected float m_fDistanceTraveled = 0.0f;
	
	
	// Private:
	
	
}


