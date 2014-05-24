// These buttons were created specifically for Hocus Procus button spritesheets
// The UVs will need to be changed to be used else where

using UnityEngine;
using System.Collections;

public class Button 
{
	private UnityGUIExt.GUI_ALLIGN m_eAllignment;
	private UnityGUIExt.GUI_ALLIGN m_eScreenAllignment;
	
	private const uint m_uiFrames = 3; // 0: Idle, 1: Mouseover, 2: Clicked
	private Texture[] m_tTextures = new Texture[m_uiFrames];
	private Rect m_rBox;
	private float m_fX = 0.0f;
	private float m_fY = 0.0f;
	private float m_fWidth = 0.0f;
	private float m_fHeight = 0.0f;
	private EState m_eState;
	string m_strText = "";
	
	enum EState
	{
		INVALID,
		IDLE = 0,
		MOUSE_OVER,
		MOUSE_CLICK,
		MAX
	}
	
	// Update is called once per frame
	public bool DoUpdate() 
	{
		if(IsMousedOver())
		{
			if(IsMouseDown())
			{
				m_eState= EState.MOUSE_CLICK;
			}
			else if(IsMouseClicked())
			{
				m_eState = EState.MOUSE_CLICK;
				
				return(true);
			}
			else
			{
				m_eState= EState.MOUSE_OVER;
			}
		}
		else
		{
			m_eState= EState.IDLE;
		}
		
		return(false);
	}
	
	public void DoGUI()
	{	
		DoGUI(UnityGUIExt.CreateRect(m_fX, m_fY, m_fWidth, m_fHeight, m_eScreenAllignment, m_eAllignment));
	}
	
	public void DoGUI(Rect _rRect)
	{	
		m_rBox = _rRect;
		
		if(m_tTextures[(int)m_eState] != null)
		{
			GUI.DrawTexture(m_rBox, m_tTextures[(int)m_eState]);
			
		
			//GUIStyle style = new GUIStyle();
			GUIStyle style = new GUIStyle(GUI.skin.label);
			style.alignment = TextAnchor.MiddleCenter;
			style.onNormal.textColor = Color.white; 
			GUI.Label(m_rBox, m_strText, style);
		}
		else
		{
			Debug.LogError("Button has not been assigned a texture");
		}
	}
	
	// This needs to be recalled if the screen changes
	public void Initialise(float _fX, float _fY, float _fWidth, float _fHeight, UnityGUIExt.GUI_ALLIGN _eScreenAllign, UnityGUIExt.GUI_ALLIGN _eAllign, string _sText)
	{
		m_fX = _fX;
		m_fY = _fY;
		m_fWidth = _fWidth;
		m_fHeight = _fHeight;
		m_eScreenAllignment = _eScreenAllign;
		m_eAllignment = _eAllign;
		m_strText = _sText;
	}
	
	public bool IsMousedOver()
	{
		return(m_rBox.Contains(GameApp.GetInstance().GetCursor().GetScreenPosition()));
	}
	
	bool IsMouseDown()
	{
		return(GameApp.GetInstance().GetCursor().IsMouseDown());
	}
	
	bool IsMouseClicked()
	{
		return(Input.GetMouseButtonUp(0));
	}
	
	bool IsPressed()
	{
		return(IsMousedOver() && IsMouseClicked());
	}
	
	public void SetMouseTextures(string _sIdle, string _sMouseOver, string _sMouseClicked)
	{
		SetIdleTexture(_sIdle);
		SetMouseOverTexture(_sMouseOver);
		SetMouseClickedTexture(_sMouseClicked);
	}
		
	public void SetIdleTexture(string _sTexture)
	{
		m_tTextures[(int)EState.IDLE] = LoadTexture(_sTexture);

	}
		
	public void SetMouseOverTexture(string _sTexture)
	{
		m_tTextures[(int)EState.MOUSE_OVER] = LoadTexture(_sTexture);
	}
	
	public void SetMouseClickedTexture(string _sTexture)
	{
		m_tTextures[(int)EState.MOUSE_CLICK] = LoadTexture(_sTexture);
	}
	
	Texture LoadTexture(string _sTexture)
	{
		// Note: Attempted to make this function a global static function but failed...
		
		Texture tTex = Resources.Load(_sTexture, typeof(Texture)) as Texture;
		
		if(tTex == null)
		{
			//renderer.enabled = false;
			Debug.LogError("Failed to load image: " + _sTexture);
			return null;
		}
		return(tTex);
	}
	
	
	public Rect GetRect()
	{
		return(m_rBox);
	}
}
