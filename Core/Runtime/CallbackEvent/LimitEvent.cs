using System;
using Gs2.Unity.Gs2Limit.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [Serializable]
    public class GetLimitModelEvent : UnityEvent<string, EzLimitModel>
    {
    }

    [Serializable]
    public class GetCounterEvent : UnityEvent<EzLimitModel, EzCounter>
    {
    }

    [Serializable]
    public class CountUpEvent : UnityEvent<EzLimitModel, EzCounter, int>
    {
    }
}