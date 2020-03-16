using System.Collections.Generic;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.UI;

public class GoldWidget : MonoBehaviour
{
    [SerializeField]
    public Text amount;

    private InventoryWatcher _watcher;

    public void Initialize(
        InventoryWatcher watcher
    )
    {
        _watcher = watcher;
    }
    
    public void OnChangeGold(
        EzInventory inventory,
        List<EzItemSet> itemSet
    )
    {
        if (_watcher.ItemSets == null || _watcher.ItemSets.Count == 0)
        {
            amount.text = "0 G";
        }
        else
        {
            amount.text = $"{_watcher.ItemSets[0].Count} G";
        }
    }

    public void Start()
    {
        _watcher.onWatchInventoryEvent.AddListener(OnChangeGold);
    }

    public void OnDestroy()
    {
        _watcher.onWatchInventoryEvent.RemoveListener(OnChangeGold);
    }
}
