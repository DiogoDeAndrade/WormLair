using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using OkapiKit;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameData       gameData;
    [SerializeField] private int            currentWave = 1;
    [SerializeField] private Door[]         doors;
    [SerializeField] private BoxCollider2D  innerArea;
    [SerializeField] private BoxCollider2D  outerArea;
    [SerializeField] private Worm           wormPrefab;
    [SerializeField] private WormModule[]   wormModulePrefabs;

    private bool[] doorUsed;

    void Start()
    {
        currentWave = 1;
        innerArea.enabled = false;
        outerArea.enabled = false;
        doorUsed = new bool[doors.Length];
        for (int i = 0; i < doorUsed.Length; i++) 
        {
            doorUsed[i] = false;
        }
    }

    void Update()
    {
    }

    [Button("Test Spawn Worm")]
    void SpawnWorm()
    {
        int nWaypoints = Mathf.FloorToInt(1 + gameData.wayPointsPerWave * currentWave);
        var (entryId, exitId, path) = BuildPath(nWaypoints);
        if (path == null) return;

        var worm = BuildWorm(path, entryId, exitId);

        doorUsed[entryId] = true;
        doorUsed[exitId] = true;

        doors[entryId].EnableEntry(worm);
        doors[exitId].EnableExit();

        Destroy(path.gameObject);
    }

    (int, int, Path) BuildPath(int nWaypoints)
    {
        // Select entry door
        var (entryId, exitId) = GetRandomDoor();
        if (entryId == -1) return (-1, -1, null);
        var entry = doors[entryId];
        var exit = doors[exitId];

        GameObject  pathObject = new GameObject("Path");
        Path        path = pathObject.AddComponent<Path>();

        path.SetWorldSpace(true);
        path.SetPathType(Path.PathType.Linear);

        List<Vector3> points = new List<Vector3>();
        points.Add(entry.wormSpawnPoint.position);
        points.Add(entry.transform.position + entry.transform.up * 80.0f);
        for (int i = 0; i < nWaypoints; i++)
        {
            BoxCollider2D   collider = outerArea;
            if ((i == 0) || (i == nWaypoints - 1)) collider = innerArea;
            Vector2         s = collider.size * 0.5f;

            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(collider.offset.x - s.x, collider.offset.x + s.x);
            pos.y = Random.Range(collider.offset.y - s.y, collider.offset.y + s.y);

            points.Add(pos);
        }
        points.Add(exit.transform.position + exit.transform.up * 80.0f);
        points.Add(exit.transform.position);
        points.Add(exit.transform.position - exit.transform.up * 200.0f);

        path.SetEditPoints(points);

        return (entryId, exitId, path);
    }

    (int, int) GetRandomDoor()
    {
        int nFail = 0;
        while (nFail < 50)
        {
            int entryId = Random.Range(0, doors.Length);

            if (!doorUsed[entryId])
            {
                var exitId = (entryId + (doors.Length / 2)) % doors.Length;
                if (!doorUsed[exitId])
                {
                    return (entryId, exitId);
                }
            }

            nFail++;
        }

        return (-1, -1);
    }

    Worm BuildWorm(Path path, int entryId, int exitId)
    {
        var startPos = doors[entryId].wormSpawnPoint.position;
        var startRotation = doors[entryId].wormSpawnPoint.rotation;

        Worm worm = Instantiate(wormPrefab, startPos, startRotation);

        worm.enabled = false;
        worm.moveSpeed = gameData.baseMoveSpeed + gameData.moveSpeedPerWave * currentWave;
        worm.rotationSpeed = gameData.baseRotationSpeed + gameData.rotationSpeedPerWave * currentWave;
        worm.SetPath(path, entryId, exitId);

        float   totalValue = gameData.baseValue + gameData.valuePerWave * currentWave;

        int count = 0;
        while (totalValue > 0)
        {
            var candidates = new List<WormModule>(wormModulePrefabs);
            candidates.RemoveAll((w) => w.moduleValue > totalValue);

            if (candidates.Count == 0) break;

            int r = Random.Range(0, candidates.Count);

            var module = Instantiate(candidates[r], null);
            module.moveSpeed = worm.moveSpeed;
            module.rotationSpeed = worm.rotationSpeed;

            worm.AddModule(module);

            count++;

            if (count >= gameData.maxSegments)
            {
                break;
            }
        }

        return worm;
    }

    public void FreeDoor(int id)
    {
        doorUsed[id] = false;
    }
}
