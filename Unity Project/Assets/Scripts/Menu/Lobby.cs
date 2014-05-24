using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Pop Ups :)
// http://wiki.unity3d.com/index.php?title=PopupList
// http://wiki.unity3d.com/index.php?title=DropDownList#C.23_-_DropDownList.cs

// Load resource in script
// http://answers.unity3d.com/questions/61531/accessing-textures-through-code.html

// Access components in script
// http://docs.unity3d.com/Documentation/ScriptReference/index.Accessing_Other_Game_Objects.html


public class Lobby : MonoBehaviour
{
	
// Member Types
	
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	// Player List Box
	//List<string> m_vPlayers = new List<string>();
	//Rect m_tPlayerListBox;
	//Rect m_tPlayerNamePlate;
	//Rect m_tPlayerListBoxContainer;
	//Texture m_cPlayerListTexture;
	Vector2 m_v2ScrollPos;

	
	// Bottom Box
	//Texture m_cBottomBoxTexture;
	Rect m_tBottomBox;
	Rect m_tQuitButton;
	Rect m_tStartGame;
	string m_strPlayerName;
	
	
	// Game Modes
	Rect m_tGameModes;
    string[] m_asGameModes = new string[] {"Normal", "Random", "WTF"};
	int m_iGameModeSelected = 0;
	
	int m_iRoundsSelected = 3;
	
	private Button m_oStartButton;
	private Button m_oBackButton;
	private Button m_oNormalButton;
	private Button m_oRandomButton;
	private Button m_oWTFButton;
	private Button m_oRounds3;
	private Button m_oRounds6;
	private Button m_oRounds9;
	
	private Texture m_tBotRight = Resources.Load("UI/Botright", typeof(Texture)) as Texture;
	private Texture m_tSelected = Resources.Load("UI/Spell Icons/Spell_Refresher", typeof(Texture)) as Texture;
	
	
	//private CChatBox m_cChatBox;
	
// Member Functions
	
	
	// Public:
	
	
	// Private:
	
	
	void Start()
	{
		//m_cChatBox = gameObject.AddComponent(typeof(CChatBox)) as CChatBox;
		//m_cChatBox.Initialise(UnityGUIExt.CreateRect(10,30,300,200, UnityGUIExt.GUI_ALLIGN.BOT_LEFT, UnityGUIExt.GUI_ALLIGN.BOT_LEFT), PlayerPrefs.GetString("PlayerName"));
		
		//m_cPlayerListTexture = Resources.Load("UI/Main_Serverlist", typeof(Texture)) as Texture;
		//m_cBottomBoxTexture = Resources.Load("UI/HUD_Bottom", typeof(Texture)) as Texture;
		
		
		ResetLayout();

		//gameObject.AddComponent(typeof(Chat));
		
		m_oStartButton = UnityGUIExt.CreateButton(33.0f, 90.0f, 100.0f, 50.0f, UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, "START");
			
		m_oBackButton = UnityGUIExt.CreateButton(33.0f, 35.0f, 100.0f, 50.0f, UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, "EXIT");
		
		float fButtonHeight = 35.0f;
		float fStartModes = 241.0f;
		float fStartRounds = 378.0f;
		
				
		m_oRounds3 = UnityGUIExt.CreateButton(33.0f, fStartRounds, 100.0f, fButtonHeight,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, "3 Rounds");
		m_oRounds6 = UnityGUIExt.CreateButton(33.0f, fStartRounds - fButtonHeight, 100.0f, fButtonHeight,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, "6 Rounds");
		m_oRounds9 = UnityGUIExt.CreateButton(33.0f, fStartRounds - fButtonHeight - fButtonHeight, 100.0f, fButtonHeight,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, "9 Rounds");
		
		m_oNormalButton = UnityGUIExt.CreateButton(33.0f, fStartModes, 100.0f, fButtonHeight,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, "Normal");	
		m_oRandomButton = UnityGUIExt.CreateButton(33.0f, fStartModes - fButtonHeight, 100.0f, fButtonHeight,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, "Random");
		m_oWTFButton = UnityGUIExt.CreateButton(33.0f,  fStartModes - fButtonHeight- fButtonHeight, 100.0f, fButtonHeight,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, "WTF");
	}
	

