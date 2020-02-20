using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ViewerPopupScript : MonoBehaviour
{
    TextMeshProUGUI tmp;
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector3 originalPosition;
    int timer;
    float dy;

    // Start is called before the first frame update
    void Start()
    {
        if (tmp == null) {
            tmp = GetComponent<TextMeshProUGUI>();
        }
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        originalPosition = transform.localPosition;
        transform.Translate(0, -1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer++;

        if (timer < 1200) {
            canvasGroup.alpha += .05f;
        } else {
            canvasGroup.alpha -= .05f;
            if (canvasGroup.alpha <= 0) {
                DestroyImmediate(gameObject);
                return;
            }
            dy += .001f;
            transform.Translate(0, dy, 0, Space.Self);
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, .25f);
    }

    public void Set(int player, int playerCount) {
        if (tmp == null) {
            tmp = GetComponent<TextMeshProUGUI>();
        }
        if (rectTransform == null) {
            rectTransform = (RectTransform)transform;
        }
        Vector3 localPosition = transform.localPosition;
        bool swap = (playerCount == 2 && player == 1) || player > 1;
        if (swap) {
            localPosition.x *= -1;
            tmp.alignment = TextAlignmentOptions.BottomLeft;
        }
        bool shrinkAndReposition = (playerCount == 3 && player < 2) || playerCount == 4;
        if (shrinkAndReposition) {
            int[] xOffs = new int[] { -720, -330, 255, 655 };
            localPosition.x = xOffs[player];
            rectTransform.sizeDelta = new Vector2(400, 100);
        }
        transform.localPosition = localPosition;
    }
    public void AddLine(string line) {
        if (tmp == null) {
            tmp = GetComponent<TextMeshProUGUI>();
        }
        if (tmp.text.Length != 0) {
            tmp.text += '\n';
        }
        tmp.text += line;
    }
    public void AddLine(string username, int points, int total, bool doubledUp) {
        if (tmp == null) {
            tmp = GetComponent<TextMeshProUGUI>();
        }
        if (tmp.text.Length != 0) {
            tmp.text += '\n';
        }
        tmp.text += string.Format("{0} +{1} pts{2} <size=75%><voffset=0.15em>►</voffset></size> {3}", username, points, doubledUp ? " (x2!)" : "", total);
    }
    public void ForceDestroy() {
        timer = 1200;
    }
}
