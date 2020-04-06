﻿using System;
 using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;

 namespace Gs2.Weave.Unit
{
    [Serializable]
    public class UnitSetting : MonoBehaviour
    {
        [SerializeField]
        public string inventoryNamespaceName;

        [SerializeField]
        public string inventoryModelName;

        [SerializeField]
        public string identifierAcquireUnitClientId;

        [SerializeField]
        public string identifierAcquireUnitClientSecret;

        [SerializeField]
        public GetInventoryModelEvent onGetInventoryModel = new GetInventoryModelEvent();

        [SerializeField]
        public GetInventoryEvent onGetInventory = new GetInventoryEvent();

        [SerializeField]
        public GetItemSetWithSignatureEvent onGetItemSetWithSignature = new GetItemSetWithSignatureEvent();
        
        [SerializeField]
        public AcquireEvent onAcquire = new AcquireEvent();

        [SerializeField]
        public ConsumeEvent onConsume = new ConsumeEvent();

        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}