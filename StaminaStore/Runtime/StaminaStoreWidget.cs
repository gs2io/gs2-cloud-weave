using System;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.StaminaStore
{
    [Serializable]
    public class BuyStaminaEvent : UnityEvent<string>
    {
    }

    [Serializable]
    public class CloseEvent : UnityEvent
    {
    }

    public class StaminaStoreWidget : MonoBehaviour
    {
        public BuyStaminaEvent onBuyStamina = new BuyStaminaEvent();

        public CloseEvent onClose = new CloseEvent();

        private ExchangeWatcher _watcher;

        public void Initialize(
            ExchangeWatcher watcher
        )
        {
            _watcher = watcher;
        }

        public void OnDisable()
        {
            onClose.Invoke();
        }

        public void OnClickStamina50Button()
        {
            onBuyStamina.Invoke("stamina_50");
            
            gameObject.SetActive(false);
        }

        public void OnClickStamina10Button()
        {
            onBuyStamina.Invoke("stamina_10");

            gameObject.SetActive(false);
        }

        public void OnClickCloseButton()
        {
            gameObject.SetActive(false);
        }
    }
}