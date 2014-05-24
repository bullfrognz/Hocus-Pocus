using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour
{
	
// Member Types
	
	
	public enum EGameMode
	{
		NORMAL,
		DRAFT,
		WTF
	}
	
	
	public enum ESettings
	{
		PORT 			= 12082,
		CONNECTIONS_MAX = 11
	}
	
	
	public struct TPlayerInfo
	{
		public NetworkPlayer tNetworkPlayer;
		public GameObject oWarlock;
		public string sUsername;
		public uint uiSlotPosition;
	}
	
	
	public struct TSlot
	{
		public NetworkPlayer tOwner;
		public bool bInUse;
	}
	
	
// Member Functions
	
	
	// Public:
	
	
	public void Initialize(string _sServerName, string _sHostUsername)
	{
		//Debug.Log("Creating game....");
		m_sName = _sServerName;
		
	
		RegisterWithMasterServer(_sHostUsername);
		
		
		m_atSlots = new TSlot[(int)ESettings.CONNECTIONS_MAX + 1];
	}
	
	
	public void Shutdown()
	{
		Destroy(GetComponent<CPlayerList>());
		Network.Disconnect();
	}
	
	
	[RPC]
	public void SetGameMode(int _iGameMode)
	{
		m_eGameMode = (EGameMode)_iGameMode;
		
		
		//Debug.Log("Changed game mode: " + ((EGameMode)_iGameMode).ToString());
		
		
		if (Network.isServer)
		{
			networkView.RPC("SetGameMode", RPCMode.OthersBuffered, _iGameMode);
		}
	}
	
	
	public void SetGameMode(EGameMode _eGameMode)
	{
		SetGameMode((int)_eGameMode);
	}
	
	[RPC]
	public void SetNumberOfRounds(int _iRounds)
	{
		m_iRounds = _iRounds;
		
		//Debug.Log("Number of Rounds: " + m_iRounds.ToString());
		
		if (Network.isServer)
		{
			networkView.RPC("SetNumberOfRounds", RPCMode.OthersBuffered, _iRounds);
		}	
	}	
	
	public int GetNumberOfRounds()
	{
		return(m_iRounds);
	}
	
	[RPC]
	public void SetPlayerUsername(string _sUsername, NetworkMessageInfo _tMessageInfo)
	{
		if (!Network.isServer)
		{
			
		}
		else
		{
			TPlayerInfo tConnectedPlayer = new TPlayerInfo();
			GetConnectedPlayer(_tMessageInfo.sender, ref tConnectedPlayer);
			tConnectedPlayer.sUsername = _sUsername;
		}
	}
	
	
	[RPC]
	public void RequestSlotId(NetworkPlayer _tNetworkPlayer)
	{
		if (Network.isServer)
		{
			for (int i = 0; i < (int)ESettings.CONNECTIONS_MAX + 1; ++ i)
			{
				if (m_atSlots[i].bInUse == false)
				{
					m_atSlots[i].bInUse = true;
					m_atSlots[i].tOwner = _tNetworkPlayer;
					
					
					GameObject[] aoAllClientObjects = GameObject.FindGameObjectsWithTag("Client Object") as GameObject[];
		    
			
					foreach(GameObject oClientObject in aoAllClientObjects)
					{
						if (oClientObject.networkView.owner == _tNetworkPlayer)
						{
							if (Network.player == _tNetworkPlayer)
							{
								GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().SetSlotId(i);
								//Debug.LogError("Sent back slot id through function calling: " + i);
							}
							else
							{
								oClientObject.networkView.RPC("SetSlotId", _tNetworkPlayer, i);
								//Debug.LogError("Sent back slot id thorugh RPC: " + i);
							}
							
							
							//GetComponent<CPlayerList>().InitialisePlayerList();
							break;
						}
					}
					
					
					break;
				}
			}
		}
		else
		{
			networkView.RPC("RequestSlotId", RPCMode.Server, _tNetworkPlayer);
		}
	}
	
	
	public TPlayerInfo GetConnectedPlayer(NetworkPlayer _tNetworkPlayer)
	{
		TPlayerInfo tConnectedPlayer = new TPlayerInfo();
		GetConnectedPlayer(_tNetworkPlayer, ref tConnectedPlayer);
		
		
		return (tConnectedPlayer);
	}
	
	
	public TPlayerInfo[] GetConnectedPlayers()
	{
		return (m_taConnectedPlayers);
	}
	
	
	public EGameMode GetGameMode()
	{
		return (m_eGameMode);
	}
	
	
	public void NotifyStartGame()
	{
		if (Network.isServer)
		{
			Network.maxConnections = -1;
			MasterServer.RegisterHost(m_ksMasterServerUniqueId, m_sName, "---Game Started---//!!``");
		}
	}
	
	// Private:
	
	
	void Start()
	{
		m_taConnectedPlayers = new TPlayerInfo[(int)ESettings.CONNECTIONS_MAX];
		gameObject.name = "Obj_Server";
	}
	

	void Update()
	{
		
	}
	
	void RegisterWithMasterServer(string _sHostUsername)
	{
		MasterServer.RegisterHost(m_ksMasterServerUniqueId, m_sName, "[" + _sHostUsername + "]");
	}
	
	
	bool GetConnectedPlayer(NetworkPlayer _tNetworkPlayer, ref TPlayerInfo _rtNetworkPlayer)
	{
		bool bFound = false;
		
		
		for (uint i = 0; i < m_taConnectedPlayers.Length; ++ i)
		{
			if (m_taConnectedPlayers[i].tNetworkPlayer.guid == _tNetworkPlayer.guid)
			{
				_rtNetworkPlayer = m_taConnectedPlayers[i];
				bFound = true;
				break;
			}
		}
		
		
		if (!bFound)
		{
			Debug.LogError("Could not find requested connected player!");
			Debug.Break();
		}
		
		
		return (bFound);
	}
	
	
	// Events:
	
	
	void OnServerInitialized()
	{
       // Debug.Log("Server initialized and ready to go!");
    }
	
	
	void OnMasterServerEvent(MasterServerEvent _eEvent)
	{
		//Debug.LogError("Master server event: " + _eEvent.ToString());
    }
	
	
	void OnFailedToConnectToMasterServer(NetworkConnectionError _eConnectionError)
	{
		//Debug.LogError("Master server connection failed: " + _eConnectionError.ToString());
	}
	
	
	void OnPlayerConnected(NetworkPlayer _Player)
	{
		//Debug.LogError("A player has connected from the server");
	}
	
	
	void OnPlayerDisconnected(NetworkPlayer _tPlayer)
	{
		//Debug.LogError("A player has disconnected from the server");
		
	
	    GameObject[] aoAllGameObjects = GameObject.FindGameObjectsWithTag("Client Object") as GameObject[];
	    
		
		foreach(GameObject oGameObject in aoAllGameObjects)
		{
			if (oGameObject.activeInHierarchy)
			{
				if (oGameObject.networkView != null &&
					oGameObject.networkView.owner == _tPlayer)
				{
					oGameObject.networkView.RPC("CleanUpThisPlayer", RPCMode.AllBuffered);
				}
			}
		}
		
		
		// Free up slot id
		for (int i = 0; i < (int)ESettings.CONNECTIONS_MAX + 1; ++ i)
		{
			if (m_atSlots[i].bInUse &&
				m_atSlots[i].tOwner == _tPlayer)
			{
				m_atSlots[i].bInUse = false;
			}
		}
		
		
		Network.RemoveRPCs(_tPlayer);
        Network.DestroyPlayerObjects(_tPlayer);
	
	}
	
	
// Variables
	
	
	// Public:
	
	
	public const string m_ksMasterServerUniqueId = "Hocus-Pocus -149ujf;'[0i230ri";
	
	
	
	// Private:
	
	
	TPlayerInfo[] m_taConnectedPlayers;
	
	
	string m_sName;
	
	
	EGameMode m_eGameMode;
	int m_iRounds = 3;
	
	
	TSlot[] m_atSlots;

	
}
