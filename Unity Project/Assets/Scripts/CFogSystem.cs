using UnityEngine;
using System.Collections;

public class CFogSystem : MonoBehaviour
{
	public float m_fFogRadius = 80.0f;
//	public float m_fMinX = -60.0f;
//	public float m_fMaxX = 60.0f;
	public float m_fMinY = -3.0f;
	public float m_fMaxY = -0.125f;
//	public float m_fMinZ = -30.0f;
//	public float m_fMaxZ = 40.0f;
	public uint m_uiNum = 300;	// Number of textured quads with the fog texture on them.
	public float m_fWindVelocity = 1.0f;	// Game units per second.
	public float m_fMinVelocity = 0.75f;	// Game units per second.
	public float m_fMaxVelocity = 1.0f;		// Game units per second.
	public float m_fMinRotation = -10.0f;	// Degrees per second.
	public float m_fMaxRotation = 10.0f;	// Degrees per second.
	
	Vector2 m_WindDir = Random.insideUnitCircle;
	
	public class SFogPiece	// There's more than just the GameObject that needs to be tracked.
	{
		public GameObject m_Fog;
		public float m_fAngularVelocity;
		public Vector2 m_LinearVelocity;
		public Vector3 m_RelativePosToCamera;
	}
	
	System.Collections.Generic.List<SFogPiece> m_Fog = new System.Collections.Generic.List<SFogPiece>();
	
	void Start()
	{
		Vector3 cameraPos = new Vector3(Camera.main.transform.position.x, (m_fMinY + m_fMaxY)*0.5f, Camera.main.transform.position.z);
		GameObject templateFogPiece = (GameObject)Resources.Load("Prefabs/FogPiece", typeof(GameObject));
		
		// Create all fog pieces.
		for(uint ui = 0; ui < m_uiNum; ++ui)
		{
			float randDir = Random.Range(0, 360);
			float fDistFromOrigin = m_fFogRadius * Mathf.Sqrt(Random.Range(0.0f, 1.0f));
			//Debug.Log(fDistFromOrigin);
			
			SFogPiece fogPiece = new SFogPiece();
			fogPiece.m_Fog = (GameObject)GameObject.Instantiate(templateFogPiece);
			
			// Generate random starting position.
			fogPiece.m_Fog.transform.position = new Vector3(cameraPos.x + Mathf.Cos(randDir) * fDistFromOrigin, Random.Range(m_fMinY, m_fMaxY), cameraPos.z + Mathf.Sin(randDir) * fDistFromOrigin);
			
			// Generate random starting rotation.
			fogPiece.m_Fog.transform.Rotate(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
			
			// Generate random rotation speed.
			fogPiece.m_fAngularVelocity = Random.Range(m_fMinRotation, m_fMaxRotation);
			
			// Generate random individual velocity (for variation from wind).
			float fLinearVelocity = Random.Range(m_fMinVelocity, m_fMaxVelocity);
			fogPiece.m_LinearVelocity = Random.insideUnitCircle * fLinearVelocity;
			fogPiece.m_RelativePosToCamera = fogPiece.m_Fog.transform.position - cameraPos;
			
			m_Fog.Add(fogPiece);
		}
	}
	
	void Update()
	{
		Vector3 cameraPos = new Vector3(Camera.main.transform.position.x, (m_fMinY + m_fMaxY)*0.5f, Camera.main.transform.position.z);
		Ray rayInfo = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
		if(rayInfo.direction.y < 0)
			cameraPos = rayInfo.origin + (rayInfo.direction * ((-rayInfo.origin.y) / rayInfo.direction.y));
		
		foreach(SFogPiece fogPiece in m_Fog)
		{
			// Adjust fog position if outside the allowed area.
			if((fogPiece.m_Fog.transform.position - cameraPos).sqrMagnitude > m_fFogRadius*m_fFogRadius)
			{
				Vector3 newPos = cameraPos - fogPiece.m_RelativePosToCamera;
				newPos.y = fogPiece.m_Fog.transform.position.y;
				fogPiece.m_Fog.transform.position = newPos;
			}
			
			fogPiece.m_RelativePosToCamera = fogPiece.m_Fog.transform.position - cameraPos;
			
			// Update rotation based on angular velocity.
			fogPiece.m_Fog.transform.Rotate(0.0f, fogPiece.m_fAngularVelocity * Time.deltaTime, 0.0f);
			
			// Update position based on linear velocity + wind velocity.
			Vector3 translation = new Vector3((fogPiece.m_LinearVelocity.x + m_WindDir.x * m_fWindVelocity) * Time.deltaTime, 0.0f, (fogPiece.m_LinearVelocity.y + m_WindDir.y * m_fWindVelocity) * Time.deltaTime);
			fogPiece.m_Fog.transform.Translate(translation, Space.World);
			
//			if(fogPiece.m_Fog.transform.position.x < m_fMinX)
//				fogPiece.m_Fog.transform.Translate(m_fMaxX - m_fMinX, 0.0f, 0.0f, Space.World);
//			else if(fogPiece.m_Fog.transform.position.x > m_fMaxX)
//				fogPiece.m_Fog.transform.Translate(-(m_fMaxX - m_fMinX), 0.0f, 0.0f, Space.World);
//			
//			if(fogPiece.m_Fog.transform.position.z < m_fMinZ)
//				fogPiece.m_Fog.transform.Translate(0.0f, 0.0f, m_fMaxZ - m_fMinZ, Space.World);
//			else if(fogPiece.m_Fog.transform.position.z > m_fMaxZ)
//				fogPiece.m_Fog.transform.Translate(0.0f, 0.0f, -(m_fMaxZ - m_fMinZ), Space.World);
		}
	}
}
