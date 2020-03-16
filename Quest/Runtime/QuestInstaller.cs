using System;
using System.Collections.Generic;
using Gs2.Core.Exception;
using Gs2.Gs2Quest.Model;
using Gs2.Unity.Gs2Quest.Model;
using Gs2.Weave.Gold;
using Gs2.Weave.Stamina;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Quest
{
    [Serializable]
    public class WeaveQuestModel
    {
        public string name;
        public string metadata;
        public int consumeStamina;
        public int acquireGold;
        public List<string> premiseQuestNames = new List<string>();

        public QuestModel ToModel(
            string staminaNamespaceName, 
            string staminaModelName, 
            string goldInventoryNamespaceName, 
            string goldInventoryModelName, 
            string goldItemModelName
        )
        {
            return new QuestModel
            {
                name = name,
                metadata = metadata,
                consumeActions = new List<ConsumeAction>
                {
                    new ConsumeAction
                    {
                        action = "Gs2Stamina:ConsumeStaminaByUserId",
                        request =
                            "{\"namespaceName\":\"" + staminaNamespaceName + "\",\"staminaName\":\"" + staminaModelName + "\",\"userId\":\"#{userId}\",\"consumeValue\":\"" + consumeStamina + "\"}",
                    },
                },
                contents = new List<Contents>
                {
                    new Contents
                    {
                        completeAcquireActions = new List<AcquireAction>
                        {
                            new AcquireAction
                            {
                                action = "Gs2Inventory:AcquireItemSetByUserId",
                                request =
                                    "{\"namespaceName\":\"" + goldInventoryNamespaceName + "\",\"inventoryName\":\"" + goldInventoryModelName + "\",\"itemName\":\"" + goldItemModelName + "\",\"userId\":\"#{userId}\",\"acquireCount\":\"" + acquireGold + "\",\"expiresAt\":\"\",\"createNewItemSet\":false,\"itemSetName\":\"\"}",
                            }
                        },
                        weight = 1,
                    }
                }
            };
        }
    }
    
    [Serializable]
    public class QuestInstaller : MonoBehaviour
    {
        [SerializeField]
        public StaminaInstaller staminaInstaller;
        
        [SerializeField]
        public GoldInstaller goldInstaller;
        
        [SerializeField]
        public string questNamespaceName = "quest";

        [SerializeField]
        public string questGroupName = "story";

        [SerializeField]
        public List<WeaveQuestModel> quests = new List<WeaveQuestModel>
        {
            new WeaveQuestModel
            {
                name = "1-1",
                acquireGold = 100,
                consumeStamina = 5,
            },
            new WeaveQuestModel
            {
                name = "1-2",
                acquireGold = 150,
                consumeStamina = 7,
                premiseQuestNames = new List<string>
                {
                    "1-1",
                }
            },
            new WeaveQuestModel
            {
                name = "1-3",
                acquireGold = 200,
                consumeStamina = 10,
                premiseQuestNames = new List<string>
                {
                    "1-2",
                }
            },
        };

        [SerializeField]
        public string keyNamespaceName = "quest";

        [SerializeField]
        public string keyName = "key";
    }
}