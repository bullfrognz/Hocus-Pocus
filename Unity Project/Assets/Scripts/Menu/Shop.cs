using UnityEngine;
using System.Collections;
using System;

public class Shop : MonoBehaviour 
{
	// Shop is owned by the Arena but needs access to the Warlock and his Spellbook
	
	
	private CSpellbook m_oSpellBook;
	private CWarlockCurrency m_oWarlockCurrency;
		
	private Rect m_rectTitle;
	private Texture m_TitleTexture = Resources.Load("UI/Shop", typeof(Texture)) as Texture;	
	
	private Rect m_rectSpellEntry;
	private Texture m_SpellEntry = Resources.Load("UI/ShopEntry", typeof(Texture)) as Texture;
	private Texture m_SpellEntry_MouseOver = Resources.Load("UI/ShopEntry_MouseOver", typeof(Texture)) as Texture;
	private Texture m_SpellEntry_Selected = Resources.Load("UI/ShopEntry_MouseUp", typeof(Texture)) as Texture;
	private Texture m_tSpellFrame = Resources.Load("UI/Spell Icons/Spell_Border", typeof(Texture)) as Texture;	
	
	private Texture m_tCrystalCost = Resources.Load("UI/CrystalAmount", typeof(Texture)) as Texture;	
	
	private Rect m_rectSpellIcon;//69/69
	private Rect m_rectSpellTitle;//132/25
	private Rect m_rectSpellCost;//53/33
	private Rect m_rectOwnedCrystals;//53/33
	
	private Rect m_rectSpellDesc;

	private CSpellData.TSpellData[] m_atSpellData;
	private CSpell.EType m_eSelectedSpell;
	
	private bool m_bFirstClick = true;
	
	Font m_fStandard = Resources.Load("Fonts/NewRocker-Regular") as Font;
	Font m_fLarge = Resources.Load("Fonts/NewRocker-Large") as Font;
	Font m_fXLarge = Resources.Load("Fonts/NewRocker-XLarge") as Font;
	Font m_fXXLarge = Resources.Load("Fonts/NewRocker-XXLarge") as Font;
	
	
		
	// Use this for initialization
	void Start () 
	{
		ResetLayout();
		
		m_oSpellBook = GameApp.GetInstance().GetWarlock().GetComponent<CSpellbook>();
		
		m_atSpellData = GetComponent<CSpellData>().GetAllSpellData();
		
		m_oWarlockCurrency = GameApp.GetInstance().GetWarlock().GetComponent<CWarlockCurrency>();
	}
	
	
	// Initalises all rectangles. Recall if the screen resolution is changed.
	void ResetLayout()
	{
		// Title
		m_rectTitle = UnityGUIExt.CreateRect(0.0f, 0.0f, 762.0f, 676.0f,
												UnityGUIExt.GUI_ALLIGN.TOP_CENTRE,
												UnityGUIExt.GUI_ALLIGN.TOP_CENTRE);
		
		m_rectSpellEntry = new Rect (72.0f, 172.0f, 272.0f, 89.0f);
		m_rectSpellIcon = new Rect(80,182,69,69);
		m_rectSpellTitle = new Rect(70,140,132,25);
		m_rectSpellCost = new Rect(275,140,60,33);
		m_rectOwnedCrystals = new Rect(621,96,54,28);
		m_rectSpellDesc = new Rect(160, 180, 170,70);
	}
	
