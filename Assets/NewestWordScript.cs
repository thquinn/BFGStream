using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewestWordScript : MonoBehaviour
{
    static Vector3 centerPosition = new Vector3(0, 0, -100);
    static int winDelayFrames = 60;

    CanvasGroup canvasGroup;
    RectTransform rectTransform;
    Vector3 originalPosition;
    float dy;
    public bool destroying = false;
    bool win;
    int frames;

    public void Set(int player, int playerCount, string word, bool win) {
        rectTransform = (RectTransform)transform;
        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
        bool simpleSwap = player == 1 && playerCount == 2;
        if (simpleSwap) {
            Vector3 localPosition = transform.localPosition;
            localPosition.x *= -1;
            transform.localPosition = localPosition;
        } else if (playerCount > 2 && !(player == 0 && playerCount == 3)) {
            if (player > 0 && playerCount == 3) {
                player++;
            }
            int[] xOffs = new int[] { -644, -232, 232, 644 };
            Vector3 localPosition = transform.localPosition;
            localPosition.x = xOffs[player];
            transform.localPosition = localPosition;
            rectTransform.sizeDelta = new Vector2(275, 80);
        }
        tmp.text = '"' + word.ToUpper() + '"';
        this.win = win;
    }

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        originalPosition = transform.localPosition;
        Vector3 localPosition = transform.localPosition;
        localPosition.y -= 100;
        transform.localPosition = localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        frames++;

        if (destroying) {
            canvasGroup.alpha -= .05f;
            if (canvasGroup.alpha <= 0) {
                DestroyImmediate(gameObject);
                return;
            }
            dy += .0033f;
            transform.Translate(0, dy, 0, Space.Self);
            return;
        } else {
            canvasGroup.alpha += .05f;
            Vector3 localPosition = transform.localPosition;
            localPosition.y = Mathf.Lerp(localPosition.y, originalPosition.y, .125f);
            transform.localPosition = localPosition;
        }

        if (!destroying && win && frames > winDelayFrames) {
            float t = Mathf.Clamp01((frames - winDelayFrames) / 60f);
            
            t = Util.EaseInOutQuad(t);
            originalPosition.z = centerPosition.z;
            transform.localPosition = Vector3.Lerp(originalPosition, centerPosition, t);
            transform.localScale = new Vector3(1 + 2 * t, 1 + 2 * t, 1);
            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, new Vector2(550, 80), .002f);
        }
    }
}
