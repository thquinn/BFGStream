using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class Util {
    public static float EaseInOutQuad(float t) {
        t *= 2;
        if (t < 1)
            return .5f * t * t;
        t--;
        return -.5f * (t * (t - 2) - 1);
    }

    public static void Shuffle<T>(IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static string JoinGrammatically(string[] strings) {
        if (strings.Length > 1) {
            strings[strings.Length - 1] = "and " + strings[strings.Length - 1];
        }
        return string.Join(strings.Length < 3 ? " " : ", ", strings);
    }
    public static string SanitizeWord(string word) {
        word = Util.RemoveDiacritics(word.ToLower());
        return GameScript.WORD_REGEX.Replace(word, "");
    }
    public static string RemoveDiacritics(string text) {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        text = text.Normalize(NormalizationForm.FormD);
        var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
        return new string(chars).Normalize(NormalizationForm.FormC);
    }
    public static string CanonicalizeViewer(string viewer) {
        return viewer.EndsWith("!") ? viewer.Substring(0, viewer.Length - 1) : viewer;
    }

    public static int GetIndexOfShortestEditDistance(string input, string[] arr) {
        int[] distances = arr.Select(s => DamerauLevenshteinDistance(s, input)).ToArray();
        int minDistance = distances.Min();
        int[] minIndices = distances.Select((b, i) => b == minDistance ? i : -1).Where(i => i != -1).ToArray();
        return minIndices[Random.Range(0, minIndices.Length)];
    }
    public static int DamerauLevenshteinDistance(string string1, string string2) {
        if (string.IsNullOrEmpty(string1)) {
            if (!string.IsNullOrEmpty(string2))
                return string2.Length;

            return 0;
        }

        if (string.IsNullOrEmpty(string2)) {
            if (!string.IsNullOrEmpty(string1))
                return string1.Length;

            return 0;
        }

        int length1 = string1.Length;
        int length2 = string2.Length;

        int[,] d = new int[length1 + 1, length2 + 1];

        int cost, del, ins, sub;

        for (int i = 0; i <= d.GetUpperBound(0); i++)
            d[i, 0] = i;

        for (int i = 0; i <= d.GetUpperBound(1); i++)
            d[0, i] = i;

        for (int i = 1; i <= d.GetUpperBound(0); i++) {
            for (int j = 1; j <= d.GetUpperBound(1); j++) {
                if (string1[i - 1] == string2[j - 1])
                    cost = 0;
                else
                    cost = 1;

                del = d[i - 1, j] + 1;
                ins = d[i, j - 1] + 1;
                sub = d[i - 1, j - 1] + cost;

                d[i, j] = Mathf.Min(del, Mathf.Min(ins, sub));

                if (i > 1 && j > 1 && string1[i - 1] == string2[j - 2] && string1[i - 2] == string2[j - 1])
                    d[i, j] = Mathf.Min(d[i, j], d[i - 2, j - 2] + cost);
            }
        }

        return d[d.GetUpperBound(0), d.GetUpperBound(1)];
    }
}