	// Update is called once per frame
	void Update () 
	{
		ResetLayout();
		
		uint uiCost;
		m_bFirstClick = true;
		bool bHasSpell;
		uint uiLevel;
		
		CCursor oCursor = GameApp.GetInstance().GetCursor();
		Vector2 vMousePos = oCursor.GetScreenPosition();
		vMousePos.x -= m_rectTitle.x;
		vMousePos.y -= m_rectTitle.y;
		
		Rect rectNextEntry = m_rectSpellEntry;
		const int iColMax = 2;
		const int iRowMax = 4;
		for(int iCol = 0; iCol < iColMax; ++iCol)
		{
			for(int iRow = 0; iRow < iRowMax; ++iRow)
			{
				int iSpellIndex = (iCol * iRowMax) + iRow;
				// Check for spell ownership
				CSpellData.TSpellData tSpell = m_atSpellData[iSpellIndex];
				if(tSpell.eType == CSpell.EType.INVALID)
				{
					Debug.Log(tSpell.eType.ToString());
					// continue
					// Note continue didnt work so I had to group it in this else block
				}
				else
				{	
					bHasSpell = m_oSpellBook.HasSpell(tSpell.eType);
					
					if(bHasSpell)
					{
						uiLevel = m_oSpellBook.GetSpell(tSpell.eType).GetUpgradedLevel();
						uiCost = tSpell.uiUpgradeCost + uiLevel;
					}
					else
					{
						//sSpellDesc = "";
						uiCost = tSpell.uiBuyCost;
						uiLevel = 0;
					}
					
					if(rectNextEntry.Contains(vMousePos))
					{
						if(Input.GetMouseButtonUp(0))
						{
							if(m_oSpellBook.HasSpell(tSpell.eType))
							{
								UpgradeSpell(tSpell.eType, uiCost);
							}
							else
							{
								PurchaseSpell(tSpell.eType, uiCost);
							}
						}
					}
					
					rectNextEntry.y += 132; // height offset
				}
			}
			
			rectNextEntry.x += 365; // width offset
			rectNextEntry.y = m_rectSpellEntry.y;
		}
	}
	
