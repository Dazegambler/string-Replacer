using BepInEx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using StringReplacer;

namespace StringReplacer.dev
{
    [BepInPlugin("TXTSWP", "TextSweep", "1.0.0")]
    public class DevTools : BaseUnityPlugin
    {
        public static void GetText(Text[] Texts)
        {
            var _data = new List<data>();
            var objs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var text in Texts) NewData(text, _data);
            using (var file = File.CreateText($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\UltraText\{SceneManager.GetActiveScene().name}.json"))
            {
                JsonSerializer serial = new JsonSerializer();
                serial.Serialize(file, _data);
            }
        }

        static void NewData(Text textobj,List<data>_data) => _data.Add(new data()
        {
            Original = textobj.text,
            Replacement = textobj.text,
        });
    }
}
