using UnityEngine;
using System.Collections;


public class CProjectileHoming : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	// Private:
	
	
	void Start()
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.HOMING);
		CSpell oSpell = GameApp.GetInstance().GetWarlock().GetComponent<CSpellbook>().GetSpell(CSpell.EType.HOMING);
		m_fDamage = oSpell.GetDamageAmount();
		m_fVelocity = 8.0f;
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
				Seek();
				
				Vector3 vVelocity = transform.forward * m_fVelocity * Time.deltaTime;
				Vector3 vNewPosition = new Vector3(transform.position.x + vVelocity.x,
												   m_fRadius / 2.0f,
												   transform.position.z + vVelocity.z);
				transform.position = vNewPosition;
				
				CheckDistance(vVelocity);		
			}		
			else
			{
				Explode();
			}
		}		
	}
	
	void Seek()
	{
		if(!m_bUpdate)
		{
			m_bUpdate = true;
			return;	
		}
		
		m_bUpdate = false;
		
		bool bFoundWizard = false;
		float ClosestDistance = float.MaxValue;
		Vector3 ClosestPosition = new Vector3(0.0f, 0.0f, 0.0f);
		
		GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Wizard");		

	    foreach(GameObject thisObject in allObjects)
		{
			if (thisObject.networkView.isMine)
			{
				continue;
			}
			
			//Code goes here
			Vector3 vDistance = thisObject.transform.position - transform.position;
			float TempClosestDistance = vDistance.magnitude;
			
			if(TempClosestDistance <= ClosestDistance)
			{
				ClosestDistance = TempClosestDistance;
				ClosestPosition = thisObject.transform.position;
			}
			
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
				
		
		Vector2 vSelfPos = new Vector2(transform.position.x, transform.position.z);
		Vector3 vDirection = (ClosestPosition - transform.position).normalized;		
		
		Vector3 vTargeDirection = new Vector3(vDirection.x, 0.0f, vDirection.z);
 		Quaternion vTargetRotation = Quaternion.LookRotation(vTargeDirection);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, vTargetRotation, Time.deltaTime * m_fRotationVelocity);
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
		GameObject[] allObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];		

	    foreach(GameObject thisObject in allObjects)
		{
	    	if (thisObject.activeInHierarchy)
			{				
	    		if(thisObject.CompareTag("Wizard"))
				{	
					if (thisObject.networkView.isMine)
					{
						continue;
					}
					
					if(CollideWizard(thisObject))
					{
						m_bExplode = true;
						
						Vector2 vSelfPosition = new Vector2(transform.position.x, transform.position.z);
						Vector2 vTargetPosition = new Vector2(thisObject.transform.position.x, thisObject.transform.position.z);
						Vector2 vecPushBack2D = vTargetPosition - vSelfPosition;
						vecPushBack2D.Normalize();
						vecPushBack2D *= m_fPushBack;
						
						thisObject.GetComponent<CWarlockController>().WarlockController_ApplyPushback(vecPushBack2D.x, vecPushBack2D.y);	
						thisObject.GetComponent<CWarlockController>().TakeDamage((int)m_fDamage);
						
						thisObject.GetComponent<CWarlockController>().WarlockController_SetLastAttacker(GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId());
					}
				}
				
				if(((GameObject)thisObject).CompareTag("Spell"))
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
	private bool m_bUpdate = true;
	
	private float m_fDamage;
	public float m_fVelocity = 5.0f;
	private float m_fRotationVelocity = 137.0f;
	public float m_fPushBack;
	public bool  m_bExplode = false;
		
	public float m_fRange;
	public float m_fDuration;
	public float m_fMaxDuration;
	public float m_fMaxSize;
	float m_fRadius = 4.0f;
	
	protected float m_fDistanceTraveled = 0.0f;
	
	
	// Private:
	
	
}


