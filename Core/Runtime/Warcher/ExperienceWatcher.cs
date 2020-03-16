using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Core.Util;
using Gs2.Unity;
using Gs2.Unity.Gs2Experience.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Core.Watcher
{
    public class ExperienceWatcher
    {
        private bool _watching;

        private Client _client;
        private GameSession _session;
        private EzExperienceModel _experienceModel;
        private Dictionary<string, EzStatus> _statuses;

        public EzExperienceModel ExperienceModel => _experienceModel;
        public Dictionary<string, EzStatus> Statuses => _statuses;

        private Func<IEnumerator, Coroutine> _startCoroutine;
        private string _experienceNamespaceName;
        private string _experienceModelName;
        private GetExperienceModelEvent _onGetExperienceModel;
        private GetStatusesEvent _onGetStatuses;
        private IncreaseExperienceEvent _onIncreaseExperience;
        private ErrorEvent _onError;

        public void OnIncreaseExperience(
            EzExperienceModel experienceModel,
            EzStatus status,
            int value
        )
        {
            if (_experienceModelName != experienceModel.Name)
            {
                return;
            }

            _startCoroutine(Refresh());
        }
        
        public IEnumerator Run(
            Client client,
            GameSession session,
            Func<IEnumerator, Coroutine> startCoroutine,
            string experienceNamespaceName,
            string experienceModelName,
            GetExperienceModelEvent onGetExperienceModel,
            GetStatusesEvent onGetStatuses,
            IncreaseExperienceEvent onIncreaseExperience,
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
            _startCoroutine = startCoroutine;
            _experienceNamespaceName = experienceNamespaceName;
            _experienceModelName = experienceModelName;
            _onGetExperienceModel = onGetExperienceModel;
            _onGetStatuses = onGetStatuses;
            _onIncreaseExperience = onIncreaseExperience;
            _onError = onError;
            
            void GetExperienceModelAction(
                string experienceModelNameTemp,
                EzExperienceModel experienceModel
            )
            {
                if (_experienceModelName != experienceModelNameTemp)
                {
                    return;
                }
                
                _experienceModel = experienceModel;
                
                _onGetExperienceModel.RemoveListener(GetExperienceModelAction);
            }

            _onGetExperienceModel.AddListener(GetExperienceModelAction);

            yield return ExperienceController.GetExperienceModel(
                _client,
                _experienceNamespaceName,
                _experienceModelName,
                _onGetExperienceModel,
                _onError
            );
            
            _onIncreaseExperience.AddListener(OnIncreaseExperience);

            yield return Refresh();
        }

        public IEnumerator Refresh()
        {
            void RefreshStatuesAction(
                EzExperienceModel experienceModelTemp, 
                List<EzStatus> statuses
            )
            {
                if (experienceModelTemp.Name != _experienceModel.Name)
                {
                    return;
                }
                
                _experienceModel = experienceModelTemp;
                _statuses = statuses.ToDictionary(status => status.PropertyId);
                
                _onGetStatuses.RemoveListener(RefreshStatuesAction);
            }

            _onGetStatuses.AddListener(RefreshStatuesAction);
            
            yield return ExperienceController.GetStatuses(
                _client,
                _session,
                _experienceNamespaceName,
                _experienceModel,
                _onGetStatuses,
                _onError
            );
        }

        public void Stop()
        {
            if (!_watching) return;
            
            _onIncreaseExperience.RemoveListener(OnIncreaseExperience);

            _watching = false;
        }
    }
}