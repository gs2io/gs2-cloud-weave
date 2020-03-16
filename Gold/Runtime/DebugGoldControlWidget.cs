using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Gs2.Core.Model;
using Gs2.Core.Net;
using Gs2.Gs2Inventory;
using Gs2.Gs2Inventory.Model;
using Gs2.Gs2Inventory.Request;
using Gs2.Gs2Money;
using Gs2.Gs2Money.Request;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;
using UnityEngine.Events;
using Weave.Core.Runtime;

namespace Gs2.Weave.Gold
{
    [Serializable]
    public class ClickAcquireButtonEvent : UnityEvent<int>
    {
        
    }

    [Serializable]
    public class ClickConsumeButtonEvent : UnityEvent<int>
    {
        
    }

    public class DebugGoldControlWidget : MonoBehaviour
    {
        public ClickAcquireButtonEvent onClickAcquireButton = new ClickAcquireButtonEvent();
        public ClickConsumeButtonEvent onClickConsumeButton = new ClickConsumeButtonEvent();
        
        public int acquireCount = 1000;
        
        public int consumeCount = 100;

        public void Initialize()
        {
            
        }
        
        public void OnClickAcquireButton()
        {
            onClickAcquireButton.Invoke(
                acquireCount
            );
        }
        
        public void OnClickConsumeButton()
        {
            onClickConsumeButton.Invoke(
                consumeCount
            );
        }
    }
}