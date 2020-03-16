using Gs2.Unity.Gs2Inventory.Model;
using LitJson;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Unit
{
    public class ClickItemEvent : UnityEvent<EzItemSet>
    {
        
    }
    
    public class UnitItem : MonoBehaviour
    {
        public Text icon;
        public Text rarity;

        public ClickItemEvent onClickItem = new ClickItemEvent();

        private EzItemSet _itemSet;

        public void Initialize(
            EzItemModel itemModel,
            EzItemSet itemSet
        )
        {
            _itemSet = itemSet;

            var metadata = JsonMapper.ToObject<Metadata>(itemModel.Metadata);
            icon.text = metadata.displayName;
            rarity.text = "";
            for (int i = 0; i < metadata.rarity + 1; i++)
            {
                rarity.text += "★";
            }

            while (rarity.text.Length < 5)
            {
                rarity.text = "☆" + rarity.text;
            }
        }
        
        public void OnClick()
        {
            onClickItem.Invoke(
                _itemSet
            );
        }
    }
}