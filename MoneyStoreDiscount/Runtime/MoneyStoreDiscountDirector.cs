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

namespace Gs2.Weave.MoneyStoreDiscount
{
    public class MoneyStoreDiscountDirector : MonoBehaviour
    {
        [SerializeField]
        public StoreWidget storeWidget;

        private Client _client;
        private GameSession _session;
        private StampSheetRunner _stampSheetRunner;
        private Dictionary<string, string> _config;
        
        private ShowcaseWatcher _showcaseWatcher;
        private LimitWatcher _limitWatcher;
        private MoneyStoreDiscountSetting _moneyStoreDiscountSetting;

        public void Start()
        {
            _showcaseWatcher = new ShowcaseWatcher();
            _limitWatcher = new LimitWatcher();
            _moneyStoreDiscountSetting = GetComponent<MoneyStoreDiscountSetting>();
            _moneyStoreDiscountSetting.onIssueBuyStampSheet.AddListener(OnIssueStampSheet);
        }

        public void OnDestroy()
        {
            _moneyStoreDiscountSetting.onIssueBuyStampSheet.RemoveListener(OnIssueStampSheet);
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            StampSheetRunner stampSheetRunner,
            Dictionary<string, string> config
        )
        {
            Debug.Log("MoneyStoreDiscountDirector::Run");

            _client = client;
            _session = session;
            _stampSheetRunner = stampSheetRunner;
            _config = config;
            
            yield return _showcaseWatcher.Run(
                client,
                session,
                _moneyStoreDiscountSetting.showcaseNamespaceName,
                _moneyStoreDiscountSetting.showcaseModelName,
                _moneyStoreDiscountSetting.onGetShowcase,
                _moneyStoreDiscountSetting.onError
            );
            
            yield return _limitWatcher.Run(
                client,
                session,
                _moneyStoreDiscountSetting.limitNamespaceName,
                _moneyStoreDiscountSetting.limitModelName,
                _moneyStoreDiscountSetting.onGetLimitModel,
                _moneyStoreDiscountSetting.onGetCounter,
                _moneyStoreDiscountSetting.onCountUpEvent,
                _moneyStoreDiscountSetting.onError
            );
        }

        /// <summary>
        /// ウォレットウィジェット でストアの表示ボタンが押されたときに呼び出される。
        /// </summary>
        /// <param name="wallet"></param>
        public void OnShowMoneyStoreDiscount(
            EzWallet wallet
        )
        {
            Debug.Log("MoneyStoreDiscountDirector::OnShowMoneyStoreDiscount");

            void OnCloseStore()
            {
                storeWidget.onChoiceSalesItem.RemoveListener(OnBuyMoney);
                storeWidget.onClose.RemoveListener(OnCloseStore);
            }
            
            storeWidget.Initialize(
                _showcaseWatcher,
                _limitWatcher
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
                    _moneyStoreDiscountSetting.showcaseNamespaceName,
                    _moneyStoreDiscountSetting.showcaseModelName,
                    salesItem.DisplayItemId,
                    _moneyStoreDiscountSetting.onIssueBuyStampSheet,
                    _moneyStoreDiscountSetting.onError,
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
            Debug.Log("MoneyStoreDiscountDirector::OnIssueStampSheet");

            StartCoroutine(
                _stampSheetRunner.Run(
                    stampSheet,
                    _moneyStoreDiscountSetting.showcaseKeyId,
                    _moneyStoreDiscountSetting.onError
                )
            );
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("MoneyStoreDiscountDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("MoneyStoreDiscountDirector::StateMachineOnDoneStampTask");

                if (task.Action == "Gs2Limit:CountUpByUserId")
                {
                    StartCoroutine(
                        _limitWatcher.Refresh()
                    );
                    StartCoroutine(
                        _showcaseWatcher.Refresh()
                    );
                }
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("MoneyStoreDiscountDirector::StateMachineOnCompleteStampSheet");
            };
        }
    }
}