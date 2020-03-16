﻿using System;
using System.Collections.Generic;
using Gs2.Core.Exception;
using Gs2.Gs2Exchange.Model;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Exchange.Model;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Unity.Gs2Stamina.Model;
using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;
using UnityEngine.Events;
using EzAcquireAction = Gs2.Unity.Gs2Exchange.Model.EzAcquireAction;
using EzConsumeAction = Gs2.Unity.Gs2Exchange.Model.EzConsumeAction;

namespace Gs2.Weave.StaminaStore
{
    [System.Serializable]
    public class StaminaStoreSetting : MonoBehaviour
    {
        [SerializeField]
        public string exchangeNamespaceName;
        
        [SerializeField]
        public string exchangeKeyId;
        
        [SerializeField]
        public GetExchangeRateEvent onGetExchangeRate = new GetExchangeRateEvent();
        
        [SerializeField]
        public ExchangeEvent onBuy = new ExchangeEvent();

        [SerializeField]
        public IssueExchangeStampSheetEvent onIssueBuyStampSheet = new IssueExchangeStampSheetEvent();

        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}