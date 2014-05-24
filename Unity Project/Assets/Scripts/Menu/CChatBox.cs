using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CChatBox : MonoBehaviour
{
	private Texture m_tBackground = Resources.Load("UI/Chatbox", typeof(Texture)) as Texture;
	private Texture m_tBackground2 = Resources.Load("UI/Chatbox1", typeof(Texture)) as Texture;
	private Texture m_tSelected = Resources.Load("UI/Spell Icons/Spell_Refresher", typeof(Texture)) as Texture;
	
	private Rect m_rectBox;
	private Rect m_rectText;
	
	float m_fX;
	float m_fY;
	float m_fWidth;
	float m_fHeight;
	UnityGUIExt.GUI_ALLIGN m_eAllignment;
	UnityGUIExt.GUI_ALLIGN m_eScreenAllignment;
	
	private int m_iMaxEntries = 15;
	
	private bool m_bSelected;
	private bool m_bDrawScroll;
	
	private string m_sPlayerName;
	private string m_sText ="";
	
	private bool m_bDoOnce = true;

	private Vector2 m_v2ScrollPos = new Vector2(0,0); // High value to keep bar at botto
	
	private Scrollbar m_oScrollbar = new Scrollbar();
	
	private List<CChatEntry> m_listChatEntries = new List<CChatEntry>();

	class CChatEntry
	{
		public string sName = "";
		public string sMessage = "";
	}
	
	void Start()
	{
		// Attach network view
		gameObject.AddComponent(typeof(NetworkView));
	}
	public void Initialise(float _fX, float _fY, float _fWidth, float _fHeight, UnityGUIExt.GUI_ALLIGN _eScreenAllign, UnityGUIExt.GUI_ALLIGN _eAllign, string _sPlayerName)
	{
		m_sPlayerName =  _sPlayerName;
		

		
		m_fX = _fX;
		m_fY = _fY;
		m_fWidth = _fWidth;
		m_fHeight = _fHeight;
		m_eAllignment = _eAllign;
		m_eScreenAllignment = _eScreenAllign;
		
		UpdateBox(m_fX, m_fY, m_fWidth, m_fHeight, m_eScreenAllignment, m_eAllignment);		
	}
	
	void UpdateBox(float _fX, float _fY, float _fWidth, float _fHeight, UnityGUIExt.GUI_ALLIGN _eScreenAllign, UnityGUIExt.GUI_ALLIGN _eAllign)
	{
		m_rectBox = UnityGUIExt.CreateRect(m_fX, m_fY, m_fWidth, m_fHeight, m_eAllignment, m_eScreenAllignment);
				
		m_rectText.x = m_rectBox.x + 1;
		m_rectText.y = m_rectBox.y + m_rectBox.height + 2;
		m_rectText.width = m_rectBox.width - 1;
		m_rectText.height = 25;
	}
	
	void Update()
	{

		if(Input.GetKeyUp(KeyCode.Return))
		{
			if(IsSelected())
			{
				// Send message
				HitEnter();
				SetSelected(false);
			}
			else
			{
				SetSelected(true);
			}	
		}
		
		if(IsSelected())
		{
			if(m_sText.Length < 25)
			{
				foreach(char c in Input.inputString)
				{
					if(c != "\b"[0])
					{
						m_sText += Input.inputString;
					}				
				}
			}
			if(Input.GetKeyUp(KeyCode.Escape))
			{
				SetSelected(false);
			}
		}
		
		// Need to wait until the warlock controller is created before this is called. Not the best place but works for now.
		if(m_bDoOnce)
		{
			string strCol = CArenaAwards.ColorToHex(GameApp.GetInstance().GetWarlock().GetComponent<CWarlockController>().transform.FindChild("Point light").light.color);
			m_sPlayerName = "<color=" + strCol + ">" + m_sPlayerName +"</color>";
			m_bDoOnce = false;
		}
	}
	
	void OnGUI()
	{

	}
	
	public void DoGUI()
	{
		UpdateBox(m_fX, m_fY, m_fWidth, m_fHeight, m_eScreenAllignment, m_eAllignment);	
		
		if(m_bSelected)
		{
			GUI.DrawTexture(UnityGUIExt.CreateRect(0,0, 512,512, UnityGUIExt.GUI_ALLIGN.BOT_LEFT, UnityGUIExt.GUI_ALLIGN.BOT_LEFT), m_tBackground);

			if(m_bDrawScroll)
			{
				m_oScrollbar.DoGUI();
			}
			
			DrawTextBox();
		}
		else
		{
			GUI.DrawTexture(UnityGUIExt.CreateRect(0,0, 512,512, UnityGUIExt.GUI_ALLIGN.BOT_LEFT, UnityGUIExt.GUI_ALLIGN.BOT_LEFT), m_tBackground2);
		}
		DrawChatBox();
	}
	
	// FIX ISSUES HERE with http://answers.unity3d.com/questions/13443/is-there-a-way-to-measure-the-pixel-withheight-of.html
	void DrawChatBox()
	{
		Rect rView = m_rectBox;
		rView.x = 0;
		rView.y = 0;
		rView.height = m_listChatEntries.Count * 30;
		
		Rect rBox = m_rectBox;
		rBox.width += 20;		
		//GUI.BeginScrollView(rBox,  m_v2ScrollPos, rView, false, false);

		//m_v2ScrollPos = new Vector2(m_v2ScrollPos.x, rView.height);
		if(m_listChatEntries.Count < 7) // quick fix for something frustating
		{
			m_v2ScrollPos.y = 0;
		}
		UnityGUIExt.BeginScrollView(rBox,  m_v2ScrollPos, rView, false, false);
		m_oScrollbar.Initialise(m_rectBox, rView, m_rectBox.width + 5, 0, m_rectBox.height, (m_rectBox.height / rView.height) * m_rectBox.height, m_v2ScrollPos);
		m_v2ScrollPos = m_oScrollbar.DoUpdate();
		
		if(m_rectBox.height / rView.height < 1.0f)
		{
			m_bDrawScroll = true;
		}
		else
		{
			m_bDrawScroll = false;
		}
				
		foreach(CChatEntry cEntry in m_listChatEntries)
		{
			if(cEntry.sName=="")
			{
				//Game message
				GUILayout.Label(cEntry.sMessage);
			
			}
			else
			{
				GUILayout.Label(cEntry.sName+": "+cEntry.sMessage);
			}		
		}
		//GUI.EndScrollView();
		UnityGUIExt.EndScrollView();
	}
	
	void DrawTextBox()
	{
		if(IsSelected())
		{	
			if(Event.current.type == EventType.keyDown)
			{
			if(Event.current.keyCode == KeyCode.Backspace)
			{
				if(m_sText.Length > 0)
				{
					m_sText = m_sText.Substring(0, m_sText.Length -1);
				}
				//Debug.Log(m_sText);
			}
			}
			
			GUI.Label(m_rectText, m_sText);
			
			Rect rectHighlight = m_rectText;
			rectHighlight.height -= 7;
			rectHighlight.y += 4;
			//GUI.DrawTexture(rectHighlight, m_tSelected);
		}
		else
		{
			GUI.Label(m_rectText, m_sText);
		}
		
	}
	
	bool IsSelected()
	{
		return(m_bSelected);
	}
	
	void SetSelected(bool _bSelected)
	{
		m_bSelected = _bSelected;
	}	
	
	void HitEnter()
	{
		if(m_sText  != "")
		{
			//m_sText = m_sText.Replace("\n", "");
			
			if(m_sText.Length > 1)
			{
				AddMessage(m_sPlayerName, m_sText);
				

				networkView.RPC("SendGlobalMessage", RPCMode.Others, m_sPlayerName, m_sText);
			}
			m_sText = ""; //Clear line
		}
	}
	
	void AddMessage(string _sName, string _sMessage)
	{
		CChatEntry cEntry = new CChatEntry();
	    cEntry.sName = _sName;
	    cEntry.sMessage = _sMessage;
	    m_listChatEntries.Add(cEntry);
		
		if(m_listChatEntries.Count > m_iMaxEntries)
		{
			m_listChatEntries.RemoveAt(0);
		}
	    m_v2ScrollPos.y = 1000000; 
	}
	
	[RPC]
	void SendGlobalMessage(string _sName, string _sMessage)
	{
		AddMessage(_sName, _sMessage);
	}
}
