using UnityEngine;
using System.Collections;

public class CSonicWaveBall : MonoBehaviour {
	
	
	float m_fDuration;
	float m_fMaxDuration;
	float m_fMaxSize;
	float m_fPushbackAmount;
	float m_fDamage;
	float m_fRadius;
	
	private bool m_bCasting = true;
	
	private float m_fTimer = 0.0f;
	private float m_fTimeTime = 0.1f;
	
	// Use this for initialization
	void Start ()
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.SONIC_WAVE);
		CSpell oSpell = GameApp.GetInstance().GetWarlock().GetComponent<CSpellbook>().GetSpell(CSpell.EType.SONIC_WAVE);
		m_fDamage = oSpell.GetDamageAmount();
		m_fPushbackAmount = oSpell.GetPushbackAmount();
		
		m_fDuration = 0.0f;
		m_fMaxDuration = 0.53f;
		m_fMaxSize = 5.0f;
		m_fRadius = oSpell.GetRadius();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(networkView.isMine)
		{
			if(m_bCasting)
			{
				CheckCollision();
				
				m_bCasting = false;
			}
			else
			{
				m_fTimer += Time.deltaTime;
				if(m_fTimer >= m_fMaxDuration)
				{
					networkView.RPC("Kill_SonicWave", RPCMode.AllBuffered);
				}
			}
		}
	}
	
	[RPC]
	void Kill_SonicWave()
	{
		Destroy(this.gameObject);
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
						thisObject.GetComponent<CWarlockController>().TakeDamage(m_fDamage);
						//thisObject.GetComponent<CWarlockController>().WarlockController_SetLastAttacker(GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId());
						continue;
					}					
					
					if(CollideWizard(thisObject))
					{	
						Vector2 vSelfPosition = new Vector2(transform.position.x, transform.position.z);
						Vector2 vTargetPosition = new Vector2(thisObject.transform.position.x, thisObject.transform.position.z);
						Vector2 vecPushBack2D = vTargetPosition - vSelfPosition;
						vecPushBack2D.Normalize();
						vecPushBack2D *= m_fPushbackAmount;
						
						thisObject.GetComponent<CWarlockController>().WarlockController_ApplyPushback(vecPushBack2D.x, vecPushBack2D.y);
						thisObject.GetComponent<CWarlockController>().TakeDamage(m_fDamage);
						thisObject.GetComponent<CWarlockController>().WarlockController_SetLastAttacker(GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId());						
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
		
		//Debug.Log(fDistance);		
		
		if (fDistance < 0.0f)
		{
			bCollided = true;
		}		
		
		return (bCollided);		
	}	
}
