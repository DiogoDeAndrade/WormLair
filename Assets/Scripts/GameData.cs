using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WormLair/Game Data")]
public class GameData : ScriptableObject
{
    public float wayPointsPerWave = 0.2f;
    public float baseMoveSpeed = 100.0f;
    public float moveSpeedPerWave = 10.0f;
    public float baseRotationSpeed = 90.0f;
    public float rotationSpeedPerWave = 5.0f;
}
