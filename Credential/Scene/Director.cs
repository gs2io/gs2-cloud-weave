using System.Collections;
using Gs2.Core.Exception;
using Gs2.Unity.Util;
using Gs2.Weave.Credential;
using UnityEngine;
using Weave.Core.Runtime;

namespace Gs2.Weave.Credential
{
    public class Director : MonoBehaviour
    {
        [SerializeField]
        public CredentialDirector credentialDirector;

        private IEnumerator InitializeImpl()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                if (!credentialDirector.isActiveAndEnabled) continue;
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
        }
        
        public void OnError(Gs2Exception e)
        {
            Debug.Log(e);
        }
    }
}