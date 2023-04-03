using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField]
    private GameObject          explosionPrefab;
    [SerializeField]
    private ParticleSystem[]    explosionPS;

    void Start()
    {
        HealthSystem hs = GetComponent<HealthSystem>();
        hs.onDeath += Explode;
    }

    private void Explode()
    {
        if (explosionPrefab)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }
        if (explosionPS != null)
        {
            foreach (var ps in explosionPS)
            {
                ps.Play();
            }

            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            if (spriteRenderers != null)
            {
                foreach (var sr in spriteRenderers)
                {
                    sr.enabled = false;
                }
            }

            var colliders = GetComponentsInChildren<Collider2D>();
            if (colliders != null)
            {
                {
                    foreach (var c in colliders)
                    {
                        c.enabled = false;
                    }
                }
            }
        }
        StartCoroutine(ExplodeCR());
    }

    IEnumerator ExplodeCR()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
