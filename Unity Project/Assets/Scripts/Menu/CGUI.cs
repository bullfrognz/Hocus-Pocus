// Note: UI element sizing ignores resolution and window size, this class makes it not ignore it and puts the code in one place.
// Rescales everything if screen res is changed
// Handles rescaling of GUI elements
// Each Obj that has a CGUI will have one of these.
// This class creates and manages all GUI elements.
// Each element has an ID.
// If an element needs to perform a specific function, ...
// store the ID then perform the test by grabbing a reference from the owned CGUI component


using UnityEngine;
using System.Collections;

public class CGUI : MonoBehaviour 
{
	enum EType
	{
		
	}
	static readonly int s_iStandardWindowWidth = 1024;
	static readonly int s_iStandardWindowHeight = 768;
	
	private int m_iPrevScreenWidth;
	private int m_iPrevScreenHeight;
	
	private Vector2 m_vScaleFactor = new Vector2(1.0f, 1.0f);

	// Use this for initialization
	void Start () 
	{
		m_iPrevScreenWidth = Screen.width;
		m_iPrevScreenWidth = Screen.height;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Check for Window resize
		if(Input.GetMouseButtonUp(0))
		{
			if(HasWindowResized())
			{
				OnWindowResize();
			}
		}
	}
	
	bool HasWindowResized() 
	{
		if (m_iPrevScreenWidth != Screen.width || 
			m_iPrevScreenHeight != Screen.height)
		{
			m_iPrevScreenWidth = Screen.width;
			m_iPrevScreenHeight = Screen.height;
			
			return(true);
		}
		
		return(false);
	}
	
	void OnWindowResize()
	{
		// Change scale factor based if window size falls below standard window size threshold

		if(m_iPrevScreenWidth < s_iStandardWindowWidth)
		{
			m_vScaleFactor.x = s_iStandardWindowWidth / m_iPrevScreenWidth;
		}
		else
		{
			m_vScaleFactor.x = 1.0f;
		}
		
		if(m_iPrevScreenHeight < s_iStandardWindowHeight)
		{
			m_vScaleFactor.y = s_iStandardWindowHeight / m_iPrevScreenHeight;
		}
		else
		{
			m_vScaleFactor.y = 1.0f;
		}
	}
	
	// Button Creation 
	uint CreateStandardButton(float _fX, float _fY, UnityGUIExt.GUI_ALLIGN _eAllignment)
	{
		return (0);
	}
}
