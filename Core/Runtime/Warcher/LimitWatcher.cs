using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Unity;
using Gs2.Unity.Gs2Limit.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;

namespace Gs2.Weave.Core.Watcher
{
    public class LimitWatcher
    {
        private bool _watching;

        private Client _client;
        private GameSession _session;
        private EzLimitModel _limitModel;

        public EzLimitModel LimitModel => _limitModel;

        private string _limitNamespaceName;
        private string _limitModelName;
        private readonly Dictionary<string, EzCounter> _counters = new Dictionary<string, EzCounter>();
        private GetLimitModelEvent _onGetLimitModel;
        private GetCounterEvent _onGetCounter;
        private CountUpEvent _onCountUpEvent;
        private ErrorEvent _onError;

        public EzCounter GetCounter(string counterName)
        {
            if (_counters.ContainsKey(counterName))
            {
                return _counters[counterName];
            }
            return new EzCounter
            {
                Name = counterName,
            };
        }

        private void CountUpAction(
            EzLimitModel limitModelTemp, 
            EzCounter counter, 
            int countUpValue
        )
        {
            if (limitModelTemp.Name != _limitModel.Name)
            {
                return;
            }
            
            _counters[counter.Name] = counter;
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            string limitNamespaceName,
            string limitModelName,
            GetLimitModelEvent onGetLimitModel,
            GetCounterEvent onGetCounter,
            CountUpEvent onCountUpEvent,
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
            _limitNamespaceName = limitNamespaceName;
            _limitModelName = limitModelName;
            _onGetLimitModel = onGetLimitModel;
            _onGetCounter = onGetCounter;
            _onCountUpEvent = onCountUpEvent;
            _onError = onError;
            
            _onCountUpEvent.AddListener(CountUpAction);

            yield return Refresh();
        }

        public IEnumerator Refresh()
        {
            void RefreshLimitModelAction(
                string namespaceName,
                EzLimitModel limitModelTemp
            )
            {
                _limitModel = limitModelTemp;
                
                _onGetLimitModel.RemoveListener(RefreshLimitModelAction);
            }

            void RefreshCounterAction(
                EzLimitModel limitModelTemp,
                EzCounter counter
            )
            {
                if (limitModelTemp.Name != _limitModel.Name)
                {
                    return;
                }

                _limitModel = limitModelTemp;
                _counters[counter.Name] = counter;
                
                _onGetCounter.RemoveListener(RefreshCounterAction);
            }

            _onGetLimitModel.AddListener(RefreshLimitModelAction);
            _onGetCounter.AddListener(RefreshCounterAction);
            
            yield return LimitController.GetLimitModel(
                _client,
                _limitNamespaceName,
                _limitModelName,
                _onGetLimitModel,
                _onError
            );
            
            yield return LimitController.ListCounters(
                _client,
                _session,
                _limitNamespaceName,
                _limitModel,
                _onGetCounter,
                _onError
            );
        }

        public void Stop()
        {
            if (!_watching) return;
            
            _onCountUpEvent.RemoveListener(CountUpAction);
            
            _watching = false;
        }
    }
}