using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Unity;
using Gs2.Unity.Gs2Formation.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Core.Watcher
{
    [Serializable]
    public class WatchFormationEvent : UnityEvent<EzMoldModel, int, EzForm>
    {
    }

    [Serializable]
    public class DoneSetupEvent : UnityEvent
    {
    }

    public class FormationWatcher
    {
        public WatchFormationEvent onWatchFormation = new WatchFormationEvent();
        public DoneSetupEvent onDoneSetup = new DoneSetupEvent();

        private bool _watching;

        private Client _client;
        private GameSession _session;
        private EzMoldModel _moldModel;
        private List<EzForm> _forms = new List<EzForm>();

        public EzMoldModel MoldModel => _moldModel;
        public List<EzForm> Forms => _forms;

        private string _formationNamespaceName;
        private string _moldModelName;
        private GetMoldModelEvent _onGetMoldModel;
        private GetFormEvent _onGetForm;
        private UpdateFormEvent _onUpdateForm;
        private ErrorEvent _onError;

        private void GetFormAction(
            EzMoldModel moldModelTemp, 
            int index, 
            EzForm form
        )
        {
            if (moldModelTemp.Name != _moldModel.Name)
            {
                return;
            }
            
            while (_forms.Count <= index)
            {
                _forms.Add(null);
            }
            _forms[index] = form;
            
            onWatchFormation.Invoke(_moldModel, index, form);
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            string formationNamespaceName,
            string moldModelName,
            GetMoldModelEvent onGetMoldModel,
            GetFormEvent onGetForm,
            UpdateFormEvent onUpdateForm,
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
            _formationNamespaceName = formationNamespaceName;
            _moldModelName = moldModelName;
            _onGetMoldModel = onGetMoldModel;
            _onGetForm = onGetForm;
            _onUpdateForm = onUpdateForm;
            _onError = onError;
            
            void GetMoldModelAction(
                string formationNamespaceNameTmp, 
                EzMoldModel moldModel
            )
            {
                if (formationNamespaceNameTmp != _formationNamespaceName)
                {
                    return;
                }
                
                _moldModel = moldModel;
                
                _onGetMoldModel.RemoveListener(GetMoldModelAction);
            }
            
            _onGetMoldModel.AddListener(GetMoldModelAction);

            yield return FormationController.GetMoldModel(
                _client,
                formationNamespaceName,
                moldModelName,
                onGetMoldModel,
                onError
            );
            
            _onGetForm.AddListener(GetFormAction);

            yield return Refresh();
        }

        public IEnumerator Refresh()
        {
            yield return FormationController.ListForms(
                _client,
                _session,
                _formationNamespaceName,
                _moldModel,
                _onGetForm,
                _onError
            );
            
            onDoneSetup.Invoke();
        }

        public void Stop()
        {
            if (!_watching) return;
            
            _onGetForm.RemoveListener(GetFormAction);
            
            _watching = false;
        }
    }
}