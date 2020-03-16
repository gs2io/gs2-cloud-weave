using System;
using UnityEngine;

namespace Gs2.Weave.JobQueue
{
    [Serializable]
    public class JobQueueInstaller : MonoBehaviour
    {
        /// <summary>
        /// GS2-JobQueue のネームスペース名
        /// </summary>
        [SerializeField]
        public string jobQueueNamespaceName = "job-queue";

    }
}