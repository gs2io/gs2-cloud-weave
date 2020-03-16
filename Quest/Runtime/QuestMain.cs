using System;
using System.Collections.Generic;
using System.Linq;
using Gs2.Gs2Inventory.Request;
using Gs2.Unity.Gs2Quest.Model;
using LitJson;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Quest
{
    [Serializable]
    public class CompleteEvent : UnityEvent<Quest, EzProgress, List<EzReward>>
    {
    }

    [Serializable]
    public class FailureEvent : UnityEvent<Quest, EzProgress>
    {
    }

    public class QuestMain : MonoBehaviour
    {
        public Slider goldSlider;
        public Text goldAmount;

        public CompleteEvent onComplete = new CompleteEvent();
        public FailureEvent onFailure = new FailureEvent();

        private Quest _quest;
        private EzProgress _progress;
        private List<EzReward> _rewards;
        
        public void Initialize(
            Quest quest,
            EzProgress progress
        )
        {
            _quest = quest;
            _progress = progress;
            _rewards = progress.Rewards;

            goldSlider.value = goldSlider.maxValue = GetAcquireGold(_rewards);
        }

        private int GetAcquireGold(List<EzReward> rewards)
        {
            return rewards.First(reward => reward.Action == "Gs2Inventory:AcquireItemSetByUserId").Value;
        }

        public void OnChangeGoldSlider(float value)
        {
            goldAmount.text = ((long)value).ToString();
        }

        public void OnClickCompleteButton()
        {
            _rewards.First(reward => reward.Action == "Gs2Inventory:AcquireItemSetByUserId").Value = (int) goldSlider.value;
            
            onComplete.Invoke(_quest, _progress, _rewards);
            
            gameObject.SetActive(false);
        }

        public void OnClickFailureButton()
        {
            onFailure.Invoke(_quest, _progress);
            
            gameObject.SetActive(false);
        }
    }
}