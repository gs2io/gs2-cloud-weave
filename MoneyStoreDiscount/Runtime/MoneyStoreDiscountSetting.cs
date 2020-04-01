﻿using System;
 using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;

 namespace Gs2.Weave.MoneyStoreDiscount
{
    [Serializable]
    public class MoneyStoreDiscountSetting : MonoBehaviour
    {
        [SerializeField]
        public string moneyNamespaceName;
        [SerializeField]
        public string showcaseNamespaceName;
        [SerializeField]
        public string showcaseModelName;
        [SerializeField]
        public string showcaseKeyId;
        [SerializeField]
        public string limitNamespaceName;
        [SerializeField]
        public string limitModelName;

        [SerializeField]
        public GetShowcaseEvent onGetShowcase = new GetShowcaseEvent();
        [SerializeField]
        public BuyEvent onBuy = new BuyEvent();
        [SerializeField]
        public IssueBuyStampSheetEvent onIssueBuyStampSheet = new IssueBuyStampSheetEvent();
        
        [SerializeField]
        public GetLimitModelEvent onGetLimitModel = new GetLimitModelEvent();
        [SerializeField]
        public GetCounterEvent onGetCounter = new GetCounterEvent();
        [SerializeField]
        public CountUpEvent onCountUpEvent = new CountUpEvent();
            
        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}