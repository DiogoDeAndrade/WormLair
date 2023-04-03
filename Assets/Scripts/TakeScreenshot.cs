using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public static class TakeScreenshot 
{
    [MenuItem("OkapiKit/Take GameScene Screenshot %#F11")]
    static void Execute()
    {
        string filename = "";
        for (int i = 0; i < 9999; i++)
        {
            filename = string.Format("Screenshots/Screenshot{0:0000}.png", i);
            if (!System.IO.File.Exists(filename))
            {
                break;
            }
        }

        ScreenCapture.CaptureScreenshot(filename, 1);
    }
}

#endif