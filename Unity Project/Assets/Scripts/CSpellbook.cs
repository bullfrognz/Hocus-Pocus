using UnityEngine;
using System.Collections;
using System;


public class CSpellbook : MonoBehaviour
{
	
// Member Types
	
	
	public enum ESettings
	{
		SPELLS_MAX = 8,
	}

	
	
// Member Functions
	
	
	// Public:
	
	// Only use this function to initialise the warlock spells.
	private void AddSpell(CSpell.EType _eSpellType, uint _uiSpellbookSlot)
	{
		switch (_eSpellType)
		{
		case CSpell.EType.SONIC_WAVE:
			m_acSpellBook[_uiSpellbookSlot] = gameObject.AddComponent<CSonicWave>();
			break;
			
		case CSpell.EType.FIREBALL:
			m_acSpellBook[_uiSpellbookSlot] = gameObject.AddComponent<CSpellFireball>();
			break;
			
		case CSpell.EType.TELEPORT:
			m_acSpellBook[_uiSpellbookSlot] = gameObject.AddComponent<CTeleport>();
			break;
			
		case CSpell.EType.SWAP:
			m_acSpellBook[_uiSpellbookSlot] = gameObject.AddComponent<CSpellSwap>();
			break;
			
		case CSpell.EType.HOMING:
			m_acSpellBook[_uiSpellbookSlot] = gameObject.AddComponent<CSpellHoming>();
			break;
			
		case CSpell.EType.METEOR:
			m_acSpellBook[_uiSpellbookSlot] = gameObject.AddComponent<CSpellMeteor>();
			break;
			
		case CSpell.EType.TETHER:
			m_acSpellBook[_uiSpellbookSlot] = gameObject.AddComponent<CSpellTether>();
			break;
			
		case CSpell.EType.BLACKHOLE:
			m_acSpellBook[_uiSpellbookSlot] = gameObject.AddComponent<CSpellBlackHole>();
			break;
			
		case CSpell.EType.SHEILD:
			//m_acSpellBook[_uiSpellbookSlot] = gameObject.AddComponent<CSpellTether>();
			break;
		}
		
		m_acSpellBook[_uiSpellbookSlot].Upgrade();
		
		++m_uiCurNumSpells;
	}
	
	// This version is used by the shop
	public void AddSpellToFreeSlot(CSpell.EType _eSpellType)
	{
		if(m_uiCurNumSpells < (uint)CSpell.EType.SPELL_MAX)
		{
			// Find next free slot
			uint uiFreeSlot = 0;
			foreach(CSpell oSpell in m_acSpellBook)
			{
				if(oSpell == null)
				{
					break;
				}
				++uiFreeSlot;
			}
			
			switch (_eSpellType)
			{
			case CSpell.EType.SONIC_WAVE:
				m_acSpellBook[uiFreeSlot] = gameObject.AddComponent<CSonicWave>();
				break;
				
			case CSpell.EType.FIREBALL:
				m_acSpellBook[uiFreeSlot] = gameObject.AddComponent<CSpellFireball>();
				break;
				
			case CSpell.EType.TELEPORT:
				m_acSpellBook[uiFreeSlot] = gameObject.AddComponent<CTeleport>();
				break;
				
			case CSpell.EType.SWAP:
				m_acSpellBook[uiFreeSlot] = gameObject.AddComponent<CSpellSwap>();
				break;
				
			case CSpell.EType.HOMING:
				m_acSpellBook[uiFreeSlot] = gameObject.AddComponent<CSpellHoming>();
				break;
				
			case CSpell.EType.METEOR:
				m_acSpellBook[uiFreeSlot] = gameObject.AddComponent<CSpellMeteor>();
				break;
				
			case CSpell.EType.TETHER:
				m_acSpellBook[uiFreeSlot] = gameObject.AddComponent<CSpellTether>();
				break;
				
			case CSpell.EType.BLACKHOLE:
				m_acSpellBook[uiFreeSlot] = gameObject.AddComponent<CSpellBlackHole>();
				break;
				
			case CSpell.EType.SHEILD:
				//m_acSpellBook[uiFreeSlot] = gameObject.AddComponent<CSpellTether>();
				break;
			}
			
			m_acSpellBook[uiFreeSlot].Upgrade();
			
			++m_uiCurNumSpells;
		}
	}
	
	
//	UnityEngine.Component GetSpell(CSpell.EType _eSpellType)
//	{
//		UnityEngine.Component cSpell = new UnityEngine.Component();
//		
//		
//		switch (_eSpellType)
//		{
//		case CSpell.EType.SONIC_WAVE:
//			//cSpell = GetComponent();
//			break;
//			
//		case CSpell.EType.FIREBALL:
//			cSpell = GetComponent<CSpellFireball>();
//			break;
//			
//		case CSpell.EType.TELEPORT:
//			break;
//		}
//		
//		
//		return (cSpell);
//	}
	
