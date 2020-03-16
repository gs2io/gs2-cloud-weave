using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Experience
{
    [Serializable]
    public class ClickIncreaseButtonEvent : UnityEvent<string, int>
    {
        
    }

    public class DebugExperienceControlWidget : MonoBehaviour
    {
        public ClickIncreaseButtonEvent onClickIncreaseButton = new ClickIncreaseButtonEvent();

        public int increaseValue = 100;

        private string _propertyId;
        
        public void Initialize(
            string propertyId
        )
        {
            _propertyId = propertyId;
        }
        
        public void OnClickIncreaseButton()
        {
            onClickIncreaseButton.Invoke(
                _propertyId,
                increaseValue
            );
        }
    }
}