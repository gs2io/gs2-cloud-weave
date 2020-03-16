﻿using Gs2.Core.Exception;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.JobQueue
{
    [System.Serializable]
    public class JobQueueSetting : MonoBehaviour
    {
        /// <summary>
        /// GS2-JobQueue のネームスペース名
        /// </summary>
        [SerializeField]
        public string jobQueueNamespaceName;

        /// <summary>
        /// GS2-JobQueue のネームスペースID
        /// </summary>
        [SerializeField]
        public string jobQueueNamespaceId;

        /// <summary>
        /// ジョブキュー実行時に発行されるイベント
        /// </summary>
        public RunJobEvent onRunJob;
        
        /// <summary>
        /// エラー発生時に発行されるイベント
        /// </summary>
        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}