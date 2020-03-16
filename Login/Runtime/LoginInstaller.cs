using System;
using UnityEngine;

namespace Gs2.Weave.Login
{
    [Serializable]
    public class LoginInstaller : MonoBehaviour
    {
        /// <summary>
        /// GS2-Account のネームスペース名
        /// </summary>
        [SerializeField]
        public string accountNamespaceName = "account";

        /// <summary>
        /// GS2-Key のネームスペース名
        /// </summary>
        [SerializeField]
        public string keyNamespaceName = "account-key";

        /// <summary>
        /// GS2-Key の暗号鍵名
        /// </summary>
        [SerializeField]
        public string keyName = "key";

    }
}