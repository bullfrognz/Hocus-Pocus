using UnityEngine;
using System.Collections;


public class CSceneArena : MonoBehaviour
{
	
// Member Types

	
	public enum EState
	{
		INVALID = 0,
		
		LOBBY,
		GAME_START_FADE,
		GAME_START_PAN,
		ROUND_RESTART_FADE,
		ROUND_START_COUNTDOWN,
		FIGHTING,
		SHOPPING_FADE,
		SHOPPING,
		PAUSED,
		
		END_GAME_FADE,
		END_GAME_PODIUM,
		
		MAX
	}
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	CScoreBoard m_cScoreBoard;
	Shop m_oShop;
	Lobby m_cLobby;
	CChatBox m_cChat;
	
	
	EState m_eState 	= EState.INVALID;
	EState m_ePrevState = EState.INVALID;
	
	
	public const float m_kfShoppingDuration  = 15.0f;
	const float m_kfCountdownDuration = 5.5f;
	float m_fShopTimer;
	float m_fCountdownTimer;
	
	uint m_uiRoundCount = 0;
	
	// Could be an array ^_^
	Texture m_tCountdown0 = Resources.Load("UI/Countdown0", typeof(Texture)) as Texture;
	Texture m_tCountdown1 = Resources.Load("UI/Countdown1", typeof(Texture)) as Texture;
	Texture m_tCountdown2 = Resources.Load("UI/Countdown2", typeof(Texture)) as Texture;
	Texture m_tCountdown3 = Resources.Load("UI/Countdown3", typeof(Texture)) as Texture;
	
	bool m_bAllowDebugShop = false;
	
	
// Member Functions
	
	
	// Public:
	
	
	[RPC]
	public void StartGame()
	{
		Destroy(gameObject.GetComponent<Lobby>());
		
		
		if (Network.isServer)
		{
			networkView.RPC("StartGame", RPCMode.OthersBuffered);
			SetState(EState.GAME_START_FADE);
			
			GameApp.GetInstance().GetServer().NotifyStartGame();
		}
		
		GetComponent<CArenaAwards>().InitialiseAwards();
	}
	
	
	[RPC]
	public void SetState(int _iState)
	{
		SetState((EState)_iState);
	}
	
	
	// ONLY SERVER SHOULD CALL THIS
	public void SetState(EState _eState)
	{
		if (Network.isServer)
		{
			networkView.RPC("SetState", RPCMode.OthersBuffered, (int)_eState);
		}
			
			
		if(m_eState != _eState)
		{
			m_ePrevState = m_eState;
		}
		
		
		m_eState = _eState;
		

		switch (m_eState)
		{
		case EState.LOBBY:
			GameApp.GetInstance().GetCursor().SetSideScrollingEnabled(true);
			GameApp.GetInstance().GetGameCamera().GetComponent<CCameraController>().ResetToStartPosition();
			break;
			
		case EState.GAME_START_FADE:
			GameApp.GetInstance().GetCursor().SetVisible(false); // Show cursor
			Destroy(m_cLobby);
			SetWarlockSpellbookMovementDisabled(true);
			m_fCountdownTimer = 4;
			GameApp.GetInstance().GetGameCamera().GetComponent<CCameraController>().FadeOutIn(4, 6);
			break;
			
		case EState.GAME_START_PAN:
			GameApp.GetInstance().GetGameCamera().GetComponent<CCameraController>().RunStartGamePan(2.0f, 0.01f, 5.0f, 20.0f, 3.0f);
			break;
			
		case EState.ROUND_RESTART_FADE:
			m_oShop.enabled = false;
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockHealth>().WarlockHealth_Reset(true);
			GameApp.GetInstance().GetGameCamera().GetComponent<CCameraController>().FadeOutIn(3, 4);
			m_fCountdownTimer = 3.0f;
			break;
			
		case EState.ROUND_START_COUNTDOWN:
			++ m_uiRoundCount;
			GameApp.GetInstance().GetGameCamera().GetComponent<CCursor>().SetSideScrollingEnabled(true);
			GameApp.GetInstance().GetCursor().SetVisible(true); // Hide cursor
			m_fCountdownTimer = m_kfCountdownDuration;
			break;
			
		case EState.FIGHTING:
			if (Network.isServer)	
			{
				networkView.RPC("Terrain_Pause", RPCMode.AllBuffered, false);
			}
			SetWarlockSpellbookMovementDisabled(false);
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockHealth>().SetHealthBarVisable(true, true);
			
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockController>().WarlockController_SetLastAttacker(-1);
			GetComponent<CArenaAwards>().InitialiseAwards();
			
			break;
			
		case EState.SHOPPING_FADE:
			GameApp.GetInstance().GetCursor().SetSideScrollingEnabled(false); // Stop side scrollling
			SetWarlockSpellbookMovementDisabled(true); // Stop movement & Casting
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockHealth>().SetHealthBarVisable(false, true);
			m_fCountdownTimer = 4.0f;
			GameApp.GetInstance().GetGameCamera().GetComponent<CCameraController>().FadeOutIn(4, 4);

			GetComponent<CArenaAwards>().AwardRoundBonus();
			
			break;
			
		case EState.SHOPPING:
			if (Network.isServer)	
			{	
				networkView.RPC("Terrain_Initialise", RPCMode.AllBuffered, Random.Range(0, int.MaxValue), 800);
			}
			m_fShopTimer = m_kfShoppingDuration;
			m_oShop.enabled = true;
			break;
			
		case EState.END_GAME_FADE:
			if (Network.isServer)	
			{
				networkView.RPC("Terrain_Pause", RPCMode.AllBuffered, true);
			}
			GameApp.GetInstance().GetCursor().SetSideScrollingEnabled(false); // Stop side scrollling
			SetWarlockSpellbookMovementDisabled(true); // Stop movement & Casting
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockHealth>().SetHealthBarVisable(false, true);
			m_fCountdownTimer = 4.0f; //Switch scene
			GameApp.GetInstance().GetGameCamera().GetComponent<CCameraController>().FadeOutIn(4, 6);
			break;
			
		case EState.END_GAME_PODIUM:
			m_fCountdownTimer = 4.0f;
			SetWarlockWinningPosition();
			break;
		}
	}
	

