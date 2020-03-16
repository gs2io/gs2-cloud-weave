using System;
using System.Collections.Generic;
using Gs2.Gs2Exchange.Model;
using Gs2.Weave.Money;
using Gs2.Weave.Stamina;
using UnityEngine;

namespace Gs2.Weave.StaminaStore
{
    
    [Serializable]
    public class Product
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public int recoverValue;
        [SerializeField]
        public int withdrawCurrencyCount;

        public RateModel ToModel(string staminaNamespaceName, string staminaModelName, string moneyNamespaceName)
        {
            return new RateModel
            {
                name = name,
                acquireActions = new List<AcquireAction>
                {
                    new AcquireAction
                    {
                        action = "Gs2Stamina:RecoverStaminaByUserId",
                        request =
                            "{\"namespaceName\":\"" + staminaNamespaceName + "\",\"staminaName\":\"" + staminaModelName + "\",\"userId\":\"#{userId}\",\"recoverValue\":\"" + recoverValue + "\"}",
                    }
                },
                consumeActions = new List<ConsumeAction>
                {
                    new ConsumeAction
                    {
                        action = "Gs2Money:WithdrawByUserId",
                        request =
                            "{\"namespaceName\":\"" + moneyNamespaceName + "\",\"userId\":\"#{userId}\",\"slot\":\"#{slot}\",\"count\":\"" + withdrawCurrencyCount + "\",\"paidOnly\":false}",
                    }
                },
            };
        }
    }
    
    [Serializable]
    public class StaminaStoreInstaller : MonoBehaviour
    {
        [SerializeField] 
        public MoneyInstaller moneyInstaller;

        [SerializeField] 
        public StaminaInstaller staminaInstaller;

        [SerializeField]
        public string exchangeNamespaceName = "stamina-money-exchange";

        [SerializeField]
        public string keyNamespaceName = "stamina-money-exchange";

        [SerializeField]
        public string keyName = "key";

        [SerializeField]
        public List<Product> products = new List<Product>
        {
            new Product
            {
                name = "stamina_10",
                recoverValue = 10,
                withdrawCurrencyCount = 2,
            },
            new Product
            {
                name = "stamina_50",
                recoverValue = 50,
                withdrawCurrencyCount = 10,
            },
        };
    }
}