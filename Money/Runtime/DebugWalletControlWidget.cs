using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Money
{
    [Serializable]
    public class ClickDepositButtonEvent : UnityEvent<float, int>
    {
        
    }

    [Serializable]
    public class ClickWithdrawButtonEvent : UnityEvent<int>
    {
        
    }

    public class DebugWalletControlWidget : MonoBehaviour
    {
        [SerializeField]
        public float depositPrice = 0;
        
        [SerializeField]
        public int depositCount = 10;
        
        [SerializeField]
        public int withdrawCount = 5;
        
        [SerializeField]
        public ClickDepositButtonEvent onClickDepositButton = new ClickDepositButtonEvent();
        
        [SerializeField]
        public ClickWithdrawButtonEvent onClickWithdrawButton = new ClickWithdrawButtonEvent();

        public void Initialize()
        {
            
        }
        
        public void OnClickDepositButton()
        {
            onClickDepositButton.Invoke(depositPrice, depositCount);
        }
        
        public void OnClickWithdrawButton()
        {
            onClickWithdrawButton.Invoke(withdrawCount);
        }
    }
}