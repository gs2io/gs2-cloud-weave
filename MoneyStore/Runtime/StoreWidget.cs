using System;
using System.Collections.Generic;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.MoneyStore
{
    [Serializable]
    public class ChoiceSalesItemEvent : UnityEvent<SalesItem>
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
        public ChoiceSalesItemEvent onChoiceSalesItem = new ChoiceSalesItemEvent();

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
                item.onClickBuyButton.AddListener(OnClickBuyButton);
            }
        }
        
        public void OnEnable()
        {
            _watcher.onWatchShowcaseEvent.AddListener(
                OnGetSalesItems
            );
            
            OnGetSalesItems(_watcher.Showcase, _watcher.DisplayItems);
        }

        public void OnDisable()
        {
            _watcher.onWatchShowcaseEvent.RemoveListener(
                OnGetSalesItems
            );
            onClose.Invoke();
        }

        public void OnClickBuyButton(SalesItem salesItem)
        {
            onChoiceSalesItem.Invoke(salesItem);
            
            gameObject.SetActive(false);
        }

        public void OnCloseButton()
        {
            gameObject.SetActive(false);
        }
    }
}