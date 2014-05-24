using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class displays who killed whom, who placed what etc...

public class CArenaEventNotifier: MonoBehaviour 
{
	public class CEventMessage
	{
		public string sMessage;
		public float fTimeElapsed;
	};
	
	public class CCrystalMessage
	{
		public GameObject oText;
		public float fTimeElapsed;
	};
	
	public Font font3D = Resources.Load("Fonts/NewRocker-Regular") as Font;
	
	private const float m_fMessageLife = 4.0f;
	private List<CEventMessage> m_listMessages = new List<CEventMessage>();
	private List<CCrystalMessage> m_listCrystalMessages = new List<CCrystalMessage>();
	GUIStyle m_cFontStyle;
	
	private const int m_iFontSize = 25;
	

	public void AddMessage(string _sMessage)
	{
		CEventMessage tNewMessage = new CEventMessage();
		tNewMessage.sMessage = _sMessage;
		tNewMessage.fTimeElapsed = m_fMessageLife;
		m_listMessages.Add(tNewMessage);
	}
	
	public void AddCrystalMessage(string _sMessage)
	{
		CCrystalMessage tNewMessage = new CCrystalMessage();
		tNewMessage.oText = new GameObject("3DText");
		TextMesh mesh = tNewMessage.oText.AddComponent<TextMesh>();
		tNewMessage.oText.AddComponent<MeshRenderer>();
		mesh.alignment = TextAlignment.Center;
		mesh.anchor = TextAnchor.MiddleCenter;
		mesh.renderer.material= font3D.material;
		mesh.font = font3D;
		mesh.text = _sMessage;
		mesh.offsetZ = -1;
		Vector3 vPos = GameApp.GetInstance().GetWarlock().GetComponent<CWarlockController>().transform.position;
		vPos.y += 5;
		mesh.transform.position = vPos;
		tNewMessage.fTimeElapsed = 2.0f;
		m_listCrystalMessages.Add(tNewMessage);
	}
	
	void Start()
	{
		m_cFontStyle = new GUIStyle();
		m_cFontStyle.fontSize = m_iFontSize;
		m_cFontStyle.alignment = TextAnchor.MiddleCenter;
		m_cFontStyle.normal.textColor = Color.white;
		m_cFontStyle.richText = true;
	}
	
	void Update()
	{
		int iRemoveRange = 0;
		foreach(CEventMessage tMsg in m_listMessages)
		{
			tMsg.fTimeElapsed -= Time.deltaTime;
			
			if(tMsg.fTimeElapsed < 0.0f)
			{
				++iRemoveRange;
			}
		}
		
		m_listMessages.RemoveRange(0, iRemoveRange);
		
		// Crystals
		iRemoveRange = 0;
		foreach(CCrystalMessage tMsg in m_listCrystalMessages)
		{
			tMsg.fTimeElapsed -= Time.deltaTime;
			
			if(tMsg.fTimeElapsed < 0.0f)
			{
				++iRemoveRange;
				
				Destroy(tMsg.oText);
			}
		}
		
		m_listCrystalMessages.RemoveRange(0, iRemoveRange);
	
//		if(Input.GetKeyUp(KeyCode.Space))
//		{
//			AddCrystalMessage("+1");
//		}
//		
//		if(Input.GetKeyUp(KeyCode.Space))
//		{
//			string hexColor = CArenaAwards.ColorToHex(GameApp.GetInstance().GetWarlock().GetComponent<CWarlockController>().transform.FindChild("Point light").light.color);
//			AddMessage("<color="+hexColor+">afasfasfas</color>");
//		}
		
		if(GameApp.GetInstance().GetScene() == GameApp.EScene.ARENA)
		{
			if(m_listCrystalMessages.Count > 0)
			{
				foreach(CCrystalMessage tMsg in m_listCrystalMessages)
				{
					Vector3 vPos = GameApp.GetInstance().GetWarlock().GetComponent<CWarlockController>().transform.position;
					vPos.y = tMsg.oText.transform.position.y;
					Color col = tMsg.oText.renderer.material.color;
					col.a -= Time.deltaTime * 0.5f;
					vPos.y += Time.deltaTime * 0.5f;
			
					tMsg.oText.transform.position = vPos;
					tMsg.oText.renderer.material.color = col;
				}
			}
		}
		
	}
	
	void OnGUI()
	{
		int iY = m_iFontSize + 5;
		Rect rLabel = UnityGUIExt.CreateRect(0,iY,0, 150,UnityGUIExt.GUI_ALLIGN.TOP_CENTRE, UnityGUIExt.GUI_ALLIGN.TOP_CENTRE);
		
		foreach(CEventMessage tMsg in m_listMessages)
		{
			Color CColorOriginal = m_cFontStyle.normal.textColor;
			Color cColorTemp = m_cFontStyle.normal.textColor;
			
			if(tMsg.fTimeElapsed < m_fMessageLife - 1.0f)
			{
				cColorTemp.a = ((tMsg.fTimeElapsed) / (m_fMessageLife - 1.5f)) * m_cFontStyle.normal.textColor.a;
			}
			m_cFontStyle.normal.textColor = cColorTemp;
			GUI.Label(rLabel, tMsg.sMessage, m_cFontStyle);
			m_cFontStyle.normal.textColor = CColorOriginal;
			rLabel.y += iY;
		}
	}
	
	void OnDestroy()
	{
		//Debug.Log ("dasdas");
		foreach(CCrystalMessage tMsg in m_listCrystalMessages)
		{
			Destroy(tMsg.oText);
		}
	}
}
