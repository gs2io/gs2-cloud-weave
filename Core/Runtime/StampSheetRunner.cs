using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core.Exception;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Util;
using UnityEngine.Events;

namespace Weave.Core.Runtime
{
    public class StampSheetRunner
    {
        private readonly Client _client;
        
        private readonly List<UnityAction<EzStampTask, EzRunStampTaskResult>> _onDoneStampTasks = new List<UnityAction<EzStampTask, EzRunStampTaskResult>>();
        private readonly List<UnityAction<EzStampSheet, EzRunStampSheetResult>> _onCompleteStampSheets = new List<UnityAction<EzStampSheet, EzRunStampSheetResult>>();
        private readonly List<UnityAction<Gs2Exception>> _onErrors = new List<UnityAction<Gs2Exception>>();
        
        public StampSheetRunner(
            Client client
        )
        {
            _client = client;
        }

        public void AddDoneStampTaskEventHandler(params UnityAction<EzStampTask, EzRunStampTaskResult>[] handlers)
        {
            foreach (var handler in handlers)
            {
                _onDoneStampTasks.Add(handler);
            }
        }

        public void AddCompleteStampSheetEvent(params UnityAction<EzStampSheet, EzRunStampSheetResult>[] handlers)
        {
            foreach (var handler in handlers)
            {
                _onCompleteStampSheets.Add(handler);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stampSheet"></param>
        /// <param name="encryptionKeyId"></param>
        /// <returns></returns>
        public IEnumerator Run(
            string stampSheet,
            string encryptionKeyId,
            UnityEvent<Gs2Exception> onError
        )
        {
            var stateMachine = new StampSheetStateMachine(
                stampSheet,
                _client,
                encryptionKeyId
            );

            foreach (var handler in _onDoneStampTasks)
            {
                stateMachine.OnDoneStampTask.AddListener(handler);
            }
            foreach (var handler in _onCompleteStampSheets)
            {
                stateMachine.OnCompleteStampSheet.AddListener(handler);
            }

            yield return stateMachine.Execute(
                onError
            );
        }
    }
}