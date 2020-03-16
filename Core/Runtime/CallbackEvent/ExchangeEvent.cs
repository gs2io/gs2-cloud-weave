using System;
using System.Collections.Generic;
using Gs2.Unity.Gs2Exchange.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [Serializable]
    public class GetExchangeRateEvent : UnityEvent<string, List<EzRateModel>>
    {
    }

    [Serializable]
    public class ExchangeEvent : UnityEvent
    {
    }

    [Serializable]
    public class IssueExchangeStampSheetEvent : UnityEvent<string>
    {
    }

}