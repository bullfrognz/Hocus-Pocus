using UnityEngine;
using System.Collections;

public class CClientConnection : MonoBehaviour
{

// Member Types
	
	
// Member Functions
	
	
	// Public:
	
	
	[RPC]
	public void CleanUpThisPlayer()
	{
		Destroy(gameObject);
	}
	
	
	public void Disconnect()
	{
		Network.Disconnect();
	}
	
	
	public bool IsConnected()
	{
		return (Network.isClient);
	}

	
	// Private:
	
	
	void Start()
	{
	}
		

	void Update()
	{
	}
	
	
	// Events:
	
	
	void OnConnectedToServer()
	{
		Debug.Log("Successfully connected to server!");
	}

	
	void OnNetworkInstantiate(NetworkMessageInfo _tMessageInfo)
	{
		// Empty
	}
	
	
// Member Variables
	
	
	// Public:
	
	
	// Protected:
	
	
	// Private:
	

}
