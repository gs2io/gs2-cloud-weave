using System;
using Gs2.Core.Exception;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Credential
{
    [Serializable]
    public class InitializeGs2AccountEvent : UnityEvent<Gs2.Unity.Util.Profile, Gs2.Unity.Client>
    {
    }

    [Serializable]
    public class FinalizeGs2AccountEvent : UnityEvent<Gs2.Unity.Util.Profile>
    {
    }

    [System.Serializable]
    public class ErrorEvent : UnityEvent<Gs2Exception>
    {
    }

    [Serializable]
    public class CredentialSetting : MonoBehaviour
    {
        [SerializeField]
        public string applicationClientId;
        
        [SerializeField]
        public string applicationClientSecret;

        [SerializeField]
        public InitializeGs2AccountEvent onInitializeGs2 = new InitializeGs2AccountEvent();

        [SerializeField]
        public FinalizeGs2AccountEvent onFinalizeGs2 = new FinalizeGs2AccountEvent();

        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}