using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OkapiKit;

public class Worm : MonoBehaviour
{
    public List<Vector3> path;
    public float         rotationSpeed = 90.0f;
    public float         moveSpeed = 100.0f;

    private int pathIndex;
    private int entryId;
    private int exitId;

    void Start()
    {
        transform.position = path[0];
        pathIndex = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate until the position to go to the next waypoint
        Vector3 nextPosition = path[pathIndex];

        Vector3 dir = nextPosition - transform.position;
        if (dir.magnitude < 1e-3)
        {
            pathIndex++;
            if (pathIndex >= path.Count)
            {
                DestroyWorm();
                return;
            }

            nextPosition = path[pathIndex];
            dir = nextPosition - transform.position;
        }

        dir.Normalize();

        float dp = Vector3.Dot(transform.up, dir);
        if (dp < 0.99f)
        {
            Quaternion dirQuaternion = Quaternion.LookRotation(Vector3.forward, dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, dirQuaternion, rotationSpeed * Time.deltaTime);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);
    }

    public void SetPath(Path path, int entryId, int exitId)
    {
        this.path = path.GetEditPoints();
        this.entryId = entryId;
        this.exitId = exitId;
    }

    void DestroyWorm()
    {
        Destroy(gameObject);

        var levelManager = FindObjectOfType<LevelManager>();
        if (levelManager)
        {
            levelManager.FreeDoor(entryId);
            levelManager.FreeDoor(exitId);
        }
    }
}
