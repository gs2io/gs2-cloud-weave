using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Gs2Stamina.Request;
using Gs2.Unity;
using Gs2.Unity.Gs2Quest.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;
using LitJson;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Core.Watcher
{
    [Serializable]
    public class WatchQuestEvent : UnityEvent<EzQuestGroupModel, EzCompletedQuestList>
    {
    }

    public class QuestWatcher
    {
        public WatchQuestEvent onWatchQuestEvent = new WatchQuestEvent();

        private bool _watching;

        private Client _client;
        private GameSession _session;

        private string _questNamespaceName;
        private string _questGroupName;
        private GetQuestModelEvent _onGetQuestModel;
        private FindProgressEvent _onFindProgress;
        private ErrorEvent _onError;

        public EzQuestGroupModel QuestGroupModel { get; private set; }
        
        public EzCompletedQuestList CompletedQuestList { get; private set; }

        public List<EzQuestModel> Quests => QuestGroupModel.Quests;
        
        public IEnumerator Run(
            Client client,
            GameSession session,
            string questNamespaceName,
            string questGroupName,
            GetQuestModelEvent onGetQuestModel,
            FindProgressEvent onFindProgress,
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

            _questNamespaceName = questNamespaceName;
            _questGroupName = questGroupName;
            _onGetQuestModel = onGetQuestModel;
            _onFindProgress = onFindProgress;
            _onError = onError;

            yield return Refresh();

            yield return QuestController.GetProgress(
            
                _client,
                _session,
                _questNamespaceName,
                _onFindProgress,
                _onError    
            );
        }

        public IEnumerator Refresh()
        {
            void RefreshQuestAction(
                EzQuestGroupModel questGroupModel,
                EzCompletedQuestList completedQuestList
            )
            {
                QuestGroupModel = questGroupModel;
                CompletedQuestList = completedQuestList;
                
                _onGetQuestModel.RemoveListener(RefreshQuestAction);
                onWatchQuestEvent.Invoke(questGroupModel, completedQuestList);
            }

            _onGetQuestModel.AddListener(RefreshQuestAction);
            
            yield return QuestController.GetQuestModel(
                _client,
                _session,
                _questNamespaceName,
                _questGroupName,
                _onGetQuestModel,
                _onError
            );
        }

        public void Stop()
        {
            if (_watching) return;
            
            _watching = false;
        }
    }
}