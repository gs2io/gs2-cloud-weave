﻿﻿﻿/*
 * Copyright 2016 Game Server Services, Inc. or its affiliates. All Rights
 * Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gs2.Weave.EditorExtension.Editor.Model;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Gs2.Weave.EditorExtension.Editor
{
    enum Status
    {
        GetPackages,
        Present,
        Installing,
    }

    public class CloudWeaveWindow : EditorWindow
    {
        private Status _status = Status.GetPackages;
        private List<Package> _packages;
        private List<Package> _filteredPackages;
        private string _keyword;
        private Vector2 _scrollPosition = Vector2.zero;

        [MenuItem ("Game Server Services/CloudWeave")]
        public static void Open ()
        {
            GetWindow<CloudWeaveWindow>(false, "CloudWeave");
        }

        private void OnEnable()
        {
            if (_status == Status.GetPackages)
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(
                    RepositoryClient.List(
                        packages =>
                        {
                            _filteredPackages = _packages = packages.Where(
                                package => package.name != "io.gs2.unity.sdk" &&
                                           package.name != "io.gs2.unity.weave.editor-extension"
                            ).ToList();
                            
                            _status = Status.Present;
                        }
                    )
                );
            }

            if (_status == Status.Installing)
            {
                CheckInstall();
            }
        }

        void OnGUI() 
        {
            if (_status == Status.GetPackages)
            {
                GUILayout.Label("パッケージを取得中...");
                Repaint();
                return;
            }

            using (var scrollScope = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = scrollScope.scrollPosition;
                
                if (_packages != null)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUI.SetNextControlName("filterField");
                        _keyword = GUILayout.TextField(_keyword, "SearchTextField");
                        if (GUI.changed)
                        {
                            _filteredPackages = _packages.Where(package => package.IsIncludeKeyWord(_keyword)).ToList();
                        }
                        GUI.FocusControl("filterField");
                        GUI.enabled = !string.IsNullOrEmpty(_keyword);
                        if (GUILayout.Button("Clear", "SearchCancelButton"))
                        {
                            _keyword = string.Empty;
                        }

                        GUI.enabled = true;
                    }

                    var style = new GUIStyle(GUI.skin.label) {wordWrap = true};
                    foreach (var package in _filteredPackages)
                    {
                        using (new GUILayout.VerticalScope(GUI.skin.box))
                        {
                            GUILayout.Label(package.displayName, style);
                            GUILayout.Label(package.description, style);
                            var author = package.author != null ? package.author.name : "unknown";
                            GUILayout.Label($"Author: {author}", style);
                            DrawInstaller(package.name);
                        }

                        GUILayout.Label("");
                    }
                }
            }
        }

        private async void DrawInstaller(string packageUrl)
        {
            if (Application.isPlaying)
            {
                if (GUILayout.Button("プレイモード中は CloudWeave を使用できません。"))
                {
                }
                return;
            }
            switch (_status)
            {
                case Status.Present:
                    if (GUILayout.Button("インストール"))
                    {
                        _status = Status.Installing;
                        PlayerPrefs.SetString("io.gs2.weave.installing", packageUrl);
                        PlayerPrefs.Save();

                        {
                            var request = Client.Add(packageUrl);
                            while (request.Status == StatusCode.InProgress)
                            {
                                await Task.Delay(1000);
                            }
                        }

                        CheckInstall();
                    }
                    break;
                case Status.Installing:
                    GUILayout.Button("インストール中… しばらくお待ちください。");
                    break;
            }
        }

        private async void CheckInstall()
        {
            var installingPackage = PlayerPrefs.GetString("io.gs2.weave.installing", null);
            if (!string.IsNullOrEmpty(installingPackage))
            {
                var package = _packages.First(item => item.name == installingPackage);

                var request = Client.List();
                while (request.Status == StatusCode.InProgress)
                {
                    await Task.Delay(1000);
                }

                if (request.Result.Count(item => item.name == package.name) > 0)
                {
                    if (package.sampleScene != null)
                    {
                        try
                        {
                            var scenePath = AssetDatabase
                                .FindAssets("t:SceneAsset")
                                .Select(AssetDatabase.GUIDToAssetPath)
                                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset)))
                                .Where(obj => obj != null)
                                .Select(obj => (SceneAsset) obj)
                                .Where(scene => scene.name == package.sampleScene)
                                .Select(AssetDatabase.GetAssetPath)
                                .First(path => path.StartsWith("Packages/" + package.name));

                            AssetDatabase.CopyAsset(scenePath, "Assets/Scenes/" + package.sampleScene + ".unity");

                            EditorSceneManager.OpenScene("Assets/Scenes/" + package.sampleScene + ".unity");
                        } catch (InvalidOperationException) {}
                    }

                    if (package.tutorialWindowClassPath != null)
                    {
                        var tutorialWindowType = Type.GetType(package.tutorialWindowClassPath);
                        if (tutorialWindowType != null)
                        {
                            GetWindowWithRect(tutorialWindowType, new Rect(0, 0, 700, 700), true, "チュートリアル");
                        }
                    }

                    Repaint();

                    PlayerPrefs.SetString("io.gs2.weave.installing", null);
                    PlayerPrefs.Save();

                    _status = Status.Present;
                }
            }
        }
    }
}