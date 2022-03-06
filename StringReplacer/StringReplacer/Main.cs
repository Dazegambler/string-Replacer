using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        Text[] GetAllTexts() => SceneManager.GetActiveScene().GetRootGameObjects().GetAllComponentsInArray<Text>();

        public void GetText(Text[] Texts)
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
        public static T[] GetAllComponentsInArray<T>(this GameObject[] source)
        {
            List<T> a = new List<T>();

            foreach (var obj in source)
            {
                if (obj.TryGetComponent<T>(out var t)) a.Add(obj.GetComponent<T>());
                foreach (var _obj in obj.ListChildren())
                {
                    if (_obj.GetComponent<T>() != null)
                    {
                        a.Add(_obj.GetComponent<T>());
                    }
                }

            }
            if (a.ToArray() != null)
            {
                return a.ToArray();
            }
            else
            {
                Debug.LogWarning($"DazeExtensions.GetComponentsInArray:Error Returning Array");
                return new T[0];
            }
        }

        public static Transform[] ListChildren(this GameObject parent)
        {
            Transform[] a;
            a = parent.GetComponentsInChildren<Transform>(true);
            try
            {
                return a;
            }
            catch
            {
                Debug.LogWarning($"DazeExtensions.ListChildren:COULD NOT LIST CHILDREN INSIDE {parent.name}");
                return new Transform[0];
            }
        }
    }
}
