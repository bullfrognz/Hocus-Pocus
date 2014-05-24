using UnityEngine;
using System.Collections;


public class GameApp : MonoBehaviour
{
	
// Member Types
	
	
	public enum EScene
	{
		INVALID = -1,
		
		MAIN_MENU,
		ARENA,
		
		MAX
	}
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	static GameApp s_cInstance;
	
	
	Color m_vErrorTextColour;
	
	
	GameObject m_oClient;
	GlobalTerrain m_cTerrian;
	CSpellData m_cSpellData;
	
	
	string m_sUsername;
	string m_sServerName;
	string m_sErrorText;
	
	
	EScene m_eScene 		= EScene.INVALID;
	EScene m_eChangeScene 	= EScene.MAIN_MENU;
	
	Font m_fFont;
	
	float m_fErrorTextTimer;

	
// Member Functions
	
	
	// Public:
	
	
	static public GameApp GetInstance()
	{
		return (s_cInstance);
	}
	
	
	public bool CreateGame(string _sServerName, string _sHostUsername)
	{
		m_sUsername = _sHostUsername;
		m_sServerName = _sServerName;
		
		
		bool bReturn =  false;
		bool bUseNat = !Network.HavePublicAddress();
		
		
		NetworkConnectionError eConnectionError = Network.InitializeServer((int)Server.ESettings.CONNECTIONS_MAX, (int)Server.ESettings.PORT, bUseNat);

		
		if (eConnectionError != NetworkConnectionError.NoError)
		{
			DisplayError("Failed to initialise server: " + eConnectionError.ToString(), 3);
		}
		else
		{
			bReturn = true;
		}
		
		
		return (bReturn);
	}
	
	
	public bool JoinGame(string _sServerIP, string _sUsername)
	{
		m_sUsername = _sUsername;
		
		
		bool bSuccessful = true;
		NetworkConnectionError eConnectionError = Network.Connect(_sServerIP, (int)Server.ESettings.PORT);
	
			
		if (eConnectionError != NetworkConnectionError.NoError)
		{
			bSuccessful = false;
		}
		
		
		Network.sendRate = 60;
			
	
		return (bSuccessful);
	}
	
	
	public void ChangeScene(EScene _eScene)
	{
		m_eChangeScene = _eScene;
	}
	
	
	public GameObject GetClientObj()
	{
		if (m_oClient == null)
		{
			Debug.LogError("Client has not been created yet!");
		}
		
		
		return (m_oClient);
	}
	
	
	public Server GetServer()
	{
		return (GameObject.Find("Obj_Server").GetComponent<Server>());
	}
	
	
	public GameObject GetServerObject()
	{
		return (GameObject.Find("Obj_Server"));
	}
	
	
	public GameObject GetWarlock()
	{
		return (m_oClient.GetComponent<CClientInfo>().GetWarlock());
	}
	
	
	public Camera GetGameCamera()
	{
		return (Camera.main.camera);
	}
	
	
	public EScene GetScene()
	{
		return (m_eScene);
	}
	
	public CSceneMainMenu GetSceneMainMenu()
	{
		return (GetComponent<CSceneMainMenu>());
	}
	
	public CSceneArena GetSceneArena()
	{
		return (GetComponent<CSceneArena>());
	}
	
	public CSpellData GetSpellDataComponent()
	{
		return(m_cSpellData);
	}
	
	public CCursor GetCursor()
	{
		return(GetGameCamera().GetComponent<CCursor>());
	}
	
	public void DisplayError(string _sText, float _fDuration)
	{
		m_vErrorTextColour = Color.red;
		m_fErrorTextTimer = _fDuration;
		m_sErrorText = _sText;
	}
	
	
	public void DisplaySuccessful(string _sText, float _fDuration)
	{
		m_vErrorTextColour = Color.green;
		m_fErrorTextTimer = _fDuration;
		m_sErrorText = _sText;
	}
	
	public CPlayerList GetPlayerList()
	{
		return(GetSceneArena().GetPlayerList());
	}
	
	// Private:
	
	
	void Awake()
	{
		s_cInstance = this;
	}
	
	
	void Start()
	{
		Application.runInBackground = true;
		
		
		m_cSpellData = gameObject.AddComponent(typeof(CSpellData)) as CSpellData;
		gameObject.AddComponent(typeof(CFogSystem));
		m_cTerrian = gameObject.AddComponent(typeof(GlobalTerrain)) as GlobalTerrain;
		
		m_fFont = Resources.Load("Fonts/NewRocker-Large") as Font;
		
	}
	
