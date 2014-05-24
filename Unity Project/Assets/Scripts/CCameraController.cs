using UnityEngine;
using System.Collections;


public class CCameraController : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	public void FadeOutIn(float _fFadeOutDuration, float _fFadeInDuration)
	{
		m_fFadeInDuration = _fFadeInDuration;
		m_fFadeOutDuration = _fFadeOutDuration;
	 	m_fFadeInTimer = 0.0f;
	 	m_fFadeOutTimer = 0.0f;
		m_bFadeActive = true;
	}
	
	
	public void RunStartGamePan(float _fHeight, float _fInitialDelay, float _fAreanPanDuration, float _fArenaPanDistanceOffset, float _fWarlockPanDuration)
	{
		m_oWarlock = GameApp.GetInstance().GetWarlock();
		
		
		m_fPanArenaDistanceOffset = _fArenaPanDistanceOffset;
		
		
		m_fPanArenaDelayDuration = _fInitialDelay;
		m_fPanArenaDelayTimer = 0.0f;
		m_fPanArenaDuration = _fAreanPanDuration;
		m_fPanArenaTimer = 0.0f;
		m_fPanWarlockDuration = _fWarlockPanDuration;
		m_fPanWarlockTimer = 0.0f;
		m_bPanning = true;
		
		
		m_vPanWarlockTargetPosition = m_oWarlock.transform.position + m_vWarlockPositionOffset;
		
		
		m_vPanInitialPosition = new Vector3(0.0f, _fHeight, -_fArenaPanDistanceOffset);
		
		
		gameObject.GetComponent<CCursor>().SetSideScrollingEnabled(false);
	}
	
	
	public void CenterCameraOnWarlock()
	{
		transform.position = GameApp.GetInstance().GetWarlock().transform.position + m_vWarlockPositionOffset;
	}
	
	
	public void ResetToStartPosition()
	{
		transform.position = m_vStartPos;
		transform.rotation = m_vGameCameraRotation;
	}
	
	
	public void StopFadingPanning()
	{
		m_bFadeActive = false;
		m_bPanning = false;
	}
	
	
	public bool IsFadingOut()
	{
		return (m_fFadeOutTimer < m_fFadeOutDuration);
	}
	
	
	public bool IsFadingIn()
	{
		return (m_fFadeInTimer < m_fFadeInDuration);
	}
	
	
	public bool IsFadding()
	{
		return (m_bFadeActive);
	}
	
	
	public bool IsPanning()
	{
		return (m_bPanning);
	}
	
	
	// Private:
	
	
	void Awake()
	{
		gameObject.AddComponent<CCursor>();
	}
	
	
	void Start()
	{
		m_cFadeTexture = new Texture2D(1, 1);
		
		
		gameObject.camera.transform.position = gameObject.camera.transform.position + new Vector3(0.0f, 5.0f, 0.0f);
		m_vStartPos = gameObject.camera.transform.position;
		//FadeOutIn(2.0f, 5.0f);
		
	}
	

	void Update()
	{
		ProcessFade();
		ProcessPan();
	}
	
	
	void ProcessFade()
	{
		if (m_bFadeActive)
		{
			// Fading out
			if (m_fFadeOutTimer < m_fFadeOutDuration)
			{
				m_fFadeOutTimer += Time.deltaTime;
			}
			
			// Fading in
			else if (m_fFadeInTimer < m_fFadeInDuration)
			{
				m_fFadeInTimer += Time.deltaTime;
			}
			
			// Fade finished
			else
			{
				m_bFadeActive = false;	
			}
		}
	}
	
	
	void ProcessPan()
	{
		if (m_bPanning)
		{
			// Delay
			if (m_fPanArenaDelayTimer < m_fPanArenaDelayDuration)
			{
				m_fPanArenaDelayTimer += Time.deltaTime;
				
				
				if (m_fPanArenaDelayTimer > m_fPanArenaDelayDuration)
				{
					transform.position = m_vPanInitialPosition;
					
					
					transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
					m_vPanInitialRotation = transform.rotation;
				}
			}
			
			// Arena Pan
			else if (m_fPanArenaTimer < m_fPanArenaDuration)
			{
				m_fPanArenaTimer += Time.deltaTime;
				
				
				float fPanRatio = m_fPanArenaTimer / m_fPanArenaDuration;
				
				
				// Update position
				float fExtrudeX = Mathf.Sin((Mathf.PI * 2) * fPanRatio);
				float fExtrudeZ = Mathf.Cos((Mathf.PI * 2) * fPanRatio);
					
					
				transform.position = new Vector3(fExtrudeX * m_fPanArenaDistanceOffset, transform.position.y, fExtrudeZ * -m_fPanArenaDistanceOffset);
				transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
			}
			
			// Warlock Pan
			else if (m_fPanWarlockTimer < m_fPanWarlockDuration)
			{
				m_fPanWarlockTimer += Time.deltaTime;
				
				
				float fPanRatio = m_fPanWarlockTimer / m_fPanWarlockDuration;
				
				
				transform.rotation = Quaternion.Slerp(m_vPanInitialRotation, m_vGameCameraRotation, fPanRatio);
				transform.position = Vector3.Lerp(m_vPanInitialPosition, m_vPanWarlockTargetPosition, fPanRatio);
			}
			
			else
			{
				m_bPanning = false;
				gameObject.GetComponent<CCursor>().SetSideScrollingEnabled(true);
			}
		}
	}
	
	
	// Events:
	
	
	void OnGUI()
	{
		if (m_bFadeActive)
		{
			float fFadeRatio = 0.0f;
			
			
			// Fading out
			if (m_fFadeOutTimer < m_fFadeOutDuration)
			{
				fFadeRatio = m_fFadeOutTimer / m_fFadeOutDuration;
			}
			
			// Fading in
			else if (m_fFadeInTimer < m_fFadeInDuration)
			{
				fFadeRatio = m_fFadeInTimer / m_fFadeInDuration;
				fFadeRatio = 1.0f - fFadeRatio;
			}
			
			
			m_cFadeTexture.SetPixel(0, 0, new Color (0.0f, 0.0f, 0.0f, fFadeRatio));
			m_cFadeTexture.Apply();
		
		
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_cFadeTexture, ScaleMode.StretchToFill);
		}
	}
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	GameObject m_oWarlock;
	Texture2D m_cFadeTexture;
	
	
	Quaternion m_vGameCameraRotation = Quaternion.Euler(new Vector3(60.0f, 0.0f, 0.0f));
	Quaternion m_vPanInitialRotation;
	
	
	Vector3 m_vWarlockPositionOffset = new Vector3(0.0f, 16.0f, -7.0f);
	Vector3 m_vPanInitialPosition;
	Vector3 m_vPanWarlockTargetPosition;
	Vector3 m_vStartPos;
	
	
	float m_fFadeInDuration;
	float m_fFadeOutDuration;
	float m_fFadeInTimer;
	float m_fFadeOutTimer;
	
	float m_fPanArenaDistanceOffset;
	float m_fPanArenaDelayDuration;
	float m_fPanArenaDelayTimer;
	float m_fPanArenaDuration;
	float m_fPanArenaTimer;
	float m_fPanWarlockDuration;
	float m_fPanWarlockTimer;
	
	
	bool m_bFadeActive;
	bool m_bPanning;
	bool m_bOnce;
	
	
}
