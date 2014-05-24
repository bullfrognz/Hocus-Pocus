using UnityEngine;
using System.Collections;


public class ServerScanner : MonoBehaviour
{
	
// Member Types
	
	
	public struct TServerInfo
	{
		public string[] saIp;
		public string sGameName;
		public string sGameType;
		public string sComment;
		public string sGuid;
		public uint uiPlayerLimit;
		public uint uiNumConnectedPlayers;
		public uint uiPort;
		public bool bPasswordProtected;
	}
	
	
// Member Functions
	
	
	// Public:
	
	
	public TServerInfo[] GetServerList()
	{
		return (m_taOnlineServers);
	}
	
	
	// Private:
	
	
	void Start()
	{
		// Emtpy
	}
	

	void Update()
	{
		ProcessRefresh();
	}
	
	
	void ProcessRefresh()
	{
		if (m_bScanning &
			!Network.isServer)
		{
			m_fRefreshTimer += Time.deltaTime;
			
			
			if (m_fRefreshTimer > m_kfRefreshInterval)
			{
				UpdateServerList();
				MasterServer.ClearHostList();
				MasterServer.RequestHostList(Server.m_ksMasterServerUniqueId);
				m_fRefreshTimer = 0.0f;
			}
		}
	}
	
	
	void UpdateServerList()
	{
		HostData[] acHostData = MasterServer.PollHostList();
		TServerInfo[] taOnlineServers = new TServerInfo[acHostData.Length];
		int iNumActiveServers = 0;

		
		if (acHostData.Length > 0)
		{
			for (int i = 0; i < acHostData.Length; ++ i)
			{
				// Game started
				if (acHostData[i].comment == "---Game Started---//!!``")
				{
					continue;
				}
				
				
				taOnlineServers[iNumActiveServers].saIp 					= acHostData[i].ip;
				taOnlineServers[iNumActiveServers].sGameName 				= acHostData[i].gameName;
				taOnlineServers[iNumActiveServers].sGameType 				= acHostData[i].gameType;
				taOnlineServers[iNumActiveServers].sComment 				= acHostData[i].comment;
				taOnlineServers[iNumActiveServers].sGuid 					= acHostData[i].guid;
				taOnlineServers[iNumActiveServers].uiPlayerLimit 			= (uint)acHostData[i].playerLimit;
				taOnlineServers[iNumActiveServers].uiNumConnectedPlayers 	= (uint)acHostData[i].connectedPlayers;
				taOnlineServers[iNumActiveServers].uiPort 					= (uint)acHostData[i].port;
				taOnlineServers[iNumActiveServers].bPasswordProtected 		= acHostData[i].passwordProtected;
				
				
				++ iNumActiveServers;
			}
		}
		
		
		m_taOnlineServers = new TServerInfo[iNumActiveServers];
		int iLol = 0;
		
		
		for (int i = 0; i < iNumActiveServers; ++ i)
		{
			if (taOnlineServers[i].sGameName.Length == 0)
			{
				continue;
			}
			
			
			m_taOnlineServers[iLol] = taOnlineServers[i];
			
			
			++ iLol;
		}
	}
	
	public void RefreshServerList()
	{
		UpdateServerList();
		MasterServer.ClearHostList();
		MasterServer.RequestHostList(Server.m_ksMasterServerUniqueId);
	}
	
	
	// Events:
	
	
	void OnConnectedToServer()
	{
		//m_bScanning = false;
	}
	
	
	void OnDisconnectedFromServer()
	{
		m_bScanning = true;
	}
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	TServerInfo[] m_taOnlineServers;
	
	
	const float m_kfRefreshInterval = 1.0f;
	float m_fRefreshTimer   	    = 0.0f;
	
	
	bool m_bScanning = true;
	
	
}