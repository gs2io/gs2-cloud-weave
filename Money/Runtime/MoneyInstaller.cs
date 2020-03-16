using System;
using Gs2.Core.Exception;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Weave.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Money
{
    [Serializable]
    public class MoneyInstaller : MonoBehaviour
    {
        [SerializeField]
        public string moneyNamespaceName = "money";

        [SerializeField] public bool enableDebugDepositAction = true;

        [SerializeField]
        [DrawIf("enableDebugDepositAction", true, ComparisonType.Equals)]
        public string identifierDepositPolicyName = "money-deposit";

        [SerializeField]
        [DrawIf("enableDebugDepositAction", true, ComparisonType.Equals)]
        public string identifierDepositUserName = "money-deposit";
    }
}