using UnityEngine;
using System.Collections;


public class CClientInfo : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	[RPC]
	public void CreateWarlock(NetworkViewID _tTransformViewId, NetworkViewID _tAnimationViewId, NetworkViewID _tHealthViewId)
	{
		//Vector3 vStartPosition = new Vector3(Random.Range(-15, 15), 3.0f, Random.Range(-15, 15));
		//Quaternion vStartRotation = new Quaternion();
		//m_oWarlock = Network.Instantiate(Resources.Load("Prefabs/Warlock", typeof(GameObject)), vStartPosition, vStartRotation, 0) as GameObject;	
		
		// Make my warlock on my side
		m_oWarlock = Resources.Load("Prefabs/Warlock", typeof(GameObject)) as GameObject;
		m_oWarlock = GameObject.Instantiate(m_oWarlock) as GameObject;
		m_oWarlock.GetComponent<CWarlockController>().Initialise(_tTransformViewId, _tAnimationViewId, _tHealthViewId);
			
		// Make my warlock on everyone elses side
		if (networkView.isMine)
		{
			networkView.RPC("CreateWarlock", RPCMode.OthersBuffered, _tTransformViewId, _tAnimationViewId, _tHealthViewId);
		}
		
		m_netPlayer = networkView.owner;
	}
	
	
	[RPC]
	public void SetUsername(string _sUsername)
	{
		m_sUsername = _sUsername;
		
		
		if (networkView.isMine)
		{
			networkView.RPC("SetUsername", RPCMode.OthersBuffered, _sUsername);
		}
	}
	
	
	[RPC]
	public void SetSlotId(int _iSlotId)
	{
		//Debug.Log("Slot Id recieved: " + _iSlotId);
		
		
		m_iSlotId = _iSlotId;
		m_oWarlock.GetComponent<CWarlockController>().NotifySlotId(m_iSlotId);
		
		
		if (networkView.isMine)
		{
			//Debug.LogError("Told others about my new slot id: " + _iSlotId);
			networkView.RPC("SetSlotId", RPCMode.OthersBuffered, _iSlotId);
		}
	}
	
	
//	public NetworkPlayer GetNetworkPlayer()
//	{
//		return (Network.player);
//	}
	
	public NetworkPlayer GetNetworkPlayer()
	{
		return (networkView.owner);
	}
	
	
	
	public string GetUsername()
	{
		return (m_sUsername);
	}
	
	
	public GameObject GetWarlock()
	{
		if (m_oWarlock == null)
		{
			Debug.LogError("Trying to get warlock that does not exist");
			Debug.DebugBreak();
		}
		
		
		return (m_oWarlock);
	}
	
	
	public int GetSlotId()
	{
		return (m_iSlotId);
	}
	
	
	// Private:
	
	
	void Awake()
	{
		if (networkView.isMine)
		{
			CreateWarlock(Network.AllocateViewID(), Network.AllocateViewID(), Network.AllocateViewID());
		}
	}
	
	
	void Start()
	{
		gameObject.name = "Obj_Client";
		
		
		if (networkView.isMine)
		{
			//SetSlotId(Network.player.GetHashCode());
		}
	}
	

	void Update()
	{
		if (networkView.isMine)
		{
			if (!m_bRequestedSlotId && 
				GameApp.GetInstance().GetServerObject() != null)
			{
				//Debug.LogError("Requested slot id");
				
				
				GameApp.GetInstance().GetServer().RequestSlotId(Network.player);
				
				
				m_bRequestedSlotId = true;
			}
		}
	}
	
	
	void OnDestroy()
	{
		Destroy(m_oWarlock);
	}
	
	
	// Events:
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	string m_sUsername;
	
	
	public GameObject m_oWarlock;
	
	
	int m_iSlotId;
	
	
	bool m_bRequestedSlotId = false;
	
	NetworkPlayer m_netPlayer;
	
	
}
