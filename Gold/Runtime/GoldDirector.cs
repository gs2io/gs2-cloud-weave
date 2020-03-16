using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Gold
{
    public class GoldDirector : MonoBehaviour
    {
        [SerializeField]
        public GoldWidget goldWidget;

        [SerializeField]
        public DebugGoldControlWidget debugGoldControlWidget;

        private Client _client;
        private GameSession _session;
        private Dictionary<string, string> _config;
        
        private EzInventoryModel _inventoryModel;
        private List<EzItemModel> _itemModels;

        private InventoryWatcher _inventoryWatcher;
        private GoldSetting _goldSetting;

        public void Start()
        {
            _inventoryWatcher = new InventoryWatcher();
            _goldSetting = GetComponent<GoldSetting>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="session"></param>
        /// <param name="canvas"></param>
        /// <param name="config"></param>
        public IEnumerator Run(
            Client client,
            GameSession session,
            Dictionary<string, string> config
        )
        {
            _client = client;
            _session = session;
            _config = config;
        
            void OnGetInventoryModel(
                string inventoryName, 
                EzInventoryModel inventoryModel, 
                List<EzItemModel> itemModels
            )
            {
                _inventoryModel = inventoryModel;
                _itemModels = itemModels;
            }
            
            _goldSetting.onGetInventoryModel.AddListener(OnGetInventoryModel);
        
            yield return InventoryController.GetInventoryModel(
                client,
                _goldSetting.inventoryNamespaceName,
                _goldSetting.inventoryModelName,
                _goldSetting.onGetInventoryModel,
                _goldSetting.onError
            );
            
            _goldSetting.onGetInventoryModel.RemoveListener(OnGetInventoryModel);

            StartCoroutine(
                _inventoryWatcher.Run(
                    client,
                    session,
                    _goldSetting.inventoryNamespaceName,
                    _inventoryModel,
                    _itemModels,
                    _goldSetting.onGetInventory,
                    _goldSetting.onAcquire,
                    _goldSetting.onConsume,
                    _goldSetting.onError
                )
            );

            goldWidget.Initialize(
                _inventoryWatcher
            );
            goldWidget.gameObject.SetActive(true);

            if (debugGoldControlWidget != null)
            {
                debugGoldControlWidget.Initialize();
                debugGoldControlWidget.onClickAcquireButton.AddListener(OnClickAcquireButton);
                debugGoldControlWidget.onClickConsumeButton.AddListener(OnClickConsumeButton);
                debugGoldControlWidget.gameObject.SetActive(true);
            }
        }

        public void OnClickAcquireButton(int acquireCount)
        {
            if (string.IsNullOrEmpty(_goldSetting.identifierAcquireGoldClientId) ||
                string.IsNullOrEmpty(_goldSetting.identifierAcquireGoldClientSecret))
            {
                Debug.LogError("インストーラーの設定でデバッグ機能が無効化されています");
            }
            
            StartCoroutine(
                InventoryController.Acquire(
                    _session,
                    _goldSetting.identifierAcquireGoldClientId,
                    _goldSetting.identifierAcquireGoldClientSecret,
                    _goldSetting.inventoryNamespaceName,
                    _goldSetting.inventoryModelName,
                    _goldSetting.itemModelName,
                    acquireCount,
                    _goldSetting.onAcquire,
                    _goldSetting.onError
                )
            );
        }

        public void OnClickConsumeButton(int consumeCount)
        {
            StartCoroutine(
                InventoryController.Consume(
                    _client,
                    _session,
                    _goldSetting.inventoryNamespaceName,
                    _goldSetting.inventoryModelName,
                    _goldSetting.itemModelName,
                    consumeCount,
                    _goldSetting.onConsume,
                    _goldSetting.onError
                )
            );
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("GoldDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("GoldDirector::StateMachineOnDoneStampTask");

                if (task.Action == "Gs2Inventory:ConsumeItemSetByUserId")
                {
                    StartCoroutine(
                        _inventoryWatcher.Refresh()
                    );
                }
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("GoldDirector::StateMachineOnCompleteStampSheet");

                if (sheet.Action == "Gs2Inventory:AcquireItemSetByUserId")
                {
                    StartCoroutine(
                        _inventoryWatcher.Refresh()
                    );
                }
            };
        }
    }
}