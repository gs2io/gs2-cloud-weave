using System;
using Gs2.Unity.Gs2Money.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [Serializable]
    public class GetWalletEvent : UnityEvent<EzWallet>
    {
    }

    [Serializable]
    public class DepositEvent : UnityEvent<EzWallet, float, int>
    {
    }

    [Serializable]
    public class WithdrawEvent : UnityEvent<EzWallet, int>
    {
    }

}