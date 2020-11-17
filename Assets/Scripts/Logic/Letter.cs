using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// We use this structure to define a Line between 2 points
/// </summary>
public class StrokeLine
{
	public int Index;
	public Vector3 FirstPosition;
	public Vector3 SecondPosition;
	public float Distance;
	public bool IsLineProperlyDrawn;
}
public class Letter : MonoBehaviour
{
	[HideInInspector]
	public LetterScriptableObject Data;

	[SerializeField]
	private GameObject _numberedCirclePrefab = null;

	/// <summary>
	/// Click offset - so we dont have to click the exact center of a circle
	/// </summary>
	[SerializeField]
	private float _clickOffset = 0;

	/// <summary>
	/// _Distance offset - so when drawing a line we dont have to reach the exact meassured distance 
	/// </summary>
	[SerializeField]
	private float _distanceOffset = 0;

	private RectTransform _rectTransform;

	private List<RectTransform> _numberedCircles = new List<RectTransform>();

	/// <summary>
	/// List with all the lines that should be drawn
	/// </summary>
	private List<StrokeLine> _strokeLines = new List<StrokeLine>();

	/// <summary>
	/// To track which line we are currently drawing
	/// </summary>
	private int _lineIndex = 0;

	/// <summary>
	/// To track on which stroke we currently are
	/// </summary>
	private int _strokeIndex = 0;

	/// <summary>
	/// To track mouse movement change
	/// </summary>
	private Vector2 _deltaPosition;

	/// <summary>
	/// To store mouse movement change
	/// </summary>
	public float _trackedPositionMagnitude;

	/// <summary>
	/// To keep track of previous mouse position
	/// </summary>
	private Vector3 _previousMousePosition;

	/// <summary>
	/// Reference to parent so we can call win or lose screens to activate
	/// </summary>
	private LetterSpawner _letterSpawner;

	/// <summary>
	/// Position of the previous node so we can keep accurate track of trackingMagnitude
	/// </summary>
	private Vector3 _previousNodePosition;

	/// <summary>
	/// Previouse node
	/// </summary>
	private StrokeLine _previousNode;


	/// <summary>
	/// Canvas for calculating accurate positions
	/// </summary>
	private Canvas _canvas;

	// Start is called before the first frame update
	void Start()
	{
		_canvas = GetComponentInParent<Canvas>();
		_letterSpawner = GetComponentInParent<LetterSpawner>();
		_rectTransform = GetComponent<RectTransform>();
		_previousMousePosition = _canvas.ScreenToCanvasPosition(Input.mousePosition);

		Image image = GetComponent<Image>();
		image.sprite = Data.Graphic;

		InitializeStrokePositions();
		InitializeNumberedCirclesDistances();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			OnInputDown();
		}

		_deltaPosition = _canvas.ScreenToCanvasPosition(Input.mousePosition) - _previousMousePosition;

		if (Input.GetMouseButton(0))
		{
			OnInput();	
		}

		if (Input.GetMouseButtonUp(0))
		{
			OnInputUp();
		}

