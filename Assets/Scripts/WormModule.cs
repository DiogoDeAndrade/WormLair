using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WormModule : MonoBehaviour
{
    public float moduleValue = 1.0f;
    public float moduleRadius = 20.0f;

    [Header("Procedural")]
    public Worm         worm;
    public WormModule   prevModule;
    public float        moveSpeed;
    public float        rotationSpeed;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!worm.enabled) return;

        var headTransform = (prevModule) ? (prevModule.transform) : (worm.transform);
        var distance = (prevModule) ? (moduleRadius + prevModule.moduleRadius) : (moduleRadius + worm.moduleRadius);

        // Rotate until the position to go to the next waypoint
        Vector3 followPos = headTransform.position;

        Vector3 dir = followPos - transform.position;
        if (dir.magnitude > distance)
        {
            dir.Normalize();

            float dp = Vector3.Dot(transform.up, dir);
            if (dp < 0.99f)
            {
                Quaternion dirQuaternion = Quaternion.LookRotation(Vector3.forward, dir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, dirQuaternion, 360.0f * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, followPos, moveSpeed * Time.deltaTime);
        }
    }

    public void DestroyModule()
    {
        worm.RemoveModule(this);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, moduleRadius);
    }

}
