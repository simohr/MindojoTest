using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterSpawner : MonoBehaviour
{
    [HideInInspector]
    public bool DrawLineActive = true;

    [SerializeField]
    private LetterScriptableObject _letterData = null;

    [SerializeField]
    private GameObject _letterPrefab = null;

    [SerializeField]
    private GameObject _winScreen = null;

    [SerializeField]
    private GameObject _loseScreen = null;

    private DrawLine _drawLine = null;

    // Start is called before the first frame update
    void Start()
    {
        _drawLine = GetComponent<DrawLine>();
        var letter = Instantiate(_letterPrefab, transform);
        Letter letterScript = letter.GetComponent<Letter>();
        letterScript.Data = _letterData;
    }

    public void ShowWinScreen()
	{
        _drawLine.DestroyInstantiatedLines();
        _winScreen.SetActive(true);
        DrawLineActive = false;
	}

    public void ShowLoseScreen()
	{
        _drawLine.DestroyInstantiatedLines();
        _loseScreen.SetActive(true);
        DrawLineActive = false;
	}

    public void Retry()
	{
        var letter = Instantiate(_letterPrefab, transform);
        Letter letterScript = letter.GetComponent<Letter>();
        letterScript.Data = _letterData;
        DrawLineActive = true;
    }

}
