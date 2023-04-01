using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OkapiKit;

public class PlayerFlip : MonoBehaviour
{
    [SerializeField] private Hypertag   cameraTag;
    [SerializeField] private Transform  gunTransform;

    Vector3 prevPosition;

    void Start()
    {
        prevPosition = transform.position;
    }

    void Update()
    {
        Camera camera = gameObject.FindObjectOfTypeWithHypertag<Camera>(cameraTag);
        var    mousePos = camera.ScreenToWorldPoint(Input.mousePosition);

        var deltaPos = mousePos - transform.position;

        if ((deltaPos.x < 0) && (transform.right.x > 0.0f)) transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        else if ((deltaPos.x > 0) && (transform.right.x < 0.0f)) transform.rotation = Quaternion.identity;
    }
}
