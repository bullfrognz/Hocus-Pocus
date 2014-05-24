using UnityEngine;

public class GlobalTerrain : MonoBehaviour
{
	[RPC]
	public void Terrain_Initialise(int iSeed, int iNumPieces)
	{
		Terrain_ShutDown();
		
		m_fTimeUntilNextFalloff = m_fTimeBetweenFalloff;
		
		GameObject templateTerrainPiece = (GameObject)Resources.Load("Prefabs/GroundBlock", typeof(GameObject));
		float fOuterRadius = templateTerrainPiece.transform.localScale.x;
		float fInnerRadius = fOuterRadius * 0.86602540378443864676372317075294f;	// (cos(30°)
		Vector2 translateX = new Vector2(fInnerRadius*2, 0);
		Vector2 translateY = new Vector2(fInnerRadius, fInnerRadius*1.7320508075688772935274463415059f);	// fInnerRadius*1.7320508075688772935274463415059 == √((fInnerRadius+fInnerRadius)² - fInnerRadius²)
		Vector2 translateZ = new Vector2(fInnerRadius, fInnerRadius*-1.7320508075688772935274463415059f);	// fInnerRadius*1.7320508075688772935274463415059 == √((fInnerRadius+fInnerRadius)² - fInnerRadius²)
		Random.seed = iSeed;	// Seed the random number generator, so all clients (including the host) generate the same terrain.
		
		uint uiTotalRecommendation = 0;
		System.Collections.Generic.Dictionary<long,Adjacent> builder = new System.Collections.Generic.Dictionary<long,Adjacent>();
		
		// Create all terrain pieces.
		for(uint uiLoop = 0; uiLoop < iNumPieces; ++uiLoop)
		{
			// Find where the new piece will be placed.
			Adjacent.Index newPos = new Adjacent.Index(0,0,0);
			if(uiLoop != 0)	// If this is not the first piece...
			{
				// Spawn from one of the recommended locations, using the Roulette Wheel genetic algorithm.
				uint uiSelection = (uint)Random.Range(0, (int)uiTotalRecommendation);
				
				foreach(System.Collections.Generic.KeyValuePair<long,Adjacent> pair in builder)
				{
					// Ignore pieces that have already been populated.
					if(pair.Value.m_TerrainPiece != null)
						continue;
					
					// Before using Roulette Wheel selection, ensure the pieces of terrain that are part of the initial solid arena are created.
					newPos.SetAs(pair.Key);
					if(pair.Value.m_uiRecommendation >= 5 && new Vector3(translateX.x*newPos.x + translateY.x*newPos.y + translateZ.x*newPos.z, templateTerrainPiece.transform.position.y, translateX.y*newPos.x + translateY.y*newPos.y + translateZ.y*newPos.z).magnitude - fOuterRadius <= m_fRadiusOfInitialSolidArena)
						break;	// Create this piece by using 'newPos'.
					
					// Check if this is the winner of Roulette Wheel selection.
					if(uiSelection < pair.Value.m_uiRecommendation)
						break;	// Create this piece by using 'newPos'.
					else	// Not winner.
						uiSelection -= pair.Value.m_uiRecommendation;
				}
			}
			
			// Create new terrain piece.
			GameObject terrainPiece =
				(GameObject)GameObject.Instantiate
				(
					templateTerrainPiece,
					new Vector3
					(	
						translateX.x*newPos.x + translateY.x*newPos.y + translateZ.x*newPos.z,
						templateTerrainPiece.transform.position.y,
						translateX.y*newPos.x + translateY.y*newPos.y + translateZ.y*newPos.z
					),
					templateTerrainPiece.transform.localRotation
				);
			
			terrainPiece.name = "Terrain " + uiLoop + " (" + newPos.x+"x "+newPos.y+"y "+newPos.z+"z)";
			
			Adjacent adjacent = null;
			if(builder.TryGetValue(newPos, out adjacent))
			{
				// If the terrain piece in 'adjacent' is already set, then the terrain has been generated far too large - no more pieces can be created.
				if(adjacent.m_TerrainPiece != null)
					break;	// Stop creating terrain.
				
				adjacent.m_TerrainPiece = terrainPiece;
			}
			else
			{
				adjacent = new Adjacent(terrainPiece);
				builder.Add(newPos, adjacent);
			}
			
			if(terrainPiece.transform.position.magnitude - fOuterRadius <= m_fRadiusOfSmallestArena)	// If this piece of terrain is within the radius of the final arena, then preserve this piece...
				m_TerrainPieces_Preserved.Add(terrainPiece);	// Add this piece to the list of terrain pieces that will not crumble.
			else	// Outside the radius of the smallest arena...
				m_TerrainPieces.Insert(0, terrainPiece);	// Add this piece to the list of terrain pieces that can crumble.
			
			uiTotalRecommendation -= adjacent.m_uiRecommendation;
			
			// Increment recommendation of all surrounding pieces (plus create them if they do not exist yet).
			Adjacent.Index relativePos;
			
			relativePos = newPos.Relative(-1,-0,-0);
			if(relativePos.x >= -1048576)	// Only bother if the value is within range of the bits allocated (20 bits).
			{
				if(!builder.TryGetValue(relativePos, out adjacent))
				{
					adjacent = new Adjacent();
					builder.Add(relativePos, adjacent);
				}
				
				if(adjacent.m_TerrainPiece == null)
				{
					++adjacent.m_uiRecommendation;
					++uiTotalRecommendation;
				}
			}
			
			relativePos = newPos.Relative(+1,+0,+0);
			if(relativePos.x < 1048576)	// Only bother if the value is within range of the bits allocated (20 bits).
			{
				if(!builder.TryGetValue(relativePos, out adjacent))
				{
					adjacent = new Adjacent();
					builder.Add(relativePos, adjacent);
				}
				
				if(adjacent.m_TerrainPiece == null)
				{
					++adjacent.m_uiRecommendation;
					++uiTotalRecommendation;
				}
			}
			
			relativePos = newPos.Relative(-0,-1,-0);
			if(relativePos.y >= -1048576)	// Only bother if the value is within range of the bits allocated (20 bits).
			{
				if(!builder.TryGetValue(relativePos, out adjacent))
				{
					adjacent = new Adjacent();
					builder.Add(relativePos, adjacent);
				}
				
				if(adjacent.m_TerrainPiece == null)
				{
					++adjacent.m_uiRecommendation;
					++uiTotalRecommendation;
				}
			}
			
			relativePos = newPos.Relative(+0,+1,+0);
			if(relativePos.y < 1048576)	// Only bother if the value is within range of the bits allocated (20 bits).
			{
				if(!builder.TryGetValue(relativePos, out adjacent))
				{
					adjacent = new Adjacent();
					builder.Add(relativePos, adjacent);
				}
				
				if(adjacent.m_TerrainPiece == null)
				{
					++adjacent.m_uiRecommendation;
					++uiTotalRecommendation;
				}
			}
			
			relativePos = newPos.Relative(-0,-0,-1);
			if(relativePos.z >= -1048576)	// Only bother if the value is within range of the bits allocated (20 bits).
			{
				if(!builder.TryGetValue(relativePos, out adjacent))
				{
					adjacent = new Adjacent();
					builder.Add(relativePos, adjacent);
				}
				
				if(adjacent.m_TerrainPiece == null)
				{
					++adjacent.m_uiRecommendation;
					++uiTotalRecommendation;
				}
			}
			
			relativePos = newPos.Relative(+0,+0,+1);
			if(relativePos.z < 1048576)	// Only bother if the value is within range of the bits allocated (20 bits).
			{
				if(!builder.TryGetValue(relativePos, out adjacent))
				{
					adjacent = new Adjacent();
					builder.Add(relativePos, adjacent);
				}
				
				if(adjacent.m_TerrainPiece == null)
				{
					++adjacent.m_uiRecommendation;
					++uiTotalRecommendation;
				}
			}
		}
	}
	