	public EState GetState()
	{
		return (m_eState);
	}
	
	
	public void ReturnToPreviousState()
	{
		if (Network.isServer)
		{
			Debug.Log(m_eState.ToString() + ", " + m_ePrevState.ToString());
			SetState(m_ePrevState);
		}
		else
		{
			Debug.LogError("CLIENTS SHOULD NOT CALL THIS FUNCTION!!!!");
		}
	}
	
	public float GetShopTimer()
	{
		return(m_fShopTimer);
	}
	
	public CPlayerList GetPlayerList()
	{
		return(GetComponent<CPlayerList>());
	}
	
	
	
	// Private:
	
	
	void Awake()
	{
		m_cScoreBoard = gameObject.AddComponent(typeof(CScoreBoard)) as CScoreBoard;
		m_cChat = gameObject.AddComponent(typeof(CChatBox)) as CChatBox;
		m_cChat.Initialise(15,35,280,165, UnityGUIExt.GUI_ALLIGN.BOT_LEFT, UnityGUIExt.GUI_ALLIGN.BOT_LEFT, PlayerPrefs.GetString("PlayerName"));
		
	}
	
	
	void Start()
	{
		if (Network.isServer)	
		{
			networkView.RPC("Terrain_Initialise", RPCMode.AllBuffered, Random.Range(0, int.MaxValue), 800);
		}
		
		
		InitialiseShop();
		
		
		if (Network.isServer)
		{
			SetState(EState.LOBBY);
		
		}
		
		m_cLobby = gameObject.AddComponent<Lobby>() as Lobby;
		gameObject.AddComponent<CArenaAwards>();
		
		gameObject.AddComponent<CPlayerList>();
		GetComponent<CPlayerList>().InitialisePlayerList();
	}
	

