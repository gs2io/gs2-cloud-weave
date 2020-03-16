using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2Exchange.Model;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Gs2Stamina.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using Weave.Core.Runtime;

namespace Gs2.Weave.StaminaStore
{
    public class StaminaStoreDirector : MonoBehaviour
    {
        [SerializeField]
        public StaminaStoreWidget staminaStoreWidget;

        private Client _client;
        private GameSession _session;
        private StampSheetRunner _stampSheetRunner;
        private Dictionary<string, string> _config;
        
        private ExchangeWatcher _exchangeWatcher;
        private StaminaStoreSetting _staminaStoreSetting;

        public void Start()
        {
            _exchangeWatcher = new ExchangeWatcher();
            _staminaStoreSetting = GetComponent<StaminaStoreSetting>();
            _staminaStoreSetting.onIssueBuyStampSheet.AddListener(OnIssueStampSheet);
        }

        public void OnDestroy()
        {
            staminaStoreWidget.onBuyStamina.RemoveListener(OnBuyStamina);
            _staminaStoreSetting.onIssueBuyStampSheet.RemoveListener(OnIssueStampSheet);
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            StampSheetRunner stampSheetRunner,
            Dictionary<string, string> config
        )
        {
            Debug.Log("StaminaStoreDirector::Run");

            _client = client;
            _session = session;
            _stampSheetRunner = stampSheetRunner;
            _config = config;
            
            yield return _exchangeWatcher.Run(
                client,
                session,
                _staminaStoreSetting.exchangeNamespaceName,
                _staminaStoreSetting.onGetExchangeRate,
                _staminaStoreSetting.onError
            );

            staminaStoreWidget.Initialize(
                _exchangeWatcher
            );
            staminaStoreWidget.onBuyStamina.AddListener(OnBuyStamina);
        }

        public void OnOpenStore(EzStamina stamina)
        {
            staminaStoreWidget.gameObject.SetActive(true);
        }

        public void OnBuyStamina(string rateName)
        {
            Debug.Log("StaminaStoreDirector::OnBuyStamina");

            StartCoroutine(
                ExchangeController.Exchange(
                    _client,
                    _session,
                    _staminaStoreSetting.exchangeNamespaceName,
                    rateName,
                    1,
                    _staminaStoreSetting.onIssueBuyStampSheet,
                    _staminaStoreSetting.onError,
                    _config.Select(item => new EzConfig
                    {
                        Key = item.Key,
                        Value = item.Value
                    }).ToList()
                )
            );
        }

        public void OnIssueStampSheet(
            string stampSheet
        )
        {
            StartCoroutine(
                _stampSheetRunner.Run(
                    stampSheet,
                    _staminaStoreSetting.exchangeKeyId,
                    _staminaStoreSetting.onError
                )
            );
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("StaminaStoreDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("StaminaStoreDirector::StateMachineOnDoneStampTask");
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("StaminaStoreDirector::StateMachineOnCompleteStampSheet");
            };
        }
    }
}