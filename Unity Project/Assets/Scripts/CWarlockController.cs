using UnityEngine;
using System.Collections;


public class CWarlockController : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	public void Initialise(NetworkViewID _tTransformViewId, NetworkViewID _tAnimationViewId, NetworkViewID _tHealthViewId)
	{
		// Sync animation
		Component cAnimation = transform.FindChild("TheWizard").GetComponent<Animation>();
		NetworkHelper.AddNetworkView(gameObject, cAnimation, _tAnimationViewId);
		
		
		// Sync transform
		NetworkHelper.AddNetworkView(gameObject, transform, _tTransformViewId);
		
		
		// I own this warlock
		if (networkView.isMine)
		{
			gameObject.AddComponent<CWarlockAnimator>();
			gameObject.AddComponent<CWarlockMotor>();
			gameObject.AddComponent<CSpellbook>();
			gameObject.AddComponent<CWarlockCurrency>();
			
			
			transform.position = new Vector3(Random.Range(-15, 15), 0.0f, Random.Range(-10, 10));
			
			//Debug.Log("Its Mine!!!!");
			
		}
		
		// Someone else owns this warlock
		else
		{
			// Empty
		}
		
		
		gameObject.AddComponent<CWarlockHealth>();
		
		
		// Sync health
		Component cWarlockHealth = transform.GetComponent<CWarlockHealth>();
		NetworkHelper.AddNetworkView(gameObject, cWarlockHealth, _tHealthViewId);
		
	}
	
	
	public void NotifySlotId(int _iSlotId)
	{
		switch(_iSlotId)
		{
		case 0:
			SetColour(new Color(1.0f, 0.0f, 0.0f, 1.0f)); // red
			break;
			
		case 1:
			SetColour(new Color(0.0f, 0.26f, 1.0f, 1.0f)); // blue
			break;
			
		case 2:
			SetColour(new Color(0.21f, 0.8f, 0.63f, 1.0f)); // cyan
			break;
			
		case 3:
			SetColour(new Color(0.33f, 0.0f, 0.51f, 1.0f)); // purple
			break;
			
		case 4:
			SetColour(new Color(1.0f, 1.0f, 0.0f, 1.0f)); // yellow
			break;
			
		case 5:
			SetColour(new Color(1.0f, 0.7f, 0.0f, 1.0f)); // orange
			break;
			
		case 6:
			SetColour(new Color(0.125f, 0.75f, 0.0f, 1.0f)); // teal
			break;
			
		case 7:
			SetColour(new Color(0.9f, 0.2f, 0.8f, 1.0f)); // pink
			break;
			
		case 8:
			SetColour(new Color(0.588f, 0.588f, 0.588f, 1.0f)); // gray
			break;
			
		case 9:
			SetColour(new Color(0.2f, 0.3f, 0.8f, 1.0f)); // light blue
			break;
			
		case 10:
			SetColour(new Color(0.063f, 0.38f, 0.27f, 1.0f)); // dark greenish
			break;
			
		case 11:
			SetColour(new Color(0.90f, 0.40f, 0.40f, 1.0f)); // we get peach
			break;
		}
	}
	
	
	[RPC]
	public void WarlockController_ApplyPushback(float _fX, float _fY)
	{
		if (!networkView.isMine)
		{
			networkView.RPC("WarlockController_ApplyPushback", networkView.owner, _fX, _fY);
		}
		else
		{
			GetComponent<CWarlockMotor>().WarlockMotor_ApplyPushback(_fX, _fY);
		}
	}
	
	[RPC]
	public void MoveWarlock(Vector3 _vPosition)
	{
		if (!networkView.isMine)
		{
			networkView.RPC("MoveWarlock", networkView.owner, _vPosition);
		}
		else
		{
			transform.position = _vPosition;
		}			
	}
	
	[RPC]
	public void AnimateWarlock(int _eCastAnimation)
	{
		if (!networkView.isMine)
		{
			networkView.RPC("AnimateWarlock", networkView.owner, _eCastAnimation);
		}
		else
		{
			GetComponent<CWarlockAnimator>().NotifyCastSpell((CWarlockAnimator.ECastAnimation) _eCastAnimation);
		}			
	}
	
	[RPC]
	public void IgnoreInput(bool _bIgnore)
	{
		if (!networkView.isMine)
		{
			networkView.RPC("IgnoreInput", networkView.owner, _bIgnore);
		}
		else
		{
			GetComponent<CWarlockMotor>().m_bIgnoreInput = _bIgnore;
		}			
	}
	
	[RPC]
	public void StopMoving(bool _bMove)
	{
		if (!networkView.isMine)
		{
			networkView.RPC("StopMoving", networkView.owner, _bMove);
		}
		else
		{
			GetComponent<CWarlockMotor>().m_bReachedPath = _bMove;
		}			
	}
	
	[RPC]
	public void AddCurrency(uint _uiCurrency)
	{
		GameApp.GetInstance().GetWarlock().GetComponent<CWarlockCurrency>().Add(_uiCurrency);		
	}
	
	[RPC]
	public void MoveTo(Vector3 _vPosition)
	{
		if (!networkView.isMine)
		{
			networkView.RPC("MoveTo", networkView.owner, _vPosition);
		}
		else
		{	
			Vector2 Vec3ToVec2 = new Vector2(_vPosition.x, _vPosition.z);
			GetComponent<CWarlockMotor>().m_bIgnoreInput = true;
			GetComponent<CWarlockMotor>().GoTo(Vec3ToVec2);			
		}			
	}
	
	[RPC]
	public void SetSpeed(float _fSpeed)
	{
		if (!networkView.isMine)
		{
			networkView.RPC("SetSpeed", networkView.owner, _fSpeed);
		}
		else
		{	
			GetComponent<CWarlockMotor>().m_fMovementVelocity = _fSpeed;
		}			
	}
	
	public void ReportDeath()
	{
		if(m_iLastAttackerSlotId != -1)
		{
			//Debug.Log("Reporting Death");
			GameApp.GetInstance().GetSceneArena().GetComponent<CArenaAwards>().ReportKill(m_iLastAttackerSlotId);
		}
		else
		{
			//Debug.Log("Reporting Suicide");
			GameApp.GetInstance().GetSceneArena().GetComponent<CArenaAwards>().ReportSuicide();
		}
	}
	
	[RPC]
	public void WarlockController_SetLastAttacker(int _iAttackerSlotId)
	{
		if (!networkView.isMine)
		{
			networkView.RPC("WarlockController_SetLastAttacker", networkView.owner, _iAttackerSlotId);
		}
		else
		{	
			//Debug.LogError("New attacker: " + _iAttackerSlotId);
			m_iLastAttackerSlotId = _iAttackerSlotId;
		}
	}

	
	public void TakeDamage(float _fAmount)
	{
		GetComponent<CWarlockHealth>().WarlockHealth_ApplyDamage(_fAmount, true);
	}
	
	
	// Private:
	
	
	void Awake()
	{
		// Empty
	}
	
	
	void Start()
	{
	}
	

	void Update()
	{

	}
	
	
	void SetColour(Color _vColour)
	{
		Texture2D cAlphaOverlay = Resources.Load("Models/Wizard/WizardATeamColourAlpha") as Texture2D;
		Texture2D cDress = Resources.Load("Models/Wizard/WizardA") as Texture2D;

		
		Texture2D Dress2 = new Texture2D(cDress.width, cDress.height);
		Dress2.SetPixels(cDress.GetPixels());
		
		
		_vColour.a = 1.0f;
	
		
		for (int y = 0; y < cAlphaOverlay.height; ++ y)
		{
			for (int x = 0; x < cAlphaOverlay.width; ++ x)
			{
				Color cPixelColour = cAlphaOverlay.GetPixel(x, y);
				
				
				if (cPixelColour.r > 0.98f &&
					cPixelColour.a > 0.98f)
				{
					Dress2.SetPixel(x, y, cDress.GetPixel(x, y) * 4.0f * _vColour);
				}
				else if (cPixelColour.b > 0.98f &&
						 cPixelColour.a > 0.98f)
				{
					Dress2.SetPixel(x, y, cDress.GetPixel(x, y) * 1.0f * _vColour);
				}
			}	
		}
		
		
		Dress2.Apply();
		
		
		transform.FindChild("TheWizard").FindChild("WizardA").renderer.material.mainTexture = Dress2;
		transform.FindChild("Point light").light.color = _vColour;
	}
	
	
	// Events:
	
	
	void OnDestroy()
	{
		Destroy(gameObject.GetComponent<CWarlockHealth>());
		
		
		// I own this warlock
		if (networkView.isMine)
		{
			Destroy(gameObject.GetComponent<CScoreBoard>());
			Destroy(gameObject.GetComponent<CWarlockAnimator>());
			Destroy(gameObject.GetComponent<CWarlockMotor>());
			Destroy(gameObject.GetComponent<CSpellbook>());
			Destroy(gameObject.GetComponent<CWarlockCurrency>());
		}
	}
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	int m_iLastAttackerSlotId = -1;
	
}