	public bool HasSpell(CSpell.EType _eSpellType)
	{
		foreach(CSpell spell in m_acSpellBook)
		{
			if(spell != null)
			{
				if(spell.GetSpellType() == _eSpellType)
				{
					return true;
				}
			}
		}
		return false;	
	}
	
	
	// Private:
	
	
	void Start()
	{
		m_acSpellBook = new CSpell[(int)ESettings.SPELLS_MAX];
		
		AddSpell(CSpell.EType.FIREBALL, 0);
		AddSpell(CSpell.EType.SONIC_WAVE, 1);
		//AddSpell(CSpell.EType.HOMING, 2);
		//AddSpell(CSpell.EType.BLACKHOLE, 3);
		//AddSpell(CSpell.EType.TETHER, 4);
		//AddSpell(CSpell.EType.METEOR, 5);
		//AddSpell(CSpell.EType.TELEPORT, 6);
		//AddSpell(CSpell.EType.SWAP, 2);
		
		
		m_aeSpellKeybinds = new KeyCode[(int)ESettings.SPELLS_MAX];
//		m_aeSpellKeybinds[0] = KeyCode.Alpha1;
//		m_aeSpellKeybinds[1] = KeyCode.Alpha2;
//		m_aeSpellKeybinds[2] = KeyCode.Alpha3;
//		m_aeSpellKeybinds[3] = KeyCode.Alpha4;
//		m_aeSpellKeybinds[4] = KeyCode.Q;
//		m_aeSpellKeybinds[5] = KeyCode.W;
//		m_aeSpellKeybinds[6] = KeyCode.E;
//		m_aeSpellKeybinds[7] = KeyCode.R;
		
		m_aeSpellKeybinds[0] = KeyCode.Q;
		m_aeSpellKeybinds[1] = KeyCode.W;
		m_aeSpellKeybinds[2] = KeyCode.E;
		m_aeSpellKeybinds[3] = KeyCode.R;
		m_aeSpellKeybinds[4] = KeyCode.A;
		m_aeSpellKeybinds[5] = KeyCode.S;
		m_aeSpellKeybinds[6] = KeyCode.D;
		m_aeSpellKeybinds[7] = KeyCode.F;

		m_CooldownIconOverlay = Resources.Load("UI/Spell Icons/Spell_Cooldown", typeof(Texture)) as Texture;
		m_RefresherIconOverlay = Resources.Load("UI/Spell Icons/Spell_Refresher", typeof(Texture)) as Texture;
		m_SelectedIconOverlay  = Resources.Load("UI/Spell Icons/Spell_Selected", typeof(Texture)) as Texture;
		
		m_GameHUD = Resources.Load("UI/Game_HUD", typeof(Texture)) as Texture;
		//m_GameHUDOverlay = Resources.Load("UI/Game_HUD_Overlay", typeof(Texture)) as Texture;
		m_SpellBorder = Resources.Load("UI/Spell Icons/Spell_Border", typeof(Texture)) as Texture;
		
		m_bCanCast = true;
	}
	

	void Update()
	{
		if(m_bCanCast)
		{
			ProcessInput();			
		}
		
		
		GameApp.GetInstance().GetCursor().SetIconLit(m_bSpellSelected);
	}
	
	
	void ProcessInput()
	{
		// Unselect
		if (m_bSpellSelected &&
			Input.GetMouseButtonDown(1))
		{
			m_bSpellSelected = false;
		}		
		
		// Select
		for (uint uiKey = 0; uiKey < m_uiCurNumSpells; ++ uiKey)
		{
			// The key for the spell is down
			if (Input.GetKeyDown(m_aeSpellKeybinds[uiKey]))
			{
				// There is a spell in that slot
				if (m_acSpellBook[uiKey] != null)
				{					
					// Spell not on cool down
					if (!m_acSpellBook[uiKey].IsOnCooldown())
					{
						DeselectAll();
						m_acSpellBook[uiKey].Select();
						m_uiSelectedSpellId = uiKey;
						m_bSpellSelected = true;
					}
					else
					{
						DeselectAll();
					}
				}
			}
		}
	}	
	
	void DeselectAll()
	{
		foreach(CSpell oSpell in m_acSpellBook)
		{
			if(oSpell != null)
			{
				oSpell.Deselect();
			}
		}
		
		
		m_bSpellSelected = false;
	}
	
	public CSpell[] GetSpells()
	{
		return(m_acSpellBook);
	}
	
