using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using OkapiKit;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameData       gameData;
    [SerializeField] private int            currentWave = 1;
    [SerializeField] private Door[]         doors;
    [SerializeField] private BoxCollider2D  innerArea;
    [SerializeField] private BoxCollider2D  outerArea;
    [SerializeField] private Worm           wormPrefab;
    [SerializeField] private WormModule     wormTailModule;
    [SerializeField] private WormModule[]   wormModulePrefabs;

    [SerializeField] private Hypertag       playerTag;

    [SerializeField] private TextMeshProUGUI    waveText;
    [SerializeField] private TextMeshProUGUI    gameOverText;

    private bool[]      doorUsed;
    private List<Worm>  wormList;
    private float       spawnTimer = 2.0f;

    void Start()
    {
#if !UNITY_EDITOR
        currentWave = 0;
#endif
        innerArea.enabled = false;
        outerArea.enabled = false;
        doorUsed = new bool[doors.Length];
        for (int i = 0; i < doorUsed.Length; i++) 
        {
            doorUsed[i] = false;
        }

        wormList = new List<Worm>();
        spawnTimer = 2.0f;

        UpdateUI();
    }

    void Update()
    {
        var player = gameObject.FindObjectWithHypertag(playerTag);
        if (player == null)
        {
            gameOverText.enabled = true;

            if (Input.GetButtonDown("Fire1"))
            {
                // Restart
                SceneManager.LoadScene(0);
            }

            return;
        }

        wormList.RemoveAll((w) => w == null);
        if (wormList.Count == 0)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                currentWave++;
                UpdateUI();

                int maxWorm = Mathf.FloorToInt(1 + gameData.wormPerWave * currentWave);
                int     nWorm = Random.Range(1, maxWorm);
                float   multiplier = Mathf.Min(1.0f, Mathf.Floor(maxWorm / nWorm));
                for (int i = 0; i < nWorm; i++)
                {
                    SpawnWorm(multiplier);
                }

                spawnTimer = 2.0f;
            }
        }
    }

    [Button("Test Spawn Worm")]
    void TestSpawnWorm()
    {
        SpawnWorm(1.0f);
    }

    void SpawnWorm(float multiplier)
    {
        int nWaypoints = Mathf.FloorToInt(1 + gameData.wayPointsPerWave * currentWave);
        var (entryId, exitId, path) = BuildPath(nWaypoints);
        if (path == null) return;

        var worm = BuildWorm(multiplier, path, entryId, exitId);
        wormList.Add(worm);

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

    Worm BuildWorm(float multiplier, Path path, int entryId, int exitId)
    {
        var startPos = doors[entryId].wormSpawnPoint.position;
        var startRotation = doors[entryId].wormSpawnPoint.rotation;

        Worm worm = Instantiate(wormPrefab, startPos, startRotation);

        worm.enabled = false;
        worm.moveSpeed = gameData.baseMoveSpeed + gameData.moveSpeedPerWave * currentWave;
        worm.rotationSpeed = gameData.baseRotationSpeed + gameData.rotationSpeedPerWave * currentWave;
        worm.SetPath(path, entryId, exitId);

        float   totalValue = (gameData.baseValue + gameData.valuePerWave * (currentWave - 1)) * multiplier;
        Debug.Log(multiplier);
        Debug.Log(totalValue);

        int count = 0;
        while (totalValue > 0)
        {
            var candidates = new List<WormModule>(wormModulePrefabs);
            candidates.RemoveAll((w) => w.moduleValue > totalValue);

            if (candidates.Count == 0) break;

            // Sort by value
            candidates.Sort((m1, m2) => m1.moduleValue.CompareTo(m2.moduleValue));

            WormModule  selectedModule = null;
            float       accum = 0;

            foreach (var c in candidates) accum += c.moduleValue;

            float val = Random.Range(0.0f, accum);
            foreach (var c in candidates)
            {
                val -= c.moduleValue;
                if (val < 0)
                {
                    selectedModule = c;
                    break;
                }
            }
            if (selectedModule == null) selectedModule = candidates[candidates.Count - 1];

            var module = Instantiate(selectedModule, null);
            module.moveSpeed = worm.moveSpeed;
            module.rotationSpeed = worm.rotationSpeed;

            totalValue -= selectedModule.moduleValue;

            worm.AddModule(module);

            count++;

            if (count >= gameData.maxSegments)
            {
                break;
            }
        }
        if (wormTailModule)
        {
            var module = Instantiate(wormTailModule, null);
            module.moveSpeed = worm.moveSpeed;
            module.rotationSpeed = worm.rotationSpeed;

            worm.AddModule(module);
        }

        return worm;
    }

    public void FreeDoor(int id)
    {
        doorUsed[id] = false;
    }

    void UpdateUI()
    {
        if (waveText)
        {
            waveText.text = $"Wave: <color=#FFFFFF>{currentWave}";
        }
    }
}
