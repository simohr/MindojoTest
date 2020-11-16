using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	[SerializeField]
	private float _offset = 0;
	[SerializeField]
	private float _distanceOffset = 0;

	private RectTransform _rectTransform;

	private List<RectTransform> _numberedCircles = new List<RectTransform>();

	private List<StrokeLine> _strokeLines = new List<StrokeLine>();

	public int lineIndex = 0;
	public int strokeIndex = 0;

	private Vector2 _deltaPosition;
	public float _trackedPositionMagnitude;
	private Vector3 _previousPosition;
	private Vector2 _vectorOffset;

	private LetterSpawner _letterSpawner;
	private Vector3 _previousNodePosition;
	private StrokeLine _previousNode;

	private Canvas _canvas;

	// Start is called before the first frame update
	void Start()
	{
		_canvas = GetComponentInParent<Canvas>();
		_letterSpawner = GetComponentInParent<LetterSpawner>();
		_rectTransform = GetComponent<RectTransform>();
		_vectorOffset = new Vector2(_offset, _offset);
		_previousPosition = _canvas.ScreenToCanvasPosition(Input.mousePosition);

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

		_deltaPosition = _canvas.ScreenToCanvasPosition(Input.mousePosition) - _previousPosition;

		if (Input.GetMouseButton(0))
		{
			OnInput();	
		}

		if (Input.GetMouseButtonUp(0))
		{
			OnInputUp();
		}

		_previousPosition = _canvas.ScreenToCanvasPosition(Input.mousePosition);
	}

	private void OnInputDown()
	{
		if (lineIndex >= _strokeLines.Count)
			return;

		if (IsPositionOnStrokePoint(_strokeLines[lineIndex].FirstPosition, _canvas.ScreenToCanvasPosition(Input.mousePosition)))
		{
			_previousPosition = _canvas.ScreenToCanvasPosition(Input.mousePosition);
			_trackedPositionMagnitude = 0;
			_previousNodePosition = _strokeLines[lineIndex].FirstPosition;
			_previousNode = _strokeLines[lineIndex];
		}
		else
		{
			lineIndex = -1;
		}
	}

	private void OnInput()
	{
		if (lineIndex >= _strokeLines.Count || lineIndex < 0)
			return;

		_trackedPositionMagnitude += _deltaPosition.magnitude;
		if (IsPositionOnStrokePoint(_previousNodePosition, _canvas.ScreenToCanvasPosition(Input.mousePosition)))
		{
			if(_trackedPositionMagnitude > _offset * 2)
			{
				_previousNode.IsLineProperlyDrawn = false;
			}
			_trackedPositionMagnitude = 0;
		}
		else
		{
			if (IsPositionOnStrokePoint(_strokeLines[lineIndex].SecondPosition, _canvas.ScreenToCanvasPosition(Input.mousePosition)))
			{
				if (_trackedPositionMagnitude <= _strokeLines[lineIndex].Distance + _distanceOffset / 3 &&
					_trackedPositionMagnitude >= _strokeLines[lineIndex].Distance - _distanceOffset)
				{
					_strokeLines[lineIndex].IsLineProperlyDrawn = true;
					_trackedPositionMagnitude = 0;
					_previousNodePosition = _strokeLines[lineIndex].SecondPosition;
					_previousNode = _strokeLines[lineIndex];
					lineIndex++;
				}
			}
		}
	}

	private void OnInputUp()
	{
		if (lineIndex >= _strokeLines.Count)
		{
			_letterSpawner.ShowWinScreen();
			ResetIndexes();
		}


		if (lineIndex < 0)
		{
			_letterSpawner.ShowLoseScreen();
			ResetIndexes();
			return;
		}

		int lineCount = Data.PositionsPerStroke[strokeIndex] - 1;

		for (int i = 0; i < lineCount; i++)
		{
			if (!_strokeLines[i].IsLineProperlyDrawn || _trackedPositionMagnitude != 0)
			{
				_letterSpawner.ShowLoseScreen();
				ResetIndexes();
				return;
			}
		}

		strokeIndex++;
	}

	private void ResetIndexes()
	{
		lineIndex = 0;
		strokeIndex = 0;
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
		if (mousePosition.x < strokePoint.x + _vectorOffset.x && mousePosition.x > strokePoint.x - _vectorOffset.x &&
			mousePosition.y < strokePoint.y + _vectorOffset.y && mousePosition.y > strokePoint.y - _vectorOffset.y)
			return true;

		return false;
	}
}
