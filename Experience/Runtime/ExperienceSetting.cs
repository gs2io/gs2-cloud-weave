﻿using System;
using System.Collections.Generic;
using Gs2.Core.Exception;
using Gs2.Gs2Exchange.Model;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Exchange.Model;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Unity.Gs2Stamina.Model;
using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;
using UnityEngine.Events;
using EzAcquireAction = Gs2.Unity.Gs2Exchange.Model.EzAcquireAction;
using EzConsumeAction = Gs2.Unity.Gs2Exchange.Model.EzConsumeAction;

namespace Gs2.Weave.Experience
{
    [System.Serializable]
    public class ExperienceSetting : MonoBehaviour
    {
        [SerializeField]
        public string experienceNamespaceName;
        
        [SerializeField]
        public string experienceModelName;

        [SerializeField]
        public string identifierIncreaseExperienceClientId;

        [SerializeField]
        public string identifierIncreaseExperienceClientSecret;

        [SerializeField]
        public GetExperienceModelEvent onGetExperienceModel = new GetExperienceModelEvent();
        
        [SerializeField]
        public GetStatusesEvent onGetStatuses = new GetStatusesEvent();

        [SerializeField]
        public IncreaseExperienceEvent onIncreaseExperience = new IncreaseExperienceEvent();

        [SerializeField]
        public IncreaseRankEvent onIncreaseRank = new IncreaseRankEvent();

        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}