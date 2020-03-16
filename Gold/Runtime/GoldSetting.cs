﻿using System;
using System.Collections.Generic;
using Gs2.Core.Exception;
using Gs2.Unity.Gs2Inventory.Model;
 using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Gold
{
    [Serializable]
    public class GoldSetting : MonoBehaviour
    {
        [SerializeField]
        public string inventoryNamespaceName;

        [SerializeField]
        public string inventoryModelName;

        [SerializeField]
        public string itemModelName;

        [SerializeField]
        public string identifierAcquireGoldClientId;

        [SerializeField]
        public string identifierAcquireGoldClientSecret;

        [SerializeField]
        public GetInventoryModelEvent onGetInventoryModel = new GetInventoryModelEvent();

        [SerializeField]
        public GetInventoryEvent onGetInventory = new GetInventoryEvent();

        [SerializeField]
        public AcquireEvent onAcquire = new AcquireEvent();

        [SerializeField]
        public ConsumeEvent onConsume = new ConsumeEvent();

        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}