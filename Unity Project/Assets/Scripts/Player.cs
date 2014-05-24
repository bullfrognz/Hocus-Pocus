using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	
// Member Types
	
	
// Member Functions
	
	
	public void SetId(uint _uiId)
	{
		m_uiPlayerId = _uiId;
	}
	
	
	public void SetUsername(string _sUsername)
	{
		m_sUsername = _sUsername;
	}
	
	
	public void SetNetworkPlayer(NetworkPlayer _NetworkPlayer)
	{
		m_NetworkPlayer = _NetworkPlayer;
	}
	
	
	public uint GetId()
	{
		return (m_uiPlayerId);
	}
	
	
	public string GetUsername()
	{
		return (m_sUsername);
	}
	
	
	public NetworkPlayer GetNetworkPlayer()
	{
		return (m_NetworkPlayer);
	}
	
	
	//public GameObject GetWarlock()
	//{
		//return (m_Warlock);
	//}


	private void Start()
	{
	
	}
	

	private void Update()
	{
	
	}
	
	
// Member Variables
	
	
	string m_sUsername;
	
	
	NetworkPlayer m_NetworkPlayer;
	//GameObject m_Warlock;
	
	
	uint m_uiPlayerId;
	
	
}