	void Update()
	{
		m_fCountdownTimer -= Time.deltaTime;
		
		
		if (m_eState == EState.LOBBY)
		{
			UpdateStateLobby();
		}
		else if (m_eState == EState.GAME_START_FADE)
		{
			UpdateStateGameStartFade();
		}
		else if (m_eState == EState.GAME_START_PAN)
		{
			UpdateStateGameStartPan();
		}
		else if (m_eState == EState.ROUND_RESTART_FADE)
		{
			UpdateStateRoundFade();
		}
		else if (m_eState == EState.ROUND_START_COUNTDOWN)
		{
			UpdateStateRoundCountdown();
		}
		else if (m_eState == EState.FIGHTING)
		{
			UpdateStateFighting();
		}
		else if (m_eState == EState.SHOPPING_FADE)
		{
			UpdateStateShoppingFade();
		}
		else if (m_eState == EState.SHOPPING)
		{
			UpdateStateShopping();
		}
		else if (m_eState == EState.END_GAME_FADE)
		{
			UpdateEndGameFade();
		}
		else if (m_eState == EState.END_GAME_PODIUM)
		{
			UpdateEndGamePodium();
		}
		
		if(m_bAllowDebugShop)
		{
			if(Input.GetKeyDown(KeyCode.F1))
			{
				SetState(EState.SHOPPING);
			}
		}
	}
	
	
	void UpdateStateLobby()
	{
		// Empty
	}
	
	
	void UpdateStateGameStartFade()
	{
		// Waiting for the camera to fade out before positioning the warlocks
		if (m_fCountdownTimer < 0.0f)
		{
			SetWarlockRoundStartPosition();
			
			
			// Only for the server
			if (Network.isServer)
			{
				SetState(EState.GAME_START_PAN);
			}
		}
	}
	
	
	void UpdateStateGameStartPan()
	{
		SetWarlockRoundStartPosition();
		
		
		// Only for the server
		if (Network.isServer)
		{
			// Has finished panning
			if (!GameApp.GetInstance().GetGameCamera().GetComponent<CCameraController>().IsPanning())
			{
				SetState(EState.ROUND_START_COUNTDOWN);
			}
		}
	}
	
	
	void UpdateStateRoundFade()
	{
		if (m_fCountdownTimer < 0.0f)
		{
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockHealth>().WarlockHealth_Reset(true);
			SetWarlockRoundStartPosition(); // Set start position
			GameApp.GetInstance().GetGameCamera().GetComponent<CCameraController>().CenterCameraOnWarlock(); // Center camera
			
			
			// Only for the server
			if (Network.isServer)
			{
				if (!GameApp.GetInstance().GetGameCamera().GetComponent<CCameraController>().IsFadding())
				{
					SetState(EState.ROUND_START_COUNTDOWN);
				}
			}
		}
	}
	
	
	void UpdateStateRoundCountdown()
	{
		// Only for the server
		if (Network.isServer)
		{
			if (m_fCountdownTimer < 0.0f)
			{
				SetState(EState.FIGHTING);
			}
		}	
	}
	
	
	void UpdateStateFighting()
	{
		if (Network.isServer)
		{
			GameObject[] aoClientObjects = GameObject.FindGameObjectsWithTag("Client Object");
			int iNumAliveWarlocks = 0;
			GameObject oLastAliveClient = null;
			
			
			foreach (GameObject oClientObject in aoClientObjects)
			{
				GameObject oWarlock = oClientObject.GetComponent<CClientInfo>().GetWarlock();
				
				
				if (oWarlock.GetComponent<CWarlockHealth>().IsAlive())
				{
					++ iNumAliveWarlocks;
					oLastAliveClient = oClientObject;
				}
			}
			
			
			if (iNumAliveWarlocks == 1)
			{
				networkView.RPC("DisplayRoundWinner", RPCMode.AllBuffered, oLastAliveClient.GetComponent<CClientInfo>().GetUsername() + " Wins Round!");
				
				
				if (m_uiRoundCount == GameApp.GetInstance().GetServer().GetNumberOfRounds())
				{
					networkView.RPC("DisplayWinner", RPCMode.AllBuffered, ">> " + oLastAliveClient.GetComponent<CClientInfo>().GetUsername() + " WINS THE GAME!!! <<");
					SetState(EState.END_GAME_FADE);
				}
				else
				{
					SetState(EState.SHOPPING_FADE);
				}
			}
			else if (iNumAliveWarlocks == 0)
			{
				networkView.RPC("DisplayRoundWinner", RPCMode.AllBuffered, "Round Draw!");
				
				
				if (m_uiRoundCount == GameApp.GetInstance().GetServer().GetNumberOfRounds())
				{
					networkView.RPC("DisplayWinner", RPCMode.AllBuffered, ">> Lol no one won the game <<");
					SetState(EState.END_GAME_FADE);
				}
				else
				{
					SetState(EState.SHOPPING_FADE);
				}
			}
		}
	}
	
	
	[RPC]
	void DisplayRoundWinner(string _sMessage)
	{
		GameApp.GetInstance().DisplaySuccessful(_sMessage, 5);
	}
	
	
	[RPC]
	void DisplayWinner(string _sMessage)
	{
		GameApp.GetInstance().DisplaySuccessful(_sMessage, 10000);
	}
	
	
	void UpdateStateShoppingFade()
	{
		if (Network.isServer)
		{
			if (m_fCountdownTimer < 0.0f)
			{
				SetState(EState.SHOPPING);
			}
		}
	}
	
	
	void UpdateStateShopping()
	{
		m_fShopTimer -= Time.deltaTime;
		
		if (m_fShopTimer < 0.0f)
		{
			m_oShop.enabled = false;
			
			// Only for the server
			if (Network.isServer)
			{
				SetState(EState.ROUND_RESTART_FADE);
			}
		}
	}
	
	
	void UpdateEndGameFade()
	{
		if (m_fCountdownTimer < 0.0f)
		{
			if (Network.isServer)
			{
				SetState(EState.END_GAME_PODIUM);
			}
		}
	}
	
	
	void UpdateEndGamePodium()
	{
	}
	
	
	void OnGUI()
	{
		if(m_eState == EState.ROUND_START_COUNTDOWN)
		{
			if (m_fCountdownTimer > 3.5f)
			{
				string sRoundText;
				
				
				if (m_uiRoundCount == GameApp.GetInstance().GetServer().GetNumberOfRounds())
				{
					sRoundText = "Last Round!";
				}
				else
				{
					sRoundText = "Round " + m_uiRoundCount;
				}
				
				
				GUIStyle cMyStyle = new GUIStyle();
				cMyStyle.alignment = TextAnchor.MiddleCenter;
				cMyStyle.fontSize = 80;
				cMyStyle.normal.textColor = new Color(0.65f, 0.168f, 0.73f, 1.0f);
				
				
				float fBoxHeight = 40;
				float fBoxWidth  = 10 * sRoundText.Length;
				float fPositionX = Screen.width / 2  - fBoxWidth / 2;
				float fPositionY = Screen.height / 2 - fBoxHeight / 2 - 100;
				
				
				GUI.TextField(new Rect (fPositionX, fPositionY, fBoxWidth, fBoxHeight), sRoundText, cMyStyle);
			}
			else if(m_fCountdownTimer > 2.5f)
			{
				GameApp.GetInstance().GetWarlock().GetComponent<CWarlockAnimator>().NotifyRevive();
				GUI.DrawTexture(UnityGUIExt.CreateRect(0,-100,256,256, UnityGUIExt.GUI_ALLIGN.CENTRE_CENTRE,  UnityGUIExt.GUI_ALLIGN.CENTRE_CENTRE), m_tCountdown3);
			}
			else if(m_fCountdownTimer > 1.5f)
			{
				GUI.DrawTexture(UnityGUIExt.CreateRect(0,-100,256,256, UnityGUIExt.GUI_ALLIGN.CENTRE_CENTRE,  UnityGUIExt.GUI_ALLIGN.CENTRE_CENTRE), m_tCountdown2);
			}
			else if(m_fCountdownTimer > 0.5f)
			{
				GUI.DrawTexture(UnityGUIExt.CreateRect(0,-100,256,256, UnityGUIExt.GUI_ALLIGN.CENTRE_CENTRE,  UnityGUIExt.GUI_ALLIGN.CENTRE_CENTRE), m_tCountdown1);
			}
			else if(m_fCountdownTimer > 0.0f)
			{
				GUI.DrawTexture(UnityGUIExt.CreateRect(0,-100,512,512, UnityGUIExt.GUI_ALLIGN.CENTRE_CENTRE,  UnityGUIExt.GUI_ALLIGN.CENTRE_CENTRE), m_tCountdown0);
			}
			else
			{
				if (Network.isServer)
				{
					SetState(EState.FIGHTING);
				}
			}
		}
	
		m_cChat.DoGUI();
	}

