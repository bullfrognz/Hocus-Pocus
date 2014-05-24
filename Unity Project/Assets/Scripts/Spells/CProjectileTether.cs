using UnityEngine;
using System.Collections;


public class CProjectileTether : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	// Private:
	
	
	void Start()
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.TETHER);		
		CSpell oSpell = GameApp.GetInstance().GetWarlock().GetComponent<CSpellbook>().GetSpell(CSpell.EType.TETHER);
		
		m_fTimer = 0.0f;
		m_fTimerTime = 0.1f;
		
		m_fDamage = oSpell.GetDamageAmount();
		m_fPushBack = oSpell.GetPushbackAmount();
		m_fVelocity = 13.0f;
		
		m_bExplode = false;
		m_bHooked = false;
		
		m_fRange = oSpell.GetRange();
		m_fDuration = 0.0f;
		m_fMaxDuration = 6.2f;
		m_fMaxSize = 2.0f;
		m_fRadius = oSpell.GetRadius();
		
		m_fTimer = 0.1f;
		
		m_fDistanceTraveled = 0.0f;		
	}
	

	void Update()
	{
		if(networkView.isMine)
		{
			if(!m_bHooked)
			{
				Vector3 vTranslation = transform.forward * m_fVelocity * Time.deltaTime;
				Vector3 vNewPosition = new Vector3(transform.position.x + vTranslation.x,
												   0.25f,
												   transform.position.z + vTranslation.z);
				transform.position = vNewPosition;
				
				CheckDistance(vTranslation);
				CheckCollision();
			}
			else
			{
				if(!m_bExplode)
				{
					m_oTetheredWarlock.GetComponent<CWarlockController>().SetSpeed(m_fPushBack);					
					m_oTetheredWarlock.GetComponent<CWarlockController>().MoveTo(m_oOwnerWarlock.transform.position);
					
					CheckCollisionWarlock();
					
					transform.position = m_oTetheredWarlock.transform.position;
					
					m_fTimer -= Time.deltaTime;
					if(m_fTimer <= 0.0f)
					{
						m_oTetheredWarlock.GetComponent<CWarlockController>().TakeDamage(m_fDamage);
						m_fTimer = 0.1f;
					}
					
					m_fDuration -= Time.deltaTime;
					if(m_fDuration <= 0.0f)
					{
						m_bExplode = true;	
					}
				}
				else
				{
					Explode();
				}
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
			if (thisObject.networkView.isMine)
			{
				m_oOwnerWarlock = thisObject;
					
				continue;
			}
			
			if(CollideWizard(thisObject))
			{
				m_bHooked = true;
				
				m_oTetheredWarlock = thisObject;
				m_fDuration = m_fMaxDuration;
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
	void CheckCollisionWarlock()
	{
		GameObject[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
		foreach(GameObject thisObject in allObjects)
		{
		    if (thisObject.activeInHierarchy)
			{				
	    		if(thisObject.CompareTag("Wizard"))
				{	
					if (!thisObject.networkView.isMine)
					{
						continue;
					}
					
					if(CollideWizard(thisObject))
					{
						m_bExplode = true;
					}
				}				
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
		m_oTetheredWarlock.GetComponent<CWarlockController>().IgnoreInput(false);
		m_oTetheredWarlock.GetComponent<CWarlockController>().SetSpeed(5.0f);
		networkView.RPC("Kill", RPCMode.AllBuffered);				
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
	
	
	private float m_fTimer = 0.0f;
	private float m_fTimerTime = 0.1f;
	
	private float m_fDamage;
	private float m_fVelocity = 10.0f;
	private float m_fPushBack;
	private bool  m_bExplode = false;
	private bool  m_bHooked = false;
	
	private GameObject m_oTetheredWarlock;
	private GameObject m_oOwnerWarlock;
		
	private float m_fRange;
	private float m_fDuration;
	private float m_fMaxDuration;
	private float m_fMaxSize;
	private float m_fRadius;
	
	public float m_fDistanceTraveled = 0.0f;
	
	protected bool m_bUpdate = true;
	
	
	// Private:
	
	
}


