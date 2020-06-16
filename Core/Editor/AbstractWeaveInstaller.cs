using System.Collections.Generic;
using Gs2.Core.Util;
using Gs2.Editor.Core;
using Gs2.Editor.Project;
using Gs2.Gs2Deploy.Model;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Event = Gs2.Gs2Deploy.Model.Event;

namespace Gs2.Weave.Core.Editor
{
    public abstract class AbstractWeaveInstaller: UnityEditor.Editor
    {
        private Manifest _manifest;
        private WeaveInstaller.PostProcess _postProcess;
        
        private bool _installing;
        private bool _updating;
        private bool _uninstalling;

        private string _status;

        protected List<Event> events;
        protected List<Output> outputs;

        public override void OnInspectorGUI()
    {
            if (Context.ProjectToken == null)
            {
                GUILayout.Label("サインインしてプロジェクトを選択してください。");
                if (GUILayout.Button("サインイン"))
                {
                    EditorWindow.GetWindow(typeof(SigninWindow), true, "Sign-in to GS2").Show();
                }

                return;
            }

            if (_manifest == null)
            {
                _manifest = Manifest.Load(this);
            }

            if (_installing || _updating || _uninstalling)
            {
                if (_installing)
                {
                    EditorGUILayout.LabelField("インストール中...");
                }
                else if (_updating)
                {
                    EditorGUILayout.LabelField("アップデート中...");
                }
                else if (_uninstalling)
                {
                    EditorGUILayout.LabelField("アンインストール中...");
                }

                if (GUILayout.Button("Back"))
                {
                    _installing = false;
                    _updating = false;
                    _uninstalling = false;
                    _status = null;
                    Repaint();
                }

                using (new GUILayout.VerticalScope(GUI.skin.box))
                {
                    if (events != null)
                    {
                        foreach (var e in events)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                if (e.eventAt != null)
                                    GUILayout.Label(UnixTime.FromUnixTime(e.eventAt.Value).ToShortTimeString());
                                GUILayout.Label(e.resourceName);
                                GUILayout.Label(e.type);
                                GUILayout.Label(e.message);
                            }
                        }
                    }
                }

                return;
            }

            if (_postProcess == null)
            {
                _postProcess = PostProcess();
            }

            if (_status == null)
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(
                    WeaveInstaller.GetStatus(
                        _manifest,
                        r =>
                        {
                            _status = r.Result.status;
                            Repaint();
                        }
                    )
                );
            }
            else if (_status == "CREATE_COMPLETE" || _status == "UPDATE_COMPLETE")
            {
                if (GUILayout.Button("アンインストール"))
                {
                    EditorCoroutineUtility.StartCoroutineOwnerless(
                        WeaveInstaller.Uninstall(
                            _manifest,
                            e =>
                            {
                                Repaint();
                                events = e;
                            },
                            () =>
                            {
                                Repaint();
                                _uninstalling = false;
                                _status = null;
                            }
                        )
                    );
                    _uninstalling = true;
                    Repaint();
                }
                
                EditorGUILayout.LabelField("");

                if (GUILayout.Button("設定値をサーバから取得"))
                {      
                    EditorCoroutineUtility.StartCoroutineOwnerless(
                        WeaveInstaller.GetOutputs(
                            _manifest,
                            r =>
                            {
                                if (r.Error != null)
                                {
                                    EditorUtility.DisplayDialog("Error", r.Error.Message, "OK");
                                }
                                else
                                {
                                    Repaint();
                                    outputs = r.Result.items;
                                }
                            }
                        )
                    );
                }

                if (_postProcess != null)
                {
                    EditorGUILayout.LabelField("");

                    if (GUILayout.Button("設定変更を反映"))
                    {
                        EditorCoroutineUtility.StartCoroutineOwnerless(
                            WeaveInstaller.Update(
                                _manifest,
                                e =>
                                {
                                    Repaint();
                                    events = e;
                                },
                                () =>
                                {
                                    Repaint();
                                    _updating = false;
                                    _status = null;
                                },
                                _postProcess
                            )
                        );
                        _updating = true;
                        Repaint();
                    }
                }
            }
            else if (_status == "DELETE_COMPLETE")
            {
                if (GUILayout.Button("インストール"))
                {
                    if (Validate())
                    {
                        EditorCoroutineUtility.StartCoroutineOwnerless(
                            WeaveInstaller.Install(
                                _manifest,
                                e => { events = e; },
                                () =>
                                {
                                    _installing = false;
                                    _status = null;
                                    Repaint();
                                },
                                _postProcess
                            )
                        );
                        _installing = true;
                        
                    
                        void RunOutputCoroutine()
                        {
                            EditorCoroutineUtility.StartCoroutineOwnerless(
                                WeaveInstaller.GetOutputs(
                                    _manifest,
                                    r =>
                                    {
                                        if (_installing)
                                        {
                                            RunOutputCoroutine();
                                        }

                                        if (r.Error == null)
                                        {
                                            Repaint();
                                            outputs = r.Result.items;
                                        }
                                    }
                                )
                            );
                        }
                                
                        RunOutputCoroutine();
                        Repaint();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Validation Error", "インストールパラメータの入力値に問題があります", "OK");
                    }
                }
            }
            else if (_status == "ROLLBACK_COMPLETE")
            {
                EditorGUILayout.LabelField("インストールに失敗しました");
                
                using (new GUILayout.VerticalScope(GUI.skin.box))
                {
                    if (events != null)
                    {
                        foreach (var e in events)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                if (e.eventAt != null)
                                    GUILayout.Label(UnixTime.FromUnixTime(e.eventAt.Value).ToShortTimeString());
                                GUILayout.Label(e.resourceName);
                                GUILayout.Label(e.type);
                                GUILayout.Label(e.message);
                            }
                        }
                    }
                }

                if (GUILayout.Button("アンインストール"))
                {
                    EditorCoroutineUtility.StartCoroutineOwnerless(
                        WeaveInstaller.Uninstall(
                            _manifest,
                            e =>
                            {
                                Repaint();
                                events = e;
                            },
                            () =>
                            {
                                Repaint();
                                _uninstalling = false;
                                _status = null;
                            }
                        )
                    );
                    
                    _uninstalling = true;
                    Repaint();
                }

            }
            else
            {
                EditorGUILayout.LabelField("状態を取得中...");
                _status = null;
            }
        }

        protected abstract bool Validate();

        protected virtual WeaveInstaller.PostProcess PostProcess()
        {
            return null;
        }
    }
}
