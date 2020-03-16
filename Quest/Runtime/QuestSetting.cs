﻿using System;
using System.Collections.Generic;
using Gs2.Core.Exception;
using Gs2.Gs2Quest.Model;
using Gs2.Unity.Gs2Quest.Model;
 using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Quest
{
    [Serializable]
    public class QuestSetting : MonoBehaviour
    {
        [SerializeField]
        public string questNamespaceName;

        [SerializeField]
        public string questGroupName;

        [SerializeField]
        public string questKeyId;

        [SerializeField]
        public GetQuestModelEvent onGetQuestModel = new GetQuestModelEvent();

        [SerializeField]
        public FindProgressEvent onFindProgress = new FindProgressEvent();
        
        [SerializeField]
        public IssueStartStampSheetEvent onIssueStartStampSheet = new IssueStartStampSheetEvent();

        [SerializeField]
        public StartQuestEvent onStartQuest = new StartQuestEvent();

        [SerializeField]
        public IssueEndStampSheetEvent onIssueEndStampSheet = new IssueEndStampSheetEvent();

        [SerializeField]
        public EndQuestEvent onEndQuest = new EndQuestEvent();

        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}