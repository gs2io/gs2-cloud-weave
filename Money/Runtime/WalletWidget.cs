using System;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Money
{
    [Serializable]
    public class ShowStoreEvent : UnityEvent<EzWallet>
    {
    }

    public class WalletWidget : MonoBehaviour
    {
        [SerializeField] 
        public ShowStoreEvent onShowStore = new ShowStoreEvent();

        [SerializeField] 
        public Text amount;

        private MoneyWatcher _watcher;

        public void Initialize(
            MoneyWatcher watcher
        )
        {
            _watcher = watcher;
        }

        void Update()
        {
            if (_watcher == null)
            {
                return;
            }

            if (_watcher.Wallet == null)
            {
                amount.text = "---";
            }
            else
            {
                amount.text = $"{_watcher.Wallet.Free + _watcher.Wallet.Paid}";
            }
        }

        public void OnClickPurchaseButton()
        {
            onShowStore.Invoke(_watcher.Wallet);
        }
    }
}
