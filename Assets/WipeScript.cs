using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WipeScript : MonoBehaviour
{
    readonly float LERP_SPEED = .08f;

    public GameObject floaterPrefab;
    public GameObject wipe, logo, title, floaterRoot;
    public TextMeshProUGUI totalTMP, namesTMP, scoresTMP;
    public RectMask2D mask;
    public CanvasGroup leaderboardGroup, floaterGroup;

    public bool on;
    Vector3 initialWipePosition, initialLogoPosition, targetLogoPosition, initialTitlePosition, targetTitlePosition;
    int frames;

    // Start is called before the first frame update
    void Start()
    {
        initialWipePosition = wipe.transform.localPosition;
        initialLogoPosition = logo.transform.localPosition;
        targetLogoPosition = initialLogoPosition;
        targetLogoPosition.x = -440;
        initialTitlePosition = title.transform.localPosition;
        targetTitlePosition = initialTitlePosition;
        targetTitlePosition.x = 150;
    }

    // Update is called once per frame
    void Update() {
        wipe.transform.localPosition = Vector3.Lerp(wipe.transform.localPosition, on ? Vector3.zero : initialWipePosition, LERP_SPEED);
        logo.transform.localPosition = Vector3.Lerp(logo.transform.localPosition, on ? targetLogoPosition : initialLogoPosition, LERP_SPEED);
        title.transform.localPosition = Vector3.Lerp(title.transform.localPosition, on ? targetTitlePosition : initialTitlePosition, LERP_SPEED);
        Vector2 maskSize = mask.rectTransform.sizeDelta;
        maskSize.x = Mathf.Lerp(maskSize.x, on ? 1200 : 0, LERP_SPEED);
        mask.rectTransform.sizeDelta = maskSize;

        if (on) {
            frames++;
            if (frames > 20) {
                floaterGroup.alpha = Mathf.Min(1, floaterGroup.alpha + .01f);
            }
            if (frames > 60) {
                leaderboardGroup.alpha = Mathf.Min(1, leaderboardGroup.alpha + .01f);
            }
            if (frames % 30 == 0) {
                Instantiate(floaterPrefab, floaterRoot.transform).GetComponent<FloaterScript>().wipeScript = this;
            }
        } else {
            floaterGroup.alpha = Mathf.Max(0, floaterGroup.alpha - .167f);
            leaderboardGroup.alpha = Mathf.Max(0, leaderboardGroup.alpha - .167f);
        }
    }

    public void Toggle (RollingScores scores) {
        on = !on;
        frames = 0;
        if (on) {
            Dictionary<string, int> totalScores;
            if (Application.isEditor) {
                // A set of example scores for testing purposes.
                totalScores = new Dictionary<string, int>() {
                    { "caeonosphere", 1050060 },
                    { "wurstwurstwurstwurst", 180550 },
                    { "cragsquad", 180535 },
                    { "jjiaa", 171169 },
                    { "supersonik319", 130005 },
                    { "syathenurin", 102220 },
                    { "anniexchen", 80333 },
                    { "korenji321", 79800 },
                    { "swarrizard", 55055 },
                    { "droolychen", Random.Range(1000, 40000) },
                    { "this_is_a_bug_if_visible", 666 },
                };
            } else {
                totalScores = scores.GetTotalScores();
            }
            var ordered = totalScores.OrderByDescending(kvp => kvp.Value).ToArray();
            StringBuilder namesSB = new StringBuilder(), scoresSB = new StringBuilder();
            namesSB.Append("<size=45pt>");
            scoresSB.Append("<size=45pt>");
            for (int i = 0; i < ordered.Length && i < 10; i++) {
                if (i == 5) {
                    namesSB.Append("<size=32pt><alpha=#A0>");
                    scoresSB.Append("<size=32pt><alpha=#A0>");
                }
                namesSB.AppendLine(ordered[i].Key);
                scoresSB.AppendLine(ordered[i].Value.ToString("N0"));
            }
            totalTMP.text = string.Format("Total Viewer Score: {0}", totalScores.Values.Sum().ToString("N0"));
            namesTMP.text = namesSB.ToString();
            scoresTMP.text = scoresSB.ToString();
        }
    }
}
