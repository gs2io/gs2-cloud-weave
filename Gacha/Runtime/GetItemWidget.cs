using System.Collections.Generic;
using System.Linq;
using Gs2.Gs2Inventory.Request;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.UI;

namespace Gs2.Weave.Gacha
{
    public class GetItemWidget : MonoBehaviour
    {
        public Text itemName;
        
        public void Initialize(
            InventoryWatcher watcher,
            List<AcquireItemSetByUserIdRequest> requests
        )
        {
            itemName.text = "";
            foreach (var request in requests)
            {
                var itemModel = watcher.ItemModels.First(model => model.Name == request.itemName);
                itemName.text += $"{itemModel.Name} x {request.acquireCount} 手に入れた\n";
            }
        }
        
        public void OnClickCloseButton()
        {
            gameObject.SetActive(false);
        }
    }
}