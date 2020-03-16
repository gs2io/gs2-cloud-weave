using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.UI;

namespace Gs2.Weave.Experience
{
    public class StatusWidget : MonoBehaviour
    {
        [SerializeField]
        public DebugExperienceControlWidget debugExperienceControlWidget;

        [SerializeField]
        public Text propertyIdValue;

        [SerializeField]
        public Text rankValue;

        [SerializeField]
        public Text experienceValue;

        private string _propertyId;
        private ExperienceWatcher _experienceWatcher;

        public void Initialize(
            string propertyId,
            ExperienceWatcher experienceWatcher
        )
        {
            _propertyId = propertyId;
            _experienceWatcher = experienceWatcher;
            
            propertyIdValue.text = "PropertyId: ---";
            rankValue.text = "Rank: --";
            experienceValue.text = "Experience: --- / ---";

            if (debugExperienceControlWidget != null)
            {
                debugExperienceControlWidget.Initialize(
                    propertyId
                );
                debugExperienceControlWidget.gameObject.SetActive(true);
            }
        }
        
        void Update()
        {
            if (!_experienceWatcher.Statuses.ContainsKey(_propertyId))
            {
                var experienceModel = _experienceWatcher.ExperienceModel;
                var nextRankExperience = experienceModel.RankThreshold.Values[0];
                propertyIdValue.text = $"PropertyId: {_propertyId}";
                rankValue.text = $"Rank: 1";
                experienceValue.text = $"Experience: 0 / {nextRankExperience}";
            }
            else
            {
                var status = _experienceWatcher.Statuses[_propertyId];
                var experienceModel = _experienceWatcher.ExperienceModel;
                propertyIdValue.text = $"PropertyId: {status.PropertyId}";
                rankValue.text = $"Rank: {status.RankValue}";
                if (status.RankCapValue <= status.RankValue)
                {
                    var nextRankExperience = experienceModel.RankThreshold.Values[(int)status.RankValue - 2];
                    experienceValue.text = $"Experience: {status.ExperienceValue} / {nextRankExperience}";
                }
                else
                {
                    var nextRankExperience = experienceModel.RankThreshold.Values[(int)status.RankValue - 1];
                    experienceValue.text = $"Experience: {status.ExperienceValue} / {nextRankExperience}";
                }
            }
        }

        public void OnClickCloseButton()
        {
            gameObject.SetActive(false);
            if (debugExperienceControlWidget != null)
            {
                debugExperienceControlWidget.gameObject.SetActive(false);
            }
        }
    }
}