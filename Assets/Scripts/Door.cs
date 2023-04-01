using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform wormSpawnPoint;

    private Animator animator;
    private Worm     currentWorm;

    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    public void EnableEntry(Worm worm)
    {
        animator.SetTrigger("FlashEnter");
        currentWorm = worm;
    }

    public void EnableExit()
    {
        animator.SetTrigger("FlashExit");
    }

    public void EnableWorm()
    {
        if (currentWorm != null)
        {
            currentWorm.enabled = true;
        }
    }
}
