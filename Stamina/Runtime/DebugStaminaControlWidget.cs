using Gs2.Unity.Gs2Stamina.Model;
using UnityEngine;
using UnityEngine.Events;
using Weave.Core.Runtime;

namespace Gs2.Weave.Stamina
{
    public class ClickConsumeButtonEvent : UnityEvent<int>
    {
        
    }

    public class DebugStaminaControlWidget : MonoBehaviour
    {
        public int consumeCount = 5;
        
        public ClickConsumeButtonEvent onClickConsumeButton = new ClickConsumeButtonEvent();

        public void Initialize()
        {
            
        }
        
        public void OnClickConsumeButton()
        {
            onClickConsumeButton.Invoke(consumeCount);
        }
    }
}