	[RPC]
	public void Terrain_Pause(bool pause)
	{
		m_bPause = pause;
		
		foreach(GameObject terrainPiece in m_TerrainPieces_Falling)
		{
			terrainPiece.GetComponent<Rigidbody>().isKinematic = pause;
		}
	}
	
	[RPC]
	public void Terrain_ShutDown()
	{
		foreach(GameObject terrainPiece in m_TerrainPieces)
		{
			GameObject.Destroy(terrainPiece);
		}
		m_TerrainPieces.Clear();
		
		foreach(GameObject terrainPiece in m_TerrainPieces_Preserved)
		{
			GameObject.Destroy(terrainPiece);
		}
		m_TerrainPieces_Preserved.Clear();
		
		foreach(SRumblingTerrain terrainPiece in m_TerrainPieces_Rumbling)
		{
			GameObject.Destroy(terrainPiece.terrainPiece);
		}
		m_TerrainPieces_Rumbling.Clear();
		
		foreach(GameObject terrainPiece in m_TerrainPieces_Falling)
		{
			GameObject.Destroy(terrainPiece);
		}
		m_TerrainPieces_Falling.Clear();
		
		m_bPause = true;
	}
	
	void OnDisconnectedFromServer()
	{
		Terrain_ShutDown();
	}
	
