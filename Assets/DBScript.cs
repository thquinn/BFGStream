using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;

public class DBScript : MonoBehaviour
{
    static Regex REGEX_WIKTIONARY_MARKUP = new Regex(@"^(\(.+\))|({{.+}})");
    static Dictionary<string, string> PARTS_OF_SPEECH_ABBREVS = new Dictionary<string, string>() {
        { "Proper noun", "n" },
        { "Noun", "n" },
        { "Conjunction", "conj" },
        { "Particle", "prt" },
        { "Adjective", "adj" },
        { "Pronoun", "pron" },
        { "Verb", "v" },
        { "Interjection", "interj" },
        { "Adverb", "adv" },
        { "Preposition", "prep" },
        { "Article", "art" },
        { "Determiner", "det" },
        { "Numeral", "num" },
        { "Postposition", "post" },
    };

    SQLiteConnection dictionaryConnection;
    void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/dictionary.db")) {
            dictionaryConnection = new SQLiteConnection(string.Format("Data Source={0}/dictionary.db;", Application.persistentDataPath));
            dictionaryConnection.Open();
        }
    }

    public string QueryDefinition(string word) {
        if (dictionaryConnection == null) {
            return null;
        }
        SQLiteCommand command = dictionaryConnection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT definition, lemma FROM dictionary WHERE word=\"{0}\" COLLATE NOCASE ORDER BY id LIMIT 1", word);
        SQLiteDataReader reader = command.ExecuteReader();
        while (reader.Read()) {
            string definition = REGEX_WIKTIONARY_MARKUP.Replace(reader.GetString(0), "").Trim();
            if (definition.Contains("#")) {
                definition = definition.Substring(0, definition.IndexOf('#'));
            }
            if (definition.Length == 0) {
                return null;
            }
            string partOfSpeech = PARTS_OF_SPEECH_ABBREVS[reader.GetString(1)];
            return string.Format("{0} <i>{1}.</i>\n{2}", word, partOfSpeech, definition);
        }
        return null;
    }
}