	void SetWarlockSpellbookMovementDisabled(bool _bDisabled)
	{
		if (_bDisabled)
		{
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockMotor>().enabled = false;
			GameApp.GetInstance().GetWarlock().GetComponent<CSpellbook>().enabled = false;
		}
		else
		{
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockMotor>().enabled = true;
			GameApp.GetInstance().GetWarlock().GetComponent<CSpellbook>().enabled = true;
		}
	}
	
	
	void SetWarlockRoundStartPosition()
	{
		const float kfOffset = -15.0f;
		
		
		GameObject[] aoWarlocks = GameObject.FindGameObjectsWithTag("Wizard") as GameObject[];
		int iNumWarlocks = aoWarlocks.Length; 
		float fPiSlice = (Mathf.PI * 2) / iNumWarlocks;
		
		
		float fPositionX = Mathf.Sin(fPiSlice * GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId());
		float fPositionZ = Mathf.Cos(fPiSlice * GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId());
		
		
		GameApp.GetInstance().GetWarlock().transform.position = new Vector3(fPositionX * kfOffset, 0.0f, fPositionZ * kfOffset);
		GameApp.GetInstance().GetWarlock().transform.LookAt(new Vector3(0.0f, transform.position.y, 0.0f));
	}
	
	
	void SetWarlockWinningPosition()
	{
		if (Network.isServer)
		{
			GameObject[] aoClientObjects = GameObject.FindGameObjectsWithTag("Client Object");
			GameObject oWinningClient = null;
			
			
			foreach (GameObject oClientObject in aoClientObjects)
			{
				
			}
		}
	}
	

	void DisplayWinner()
	{
		//GameApp.GetInstance().DisplaySuccessful(_sString, 10000000);
	}
	
	
	void InitialiseShop()
	{
		m_oShop = gameObject.AddComponent<Shop>();
		m_oShop.enabled = false;

	}
	
	
	// Events:
	
	
	void OnDestroy()
	{
		Destroy(m_cLobby);
		Destroy(m_cScoreBoard);
		Destroy(m_oShop);
		Destroy(m_cChat);
		Destroy(GetComponent<CArenaAwards>());
	}
	
	
}
