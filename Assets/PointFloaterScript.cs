using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointFloaterScript : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI tmp;

    int frames;

    public void Set(TextMeshProUGUI playerLabel, PointFloaterIcon icon, int amount) {
        RectTransform rectTransform = (RectTransform)transform;
        transform.position = playerLabel.gameObject.transform.position;
        rectTransform.sizeDelta = playerLabel.rectTransform.sizeDelta;
        tmp.alignment = playerLabel.alignment;
        string amountText = amount > 0 ? "+" + amount : amount.ToString();
        tmp.text = string.Format("<size=70%><sprite index={0} color=#DFD5EA></size> {1}", (int)icon, amountText);
    }

    // Update is called once per frame
    void Update()
    {
        frames++;
        
        if (frames > 180) {
            canvasGroup.alpha -= .02f;
            if (canvasGroup.alpha == 0) {
                DestroyImmediate(gameObject);
                return;
            }
        } else {
            canvasGroup.alpha += .02f;
        }

        float dy = Mathf.Clamp(.25f / frames, .001f, .02f) - .001f;
        transform.Translate(0, dy, 0);
    }
}

public enum PointFloaterIcon : int {
    BITS = 0,
    PUNISH = 1,
}
