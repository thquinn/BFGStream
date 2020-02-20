using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRowScript : MonoBehaviour
{
    public Sprite[] TROPHY_SPRITES;
    public GameObject trophy, glint;
    public TextMeshProUGUI tmpName, tmpTitle;
    public ParticleSystem particles;

    Image trophyImage;
    CanvasGroup canvasGroup;
    BotScript botScript;
    bool first = true;
    bool destroying = false;
    float targetX, targetY, dy, rotation;
    int currentTrophy, targetTrophy, frames;
    public bool sub;
    Color viewerNameColor;

    void Start() {
        canvasGroup = GetComponent<CanvasGroup>();
        viewerNameColor = tmpName.color;
    }

    void Update() {
        frames++;
        if (destroying) {
            dy -= .005f;
            canvasGroup.alpha -= .05f;
            if (canvasGroup.alpha <= 0) {
                DestroyImmediate(gameObject);
                return;
            }
        } else {
            float x = Mathf.Lerp(transform.localPosition.x, targetX, .25f);
            float y = Mathf.Lerp(transform.localPosition.y, targetY, .25f);
            transform.localPosition = new Vector3(x, y, 0);
            canvasGroup.alpha += .05f;
        }

        if (sub) {
            viewerNameColor = Color.Lerp(viewerNameColor, GameScript.SUB_COLOR, .05f);
        }

        if (currentTrophy != targetTrophy || rotation > 0) {
            bool halfway = rotation <= .5f;
            rotation = Mathf.Min(rotation + .05f, 1);
            if (!halfway && rotation > .5f) {
                trophyImage.sprite = TROPHY_SPRITES[targetTrophy];
                currentTrophy = targetTrophy;
                frames = -20;
            }
            float degrees = Util.EaseInOutQuad(rotation) * 180;
            if (degrees > 90) {
                degrees -= 180;
            }
            trophy.transform.rotation = Quaternion.Euler(0, degrees, 0);
            if (rotation == 1) {
                rotation = 0;
            }
        }
        if (GameScript.viewerTitles.ContainsKey(name)) {
            string title = '"' + GameScript.viewerTitles[name] + '"';
            if (tmpTitle.text != title) {
                tmpTitle.text = title;
            }
        }

        int framesMod = frames % (60 * 11);
        float glintX;
        if (currentTrophy != 0 || framesMod > 60) {
            glintX = -4f;
        } else {
            glintX = -3f + Util.EaseInOutQuad(Util.EaseInOutQuad(framesMod / 60f)) * 6;
        }
        glint.transform.localPosition = new Vector3(glintX, 0, 0);

        transform.Translate(0, dy, 0);
    }

    public void Set(string player, int place, int index, int points, bool isSubscriber) {
        if (first) {
            trophyImage = trophy.GetComponent<Image>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            currentTrophy = place;
            trophyImage.sprite = TROPHY_SPRITES[place];
        }
        targetTrophy = place;
        name = player;
        tmpName.text = "<color=#" + ColorUtility.ToHtmlStringRGB(viewerNameColor) + ">" + player + "</color> - " + points + " pts";
        if (place > 0) {
            particles.Stop();
        } else if (!particles.isPlaying) {
            particles.Play();
        }
        if (botScript == null) {
            botScript = GameObject.FindGameObjectWithTag("BotScript").GetComponent<BotScript>();
        }
        // Set is actually getting called every frame on every leaderboard row, so this is pretty jank.
        if (isSubscriber) {
            sub = true;
        }
        targetX = (index % 2 == 0) ? 0 : -0.8f;
        targetY = -1.05f * index;
        
        if (first) {
            transform.localPosition = new Vector3(targetX + 1, targetY);
        }
        first = false;
        destroying = false;
    }
    public void ProvisionalDestroy() {
        destroying = true;
    }
}
