using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Gs2Inventory.Request;
using Gs2.Gs2Lottery.Model;
using Gs2.Gs2Lottery.Result;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using Gs2.Weave.Core.Watcher;
using Gs2.Util.LitJson;
using UnityEngine;
using UnityEngine.Events;
using Weave.Core.Runtime;

namespace Gs2.Weave.Gacha
{
    public class GachaDirector : MonoBehaviour
    {
        [SerializeField]
        public StoreWidget storeWidget;

        private Client _client;
        private GameSession _session;
        private StampSheetRunner _stampSheetRunner;
        private Dictionary<string, string> _config;
        
        private ShowcaseWatcher _showcaseWatcher;
        private GachaSetting _gachaSetting;

        public void Start()
        {
            _showcaseWatcher = new ShowcaseWatcher();
            _gachaSetting = GetComponent<GachaSetting>();
            _gachaSetting.onIssueBuyStampSheet.AddListener(OnIssueStampSheet);
        }

        public void OnDestroy()
        {
            _gachaSetting.onIssueBuyStampSheet.RemoveListener(OnIssueStampSheet);
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            StampSheetRunner stampSheetRunner,
            Dictionary<string, string> config
        )
        {
            Debug.Log("GachaDirector::Run");

            _client = client;
            _session = session;
            _stampSheetRunner = stampSheetRunner;
            _config = config;
            
            yield return _showcaseWatcher.Run(
                client,
                session,
                _gachaSetting.showcaseNamespaceName,
                _gachaSetting.showcaseModelName,
                _gachaSetting.onGetShowcase,
                _gachaSetting.onError
            );
        }

        /// <summary>
        /// ウォレットウィジェット でストアの表示ボタンが押されたときに呼び出される。
        /// </summary>
        public void OnShowGachaStore()
        {
            Debug.Log("GachaDirector::OnShowGacha");

            void OnCloseStore()
            {
                storeWidget.onBuy.RemoveListener(OnBuyGacha);
                storeWidget.onClose.RemoveListener(OnCloseStore);
            }
            
            storeWidget.Initialize(
                _showcaseWatcher
            );
            storeWidget.onBuy.AddListener(OnBuyGacha);
            storeWidget.onClose.AddListener(OnCloseStore);
            storeWidget.gameObject.SetActive(true);
        }

        /// <summary>
        /// </summary>
        /// <param name="salesItem"></param>
        public void OnBuyGacha(
            SalesItem salesItem
        )
        {
            Debug.Log("GachaDirector::OnBuyGacha");

            StartCoroutine(
                ShowcaseController.Buy(
                    _client,
                    _session,
                    _gachaSetting.showcaseNamespaceName,
                    _gachaSetting.showcaseModelName,
                    salesItem.DisplayItemId,
                    _gachaSetting.onIssueBuyStampSheet,
                    _gachaSetting.onError,
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
            Debug.Log("GachaDirector::OnIssueStampSheet");

            StartCoroutine(
                _stampSheetRunner.Run(
                    stampSheet,
                    _gachaSetting.showcaseKeyId,
                    _gachaSetting.onError
                )
            );
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("GachaDirector::StateMachineOnDoneStampTask");
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("GachaDirector::StateMachineOnCompleteStampSheet");

                if (sheet.Action == "Gs2Lottery:DrawByUserId")
                {
                    var json = JsonMapper.ToObject(sheetResult.Result);
                    var result = DrawByUserIdResult.FromDict(json);
                    var mergedAcquireRequests = new List<AcquireItemSetByUserIdRequest>();
                    foreach (var acquireRequests in result.items.Select(item => (
                        from acquireAction in item.acquireActions 
                        where acquireAction.action == "Gs2Inventory:AcquireItemSetByUserId" 
                        select JsonMapper.ToObject(acquireAction.request) into acquireJson 
                        select AcquireItemSetByUserIdRequest.FromDict(acquireJson)
                    ).ToList()))
                    {
                        mergedAcquireRequests.AddRange(acquireRequests);
                    }
                    _gachaSetting.onAcquireInventoryItem.Invoke(
                        mergedAcquireRequests
                    );
                    StartCoroutine(
                        _stampSheetRunner.Run(
                            result.stampSheet,
                            _gachaSetting.lotteryKeyId,
                            _gachaSetting.onError
                        )
                    );
                }
            };
        }
    }
}