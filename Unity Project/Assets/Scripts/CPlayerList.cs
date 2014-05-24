using UnityEngine;
using System.Collections;

public class CPlayerList : MonoBehaviour 
{
	ArrayList m_aPlayerInfo = new ArrayList();
	bool m_bRefresh = false;
	
	public void InitialisePlayerList()
	{	
		m_aPlayerInfo.Clear();
		m_aPlayerInfo = new ArrayList();
		 
		GameObject[] aoClients = GameObject.FindGameObjectsWithTag("Client Object");
		foreach(GameObject oPlayer in aoClients)
		{
			m_aPlayerInfo.Add(oPlayer.GetComponent<CClientInfo>());
		}

	}
	
	
	void Update()
	{
		if(m_bRefresh)
		{
			InitialisePlayerList();
		}
	}
	
//	void OnGUI()
//	{
//		InitialisePlayerList();
//		float fStart = 20;
//		float fOffset = 30;
//		
//		GUI.Label(new Rect(0, 0, 100, 25), "Players, ID:");
//		
//		foreach(CClientInfo cInfo in m_aPlayerInfo)
//		{
//			GUI.Label(new Rect(10, fStart, 100, 25), cInfo.GetUsername() + " " + cInfo.GetNetworkPlayer().ToString());
//			fStart+=fOffset;
//		}
//		
//		fStart+=fOffset;
//		GUI.Label(new Rect(0, fStart, 100, 25), "Connected ID:");
//		fStart+=fOffset;
//		
//		NetworkPlayer[] players = Network.connections;
//		foreach(NetworkPlayer cplayer in players)
//		{
//			GUI.Label(new Rect(10, fStart, 100, 25), cplayer.ToString());
//			fStart+=fOffset;
//		}
//	}
	
	public ArrayList GetPlayerInfo()
	{
		return(m_aPlayerInfo);
	}
	
	public CClientInfo RetrieveNetPlayerInfo(int _iPlayerSlotId)
	{
		GameObject[] aoClients = GameObject.FindGameObjectsWithTag("Client Object");
		
		
		foreach(GameObject oClient in aoClients)
		{
			CClientInfo cClientInfo = oClient.GetComponent<CClientInfo>();
			
			
			if(cClientInfo.GetSlotId() == _iPlayerSlotId)
			{
				return(cClientInfo);
			}
		}
		
		Debug.LogError("Could not retrieve player info.");
		
		return(null);
	}
	
	public CClientInfo RetrieveNetPlayerInfo(NetworkPlayer _netPlayer)
	{
		GameObject[] aoClients = GameObject.FindGameObjectsWithTag("Client Object");
		
		
		foreach(GameObject oClient in aoClients)
		{
			CClientInfo cClientInfo = oClient.GetComponent<CClientInfo>();
			
			
			if(cClientInfo.GetNetworkPlayer() == _netPlayer)
			{
				return(cClientInfo);
			}
		}
		
		Debug.LogError("Could not retrieve player info.");
		
		return(null);
	}
	
	void OnPlayerConnected(NetworkPlayer _Player)
	{
		m_bRefresh = true;
	}
	
	
	void OnPlayerDisconnected(NetworkPlayer _tPlayer)
	{
		m_bRefresh = true;
	}
	
	public int Count()
	{
		return(m_aPlayerInfo.Count);
	}
}
