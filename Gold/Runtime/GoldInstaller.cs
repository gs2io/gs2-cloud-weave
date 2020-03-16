using System;
using System.Collections.Generic;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Weave.Core;
using UnityEngine;

namespace Gs2.Weave.Gold
{
    [Serializable]
    public class GoldInstaller : MonoBehaviour
    {
        [SerializeField]
        public string inventoryNamespaceName = "gold";

        [SerializeField] 
        public string inventoryModelName = "gold";

        [SerializeField]
        public string itemModelName = "gold";

        [SerializeField]
        public long limitOfCount = 99999999;

        [SerializeField]
        public bool enableDebugAcquireGoldAction = true;

        [SerializeField]
        [DrawIf("enableDebugAcquireGoldAction", true, ComparisonType.Equals)]
        public string identifierAcquireGoldPolicyName = "inventory-acquire-gold";

        [SerializeField]
        [DrawIf("enableDebugAcquireGoldAction", true, ComparisonType.Equals)]
        public string identifierAcquireGoldUserName = "inventory-acquire-gold";

    }
}