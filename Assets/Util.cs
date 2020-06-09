using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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
}
