using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Inventory
{
    [Serializable]
    public class ClickAcquireButtonEvent : UnityEvent<string, int>
    {
        
    }

    public class DebugInventoryControlWidget : MonoBehaviour
    {
        public ClickAcquireButtonEvent onClickAcquireButton = new ClickAcquireButtonEvent();

        public int acquireCount = 5;
        
        public void Initialize()
        {
            
        }
        
        public void OnClickFireElementButton()
        {
            OnClickAcquireButton(
                "fire_element"
            );
        }

        public void OnClickWaterElementButton()
        {
            OnClickAcquireButton(
                "water_element"
            );
        }

        public void OnClickAcquireButton(
            string itemModelName
        )
        {
            onClickAcquireButton.Invoke(
                itemModelName,
                acquireCount
            );
        }
    }
}