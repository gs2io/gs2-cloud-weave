using System;
using System.Collections.Generic;
using Gs2.Gs2Showcase.Model;
using Gs2.Weave.Core;
using Gs2.Weave.Money;
using UnityEngine;


namespace Gs2.Weave.MoneyStoreDiscount
{
    [Serializable]
    public class WeaveProduct
    {
        public string name;
        public string metadata;
        public string contentsId;
        public float price;
        public int acquireCount;
        public bool specialOffer;
        [DrawIf("specialOffer", true, ComparisonType.Equals)]
        public string specialOfferName;
        [DrawIf("specialOffer", true, ComparisonType.Equals)]
        public string specialOfferMetadata;
        [DrawIf("specialOffer", true, ComparisonType.Equals)]
        public string specialOfferContentsId;
        [DrawIf("specialOffer", true, ComparisonType.Equals)]
        public int specialOfferPrice;
        [DrawIf("specialOffer", true, ComparisonType.Equals)]
        public string specialOfferCounterName;
        [DrawIf("specialOffer", true, ComparisonType.Equals)]
        public int specialOfferLimit;

        public DisplayItem ToModel(
            string moneyNamespaceName, 
            string limitNamespaceName, 
            string limitModelName
        )
        {
            if (specialOffer)
            {
                return new DisplayItem
                {
                    type = "salesItemGroup",
                    salesItemGroup = new Gs2Showcase.Model.SalesItemGroup
                    {
                        name = name,
                        metadata = metadata,
                        salesItems = new List<Gs2Showcase.Model.SalesItem>
                        {
                            new Gs2Showcase.Model.SalesItem
                            {
                                name = specialOfferName,
                                metadata = specialOfferMetadata,
                                consumeActions = new List<ConsumeAction>
                                {
                                    new ConsumeAction
                                    {
                                        action = "Gs2Limit:CountUpByUserId",
                                        request =
                                            "{\"namespaceName\":\"" + limitNamespaceName + "\",\"limitName\":\"" + limitModelName + "\",\"counterName\":\"" + specialOfferCounterName + "\",\"userId\":\"#{userId}\",\"countUpValue\":1,\"maxValue\":" + specialOfferLimit + "}",
                                    },
                                    new ConsumeAction
                                    {
                                        action = "Gs2Money:RecordReceipt",
                                        request =
                                            "{\"namespaceName\":\"" + moneyNamespaceName + "\",\"userId\":\"#{userId}\",\"contentsId\":\"" + specialOfferContentsId + "\",\"receipt\":\"#{receipt}\"}",
                                    },
                                },
                                acquireActions = new List<AcquireAction>
                                {
                                    new AcquireAction
                                    {
                                        action = "Gs2Money:DepositByUserId",
                                        request =
                                            "{\"namespaceName\":\"" + moneyNamespaceName + "\",\"userId\":\"#{userId}\",\"slot\":\"#{slot}\",\"price\":\"" + specialOfferPrice + "\",\"count\":\"" + acquireCount + "\"}",
                                    }
                                },
                            },
                            new Gs2Showcase.Model.SalesItem
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
                                },
                            },
                        },
                    }
                };
            }
            else
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
    }
    
    [Serializable]
    public class MoneyStoreDiscountInstaller : MonoBehaviour
    {
        [SerializeField]
        public MoneyInstaller moneyInstaller;
        [SerializeField]
        public string showcaseNamespaceName = "money-store-discount";
        [SerializeField]
        public string showcaseModelName = "money";
        [SerializeField]
        public List<WeaveProduct> products = new List<WeaveProduct>
        {
            new WeaveProduct
            {
                name = "10gems",
                contentsId = "io.gs2.sample.contents.120yen",
                price = 120,
                acquireCount = 10,
            },
            new WeaveProduct
            {
                name = "50gems",
                contentsId = "io.gs2.sample.contents.480yen",
                price = 480,
                acquireCount = 50,
            },
            new WeaveProduct
            {
                name = "120gems",
                contentsId = "io.gs2.sample.contents.1000yen",
                price = 1000,
                acquireCount = 120,
                specialOffer = true,
                specialOfferName = "special120gems",
                specialOfferPrice = 120,
                specialOfferContentsId = "io.gs2.sample.contents.120yen",
                specialOfferCounterName = "special-offer-money-0001",
                specialOfferLimit = 1,
            },
        };
        [SerializeField]
        public string limitNamespaceName = "money-store-discount";
        [SerializeField]
        public string limitModelName = "specialOffer";
        [SerializeField]
        public string keyNamespaceName = "money-store-discount";
        [SerializeField]
        public string keyName = "key";
    }
}