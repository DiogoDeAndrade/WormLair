using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private float      cooldown;
    [SerializeField] private Transform  shootPoint;
    [SerializeField] private GameObject bulletPrefab;

    float cooldownTimer;

    void Start()
    {
        cooldownTimer = 0;
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer < 0) cooldownTimer = 0;
        }

        if ((Input.GetButton("Fire1")) && (cooldownTimer <= 0))
        {
            Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            cooldownTimer = cooldown;
        }
    }
}
