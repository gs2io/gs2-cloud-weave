﻿using System;
 using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;

 namespace Gs2.Weave.MoneyStore
{
    [Serializable]
    public class MoneyStoreSetting : MonoBehaviour
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
        public GetShowcaseEvent onGetShowcase = new GetShowcaseEvent();
        [SerializeField]
        public BuyEvent onBuy = new BuyEvent();
        [SerializeField]
        public IssueBuyStampSheetEvent onIssueBuyStampSheet = new IssueBuyStampSheetEvent();
        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}