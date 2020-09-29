using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastsScript : MonoBehaviour
{
    static int TIMER_FRAMES = 175;

    public GameObject toastPrefab;
    public GameScript gameScript;
    public Sprite[] toastTypeIcons;
    public AudioSource sfxToast;

    Queue<ToastStruct> queue;
    List<GameObject> toasts;
    List<SpriteRenderer> spriteRenderers;
    Dictionary<int, int> framesLeft;
    int timer;

    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<ToastStruct>();
        toasts = new List<GameObject>();
        spriteRenderers = new List<SpriteRenderer>();
        framesLeft = new Dictionary<int, int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0) {
            timer--;
        } else {
            Dequeue();
        }

        if (toasts.Count == 0) {
            return;
        }
        // Note: this can cause very tall toasts to despawn as soon as they're instantiated.
        float y = (spriteRenderers[0].size.y - 1) * 25f;
        int deadIndex = -1;
        for (int i = 0; i < toasts.Count; i++) {
            framesLeft[toasts[i].GetInstanceID()]--;
            if (y > 350 || framesLeft[toasts[i].GetInstanceID()] <= 0) {
                Vector3 target = toasts[i].transform.localPosition;
                target.x = -500;
                toasts[i].transform.localPosition = Vector3.Lerp(toasts[i].transform.localPosition, target, .2f);
                if (toasts[i].transform.localPosition.x < -400) {
                    DestroyImmediate(toasts[i]);
                    framesLeft.Remove(toasts[i].GetInstanceID());
                    if (deadIndex == -1) {
                        deadIndex = i;
                    }
                }
                continue;
            }
            toasts[i].transform.localPosition = Vector3.Lerp(toasts[i].transform.localPosition, new Vector3(0, y, 0), .2f);
            if (i < toasts.Count - 1) {
                y += spriteRenderers[i].size.y * 25f;
                y += spriteRenderers[i + 1].size.y * 25f;
                y += 5;
            }
        }
        if (deadIndex != -1) {
            toasts.RemoveRange(deadIndex, toasts.Count - deadIndex);
            spriteRenderers.RemoveRange(deadIndex, spriteRenderers.Count - deadIndex);
        }
    }

    public void Toast(ToastType type, string message) {
        queue.Enqueue(new ToastStruct(type, message));
    }
    void Dequeue() {
        if (queue.Count == 0) {
            return;
        }
        if (gameScript.players.Count > 3) {
            // In 4FG, the leftmost player's viewer popup can block the view of toasts. If it has two or more lines,
            // wait until it goes away to start dequeueing toasts again.
            GameObject viewerPopup = GameObject.FindGameObjectWithTag("ViewerPopup");
            if (viewerPopup != null && viewerPopup.GetComponent<TextMeshProUGUI>().text.Split('\n').Length > 1) {
                return;
            }
        }
        timer = TIMER_FRAMES;

        ToastStruct toastStruct = queue.Dequeue();
        GameObject toast = Instantiate(toastPrefab, transform);
        toasts.Insert(0, toast);
        SpriteRenderer spriteRenderer = toast.transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderers.Insert(0, spriteRenderer);

        TextMeshProUGUI tmp = toast.GetComponentInChildren<TextMeshProUGUI>();
        tmp.SetText(toastStruct.message);
        tmp.ForceMeshUpdate();
        int lines = Mathf.RoundToInt(tmp.preferredHeight / 44.64f);
        float tmpWidth = tmp.textBounds.extents.x * 2;
        float borderWidth = Mathf.Lerp(1, 7, tmpWidth / 600); // text width maxes out at 600.
        spriteRenderer.size = new Vector2(borderWidth, .5f + lines * .5f);
        spriteRenderer.gameObject.transform.localPosition = new Vector3(-(7 - borderWidth) / 2 + .05f, 0, 0);
        toast.transform.localPosition = new Vector3(-400, (spriteRenderer.size.y - 1) * 25f);
        framesLeft[toast.GetInstanceID()] = 900;

        Image image = toast.GetComponentInChildren<Image>();
        image.sprite = toastTypeIcons[(int)toastStruct.type];

        sfxToast.PlayOneShot(sfxToast.clip);
    }
}

struct ToastStruct {
    public ToastType type;
    public string message;

    public ToastStruct(ToastType type, string message) {
        this.type = type;
        this.message = message;
    }
}

public enum ToastType : int {
    AWARD = 0,
    FIRST_MATCH = 1,
    FOLLOW = 2,
    HOST = 3,
    NEW_PLAYER = 4,
    STREAK = 5,
    STREAK_BROKEN = 6,
    TITLE = 7,
    ALL_MISS = 8,
    BITS = 9,
    SUB = 10,
    DOUBLE_UP = 11,
    PUNISH = 12,
    GIFT = 13,
    DICTIONARY = 14,
    ADDITIONAL_WORD = 15,
    DOUBLE_LIFE = 16,
    CALL_SHOT = 17,
    CALL_SHOT_MISSED = 18,
}