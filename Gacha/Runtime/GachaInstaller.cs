using System;
using System.Collections.Generic;
using System.Linq;
using Gs2.Gs2Lottery.Model;
using Gs2.Gs2Showcase.Model;
using Gs2.Weave.JobQueue;
using Gs2.Weave.Money;
using Gs2.Weave.Unit;
using UnityEngine;
using ShowcaseAcquireAction = Gs2.Gs2Showcase.Model.AcquireAction;
using LotteryAcquireAction = Gs2.Gs2Lottery.Model.AcquireAction;


namespace Gs2.Weave.Gacha
{
    [Serializable]
    public class WeaveMoneyProduct
    {
        public string name;
        public string metadata;
        public string lotteryName;
        public int price;
        public int lotteryCount;

        public DisplayItem ToModel(
            string moneyNamespaceName,
            string lotteryNamespaceName
        )
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
                            action = "Gs2Money:WithdrawByUserId",
                            request =
                                "{\"namespaceName\":\"" + moneyNamespaceName + "\",\"userId\":\"#{userId}\",\"slot\":\"#{slot}\",\"count\":\"" + price + "\",\"paidOnly\":false}",
                        }
                    },
                    acquireActions = new List<ShowcaseAcquireAction>
                    {
                        new ShowcaseAcquireAction
                        {
                            action = "Gs2Lottery:DrawByUserId",
                            request =
                                "{\"namespaceName\":\"" + lotteryNamespaceName + "\",\"lotteryName\":\"" + lotteryName + "\",\"userId\":\"#{userId}\",\"count\":\"" + lotteryCount + "\"}",
                        }
                    }
                }
            };
        }
    }
    
    [Serializable]
    public class WeaveLotteryProduct
    {
        public string metadata;

        public LotteryModelMaster ToModel(
            string lotteryModelName
        )
        {
            return new LotteryModelMaster
            {
                name = lotteryModelName,
                metadata = metadata,
                mode = "normal",
                method = "prize_table",
                prizeTableName = "rarity",
            };
        }
    }

    [Serializable]
    public class WeaveLotteryPrize
    {
        public string itemModelName;
        public int weight;

        public Prize ToModel(
            string inventoryNamespaceName,
            string inventoryModelName
        )
        {
            return new Prize
            {
                type = "action",
                acquireActions = new List<LotteryAcquireAction>
                {
                    new LotteryAcquireAction
                    {
                        action = "Gs2Inventory:AcquireItemSetByUserId",
                        request = 
                            "{\"namespaceName\":\"" + inventoryNamespaceName + "\",\"inventoryName\":\"" + inventoryModelName + "\",\"itemName\":\"" + itemModelName + "\",\"userId\":\"#{userId}\",\"acquireCount\":\"1\"}",
                    }
                },
                weight = weight,
            };
        }
    }

    [Serializable]
    public class WeaveLotteryPrizeSetting
    {
        public string name;
        public string metadata;
        public int ssrWeight;
        public int srWeight;
        public int rWeight;
        public int nWeight;

        public List<WeaveLotteryPrize> ssrPrizes = new List<WeaveLotteryPrize>();
        public List<WeaveLotteryPrize> srPrizes = new List<WeaveLotteryPrize>();
        public List<WeaveLotteryPrize> rPrizes = new List<WeaveLotteryPrize>();
        public List<WeaveLotteryPrize> nPrizes = new List<WeaveLotteryPrize>();

        public List<LotteryModelMaster> ToLotteryModel()
        {
            return new List<LotteryModelMaster>
            {
                new LotteryModelMaster
                {
                    name = name,
                    metadata = metadata,
                    mode = "normal",
                    method = "prize_table",
                    prizeTableName = "rarity",
                }
            };
        }
        
        public List<PrizeTableMaster> ToPrizeTableModel(
            string inventoryNamespaceName,
            string inventoryModelName
        )
        {
            return new List<PrizeTableMaster>
            {
                new PrizeTableMaster
                {
                    name = "rarity",
                    metadata = metadata,
                    prizes = new List<Prize>
                    {
                        new Prize
                        {
                            type = "prize_table",
                            prizeTableName = "ssr",
                            weight = ssrWeight,
                        },
                        new Prize
                        {
                            type = "prize_table",
                            prizeTableName = "sr",
                            weight = srWeight,
                        },
                        new Prize
                        {
                            type = "prize_table",
                            prizeTableName = "r",
                            weight = rWeight,
                        },
                        new Prize
                        {
                            type = "prize_table",
                            prizeTableName = "n",
                            weight = nWeight,
                        },
                    }
                },
                new PrizeTableMaster
                {
                    name = "ssr",
                    prizes = ssrPrizes.Select(prize => prize.ToModel(
                        inventoryNamespaceName,
                        inventoryModelName
                    )).ToList(),
                },
                new PrizeTableMaster
                {
                    name = "sr",
                    prizes = srPrizes.Select(prize => prize.ToModel(
                        inventoryNamespaceName,
                        inventoryModelName
                    )).ToList(),
                },
                new PrizeTableMaster
                {
                    name = "r",
                    prizes = rPrizes.Select(prize => prize.ToModel(
                        inventoryNamespaceName,
                        inventoryModelName
                    )).ToList(),
                },
                new PrizeTableMaster
                {
                    name = "n",
                    prizes = nPrizes.Select(prize => prize.ToModel(
                        inventoryNamespaceName,
                        inventoryModelName
                    )).ToList(),
                },
            };
        }
    }

    [Serializable]
    public class GachaInstaller : MonoBehaviour
    {
        [SerializeField]
        public MoneyInstaller moneyInstaller;

        [SerializeField]
        public UnitInstaller unitInstaller;

        [SerializeField]
        public JobQueueInstaller jobQueueInstaller;

        [SerializeField]
        public string showcaseNamespaceName = "gacha-store";
        [SerializeField]
        public string showcaseModelName = "money";
        [SerializeField]
        public string lotteryNamespaceName = "gacha-store";
        [SerializeField]
        public string jobQueueNamespaceName = "gacha-store";
        [SerializeField]
        public List<WeaveMoneyProduct> products = new List<WeaveMoneyProduct>
        {
            new WeaveMoneyProduct
            {
                name = "gacha_10",
                lotteryName = "gacha",
                price = 50,
                lotteryCount = 10,
            },
            new WeaveMoneyProduct
            {
                name = "gacha_1",
                lotteryName = "gacha",
                price = 5,
                lotteryCount = 1,
            },
        };

        [SerializeField] 
        public WeaveLotteryPrizeSetting gachaSettings = new WeaveLotteryPrizeSetting
        {
            name = "gacha",
            ssrWeight = 3,
            srWeight = 10,
            rWeight = 27,
            nWeight = 60,
            ssrPrizes = 
            {
                new WeaveLotteryPrize
                {
                    itemModelName = "ssrare_0001",
                    weight = 1,
                },
                new WeaveLotteryPrize
                {
                    itemModelName = "ssrare_0002",
                    weight = 2,
                },
                new WeaveLotteryPrize
                {
                    itemModelName = "ssrare_0003",
                    weight = 3,
                },
            },
            srPrizes = 
            {
                new WeaveLotteryPrize
                {
                    itemModelName = "srare_0001",
                    weight = 1,
                },
                new WeaveLotteryPrize
                {
                    itemModelName = "srare_0002",
                    weight = 2,
                },
                new WeaveLotteryPrize
                {
                    itemModelName = "srare_0003",
                    weight = 2,
                },
            },
            rPrizes =  {
                new WeaveLotteryPrize
                {
                    itemModelName = "rare_0001",
                    weight = 1,
                },
                new WeaveLotteryPrize
                {
                    itemModelName = "rare_0002",
                    weight = 1,
                },
                new WeaveLotteryPrize
                {
                    itemModelName = "rare_0003",
                    weight = 1,
                },
            },
            nPrizes = {
                new WeaveLotteryPrize
                {
                    itemModelName = "normal_0001",
                    weight = 1,
                },
                new WeaveLotteryPrize
                {
                    itemModelName = "normal_0002",
                    weight = 1,
                },
                new WeaveLotteryPrize
                {
                    itemModelName = "normal_0003",
                    weight = 1,
                },
            },
        };
        [SerializeField]
        public string keyNamespaceName = "gacha";
        [SerializeField]
        public string storeKeyName = "store-key";
        [SerializeField]
        public string lotteryKeyName = "lottery-key";
    }
}