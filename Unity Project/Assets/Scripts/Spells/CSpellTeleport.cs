using UnityEngine;
using System.Collections;

/*
public class CSpellFireball : CSpell
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	// Protected:
	
	
	override protected void Initialise(ref string _rsDescription, ref EType _reSpellType, ref float _rfCooldownLength, ref float _rfDamageLevelIncrement,
									   ref float _rfPushbackLevelIncrement, ref float _rfRange, ref uint _ruiCurrancyUpgradeCost)
	{
		_rsDescription = "Unleash a fiery projectile that explodes on contact";
		_reSpellType = CSpell.EType.FIREBALL;
		_rfCooldownLength = 2.5f;
		_rfDamageLevelIncrement = 0.0f;
		_rfPushbackLevelIncrement = 0.0f;
		_rfRange = 5.0f;
		_ruiCurrancyUpgradeCost = 4;
	}
	
	
	// Private:
	
	
	override public bool Cast(Vector3 _vTargetPoint, GameObject _oTargetWarlock)
	{
		//Vector3 TempLook = _vPoint - transform.FindChild("TheWizard").transform.position;
		Vector3 TempLook =  transform.FindChild("TheWizard").transform.position - _vTargetPoint;
		Network.Instantiate(Resources.Load("Prefabs/Fireball", typeof(GameObject)), _vTargetPoint, Quaternion.LookRotation(TempLook), 0);
		
		
		return (true);
	}
	
	
	// Events:
	
	
// Member Variables
	
	
	// Public:
	
	
	// Private:
	
	
}
*/