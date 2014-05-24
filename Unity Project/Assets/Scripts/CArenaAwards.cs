using UnityEngine;
using System.Collections;
using System;

public class CArenaAwards : MonoBehaviour
{	
	public static readonly uint s_uiKillReward = 2;
	public static readonly uint s_uiFirstBloodReward = 2;
	public static readonly uint s_uiFirstPlaceReward = 3;
	public static readonly uint s_uiSecondPlaceReward = 2;
	public static readonly uint s_uiThirdPlaceReward = 1;
	public static readonly uint s_uiRoundReward = 10;
	
	bool m_bFirstBloodTaken;
	
	CPlayerList m_cPlayerList;
	
	int m_iStartingPlayers;
	public int m_iPlayersDead;
	
	//NetworkPlayer

	// Use this for initialization
	void Start () 
	{
		InitialiseAwards();
		
		gameObject.AddComponent<CArenaEventNotifier>();
	}
	
	void OnDestroy()
	{
		Destroy(GetComponent<CArenaEventNotifier>());
	}
	
	public void InitialiseAwards()
	{
		m_bFirstBloodTaken = false;
		m_cPlayerList = GameApp.GetInstance().GetPlayerList();
		
		m_iStartingPlayers = m_cPlayerList.Count();
		m_iPlayersDead = 0;
	}
	
	// Called by the person who died
	public void ReportSuicide()
	{
		//Debug.Log("Announced Suicide");
		
		int iSlotID = GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId();
		networkView.RPC("AnnounceSuicide", RPCMode.AllBuffered, iSlotID);
		
		
		if(!Network.isServer)
		{
			networkView.RPC("CheckPlacings", RPCMode.Server, networkView.owner, iSlotID);
		}
		else
		{
			CheckPlacings(networkView.owner, iSlotID);
		}
	}

	// Called by the person who died
	public void ReportKill(int _iKillerSlotId)
	{
		CClientInfo cKillerClientInfo = m_cPlayerList.RetrieveNetPlayerInfo(_iKillerSlotId);
		

		
		if(!m_bFirstBloodTaken)
		{
			//Debug.Log("Announced First blood");
			Color colGuy = cKillerClientInfo.GetWarlock().GetComponent<CWarlockController>().transform.FindChild("Point light").light.color;
			string hexColGuy = ColorToHex(colGuy);
			
			networkView.RPC("AnnounceFirstBlood", RPCMode.All, ("<color=" + hexColGuy + ">" + cKillerClientInfo.GetUsername() + "</color>"));
			
			if(!Network.isServer)
			{
				networkView.RPC("AwardFirstBlood", RPCMode.Server, _iKillerSlotId);
			}
			else
			{
				AwardFirstBlood(_iKillerSlotId);
			}
		}
			
		//Debug.Log("Announced Kill");
		int iMyID = GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId();
		networkView.RPC("AnnounceKill", RPCMode.All, cKillerClientInfo.GetSlotId(), iMyID);
		
		if(!Network.isServer)
		{
			networkView.RPC("AwardKillReward", RPCMode.Server, _iKillerSlotId);
		}
		else
		{
			AwardKillReward(_iKillerSlotId);
		}
		
		if(!Network.isServer)
		{
			networkView.RPC("CheckPlacings", RPCMode.Server, networkView.owner, iMyID);
		}
		else
		{
			CheckPlacings(networkView.owner, iMyID);
		}
	}

	[RPC]
	void CheckPlacings(NetworkPlayer _netPlayer, int _iPlayerSlotID)
	{
		//Debug.Log("Checked placing");	
		//Debug.Log(m_iStartingPlayers);
		//Debug.Log(m_iPlayersDead);
		int iPlace = m_iStartingPlayers - m_iPlayersDead;
		if(iPlace >= 0 &&
			iPlace <= 2) 
		{
			//networkView.RPC("AnnouncePlacingAward", RPCMode.All, _iPlayerSlotID, iPlace);
			
			if(GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId() == _iPlayerSlotID)
			{
				AwardFirstPlaceReward(iPlace);
			}
			else
			{
				networkView.RPC("AwardFirstPlaceReward", m_cPlayerList.RetrieveNetPlayerInfo(_iPlayerSlotID).GetNetworkPlayer(), iPlace);
			}
		}
	}
	
//	void OnGUI()
//	{
//		GUI.Label(new Rect(200, 10, 100,100), m_iPlayersDead.ToString());
//	}
	
	
	// Call at end of round
	public void AwardRoundBonus()
	{	
		//Debug.Log("Earnt: " + s_uiRoundReward.ToString() + " for round.");
		GameApp.GetInstance().GetWarlock().GetComponent<CWarlockCurrency>().Add(CArenaAwards.s_uiRoundReward);
	}
	

	
	[RPC]
	void AwardKillReward(int _iKillerSlotID)
	{
		if(Network.isServer)
		{
			if(GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId() == _iKillerSlotID)
			{
				AwardMoney((int)CArenaAwards.s_uiKillReward);
			}
			else
			{
				networkView.RPC("AwardMoney", m_cPlayerList.RetrieveNetPlayerInfo(_iKillerSlotID).GetNetworkPlayer(), (int)CArenaAwards.s_uiKillReward);
			}
		}
	}

	[RPC]
	void AwardMoney(int _iAmount)
	{
		//Debug.LogError("Earnt: " + _iAmount.ToString());
		GameApp.GetInstance().GetWarlock().GetComponent<CWarlockCurrency>().Add((uint)_iAmount);
		GetComponent<CArenaEventNotifier>().AddCrystalMessage("+ " + _iAmount.ToString());
	}
	
