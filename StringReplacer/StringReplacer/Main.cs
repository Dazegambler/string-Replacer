using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StringReplacer
{
    [BepInPlugin("String.Replacer", "String Replacer", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        public Dictionary<string, string> Loaded = new Dictionary<string, string>();

        public void Start()
        {
            Debug.LogWarning("Reading Text Databases");
            var Path = Directory.GetCurrentDirectory() + @"\TextDatabases";
            if (Directory.Exists(Path))
            {
                var Files = Directory.GetFiles(Path, "*.json");
                foreach (var file in Files)
                {
                    Debug.Log($"FOUND:{file}");
                    LoadTextFiles(file);
                }
                Debug.LogWarning($"Finished Reading Text Databases...Found:{Files.Length}");
            }
            else
            {
                Debug.LogWarning("TextDatabases File not found creating file");
                Directory.CreateDirectory(Path);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                GetText(GetAllTexts());
            }
            foreach (var text in FindObjectsOfType<Text>())
            {
                ReplaceString(text);
            }
        }

        bool ReplaceString(Text target)
        {
            if (Loaded.ContainsKey(target.text))
            {
                //Debug.Log(("Replaced \"", target.text, "\" with \"", (string)Loaded[target.text], '\"'));
                target.text = Loaded[target.text];
                return true;
            }
            else
            {
                //Debug.Log(("Failed to replace \"", target.text, '\"'));
                return false;
            }
        }

        bool Exists(string text,List<data> Loaded)
        {
            foreach(var loads in Loaded)
            {
                if (loads.Original == text)
                {
                    return true;
                }
            }
            return false;
        }

        void LoadTextFiles(string file)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                string json = reader.ReadToEnd();
                var _data = JsonConvert.DeserializeObject<List<data>>(json);
                foreach (var data in _data)
                {
                    if (data.Original != data.Replacement)
                        if (!Loaded.ContainsKey(data.Original))
                            Loaded.Add(data.Original, data.Replacement);
                        else
                            Debug.Log(("Error: string \"", data.Original, "\" appears more than one time"));
                    else
                        Debug.Log(("String \"", data.Original, "\" got no replacements"));

                }
            }
        }

        List<Text> GetAllTexts() => SceneManager.GetActiveScene().GetRootGameObjects().GetAllComponentsInArray<Text>();

        public void GetText(List<Text> Texts)
        {
            var _data = new List<data>();
            foreach (var text in Texts)
            {
                if(Exists(text.text,_data)== false)
                {
                    NewData(text, _data);
                }
            }
            using (var file = File.CreateText($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\{SceneManager.GetActiveScene().name}.json"))
            {
                string json = JsonConvert.SerializeObject(_data.ToArray(), Formatting.Indented);
                file.Write(json);
            }
            Debug.Log("All Strings have been Logged");
        }

        private static string format_json(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        void NewData(Text textobj, List<data> _data) => _data.Add(new data()
        {
            Original = textobj.text,
            Replacement = textobj.text,
        });
    }
    public static class Extensions
    {
        public static List<T> GetAllComponentsInArray<T>(this GameObject[] source)
        {
            List<T> a = new List<T>();

            source.ToList().ForEach(gobject =>
            {
                if (gobject.TryGetComponent<T>(out var s))
                {
                    a.Add(gobject.GetComponent<T>());
                }
                a.AddRange(gobject.ListChildren().ToArray().GetAllComponentsInArray<T>());
            });
            return a;
        }

        public static List<GameObject> ListChildren(this GameObject parent)
        {
            List<GameObject> a = new List<GameObject>();
            for(int i = 0; i < parent.transform.childCount; i++)
            {
                a.Add(parent.transform.GetChild(i).gameObject);
            }
            return a;
        }
    }
}
