using OkapiKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleDestroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        var wormModule = collider.GetComponent<WormModule>();
        if (wormModule)
        {
            if (wormModule.worm.hasExit)
            {
                wormModule.DestroyModule();
            }
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        var wormModule = collider.GetComponent<WormModule>();
        if (wormModule)
        {
            if (wormModule.worm.hasExit)
            {
                wormModule.DestroyModule();
            }
        }
    }
}
