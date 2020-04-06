using System;
using System.Collections.Generic;
using System.Linq;
using Gs2.Gs2Inventory.Model;
using Gs2.Unity.Gs2Formation.Model;
using Gs2.Weave.Core.Watcher;
using Gs2.Weave.Unit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Party
{
    [Serializable]
    public class ChoiceSlotEvent : UnityEvent<EzForm, int, EzSlot>
    {
        
    }
    
    public class PartyListRow : MonoBehaviour
    {
        [SerializeField] 
        public HorizontalLayoutGroup horizontalLayoutGroup;

        [SerializeField] 
        public UnitItem unitItemPrefab;

        [SerializeField] 
        public UnitItemEmpty unitItemEmptyPrefab;

        [SerializeField] 
        public MonoBehaviour choiceUnitPrefab;

        private InventoryWatcher _inventoryWatcher;
        private FormationWatcher _formationWatcher;
        private int _index;
        private bool _enableSelectUnit;
        private ChoicePartyEvent _onChoiceParty;
        private ChoiceSlotEvent _onChoiceSlot;

        public void Initialize(
            InventoryWatcher inventoryWatcher,
            FormationWatcher formationWatcher,
            int index,
            bool enableSelectUnit,
            ChoicePartyEvent onChoiceParty,
            ChoiceSlotEvent onChoiceSlot
        )
        {
            _inventoryWatcher = inventoryWatcher;
            _formationWatcher = formationWatcher;
            _index = index;
            _enableSelectUnit = enableSelectUnit;
            _onChoiceParty = onChoiceParty;
            _onChoiceSlot = onChoiceSlot;
            _formationWatcher.onWatchFormation.AddListener(OnChangeFormation);

            OnChangeFormation(
                _formationWatcher.MoldModel,
                _index,
                _formationWatcher.Forms.FirstOrDefault(form => form.Index == index)
            );
        }

        public void OnChangeFormation(
            EzMoldModel moldModel,
            int moldIndex,
            EzForm form
        )
        {
            if (_index != moldIndex)
            {
                return;
            }
            
            var unitGameObjects = new List<MonoBehaviour>();

            void ClearChoicePanel()
            {
                foreach (var unitGameObject in unitGameObjects)
                {
                    for (var i = 0; i < unitGameObject.transform.childCount; i++)
                    {
                        var childGameObject = unitGameObject.transform.GetChild(i);
                        if (childGameObject.name.StartsWith("Choice"))
                        {
                            Destroy(childGameObject.gameObject);
                        }
                    }
                }
            }
            
            for (var i = 0; i < horizontalLayoutGroup.transform.childCount; i++)
            {
                Destroy(horizontalLayoutGroup.transform.GetChild(i).gameObject);
            }

            for (var i = 0; i < moldModel.FormModel.Slots.Count; i++)
            {
                var slot = form?.Slots?.FirstOrDefault(v => v?.Name == i.ToString());
                
                MonoBehaviour item;
                if (slot?.PropertyId == null)
                {
                    item = Instantiate(unitItemEmptyPrefab, horizontalLayoutGroup.transform);
                }
                else
                {
                    var unitItem = Instantiate(unitItemPrefab, horizontalLayoutGroup.transform);
                    unitItem.Initialize(
                        _inventoryWatcher.ItemModels.FirstOrDefault(itemModel =>
                            itemModel.Name == ItemSet.GetItemNameFromGrn(slot.PropertyId)),
                        _inventoryWatcher.ItemSets.FirstOrDefault(itemSet => itemSet.ItemSetId == slot.PropertyId)
                    );
                    item = unitItem;
                }

                var i1 = i;
                item.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ClearChoicePanel();
                    Instantiate(choiceUnitPrefab, item.transform);
                    _onChoiceSlot.Invoke(form, i1, slot);
                });
                item.gameObject.SetActive(true);
                item.GetComponent<Image>().raycastTarget = _enableSelectUnit;
                item.GetComponent<Button>().enabled = _enableSelectUnit;
                unitGameObjects.Add(item);
            }
        }

        public void OnClickParty()
        {
            _onChoiceParty.Invoke(
                _formationWatcher.MoldModel, 
                _index, 
                _formationWatcher.Forms.FirstOrDefault(form => form.Index == _index)
            );
        }
    }
}