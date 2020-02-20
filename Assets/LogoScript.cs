using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoScript : MonoBehaviour
{
    public GameObject[] models;
    float[] originalX;
    bool timeToPick = true;
    int anim;
    bool flip;
    float extraY;

    // Start is called before the first frame update
    void Start()
    {
        originalX = new float[models.Length];
        for (int i = 0; i < models.Length; i++) {
            originalX[i] = models[i].transform.localPosition.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Time.time / 1.5f) % 31;
        if (t <= 1) {
            if (timeToPick) {
                anim = Random.Range(0, 5);
                flip = Random.value < .5f;
                timeToPick = false;
            }
            switch (anim) {
                case 0:
                    AnimationSwivelLetters(t);
                    break;
                case 1:
                    AnimationStackLetters(t);
                    break;
                case 2:
                    AnimationGridLetters(t);
                    break;
                case 3:
                    AnimationQuickSpin(t);
                    break;
                default:
                    AnimationSpring(t);
                    break;
            }
        } else {
            timeToPick = true;
            extraY = 0;
        }

        float angle = Time.time * 0.5f;
        transform.localRotation = Quaternion.Euler(9 * Mathf.Cos(angle), 9 * Mathf.Sin(angle) + extraY, 0);
    }

    void AnimationSwivelLetters(float t) {
        float[] modelTs = new float[] {
            Util.EaseInOutQuad(Mathf.Clamp01(t * 3 / 2)),
            Util.EaseInOutQuad(Mathf.Clamp01((t - 0.111f) * 3 / 2)),
            Util.EaseInOutQuad(Mathf.Clamp01((t - 0.222f) * 3 / 2)),
            Util.EaseInOutQuad(Mathf.Clamp01((t - 0.333f) * 3 / 2))
        };
        for (int i = 0; i < modelTs.Length; i++) {
            models[i].transform.localRotation = Quaternion.Euler(0, modelTs[i] * -360, 0);
        }
    }
    void AnimationStackLetters(float t) {
        if (t > .5) {
            t = 1 - t;
        }
        t *= 4;
        t = Mathf.Clamp01(t);
        t = Util.EaseInOutQuad(t);
        float distance = 1f;
        for (int i = 0; i < 4; i++) {
            float targetZ = distance * (-1.5f + i);
            float halfMult = 1 - Mathf.Abs(.5f - t) * 3 + 1;
            targetZ *= halfMult;
            Vector3 position = new Vector3(Mathf.Lerp(originalX[i], 0, t), 0, Mathf.Lerp(0, targetZ, t));
            models[i].transform.localPosition = position;
        }
    }
    void AnimationGridLetters(float t) {
        if (t > .5) {
            t = 1 - t;
        }
        t *= 8;
        t = Mathf.Clamp01(t);
        t = Util.EaseInOutQuad(t);
        for (int i = 0; i < 4; i++) {
            float x = -.5f + (i % 2);
            float y = -.5f + (i / 2);
            x *= .8f;
            y *= .8f;
            Vector3 position = new Vector3(Mathf.Lerp(originalX[i], x, t), -Mathf.Lerp(0, y, t), 0);
            models[i].transform.localPosition = position;
        }
    }
    void AnimationQuickSpin(float t) {
        bool secondHalf = false;
        if (t > .5f) {
            t = 1 - t;
            secondHalf = true;
        }
        t = 14.1369f * Mathf.Pow(t, 5) - 35.3423f * Mathf.Pow(t, 4) + 24.2857f * Mathf.Pow(t, 3) - 1.08631f * Mathf.Pow(t, 2) - t; // thanks for the quintic, Wolfram
        extraY = 360 * t;
        if (secondHalf) {
            extraY = 360 - extraY;
        }
        if (flip) {
            extraY *= -1;
        }
    }
    void AnimationSpring(float t) {
        if (t < .4f) {
            t /= .4f;
            transform.localPosition = new Vector3(0, t * t * -.4f, 0);
        } else {
            t -= .4f;
            t /= .6f;
            float factor = 1 - t;
            factor *= .2f;
            float y = factor * Mathf.Cos(t * 30);
            transform.localPosition = new Vector3(0, y, 0);
        }
    }
}
