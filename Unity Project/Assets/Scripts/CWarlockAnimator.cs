using UnityEngine;
using System.Collections;


public class CWarlockAnimator : MonoBehaviour
{
	
// Member Types
	
	
	public enum EAniamtion
	{
		INVALID = -1,
		SPIN,
		SLAM_STAFF,
		RASE_STAFF,
		RASE_HOLD_STAFF,
		CAST_1,
		CAST_2,
		CAST_3,
		CAST_4,
		CAST_5,
		TAKE_HIT,
		DIE,
		REVIVE,
		RUN,
		WALK,
		IDLE,
		GET_UP,
		MAX
	}
	
	
	public enum ECastAnimation
	{
		RANDOM_SPIN,
		RANDOM_RASE_STAFF,
		RANDOM_CAST,
		SLAM_STAFF,
	}
	
	
	public struct TAnimationInfo
	{
		public string m_sName;
		public bool m_bAllowInterupt;
	}

	
	
// Member Functions
	
	
	// Public:
	
	
	public void NotifyCastSpell(ECastAnimation _eCastAnimation)
	{
		if (!GetComponent<CWarlockHealth>().IsAlive())
		{
			return ;
		}
		
		
		EAniamtion eCastAnimation = EAniamtion.INVALID;
		int iRandomNumber = Random.Range(0, 1000);
		
		
		// RANDOM_SPIN_
		if (_eCastAnimation == ECastAnimation.RANDOM_SPIN)
		{
			switch (0)
			{
			case 0:
				eCastAnimation = EAniamtion.SPIN;
				break;
			}
		}
		
		// RANDOM_SLAM_STAFF
		if (_eCastAnimation == ECastAnimation.SLAM_STAFF)
		{
			switch (0)
			{
			case 0:
				eCastAnimation = EAniamtion.SLAM_STAFF;
				break;
			}
		}
		
		
		// RANDOM_RASE_STAFF
		else if (_eCastAnimation == ECastAnimation.RANDOM_RASE_STAFF)
		{
			switch (iRandomNumber % 2)
			{
			case 0:
				eCastAnimation = EAniamtion.RASE_STAFF;
				break;
				
			case 1:
				eCastAnimation = EAniamtion.RASE_HOLD_STAFF;
				break;
			}
		}
		
		// RANDOM_CAST
		else if (_eCastAnimation == ECastAnimation.RANDOM_CAST)
		{
			switch (iRandomNumber % 5)
			{
			case 0:
				eCastAnimation = EAniamtion.CAST_1;
				break;
				
			case 1:
				eCastAnimation = EAniamtion.CAST_2;
				break;
				
			case 2:
				eCastAnimation = EAniamtion.CAST_3;
				break;
				
			case 3:
				eCastAnimation = EAniamtion.CAST_4;
				break;
				
			case 4:
				eCastAnimation = EAniamtion.CAST_5;
				break;
			}
		}
		
	
		ChangeAnimation(eCastAnimation);
	}
	
	
	public void NotifyMoving()
	{
		if (GetComponent<CWarlockHealth>().IsAlive())
		{
			ChangeAnimation(EAniamtion.RUN);
		}
		else
		{
			Debug.LogError("Cannot run aniamtion because dead");
		}
	}
	
	
	public void NotifyMovingStop()
	{
		if (GetComponent<CWarlockHealth>().IsAlive())
		{
			ChangeAnimation(EAniamtion.IDLE);
		}
	}
	
	
	public void NotifyDeath()
	{
		m_eQueuedAnimation = EAniamtion.INVALID;
		ChangeAnimation(EAniamtion.DIE);
	}
	
	
	public void NotifyRevive()
	{
		if (m_eCurrentAnimation == EAniamtion.DIE)
		{
			m_eCurrentAnimation = EAniamtion.IDLE;
			ChangeAnimation(EAniamtion.GET_UP);
			ChangeAnimation(EAniamtion.IDLE);
		}
	}
	
	
	public bool IsAnimating()
	{
		return (m_oWarlock.animation.isPlaying);
	}
	
	
	// Private:
	
	
	void Start()
	{
		m_oWarlock = transform.FindChild("TheWizard").gameObject;
		
		
		m_atAnimations = new TAnimationInfo[(int)EAniamtion.MAX];
		
		
		m_atAnimations[(int)EAniamtion.SPIN].m_sName 		= "HelixSpell";
		m_atAnimations[(int)EAniamtion.SLAM_STAFF].m_sName 	= "StaffEarthQuake";
		m_atAnimations[(int)EAniamtion.RASE_STAFF].m_sName 	= "MagicLightSpell";
		m_atAnimations[(int)EAniamtion.RASE_HOLD_STAFF].m_sName = "BuffSpellA";
		m_atAnimations[(int)EAniamtion.CAST_1].m_sName 		= "StraightDownHit";
		m_atAnimations[(int)EAniamtion.CAST_2].m_sName 		= "SwingRight";
		m_atAnimations[(int)EAniamtion.CAST_3].m_sName 		= "MagicShotStraight";
		m_atAnimations[(int)EAniamtion.CAST_4].m_sName 		= "SpellCastC";
		m_atAnimations[(int)EAniamtion.CAST_5].m_sName 		= "SpellCastD";
		m_atAnimations[(int)EAniamtion.TAKE_HIT].m_sName 	= "TakingHit";
		m_atAnimations[(int)EAniamtion.DIE].m_sName 		= "Dying";
		m_atAnimations[(int)EAniamtion.REVIVE].m_sName 		= "GetUp";
		m_atAnimations[(int)EAniamtion.RUN].m_sName 		= "Run";
		m_atAnimations[(int)EAniamtion.WALK].m_sName 		= "Walk";
		m_atAnimations[(int)EAniamtion.IDLE].m_sName 		= "CombatModeA";
		m_atAnimations[(int)EAniamtion.GET_UP].m_sName 		= "GetUp";
		
		
		m_atAnimations[(int)EAniamtion.SPIN].m_bAllowInterupt 		= false;
		m_atAnimations[(int)EAniamtion.SLAM_STAFF].m_bAllowInterupt = false;	
		m_atAnimations[(int)EAniamtion.RASE_STAFF].m_bAllowInterupt = false;	
		m_atAnimations[(int)EAniamtion.RASE_HOLD_STAFF].m_bAllowInterupt = false;
		m_atAnimations[(int)EAniamtion.CAST_1].m_bAllowInterupt 	= false;	
		m_atAnimations[(int)EAniamtion.CAST_2].m_bAllowInterupt 	= false;	
		m_atAnimations[(int)EAniamtion.CAST_3].m_bAllowInterupt 	= false;	
		m_atAnimations[(int)EAniamtion.CAST_4].m_bAllowInterupt 	= false;	
		m_atAnimations[(int)EAniamtion.CAST_5].m_bAllowInterupt 	= false;	
		m_atAnimations[(int)EAniamtion.TAKE_HIT].m_bAllowInterupt 	= false;
		m_atAnimations[(int)EAniamtion.DIE].m_bAllowInterupt 		= false;
		m_atAnimations[(int)EAniamtion.REVIVE].m_bAllowInterupt 	= false;	
		m_atAnimations[(int)EAniamtion.RUN].m_bAllowInterupt 		= true;
		m_atAnimations[(int)EAniamtion.WALK].m_bAllowInterupt 		= true;
		m_atAnimations[(int)EAniamtion.IDLE].m_bAllowInterupt 		= true;
		m_atAnimations[(int)EAniamtion.GET_UP].m_bAllowInterupt 	= false;
		
		ChangeAnimation(EAniamtion.IDLE);
	}
	

