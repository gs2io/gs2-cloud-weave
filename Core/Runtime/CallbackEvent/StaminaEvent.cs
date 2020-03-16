using System;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Unity.Gs2Quest.Model;
using Gs2.Unity.Gs2Stamina.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [System.Serializable]
    public class GetStaminaModelEvent : UnityEvent<string, EzStaminaModel>
    {
    }

    [System.Serializable]
    public class GetStaminaEvent : UnityEvent<EzStaminaModel, EzStamina>
    {
    }

    [System.Serializable]
    public class RecoverStaminaEvent : UnityEvent<EzStaminaModel, EzStamina, int>
    {
    }

    [System.Serializable]
    public class ConsumeStaminaEvent : UnityEvent<EzStaminaModel, EzStamina, int>
    {
    }
}