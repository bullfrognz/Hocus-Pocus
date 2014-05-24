using UnityEngine;
using System.Collections;


public class CWarlockCurrency : MonoBehaviour
{
	
// Member Types

	
	
// Member Functions
	
	
	// Public:
	
	
	public void Set(uint _uiTotal)
	{
		m_uiTotal = _uiTotal;
	}
	
	
	public void Add(uint _uiAmount)
	{
		m_uiTotal += _uiAmount;
	}
	
	public void Deduct(uint _uiAmount)
	{
		m_uiTotal -= _uiAmount;
	}
	
	public uint Get()
	{
		return(m_uiTotal);
	}
	
	
	uint GetAmount()
	{
		return (m_uiTotal);
	}
	
	
	// Private:
	
	
	void Start()
	{
		m_uiTotal = 10;
	}
	

	void Update()
	{
		
	}
	
	// Events:
	
	
// Member Variables
	
	
	// Public:

	
	// Private:
	
	
	uint m_uiTotal;
		

	
	
	
}
