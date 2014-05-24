using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CScoreBoard : MonoBehaviour
{
	enum EColumn
	{
		Name,
		Kills,
		Deaths,
		Score,
		MAX
	}
	
	// Scoreboard data structure
	struct ScoreboardVarStruct
	{
		public bool bAlive;
		public string strName;
		public int iKills;
		public int iDeaths;
		public int iScore;
		
		public ScoreboardVarStruct(string _Name, int _iKills, int _iDeaths, int _iScore)
		{
			bAlive = true;
			strName = _Name;
			iKills = _iKills;
			iDeaths = _iDeaths;
			iScore = _iScore;
		}
	}
	
	// Scoreboard dimensions.
	float mfScoreboardWidthScalar = 0.8f;	// Percentage of total screen width.
	float mfScoreboardHeightScalar = 0.8f;	// Percentage of total screen height.
	uint muiNumRows = 20;	// Number of rows in the scoreboard. Includes header.
	
	// Text position.
	float mfTextHeightScalar	= 0.9f;	// Percentage of the height of each row.
	float mfTextWidthScalar		= 0.95f;	// Percentage of the width used in each row.
	
	string[] maColumnName;
	
	// Scoreboard
	System.Collections.Generic.SortedDictionary<int, ScoreboardVarStruct> mPlayers = new System.Collections.Generic.SortedDictionary<int, ScoreboardVarStruct>();
	
	[RPC]
	void Scoreboard_AddWarlock(int _iSlotID, string _Name, int _iKills, int _iDeaths, int _iScore)
	{
		if(mPlayers.ContainsKey(_iSlotID))
		{
			Debug.LogError("Attempted to add player " + _iSlotID + ", but they already exist!");
			return;
		}
		
		// Add new player.
		mPlayers.Add(_iSlotID, new ScoreboardVarStruct(_Name, _iKills, _iDeaths, _iScore));
	}
	
	[RPC]
	void Scoreboard_RemoveWarlock(int _iSlotID)
	{
		if(!mPlayers.ContainsKey(_iSlotID))
		{
			Debug.LogError("Attempted to remove player " + _iSlotID + ", but they do not exist!");
			return;
		}
		
		mPlayers.Remove(_iSlotID);
	}
	
	[RPC]
	void Scoreboard_Kill(int _iSlotID)
	{
		ScoreboardVarStruct player;
		if(mPlayers.TryGetValue(_iSlotID, out player))
			++player.iKills;
		else
			Debug.LogError("Attempted to add kill to player " + _iSlotID + ", but they do not exist!");
	}
	
	[RPC]
	void Scoreboard_Death(int _iSlotID)
	{
		ScoreboardVarStruct player;
		if(mPlayers.TryGetValue(_iSlotID, out player))
			++player.iDeaths;
		else
			Debug.LogError("Attempted to add death to player " + _iSlotID + ", but they do not exist!");
	}
	
	[RPC]
	void Scoreboard_Score(int _iSlotID, int _iScoreModifier)
	{
		ScoreboardVarStruct player;
		if(mPlayers.TryGetValue(_iSlotID, out player))
			player.iScore += _iScoreModifier;
		else
			Debug.LogError("Attempted to modify score of player " + _iSlotID + ", but they do not exist!");
	}
	
	// Startup
	void Start()
	{
		maColumnName = new string[(int)EColumn.MAX];
		maColumnName[(int)EColumn.Name] = "Name";
		maColumnName[(int)EColumn.Kills] = "Kills";
		maColumnName[(int)EColumn.Deaths] = "Deaths";
		maColumnName[(int)EColumn.Score] = "Score";
	}

	// Called once per frame
	void Update()
	{
		
	}
	
	void OnGUI()
	{
		if (Input.GetKey(KeyCode.Tab))
		{
			// Get dimensions for the scoreboard.
			float fScoreboardWidth	= Screen.width * mfScoreboardWidthScalar;
			float fScoreboardHeight	= Screen.height * mfScoreboardHeightScalar;
			float fScoreboardLeft = (Screen.width - fScoreboardWidth)/2;
			float fScoreboardTop = (Screen.height - fScoreboardHeight)/2;
			float fTextLeft = fScoreboardLeft + fScoreboardWidth * (1.0f - mfTextWidthScalar);
			float fRowHeight = fScoreboardHeight / muiNumRows;
			float fTextTop = fScoreboardTop + fRowHeight * (1.0f - mfTextHeightScalar);
			float fTextWidth = fScoreboardWidth * mfTextWidthScalar;
			float fColumnWidth = fTextWidth/(int)EColumn.MAX;
			float fTextHeight = fScoreboardHeight * mfTextHeightScalar;
			
			// Sort the scores
			System.Collections.Generic.SortedList<int, ScoreboardVarStruct> scoreboard = new System.Collections.Generic.SortedList<int, ScoreboardVarStruct>(mPlayers.Count);
			foreach(System.Collections.Generic.KeyValuePair<int, ScoreboardVarStruct> pair in mPlayers)
				scoreboard.Add(pair.Value.iScore, pair.Value);
			
			// Scoreboard.
			GUI.Box(new Rect(	fScoreboardLeft,
								fScoreboardTop,
								fScoreboardWidth,
								fScoreboardHeight),
								"");
			
			// Print each of the column headers.
			for(int i = 0; i < (int)EColumn.MAX; ++i)
			{
				GUI.Label(new Rect(	fTextLeft + fColumnWidth*i,
									fTextTop,
									fColumnWidth,
									fTextHeight),
									maColumnName[i]);
			}
			
			UnityEngine.Color oldGuiColour = GUI.color;	// Remember old GUI colour to restore it after drawing everything.
			
			// For each client
			uint uiPlayer = 0;
			foreach(System.Collections.Generic.KeyValuePair<int, ScoreboardVarStruct> pair in scoreboard)
			{
				++uiPlayer;	// +1 beforehand to skip row used by headers.
				
				string[] aPlayerStats = new string[(int)EColumn.MAX];
				aPlayerStats[(int)EColumn.Name] = pair.Value.strName;
				aPlayerStats[(int)EColumn.Kills] = pair.Value.iKills.ToString();
				aPlayerStats[(int)EColumn.Deaths] = pair.Value.iDeaths.ToString();
				aPlayerStats[(int)EColumn.Score] = pair.Value.iScore.ToString();
				
				// Gray out 'dead' players
				GUI.color = pair.Value.bAlive ? Color.white : Color.gray;
				
				// Print out stats for each column.
				for(uint uiStat = 0; uiStat < (uint)EColumn.MAX; ++uiStat)
				{
					GUI.Label(new Rect(	fTextLeft + fColumnWidth*uiStat,
										fTextTop + fRowHeight * uiPlayer,
										fColumnWidth,
										fTextHeight),
										pair.Value.strName);
				}
			}
			
			// Reset GUI colour.
			GUI.color = oldGuiColour;
		}
	}
}