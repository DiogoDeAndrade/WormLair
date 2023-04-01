using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OkapiKit;
using System.Reflection;

public class Worm : MonoBehaviour
{
    public List<Vector3>    path;
    public float            rotationSpeed = 90.0f;
    public float            moveSpeed = 100.0f;

    public bool             isMoving = false;
    public float            moduleRadius => 24;

    private int                 pathIndex;
    private int                 entryId;
    private int                 exitId;
    private List<WormModule>    modules;

    public bool hasExit => (pathIndex >= path.Count);

    void Start()
    {
        transform.position = path[0];
        pathIndex = 1;
    }

    // Update is called once per frame
    void Update()
    {
        isMoving = false;

        if (hasExit)
        {
            if (modules.Count == 0)
            {
                DestroyWorm();
            }
            return;
        }

        // Rotate until the position to go to the next waypoint
        Vector3 nextPosition = path[pathIndex];

        Vector3 dir = nextPosition - transform.position;
        if (dir.magnitude < 1e-3)
        {
            pathIndex++;
            if (hasExit)
            {
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
        isMoving = true;
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

    public void AddModule(WormModule module)
    {
        if (modules == null) modules = new List<WormModule>();
        module.worm = this;
        module.prevModule = (modules.Count > 0) ? (modules[modules.Count - 1]) : (null);

        float offset = module.moduleRadius + 24.0f;
        for (int i = 0; i < modules.Count; i++)
        {
            offset = modules[i].moduleRadius * 2.0f;
        }

        var headTransform = (module.prevModule) ? (module.prevModule.transform) : (module.worm.transform);
        module.transform.position = headTransform.position - module.worm.transform.up * offset;

        modules.Add(module);
    }

    public void RemoveModule(WormModule module)
    {
        WormModule prevModule = null;
        for (int i = 0; i < modules.Count; i++)
        {
            if (modules[i] == module)
            {
                // Remove this
                modules[i] = null;
            }
            else
            {
                if (modules[i].prevModule == module)
                {
                    modules[i].prevModule = prevModule;                    
                }
                prevModule = modules[i];
            }
        }
        modules.RemoveAll((m) => (m == null));
    }
}
