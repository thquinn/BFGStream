using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Linq;

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

    SQLiteConnection saveConnection, dictionaryConnection;
    void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/save.db")) {
            saveConnection = new SQLiteConnection(string.Format("Data Source={0}/save.db;", Application.persistentDataPath));
            saveConnection.Open();
        }
        if (File.Exists(Application.persistentDataPath + "/dictionary.db")) {
            dictionaryConnection = new SQLiteConnection(string.Format("Data Source={0}/dictionary.db;", Application.persistentDataPath));
            dictionaryConnection.Open();
        }
    }

    // Save functions.
    public string[] GetOwnedWords(string username) {
        SQLiteCommand command = saveConnection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT word FROM owned_word WHERE username = '{0}'", username);
        SQLiteDataReader reader = command.ExecuteReader();
        List<string> output = new List<string>();
        while (reader.Read()) {
            output.Add(reader.GetString(0));
        }
        string[] arr = output.ToArray();
        Array.Sort(arr);
        return arr;
    }
    public void AddOwnedWord(string username, string word) {
        SQLiteCommand command = saveConnection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("INSERT INTO owned_word(username, word) SELECT '{0}', '{1}' WHERE NOT EXISTS(SELECT 1 FROM owned_word WHERE username = '{0}' AND word = '{1}')", username, word);
        command.ExecuteNonQuery();
    }
    public Dictionary<string, string> GetTitles() {
        SQLiteCommand command = saveConnection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT username, title FROM user");
        SQLiteDataReader reader = command.ExecuteReader();
        Dictionary<string, string> output = new Dictionary<string, string>();
        while (reader.Read()) {
            output.Add(reader.GetString(0), reader.GetString(1).ToUpper());
        }
        return output;
    }
    public void SetTitle(string username, string title) {
        SQLiteCommand command = saveConnection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("INSERT INTO user(username, title) VALUES('{0}', '{1}') ON CONFLICT(username) DO UPDATE SET title='{1}'", username, title.ToLower());
        command.ExecuteNonQuery();
    }
    public Dictionary<string, int> LoadScores(string month) {
        SQLiteCommand command = saveConnection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("SELECT username, score FROM monthly_score WHERE month = '{0}'", month);
        SQLiteDataReader reader = command.ExecuteReader();
        Dictionary<string, int> output = new Dictionary<string, int>();
        while (reader.Read()) {
            output.Add(reader.GetString(0), reader.GetInt32(1));
        }
        return output;
    }
    public void SaveScores(string month, Dictionary<string, int> scores) {
        if (scores.Count == 0) {
            return;
        }
        SQLiteCommand command = saveConnection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = string.Format("REPLACE INTO monthly_score(month, username, score) VALUES {1}", month, string.Join(" ", scores.Select(kvp => string.Format("('{0}', '{1}', {2})", month, kvp.Key, kvp.Value))));
        Debug.Log(command.CommandText);
        command.ExecuteNonQuery();
    }

    // Dictionary functions.
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
            while (definition.Length > 0 && !char.IsLetter(definition[0])) {
                definition = definition.Substring(1);
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
