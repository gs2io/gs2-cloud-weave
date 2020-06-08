using System;
using System.Collections.Generic;
using System.Linq;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Util.LitJson;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Unit
{
    [Serializable]
    public class ClickAcquireButtonEvent : UnityEvent<string, int>
    {
        
    }

    public class DebugUnitControlWidget : MonoBehaviour
    {
        public Dropdown dropdown;

        public int acquireCount = 1;

        public ClickAcquireButtonEvent onClickAcquireButton = new ClickAcquireButtonEvent();

        private List<EzItemModel> _itemModels;
        
        public void Initialize(
            List<EzItemModel> itemModels
        )
        {
            _itemModels = itemModels;
            
            dropdown.options = itemModels.Select(
                itemModel => new Dropdown.OptionData(
                    JsonMapper.ToObject<Metadata>(itemModel.Metadata).displayName
                )
            ).ToList();
        }
        
        public void OnClickAcquireButton()
        {
            onClickAcquireButton.Invoke(
                _itemModels[dropdown.value].Name,
                acquireCount
            );
        }
    }
}