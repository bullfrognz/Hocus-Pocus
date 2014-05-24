using UnityEngine;
using System.Collections;


public class CCursor : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	public void SetVisible(bool _bVisable)
	{
		if (!m_bVisable &&
			_bVisable)
		{
			m_vCursorPosition = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f);
		}
		
		
		m_bVisable = _bVisable;
	}
	
	
	
	public void SetSideScrollingEnabled(bool _bEnabled)
	{
		m_bSideScrolling = _bEnabled;
	}
	
	
	public void SetIconLit(bool _bLit)
	{
		m_bLit = _bLit;	
	}
	
	public Vector3 Get3DPosition()
	{
		return (m_vCursorPosition);
	}
	
	public Vector2 GetScreenPosition()
	{
		return(new Vector2(m_vCursorPosition.x, Screen.height - m_vCursorPosition.y));
	}
	
	public bool IsMouseDown()
	{
		return(m_bMouseDown);
	}
	
	
	public void HideCursor()
	{
		if (m_bFocued)
		{
			//Screen.lockCursor = false;
			Screen.lockCursor = true;
		}
	}
	
	
	// Private:
	
	
	void Start()
	{
		m_cCursorUnlitIcon = Resources.Load("Textures/Cursor_Unlit", typeof(Texture2D)) as Texture2D;
		m_cCursorLitIcon = Resources.Load("Textures/Cursor_Lit", typeof(Texture2D)) as Texture2D;

		
		m_vCursorPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
		
		
		HideCursor();
	}
	

	void Update()
	{
		HideCursor();
		
		
		ProcessCursorPosition();
		ProcessSideScrolling();
		
		if(Input.GetMouseButtonDown(0))
		{
			m_bMouseDown = true;
		}
		if(Input.GetMouseButtonUp(0))
		{
			m_bMouseDown = false;
		}
	}
	
	
	void ProcessCursorPosition()
	{
		float fNewX = m_vCursorPosition.x + Input.GetAxis("Mouse X") * m_fSensitivityX;
		float fNewY = m_vCursorPosition.y + Input.GetAxis("Mouse Y") * m_fSensitivityY;
		
		
		if (fNewX >= Screen.width - 4)
		{
			fNewX = Screen.width - 4;
		}
		else if (fNewX <= 0)
		{
			fNewX = 0;
		}
		
		
		if (fNewY >= Screen.height - 6)
		{
			fNewY = Screen.height - 6;
		}
		else if (fNewY <= 0)
		{
			fNewY = 0;
		}
		
		
		m_vCursorPosition.x = fNewX;
		m_vCursorPosition.y = fNewY;
	}
	
	
	void ProcessSideScrolling()
	{
		if (m_bSideScrolling)
		{
			float fTranslationX = 0.0f;
			float fTranslationZ = 0.0f;
			
			
			float fDetectionWidth  = Screen.width  * m_fSideDetectionRatio;
			float fDetectionHeight = Screen.height * m_fSideDetectionRatio;
			
			
			// South
			if (m_vCursorPosition.y >= Screen.height - fDetectionHeight &&
				m_vCursorPosition.y <= Screen.height)
			{
				fTranslationZ += m_fSideScrollingVelocity * Time.deltaTime;
			}
			
			
			// North
			if (m_vCursorPosition.y <= fDetectionHeight &&
				m_vCursorPosition.y >= 0)
			{
				fTranslationZ -= m_fSideScrollingVelocity * Time.deltaTime;
			}
			
			
			// West
			if (m_vCursorPosition.x <= fDetectionWidth &&
				m_vCursorPosition.x >= 0)
			{
				fTranslationX -= m_fSideScrollingVelocity * Time.deltaTime;
			}
			
			
			// East
			if (m_vCursorPosition.x >= Screen.width - fDetectionWidth &&
				m_vCursorPosition.x <= Screen.width)
			{
				fTranslationX += m_fSideScrollingVelocity * Time.deltaTime;
			}
			
			
			Vector3 vNewPosition = transform.position + new Vector3(fTranslationX, 0.0f, fTranslationZ);
			
			
			if (vNewPosition.x > -m_fMaxMinPositionX &&
				vNewPosition.x <  m_fMaxMinPositionX)
			{
				transform.position = transform.position + new Vector3(fTranslationX, 0.0f, 0.0f);
			}
			
			
			if (vNewPosition.z > m_fMinPositionZ &&
				vNewPosition.z < m_fMaxPositionZ)
			{
				transform.position = transform.position + new Vector3(0.0f, 0.0f, fTranslationZ);
			}
		}
	}
	
	
	void OnGUI()
	{
		if (m_bVisable)
		{
			if (m_bLit)
			{
				GUI.DrawTexture(new Rect(m_vCursorPosition.x, Screen.height - m_vCursorPosition.y, m_cCursorLitIcon.width, m_cCursorLitIcon.height), m_cCursorLitIcon);
			}
			else
			{
				GUI.DrawTexture(new Rect(m_vCursorPosition.x, Screen.height - m_vCursorPosition.y, m_cCursorUnlitIcon.width, m_cCursorUnlitIcon.height), m_cCursorUnlitIcon);
			}
		}
	}
	
	
	void OnApplicationFocus(bool _bFocus)
	{
		m_bFocued = _bFocus;
	}
	
	
	// Events:
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	Texture2D m_cCursorUnlitIcon;
	Texture2D m_cCursorLitIcon;
	
	
	Vector3 m_vCursorPosition;
	
	
	const float m_fSideDetectionRatio 	 = 0.02f;
	const float m_fSideScrollingVelocity = 25.0f;
	float m_fSensitivityX = 20.0f;
	float m_fSensitivityY = 20.0f;
	
	
	float m_fMaxMinPositionX = 20;
	float m_fMaxPositionZ = 15;
	float m_fMinPositionZ = -30;
	
	bool m_bSideScrolling = false;
	bool m_bLit;
	
	bool m_bMouseDown = false;
	bool m_bFocued = true;
	bool m_bVisable = true;
	
	
}
