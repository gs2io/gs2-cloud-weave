using System;
using Gs2.Unity.Gs2Showcase.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [Serializable]
    public class GetShowcaseEvent : UnityEvent<EzShowcase>
    {
    }
    
    [Serializable]
    public class BuyEvent : UnityEvent<EzDisplayItem>
    {
    }

    [Serializable]
    public class IssueBuyStampSheetEvent : UnityEvent<string>
    {
    }
}