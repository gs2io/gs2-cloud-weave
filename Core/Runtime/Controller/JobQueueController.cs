using System;
using System.Collections;
using Gs2.Core;
using Gs2.Unity;
using Gs2.Unity.Gs2JobQueue.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class JobQueueController: MonoBehaviour
    {
        public static IEnumerator Run(
            Client client,
            GameSession gameSession,
            string jobQueueNamespaceName,
            RunJobEvent onRunJob,
            ErrorEvent onError
        )
        {
            AsyncResult<EzRunResult> result = null;
            yield return client.JobQueue.Run(
                r => { result = r; },
                gameSession,
                jobQueueNamespaceName
            );

            if (result.Error != null)
            {
                Debug.LogError(result.Error);
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var job = result.Result.Item;
            var jobResult = result.Result.Result;
            var isLastJob = result.Result.IsLastJob;

            Debug.Log(isLastJob);
            onRunJob.Invoke(job, jobResult, isLastJob);
            
            yield return new WaitForSeconds(1);
        }
    }
}