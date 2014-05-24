using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour
{

// Member Types
	
	
// Member Functions
	
	
	// Public:
	
	
	public bool JoinGame(string _sServerIP)
	{
		bool bSuccessful = true;
		NetworkConnectionError eConnectionError = Network.Connect(_sServerIP, (int)Server.ESettings.PORT);
	
			
		if (eConnectionError != NetworkConnectionError.NoError)
		{
			bSuccessful = false;
		}
			
	
		return (bSuccessful);
	}
	
	
	public void Disconnect()
	{
		Network.Disconnect();
	}
	
	// Commented to remove warning - Mana 23/05
//	public bool IsConnected()
//	{
//		return (Network.connections[0] != null);
//	}

	
	// Private:
	
	
	void Start()
	{
		// Empty
	}
		

	void Update()
	{
		// Empty
	}
	
	
	// Events:
	
	
	void OnConnectedToServer()
	{
		Debug.Log("Successfully connected to server!");
	}
	
	
	void OnDisconnectedFromServer()
	{
		Debug.Log("Disconnected from server.");
	}
	
	
	void OnFailedToConnect(NetworkConnectionError _eConnectionError)
	{
		Debug.Log("Failed to join game: " + _eConnectionError.ToString());
	}
	
	
	void OnNetworkInstantiate(NetworkMessageInfo _tMessageInfo)
	{
		Debug.Log("An object was instantiated over the network");
	}
	
	
	[RPC]
	void OnLogin()
	{
		Debug.LogError("CAEK IS A LIE");
	}
	
	
// Member Variables
	
	
	// Public:
	
	
	// Protected:
	
	
	// Private:
	

}