		_previousMousePosition = _canvas.ScreenToCanvasPosition(Input.mousePosition);
	}

	private void OnInputDown()
	{
		if (_lineIndex >= _strokeLines.Count)
			return;

		if (IsPositionOnStrokePoint(_strokeLines[_lineIndex].FirstPosition, _canvas.ScreenToCanvasPosition(Input.mousePosition)))
		{
			_previousMousePosition = _canvas.ScreenToCanvasPosition(Input.mousePosition);
			_trackedPositionMagnitude = 0;
			_previousNodePosition = _strokeLines[_lineIndex].FirstPosition;
			_previousNode = _strokeLines[_lineIndex];
		}
		else
		{
			_lineIndex = -1;
		}
	}

	private void OnInput()
	{
		if (_lineIndex >= _strokeLines.Count || _lineIndex < 0)
			return;

		_trackedPositionMagnitude += _deltaPosition.magnitude;
		if (IsPositionOnStrokePoint(_previousNodePosition, _canvas.ScreenToCanvasPosition(Input.mousePosition)))
		{
			if(_trackedPositionMagnitude > _clickOffset * 2)
			{
				_previousNode.IsLineProperlyDrawn = false;
			}
			_trackedPositionMagnitude = 0;
		}
		else
		{
			if (IsPositionOnStrokePoint(_strokeLines[_lineIndex].SecondPosition, _canvas.ScreenToCanvasPosition(Input.mousePosition)))
			{
				if (_trackedPositionMagnitude <= _strokeLines[_lineIndex].Distance + _distanceOffset / 3 &&
					_trackedPositionMagnitude >= _strokeLines[_lineIndex].Distance - _distanceOffset)
				{
					_strokeLines[_lineIndex].IsLineProperlyDrawn = true;
					_trackedPositionMagnitude = 0;
					_previousNodePosition = _strokeLines[_lineIndex].SecondPosition;
					_previousNode = _strokeLines[_lineIndex];
					_lineIndex++;
				}
			}
		}
	}

	private void OnInputUp()
	{
		if (_lineIndex >= _strokeLines.Count)
		{
			_letterSpawner.ShowWinScreen();
			ResetIndexes();
		}


		if (_lineIndex < 0)
		{
			_letterSpawner.ShowLoseScreen();
			ResetIndexes();
			return;
		}

		int lineCount = Data.PositionsPerStroke[_strokeIndex] - 1;

		for (int i = 0; i < lineCount; i++)
		{
			if (!_strokeLines[i].IsLineProperlyDrawn || _trackedPositionMagnitude != 0)
			{
				_letterSpawner.ShowLoseScreen();
				ResetIndexes();
				return;
			}
		}

		_strokeIndex++;
	}

	private void ResetIndexes()
	{
		_lineIndex = 0;
		_strokeIndex = 0;
		Destroy(gameObject);
	}

	private void InitializeStrokePositions()
	{
		int count = 1;
		foreach (var item in Data.StrokePositions)
		{
			GameObject numberedCircle = Instantiate(_numberedCirclePrefab, _rectTransform);
			RectTransform numberedCircleRT = numberedCircle.GetComponent<RectTransform>();
			numberedCircleRT.anchoredPosition = new Vector2(item.x * _rectTransform.rect.width, item.y * _rectTransform.rect.height);
			NumberedCircle numberedCircleScript = numberedCircle.GetComponent<NumberedCircle>();
			numberedCircleScript.UpdateNumber(count);
			_numberedCircles.Add(numberedCircleRT);
			count++;
		}

	}

	private void InitializeNumberedCirclesDistances()
	{
		int index = 0;
		for (int i = 0; i < Data.PositionsPerStroke.Count; i++)
		{
			for (int j = 0; j < Data.PositionsPerStroke[i]; j++)
			{
				if (j + 1 < Data.PositionsPerStroke[i])
				{
					Vector3 firstPosition;
					firstPosition = _canvas.WorldToCanvasPosition(_numberedCircles[index].position);

					Vector3 secondPosition;
					secondPosition = _canvas.WorldToCanvasPosition(_numberedCircles[index + 1].position);

					_strokeLines.Add(new StrokeLine
					{
						Index = index,
						FirstPosition =firstPosition,
						SecondPosition = secondPosition,
						Distance = Vector3.Distance(firstPosition, secondPosition),
						IsLineProperlyDrawn = false
					});
				}
				index++;
			}
		}

		foreach (var item in _strokeLines)
		{
			print(item.FirstPosition + "  " + item.SecondPosition + "  " +item.Distance);
		}

	}

	private bool IsPositionOnStrokePoint(Vector2 strokePoint, Vector2 mousePosition)
	{
		if (mousePosition.x < strokePoint.x + _clickOffset && mousePosition.x > strokePoint.x - _clickOffset &&
			mousePosition.y < strokePoint.y + _clickOffset && mousePosition.y > strokePoint.y - _clickOffset)
			return true;

		return false;
	}
}
