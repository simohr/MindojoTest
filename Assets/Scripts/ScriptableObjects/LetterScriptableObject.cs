using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Letter", menuName = "ScriptableObjects/CreateLetter", order = 1)]
public class LetterScriptableObject : ScriptableObject
{
	public string Name;
	public List<int> PositionsPerStroke;
	public List<Vector2> StrokePositions;
	public Sprite Graphic;
}
