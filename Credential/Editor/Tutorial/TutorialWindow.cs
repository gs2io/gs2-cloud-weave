﻿/*
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

namespace Gs2.Weave.Credential.Editor.Tutorial
{
    enum Steps
    {
        Hello,
        SelectInstaller,
        SelectInstall,
        WaitInstall,
        WaitStart,
        CheckConsoleLog,
        Bye,
    }
    
    public class TutorialWindow : EditorWindow
    {
        private Texture2D _texture = null;

        private Steps _steps = Steps.Hello;

        private void OnEnable()
        {
            if (!bool.Parse(PlayerPrefs.GetString("io.gs2.tutorial.credential", "false")))
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
                    GUILayout.Label("お。ちゃんとインストールできたみたいだにゃ？");
                    GUILayout.Label("");
                    GUILayout.Label("このサンプルは GS2 の クレデンシャル を作成するサンプルだにゃ。");
                    GUILayout.Label("さっきも説明した通り、クレデンシャル を作るには GS2-Deploy を使うのが楽ちんだにゃ。");
                    GUILayout.Label("にゃんと、 GS2-SDK for Unity では GS2-Deploy の実行がボタン一つでできるのにゃ");
                    GUILayout.Label("");

                    if (GUILayout.Button("そうなの？"))
                    {
                        PlayerPrefs.SetString("io.gs2.tutorial.credential", false.ToString());
                        PlayerPrefs.Save();
                        
                        _steps = Steps.SelectInstaller;
                        Repaint();
                    }
                    break;
                case Steps.SelectInstaller:
                    var credentialGameObject = GameObject.Find("Credential");
                    EditorGUIUtility.PingObject(credentialGameObject);
                    
                    GUILayout.Label("そうなのにゃ！");
                    GUILayout.Label("");
                    GUILayout.Label("まずは、シーン内に配置された Credential ってゲームオブジェクトを選択するにゃ。");
                    GUILayout.Label("インスペクターが表示されたら教えてくれれにゃ。");
                    GUILayout.Label("");

                    if (GUILayout.Button("できたよ"))
                    {
                        _steps = Steps.SelectInstall;
                        Repaint();
                    }
                    break;
                case Steps.SelectInstall:
                    
                    GUILayout.Label("できたにゃ？");
                    GUILayout.Label("");
                    GUILayout.Label("インスペクターの Credential Installer に 『インストール』 ってボタンがみえるかにゃ？");
                    GUILayout.Label("それをクリックしたらおしえてにゃ。");
                    GUILayout.Label("");

                    if (GUILayout.Button("できたよ"))
                    {
                        _steps = Steps.WaitInstall;
                        Repaint();
                    }
                    break;
                case Steps.WaitInstall:
                    
                    GUILayout.Label("インストール中…って表示になったかにゃ？");
                    GUILayout.Label("");
                    GUILayout.Label("しばらく待ってるとインストールは終わるからおわったら教えてにゃ");
                    GUILayout.Label("");

                    if (GUILayout.Button("おわったよ"))
                    {
                        _steps = Steps.WaitStart;
                        Repaint();
                    }
                    break;
                case Steps.WaitStart:
                    
                    GUILayout.Label("それじゃ、実行してみるにゃ");
                    GUILayout.Label("");
                    GUILayout.Label("実行したら教えてほしいにゃ");
                    GUILayout.Label("");

                    if (GUILayout.Button("実行したよ"))
                    {
                        _steps = Steps.CheckConsoleLog;
                        Repaint();
                    }
                    break;
                case Steps.CheckConsoleLog:
                    
                    GUILayout.Label("ありがとにゃ。");
                    GUILayout.Label("コンソールタブを開くと、ログが出ているはずだにゃ。");
                    GUILayout.Label("");
                    GUILayout.Label("『SceneDirector::OnCreateGs2Client』 ");
                    GUILayout.Label("ってログが出ていればクレデンシャルは正しく発行されて、初期化がおわってるにゃ。");
                    GUILayout.Label("");
                    GUILayout.Label("続けて、CloudWeave で Login のサンプルを インストール してほしいにゃ。");
                    GUILayout.Label("");

                    if (GUILayout.Button("おっけー"))
                    {
                        PlayerPrefs.SetString("io.gs2.tutorial.login", true.ToString());
                        PlayerPrefs.Save();
                        
                        _steps = Steps.Bye;
                        Repaint();
                    }
                    break;
                case Steps.Bye:
                    
                    GUILayout.Label("それじゃ、インストールが終わったらおしえてにゃ。");
                    GUILayout.Label("");

                    if (GUILayout.Button("バイバイ"))
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