﻿using System;
 using System.Collections.Generic;
 using System.Linq;
 using Gs2.Gs2Inventory.Model;
 using Gs2.Weave.Core;
 using UnityEngine;

 namespace Gs2.Weave.Inventory
{
    [Serializable]
    public class WeaveItemModel
    {
        public string name;
        public int stackingLimit;
        public bool allowMultipleStacks;
        public int sortValue;
        
        public ItemModel ToModel()
        {
            return new ItemModel
            {
                name = name,
                stackingLimit = stackingLimit,
                allowMultipleStacks = allowMultipleStacks,
                sortValue = sortValue,
            };
        }
    }
    
    [Serializable]
    public class WeaveInventoryModel
    {
        public string name;
        public int initialCapacity;
        public int maxCapacity;
        public List<WeaveItemModel> itemModels = new List<WeaveItemModel>();

        public InventoryModel ToModel()
        {
            return new InventoryModel
            {
                name = name,
                initialCapacity = initialCapacity,
                maxCapacity = maxCapacity,
                itemModels = itemModels.Select(itemModel => itemModel.ToModel()).ToList(),
            };
        }
    }
    
    [Serializable]
    public class InventoryInstaller : MonoBehaviour
    {
        [SerializeField]
        public string inventoryNamespaceName = "inventory";

        [SerializeField]
        public WeaveInventoryModel inventoryModel = new WeaveInventoryModel
        {
            name = "inventory",
            initialCapacity = 50,
            maxCapacity = 100,
            itemModels = new List<WeaveItemModel>
            {
                new WeaveItemModel
                {
                    name = "fire_element",
                    stackingLimit = 99,
                    allowMultipleStacks = true,
                    sortValue = 0
                },
                new WeaveItemModel
                {
                    name = "water_element",
                    stackingLimit = 99,
                    allowMultipleStacks = true,
                    sortValue = 1
                },
            }
        };
        
        [SerializeField]
        public bool enableDebugAcquireItemAction = true;

        [SerializeField]
        [DrawIf("enableDebugAcquireItemAction", true, ComparisonType.Equals)]
        public string identifierAcquireItemPolicyName = "inventory-acquire-item";

        [SerializeField]
        [DrawIf("enableDebugAcquireItemAction", true, ComparisonType.Equals)]
        public string identifierAcquireItemUserName = "inventory-acquire-item";
    }
}