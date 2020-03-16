using System;
using System.Collections.Generic;
using Gs2.Gs2Stamina.Model;
using Gs2.Unity.Gs2Stamina.Model;
using UnityEngine;

namespace Gs2.Weave.Stamina
{
    [Serializable]
    public class StaminaInstaller : MonoBehaviour
    {
        [SerializeField]
        public string staminaNamespaceName = "stamina";

        [SerializeField]
        public string staminaModelName = "stamina";

        [SerializeField]
        public int capacity = 50;
        
        [SerializeField]
        public int recoverValue = 1;
        
        [SerializeField]
        public int recoverIntervalMinutes = 1;
    }
}