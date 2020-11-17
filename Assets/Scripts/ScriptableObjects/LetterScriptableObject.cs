using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Letter", menuName = "ScriptableObjects/CreateLetter", order = 1)]
public class LetterScriptableObject : ScriptableObject
{
	/// <summary>
	/// Name for clarity
	/// </summary>
	public string Name;

	/// <summary>
	/// How many strokes we have - each element of this list defines how many points there are in each stroke
	/// </summary>
	public List<int> PositionsPerStroke;

	/// <summary>
	/// The length of this list should be based on the count of points in PositionsPerStroke. The points should be in sequential order.
	/// </summary>
	public List<Vector2> StrokePositions;

	/// <summary>
	/// Sprite of the letter
	/// </summary>
	public Sprite Graphic;
}
