using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberedCircle : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _numberText = null;

	public void UpdateNumber(int number)
	{
		_numberText.text = number.ToString();
	}
}
