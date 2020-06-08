using System;
using System.Collections.Generic;
using Gs2.Gs2Inventory.Model;
using Gs2.Weave.Core;
using Gs2.Util.LitJson;
using UnityEngine;

namespace Gs2.Weave.Unit
{
    [Serializable]
    public class Metadata
    {
        [SerializeField]
        public string displayName;
        [SerializeField]
        public int rarity;
    }
    
    [Serializable]
    public class Unit
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public Metadata metadata;
        [SerializeField]
        public int sortValue;

        public ItemModel ToModel()
        {
            return new ItemModel
            {
                name = name,
                metadata = JsonMapper.ToJson(metadata),
                allowMultipleStacks = true,
                stackingLimit = 1,
                sortValue = sortValue,
            };
        }
    }
    
    [Serializable]
    public class UnitInstaller : MonoBehaviour
    {
        [SerializeField]
        public string inventoryNamespaceName = "unit";

        [SerializeField]
        public string inventoryModelName = "unit";

        [SerializeField]
        public int capacity = 50;

        [SerializeField]
        public List<Unit> units = new List<Unit>
        {
            new Unit
            {
                name = "normal_0001",
                metadata = new Metadata
                {
                    displayName = "[N] 0001",
                    rarity = 0,
                },
                sortValue = 10000,
            },
            new Unit
            {
                name = "normal_0002",
                metadata = new Metadata
                {
                    displayName = "[N] 0002",
                    rarity = 0,
                },
                sortValue = 10100,
            },
            new Unit
            {
                name = "normal_0003",
                metadata = new Metadata
                {
                    displayName = "[N] 0003",
                    rarity = 0,
                },
                sortValue = 10200,
            },
            new Unit
            {
                name = "rare_0001",
                metadata = new Metadata
                {
                    displayName = "[R] 0001",
                    rarity = 1,
                },
                sortValue = 20000,
            },
            new Unit
            {
                name = "rare_0002",
                metadata = new Metadata
                {
                    displayName = "[R] 0002",
                    rarity = 1,
                },
                sortValue = 20100,
            },
            new Unit
            {
                name = "rare_0003",
                metadata = new Metadata
                {
                    displayName = "[R] 0003",
                    rarity = 1,
                },
                sortValue = 20200,
            },
            new Unit
            {
                name = "srare_0001",
                metadata = new Metadata
                {
                    displayName = "[SR] 0001",
                    rarity = 2,
                },
                sortValue = 30000,
            },
            new Unit
            {
                name = "srare_0002",
                metadata = new Metadata
                {
                    displayName = "[SR] 0002",
                    rarity = 2,
                },
                sortValue = 30100,
            },
            new Unit
            {
                name = "srare_0003",
                metadata = new Metadata
                {
                    displayName = "[SR] 0003",
                    rarity = 2,
                },
                sortValue = 30200,
            },
            new Unit
            {
                name = "ssrare_0001",
                metadata = new Metadata
                {
                    displayName = "[SSR] 0001",
                    rarity = 3,
                },
                sortValue = 40000,
            },
            new Unit
            {
                name = "ssrare_0002",
                metadata = new Metadata
                {
                    displayName = "[SSR] 0002",
                    rarity = 3,
                },
                sortValue = 40100,
            },
            new Unit
            {
                name = "ssrare_0003",
                metadata = new Metadata
                {
                    displayName = "[SSR] 0003",
                    rarity = 3,
                },
                sortValue = 40200,
            },
        };

        [SerializeField]
        public bool enableDebugAcquireUnitAction = true;

        [SerializeField]
        [DrawIf("enableDebugAcquireUnitAction", true, ComparisonType.Equals)]
        public string identifierAcquireUnitPolicyName = "inventory-acquire-unit";

        [SerializeField]
        [DrawIf("enableDebugAcquireUnitAction", true, ComparisonType.Equals)]
        public string identifierAcquireUnitUserName = "inventory-acquire-unit";
    }
}