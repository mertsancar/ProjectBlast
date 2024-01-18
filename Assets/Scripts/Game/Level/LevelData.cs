using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public List<LevelInfo> levels;
}

[Serializable]
public struct LevelInfo
{
    public int levelIndex;
    public int ballCount;
    public int moveCount;
    public Vector3 levelStartPosition;
    public GameObject levelPrefab;
    public List<ColorInfo> colorPercentages;
    public List<TargetBubbleInfo> targetColors;
}

[Serializable]
public struct ColorInfo
{
    public BubbleColor color;
    public int percentage;
}

[Serializable]
public struct TargetBubbleInfo
{
    public BubbleColor color;
    public int count;
}