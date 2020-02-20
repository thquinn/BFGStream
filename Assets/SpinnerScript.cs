using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerScript : MonoBehaviour
{
    public GameScript gameScript;
    public CanvasGroup canvasGroup;

    void Update()
    {
        transform.Rotate(0, 0, -3.57f);
        if (gameScript != null && gameScript.spinnerOn) {
            canvasGroup.alpha += .025f;
        } else {
            canvasGroup.alpha = 0;
        }
    }
}
