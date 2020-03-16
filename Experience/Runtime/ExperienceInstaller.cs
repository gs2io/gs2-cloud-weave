using System;
using System.Collections.Generic;
using Gs2.Weave.Core;
using UnityEngine;

namespace Gs2.Weave.Experience
{
    [Serializable]
    public class ExperienceInstaller : MonoBehaviour
    {
        [SerializeField]
        public string experienceNamespaceName = "experience";

        [SerializeField]
        public string experienceModelName = "unit";

        [SerializeField] 
        public List<long> threshold = new List<long>
        {
            100,
            300,
            1000,
            3000,
            10000,
        };
        
        [SerializeField] 
        public int levelCap = 6;
        
        [SerializeField]
        public bool enableDebugIncreaseExperienceAction = true;

        [SerializeField]
        [DrawIf("enableDebugIncreaseExperienceAction", true, ComparisonType.Equals)]
        public string identifierIncreaseExperiencePolicyName = "experience-increase-experience";

        [SerializeField]
        [DrawIf("enableDebugIncreaseExperienceAction", true, ComparisonType.Equals)]
        public string identifierIncreaseExperienceUserName = "experience-increase-experience";
    }
}