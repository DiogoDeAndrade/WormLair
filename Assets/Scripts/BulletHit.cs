using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OkapiKit;

public class BulletHit : MonoBehaviour
{
    [SerializeField] private Hypertag[] tags;
    [SerializeField] private GameObject effectPrefb;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.HasHypertags(tags))
        {
            var rotation = Quaternion.LookRotation(Vector3.forward, -transform.right);
            Instantiate(effectPrefb, transform.position, rotation);
        }
    }
}
