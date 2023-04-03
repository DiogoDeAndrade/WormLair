using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OkapiKit;

public class BulletHit : MonoBehaviour
{
    [SerializeField] private Hypertag[] effectTags;
    [SerializeField] private GameObject effectPrefb;

    [SerializeField] private Hypertag[] damageTags;
    [SerializeField] private float      damage = 1;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.HasHypertags(effectTags))
        {
            var rotation = Quaternion.LookRotation(Vector3.forward, -transform.right);
            Instantiate(effectPrefb, transform.position, rotation);
        }
        if (collider.gameObject.HasHypertags(damageTags))
        {
            HealthSystem hs = collider.GetComponent<HealthSystem>();
            if (hs)
            {
                hs.Change(-damage);
            }
        }
    }
}
