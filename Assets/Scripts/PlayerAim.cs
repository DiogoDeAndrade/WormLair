using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OkapiKit;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Hypertag cameraTag;

    Vector3 prevPosition;

    void Start()
    {
        prevPosition = transform.position;
    }

    void Update()
    {
        Camera camera = gameObject.FindObjectOfTypeWithHypertag<Camera>(cameraTag);
        var mousePos = camera.ScreenToWorldPoint(Input.mousePosition);

        var deltaPos = (mousePos - transform.position).normalized;

        deltaPos = new Vector2(-deltaPos.y, deltaPos.x);

        if (transform.forward.z < 0)
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, -deltaPos);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(transform.forward, deltaPos);
        }
    }
}
