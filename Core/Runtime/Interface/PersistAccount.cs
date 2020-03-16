using System;
using UnityEngine;

namespace Gs2.Weave.Login
{
    [Serializable]
    public class PersistAccount
    {
        [SerializeField]
        public string UserId;
        [SerializeField]
        public string Password;
    }
}