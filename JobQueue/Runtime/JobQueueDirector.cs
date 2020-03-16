using System.Collections;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.JobQueue
{
    public class JobQueueDirector : MonoBehaviour
    {
        private JobQueueWatcher _jobQueueWatcher;
        private JobQueueSetting _jobQueueSetting;

        private Client _client;

        public JobQueueWatcher Watcher => _jobQueueWatcher;

        public void Start()
        {
            _jobQueueWatcher = new JobQueueWatcher();
            _jobQueueSetting = GetComponent<JobQueueSetting>();
        }

        void OnPushJob()
        {
            StartCoroutine(
                _jobQueueWatcher.Execute()
            );
        }

        public void OnDisable()
        {
            _jobQueueWatcher.onWatchPushJob.AddListener(OnPushJob);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="session"></param>
        public IEnumerator Run(
            Client client,
            GameSession session
        )
        {
            Debug.Log("JobQueueDirector::Run");

            _client = client;

            _jobQueueWatcher.onWatchPushJob.AddListener(OnPushJob);
            
            StartCoroutine(
                _jobQueueWatcher.Run(
                    client,
                    session,
                    _jobQueueSetting.jobQueueNamespaceName,
                    _jobQueueSetting.onRunJob,
                    _jobQueueSetting.onError
                )
            );

            yield break;
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("JobQueueDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("JobQueueDirector::StateMachineOnDoneStampTask");
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("JobQueueDirector::StateMachineOnCompleteStampSheet");

                if (sheet.Action == "Gs2JobQueue:PushByUserId")
                {
                    _jobQueueWatcher.onWatchPushJob.Invoke();
                }
            };
        }
    }
}