	void OnGUI()
	{
		GUI.skin.font = m_fFont;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.richText = true;
		
		if (m_fErrorTextTimer > 0.0f)
		{
			GUIStyle cMyStyle = new GUIStyle();
			cMyStyle.alignment = TextAnchor.MiddleCenter;
			cMyStyle.fontSize = 50;
			cMyStyle.normal.textColor = m_vErrorTextColour;
			
			
			float fBoxHeight = 40;
			float fBoxWidth  = 10 * m_sErrorText.Length;
			float fPositionX = Screen.width / 2  - fBoxWidth / 2;
			float fPositionY = Screen.height - 100 - fBoxHeight / 2;
			
			
			GUI.TextField(new Rect (fPositionX, fPositionY, fBoxWidth, fBoxHeight), m_sErrorText, cMyStyle);
		}
	}
	
	
	float m_fFPSTimer;
	uint m_uiNumFrames;
	
	
	void Update()
	{
		++ m_uiNumFrames;
		m_fFPSTimer += Time.deltaTime;
		
		
		if (m_fFPSTimer > 0.5f)
		{
			//Debug.Log("Frames: " + m_uiNumFrames * 2);
			m_fFPSTimer -= 0.5f;
			m_uiNumFrames = 0;
		}
		
		
		if(m_eChangeScene != EScene.INVALID)
		{
			HandleSceneChange();
		}
		
		
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (Network.isClient ||
				Network.isServer)
			{
				GetClientObj().GetComponent<CClientConnection>().Disconnect();
			}
			else
			{
				Application.Quit();
			}
		}
		
		
		ProcessErrorText();
	}
	
	
	void ProcessErrorText()
	{
		if (m_fErrorTextTimer > 0.0f)
		{
			m_fErrorTextTimer -= Time.deltaTime;
		}
	}
	
	
	void HandleSceneChange()
	{
		if (m_eChangeScene == EScene.MAIN_MENU)
		{
			GetGameCamera().GetComponent<CCameraController>().ResetToStartPosition();
			GetGameCamera().GetComponent<CCameraController>().StopFadingPanning();
			GetCursor().SetVisible(true);
			GetCursor().SetIconLit(false);
			GetCursor().SetSideScrollingEnabled(false);
			
			
			m_cTerrian.Terrain_ShutDown();
			Destroy(GetComponent<CSceneArena>());
			gameObject.AddComponent<CSceneMainMenu>();
		}
		else if (m_eChangeScene == EScene.ARENA)
		{
			Destroy(GetComponent<CSceneMainMenu>());
			gameObject.AddComponent<CSceneArena>();
		}
		else
		{
			Debug.LogError("Unknown scene");
		}
		
		
		m_eScene = m_eChangeScene;
		m_eChangeScene = EScene.INVALID;
	}
	
	
	// Events:
	
	
	// Server did not have it's own info, so this function was created and set when clientrelay is made in the server script - Mana 12/05
	public void SetClient(GameObject _oClient)
	{
		m_oClient = _oClient;
	}
	
	
	void OnServerInitialized()
	{
		GameObject oServer = Network.Instantiate(Resources.Load("Prefabs/Obj_Server", typeof(GameObject)), Vector3.zero, Quaternion.identity, 0) as GameObject;
		oServer.GetComponent<Server>().Initialize(m_sServerName, m_sUsername);
			
			
		OnConnectedToServer();
		//Debug.Log("Server initialized and ready to go!");
    }
	
	
	void OnConnectedToServer()
	{
		if (m_oClient == null)
		{
			//Debug.Log("Connected client created");
			m_oClient = Network.Instantiate(Resources.Load("Prefabs/Obj_Client", typeof(GameObject)), Vector3.zero, Quaternion.identity, 0) as GameObject;
			m_oClient.GetComponent<CClientInfo>().SetUsername(m_sUsername);
			
			
			Network.sendRate = 100;
		}
	}
	
	
	void OnDisconnectedFromServer()
	{
		DisplayError("Disconnected from server", 3);
		
		
		Destroy(GetServerObject());	
		
	
		// Destory all client objects
	    GameObject[] aoClientObjects = GameObject.FindGameObjectsWithTag("Client Object") as GameObject[];
	    
		
		foreach(GameObject oClientObject in aoClientObjects)
		{
			Destroy(oClientObject);
		}
		
		
		ChangeScene(EScene.MAIN_MENU);
	}
	
	
	void OnFailedToConnect(NetworkConnectionError _eConnectionError)
	{
		DisplayError("Failed to join game: " + _eConnectionError.ToString(), 3);
	}
	
	
	void OnFailedToConnectToMasterServer(NetworkConnectionError _eConnectionError)
	{
		DisplayError("Master server connection failed: " + _eConnectionError.ToString(), 3);
	}
	
	
}