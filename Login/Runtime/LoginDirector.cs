using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core.Exception;
using Gs2.Unity;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using UnityEngine;
using UnityEngine.Events;
using Weave.Core.Runtime;

namespace Gs2.Weave.Login
{
    [Serializable]
    public class CreateGs2GameSessionEvent : UnityEvent<Gs2GameSession>
    {
    }

    public class LoginDirector : MonoBehaviour
    {
        public CreateGs2GameSessionEvent onCreateGs2GameSessionEvent = new CreateGs2GameSessionEvent();

        public Gs2GameSession gs2GameSessionPrefab;
        
        private LoginSetting _loginSetting;

        private Client _client;
        private IAccountRepository _repository;

        public void Start()
        {
            _loginSetting = GetComponent<LoginSetting>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="client"></param>
        public IEnumerator Run(
            Client client,
            IAccountRepository repository
        )
        {
            Debug.Log("LoginDirector::Run");

            _client = client;
            _repository = repository;
            
            _loginSetting.onLogin.AddListener(OnLogin);
            _loginSetting.onError.AddListener(OnError);
            
            yield return LoginController.AutoLogin(
                _client,
                repository,
                _loginSetting.accountNamespaceName,
                _loginSetting.accountEncryptionKeyId,
                _loginSetting.onCreateAccount,
                _loginSetting.onLogin,
                _loginSetting.onError
            );
            
            _loginSetting.onLogin.RemoveListener(OnLogin);
            _loginSetting.onError.RemoveListener(OnError);
        }

        public void OnLogin(EzAccount account, GameSession session)
        {
            Debug.Log("LoginDirector::OnLogin");
            
            var gs2GameSession = Instantiate(gs2GameSessionPrefab);
            gs2GameSession.name = gs2GameSessionPrefab.name;
            gs2GameSession.Account = account;
            gs2GameSession.Session = session;
            
            onCreateGs2GameSessionEvent.Invoke(gs2GameSession);
        }

        public void OnError(Gs2Exception e)
        {
            if (e.Errors[0].message == "account.account.account.error.notAuthorized")
            {
                Debug.Log("アカウントの認証に失敗したため、アカウントを削除します");
                _repository.DeleteAccount();
            }
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("JobQueueDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("LoginDirector::StateMachineOnDoneStampTask");
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("LoginDirector::StateMachineOnCompleteStampSheet");
            };
        }
    }
}