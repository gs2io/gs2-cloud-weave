using System;
using System.Linq;
using Gs2.Gs2Stamina.Request;
using Gs2.Unity.Gs2Quest.Model;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

namespace Gs2.Weave.Quest
{
    public class QuestListRow : MonoBehaviour
    {
        [SerializeField]
        public Text contents;

        [SerializeField]
        public ChoiceQuestEvent onChoiceQuest = new ChoiceQuestEvent();

        private EzQuestModel _questModel;
        private Quest _quest;

        public void Initialize(
            EzQuestModel questModel
        )
        {
            _questModel = questModel;
            _quest = new Quest(questModel);
        }
        
        public void Update()
        {
            contents.text = $"{_quest.Name} / {_quest.ConsumeStamina} Stamina";
        }

        public void OnClickStartButton()
        {
            onChoiceQuest.Invoke(_questModel);
        }
    }
}