using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WormLair/Game Data")]
public class GameData : ScriptableObject
{
    public float wormPerWave = 0.1f;
    public float wayPointsPerWave = 0.2f;
    public float baseMoveSpeed = 100.0f;
    public float moveSpeedPerWave = 10.0f;
    public float baseRotationSpeed = 90.0f;
    public float rotationSpeedPerWave = 5.0f;
    public float baseValue = 5.0f;
    public float valuePerWave = 2.0f;
    public int   maxSegments = 10;
}
