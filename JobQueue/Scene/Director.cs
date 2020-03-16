using System.Collections;
using Gs2.Core.Exception;
using Gs2.Unity.Util;
using Gs2.Weave.Credential;
using Gs2.Weave.Login;
using UnityEngine;
using Weave.Core.Runtime;

namespace Gs2.Weave.JobQueue
{
    public class Director : MonoBehaviour
    {
        [SerializeField]
        public CredentialDirector credentialDirector;

        [SerializeField]
        public LoginDirector loginDirector;

        [SerializeField]
        public JobQueueDirector jobQueueDirector;

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
                if (!jobQueueDirector.isActiveAndEnabled) continue;
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
        /// JobQueueController::AutoJobQueue
        ///  ↓
        /// JobQueueSample::OnJobQueueAccount
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
                jobQueueDirector.GetTaskCompleteAction()
            );
            _stampSheetRunner.AddCompleteStampSheetEvent(
                jobQueueDirector.GetSheetCompleteAction()
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

            StartCoroutine(
                jobQueueDirector.Run(
                    _client.Client,
                    _session.Session
                )
            );
        }
        
        public void OnError(Gs2Exception e)
        {
            Debug.Log(e);
        }
    }
}