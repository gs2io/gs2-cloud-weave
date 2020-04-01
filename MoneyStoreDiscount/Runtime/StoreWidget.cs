using System;
using System.Collections.Generic;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.MoneyStoreDiscount
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

        private ShowcaseWatcher _showcaseWatcher;
        private LimitWatcher _limitWatcher;

        public void Initialize(
            ShowcaseWatcher showcaseWatcher,
            LimitWatcher limitWatcher
        )
        {
            _showcaseWatcher = showcaseWatcher;
            _limitWatcher = limitWatcher;
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
                item.Initialize(
                    new SalesItem(displayItem.DisplayItemId, displayItem.SalesItem),
                    _limitWatcher
                );
                item.onClickBuyButton.AddListener(OnClickBuyButton);
            }
        }
        
        public void OnEnable()
        {
            _showcaseWatcher.onWatchShowcaseEvent.AddListener(
                OnGetSalesItems
            );
            
            OnGetSalesItems(_showcaseWatcher.Showcase, _showcaseWatcher.DisplayItems);
        }

        public void OnDisable()
        {
            _showcaseWatcher.onWatchShowcaseEvent.RemoveListener(
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