using System;
using System.Collections;
using Gs2.Unity;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Core.Watcher
{
    [Serializable]
    public class WatchWalletEvent : UnityEvent<EzWallet>
    {
    }

    public class MoneyWatcher
    {
        [SerializeField]
        public WatchWalletEvent onWatchWalletEvent = new WatchWalletEvent();

        private bool _watching;

        private Client _client;
        private GameSession _session;
        private EzWallet _wallet;

        private string _moneyNamespaceName;
        private int _slot;
        private GetWalletEvent _onGetWallet;
        private DepositEvent _onDeposit;
        private WithdrawEvent _onWithdraw;
        private ErrorEvent _onError;

        public EzWallet Wallet => _wallet;

        private void DepositAction(
            EzWallet wallet, 
            float price,
            int amount
        )
        {
            _wallet = wallet;
            
            onWatchWalletEvent.Invoke(_wallet);
        }

        private void WithdrawAction(
            EzWallet wallet, 
            int amount
        )
        {
            _wallet = wallet;
            
            onWatchWalletEvent.Invoke(_wallet);
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            string moneyNamespaceName,
            int slot,
            GetWalletEvent onGetWallet,
            DepositEvent onDeposit,
            WithdrawEvent onWithdraw,
            ErrorEvent onError
        )
        {
            if (_watching)
            {
                throw new InvalidOperationException("already started");
            }
            
            _watching = true;

            _client = client;
            _session = session;

            _moneyNamespaceName = moneyNamespaceName;
            _slot = slot;
            _onGetWallet = onGetWallet;
            _onDeposit = onDeposit;
            _onWithdraw = onWithdraw;
            _onError = onError;

            _onDeposit.AddListener(DepositAction);
            _onWithdraw.AddListener(WithdrawAction);

            yield return Refresh();
        }

        public IEnumerator Refresh()
        {
            void RefreshMoneyAction(
                EzWallet wallet
            )
            {
                _wallet = wallet;
                
                _onGetWallet.RemoveListener(RefreshMoneyAction);
                
                onWatchWalletEvent.Invoke(_wallet);
            }

            _onGetWallet.AddListener(RefreshMoneyAction);
            
            yield return MoneyController.GetWallet(
                _client,
                _session,
                _moneyNamespaceName,
                _slot,
                _onGetWallet,
                _onError
            );
        }

        public void Stop()
        {
            if (_watching) return;
            
            _onDeposit.RemoveListener(DepositAction);
            _onWithdraw.RemoveListener(WithdrawAction);
            
            _watching = false;
        }
    }
}