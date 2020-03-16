using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Gs2Stamina.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Stamina
{
    public class StaminaDirector : MonoBehaviour
    {
        [SerializeField]
        public StaminaWidget staminaWidget;

        [SerializeField]
        public DebugStaminaControlWidget debugStaminaControlWidget;

        private Client _client;
        private GameSession _session;
        private Dictionary<string, string> _config;
        
        private StaminaWatcher _staminaWatcher;

        private EzStaminaModel _staminaModel;
        private StaminaSetting _staminaSetting;

        public void Start()
        {
            _staminaWatcher = new StaminaWatcher();
            _staminaSetting = GetComponent<StaminaSetting>();
        }

        public void OnDestroy()
        {
            _staminaWatcher.Stop();
        }

        public IEnumerator Run(
            Client client,
            GameSession session,
            Dictionary<string, string> config
        )
        {
            _client = client;
            _session = session;
            _config = config;
            
            void OnGetStaminaModel(
                string staminaModelNameTemp, 
                EzStaminaModel staminaModel
            )
            {
                Debug.Log("StaminaDirector::OnGetStaminaModel");
                
                _staminaSetting.onGetStaminaModel.RemoveListener(OnGetStaminaModel);

                _staminaModel = staminaModel;
            }

            _staminaSetting.onGetStaminaModel.AddListener(OnGetStaminaModel);
        
            yield return StaminaController.GetStaminaModel(
                client,
                _staminaSetting.staminaNamespaceName,
                _staminaSetting.staminaModelName,
                _staminaSetting.onGetStaminaModel,
                _staminaSetting.onError
            );
        
            _staminaWatcher.Initialize(
                _staminaSetting.staminaNamespaceName,
                _staminaModel,
                StartCoroutine,
                StopCoroutine,
                _staminaSetting.onGetStamina,
                _staminaSetting.onRecoverStamina,
                _staminaSetting.onConsumeStamina,
                _staminaSetting.onError
            );
            yield return _staminaWatcher.Run(
                client,
                session
            );
            
            staminaWidget.Initialize(
                _staminaWatcher
            );
            staminaWidget.gameObject.SetActive(true);

            if (debugStaminaControlWidget != null)
            {
                debugStaminaControlWidget.gameObject.SetActive(true);
                debugStaminaControlWidget.Initialize();
                debugStaminaControlWidget.onClickConsumeButton.AddListener(OnClickDebugConsumeButton);
            }
        }

        public void OnClickDebugConsumeButton(int consumeValue)
        {
            StartCoroutine(
                StaminaController.ConsumeStamina(
                    _client,
                    _session,
                    _staminaSetting.staminaNamespaceName,
                    _staminaModel,
                    consumeValue,
                    _staminaSetting.onConsumeStamina,
                    _staminaSetting.onGetStamina,
                    _staminaSetting.onError
                )
            );
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("StaminaDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("StaminaDirector::StateMachineOnDoneStampTask");
                
                if (task.Action == "Gs2Stamina:ConsumeStaminaByUserId")
                {
                    StartCoroutine(
                        _staminaWatcher.Refresh()
                    );
                }
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("StaminaDirector::StateMachineOnCompleteStampSheet");
                
                if (sheet.Action == "Gs2Stamina:RecoverStaminaByUserId")
                {
                    StartCoroutine(
                        _staminaWatcher.Refresh()
                    );
                }
            };
        }
    }
}