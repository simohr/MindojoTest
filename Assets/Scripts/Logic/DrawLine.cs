using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawLine : MonoBehaviour
{
    public GameObject linePrefab;
    private GameObject _currentLine;
    private LineRenderer _lineRenderer;
    private List<Vector3> _fingerPositions = new List<Vector3>();
    private RectTransform _rectTransform;
    private LetterSpawner _letterSpawner;

    private Camera _camera;
    private Canvas _canvas;

    private List<GameObject> _instantiatedLines = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _canvas = GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _letterSpawner = GetComponent<LetterSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_letterSpawner.DrawLineActive)
            return;
        if(Input.GetMouseButtonDown(0))
		{
            CreateLine();
        }

		if (Input.GetMouseButton(0))
		{
            Vector3 tempFingerPos = GetMousePosition();
            if (Vector2.Distance(tempFingerPos, _fingerPositions[_fingerPositions.Count - 1]) > .1f)
			{
				UpdateLine(tempFingerPos);
			}
		}
	}

    public void DestroyInstantiatedLines()
	{
		foreach (var item in _instantiatedLines)
		{
            Destroy(item);
		}
	}

    void CreateLine()
	{
        _currentLine = Instantiate(linePrefab, new Vector3(0,0, 0), Quaternion.identity);
        _lineRenderer = _currentLine.GetComponent<LineRenderer>();
        _fingerPositions.Clear();
        Vector3 tempFingerPos = GetMousePosition();
        _fingerPositions.Add(tempFingerPos);
		_fingerPositions.Add(tempFingerPos);
		_lineRenderer.SetPosition(0, _fingerPositions[0]);
        _lineRenderer.SetPosition(1, _fingerPositions[1]);
        _instantiatedLines.Add(_currentLine);
    }

    void UpdateLine(Vector3 newFingerPos)
	{
        _fingerPositions.Add(newFingerPos);
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, newFingerPos);
	}

    Vector3 GetMousePosition()
    {
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            Input.mousePosition,_camera,
            out movePos);
        Vector3 positionToReturn = _canvas.transform.TransformPoint(movePos);
        positionToReturn.z = _canvas.transform.position.z - 0.01f;
        return positionToReturn;
    }
}
