using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Unity;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;
using UnityEngine.Events;

namespace Gs2.Weave.Core.Watcher
{
    [Serializable]
    public class WatchInventoryEvent : UnityEvent<EzInventory, List<EzItemSet>>
    {
    }

    public class InventoryWatcher
    {
        public WatchInventoryEvent onWatchInventoryEvent = new WatchInventoryEvent();

        private bool _watching;

        private Client _client;
        private GameSession _session;
        private string _inventoryNamespaceName;
        private EzInventoryModel _inventoryModel;
        private List<EzItemModel> _itemModels;
        private EzInventory _inventory;
        private List<EzItemSet> _itemSets = new List<EzItemSet>();

        private GetInventoryEvent _onGetInventory;
        private AcquireEvent _onAcquire;
        private ConsumeEvent _onConsume;
        private ErrorEvent _onError;
        
        public EzInventory Inventory => _inventory;

        public List<EzItemModel> ItemModels => _itemModels;

        public List<EzItemSet> ItemSets => _itemSets;

        private void AcquireAction(
            EzInventory inventory, 
            List<EzItemSet> itemSets, 
            int acquireValue
        )
        {
            if (inventory.InventoryName != _inventoryModel.Name)
            {
                return;
            }
            
            _inventory = inventory;
            foreach (var itemSet in itemSets)
            {
                _itemSets = _itemSets.Where(item => item.Name != itemSet.Name).ToList();
                _itemSets.Add(itemSet);
            }
            _itemSets.Sort((o1, o2) => o1.SortValue != o2.SortValue ? o1.SortValue - o2.SortValue : (int)(o2.Count - o1.Count));
            
            onWatchInventoryEvent.Invoke(_inventory, _itemSets);
        }

        private void ConsumeAction(
            EzInventory inventory, 
            List<EzItemSet> itemSets, 
            int consumeValue
        )
        {
            if (inventory.InventoryName != _inventoryModel.Name)
            {
                return;
            }
            
            _inventory = inventory;
            foreach (var itemSet in itemSets)
            {
                _itemSets = _itemSets.Where(item => item.Name != itemSet.Name).ToList();
                if (itemSet.Count != 0)
                {
                    _itemSets.Add(itemSet);
                }
            }
            _itemSets.Sort((o1, o2) => o1.SortValue != o2.SortValue ? o1.SortValue - o2.SortValue : (int)(o2.Count - o1.Count));
            
            onWatchInventoryEvent.Invoke(_inventory, _itemSets);
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            string inventoryNamespaceName,
            EzInventoryModel inventoryModel,
            List<EzItemModel> itemModels,
            GetInventoryEvent onGetInventory,
            AcquireEvent onAcquire,
            ConsumeEvent onConsume,
            ErrorEvent onError
        )
        {
            if (_watching)
            {
                throw new InvalidOperationException("already started");
            }
            
            _watching = true;

            _client = client;
            _session = session;
            
            _inventoryNamespaceName = inventoryNamespaceName;
            _inventoryModel = inventoryModel;
            _itemModels = itemModels;

            _onGetInventory = onGetInventory;
            _onAcquire = onAcquire;
            _onConsume = onConsume;
            _onError = onError;

            onAcquire.AddListener(AcquireAction);
            onConsume.AddListener(ConsumeAction);

            yield return Refresh();
        }

        public IEnumerator Refresh()
        {
            void RefreshInventoryAction(
                EzInventory inventory, 
                List<EzItemSet> itemSets
            )
            {
                if (inventory.InventoryName != _inventoryModel.Name)
                {
                    return;
                }
                
                _inventory = inventory;
                _itemSets = itemSets;
                _itemSets.Sort((o1, o2) => o1.SortValue != o2.SortValue ? o1.SortValue - o2.SortValue : (int)(o2.Count - o1.Count));
                
                _onGetInventory.RemoveListener(RefreshInventoryAction);
                
                onWatchInventoryEvent.Invoke(_inventory, _itemSets);
            }

            _onGetInventory.AddListener(RefreshInventoryAction);
            yield return InventoryController.GetInventory(
                _client,
                _session,
                _inventoryNamespaceName,
                _inventoryModel.Name,
                _onGetInventory,
                _onError
            );
        }

        public void Stop()
        {
            if (_watching)
            {
                throw new InvalidOperationException("not started");
            }
            
            _onAcquire.RemoveListener(AcquireAction);
            _onConsume.RemoveListener(ConsumeAction);
            
            _watching = false;
        }
    }
}