﻿using System;
 using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;

 namespace Gs2.Weave.Login
{
    [Serializable]
    public class LoginSetting : MonoBehaviour
    {
        /// <summary>
        /// GS2-Account のネームスペース名
        /// </summary>
        [SerializeField]
        public string accountNamespaceName;

        /// <summary>
        /// GS2-Account でアカウント情報の暗号化に使用する GS2-Key の暗号鍵GRN
        /// </summary>
        [SerializeField]
        public string accountEncryptionKeyId;
        
        /// <summary>
        /// アカウント作成時に発行されるイベント
        /// </summary>
        [SerializeField]
        public CreateAccountEvent onCreateAccount = new CreateAccountEvent();

        /// <summary>
        /// ログイン時に発行されるイベント
        /// </summary>
        [SerializeField]
        public LoginEvent onLogin = new LoginEvent();

        /// <summary>
        /// エラー発生時に発行されるイベント
        /// </summary>
        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}