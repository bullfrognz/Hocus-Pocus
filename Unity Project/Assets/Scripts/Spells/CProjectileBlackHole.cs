using UnityEngine;
using System.Collections;


public class CProjectileBlackHole : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	// Private:
	
	
	void Start()
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.BLACKHOLE);
		
			CSpell oSpell = GameApp.GetInstance().GetWarlock().GetComponent<CSpellbook>().GetSpell(CSpell.EType.BLACKHOLE);
		
		m_fDamage = oSpell.GetDamageAmount();
		m_fVelocity = 6.0f;
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
				// = transform.forward * m_fVelocity * Time.deltaTime;
				Vector3 vTranslation = new Vector3();
//				Vector3 vNewPosition = new Vector3(transform.position.x + vTranslation.x,
//												  	1.0f,
//												   transform.position.z + vTranslation.z);
				//transform.position = vNewPosition;
				
				CheckDistance(vTranslation); //This is now a timer
				
				m_fTimer -= Time.deltaTime;
				if(m_fTimer <= 0.0f)
				{
					m_fTimer = m_fTimerTime;
					CheckCollision();
				}				
			}
			else
			{
				Explode();
			}
		}
	}
	
	void CheckDistance(Vector3 _vDeltaDistance)
	{
		//Once the time has run out explode
		m_fDistanceTraveled += Time.deltaTime;
		if(m_fDistanceTraveled >= m_fRange)
		{
			m_bExplode = true;
		}
	}
	
	void CheckCollision()
	{
		GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Wizard");		

	    foreach(GameObject thisObject in allObjects)
		{
			if (thisObject.networkView.isMine)
			{
				continue;
			}
			
			if(CollideWizard(thisObject))
			{				
				thisObject.GetComponent<CWarlockController>().SetSpeed(m_fPushBack);
				thisObject.GetComponent<CWarlockController>().TakeDamage(m_fDamage);
				thisObject.GetComponent<CWarlockController>().MoveTo(transform.position);
				
				thisObject.GetComponent<CWarlockController>().WarlockController_SetLastAttacker(GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId());
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
		m_fDuration += Time.deltaTime;
		
		if(m_fDuration < m_fMaxDuration)
		{
			Vector3 TempScale = new Vector3(1.0f, 1.0f, 1.0f);
			TempScale *= ((1 / (m_fMaxDuration / m_fDuration)) * m_fMaxSize);
			transform.localScale = TempScale;
		}
		else
		{
			GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Wizard");		

		    foreach(GameObject thisObject in allObjects)
			{
				if (thisObject.networkView.isMine)
				{
					continue;
				}	
				
				thisObject.GetComponent<CWarlockController>().SetSpeed(5.0f);
				thisObject.GetComponent<CWarlockController>().StopMoving(true);
				thisObject.GetComponent<CWarlockController>().IgnoreInput(false);
				
			}
			
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
	private float m_fTimer = 0.0f;
	private float m_fTimerTime = 0.1f;
	
	public float m_fVelocity = 10.0f;
	private float m_fPushBack = 0.1f;
		
	private float m_fRange = 22.0f;
	private float m_fDuration;
	private float m_fMaxDuration;
	private float m_fMaxSize;
	private float m_fRadius = 10.0f;
	
	protected float m_fDistanceTraveled = 0.0f;
	
	private bool m_bExplode = false;
	
	protected bool m_bUpdate = true;
	
	
	// Private:
	
	
}


