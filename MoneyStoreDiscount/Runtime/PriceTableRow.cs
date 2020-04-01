using System;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.MoneyStoreDiscount
{
    [Serializable]
    public class ClickBuyButtonEvent : UnityEvent<SalesItem>
    {
    }

    public class PriceTableRow : MonoBehaviour
    {
        [SerializeField]
        public Text contents;

        [SerializeField]
        public Text limit;

        private SalesItem _salesItem;
        private LimitWatcher _limitWatcher;
        
        public void Initialize(
            SalesItem salesItem,
            LimitWatcher limitWatcher
        )
        {
            _salesItem = salesItem;
            _limitWatcher = limitWatcher;
        }
        
        [SerializeField]
        public ClickBuyButtonEvent onClickBuyButton = new ClickBuyButtonEvent();

        public void Update()
        {
            contents.text = $"{_salesItem.Count} Gems / {_salesItem.Price} Yen";
            if (_salesItem.LimitCounterName == null)
            {
                limit.text = "";
            }
            else
            {
                limit.text = $"あと {_salesItem.MaxLimit - _limitWatcher.GetCounter(_salesItem.LimitCounterName).Count} 回購入できます";
            }
        }

        public void OnClickBuyButton()
        {
            onClickBuyButton.Invoke(_salesItem);
        }
    }
}