﻿using System;
using Gs2.Core.Exception;
using Gs2.Unity.Gs2Money.Model;
 using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Money
{
    [Serializable]
    public class MoneySetting : MonoBehaviour
    {
        [SerializeField]
        public string moneyNamespaceName;

        [SerializeField]
        public int slot;

        [SerializeField]
        public string identifierDepositClientId;

        [SerializeField]
        public string identifierDepositClientSecret;

        [SerializeField]
        public GetWalletEvent onGetWallet = new GetWalletEvent();

        [SerializeField]
        public DepositEvent onDeposit = new DepositEvent();

        [SerializeField]
        public WithdrawEvent onWithdraw = new WithdrawEvent();

        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}