	[RPC]
	void AwardFirstBlood(int _iKillerSlotID)
	{
		if(Network.isServer)
		{
			if(GameApp.GetInstance().GetClientObj().GetComponent<CClientInfo>().GetSlotId() == _iKillerSlotID)
			{
				AwardMoney((int)CArenaAwards.s_uiFirstBloodReward);
			}
			else
			{
				networkView.RPC("AwardMoney", m_cPlayerList.RetrieveNetPlayerInfo(_iKillerSlotID).GetNetworkPlayer(), (int)CArenaAwards.s_uiFirstBloodReward);
			}
		}
	}
	
	
	[RPC]
	void AnnounceKill(int _iKillerID, int _iKilledID)
	{	
		
		//Debug.Log(m_cPlayerList.RetrieveNetPlayerInfo(_iKillerID).GetUsername() + " has defeated " +	m_cPlayerList.RetrieveNetPlayerInfo(_iKilledID).GetUsername() + "!");
		Color colKiller = m_cPlayerList.RetrieveNetPlayerInfo(_iKillerID).GetWarlock().GetComponent<CWarlockController>().transform.FindChild("Point light").light.color;
		Color colKilled = m_cPlayerList.RetrieveNetPlayerInfo(_iKilledID).GetWarlock().GetComponent<CWarlockController>().transform.FindChild("Point light").light.color;
		
		string hexColKiller = ColorToHex(colKiller);
		string hexColKilled = ColorToHex(colKilled);
		
		string strKiller = "<color=" + hexColKiller + ">" + m_cPlayerList.RetrieveNetPlayerInfo(_iKillerID).GetUsername() + "</color>";
		string strKilled = "<color=" + hexColKilled + ">" + m_cPlayerList.RetrieveNetPlayerInfo(_iKilledID).GetUsername() + "</color>";
		
		GetComponent<CArenaEventNotifier>().AddMessage(strKiller + " has knocked out " + strKilled);
	}
		
	[RPC]
	void AnnounceSuicide(int _iSinner)
	{
		//Debug.Log(m_cPlayerList.RetrieveNetPlayerInfo(_iSinner).GetUsername() + " has committed sucide!");.
		Color colGuy = m_cPlayerList.RetrieveNetPlayerInfo(_iSinner).GetWarlock().GetComponent<CWarlockController>().transform.FindChild("Point light").light.color;
		string hexColGuy = ColorToHex(colGuy);
		
		GetComponent<CArenaEventNotifier>().AddMessage("<color=" + hexColGuy + ">" + m_cPlayerList.RetrieveNetPlayerInfo(_iSinner).GetUsername() + "</color> knocked himself out!");
	}
	
	// This needs to be sent to everyone so that the bool is set
	[RPC]
	public void AnnounceFirstBlood(string _sPlayer)
	{
		m_bFirstBloodTaken = true;
		
		//Debug.Log(_sPlayer + " got first blood!");
		
		GetComponent<CArenaEventNotifier>().AddMessage(_sPlayer  + " earnt " + s_uiFirstBloodReward.ToString() + " crystals earning first blood!");
	}
	
	[RPC]
	void AwardFirstPlaceReward(int _iPlacing)
	{
		if(_iPlacing == 0)
		{
			//Debug.LogError("1ST: " + s_uiFirstPlaceReward.ToString());
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockCurrency>().Add(CArenaAwards.s_uiFirstPlaceReward);
		}
		else if(_iPlacing == 1)
		{
			//Debug.LogError("2ND: " + s_uiSecondPlaceReward.ToString());
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockCurrency>().Add(CArenaAwards.s_uiSecondPlaceReward);
		}
		else if(_iPlacing == 2)
		{
			//Debug.LogError("3RD: " + s_uiThirdPlaceReward.ToString());
			GameApp.GetInstance().GetWarlock().GetComponent<CWarlockCurrency>().Add(CArenaAwards.s_uiThirdPlaceReward);
		}
	}
	
	[RPC]
	void AnnouncePlacingAward(int _iSlotID, int _iPlace)
	{
		string sPlace = "nth";
		string sReward = "0";
		
		if(_iPlace == 0)
		{
			sPlace = "1st";
			sReward = CArenaAwards.s_uiFirstPlaceReward.ToString();
		}
		else if(_iPlace == 1)
		{
			sPlace = "2nd";
			sReward = CArenaAwards.s_uiSecondPlaceReward.ToString();
		}
		else if(_iPlace == 2)
		{
			sPlace = "3rd";
			sReward = CArenaAwards.s_uiThirdPlaceReward.ToString();
		}
		
		Color colGuy = m_cPlayerList.RetrieveNetPlayerInfo(_iSlotID).GetComponent<CWarlockController>().transform.FindChild("Point light").light.color;
		string hexColGuy = ColorToHex(colGuy);	
		
		GetComponent<CArenaEventNotifier>().AddMessage("<color=" + hexColGuy + ">" + m_cPlayerList.RetrieveNetPlayerInfo(_iSlotID).GetUsername()  + "</color> earnt " + sReward + " crystals for coming " + sPlace + "!");
	}
	
	// http://wiki.unity3d.com/index.php?title=HexConverter
	static public string ColorToHex(Color32 color)
	{
		string hex = "#"+ color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
		
//		if("#1CE6BBFF" == hex)
//		{
//			// This is an ugly fix for cyan (player 3)
//			return("#00E6BBFF");
//		}
		return hex;
	}
}
