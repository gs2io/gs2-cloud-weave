using Gs2.Unity.Gs2Inventory.Model;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Inventory
{
    public class ClickItemEvent : UnityEvent<EzItemSet>
    {
        
    }
    
    public class InventoryItem : MonoBehaviour
    {
        public Text icon;
        public Text stackCount;

        public ClickItemEvent onClickItem = new ClickItemEvent();

        private EzItemSet _itemSet;

        public void Initialize(
            EzItemSet itemSet
        )
        {
            _itemSet = itemSet;
        }
        
        public void Start()
        {
            if (_itemSet.ItemName == "fire_element")
            {
                icon.text = "炎";
            }
            else if (_itemSet.ItemName == "water_element")
            {
                icon.text = "水";
            }
            else
            {
                icon.text = "？";
            }

            stackCount.text = $"{_itemSet.Count}";
        }

        public void OnClick()
        {
            onClickItem.Invoke(
                _itemSet
            );
        }
    }
}