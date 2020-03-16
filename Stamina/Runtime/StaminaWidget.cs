using System;
using Gs2.Unity.Gs2Stamina.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Stamina
{
    [Serializable]
    public class ShowStaminaStoreEvent : UnityEvent<EzStamina>
    {
    }

    public class StaminaWidget : MonoBehaviour
    {
        [SerializeField]
        public ShowStaminaStoreEvent onShowStaminaStore = new ShowStaminaStoreEvent();

        [SerializeField]
        public Text staminaValue;

        [SerializeField]
        public Text nextRecoverTime;

        private StaminaWatcher _staminaWatcher;

        public void Initialize(
            StaminaWatcher staminaWatcher
        )
        {
            _staminaWatcher = staminaWatcher;
        }
        
        void Update()
        {
            if (_staminaWatcher.Stamina == null)
            {
                staminaValue.text = "--- / ---";
                nextRecoverTime.text = "--:--";
            }
            else
            {
                staminaValue.text = $"{_staminaWatcher.Stamina.Value} / {_staminaWatcher.Stamina.MaxValue}";
                if (_staminaWatcher.Stamina.Value >= _staminaWatcher.Stamina.MaxValue)
                {
                    nextRecoverTime.text = "--:--";
                }
                else
                {
                    nextRecoverTime.text = $"{_staminaWatcher.NextRecoverSpan.Minutes:00}:{_staminaWatcher.NextRecoverSpan.Seconds:00}";
                }
            }
        }

        public void OnClickPurchaseButton()
        {
            onShowStaminaStore.Invoke(_staminaWatcher.Stamina);
        }
    }
}