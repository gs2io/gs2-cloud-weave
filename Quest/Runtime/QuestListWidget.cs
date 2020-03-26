using System;
using System.Linq;
using Gs2.Unity.Gs2Quest.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Quest
{
    [Serializable]
    public class ChoiceQuestEvent : UnityEvent<EzQuestModel>
    {
    }

    public class QuestListWidget : MonoBehaviour
    {
        [SerializeField]
        public QuestListRow questListRowPrefab;

        [SerializeField]
        public VerticalLayoutGroup verticalLayoutGroup;

        [SerializeField]
        public ChoiceQuestEvent onChoiceQuest = new ChoiceQuestEvent();

        private QuestWatcher _watcher;

        public void Initialize(
            QuestWatcher watcher
        )
        {
            _watcher = watcher;
            _watcher.onWatchQuestEvent.AddListener(
                OnGetQuestList
            );
            
            OnGetQuestList(_watcher.QuestGroupModel, _watcher.CompletedQuestList);
        }

        public void OnDisable()
        {
            _watcher.onWatchQuestEvent.RemoveListener(
                OnGetQuestList
            );
        }
        
        public void OnGetQuestList(EzQuestGroupModel questGroupModel, EzCompletedQuestList completedQuestList)
        {
            for (var i = 0; i < verticalLayoutGroup.transform.childCount; i++)
            {
                Destroy(verticalLayoutGroup.transform.GetChild(i).gameObject);
            }

            foreach (var quest in questGroupModel.Quests)
            {
                if (quest.PremiseQuestNames.Intersect(_watcher.CompletedQuestList.CompleteQuestNames).Count() ==
                    quest.PremiseQuestNames.Count)
                {
                    var item = Instantiate(questListRowPrefab, verticalLayoutGroup.transform);
                    item.Initialize(
                        quest
                    );
                    item.onChoiceQuest.AddListener(OnClickChoiceQuestButton);
                }
            }
        }
        
        public void OnClickChoiceQuestButton(EzQuestModel quest)
        {
            onChoiceQuest.Invoke(quest);
            
            gameObject.SetActive(false);
        }

        public void OnCloseButton()
        {
            gameObject.SetActive(false);
        }
    }
}