	void OnGUI()
	{			
		GUI.skin.font = m_fStandard;
		
		// Title
      	GUI.DrawTexture(m_rectTitle, m_TitleTexture);
		
		GUILayout.BeginArea(m_rectTitle);
		
		Rect rectNextEntry = m_rectSpellEntry;
		Rect rectNextSpellTitle = m_rectSpellTitle;
		Rect rectNextSpellIcon = m_rectSpellIcon;
		Rect rectNextSpellCost = m_rectSpellCost;
		Rect rectNextSpellDesc = m_rectSpellDesc;
		
		CCursor oCursor = GameApp.GetInstance().GetCursor();
		Vector2 vMousePos = oCursor.GetScreenPosition();
		vMousePos.x -= m_rectTitle.x;
		vMousePos.y -= m_rectTitle.y;
		
		bool bHasSpell;
		//string sSpellDesc;
		uint uiCost;
		uint uiLevel;
		
		const int iColMax = 2;
		const int iRowMax = 4;
		
		for(int iCol = 0; iCol < iColMax; ++iCol)
		{
			for(int iRow = 0; iRow < iRowMax; ++iRow)
			{
				int iSpellIndex = (iCol * iRowMax) + iRow;
				// Check for spell ownership
				CSpellData.TSpellData tSpell = m_atSpellData[iSpellIndex];
				if(tSpell.eType == CSpell.EType.INVALID)
				{
					Debug.Log(tSpell.eType.ToString());
					// continue
					// Note continue didnt work so I had to group it in this else block
				}
				else
				{
					bHasSpell = m_oSpellBook.HasSpell(tSpell.eType);
					
					if(bHasSpell)
					{
						uiLevel = m_oSpellBook.GetSpell(tSpell.eType).GetUpgradedLevel();
						uiCost = tSpell.uiUpgradeCost + uiLevel;
					}
					else
					{
						//sSpellDesc = "";
						uiCost = tSpell.uiBuyCost;
						uiLevel = 0;
					}
					
					GUI.DrawTexture(rectNextEntry, m_SpellEntry);
					
					// Show Desc here
					GUI.Label(rectNextSpellDesc, "Lv: " + uiLevel + Environment.NewLine + tSpell.sDescription);
					

					
					if(rectNextEntry.Contains(vMousePos))
					{
						if(oCursor.IsMouseDown())
						{
							GUI.DrawTexture(rectNextEntry, m_SpellEntry_Selected);
						}
						else
						{
							GUI.DrawTexture(rectNextEntry, m_SpellEntry_MouseOver);
						}
						if(Input.GetMouseButtonUp(0) && m_bFirstClick)
						{
//							if(m_oSpellBook.HasSpell(tSpell.eType))
//							{
//								UpgradeSpell(tSpell.eType, uiCost);
//							}
//							else
//							{
//								//PurchaseSpell(tSpell.eType, uiCost);
//							}
//							
//							 m_bFirstClick = false;
						}
						
						// Show Buy/Upgrade Desc here
												
						if(bHasSpell)
						{
							GUI.Label(rectNextSpellDesc, "Lv: " + uiLevel + " -> " + (uiLevel + 1) + Environment.NewLine + m_oSpellBook.GetSpell(tSpell.eType).GetShopUpgradeDetails());
						}
						else
						{
							GUI.Label(rectNextSpellDesc, "Lv: " + uiLevel + " -> " + (uiLevel + 1) + Environment.NewLine + tSpell.sShopDetails);
						}
					}
					
					GUI.DrawTexture(rectNextSpellTitle, tSpell.tNameTexture);
					
					// Text Spell titles
					GUI.skin.font = m_fXXLarge;
					Rect rectLabel = rectNextSpellTitle;
					rectLabel.y -= 5;
					rectLabel.height += 5;
					GUI.Label(rectLabel, tSpell.sTitle);
					GUI.skin.font = m_fStandard;
					
					GUI.DrawTexture(rectNextSpellIcon, tSpell.tTexture);
					GUI.DrawTexture(rectNextSpellIcon, m_tSpellFrame);
					
					
					GUI.DrawTexture(rectNextSpellCost, m_tCrystalCost);
					Rect rectSpellCost = rectNextSpellCost;
					rectSpellCost.x += 35;
					rectSpellCost.y += 6;
					
					GUI.skin.font = m_fXLarge;
					GUI.Label(rectSpellCost, uiCost.ToString());
					GUI.skin.font = m_fStandard;
					
					
					rectNextEntry.y += 132; // height offset
					rectNextSpellTitle.y += 132;
					rectNextSpellIcon.y += 132;
					rectNextSpellCost.y += 132;	
					rectNextSpellDesc.y += 132;
				}
			}
			
			rectNextEntry.x += 365; // width offset
			rectNextEntry.y = m_rectSpellEntry.y;
				
			rectNextSpellTitle.x += 365; // width offset
			rectNextSpellTitle.y = m_rectSpellTitle.y;
			
			rectNextSpellIcon.x += 365; // width offset
			rectNextSpellIcon.y = m_rectSpellIcon.y;
			
			rectNextSpellCost.x += 365; // width offset
			rectNextSpellCost.y = m_rectSpellCost.y;
			
			rectNextSpellDesc.x += 365; // width offset
			rectNextSpellDesc.y = m_rectSpellDesc.y;
		}
		
		GUI.skin.font = m_fXXLarge;
		GUI.Label(m_rectOwnedCrystals, m_oWarlockCurrency.Get().ToString());
		
		Rect rTime = m_rectOwnedCrystals;
		rTime.x -= 200;
		//GUI.Label(rTime, CSceneArena.m_kfShoppingDuration.ToString());
		GUI.Label(rTime, GameApp.GetInstance().GetSceneArena().GetShopTimer().ToString());
				
		GUILayout.EndArea();
		
		
		
		
		GUI.skin.font = m_fLarge;
	}
	
	void PurchaseSpell(CSpell.EType _eSpellType, uint _uiPurchaseCost)
	{
		if(m_oWarlockCurrency.Get() >= _uiPurchaseCost)
		{
			m_oSpellBook.AddSpellToFreeSlot(_eSpellType);
			m_oWarlockCurrency.Deduct(_uiPurchaseCost);
			
		}
	}
	
	void UpgradeSpell(CSpell.EType _eSpellType, uint _uiUpgradeCost)
	{
		if(m_oWarlockCurrency.Get() >= _uiUpgradeCost)
		{
			m_oSpellBook.GetSpell(_eSpellType).Upgrade();
			m_oWarlockCurrency.Deduct(_uiUpgradeCost);
		}
	}
}