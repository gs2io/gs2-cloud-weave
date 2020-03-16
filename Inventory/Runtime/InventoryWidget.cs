using System.Collections.Generic;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Inventory
{
    public class UseItemEvent : UnityEvent<EzItemSet, int>
    {
        
    }

    public class InventoryWidget : MonoBehaviour
    {
        [SerializeField] 
        public InventoryItem inventoryItemPrefab;

        [SerializeField] 
        public InventoryItemEmpty inventoryItemEmptyPrefab;

        [SerializeField] 
        public Text capacity;

        [SerializeField] 
        public GridLayoutGroup gridLayoutGroup;

        [SerializeField] 
        public UseItemEvent onUseItem = new UseItemEvent();

        private InventoryWatcher _watcher;

        public void Initialize(
            InventoryWatcher inventoryWatcher
        )
        {
            _watcher = inventoryWatcher;
            _watcher.onWatchInventoryEvent.AddListener(OnChangeInventory);
        }

        public void OnChangeInventory(
            EzInventory inventory,
            List<EzItemSet> itemSets
        )
        {
            for (var i = 0; i < gridLayoutGroup.transform.childCount; i++)
            {
                Destroy(gridLayoutGroup.transform.GetChild(i).gameObject);
            }

            foreach (var itemSet in _watcher.ItemSets)
            {
                var item = Instantiate(inventoryItemPrefab, gridLayoutGroup.transform);
                item.Initialize(
                    itemSet
                );
                item.onClickItem.AddListener(OnClickItem);
            }

            for (var i = _watcher.ItemSets.Count; i < inventory.CurrentInventoryMaxCapacity; i++)
            {
                Instantiate(inventoryItemEmptyPrefab, gridLayoutGroup.transform);
            }

            capacity.text =
                $"Capacity: {inventory.CurrentInventoryCapacityUsage} / {inventory.CurrentInventoryMaxCapacity}";
        }

        public void OnClickItem(EzItemSet itemSet)
        {
            onUseItem.Invoke(itemSet, 1);
        }
    }
}