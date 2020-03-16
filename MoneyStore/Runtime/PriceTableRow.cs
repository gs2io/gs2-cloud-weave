using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.MoneyStore
{
    [Serializable]
    public class ClickBuyButtonEvent : UnityEvent<SalesItem>
    {
    }

    public class PriceTableRow : MonoBehaviour
    {
        [SerializeField]
        public SalesItem salesItem;
        
        [SerializeField]
        public Text contents;

        [SerializeField]
        public ClickBuyButtonEvent onClickBuyButton = new ClickBuyButtonEvent();

        public void Update()
        {
            contents.text = $"{salesItem.Count} Gems / {salesItem.Price} Yen";
        }

        public void OnClickBuyButton()
        {
            onClickBuyButton.Invoke(salesItem);
        }
    }
}