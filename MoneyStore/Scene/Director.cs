﻿using System.Collections;
using System.Collections.Generic;
using Gs2.Core.Exception;
using Gs2.Weave.Credential;
using Gs2.Weave.Login;
using Gs2.Weave.Money;
using UnityEngine;
using Weave.Core.Runtime;

namespace Gs2.Weave.MoneyStore
{
    public class Director : MonoBehaviour
    {
        [SerializeField]
        public int slot = 0;

        [SerializeField]
        public CredentialDirector credentialDirector;

        [SerializeField]
        public LoginDirector loginDirector;

        [SerializeField] 
        public MoneyDirector moneyDirector;

        [SerializeField] 
        public MoneyStoreDirector moneyStoreDirector;

        private Gs2Client _client;
        private Gs2GameSession _session;
        private StampSheetRunner _stampSheetRunner;

        private IEnumerator InitializeImpl()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (!credentialDirector.isActiveAndEnabled) continue;
                if (!loginDirector.isActiveAndEnabled) continue;
                if (!moneyDirector.isActiveAndEnabled) continue;
                if (!moneyStoreDirector.isActiveAndEnabled) continue;
                break;
            }

            yield return credentialDirector.Run();
        }
        
        /// <summary>
        /// シーンの開始時に実行される。
        /// GS2 SDK の初期化を行う。
        ///
        /// 初期化は以下の流れで処理され、コールバックにより初期化の完了を受け取る。
        /// CredentialController::InitializeGs2
        ///  ↓
        /// CredentialSample::OnInitializeGs2
        ///  ↓
        /// this::OnCreateGs2Client
        /// </summary>
        public void Start()
        {
            Debug.Log("SceneDirector::Start");
            StartCoroutine(
                InitializeImpl()
            );
        }

        /// <summary>
        /// GS2 SDK の初期化が完了し、クライアントの取得が終わったときに呼び出される。
        /// ここでは、受け取った GS2 Client を使用して、アカウントの新規作成・ログインを実行する。
        ///
        /// アカウントの新規作成・ログインは以下の流れで処理され、コールバックによりログイン結果を受け取る
        /// LoginController::AutoLogin
        ///  ↓
        /// LoginSample::OnLoginAccount
        ///  ↓
        /// this::OnCreateGameSession
        /// </summary>
        /// <param name="client"></param>
        public void OnCreateGs2Client(Gs2Client client)
        {
            Debug.Log("SceneDirector::OnCreateGs2Client");

            _client = client;
            
            _stampSheetRunner = new StampSheetRunner(
                _client.Client
            );
            _stampSheetRunner.AddDoneStampTaskEventHandler(
                moneyDirector.GetTaskCompleteAction(),
                moneyStoreDirector.GetTaskCompleteAction()
            );
            _stampSheetRunner.AddCompleteStampSheetEvent(
                moneyDirector.GetSheetCompleteAction(),
                moneyStoreDirector.GetSheetCompleteAction()
            );

            StartCoroutine(
                loginDirector.Run(
                    client.Client,
                    new PlayerPrefsAccountRepository()
                )
            );
        }

        /// <summary>
        /// アカウントの作成・ログインが完了し、ログインセッションの作成が終わったときに呼び出される。
        /// </summary>
        /// <param name="session"></param>
        public void OnCreateGameSession(Gs2GameSession session)
        {
            Debug.Log("SceneDirector::OnCreateGameSession");

            _session = session;
            
            var config = new Dictionary<string, string>
            {
                ["slot"] = slot.ToString()
            };

            StartCoroutine(
                moneyDirector.Run(
                    _client.Client,
                    _session.Session,
                    slot,
                    config
                )
            );

            StartCoroutine(
                moneyStoreDirector.Run(
                    _client.Client,
                    _session.Session,
                    _stampSheetRunner,
                    config
                )
            );
        }
        
        public void OnError(Gs2Exception e)
        {
            Debug.Log(e);
        }
    }
}