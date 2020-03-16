using System;
using Gs2.Unity.Gs2JobQueue.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [Serializable]
    public class RunJobEvent : UnityEvent<EzJob, EzJobResultBody, bool>
    {
    }

}