using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 720;
    [SerializeField] private float bounceAmplitude = 4;

    Vector3 prevPosition;
    float   angle;

    void Start()
    {
        prevPosition = transform.position;
    }

    void Update()
    {
        var deltaPos = (transform.position - prevPosition).magnitude;

        if (deltaPos > 1e-3)
        {
            angle += degreesPerSecond * Time.deltaTime;
            if (angle >= 360.0f) angle -= 360.0f;
        }
        else
        {
            if (angle > 0)
            {
                angle += degreesPerSecond * Time.deltaTime;
                if (angle >= 360) angle = 0;
            }
        }

        transform.localPosition = new Vector3(0.0f, Mathf.Abs(bounceAmplitude * Mathf.Sin(angle * Mathf.Deg2Rad)), 0.0f);

        prevPosition = transform.position;
    }
}
