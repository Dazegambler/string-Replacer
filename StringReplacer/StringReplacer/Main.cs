using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using StringReplacer.dev;
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
        ConfigEntry<bool> DevMode;

        List<data> Loaded = new List<data>();

        List<String> Originals = new List<string>(),
            Replacements = new List<string>();

        void Start()
        {
            DevMode = Config.Bind("Dev", "Dev Mode", false);


            var Path = Directory.GetCurrentDirectory() + @"\TextDatabases";
            if (File.Exists(Path))
            {
                var Files = Directory.GetFiles(Path, "*.json");
                LoadTextFiles(Files);
            }
            else File.Create(Path);

            foreach (var data in Loaded)
            {
                Originals.Add(data.Original);
                Replacements.Add(data.Replacement);
            }
        }

        void Update()
        {
            if (DevMode.Value == true)
            {
                if (Input.GetKeyDown(KeyCode.N)) DevTools.GetText(GetAllTexts());
            }
            else
            {
                foreach (var text in GetAllTexts())
                {
                    if (Originals.Contains(text.text)) text.text = Replacements[Originals.IndexOf(text.text)];
                }
            }
        }

        void LoadTextFiles(string[] Files)
        {
            foreach (var file in Files)
            {
                StreamReader reader = new StreamReader(file);
                string json = reader.ReadToEnd();
                var _data = JsonConvert.DeserializeObject<data>(json);
                Loaded.Add(_data);
            }
        }

        Text[] GetAllTexts() => SceneManager.GetActiveScene().GetRootGameObjects().GetAllComponentsInArray<Text>();
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