	void Update()
	{
		AnimationState cCurrentAnimation = m_oWarlock.animation[m_atAnimations[(int)m_eCurrentAnimation].m_sName];
		
		
		// There is a waiting queued animation
		if (m_eQueuedAnimation != EAniamtion.INVALID)
		{
			// Current animation is ending within cross-fade time
			if (cCurrentAnimation.time + 0.3f > cCurrentAnimation.length ||
				!IsAnimating())
			{
				m_eCurrentAnimation = m_eQueuedAnimation;
				
				
				m_oWarlock.animation[m_atAnimations[(int)m_eCurrentAnimation].m_sName].time = 0.0f;
				m_oWarlock.animation.CrossFade(m_atAnimations[(int)m_eCurrentAnimation].m_sName);
				
				
				m_eQueuedAnimation = EAniamtion.INVALID;
				//Debug.Log("Queued animation2: " + m_eQueuedAnimation.ToString());
			}
		}
		
		// Current animation has finsihed
		else if (!m_oWarlock.animation.isPlaying)
		{
			// Don't restart the death animation
			if (m_eCurrentAnimation != EAniamtion.DIE)
			{
				m_oWarlock.animation.Play(m_atAnimations[(int)m_eCurrentAnimation].m_sName);
			}
		}
	}
	
	
	void ChangeAnimation(EAniamtion _eNewAnimation)
	{
		if (m_eCurrentAnimation != _eNewAnimation)
		{
			// Check if current animation can be interupted
			if (m_atAnimations[(int)m_eCurrentAnimation].m_bAllowInterupt)
			{
				m_eCurrentAnimation = _eNewAnimation;
				m_eQueuedAnimation = EAniamtion.INVALID;
				
				
				m_oWarlock.animation[m_atAnimations[(int)m_eCurrentAnimation].m_sName].time = 0.0f;
				m_oWarlock.animation.CrossFade(m_atAnimations[(int)m_eCurrentAnimation].m_sName);
				
				
				//Debug.Log("Crossfade animation: " + _eNewAnimation.ToString());
			}
			
			// Queue the animation for when the current animation finishes
			else if (m_eQueuedAnimation != _eNewAnimation)
			{
				m_eQueuedAnimation = _eNewAnimation;
				
				
				//Debug.Log("Queued animation: " + _eNewAnimation.ToString());
			}	
		}
	}
	
	
	// Events:
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
	GameObject m_oWarlock;
	
	
	TAnimationInfo[] m_atAnimations;
	
	
	EAniamtion m_eCurrentAnimation = EAniamtion.IDLE;
	EAniamtion m_eQueuedAnimation  = EAniamtion.INVALID;
	
	
	
}