	[RPC]
	public void TerrainPiece_Rumble()
	{
		if(m_TerrainPieces.Count > 0)
		{
			// Make a piece fall.
			SRumblingTerrain newRumblingPiece = new SRumblingTerrain();
			newRumblingPiece.terrainPiece = m_TerrainPieces[0];
			newRumblingPiece.timeRemaining = m_fTimeSpentRumbling;
			
			m_TerrainPieces_Rumbling.Add(newRumblingPiece);
			m_TerrainPieces.RemoveAt(0);
		}
		else
		{
			Debug.LogError("Was told to rumble terrain piece when there was none left");
		}
	}

	// Use this for initialization
	void Start()
	{
		
	}
	
	// Update is called once per frame
	void Update()
	{
		if(m_bPause)
		{
			return;
		}
		
		// Handle rumbling terrain pieces.
		uint uiNumPiecesToRemove = 0;
		foreach(SRumblingTerrain obj in m_TerrainPieces_Rumbling)
		{
			obj.timeRemaining -= Time.deltaTime;
			if(obj.timeRemaining <= 0)
			{
				m_TerrainPieces_Falling.Add(obj.terrainPiece);
				obj.terrainPiece.GetComponent<Rigidbody>().isKinematic = false;	// Can now move.
				obj.terrainPiece.GetComponent<Rigidbody>().WakeUp();	// Found out the hard way this is necessary.
				obj.terrainPiece.GetComponent<BoxCollider>().enabled = false;
				++uiNumPiecesToRemove;
			}
			else	// Continue rumbling.
			{
				float randomRotation = Random.Range(-90.0f, 90.0f);
				obj.terrainPiece.transform.Rotate(Mathf.Sin(randomRotation),0,Mathf.Cos(randomRotation));
			}
		}
		
		// Remove terrain pieces that have rumbled for long enough (STUPID C# LIMITATION: Removing while iterating breaks the iteration, so iteration must restart every time).
		while(uiNumPiecesToRemove > 0)
		{
			foreach(SRumblingTerrain obj in m_TerrainPieces_Rumbling)
			{
				if(obj.timeRemaining <= 0)
				{
					--uiNumPiecesToRemove;
					m_TerrainPieces_Rumbling.Remove(obj);
					break;
				}
			}
		}
		
		if(Network.isServer)
		{
			// Handle rumble event.
			m_fTimeUntilNextFalloff -= Time.deltaTime;
			if(m_fTimeUntilNextFalloff <= 0)
			{
				m_fTimeUntilNextFalloff += 	m_fTimeBetweenFalloff;	// Prepare for the next time pieces will rumble.
				
				// Initialise variables for this fall.
				m_uiNumPiecesToFall += m_uiNumPiecesPerFalloff;
				m_fTimeUntilNextPieceFalls = Time.deltaTime;	// Delta is subtracted per frame, so this makes a piece rumble immediately without messing with the next piece.
			}
			
			// Handle each piece falling in an event.
			if(m_uiNumPiecesToFall > 0)	// If there are pieces of the terrain that must rumble...
			{
				// Wait until it is the right time to make pieces fall.
				m_fTimeUntilNextPieceFalls -= Time.deltaTime;
				while(m_fTimeUntilNextPieceFalls <= 0 && m_uiNumPiecesToFall > 0)	// For every piece that must fall this frame...
				{
					// Formalities for processing.
					m_fTimeUntilNextPieceFalls += m_fTimeBetweenEachPieceFalloff;
					--m_uiNumPiecesToFall;
					
					if(m_TerrainPieces.Count > 0)	// If there are any terrain pieces left...
					{
						// Make a piece rumble.
						networkView.RPC("TerrainPiece_Rumble", RPCMode.AllBuffered);
					}
				}
			}
		}
		// Else is client.
		
		// Handle fallen pieces fading out and ultimately being deleted.
		foreach(GameObject obj in m_TerrainPieces_Falling)
		{
			if(obj.transform.position.y <= m_fDeleteTerrainBelowHeight)
			{
				m_TerrainPieces_Falling.Remove(obj);
				GameObject.Destroy(obj);
				break;	// Can't continue the foreach loop after messing up the iterator.
			}
		}
	}
	
