using UnityEngine;
using System.Collections;


public class CWarlockHealth : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	[RPC]
	public void WarlockHealth_ApplyDamage(float _fAmount, bool _bTellOthers)
	{
		if (m_bTakeDamage &&
			GameApp.GetInstance().GetSceneArena().GetState() == CSceneArena.EState.FIGHTING)
		{
			if (_bTellOthers)
			{
				networkView.RPC("WarlockHealth_ApplyDamage", RPCMode.OthersBuffered, _fAmount, false);
			}
			
			
			m_fCurrentHealth -= _fAmount;
			
			
			if (m_bAlive &&
				m_fCurrentHealth < 0)
			{
				WarlockHealth_Kill(false); // Don't send to others because this will be called on all clients machines
			}
			else
			{
				m_bUpdateHealthTexture = true;
			}
		}
	}
	
	
	[RPC]
	public void WarlockHealth_Reset(bool _bTellOthers)
	{
		if (_bTellOthers)
		{
			networkView.RPC("WarlockHealth_Reset", RPCMode.OthersBuffered, false);
		}
		
		
		m_fCurrentHealth = m_fDefaultHealth;
		m_bAlive = true;
		m_bUpdateHealthTexture = true;
		transform.FindChild("Point light").light.enabled = true;
		m_vFallAcceleration = Vector3.zero;
	}
	
	
	[RPC]
	public void WarlockHealth_Kill(bool _bTellOthers)
	{
		if (_bTellOthers)
		{
			networkView.RPC("WarlockHealth_Kill", RPCMode.OthersBuffered, false);
		}
		
		
		m_fCurrentHealth = 0;
		m_bAlive = false;
		transform.FindChild("Point light").light.enabled = false;
		
		
		GameApp.GetInstance().GetSceneArena().GetComponent<CArenaAwards>().m_iPlayersDead++;
		
		if (networkView.isMine)
		{
			gameObject.GetComponent<CWarlockAnimator>().NotifyDeath();
			gameObject.GetComponent<CWarlockController>().ReportDeath();
			GetComponent<CWarlockMotor>().enabled = false;
			GetComponent<CSpellbook>().enabled = false;
		}	
	}
	
	
	[RPC]
	public void SetHealthBarVisable(bool _bVisable, bool _bTellOthers)
	{
		if (_bTellOthers)
		{
			networkView.RPC("SetHealthBarVisable", RPCMode.OthersBuffered, _bVisable, false);
		}
		
		
		m_bDrawTexture = _bVisable;
	}
	
	
	public bool IsAlive()
	{
		return (m_bAlive);
	}
	
	
	// Private:
	
	
	void Start()
	{
		m_cHealthBar = new Texture2D(m_iHealthTextureWidth, m_iHealthTextureHeight);
		WarlockHealth_Reset(false); // Will be called on all clients machines
	}
	

	void Update()
	{
		UpdateHealthBarTexture();
		ProcessOffArena();
	}
	
	
	void UpdateHealthBarTexture()
	{
		if (m_bUpdateHealthTexture)
		{
			float fHealthRemainingRatio = Mathf.Ceil((m_fCurrentHealth / m_fDefaultHealth) * 100) / 100;
			int iNumRedPixels = (int)(m_iHealthTextureWidth * fHealthRemainingRatio);
			int iHalfNumRedPixels = iNumRedPixels / 2;
			int iHalfTextureWidth = m_iHealthTextureWidth / 2;
			Color vPixelColour;
			
		
			for (int iX = 0; iX < m_iHealthTextureWidth; ++ iX)
			{
				if (iX > iHalfTextureWidth - iHalfNumRedPixels &&
					iX < iHalfTextureWidth + iHalfNumRedPixels)
				{
					vPixelColour = Color.red;
					vPixelColour.a = 0.8f;
				}
				else
				{
					vPixelColour = Color.black;
					vPixelColour.a = 0.4f;
				}
				
				
				for (int iY = 0; iY < m_iHealthTextureHeight; ++ iY)
				{
					m_cHealthBar.SetPixel(iX, iY, vPixelColour);
				}
			}
			
			
			m_cHealthBar.Apply();
			m_bUpdateHealthTexture = false;
		}
	}
	
	
	void ProcessOffArena()
	{
		if (networkView.isMine &&
			GameApp.GetInstance().GetSceneArena().GetState() == CSceneArena.EState.FIGHTING)
		{
			bool bOnArena = false;
			
			
			RaycastHit[] tHitsInfo = Physics.RaycastAll(transform.position + new Vector3(0.0f, 10.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f), 11.0f);

			
			for (int i = 0; i < tHitsInfo.Length; ++ i)
			{
				if (tHitsInfo[i].collider.gameObject.tag == "Tag_TerrainPiece")
				{
					bOnArena = true;
					break;
				}
			}
			
			
			if (!bOnArena)
			{

				GetComponent<CWarlockController>().TakeDamage(m_kfOffArenaDamageSecond * Time.deltaTime);
				
				
				// Make fall if dead
				if (!m_bAlive &&
					!GetComponent<CWarlockAnimator>().IsAnimating())
				{
					if (transform.position.y > -20.0f)
					{
						m_vFallAcceleration.y += m_fFallSpeed * Time.deltaTime;
							
						transform.position = transform.position + m_vFallAcceleration * Time.deltaTime;
					}
					else
					{
						//Hide him
						transform.position = new Vector3(1000.0f, 100.0f, 1000.0f);
					}
				}
			}
		}
	}
	
	
	// Events:
	
	
	void OnGUI()
	{
		if (m_bDrawTexture &&
			m_bAlive)
		{
			Vector3 vTargetPositon = transform.position;
			vTargetPositon.y += 2.0f;
			Vector3 vGuiPosition = Camera.main.WorldToViewportPoint(vTargetPositon);
			vGuiPosition.x = (Screen.width * vGuiPosition.x) - (m_cHealthBar.width / 2);
			vGuiPosition.y = Screen.height - (vGuiPosition.y * Screen.height);
			GUI.DrawTexture(new Rect(vGuiPosition.x, vGuiPosition.y, m_cHealthBar.width, m_cHealthBar.height), m_cHealthBar);
		}
	}

	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	Texture2D m_cHealthBar;
	
	
	Vector3 m_vFallAcceleration;
	
	
	const float m_kfOffArenaDamageSecond = 20;
	const float m_fDefaultHealth = 100.0f;
	float m_fFallSpeed = -10.0f;
	float m_fCurrentHealth;
	
	
	const int m_iHealthTextureWidth    = 70;
	const int m_iHealthTextureHeight   = 7;
	
	
	bool m_bAlive						= true;
	bool m_bUpdateHealthTexture			= true;
	bool m_bTakeDamage					= true;
	bool m_bDrawTexture					= false;
	
	
}
