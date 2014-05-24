using UnityEngine;
using System.Collections;

public class Scrollbar 
{
	private Vector2 m_vPosition;
	private float m_fLongLength; // The long part
	private float m_fSmallLength; // The smaller part
	private Rect m_rectLong;
	private Rect m_rectSmall;
	private float m_fX;
	private float m_fY;
	private bool m_bSelected;
	private CCursor m_oCursor;
	Rect m_rectParent;
	Rect m_rectView;
	
	Texture m_tBackground = Resources.Load("UI/Background", typeof(Texture)) as Texture;
	Texture m_tBar = Resources.Load("UI/HostEntry", typeof(Texture)) as Texture;
	Texture m_tBarTop;
	Texture m_tBarBot;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	public Vector2 DoUpdate()
	{
		if(m_rectSmall.height < m_rectParent.height)
		{
		Vector2 vCursor = GameApp.GetInstance().GetCursor().GetScreenPosition();
		if(GameApp.GetInstance().GetCursor().IsMouseDown())
		{
			if(m_rectLong.Contains(vCursor))
			{
				m_bSelected = true;
			}
		}
		else
		{
			m_bSelected = false;
		}
		if(m_bSelected)
		{
			vCursor.y -= m_rectParent.y;
			
			vCursor.y /= m_rectParent.height;
			
			m_vPosition.y = vCursor.y * m_rectView.height;
		}
		
		if(m_vPosition.y < 0.0f)
		{
			m_vPosition.y = 0.0f;
		}
		else if(m_vPosition.y >= m_rectView.height - (m_rectSmall.height / m_rectParent.height) * m_rectView.height)
		{
			m_vPosition.y = m_rectView.height - (m_rectSmall.height / m_rectParent.height) * m_rectView.height;
		}
		}
		
		return(m_vPosition);
	}
	
	public void DoGUI()
	{
		GUI.DrawTexture(m_rectLong, m_tBackground);
		GUI.DrawTexture(m_rectSmall, m_tBar);
		
		if(m_tBarTop != null)
		{
			Rect rectTop = m_rectSmall;
			rectTop.height = 16;
			GUI.DrawTexture(rectTop, m_tBarTop);
		}
		
		if(m_tBarBot != null)
		{
			Rect rectBot = m_rectSmall;
			rectBot.height = 16;
			rectBot.y += m_rectSmall.height - rectBot.height;
			GUI.DrawTexture(rectBot, m_tBarBot);
		}
	}
	
	public void Initialise(Rect _rParentRect, Rect _rView, float _fX, float _fY, float _fLongLength, float _fSmallLength, Vector2 _vPos)
	{
		m_rectView = _rView;
		m_rectParent = _rParentRect;
		m_fX = _rParentRect.x + _fX;
		m_fY = _rParentRect.y + _fY;
		m_fLongLength = _fLongLength;
		m_fSmallLength = _fSmallLength;
		

		m_rectSmall.width = 15.0f;
		m_rectSmall.height = _fSmallLength;
		m_rectSmall.x = m_fX;
		m_rectSmall.y = m_fY + (_vPos.y / _rView.height) * m_rectLong.height;
		
		m_rectLong.x = m_fX;
		m_rectLong.y = m_fY;
		m_rectLong.width = 15.0f;
		m_rectLong.height = _fLongLength;
		
		m_vPosition = _vPos;
	}
	
	public void SetBackgroundTexture(string _sTexture)
	{
		m_tBackground = LoadTexture(_sTexture);
	}
	
	public void SetBarTextures(string _sBar)
	{
		m_tBar = LoadTexture(_sBar);
	}
	
	public void SetBarTextures(string _sBar, string _sTop, string _sBot)
	{
		m_tBar = LoadTexture(_sBar);
		m_tBarTop = LoadTexture(_sTop);
		m_tBarBot = LoadTexture(_sBot);
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
}
