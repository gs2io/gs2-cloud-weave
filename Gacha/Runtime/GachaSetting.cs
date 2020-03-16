﻿using System;
 using System.Collections.Generic;
 using Gs2.Gs2Inventory.Request;
 using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;
 using UnityEngine.Events;

 namespace Gs2.Weave.Gacha
{
    [Serializable]
    public class AcquireInventoryItemEvent : UnityEvent<List<AcquireItemSetByUserIdRequest>>
    {
        
    }
    
    [Serializable]
    public class GachaSetting : MonoBehaviour
    {
        [SerializeField]
        public string lotteryNamespaceName;
        [SerializeField]
        public string jobQueueNamespaceName;
        [SerializeField]
        public string showcaseNamespaceName;
        [SerializeField]
        public string showcaseModelName;
        [SerializeField]
        public string showcaseKeyId;
        [SerializeField]
        public string lotteryKeyId;

        [SerializeField]
        public GetShowcaseEvent onGetShowcase = new GetShowcaseEvent();
        [SerializeField]
        public BuyEvent onBuy = new BuyEvent();
        [SerializeField]
        public AcquireInventoryItemEvent onAcquireInventoryItem = new AcquireInventoryItemEvent();
        [SerializeField]
        public IssueBuyStampSheetEvent onIssueBuyStampSheet = new IssueBuyStampSheetEvent();
        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}