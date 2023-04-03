using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using OkapiKit;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    private float currentHealth;

    public delegate void OnDeathHandler();
    public event OnDeathHandler onDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Change(float delta)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth + delta, 0, maxHealth);
        if (currentHealth <= 0)
        {
            if (onDeath != null) onDeath.Invoke();
        }
        else
        {
            var flashes = GetComponentsInChildren<ActionFlash>();
            foreach (var flash in flashes)
            {
                flash.Execute();
            }
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
