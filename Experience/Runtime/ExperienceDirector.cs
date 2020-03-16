using System.Collections;
using System.Collections.Generic;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using Weave.Core.Runtime;

namespace Gs2.Weave.Experience
{
    public class ExperienceDirector : MonoBehaviour
    {
        [SerializeField]
        public StatusWidget statusWidget;

        private Client _client;
        private GameSession _session;
        private StampSheetRunner _stampSheetRunner;
        private Dictionary<string, string> _config;
        
        private ExperienceWatcher _experienceWatcher;
        private ExperienceSetting _experienceSetting;

        public ExperienceWatcher Watcher => _experienceWatcher;

        public void Start()
        {
            _experienceWatcher = new ExperienceWatcher();
            _experienceSetting = GetComponent<ExperienceSetting>();
        }

        public void OnDestroy()
        {
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            StampSheetRunner stampSheetRunner,
            Dictionary<string, string> config
        )
        {
            Debug.Log("ExperienceDirector::Run");

            _client = client;
            _session = session;
            _stampSheetRunner = stampSheetRunner;
            _config = config;
            
            yield return _experienceWatcher.Run(
                client,
                session,
                StartCoroutine,
                _experienceSetting.experienceNamespaceName,
                _experienceSetting.experienceModelName,
                _experienceSetting.onGetExperienceModel,
                _experienceSetting.onGetStatuses,
                _experienceSetting.onIncreaseExperience,
                _experienceSetting.onError
            );
        }

        public void OnIncreaseExperience(
            string propertyId,
            int value
        )
        {
            if (string.IsNullOrEmpty(_experienceSetting.identifierIncreaseExperienceClientId) ||
                string.IsNullOrEmpty(_experienceSetting.identifierIncreaseExperienceClientSecret))
            {
                Debug.LogError("インストーラーの設定でデバッグ機能が無効化されています");
            }

            StartCoroutine(
                ExperienceController.IncreaseExperience(
                    _session,
                    _experienceSetting.identifierIncreaseExperienceClientId,
                    _experienceSetting.identifierIncreaseExperienceClientSecret,
                    _experienceSetting.experienceNamespaceName,
                    _experienceWatcher.ExperienceModel,
                    propertyId,
                    value,
                    _experienceSetting.onIncreaseExperience,
                    _experienceSetting.onError
                )
            );
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("ExperienceDirector::GetJobQueueAction");
                
                if (job.ScriptId.EndsWith(
                    "system:script:general:script:execute_experience_add_experience_by_user_id"
                ))
                {
                    StartCoroutine(
                        _experienceWatcher.Refresh()
                    );
                }
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("ExperienceDirector::StateMachineOnDoneStampTask");
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("ExperienceDirector::StateMachineOnCompleteStampSheet");
                
                if (sheet.Action == "Gs2Experience:AddExperienceByUserId")
                {
                    StartCoroutine(
                        _experienceWatcher.Refresh()
                    );
                }
            };
        }
        
        public void OnShowStatusWidget(string propertyId)
        {
            statusWidget.Initialize(
                propertyId,
                _experienceWatcher
            );
            statusWidget.gameObject.SetActive(true);
        }
    }
}