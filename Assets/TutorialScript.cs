using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialScript : MonoBehaviour {
    private static readonly string[] TEXTS = new string[]{ "think of a word exactly in between...",
                                                           "and whisper it to thqbot to play along!",};
    private static readonly int TOTAL_TIME = 450, FADE_TIME = 15;

    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;

    int index, timer;

    void Start() {
        text.text = TEXTS[0];
    }
    void Update()
    {
        timer++;
        if (timer == TOTAL_TIME) {
            index = (index + 1) % TEXTS.Length;
            text.text = TEXTS[index];
            timer = 0;
        }
        if (timer < FADE_TIME) {
            canvasGroup.alpha = timer / (float)FADE_TIME;
        } else if (timer > TOTAL_TIME - FADE_TIME) {
            canvasGroup.alpha = (TOTAL_TIME - timer) / (float)FADE_TIME;
        } else {
            canvasGroup.alpha = 1;
        }
    }
}
