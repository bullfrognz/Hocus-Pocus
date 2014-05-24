using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSceneMainMenu : MonoBehaviour 
{
	private Texture m_Wizard = Resources.Load("UI/Wizard", typeof(Texture)) as Texture;
	private Texture m_TitleTexture = Resources.Load("UI/Title", typeof(Texture)) as Texture;	
	private Texture m_HostEntry = Resources.Load("UI/HostEntry", typeof(Texture)) as Texture;	
	private Texture m_tSelected = Resources.Load("UI/Spell Icons/Spell_Refresher", typeof(Texture)) as Texture;
	
	private Rect m_rectTitle; // Includes the title and the main box
	private Rect m_rectServer; // The box that contains the servers with a scrollbar
	private Rect m_rectServersContainer; // Child of above
	private Rect m_rectNameField;
	private Rect m_rectHostButton;
	private Rect m_rectJoinButton;
	private Rect m_rectRefreshButton;
	private Rect m_rectQuitButton;

	private Vector2 m_v2ScrollPos = Vector2.zero; // Holds current position of the scrollbar.

	private Button m_oJoinButton;
	private Button m_oHostButton;
	private Button m_oQuitButton;
	
	private Scrollbar m_oScroller;
	
	private const int m_kiINVALID = -1;
	private int m_iChosenServer = m_kiINVALID;
	
	private string m_sChosenServer = "";
	private string m_strPlayerName;
	
	private bool m_bNameBoxSelected = false;
	
	// Use this for initialization
	void Start () 
	{ 	
		// Add the scanner so we can immediately bring up a list of games
		gameObject.AddComponent(typeof(ServerScanner));
		RefreshList();
		
		// Load the players name or set it to a default value
		if(PlayerPrefs.HasKey("PlayerName"))
		{
			m_strPlayerName = PlayerPrefs.GetString("PlayerName");
		}
		else
		{
			// Assign a random name
			SetName("");
		}
		
		m_oJoinButton = UnityGUIExt.CreateButton(300.0f, 450.0f, 100.0f, 50.0f, UnityGUIExt.GUI_ALLIGN.TOP_CENTRE, UnityGUIExt.GUI_ALLIGN.CENTRE, "JOIN");
		
		m_oHostButton = UnityGUIExt.CreateButton(300.0f, 525.0f, 100.0f, 50.0f,UnityGUIExt.GUI_ALLIGN.TOP_CENTRE,UnityGUIExt.GUI_ALLIGN.CENTRE, "HOST");

		m_oQuitButton = UnityGUIExt.CreateButton(300.0f, 650.0f, 100.0f, 50.0f, UnityGUIExt.GUI_ALLIGN.TOP_CENTRE, UnityGUIExt.GUI_ALLIGN.CENTRE, "QUIT");

		m_oScroller = new Scrollbar();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		ProcessButtons();
		ProcessNameField();
		
	}
	
	// Draws the GUI (Some processing done)	
	void OnGUI()
	{		
		// Wizard
		GUI.DrawTexture(UnityGUIExt.CreateRect(350.0f, -100.0f, 870.0f, 870.0f,
												UnityGUIExt.GUI_ALLIGN.TOP_CENTRE,
												UnityGUIExt.GUI_ALLIGN.CENTRE), m_Wizard);
		
		// Title (Background)
      	GUI.DrawTexture(UnityGUIExt.CreateRect(0.0f, 10.0f, 815.0f, 753.0f,
												UnityGUIExt.GUI_ALLIGN.TOP_CENTRE,
												UnityGUIExt.GUI_ALLIGN.CENTRE), m_TitleTexture);
		
		// Name field
		ProcessNameFieldGUI();
		
		m_rectNameField = UnityGUIExt.CreateRect(293.0f, 398.0f, 175.0f, 23.0f,
												UnityGUIExt.GUI_ALLIGN.TOP_CENTRE,
												UnityGUIExt.GUI_ALLIGN.CENTRE);
		
		if(m_bNameBoxSelected)
		{
			GUI.DrawTexture(m_rectNameField, m_tSelected);
		}
		GUI.Label(m_rectNameField, m_strPlayerName);
		
		ProcessServerListGUI();
		
		m_oJoinButton.DoGUI();
		m_oHostButton.DoGUI();
		m_oQuitButton.DoGUI();
		m_oScroller.DoGUI();
	}
	
	void ProcessButtons()
	{
		// Button: Join
		if(m_oJoinButton.DoUpdate())
		{
			if(m_sChosenServer != "")
			{
				JoinGame(m_sChosenServer, m_strPlayerName);
			}
		}
			
		// Button: Host
		if(m_oHostButton.DoUpdate())
		{
			if(m_strPlayerName == "")
			{
				SetName(""); // Gives a default name
			}
			GameApp.GetInstance().CreateGame(m_strPlayerName, m_strPlayerName);
		}
		
		// Button: Quit
		if(m_oQuitButton.DoUpdate())
		{
			// TODO: Open a window to confirm exit.
			Application.Quit();
		}
	}
	
	void ProcessNameField()
	{
		// Check if the Name input field has been selected
		
		if(Input.GetMouseButtonUp(0))
		{
			if(m_rectNameField.Contains(GameApp.GetInstance().GetCursor().GetScreenPosition()))
			{
				m_bNameBoxSelected = true;
			}
			else
			{
				m_bNameBoxSelected = false;
				SetName(m_strPlayerName);
			}
		}
		
		// Update the Name input field with player input
		
		if(m_bNameBoxSelected)
		{			
			if (m_strPlayerName.Length < 15)
			{
				foreach (char c in Input.inputString)
				{
					if(c != "\b"[0])
					{
						m_strPlayerName += Input.inputString;
						SetName(m_strPlayerName);
					}
				}
			}
		}
	}
	
	void ProcessNameFieldGUI()
	{
		// Note: Event only works in GUI so the process needed to be split.
		// The backspace was not working correctly in the other function so I had to do it this way - Mana
	
		// Check for backspaces
		
		if(Event.current.type == EventType.keyDown)
		{
			if(m_bNameBoxSelected)
			{
				if(Event.current.keyCode == KeyCode.Backspace)
				{
					if(m_strPlayerName.Length > 0)
					{
						m_strPlayerName = m_strPlayerName.Substring(0, m_strPlayerName.Length -1);
					}
				}
			}
			
			// Check for return key
			
			if( Event.current.keyCode == KeyCode.Return)
			{
				if(m_bNameBoxSelected)
				{
					SetName(m_strPlayerName);
					m_bNameBoxSelected = false;
				}
				else
				{
					m_bNameBoxSelected = true;
				}
			}
		}
	}
	
	void ProcessServerListGUI()
	{
		ServerScanner.TServerInfo[] taOnlineServers = GetComponent<ServerScanner>().GetServerList();
		
		if(taOnlineServers != null && 
			taOnlineServers.Length > 0)
		{
			// This box contains the box that contains the server list. It acts as the parent to everything.
			m_rectServer = UnityGUIExt.CreateRect(-99.0f, 325.0f, 542.0f, 386.0f,
													UnityGUIExt.GUI_ALLIGN.TOP_CENTRE,
													UnityGUIExt.GUI_ALLIGN.CENTRE);
			
			Rect rectHostEntry = new Rect(0,0, 543.0f, 95.0f);
			Rect rectHostName = new Rect(130, 11, 100.0f, 100.0f);
			Rect rectNumPlayers = new Rect(105,53, 100.0f, 100.0f);
			
			// This box contains the list of servers
			// It is relatively positioned to the parent
			// The height is determined by the number of hosted games
			m_rectServersContainer = new Rect(0.0f,0.0f, 525.0f, 0.0f);
			m_rectServersContainer.height = (taOnlineServers.Length * rectHostEntry.height);
			
			GUI.BeginScrollView(m_rectServer, m_v2ScrollPos, m_rectServersContainer);
			
			// Work out if a scroll bar is needed
			// If the box containing the server list is larger than the parent box then we need a scrollbar
			float fScrollbarSize = (m_rectServer.height / m_rectServersContainer.height);
		
			if(fScrollbarSize < 1.0f)
			{
				m_oScroller.Initialise(m_rectServer, m_rectServersContainer, m_rectServer.width - 15, 0, m_rectServer.height, fScrollbarSize* m_rectServer.height, m_v2ScrollPos);
				m_v2ScrollPos = m_oScroller.DoUpdate();
			}
			
			// Grab the mouse pos and convert it into the parent box's space
			
			Vector2 vCursor = GameApp.GetInstance().GetCursor().GetScreenPosition();
			vCursor.x -= m_rectServer.x;
			vCursor.y -= m_rectServer.y - m_v2ScrollPos.y;
	
			int i = 0;
			foreach (ServerScanner.TServerInfo tServerInfo in taOnlineServers)
			{
				// Draw the entry
				GUI.DrawTexture(rectHostEntry, m_HostEntry);
				GUI.Label(rectHostName, tServerInfo.sGameName);
				GUI.Label(rectNumPlayers, tServerInfo.uiNumConnectedPlayers + " / " + tServerInfo.uiPlayerLimit);
				
				// Check for selection
				if(rectHostEntry.Contains(vCursor))
				{	
					if(GameApp.GetInstance().GetCursor().IsMouseDown())
					{
						m_iChosenServer = i;
					}
					
//					// Double click
//					// Does not function properly
					Event evMouse = Event.current;
					if(evMouse.isMouse && evMouse.clickCount == 2)
					{
						JoinGame(taOnlineServers[m_iChosenServer].saIp[0], m_strPlayerName);
					}
				}
				
				// Highlight the chosen server
				if(m_iChosenServer == i)
				{
					GUI.DrawTexture(rectHostEntry, m_tSelected);
				}
								
				// Increment to/for next entry
				rectHostEntry.y += rectHostEntry.height;
				rectHostName.y += rectHostEntry.height;
				rectNumPlayers.y += rectHostEntry.height;
				
				++i;
			}
			
			// Store the IP of the chosen server
			if(m_iChosenServer != m_kiINVALID)
			{
				m_sChosenServer = taOnlineServers[m_iChosenServer].saIp[0];
			}
			
			GUI.EndScrollView();
		}
	}
	
	void RefreshList()
	{
		m_sChosenServer = "";
		m_iChosenServer = m_kiINVALID;
		GetComponent<ServerScanner>().RefreshServerList();
		m_v2ScrollPos = Vector2.zero;
	}

	
	void OnConnectedToServer()
	{
		GetComponent<GameApp>().ChangeScene(GameApp.EScene.ARENA);
	}
	
	
	void OnServerInitialized()
	{
		GetComponent<GameApp>().ChangeScene(GameApp.EScene.ARENA);
    }
	
	void JoinGame(string _sHostIP, string _sPlayerName)
	{
		GameApp.GetInstance().JoinGame(_sHostIP, _sPlayerName);
	}
	
	void SetName(string _sName)
	{
		if(_sName != "")
		{
			m_strPlayerName = _sName;
			PlayerPrefs.SetString("PlayerName", _sName);
		}
		else
		{
			m_strPlayerName = "Warlock " + Random.Range(0, 1000).ToString();
			SetName(m_strPlayerName);
		}
	
	}
}