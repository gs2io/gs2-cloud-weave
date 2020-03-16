using System;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Unity.Gs2Quest.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [Serializable]
    public class GetQuestModelEvent : UnityEvent<EzQuestGroupModel, EzCompletedQuestList>
    {
    }

    [Serializable]
    public class IssueStartStampSheetEvent : UnityEvent<string>
    {
    }

    [Serializable]
    public class FindProgressEvent : UnityEvent<EzQuestModel, EzProgress>
    {
    }

    [Serializable]
    public class StartQuestEvent : UnityEvent<EzQuestModel, EzProgress>
    {
    }

    [Serializable]
    public class IssueEndStampSheetEvent : UnityEvent<string>
    {
    }

    [Serializable]
    public class EndQuestEvent : UnityEvent<EzQuestModel, EzProgress>
    {
    }
}