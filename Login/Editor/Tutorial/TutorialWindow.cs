﻿﻿/*
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
using UnityEditor;
using UnityEngine;

namespace Gs2.Weave.Login.Editor.Tutorial
{
    enum Steps
    {
        Hello,
        SetupCredential,
        WaitInstall,
        CheckLog,
        Bye,
    }
    
    public class TutorialWindow : EditorWindow
    {
        private Texture2D _texture = null;

        private Steps _steps = Steps.Hello;
        
        private void OnEnable()
        {
            if (!bool.Parse(PlayerPrefs.GetString("io.gs2.tutorial.login", "false")))
            {
                Close();
            }
        }

        void OnGUI() 
        {
            if (_texture == null)
            {
                _texture = Resources.Load("mira") as Texture2D;
            }
            switch (_steps)
            {
                case Steps.Hello:
                    GUILayout.Label("できたにゃ？");
                    GUILayout.Label("");
                    GUILayout.Label("このサンプルは GS2 のアカウント管理機能 GS2-Account でログイン機能を実装するサンプルだにゃ。");
                    GUILayout.Label("まずは Credential の時と同じように初期設定をするにゃ。");
                    GUILayout.Label("");

                    if (GUILayout.Button("そうなの？"))
                    {
                        PlayerPrefs.SetString("io.gs2.tutorial.login", false.ToString());
                        PlayerPrefs.Save();

                        _steps = Steps.SetupCredential;
                        Repaint();
                    }
                    break;
                case Steps.SetupCredential:
                    var credentialGameObject = GameObject.Find("Credential");
                    EditorGUIUtility.PingObject(credentialGameObject);
                    
                    GUILayout.Label("まずは、シーン内に配置された Credential ってゲームオブジェクトを選択するにゃ。");
                    GUILayout.Label("");
                    GUILayout.Label("これはもうインストールしたことがあるから 「アンインストール」 メニューがでてるはずだにゃ。");
                    GUILayout.Label("でも、このままじゃ使えないにゃ。");
                    GUILayout.Label("『Credential Setting』 にクレデンシャルの情報が設定されてないからにゃ。");
                    GUILayout.Label("");
                    GUILayout.Label("『設定値をサーバから取得』 を選ぶとインストール済みの設定を読み込めるのにゃ。");
                    GUILayout.Label("読み込みが出来たら教えてほしいにゃ。");
                    GUILayout.Label("");

                    if (GUILayout.Button("できたよ"))
                    {
                        _steps = Steps.WaitInstall;
                        Repaint();
                    }
                    break;
                case Steps.WaitInstall:
                    var loginGameObject = GameObject.Find("Login");
                    EditorGUIUtility.PingObject(loginGameObject);

                    GUILayout.Label("よし、にゃ。");
                    GUILayout.Label("");
                    GUILayout.Label("Login という名前のゲームオブジェクトを選択するにゃ。");
                    GUILayout.Label("これも同じようにインストールして、実行してにゃ。");
                    GUILayout.Label("");

                    if (GUILayout.Button("できたよ"))
                    {
                        _steps = Steps.CheckLog;
                        Repaint();
                    }
                    break;
                case Steps.CheckLog:
                    
                    GUILayout.Label("さっきと同じようにコンソールログを開いたら。");
                    GUILayout.Label("");
                    GUILayout.Label("『LoginDirector::OnLogin』");
                    GUILayout.Label("『SceneDirector::OnCreateGameSession』");
                    GUILayout.Label("");
                    GUILayout.Label("ってログが出てるかにゃ？");
                    GUILayout.Label("これで、アカウント作成をして PlayerPrefs に保存して、ログインしているのにゃ。");
                    GUILayout.Label("");

                    if (GUILayout.Button("なるほど"))
                    {
                        _steps = Steps.Bye;
                        Close();
                    }
                    break;
                case Steps.Bye:
                    
                    GUILayout.Label("CloudWeave はこんな風に色々な機能をボタン一つで取り込めるのにゃ。");
                    GUILayout.Label("色々な機能が用意されてるから試してみてほしいのにゃ");
                    GUILayout.Label("");

                    if (GUILayout.Button("わかりました"))
                    {
                        Close();
                    }
                    break;
            }
            
            var textureWidth = (float)_texture.width;
            var textureHeight = (float)_texture.height;
                
            if (position.width < textureWidth
                || position.height < textureHeight)
            {
                var shrinkWidth = Mathf.Min(position.width / textureWidth / 2, position.height / textureHeight / 2);

                textureWidth *= shrinkWidth;
                textureHeight *= shrinkWidth;

            }
            
            var posX = position.width - textureWidth;
            var posY = position.height - textureHeight;
            GUI.DrawTexture(new Rect(posX, posY, textureWidth, textureHeight), _texture);
        }
    }
}