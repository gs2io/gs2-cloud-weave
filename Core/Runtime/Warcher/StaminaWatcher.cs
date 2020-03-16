using System;
using System.Collections;
using Gs2.Core.Util;
using Gs2.Unity;
using Gs2.Unity.Gs2Stamina.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Core.Controller;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Core.Watcher
{
    public class StaminaWatcher
    {
        private bool _watching;
        private Coroutine _coroutine;

        private Client _client;
        private GameSession _session;
        private EzStamina _stamina;

        public EzStamina Stamina => _stamina;

        public TimeSpan NextRecoverSpan => _stamina == null ? TimeSpan.Zero : UnixTime.FromUnixTime(Stamina.NextRecoverAt) - DateTime.UtcNow;

        private string _staminaNamespaceName;
        private EzStaminaModel _staminaModel;
        private Func<IEnumerator, Coroutine> _startCoroutine;
        private Action<Coroutine> _stopCoroutine;
        private GetStaminaEvent _onGetStamina;
        private RecoverStaminaEvent _onRecoverStamina;
        private ConsumeStaminaEvent _onConsumeStamina;
        private ErrorEvent _onError;

        public void Initialize(
            string staminaNamespaceName,
            EzStaminaModel staminaModel,
            Func<IEnumerator, Coroutine> startCoroutine,
            Action<Coroutine> stopCoroutine,
            GetStaminaEvent onGetStamina,
            RecoverStaminaEvent onRecoverStamina,
            ConsumeStaminaEvent onConsumeStamina,
            ErrorEvent onError
        )
        {
            _staminaNamespaceName = staminaNamespaceName;
            _staminaModel = staminaModel;
            _startCoroutine = startCoroutine;
            _stopCoroutine = stopCoroutine;
            _onGetStamina = onGetStamina;
            _onRecoverStamina = onRecoverStamina;
            _onConsumeStamina = onConsumeStamina;
            _onError = onError;
        }
        
        private void ConsumeStaminaAction(
            EzStaminaModel staminaModelTemp, 
            EzStamina stamina, 
            int consumeValue
        )
        {
            if (staminaModelTemp.Name != _staminaModel.Name)
            {
                return;
            }
            
            _stamina = stamina;
        }

        public IEnumerator Run(
            Client client,
            GameSession session
        )
        {
            if (_watching)
            {
                throw new InvalidOperationException("already started");
            }
            
            _watching = true;

            _client = client;
            _session = session;

            _coroutine = _startCoroutine(Watch());
            _onConsumeStamina.AddListener(ConsumeStaminaAction);

            yield return Refresh();
        }

        public IEnumerator Refresh()
        {
            void RefreshStaminaAction(
                EzStaminaModel staminaModelTemp, 
                EzStamina stamina
            )
            {
                if (staminaModelTemp.Name != _staminaModel.Name)
                {
                    return;
                }
                
                _stamina = stamina;
                
                _onGetStamina.RemoveListener(RefreshStaminaAction);
            }

            _onGetStamina.AddListener(RefreshStaminaAction);
            
            yield return StaminaController.GetStamina(
                _client,
                _session,
                _staminaNamespaceName,
                _staminaModel,
                _onGetStamina,
                _onError
            );
        }

        private IEnumerator Watch()
        {
            while (true)
            {
                if (_stamina != null)
                {
                    if (_stamina.NextRecoverAt != 0 && _stamina.NextRecoverAt < UnixTime.ToUnixTime(DateTime.UtcNow))
                    {
                        var beforeStaminaValue = _stamina.Value;
                        _stamina.Value += _stamina.RecoverValue;
                        if (_stamina.Value > _staminaModel.MaxCapacity)
                        {
                            _stamina.Value = _staminaModel.MaxCapacity;
                        }

                        if (beforeStaminaValue != _stamina.Value)
                        {
                            _onRecoverStamina.Invoke(
                                _staminaModel,
                                _stamina,
                                _stamina.Value - beforeStaminaValue
                            );
                        }

                        _stamina.NextRecoverAt += _stamina.RecoverIntervalMinutes * 60 * 1000;
                    }
                }

                yield return new WaitForSeconds(1);
            }
        }

        public void Stop()
        {
            if (!_watching) return;
            
            _stopCoroutine(_coroutine);
            
            _onConsumeStamina.RemoveListener(ConsumeStaminaAction);
            
            _watching = false;
        }
    }
}