	class SRumblingTerrain
	{
		public GameObject terrainPiece;
		public float timeRemaining;
	}
	
	class Adjacent
	{
		public uint m_uiRecommendation = 0;	// The chance (relative to other pieces) that this will spawn.
		public GameObject m_TerrainPiece = null;
		
		public Adjacent(){}
		
		public Adjacent(GameObject _TerrainPiece)
		{
			m_TerrainPiece = _TerrainPiece;
		}
		
		public class Index
		{
			public int x;
			public int y;
			public int z;
			
			public void SetAs(long _r)
			{
				x = (int)(_r&0xFFFFF);
				y = (int)((_r >> 20)&0xFFFFF);
				z = (int)((_r >> 40)&0xFFFFF);
				
				if(((_r>>60)&1) == 1)
					x |= -1 ^ 0xFFFFF;
					
				if(((_r>>61)&1) == 1)
					y |= -1 ^ 0xFFFFF;
				
				if(((_r>>62)&1) == 1)
					z |= -1 ^ 0xFFFFF;
			}
			
			public Index(int _x, int _y, int _z)
			{
				x = _x;
				y = _y;
                z = _z;

                // Error correction.
                while (y > 0 && z > 0)
                { ++x; --y; --z; }

                while (y < 0 && z < 0)
                { --x; ++y; ++z; }

                while (y < 0 && x > 0)
                { --x; ++y; ++z; }

                while (y > 0 && x < 0)
                { ++x; --y; --z; }

                while (x > 0 && z < 0)
                { --x; ++y; ++z; }

                while (x < 0 && z > 0)
                { ++x; --y; --z; }
			}
			
			public static implicit operator long(Index _a)
			{
				long ret = (long)((((ulong)(_a.z&0xFFFFF))<<40) | (((ulong)(_a.y&0xFFFFF))<<20) | ((ulong)(_a.x&0xFFFFF)));
				ret |= ((long)(_a.x>>31)&1)<<60;
				ret |= ((long)(_a.y>>31)&1)<<61;
				ret |= ((long)(_a.z>>31)&1)<<62;
				return ret;
			}
			
			public Index Relative(int _x, int _y, int _z)
			{
				return new Index(x + _x, y + _y, z + _z);
			}
		}
	}
	
	System.Collections.Generic.List<GameObject> m_TerrainPieces = new System.Collections.Generic.List<GameObject>();
	System.Collections.Generic.List<GameObject> m_TerrainPieces_Preserved = new System.Collections.Generic.List<GameObject>();
	System.Collections.Generic.List<SRumblingTerrain> m_TerrainPieces_Rumbling = new System.Collections.Generic.List<SRumblingTerrain>();
	System.Collections.Generic.List<GameObject> m_TerrainPieces_Falling = new System.Collections.Generic.List<GameObject>();
	
	public float m_fRadiusOfSmallestArena = 3.0f;
	public float m_fRadiusOfInitialSolidArena = 15.0f;	// Note: This is based off the warlock's starting position.
	public uint m_uiNumPiecesPerFalloff = 40;
	public float m_fTimeBetweenFalloff = 3.0f;
	public float m_fTimeBetweenEachPieceFalloff = 0.02f;
	public float m_fTimeSpentRumbling = 1.5f;	// Terrain rumbles before it falls.
	public float m_fDeleteTerrainBelowHeight = -5;
	
	float m_fTimeUntilNextFalloff;
	float m_fTimeUntilNextPieceFalls;
	uint m_uiNumPiecesToFall = 0;
	bool m_bPause = true;
}
