﻿using System;
using System.Collections.Generic;
using Gs2.Core.Exception;
 using Gs2.Gs2Stamina.Model;
 using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Stamina.Model;
using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Stamina
{
    [System.Serializable]
    public class StaminaSetting : MonoBehaviour
    {
        [SerializeField]
        public string staminaNamespaceName;

        [SerializeField]
        public string staminaModelName;

        [SerializeField]
        public GetStaminaModelEvent onGetStaminaModel = new GetStaminaModelEvent();

        [SerializeField]
        public GetStaminaEvent onGetStamina = new GetStaminaEvent();

        [SerializeField]
        public RecoverStaminaEvent onRecoverStamina = new RecoverStaminaEvent();

        [SerializeField]
        public ConsumeStaminaEvent onConsumeStamina = new ConsumeStaminaEvent();

        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}