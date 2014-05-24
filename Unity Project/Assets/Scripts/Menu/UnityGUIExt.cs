using UnityEngine;
using System.Collections;

public class UnityGUIExt : MonoBehaviour 
{
	// Enum for alligning GUI elements on screen.
	// First word is allignment on Y
	// Second word is allignment on X
	public enum GUI_ALLIGN 
	{
		TOP_LEFT,
		TOP_RIGHT,
		TOP_CENTRE,
		BOT_LEFT,
		BOT_RIGHT,
		BOT_CENTRE,
		CENTRE_LEFT,
		CENTRE_CENTRE,
		CENTRE_RIGHT,
		CENTRE, // Centres on the X 
	};
	
	// Creates, positions and alligns rectangle as specified
	public static Rect CreateRect( float _fStartX, float _fStartY,
							float _fWidth, float _fHeight,
							GUI_ALLIGN _eScreenAllign, // Where on screen the Rect will be relative to
							GUI_ALLIGN _eAllign) // How the with and height of Rect will expand from Start X, Y
	{
		Rect returnRect = new Rect();
		returnRect.width = _fWidth;
		returnRect.height = _fHeight;
		
		// First get the rectangle to the correct place on screen.
		switch(_eScreenAllign)
		{
			case GUI_ALLIGN.TOP_LEFT:
				{
					returnRect.x = _fStartX;
					returnRect.y = _fStartY;
				}
				break;
			case GUI_ALLIGN.TOP_RIGHT:
				{
					returnRect.x = Screen.width - _fStartX;
					returnRect.y = _fStartY;
				}
				break;
			case GUI_ALLIGN.TOP_CENTRE:
				{
					returnRect.x = _fStartX + (Screen.width / 2);
					returnRect.y = _fStartY;
				}
				break;
			case GUI_ALLIGN.BOT_LEFT:
				{
					returnRect.x = _fStartX;
					returnRect.y = Screen.height - _fStartY;
				}
				break;
			case GUI_ALLIGN.BOT_RIGHT:
				{
					returnRect.x = Screen.width - _fStartX;
					returnRect.y = Screen.height - _fStartY;
				}
				break;
			case GUI_ALLIGN.BOT_CENTRE:
				{
					returnRect.x = _fStartX + (Screen.width / 2);
					returnRect.y = Screen.height - _fStartY;
				}
				break;
			case GUI_ALLIGN.CENTRE_LEFT:
				{
					returnRect.x = _fStartX;
					returnRect.y = _fStartY + (Screen.height / 2);
				}
				break;
			case GUI_ALLIGN.CENTRE_CENTRE:
				{
					returnRect.x = _fStartX + (Screen.width / 2);
					returnRect.y = _fStartY + (Screen.height / 2);
				}
				break;
			case GUI_ALLIGN.CENTRE_RIGHT:
				{
					returnRect.x = Screen.width - _fStartX;
					returnRect.y = _fStartY + (Screen.height / 2);
				}
				break;
			case GUI_ALLIGN.CENTRE:
				{
					returnRect.x = returnRect.x - _fWidth / 2;
				}
				break;
			default:
				{
					// TODO: Log
					break;
				}
		}
		
		// Now change the rectangle based on it's own allignment
		switch(_eAllign)
		{
			case GUI_ALLIGN.TOP_LEFT:
				{
					// No changes required
				}
				break;
			case GUI_ALLIGN.TOP_RIGHT:
				{
					returnRect.x = returnRect.x - _fWidth;
				}
				break;
			case GUI_ALLIGN.TOP_CENTRE:
				{
					returnRect.x = returnRect.x - (_fWidth / 2);
				}
				break;
			case GUI_ALLIGN.BOT_LEFT:
				{
					returnRect.y = returnRect.y - _fHeight;
				}
				break;
			case GUI_ALLIGN.BOT_RIGHT:
				{
					returnRect.x = returnRect.x - _fWidth;
					returnRect.y = returnRect.y - _fHeight;
				}
				break;
			case GUI_ALLIGN.BOT_CENTRE:
				{
					returnRect.x = returnRect.x - (_fWidth / 2);
					returnRect.y = returnRect.y - _fHeight;
				}
				break;
			case GUI_ALLIGN.CENTRE_LEFT:
				{
					returnRect.y = returnRect.y - (_fHeight / 2);
				}
				break;
			case GUI_ALLIGN.CENTRE_CENTRE:
				{
					returnRect.x = returnRect.x - (_fWidth / 2);
					returnRect.y = returnRect.y - (_fHeight / 2);
				}
				break;
			case GUI_ALLIGN.CENTRE_RIGHT:
				{
					returnRect.x = returnRect.x - _fWidth;
					returnRect.y = returnRect.y - (_fHeight / 2);
				}
				break;
			case GUI_ALLIGN.CENTRE:
				{
					returnRect.x = returnRect.x - _fWidth / 2;
				}
				break;
			default:
				{
					// TODO: Log
					break;
				}
		}
		
		return(returnRect);
	}
	
	public static Button CreateButton(float _fX, float _fY, float _fWidth, float _fHeight, UnityGUIExt.GUI_ALLIGN _eScreenAllign, UnityGUIExt.GUI_ALLIGN _eAllign, string _sText)
	{
		Button newButton = new Button();
		
		newButton.Initialise(_fX, _fY, _fWidth, _fHeight, _eScreenAllign, _eAllign, _sText);
		
		newButton.SetMouseTextures("UI/Button", "UI/Button_MouseOver", "UI/Button_Down");
		
		return(newButton);
	}
	
	// http://wiki.unity3d.com/index.php?title=CustomScrollView
    public static Vector2 BeginScrollView(	Rect position,
									        Vector2 scrollPosition,
									        Rect contentRect,
									        bool useHorizontal,
									        bool useVertical,
									        GUIStyle hStyle,
									        GUIStyle vStyle)
    {
 
        Vector2 scrollbarSize = new Vector2(hStyle.CalcSize(GUIContent.none).y,vStyle.CalcSize(GUIContent.none).x);
        Rect viewArea = position;
        if (useHorizontal)
		{
            viewArea.height -= scrollbarSize.x;
		}
        if (useVertical)
		{
            viewArea.width -= scrollbarSize.y;
		}
		
        if (useHorizontal)
        {
            Rect hScrRect = new Rect(position.x, position.y + viewArea.height, viewArea.width, scrollbarSize.x);
            scrollPosition.x = GUI.HorizontalScrollbar(hScrRect,scrollPosition.x,viewArea.width,0,contentRect.width);
        }
        if (useVertical)
        {
            Rect vScrRect = new Rect(position.x + viewArea.width, position.y, scrollbarSize.y, viewArea.height);
            scrollPosition.y = GUI.VerticalScrollbar(vScrRect,scrollPosition.y,viewArea.height,0,contentRect.height);
        }
		
        GUI.BeginGroup(viewArea);
        contentRect.x = -scrollPosition.x;
        contentRect.y = -scrollPosition.y;
        GUI.BeginGroup(contentRect);
        return scrollPosition;
    }
 
    public static Vector2 BeginScrollView(
        Rect position,
        Vector2 scrollPosition,
        Rect contentRect,
        bool useHorizontal,
        bool useVertical)
    {
        return BeginScrollView(position, scrollPosition, contentRect, useHorizontal, useVertical, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);
    }
 
    public static void EndScrollView()
    {
        GUI.EndGroup();
        GUI.EndGroup();
    }
}