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

namespace Gs2.Weave.Inventory
{
    public class InventoryDirector : MonoBehaviour
    {
        [SerializeField]
        public InventoryWidget inventoryWidget;

        [SerializeField]
        public DebugInventoryControlWidget debugInventoryControlWidget;

        private Client _client;
        private GameSession _session;
        private Dictionary<string, string> _config;
        
        private EzInventoryModel _inventoryModel;
        private List<EzItemModel> _itemModels;

        private InventoryWatcher _inventoryWatcher;
        private InventorySetting _inventorySetting;

        public InventoryWatcher Watcher => _inventoryWatcher;

        public void Start()
        {
            _inventoryWatcher = new InventoryWatcher();
            _inventorySetting = GetComponent<InventorySetting>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="session"></param>
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
            
            _inventorySetting.onGetInventoryModel.AddListener(OnGetInventoryModel);

            yield return InventoryController.GetInventoryModel(
                client,
                _inventorySetting.inventoryNamespaceName,
                _inventorySetting.inventoryModelName,
                _inventorySetting.onGetInventoryModel,
                _inventorySetting.onError
            );
            
            _inventorySetting.onGetInventoryModel.RemoveListener(OnGetInventoryModel);

            StartCoroutine(
                _inventoryWatcher.Run(
                    client,
                    session,
                    _inventorySetting.inventoryNamespaceName,
                    _inventoryModel,
                    _itemModels,
                    _inventorySetting.onGetInventory,
                    _inventorySetting.onAcquire,
                    _inventorySetting.onConsume,
                    _inventorySetting.onError
                )
            );
            
            inventoryWidget.Initialize(
                _inventoryWatcher
            );
            inventoryWidget.onUseItem.AddListener(OnUseItem);
            inventoryWidget.gameObject.SetActive(true);
            
            debugInventoryControlWidget.Initialize();
            debugInventoryControlWidget.gameObject.SetActive(true);
        }

        public void OnAcquireItem(
            string itemModelName,
            int count
        )
        {
            if (string.IsNullOrEmpty(_inventorySetting.identifierAcquireItemClientId) ||
                string.IsNullOrEmpty(_inventorySetting.identifierAcquireItemClientSecret))
            {
                Debug.LogError("インストーラーの設定でデバッグ機能が無効化されています");
            }

            StartCoroutine(
                InventoryController.Acquire(
                    _session,
                    _inventorySetting.identifierAcquireItemClientId,
                    _inventorySetting.identifierAcquireItemClientSecret,
                    _inventorySetting.inventoryNamespaceName,
                    _inventorySetting.inventoryModelName,
                    itemModelName,
                    count,
                    _inventorySetting.onAcquire,
                    _inventorySetting.onError
                )
            );
        }
        
        public void OnUseItem(
            EzItemSet itemSet,
            int count
        )
        {
            StartCoroutine(
                InventoryController.Consume(
                    _client,
                    _session,
                    _inventorySetting.inventoryNamespaceName,
                    _inventorySetting.inventoryModelName,
                    itemSet.ItemName,
                    count,
                    _inventorySetting.onConsume,
                    _inventorySetting.onError,
                    itemSet.Name
                )
            );
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("InventoryDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("InventoryDirector::StateMachineOnDoneStampTask");

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
                Debug.Log("InventoryDirector::StateMachineOnCompleteStampSheet");
            };
        }
    }
}