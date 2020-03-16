using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using Weave.Core.Runtime;

namespace Gs2.Weave.MoneyStore
{
    public class MoneyStoreDirector : MonoBehaviour
    {
        [SerializeField]
        public StoreWidget storeWidget;

        private Client _client;
        private GameSession _session;
        private StampSheetRunner _stampSheetRunner;
        private Dictionary<string, string> _config;
        
        private ShowcaseWatcher _showcaseWatcher;
        private MoneyStoreSetting _moneyStoreSetting;

        public void Start()
        {
            _showcaseWatcher = new ShowcaseWatcher();
            _moneyStoreSetting = GetComponent<MoneyStoreSetting>();
            _moneyStoreSetting.onIssueBuyStampSheet.AddListener(OnIssueStampSheet);
        }

        public void OnDestroy()
        {
            _moneyStoreSetting.onIssueBuyStampSheet.RemoveListener(OnIssueStampSheet);
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            StampSheetRunner stampSheetRunner,
            Dictionary<string, string> config
        )
        {
            Debug.Log("MoneyStoreDirector::Run");

            _client = client;
            _session = session;
            _stampSheetRunner = stampSheetRunner;
            _config = config;
            
            yield return _showcaseWatcher.Run(
                client,
                session,
                _moneyStoreSetting.showcaseNamespaceName,
                _moneyStoreSetting.showcaseModelName,
                _moneyStoreSetting.onGetShowcase,
                _moneyStoreSetting.onError
            );
        }

        /// <summary>
        /// ウォレットウィジェット でストアの表示ボタンが押されたときに呼び出される。
        /// </summary>
        /// <param name="wallet"></param>
        public void OnShowMoneyStore(
            EzWallet wallet
        )
        {
            Debug.Log("MoneyStoreDirector::OnShowMoneyStore");

            void OnCloseStore()
            {
                storeWidget.onChoiceSalesItem.RemoveListener(OnBuyMoney);
                storeWidget.onClose.RemoveListener(OnCloseStore);
            }
            
            storeWidget.Initialize(
                _showcaseWatcher
            );
            storeWidget.onChoiceSalesItem.AddListener(OnBuyMoney);
            storeWidget.onClose.AddListener(OnCloseStore);
            storeWidget.gameObject.SetActive(true);
        }

        /// <summary>
        /// </summary>
        /// <param name="salesItem"></param>
        public void OnBuyMoney(
            SalesItem salesItem
        )
        {
            Debug.Log("SceneDirector::OnBuyMoney");
            
            StartCoroutine(
                ShowcaseController.Buy(
                    _client,
                    _session,
                    _moneyStoreSetting.showcaseNamespaceName,
                    _moneyStoreSetting.showcaseModelName,
                    salesItem.DisplayItemId,
                    _moneyStoreSetting.onIssueBuyStampSheet,
                    _moneyStoreSetting.onError,
                    _config.Select(item => new EzConfig
                    {
                        Key = item.Key,
                        Value = item.Value
                    }).ToList(),
                    salesItem.ContentsId
                )
            );
        }

        public void OnIssueStampSheet(
            string stampSheet
        )
        {
            Debug.Log("MoneyStoreDirector::OnIssueStampSheet");

            StartCoroutine(
                _stampSheetRunner.Run(
                    stampSheet,
                    _moneyStoreSetting.showcaseKeyId,
                    _moneyStoreSetting.onError
                )
            );
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("MoneyStoreDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("MoneyStoreDirector::StateMachineOnDoneStampTask");
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("MoneyStoreDirector::StateMachineOnCompleteStampSheet");
            };
        }
    }
}