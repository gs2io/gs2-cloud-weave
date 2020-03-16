using System;
using System.Collections;
using Gs2.Core.Exception;
using Gs2.Unity;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;
using UnityEngine.Events;

namespace Gs2.Weave.Core.Watcher
{
    public class WatchPushJobEvent : UnityEvent
    {
        
    }

    public class WatchRunJobEvent : UnityEvent<EzJob, EzJobResultBody>
    {
        
    }

    public class JobQueueWatcher
    {
        public WatchPushJobEvent onWatchPushJob = new WatchPushJobEvent();
        public WatchRunJobEvent onWatchRunJob = new WatchRunJobEvent();
        
        private bool _watching;

        private Client _client;
        private GameSession _session;
        private string _jobQueueNamespaceName;

        private RunJobEvent _onRunJob;
        private ErrorEvent _onError;
        
        void OnRunJob(EzJob job, EzJobResultBody body, bool isLastJob)
        {
            if (!isLastJob)
            {
                onWatchPushJob.Invoke();
            }

            if (job != null)
            {
                onWatchRunJob.Invoke(job, body);
            }
        }

        void OnError(Gs2Exception e)
        {
            onWatchPushJob.Invoke();
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            string jobQueueNamespaceName,
            RunJobEvent onRunJob,
            ErrorEvent onError
        )
        {
            if (_watching)
            {
                throw new InvalidOperationException("already started");
            }
            
            _watching = true;

            _client = client;
            _session = session;
            
            _jobQueueNamespaceName = jobQueueNamespaceName;

            _onRunJob = onRunJob;
            _onError = onError;

            _onRunJob.AddListener(OnRunJob);
            _onError.AddListener(OnError);

            yield return Execute();
        }

        public IEnumerator Execute()
        {
            yield return JobQueueController.Run(
                _client,
                _session,
                _jobQueueNamespaceName,
                _onRunJob,
                _onError
            );
        }

        public void Stop()
        {
            if (_watching)
            {
                throw new InvalidOperationException("not started");
            }
        
            _onRunJob.RemoveListener(OnRunJob);
            _onError.RemoveListener(OnError);
            
            _watching = false;
        }
    }
}