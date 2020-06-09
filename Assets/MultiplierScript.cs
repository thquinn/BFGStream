using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplierScript : MonoBehaviour
{
    public GameScript gameScript;
    public TextMeshProUGUI text;
    public ParticleSystem particles;

    bool isShowing;
    int transitionFrames = 999;
    int lastCount;

    void Start() {
        transform.localScale = new Vector3(0, 0, 1);
        ModifyParticleAlpha(-1);
    }

    void Update()
    {
        if (!isShowing && gameScript.doubledUpViewers.Count > 0) {
            isShowing = true;
            transitionFrames = 0;
        } else if (isShowing && gameScript.doubledUpViewers.Count == 0) {
            isShowing = false;
            transitionFrames = 0;
        }
        transitionFrames++;
        if (isShowing) {
            float scaleTarget = Mathf.Clamp(2 - transitionFrames * .1f, 1, 1.5f);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(scaleTarget, scaleTarget, 1), .2f);
            if (gameScript.doubledUpViewers.Count != lastCount) {
                lastCount = gameScript.doubledUpViewers.Count;
                float multiplier = GameScript.GetDoubleUpMultiplier(lastCount);
                float fractional = multiplier % 1;
                text.text = string.Format("<b>×</b>{0}<size=50%><voffset=.7em>{1}", Mathf.FloorToInt(multiplier), fractional == 0 ? "" : fractional.ToString().Substring(1));
            }
        } else {
            float scaleTarget = Mathf.Clamp(2 - transitionFrames * .4f, 0, 1.5f);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(scaleTarget, scaleTarget, 1), .2f);
        }
        // Fade particles.
        ModifyParticleAlpha(isShowing ? .1f : -.1f);
    }

    void ModifyParticleAlpha(float d) {
        Color c = particles.main.startColor.color;
        c.a = Mathf.Clamp01(c.a + d);
        var main = particles.main;
        main.startColor = c;
    }
}
