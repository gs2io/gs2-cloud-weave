using System;
using System.Collections.Generic;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Unity.Gs2Quest.Model;
using Gs2.Unity.Gs2Experience.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [System.Serializable]
    public class GetExperienceModelEvent : UnityEvent<string, EzExperienceModel>
    {
    }

    [System.Serializable]
    public class GetStatusesEvent : UnityEvent<EzExperienceModel, List<EzStatus>>
    {
    }

    [System.Serializable]
    public class IncreaseExperienceEvent : UnityEvent<EzExperienceModel, EzStatus, int>
    {
    }

    [System.Serializable]
    public class IncreaseRankEvent : UnityEvent<EzExperienceModel, EzStatus, int>
    {
    }
}