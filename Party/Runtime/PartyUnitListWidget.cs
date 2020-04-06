using System;
using System.Collections.Generic;
using System.Linq;
using Gs2.Unity.Gs2Formation.Model;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Weave.Core.Watcher;
using Gs2.Weave.Unit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Party
{
    [Serializable]
    public class ChoiceUnitEvent : UnityEvent<EzItemSet>
    {
        
    }
    
    [Serializable]
    public class CloseWidgetUnitEvent : UnityEvent
    {
        
    }

    public class PartyUnitListWidget : MonoBehaviour
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
        public MonoBehaviour usingPrefab;

        [SerializeField] 
        public Text capacity;

        [SerializeField] 
        public GridLayoutGroup gridLayoutGroup;

        private InventoryWatcher _unitWatcher;
        private FormationWatcher _partyWatcher;

        private EzForm _form;

        void OnChangeFormation(
            EzMoldModel moldModel,
            int moldIndex,
            EzForm form
        )
        {
            if (_form.Index != form.Index)
            {
                return;
            }

            OnChangeInventory(
                _unitWatcher.Inventory,
                _unitWatcher.ItemSets
            );
        }
        
        public void Initialize(
            InventoryWatcher unitWatcher,
            FormationWatcher partyWatcher,
            int moldIndex,
            EzForm form
        )
        {
            _unitWatcher = unitWatcher;
            _partyWatcher = partyWatcher;
            _unitWatcher.onWatchInventoryEvent.AddListener(OnChangeInventory);
            _partyWatcher.onWatchFormation.AddListener(OnChangeFormation);

            _form = form;

            OnChangeFormation(
                _partyWatcher.MoldModel,
                moldIndex,
                form
            );
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

            foreach (var itemSet in _unitWatcher.ItemSets)
            {
                var item = Instantiate(unitItemPrefab, gridLayoutGroup.transform);
                item.Initialize(
                    _unitWatcher.ItemModels.First(itemModel => itemModel.Name == itemSet.ItemName),
                    itemSet
                );
                if (_form != null && _form.Slots.Select(slot => slot?.PropertyId).Contains(itemSet.ItemSetId))
                {
                    Instantiate(usingPrefab, item.transform);
                }
                else
                {
                    item.onClickItem.AddListener(OnClickItem);
                }

                item.gameObject.SetActive(true);
            }

            for (var i = _unitWatcher.ItemSets.Count; i < unit.CurrentInventoryMaxCapacity; i++)
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