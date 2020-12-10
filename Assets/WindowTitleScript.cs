using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowTitleScript : MonoBehaviour
{
    public GameScript gameScript;

    //Import the following.
    [DllImport("user32.dll", EntryPoint = "SetWindowText")]
    public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern System.IntPtr FindWindow(System.String className, System.String windowName); //Get the window handle.

    IntPtr windowPtr;
    bool lightningTitle;

    // Start is called before the first frame update
    void Start()
    {
        windowPtr = FindWindow(null, "BFGStream");
    }

    // Update is called once per frame
    void Update()
    {
        bool lightningRound = gameScript.lightningRound && gameScript.words.Count > 0;
        if (lightningTitle != lightningRound) {
            lightningTitle = lightningRound;
            SetWindowText(windowPtr, lightningTitle ? "BFGStream!" : "BFGStream");
        }
    }
}
