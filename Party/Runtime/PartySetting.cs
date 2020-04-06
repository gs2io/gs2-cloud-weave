﻿using System;
 using Gs2.Unity.Gs2Formation.Model;
 using Gs2.Unity.Gs2Inventory.Model;
 using Gs2.Unity.Util;
 using Gs2.Weave.Core.CallbackEvent;
 using UnityEngine;
 using UnityEngine.Events;

 namespace Gs2.Weave.Party
{
    [Serializable]
    public class ChangeFormationEvent : UnityEvent<EzMoldModel, int, int, EzItemSet>
    {
        
    }

    [Serializable]
    public class PartySetting : MonoBehaviour
    {
        [SerializeField]
        public string formationNamespaceName;

        [SerializeField]
        public string moldModelName;

        [SerializeField]
        public string formModelName;

        [SerializeField]
        public string partyKeyId;

        [SerializeField]
        public GetMoldModelEvent onGetMoldModelModel = new GetMoldModelEvent();

        [SerializeField]
        public GetFormEvent onGetForm = new GetFormEvent();

        [SerializeField]
        public ChangeFormationEvent onChangeFormation = new ChangeFormationEvent();

        [SerializeField]
        public UpdateFormEvent onUpdateForm = new UpdateFormEvent();

        [SerializeField]
        public ErrorEvent onError = new ErrorEvent();
    }
}