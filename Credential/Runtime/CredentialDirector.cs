using System;
using System.Collections;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Util;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Weave.Core.Runtime;

namespace Gs2.Weave.Credential
{
    [Serializable]
    public class CreateClientEvent : UnityEvent<Gs2Client>
    {
    }

    public class CredentialDirector : MonoBehaviour
    {
        public CreateClientEvent onCreateClient = new CreateClientEvent();

        public Gs2Client gs2ClientPrefab;

        private CredentialSetting _credentialSetting;

        public void Start()
        {
            _credentialSetting = GetComponent<CredentialSetting>();
            Assert.IsNotNull(_credentialSetting);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerator Run()
        {
            Debug.Log("CredentialDirector::Run");

            _credentialSetting.onInitializeGs2.AddListener(OnInitializeGs2);
            
            yield return CredentialController.InitializeGs2(
                _credentialSetting.applicationClientId,
                _credentialSetting.applicationClientSecret,
                _credentialSetting.onInitializeGs2,
                _credentialSetting.onError
            );
            
            _credentialSetting.onInitializeGs2.RemoveListener(OnInitializeGs2);
        }

        public void OnInitializeGs2(
            Profile profile,
            Client client
        )
        {
            Debug.Log("CredentialDirector::OnInitializeGs2");
            
            var gs2Client = Instantiate(gs2ClientPrefab);
            gs2Client.name = gs2ClientPrefab.name;
            gs2Client.Profile = profile;
            gs2Client.Client = client;
            
            onCreateClient.Invoke(gs2Client);
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("CredentialDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("CredentialDirector::StateMachineOnDoneStampTask");
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("CredentialDirector::StateMachineOnCompleteStampSheet");
            };
        }
    }
}