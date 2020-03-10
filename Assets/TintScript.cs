using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TintScript : MonoBehaviour
{
    static Tuple<Color, Color> COLORS_LIGHT = new Tuple<Color, Color>(new Color(0.8705882f, 0.8313726f, 0.9137255f), new Color(0.9215686f, 0.8352941f, 0.8352941f)),
                               COLORS_MID = new Tuple<Color, Color>(new Color(0.5686275f, 0.4705882f, 0.7176471f), new Color(0.7215686f, 0.4745098f, 0.4745098f)),
                               COLORS_DARK = new Tuple<Color, Color>(new Color(0.3921569f, 0.2666667f, 0.6078432f), new Color(0.6698113f, 0.1548149f, 0.1548149f)),
                               COLORS_LOGO = new Tuple<Color, Color>(new Color(0.8077272f, 0.7134212f, 0.9056604f), new Color(0.9098039f, 0.7176471f, 0.7176471f));
    static float SPEED = .01f;

    public GameScript gameScript;
    public WipeScript wipeScript;

    public ParticleSystem[] gradientParticles, darkParticles;
    public Image[] lightImages, midImages, darkImages;
    public SpriteRenderer[] lightRenderers, midRenderers, darkRenderers;
    public TextMeshProUGUI[] texts;
    public Material logoMaterial, outlineTextMaterial;
    public GameObject leaderboardRowAnchor, toastAnchor, viewerPopupAnchor, pointFloaterAnchor;
    public GameObject leaderboardRowPrefab, toastPrefab, viewerPopupPrefab, newestWordPrefab, pointFloaterPrefab;

    public float target = 0;
    float t = 0;

    void Start() {
        ResetAssets();
    }
    void Update()
    {
        target = (gameScript.lightningRound && gameScript.words.Count > 0) ? 1 : 0;
        float modifiedTarget = wipeScript.on ? 0 : target;
        if (t == modifiedTarget) {
            return;
        }
        float modifiedSpeed = wipeScript.on ? SPEED * 3 : SPEED;
        float delta = t > modifiedTarget ? -modifiedSpeed : modifiedSpeed;
        if (Mathf.Abs(t - modifiedTarget) <= delta) {
            t = modifiedTarget;
        } else {
            t += delta;
        }
        Color colorLight = Color.Lerp(COLORS_LIGHT.Item1, COLORS_LIGHT.Item2, t);
        Color colorMid = Color.Lerp(COLORS_MID.Item1, COLORS_MID.Item2, t);
        Color colorDark = Color.Lerp(COLORS_DARK.Item1, COLORS_DARK.Item2, t);
        Color colorLogo = Color.Lerp(COLORS_LOGO.Item1, COLORS_LOGO.Item2, t);
        foreach (ParticleSystem particle in gradientParticles) {
            var main = particle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(colorLight, colorDark);
        }
        foreach (ParticleSystem particle in darkParticles) {
            var main = particle.main;
            main.startColor = colorDark;
        }
        foreach (Image image in lightImages) {
            image.color = colorLight;
        }
        foreach (Image image in midImages) {
            image.color = colorMid;
        }
        foreach (Image image in darkImages) {
            image.color = colorDark;
        }
        foreach (SpriteRenderer renderer in lightRenderers) {
            renderer.color = colorLight;
        }
        foreach (SpriteRenderer renderer in midRenderers) {
            renderer.color = colorMid;
        }
        foreach (SpriteRenderer renderer in darkRenderers) {
            renderer.color = colorDark;
        }
        foreach (TextMeshProUGUI text in texts) {
            text.color = colorLight;
        }
        logoMaterial.color = colorLogo;
        outlineTextMaterial.SetColor("_OutlineColor", colorDark);
        // Grab prefab instances: pointfloaters
        // Leaderboard rows.
        leaderboardRowPrefab.transform.GetChild(1).GetComponent<Image>().color = colorMid;
        LeaderboardRowScript leaderboardPrefabScript = leaderboardRowPrefab.GetComponent<LeaderboardRowScript>();
        leaderboardPrefabScript.tmpName.color = colorLight;
        leaderboardPrefabScript.tmpTitle.color = colorLight;
        foreach (Transform child in leaderboardRowAnchor.transform) {
            child.GetChild(1).GetComponent<Image>().color = colorMid;
            LeaderboardRowScript script = child.GetComponent<LeaderboardRowScript>();
            script.tmpName.color = colorLight;
            script.tmpTitle.color = colorLight;
        }
        // Toasts.
        colorDark.a = 0.6431373f;
        toastPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colorDark;
        toastPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = colorLight;
        toastPrefab.transform.GetChild(2).GetComponent<Image>().color = colorLight;
        foreach (Transform child in toastAnchor.transform) {
            child.GetChild(0).GetComponent<SpriteRenderer>().color = colorDark;
            child.GetChild(1).GetComponent<TextMeshProUGUI>().color = colorLight;
            child.GetChild(2).GetComponent<Image>().color = colorLight;
        }
        colorDark.a = 1;
        // Viewer popups.
        viewerPopupPrefab.GetComponent<TextMeshProUGUI>().color = colorLight;
        foreach (Transform child in viewerPopupAnchor.transform) {
            child.GetComponent<TextMeshProUGUI>().color = colorLight;
        }
        // Newest words.
        newestWordPrefab.GetComponent<TextMeshProUGUI>().color = colorLight;
        foreach (GameObject newestWord in GameObject.FindGameObjectsWithTag("NewestWord")) {
            newestWord.GetComponent<TextMeshProUGUI>().color = colorLight;
        }
        // Point floaters.
        pointFloaterPrefab.GetComponent<TextMeshProUGUI>().color = colorLight;
        foreach (Transform child in pointFloaterAnchor.transform) {
            child.GetComponent<TextMeshProUGUI>().color = colorLight;
        }
    }
    void OnApplicationQuit() {
        ResetAssets();
    }
    void ResetAssets() {
        logoMaterial.color = COLORS_LOGO.Item1;
        outlineTextMaterial.SetColor("_OutlineColor", COLORS_DARK.Item1);
        leaderboardRowPrefab.transform.GetChild(1).GetComponent<Image>().color = COLORS_MID.Item1;
        LeaderboardRowScript leaderboardPrefabScript = leaderboardRowPrefab.GetComponent<LeaderboardRowScript>();
        leaderboardPrefabScript.tmpName.color = COLORS_LIGHT.Item1;
        leaderboardPrefabScript.tmpTitle.color = COLORS_LIGHT.Item1;
        Color toastColor = COLORS_DARK.Item1;
        toastColor.a = 0.6431373f;
        toastPrefab.transform.GetChild(0).GetComponent<SpriteRenderer>().color = toastColor;
        toastPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = COLORS_LIGHT.Item1;
        toastPrefab.transform.GetChild(2).GetComponent<Image>().color = COLORS_LIGHT.Item1;
        viewerPopupPrefab.GetComponent<TextMeshProUGUI>().color = COLORS_LIGHT.Item1;
        newestWordPrefab.GetComponent<TextMeshProUGUI>().color = COLORS_LIGHT.Item1;
        pointFloaterPrefab.GetComponent<TextMeshProUGUI>().color = COLORS_LIGHT.Item1;
    }
}
