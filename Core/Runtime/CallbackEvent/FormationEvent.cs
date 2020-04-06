using System;
using System.Collections.Generic;
using Gs2.Unity.Gs2Formation.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [Serializable]
    public class GetMoldModelEvent : UnityEvent<string, EzMoldModel>
    {
    }

    [Serializable]
    public class GetFormEvent : UnityEvent<EzMoldModel, int, EzForm>
    {
    }

    [Serializable]
    public class UpdateFormEvent : UnityEvent<EzMoldModel, int, EzForm>
    {
    }
}