using System;
using System.Collections.Generic;
using Gs2.Unity.Gs2Inventory.Model;
using UnityEngine.Events;

namespace Gs2.Weave.Core.CallbackEvent
{
    [Serializable]
    public class GetInventoryModelEvent : UnityEvent<string, EzInventoryModel, List<EzItemModel>>
    {
    }

    [Serializable]
    public class GetInventoryEvent : UnityEvent<EzInventory, List<EzItemSet>>
    {
    }

    [Serializable]
    public class GetItemSetWithSignatureEvent : UnityEvent<string, string, string, string>
    {
    }

    [Serializable]
    public class AcquireEvent : UnityEvent<EzInventory, List<EzItemSet>, int>
    {
    }

    [Serializable]
    public class ConsumeEvent : UnityEvent<EzInventory, List<EzItemSet>, int>
    {
    }

}