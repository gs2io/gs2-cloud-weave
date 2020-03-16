using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Gs2Money.Request;
using Gs2.Gs2Stamina.Request;
using Gs2.Unity;
using Gs2.Unity.Gs2Exchange.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;
using LitJson;
using UnityEngine.Events;

namespace Gs2.Weave.Core.Watcher
{
    [Serializable]
    public class WatchExchangeEvent : UnityEvent<string, List<EzRateModel>>
    {
    }

    public class ExchangeWatcher
    {
        public WatchExchangeEvent onWatchExchangeEvent = new WatchExchangeEvent();

        private bool _watching;

        private Client _client;
        private GameSession _session;

        private string _exchangeNamespaceName;
        private string _rateName;
        private GetExchangeRateEvent _onGetExchangeRate;
        private ErrorEvent _onError;

        public List<EzRateModel> ExchangeRates;

        public IEnumerator Run(
            Client client,
            GameSession session,
            string exchangeNamespaceName,
            GetExchangeRateEvent onGetExchangeRate,
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

            _exchangeNamespaceName = exchangeNamespaceName;
            _onGetExchangeRate = onGetExchangeRate;
            _onError = onError;

            yield return Refresh();
        }

        public IEnumerator Refresh()
        {
            void RefreshExchangeAction(
                string exchangeNamespaceName,
                List<EzRateModel> rateModels
            )
            {
                ExchangeRates = rateModels;
                
                _onGetExchangeRate.RemoveListener(RefreshExchangeAction);
                onWatchExchangeEvent.Invoke(exchangeNamespaceName, ExchangeRates);
            }

            _onGetExchangeRate.AddListener(RefreshExchangeAction);
            
            yield return ExchangeController.GetExchangeRate(
                _client,
                _exchangeNamespaceName,
                _onGetExchangeRate,
                _onError
            );

        }

        public void Stop()
        {
            if (_watching)
            {
                throw new InvalidOperationException("not started");
            }
            
            _watching = false;
        }
    }
}