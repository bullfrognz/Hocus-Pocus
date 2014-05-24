using UnityEngine;
using System.Collections;


public class CProjectileMeteor : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	// Private:
	
	
	void Start()
	{
		CSpellData.TSpellData tData = GameApp.GetInstance().GetSpellDataComponent().GetSpellData(CSpell.EType.METEOR);
		CSpell oSpell = GameApp.GetInstance().GetWarlock().GetComponent<CSpellbook>().GetSpell(CSpell.EType.METEOR);
		m_fDamage = oSpell.GetDamageAmount();
		m_fVelocity = 18.0f;
		m_fPushBack = oSpell.GetPushbackAmount();
		m_bExplode = false;
		
		m_fRange = oSpell.GetRange();
		m_fDuration = 0.0f;
		m_fMaxDuration = 0.2f;
		m_fMaxSize = 8.0f;
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
												   transform.position.y + vTranslation.y,
												   transform.position.z + vTranslation.z);
				transform.position = vNewPosition;
				
				CheckDistance(vNewPosition);
				/*  Old Code  */
				//CheckCollision();
			}		
			else
			{
				CheckCollision();
				Explode();
			}
		}
	}
	
	void CheckDistance(Vector3 _vDeltaDistance)
	{
		if(_vDeltaDistance.y <= 0.0f)
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
		
		m_bUpdate = false;
		
		GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Wizard");		

	    foreach(GameObject thisObject in allObjects)
		{
			if (thisObject.networkView.isMine)
			{
				continue;
			}
			
			if(CollideWizard(thisObject))
			{
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
	//private static CSpellData Spell;
	//private static CSpellData.TSpellData SpellData = Spell.GetSpellData(CSpell.EType.METEOR);
	
	// Public:
	
	private float m_fDamage;
	public float m_fVelocity = 10.0f;
	private  float m_fPushBack = 0.5f;//SpellData.fPushback;
	private float m_fRange = 50.0f;//SpellData.fRange;
	private bool  m_bExplode = false;		
	
	public Vector3 m_vTargetPos;
	
	public float m_fDuration;
	public float m_fMaxDuration;
	public float m_fMaxSize;
	private float m_fRadius = 3.0f;
	
	protected float m_fDistanceTraveled = 0.0f;
	
	protected bool m_bUpdate = true;
	
	private float m_fTimer = 0.0f;
	private float m_fTimeTime = 0.1f;
	
	
	// Private:
	
	
}


