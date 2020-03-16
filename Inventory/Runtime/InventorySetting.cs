﻿using System;
 using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;

 namespace Gs2.Weave.Inventory
{
    [Serializable]
    public class InventorySetting : MonoBehaviour
    {
        [SerializeField]
        public string inventoryNamespaceName;

        [SerializeField]
        public string inventoryModelName;

        [SerializeField]
        public string identifierAcquireItemClientId;

        [SerializeField]
        public string identifierAcquireItemClientSecret;

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