	public CSpell GetSpell(CSpell.EType _eSpell)
	{
		foreach(CSpell spell in m_acSpellBook)
		{
			if(spell.GetSpellType() == _eSpell)
			{
				return spell;
			}
		}
		return null;
	}
	
	
	void OnGUI()
	{
		// Draw the back
		GUI.DrawTexture(UnityGUIExt.CreateRect(0, 0, 800, 188, 
												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE,
												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE), m_GameHUD);
		
		int iBorderWidth = 50;
		int iBorderHeight = 50;
		int iOffset = 2;
		int iIconStartX = -100;
		int iIconStartY = 57;
		
		// Draw the spells
		Rect rectSpellIcon = UnityGUIExt.CreateRect(iIconStartX, iIconStartY, iBorderWidth, iBorderHeight, 
												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE,
												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE);
		
		int iStartX = (int)rectSpellIcon.x;
		int iCol = 0;
		int iSpell = 0;
		CSpellData cData = GameApp.GetInstance().GetSpellDataComponent();
		
		string[] asKeys = new string[(int)ESettings.SPELLS_MAX];
//		asKeys[0] = "1";
//		asKeys[1] = "2";
//		asKeys[2] = "3";
//		asKeys[3] = "4";
//		asKeys[4] = "Q";
//		asKeys[5] = "W";
//		asKeys[6] = "E";
//		asKeys[7] = "R";
		
		asKeys[0] = " Q";
		asKeys[1] = " W";
		asKeys[2] = " E";
		asKeys[3] = " R";
		asKeys[4] = " A";
		asKeys[5] = " S";
		asKeys[6] = " D";
		asKeys[7] = " F";
	
		foreach (CSpell spell in m_acSpellBook)
		{	
			if(spell == null)
			{
				continue;
			}
			
			if(spell.GetSpellType() == CSpell.EType.INVALID)
			{
				continue;
			}
			
			CSpellData.TSpellData tSpell = cData.GetSpellData(spell.GetSpellType());
			
			string sSpellDesc = tSpell.sTitle + Environment.NewLine +
								"Level: " + GetSpell(spell.GetSpellType()).GetUpgradedLevel() + Environment.NewLine +
								tSpell.sDescription;
			
			GUI.DrawTexture(rectSpellIcon, cData.GetSpellData(spell.GetSpellType()).tTexture);
			if(rectSpellIcon.Contains(GameApp.GetInstance().GetCursor().GetScreenPosition()))
			{
				GUI.Label(UnityGUIExt.CreateRect(0, 50, 200, 200, 
												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE,
												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE), sSpellDesc);
			}
			
			
			
			// Draw Keys		
			GUI.Label(rectSpellIcon, asKeys[iSpell]);
			
			
			Rect rectThisSpell = rectSpellIcon;
				
			
			if(spell.IsOnCooldown())
			{
			
				GUI.DrawTexture(rectThisSpell, m_CooldownIconOverlay);
				
				rectThisSpell.y += rectThisSpell.height;
				
				rectThisSpell.height = rectSpellIcon.height * ((spell.GetCooldownTimer() - spell.GetCooldownLength()) / spell.GetCooldownLength());
				if(rectThisSpell.height < 0.0f)
				{
					rectThisSpell.height *= -1.0f;
				}
				rectThisSpell.y -= rectThisSpell.height;
				
				GUI.DrawTexture(rectThisSpell, m_RefresherIconOverlay);
			}
			else if(m_bSpellSelected &&
					iSpell == m_uiSelectedSpellId)
			{
				GUI.DrawTexture(rectThisSpell, m_SelectedIconOverlay);
			}
			
			rectSpellIcon.x += iOffset + iBorderWidth;
			
			++iCol;
			if(iCol == 4)
			{
				rectSpellIcon.x = iStartX;
				rectSpellIcon.y += iOffset + iBorderHeight;
			}
			
			++iSpell;
		}
		
		// Draw the borders over the spell

		Rect rectBorder = UnityGUIExt.CreateRect(iIconStartX, iIconStartY, iBorderWidth, iBorderHeight, 
										UnityGUIExt.GUI_ALLIGN.BOT_CENTRE,
										UnityGUIExt.GUI_ALLIGN.BOT_CENTRE);

		for(int i = 0; i < 2; ++i)
		{
			iStartX = (int)rectBorder.x;
			for(int j = 0; j < 4; ++j)
			{
				GUI.DrawTexture(rectBorder, m_SpellBorder);
				
				rectBorder.x += iOffset + iBorderWidth;
			}
			
			rectBorder.x = iStartX;
			rectBorder.y += iOffset + iBorderHeight;
		}
			
		
		
		// Draw the overlay
//		GUI.DrawTexture(UnityGUIExt.CreateRect(0, 0, 800, 188, 
//												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE,
//												UnityGUIExt.GUI_ALLIGN.BOT_CENTRE), m_GameHUDOverlay);
	}

	
	// Events:
	
	
	void OnDisable()
	{
		DeselectAll();
	}

	void OnDestroy()
	{
		GameObject[] aoSpellObjects = GameObject.FindGameObjectsWithTag("Spell") as GameObject[];

		
		for (int i = 0; i < aoSpellObjects.Length; ++ i)
		{
			Destroy(aoSpellObjects[i]);
		}
	}
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	CSpell[] m_acSpellBook;
	
		
	KeyCode[] m_aeSpellKeybinds;
	
	
	uint m_uiSelectedSpellId;
	
	
	public bool m_bSpellSelected;
	
	CWarlockMotor Warlock;
	
	private uint m_uiCurNumSpells;
	
	public bool m_bCanCast;
	
	private Texture m_FireballIcon;	
	private Texture m_TeleportIcon;
	private Texture m_WaveOfBlastIcon;
	private Texture m_SwapIcon;
	private Texture m_CooldownIconOverlay;
	private Texture m_RefresherIconOverlay;
	private Texture m_SelectedIconOverlay;
	private Texture m_GameHUD;
	//private Texture m_GameHUDOverlay;
	private Texture m_SpellBorder;
}
