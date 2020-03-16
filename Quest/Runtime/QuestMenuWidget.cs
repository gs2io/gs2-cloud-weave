using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Quest
{
    [Serializable]
    public class ShowQuestMenuEvent : UnityEvent
    {
    }

    public class QuestMenuWidget : MonoBehaviour
    {
        public ShowQuestMenuEvent onShowQuestMenu = new ShowQuestMenuEvent();

        public void Initialize()
        {
            
        }
        
        public void OnClickStoryButton()
        {
            onShowQuestMenu.Invoke();
        }
    }
}