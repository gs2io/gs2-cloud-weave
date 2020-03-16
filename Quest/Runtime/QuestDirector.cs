using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Gs2Quest.Result;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Gs2Quest.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using Gs2.Weave.Core.Watcher;
using LitJson;
using UnityEngine;
using UnityEngine.Events;
using Weave.Core.Runtime;

namespace Gs2.Weave.Quest
{
    public class QuestDirector : MonoBehaviour
    {
        [SerializeField]
        public QuestMenuWidget questMenuWidget;

        [SerializeField]
        public QuestListWidget questListWidget;

        [SerializeField]
        public QuestMain questMain;

        private Client _client;
        private GameSession _session;
        private StampSheetRunner _stampSheetRunner;
        private Dictionary<string, string> _config;

        private EzQuestModel _currentQuest;
        private EzProgress _currentProgress;
        
        private QuestWatcher _questWatcher;
        private QuestSetting _questSetting;

        public void Start()
        {
            _questWatcher = new QuestWatcher();
            _questSetting = GetComponent<QuestSetting>();
            _questSetting.onIssueStartStampSheet.AddListener(OnIssueStartStampSheet);
            _questSetting.onStartQuest.AddListener(OnStartQuest);
            _questSetting.onFindProgress.AddListener(OnStartQuest);
            _questSetting.onIssueEndStampSheet.AddListener(OnIssueEndStampSheet);
            _questSetting.onEndQuest.AddListener(OnEndQuest);
        }

        public void OnDisable()
        {
            questListWidget.onChoiceQuest.RemoveListener(OnChoiceQuest);
            questMenuWidget.onShowQuestMenu.RemoveListener(OnShowQuestMenu);
            _questSetting.onIssueStartStampSheet.RemoveListener(OnIssueStartStampSheet);
            _questSetting.onStartQuest.RemoveListener(OnStartQuest);
            _questSetting.onFindProgress.RemoveListener(OnStartQuest);
            _questSetting.onIssueEndStampSheet.RemoveListener(OnIssueEndStampSheet);
            _questSetting.onEndQuest.RemoveListener(OnEndQuest);
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            StampSheetRunner stampSheetRunner,
            Dictionary<string, string> config
        )
        {
            _client = client;
            _session = session;
            _stampSheetRunner = stampSheetRunner;
            _config = config;
            
            yield return _questWatcher.Run(
                client,
                session,
                _questSetting.questNamespaceName,
                _questSetting.questGroupName,
                _questSetting.onGetQuestModel,
                _questSetting.onFindProgress,
                _questSetting.onError
            );

            questMenuWidget.Initialize();
            questMenuWidget.onShowQuestMenu.AddListener(OnShowQuestMenu);
            questMenuWidget.gameObject.SetActive(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnShowQuestMenu()
        {
            Debug.Log("QuestDirector::OnShowQuestMenu");

            questListWidget.Initialize(
                _questWatcher
            );
            questListWidget.onChoiceQuest.AddListener(OnChoiceQuest);
            questListWidget.gameObject.SetActive(true);
        }

        public void OnChoiceQuest(EzQuestModel questModel)
        {
            Debug.Log("QuestDirector::OnChoiceQuest");

            _currentQuest = questModel;
            StartCoroutine(
                QuestController.StartQuest(
                    _client,
                    _session,
                    _questSetting.questNamespaceName,
                    _questSetting.questGroupName,
                    questModel.Name,
                    _questSetting.onIssueStartStampSheet,
                    _questSetting.onError
                )
            );
        }

        public void OnIssueStartStampSheet(
            string stampSheet
        )
        {
            Debug.Log("QuestDirector::OnIssueStartStampSheet");

            StartCoroutine(
                _stampSheetRunner.Run(
                    stampSheet,
                    _questSetting.questKeyId,
                    _questSetting.onError
                )
            );
        }

        public void OnStartQuest(EzQuestModel quest, EzProgress progress)
        {
            Debug.Log("QuestDirector::OnStartQuest");

            _currentProgress = progress;
            
            questMain.Initialize(
                new Quest(quest),
                progress
            );
            questMain.onComplete.AddListener(OnCompleteQuest);
            questMain.onFailure.AddListener(OnFailureQuest);
            questMain.gameObject.SetActive(true);
        }

        public void OnCompleteQuest(Quest questModel, EzProgress progress, List<EzReward> rewards)
        {
            Debug.Log("QuestDirector::OnCompleteQuest");

            StartCoroutine(
                QuestController.End(
                    _client,
                    _session,
                    _questSetting.questNamespaceName,
                    progress.TransactionId,
                    rewards,
                    true,
                    _questSetting.onIssueEndStampSheet,
                    _questSetting.onError,
                    _config.Select(item => new EzConfig
                    {
                        Key = item.Key,
                        Value = item.Value
                    }).ToList()
                )
            );
        }

        public void OnFailureQuest(Quest questModel, EzProgress progress)
        {
            Debug.Log("QuestDirector::OnFailureQuest");

            StartCoroutine(
                QuestController.End(
                    _client,
                    _session,
                    _questSetting.questNamespaceName,
                    progress.TransactionId,
                    progress.Rewards,
                    false,
                    _questSetting.onIssueEndStampSheet,
                    _questSetting.onError,
                    _config.Select(item => new EzConfig
                    {
                        Key = item.Key,
                        Value = item.Value
                    }).ToList()
                )
            );
        }

        public void OnIssueEndStampSheet(
            string stampSheet
        )
        {
            Debug.Log("QuestDirector::OnIssueStartStampSheet");

            IEnumerator StampSheetExecute()
            {
                yield return _stampSheetRunner.Run(
                    stampSheet,
                    _questSetting.questKeyId,
                    _questSetting.onError
                );
                
                _questSetting.onEndQuest.Invoke(_currentQuest, _currentProgress);
            }

            StartCoroutine(
                StampSheetExecute()
            );
        }

        public void OnEndQuest(EzQuestModel quest, EzProgress progress)
        {
            Debug.Log("QuestDirector::OnEndQuest");
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("QuestDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("QuestDirector::StateMachineOnDoneStampTask");
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("QuestDirector::StateMachineOnCompleteStampSheet");

                if (sheet.Action == "Gs2Quest:CreateProgressByUserId")
                {
                    var result = CreateProgressByUserIdResult.FromDict(JsonMapper.ToObject(sheetResult.Result));
                    _questSetting.onStartQuest.Invoke(_currentQuest, new EzProgress(result.item));
                    
                    StartCoroutine(
                        _questWatcher.Refresh()
                    );
                }
            };
        }
    }
}