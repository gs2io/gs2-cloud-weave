using System;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Unity.Util;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [System.Serializable]
    public class CreateAccountEvent : UnityEvent<EzAccount>
    {
    }
    
    [System.Serializable]
    public class LoginEvent : UnityEvent<EzAccount, GameSession>
    {
    }
}