using System.Collections.Generic;
using System.IO;
using Gs2.Util.LitJson;
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Core.Editor
{
    public class Manifest
    {
        public string package;
        public string name;
        public string author;
        public string version;
        public Dictionary<string, string> dependencies;
        public List<string> templates;

        public string basePath;

        public static Manifest Load(ScriptableObject clazz)
        {
            var currentCodePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(clazz));
            var currentCodeDirectory = currentCodePath.Substring(0, currentCodePath.LastIndexOf('/'));
            var manifestFile = new StreamReader (currentCodeDirectory + "/../gs2.json", System.Text.Encoding.UTF8);
            var manifest =  JsonMapper.ToObject<Manifest>(manifestFile);
            manifest.basePath = currentCodeDirectory + "/../";
            return manifest;
        }
    }
}