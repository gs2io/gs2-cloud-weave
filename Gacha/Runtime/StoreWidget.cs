using System;
using System.Collections.Generic;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Gacha
{
    [Serializable]
    public class BuyEvent : UnityEvent<SalesItem>
    {
    }

    [Serializable]
    public class CloseEvent : UnityEvent
    {
    }

    public class StoreWidget : MonoBehaviour
    {
        [SerializeField]
        public PriceTableRow priceTableRowPrefab;

        [SerializeField]
        public VerticalLayoutGroup verticalLayoutGroup;

        [SerializeField]
        public BuyEvent onBuy = new BuyEvent();

        [SerializeField]
        public CloseEvent onClose = new CloseEvent();

        private ShowcaseWatcher _watcher;

        public void Initialize(
            ShowcaseWatcher watcher
        )
        {
            _watcher = watcher;
        }
        
        public void OnGetSalesItems(EzShowcase showcase, List<EzDisplayItem> displayItems)
        {
            for (var i = 0; i < verticalLayoutGroup.transform.childCount; i++)
            {
                Destroy(verticalLayoutGroup.transform.GetChild(i).gameObject);
            }

            foreach (var displayItem in displayItems)
            {
                var item = Instantiate(priceTableRowPrefab, verticalLayoutGroup.transform);
                item.salesItem = new SalesItem(displayItem.DisplayItemId, displayItem.SalesItem);
                item.onBuy.AddListener(OnClickBuyButton);
                item.gameObject.SetActive(true);
            }
        }
        
        public void Start()
        {
            _watcher.onWatchShowcaseEvent.AddListener(
                OnGetSalesItems
            );
            
            OnGetSalesItems(_watcher.Showcase, _watcher.DisplayItems);
        }

        public void OnClickBuyButton(SalesItem salesItem)
        {
            onBuy.Invoke(salesItem);
            
            gameObject.SetActive(false);
        }

        public void OnCloseButton()
        {
            gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            _watcher.onWatchShowcaseEvent.RemoveListener(
                OnGetSalesItems
            );
            onClose.Invoke();
        }
    }
}