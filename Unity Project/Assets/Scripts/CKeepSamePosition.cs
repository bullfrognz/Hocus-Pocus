using UnityEngine;
using System.Collections;

public class CKeepSamePosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(followingObject != null)
		{
			gameObject.transform.position = followingObject.transform.position;	
		}
	}
	
	public GameObject followingObject;
}
