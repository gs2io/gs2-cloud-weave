using System;
using System.Collections.Generic;
using Gs2.Gs2Formation.Model;
using Gs2.Gs2Inventory.Model;
using Gs2.Weave.Core;
using Gs2.Util.LitJson;
using UnityEngine;

namespace Gs2.Weave.Party
{
    [Serializable]
    public class PartyInstaller : MonoBehaviour
    {
        [SerializeField]
        public string formationNamespaceName = "party";

        [SerializeField]
        public string moldModelName = "party";

        [SerializeField]
        public string formModelName = "party";

        [SerializeField]
        public string keyNamespaceName = "party";

        [SerializeField]
        public string keyName = "key";
        
        [SerializeField]
        public int numberOfSaveArea = 10;
        
        [SerializeField]
        public int numberOfUnit = 5;
    }
}