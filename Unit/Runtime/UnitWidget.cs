using System;
using System.Collections.Generic;
using System.Linq;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Unit
{
    [Serializable]
    public class ChoiceUnitEvent : UnityEvent<EzItemSet>
    {
        
    }
    
    [Serializable]
    public class CloseWidgetUnitEvent : UnityEvent
    {
        
    }

    public class UnitWidget : MonoBehaviour
    {
        [SerializeField] 
        public ChoiceUnitEvent onChoiceUnit = new ChoiceUnitEvent();

        [SerializeField] 
        public CloseWidgetUnitEvent onClose = new CloseWidgetUnitEvent();
        
        [SerializeField] 
        public UnitItem unitItemPrefab;

        [SerializeField] 
        public UnitItemEmpty unitItemEmptyPrefab;

        [SerializeField] 
        public Text capacity;

        [SerializeField] 
        public GridLayoutGroup gridLayoutGroup;

        private InventoryWatcher _watcher;

        public void Initialize(
            InventoryWatcher unitWatcher
        )
        {
            _watcher = unitWatcher;
            _watcher.onWatchInventoryEvent.AddListener(OnChangeInventory);
        }

        public void OnChangeInventory(
            EzInventory unit,
            List<EzItemSet> itemSets
        )
        {
            for (var i = 0; i < gridLayoutGroup.transform.childCount; i++)
            {
                Destroy(gridLayoutGroup.transform.GetChild(i).gameObject);
            }

            foreach (var itemSet in _watcher.ItemSets)
            {
                var item = Instantiate(unitItemPrefab, gridLayoutGroup.transform);
                item.Initialize(
                    _watcher.ItemModels.First(itemModel => itemModel.Name == itemSet.ItemName),
                    itemSet
                );
                item.onClickItem.AddListener(OnClickItem);
                item.gameObject.SetActive(true);
            }

            for (var i = _watcher.ItemSets.Count; i < unit.CurrentInventoryMaxCapacity; i++)
            {
                var item = Instantiate(unitItemEmptyPrefab, gridLayoutGroup.transform);
                item.gameObject.SetActive(true);
            }

            capacity.text =
                $"Capacity: {unit.CurrentInventoryCapacityUsage} / {unit.CurrentInventoryMaxCapacity}";
        }

        public void OnClickItem(EzItemSet itemSet)
        {
            onChoiceUnit.Invoke(itemSet);
        }

        public void OnClose()
        {
            gameObject.SetActive(false);
            onClose.Invoke();
        }
    }
}