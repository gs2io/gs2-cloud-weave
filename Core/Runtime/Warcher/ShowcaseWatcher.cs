using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Unity;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;
using Gs2.Util.LitJson;
using UnityEngine.Events;

namespace Gs2.Weave.Core.Watcher
{
    [Serializable]
    public class WatchShowcaseEvent : UnityEvent<EzShowcase, List<EzDisplayItem>>
    {
    }

    public class ShowcaseWatcher
    {
        public WatchShowcaseEvent onWatchShowcaseEvent = new WatchShowcaseEvent();

        private bool _watching;

        private Client _client;
        private GameSession _session;

        private string _showcaseNamespaceName;
        private string _showcaseName;
        private GetShowcaseEvent _onGetShowcase;
        private ErrorEvent _onError;

        public EzShowcase Showcase { get; private set; }

        public List<EzDisplayItem> DisplayItems => Showcase.DisplayItems;
        
        public IEnumerator Run(
            Client client,
            GameSession session,
            string showcaseNamespaceName,
            string showcaseName,
            GetShowcaseEvent onGetShowcase,
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

            _showcaseNamespaceName = showcaseNamespaceName;
            _showcaseName = showcaseName;
            _onGetShowcase = onGetShowcase;
            _onError = onError;

            yield return Refresh();
        }

        public IEnumerator Refresh()
        {
            void RefreshShowcaseAction(
                EzShowcase showcase
            )
            {
                Showcase = showcase;
                
                _onGetShowcase.RemoveListener(RefreshShowcaseAction);
                onWatchShowcaseEvent.Invoke(showcase, DisplayItems);
            }

            _onGetShowcase.AddListener(RefreshShowcaseAction);
            yield return ShowcaseController.GetShowcase(
                _client,
                _session,
                _showcaseNamespaceName,
                _showcaseName,
                _onGetShowcase,
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