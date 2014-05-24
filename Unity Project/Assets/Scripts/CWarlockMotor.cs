using UnityEngine;
using System.Collections;


public class CWarlockMotor : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	public void GoTo(Vector2 _vTargetPos)
	{
		m_vTargetPos = _vTargetPos;
		m_bReachedPath = false;
	}
	

	public void LookAt(Vector2 _vTargetPos)
	{
		Vector2 vSelfPos = new Vector2(transform.position.x, transform.position.z);
		Vector2 vDirection = (_vTargetPos - vSelfPos).normalized;
		
		
		Vector3 vTargeDirection = new Vector3(vDirection.x, 0.0f, vDirection.y);
 		m_vTargetRotation = Quaternion.LookRotation(vTargeDirection);
		m_bReachedRotation = false;
	}
	
	
	public void WarlockMotor_ApplyPushback(float _fX, float _fY)
	{
		if (enabled)
		{
			m_vPushbackDirection.x += _fX;
			m_vPushbackDirection.y += _fY;
			
			
			m_fPushBackDeltaTime = m_fPushbackTime;
		}
	}
	
	
	// Private:
	
	
	void Start()
	{
		// Empty	
	}
	

	void Update()
	{
		if (networkView.isMine)
		{
			//CheckCollision();
			ProcessInput();
			
			if (!m_bReachedRotation)
			{
				ProcessRotation();
			}
			
			if (!m_bReachedPath)
			{			
				ProcessMovement();				
				
				GetComponent<CWarlockAnimator>().NotifyMoving();
			}
			else
			{
				GetComponent<CWarlockAnimator>().NotifyMovingStop();
			}			
			
			UpdatePushback();
		}
	}
	
	
	void ProcessInput()
	{
		if(!m_bIgnoreInput)
		{
			if (Input.GetMouseButton(1))
			{
				Ray rayInfo = Camera.main.ScreenPointToRay(GameApp.GetInstance().GetCursor().Get3DPosition());
				if(rayInfo.direction.y < 0)
				{
					Vector3 moveToPos = rayInfo.origin + (rayInfo.direction * ((-rayInfo.origin.y) / rayInfo.direction.y));
					GoTo (new Vector2(moveToPos.x, moveToPos.z));
				}
				
				// OLD CODE
//				RaycastHit tHitInfo;
//	
//				
//				if (GameObject.Find("Floor").collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out tHitInfo, 10000.0f))
//				{
//					GoTo(new Vector2(tHitInfo.point.x, tHitInfo.point.z));
//				}
			}
		}
	}
	
	
	void ProcessRotation()
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, m_vTargetRotation, Time.deltaTime * m_fRotationVelocity);
		
		
		if (transform.rotation == m_vTargetRotation)
		{
			m_bReachedRotation = true;
		}
	}
	
	
	void ProcessMovement()
	{
		if (!m_bReachedPath)
		{
			Vector2 vSelfPos = new Vector2(transform.position.x, transform.position.z);
			Vector2 vDirection = (m_vTargetPos - vSelfPos).normalized;
			Vector2 vTranslation = vDirection * m_fMovementVelocity * Time.deltaTime;
			
			
			transform.position = new Vector3(vSelfPos.x + vTranslation.x, 
											 transform.position.y,
											 vSelfPos.y + vTranslation.y);
			
		
			LookAt(m_vTargetPos);
			UpdateReachedPath();
		}
	}
	
	
	void UpdateReachedPath()
	{
		if (!m_bReachedPath)
		{
			Vector2 vSelfPos = new Vector2(transform.position.x, transform.position.z);
			Vector2 vTargetDisplacement = vSelfPos - m_vTargetPos;
			
			
			if (vTargetDisplacement.magnitude < 0.4f)
			{
				m_bReachedPath = true;
			}
		}
	}
	
	void UpdatePushback()
	{
		if(m_fPushBackDeltaTime > 0)
		{
			m_fPushBackDeltaTime -= Time.deltaTime;
			float fPushBackPower = 1.0f / (m_fPushbackTime / m_fPushBackDeltaTime);
			m_vPushBack.x = (m_vPushbackDirection.x * fPushBackPower);
			m_vPushBack.y = (m_vPushbackDirection.y * fPushBackPower);
			//Debug.Log("PushBack");
		}
		else
		{
			m_vPushbackDirection.x = 0.0f;
			m_vPushbackDirection.y = 0.0f;
			m_vPushBack.x = 0.0f;
			m_vPushBack.y = 0.0f;
			//Debug.Log("No PushBack");
		}
		
		Vector2 vSelfPos = new Vector2(transform.position.x, transform.position.z);
		Vector2 vTranslation = m_vPushBack * m_fMovementVelocity * Time.deltaTime;			
			
		transform.position = new Vector3(vSelfPos.x + vTranslation.x, 
										 transform.position.y,
										 vSelfPos.y + vTranslation.y);			
	}
	
	
	// Events:
	
	
	void Awake()
	{
		m_oWarlockModel = transform.FindChild("TheWizard").gameObject;
	}
	
	
	void OnDisable()
	{
		GetComponent<CWarlockAnimator>().NotifyMovingStop();
		m_bReachedRotation = true;
		m_bReachedPath = true;
		
		m_vPushbackDirection = Vector2.zero;
		m_vPushBack = Vector2.zero;
	}

	
// Member Variables
	
	
	// Public:
	
	
	public float m_fMovementVelocity = 5.0f;
	public float m_fRotationVelocity = 360.0f;
	public float m_fRadius			 = 1.0f;
	
	public bool m_bReachedPath 		= true;
	public bool m_bReachedRotation 	= false;
	public bool m_bIgnoreInput		= false;
	
	
	// Private:
	
	
	GameObject m_oWarlockModel;
	
	
	Quaternion m_vTargetRotation;
	
	
	Vector2 m_vTargetPos;
	Vector2 m_vPushbackDirection;
	Vector2 m_vPushBack;
	float	m_fPushBackDeltaTime;
	float 	m_fPushbackTime = 3.0f;	
	
	CSpell Spell;
	
	
}
