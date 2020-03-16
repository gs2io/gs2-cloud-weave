using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Core.Exception;
using Gs2.Unity;
using Gs2.Unity.Gs2Quest.Model;
using Gs2.Unity.Gs2Quest.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class QuestController : MonoBehaviour
    {
        public static IEnumerator GetQuestModel(
            Client client,
            GameSession session,
            string questNamespaceName,
            string questGroupName,
            GetQuestModelEvent onGetQuestModel,
            ErrorEvent onError
        )
        {
            EzQuestGroupModel questGroupModel;
            {
                AsyncResult<EzGetQuestGroupResult> result = null;
                yield return client.Quest.GetQuestGroup(
                    r => { result = r; },
                    questNamespaceName,
                    questGroupName
                );

                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                questGroupModel = result.Result.Item;
            }
            EzCompletedQuestList completedQuestList;
            {
                AsyncResult<EzGetCompletedQuestListResult> result = null;
                yield return client.Quest.GetCompletedQuestList(
                    r => { result = r; },
                    session,
                    questNamespaceName,
                    questGroupName
                );

                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                completedQuestList = result.Result.Item;
            }

            onGetQuestModel.Invoke(questGroupModel, completedQuestList);
        }

        public static IEnumerator StartQuest(
            Client client,
            GameSession session,
            string questNamespaceName,
            string questGroupName,
            string questName,
            IssueStartStampSheetEvent onIssueStartStampSheet,
            ErrorEvent onError
        )
        {
            AsyncResult<EzStartResult> result = null;
            yield return client.Quest.Start(
                r => { result = r; },
                session,
                questNamespaceName,
                questGroupName,
                questName
            );

            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var stampSheet = result.Result.StampSheet;

            onIssueStartStampSheet.Invoke(stampSheet);
        }

        public static IEnumerator GetProgress(
            Client client,
            GameSession session,
            string questNamespaceName,
            FindProgressEvent onFindProgress,
            ErrorEvent onError
        )
        {
            AsyncResult<EzGetProgressResult> result = null;
            yield return client.Quest.GetProgress(
                r => { result = r; },
                session,
                questNamespaceName
            );

            if (result.Error != null)
            {
                if (result.Error is NotFoundException)
                {
                    
                }
                else
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }
            }
            else
            {
                var questModel = result.Result.Quest;
                var progress = result.Result.Item;

                onFindProgress.Invoke(questModel, progress);
            }
        }

        public static IEnumerator End(
            Client client,
            GameSession session,
            string questNamespaceName,
            string transactionId,
            List<EzReward> rewards,
            bool isComplete,
            IssueEndStampSheetEvent onIssueEndStampSheet,
            ErrorEvent onError,
            List<EzConfig> config
        )
        {
            AsyncResult<EzEndResult> result = null;
            yield return client.Quest.End(
                r => { result = r; },
                session,
                questNamespaceName,
                transactionId,
                rewards,
                isComplete,
                config
            );

            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var stampSheet = result.Result.StampSheet;

            onIssueEndStampSheet.Invoke(stampSheet);
        }
    }
}