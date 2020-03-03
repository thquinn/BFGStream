using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScript : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public GameObject shackle;

    bool on = false;
    float baseScale, shackleOffset;
    int timer;

    public void Set(bool on) {
        this.on = on;
        timer = 0;

        if (canvasGroup == null) {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        canvasGroup.alpha = on ? 0 : 1;
        shackle.transform.localPosition = new Vector3(on ? shackleOffset : 0, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (canvasGroup == null) {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        baseScale = transform.localScale.x;
        shackleOffset = shackle.transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (on && canvasGroup.alpha < 1) {
            canvasGroup.alpha += .1f;
        }
        if (on && canvasGroup.alpha == 1) {
            timer++;
            int scaleFrames = Mathf.Abs(14 - timer);
            float t = 1 - Mathf.Clamp01(scaleFrames / 6f);
            float scale = Mathf.Lerp(baseScale, 1f, t);
            transform.localScale = new Vector3(scale, scale, 1);
            if (timer == 10) {
                shackle.transform.localPosition = Vector3.zero;
            }
        }

        if (!on && canvasGroup.alpha > 0) {
            canvasGroup.alpha -= .1f;
        }
    }
}
