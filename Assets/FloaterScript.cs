using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloaterScript : MonoBehaviour
{
    public WipeScript wipeScript;
    public RectTransform rectTransform;
    public Image image;

    float dx, dy;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform.localPosition = new Vector3(Random.Range(-1200, 1200), -800, 0);
        float scale = Random.Range(200, 500);
        rectTransform.sizeDelta = new Vector2(scale, scale);
        Color color = image.color;
        color.a = Random.Range(.05f, .075f);
        image.color = color;
        dx = Random.Range(-.002f, .002f);
        dy = Random.Range(.015f, .0075f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!wipeScript.on) {
            return;
        }
        rectTransform.Translate(dx, dy, 0);
        if (rectTransform.localPosition.y > 800) {
            Destroy(gameObject);
        }
    }
}