	void Update()
	{
		//GetComponent<Chat>().SetBox(10, 10, 300, 125, UnityGUIExt.GUI_ALLIGN.BOT_LEFT);
		
		if (Network.isServer)
		{
			if(m_oStartButton.DoUpdate())
			{
				//if (Network.connections.Length < 1)
				{
				//	GameApp.GetInstance().DisplayError("Need at least 2 players to start the game", 2);
				}
				//else
				{
					GetComponent<CSceneArena>().StartGame();
				}
			}
		}
		if(m_oBackButton.DoUpdate())
		{
			Back();
		}
		
		
				
		if (Network.isClient)
		{
			Server.EGameMode eGameMode = GameApp.GetInstance().GetServer().GetGameMode();
			m_iGameModeSelected = (int)eGameMode;
			
			m_iRoundsSelected = GameApp.GetInstance().GetServer().GetNumberOfRounds();
		}
		else
		{
			// Mode
			int iSelection = m_iGameModeSelected;
			
			if(m_oNormalButton.DoUpdate())
			{
				iSelection = 0;
			}
			else if(m_oRandomButton.DoUpdate())
			{
				iSelection = 1;
			}
			else if(m_oWTFButton.DoUpdate())
			{
				iSelection = 2;
			}
			
			if (Network.isServer &&
				iSelection != m_iGameModeSelected)
			{
				m_iGameModeSelected = iSelection;
				GameApp.GetInstance().GetServer().SetGameMode(m_iGameModeSelected);
			}
			
			iSelection = m_iRoundsSelected;
			
			// Rounds
			if(m_oRounds3.DoUpdate())
			{
				iSelection = 3;
			}
			else if(m_oRounds6.DoUpdate())
			{
				iSelection = 6;
			}
			else if(m_oRounds9.DoUpdate())
			{
				iSelection = 9;
			}
			
			if (Network.isServer &&
				iSelection != m_iRoundsSelected)
			{
				m_iRoundsSelected = iSelection;
				GameApp.GetInstance().GetServer().SetNumberOfRounds(m_iRoundsSelected);
			}
		}
		
				
		
	}
	
	
	// Initalises all rectangles. Recall if the screen resolution is changed.
	void ResetLayout()
	{
		// Player List
//		m_tPlayerListBox = UnityGUIExt.CreateRect(45, 0, 300, 119,
//										UnityGUIExt.GUI_ALLIGN.BOT_CENTRE,
//										UnityGUIExt.GUI_ALLIGN.BOT_CENTRE);
//		
//		m_tPlayerListBoxContainer = UnityGUIExt.CreateRect(45, 0, 280, 1000,
//												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE,
//												UnityGUIExt.GUI_ALLIGN.TOP_CENTRE);
//		
//		m_tPlayerNamePlate = UnityGUIExt.CreateRect(0, 0, 100, 20,
//												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE,
//												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE);
		
	}
	
	
	void OnGUI()
	{	
		GUI.DrawTexture(UnityGUIExt.CreateRect(0,0,512,512, UnityGUIExt.GUI_ALLIGN.BOT_RIGHT, UnityGUIExt.GUI_ALLIGN.BOT_RIGHT), m_tBotRight);
		
		
		if (Network.isServer)
		{
			m_oStartButton.DoGUI();
		}
		m_oBackButton.DoGUI();
		
		DrawGuiPlayerList();
		
		m_oNormalButton.DoGUI();
		m_oRandomButton.DoGUI();
		m_oWTFButton.DoGUI();
		
		if(m_iGameModeSelected == 0)
		{
			GUI.DrawTexture(m_oNormalButton.GetRect(), m_tSelected);
		}
		else if(m_iGameModeSelected == 1)
		{
			GUI.DrawTexture(m_oRandomButton.GetRect(), m_tSelected);
		}
		else if(m_iGameModeSelected == 2)
		{
			GUI.DrawTexture(m_oWTFButton.GetRect(), m_tSelected);
		}
		
		m_oRounds3.DoGUI();
		m_oRounds6.DoGUI();
		m_oRounds9.DoGUI();
		
		if(m_iRoundsSelected == 3)
		{
			GUI.DrawTexture(m_oRounds3.GetRect(), m_tSelected);
		}
		else if(m_iRoundsSelected == 6)
		{
			GUI.DrawTexture(m_oRounds6.GetRect(), m_tSelected);
		}
		else if(m_iRoundsSelected == 9)
		{
			GUI.DrawTexture(m_oRounds9.GetRect(), m_tSelected);
		}
		
		//m_cChatBox.DoGUI();
	}
	
	
	void DrawGuiGameModes()
	{
		if (Network.isClient)
		{
			Server.EGameMode eGameMode = GameApp.GetInstance().GetServer().GetGameMode();
			m_iGameModeSelected = (int)eGameMode;
		}
		
		
		
		int m_iCurrnetSelection = GUI.SelectionGrid( UnityGUIExt.CreateRect(120.0f, 10.0f, 100.0f, 100.0f,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT,
												UnityGUIExt.GUI_ALLIGN.BOT_RIGHT), m_iGameModeSelected, m_asGameModes, 1);
		
		
		if (Network.isServer &&
			m_iCurrnetSelection != m_iGameModeSelected)
		{
			m_iGameModeSelected = m_iCurrnetSelection;
			GameApp.GetInstance().GetServer().SetGameMode(m_iGameModeSelected);
		}
	}
	
	
	void DrawGuiPlayerList()
	{
//		GUI.DrawTexture(m_tPlayerListBox, m_cPlayerListTexture);
//		
//		Rect rectButtons = m_tPlayerNamePlate;
//		
//		Server.TPlayerInfo[] playerInfo = GetComponent<Server>().GetConnectedPlayers();
//		Rect rectPlayerListBoxContainer = m_tPlayerListBoxContainer;
//		rectPlayerListBoxContainer.height = playerInfo.Length * (m_tPlayerNamePlate.height + m_fNamePlateFormatOffsetY);
//		if(rectPlayerListBoxContainer.height < m_tPlayerListBox.height)
//		{
//			rectPlayerListBoxContainer.height = m_tPlayerListBox.height;
//		}
//
//		m_v2ScrollPos = GUI.BeginScrollView(m_tPlayerListBox, m_v2ScrollPos, rectPlayerListBoxContainer, false, true);
//	
//		foreach(Server.TPlayerInfo player in playerInfo)
//		{			
//			GUI.Button(rectButtons, player.tNetworkPlayer.ipAddress);
//			
//			rectButtons.y += m_fNamePlateFormatOffsetY + rectButtons.height;
//		}
//		
//		GUI.EndScrollView();
	}
	
	
	void Back()
	{
		GameApp.GetInstance().GetClientObj().GetComponent<CClientConnection>().Disconnect();
	}
	
	
	// Events:
	
	
}