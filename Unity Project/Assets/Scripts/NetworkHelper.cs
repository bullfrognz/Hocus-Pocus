using UnityEngine;
using System.Collections;

public class NetworkHelper : MonoBehaviour
{
	
	
	public static UnityEngine.Component AddNetworkView(GameObject _oObject, UnityEngine.Component _cWatchComponent)
	{
		return (AddNetworkView(_oObject, _cWatchComponent, Network.AllocateViewID()));
	}
	
	
	public static UnityEngine.Component AddNetworkView(GameObject _oObject, UnityEngine.Component _cWatchComponent, NetworkViewID _tViewId)
	{
	    NetworkView cNetworkView = _oObject.AddComponent<NetworkView>();
	    cNetworkView.observed = _cWatchComponent;
	    cNetworkView.stateSynchronization = NetworkStateSynchronization.ReliableDeltaCompressed;
		cNetworkView.viewID =  _tViewId;
		
		
		//Debug.Log("Created network view" + cNetworkView);
		
		
		return (cNetworkView);
	}
	

}
