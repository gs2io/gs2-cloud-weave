using System;
using System.Collections.Generic;
using Gs2.Gs2Showcase.Model;
using Gs2.Weave.Money;
using UnityEngine;


namespace Gs2.Weave.MoneyStore
{
    [Serializable]
    public class WeaveProduct
    {
        public string name;
        public string metadata;
        public string contentsId;
        public float price;
        public int acquireCount;

        public DisplayItem ToModel(string moneyNamespaceName)
        {
            return new DisplayItem
            {
                type = "salesItem",
                salesItem = new Gs2Showcase.Model.SalesItem
                {
                    name = name,
                    metadata = metadata,
                    consumeActions = new List<ConsumeAction>
                    {
                        new ConsumeAction
                        {
                            action = "Gs2Money:RecordReceipt",
                            request =
                                "{\"namespaceName\":\"" + moneyNamespaceName + "\",\"userId\":\"#{userId}\",\"contentsId\":\"" + contentsId + "\",\"receipt\":\"#{receipt}\"}",
                        }
                    },
                    acquireActions = new List<AcquireAction>
                    {
                        new AcquireAction
                        {
                            action = "Gs2Money:DepositByUserId",
                            request =
                                "{\"namespaceName\":\"" + moneyNamespaceName + "\",\"userId\":\"#{userId}\",\"slot\":\"#{slot}\",\"price\":\"" + price + "\",\"count\":\"" + acquireCount + "\"}",
                        }
                    }
                }
            };
        }
    }
    
    [Serializable]
    public class MoneyStoreInstaller : MonoBehaviour
    {
        [SerializeField]
        public MoneyInstaller moneyInstaller;
        [SerializeField]
        public string showcaseNamespaceName = "money-store";
        [SerializeField]
        public string showcaseModelName = "money";
        [SerializeField]
        public List<WeaveProduct> products = new List<WeaveProduct>
        {
            new WeaveProduct
            {
                name = "120yen",
                contentsId = "io.gs2.sample.contents.120yen",
                price = 120,
                acquireCount = 10,
            },
            new WeaveProduct
            {
                name = "480yen",
                contentsId = "io.gs2.sample.contents.480yen",
                price = 480,
                acquireCount = 50,
            },
            new WeaveProduct
            {
                name = "1000yen",
                contentsId = "io.gs2.sample.contents.1000yen",
                price = 1000,
                acquireCount = 120,
            },
        };
        [SerializeField]
        public string keyNamespaceName = "money-store";
        [SerializeField]
        public string keyName = "key";
    }
}