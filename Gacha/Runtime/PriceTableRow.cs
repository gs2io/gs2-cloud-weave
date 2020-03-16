using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gs2.Weave.Gacha
{
    public class PriceTableRow : MonoBehaviour
    {
        [SerializeField]
        public SalesItem salesItem;
        
        [SerializeField]
        public Text contents;

        [SerializeField]
        public BuyEvent onBuy = new BuyEvent();

        public void Update()
        {
            contents.text = $"{salesItem.LotteryCount} 回 / {salesItem.Price} Gems";
        }

        public void OnClickBuyButton()
        {
            onBuy.Invoke(salesItem);
        }
    }
}