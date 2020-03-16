using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Core.Model;
using Gs2.Core.Net;
using Gs2.Gs2Experience;
using Gs2.Gs2Experience.Request;
using Gs2.Gs2Inventory;
using Gs2.Unity;
using Gs2.Unity.Gs2Experience.Model;
using Gs2.Unity.Gs2Experience.Result;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class ExperienceController: MonoBehaviour
    {
        public static IEnumerator GetExperienceModel(
            Client client,
            string experienceNamespaceName,
            string experienceModelName,
            GetExperienceModelEvent onGetExperienceModel,
            ErrorEvent onError
        )
        {
            AsyncResult<EzGetExperienceModelResult> result = null;
            yield return client.Experience.GetExperienceModel(
                r =>
                {
                    result = r;
                },
                experienceNamespaceName,
                experienceModelName
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var experienceModel = result.Result.Item;

            onGetExperienceModel.Invoke(experienceModelName, experienceModel);
        }

        public static IEnumerator GetStatuses(
            Client client,
            GameSession session,
            string experienceNamespaceName,
            EzExperienceModel experienceModel,
            GetStatusesEvent onGetStatuses,
            ErrorEvent onError
        )
        {
            var statuses = new List<EzStatus>();
            string pageToken = null;
            while (true)
            {
                AsyncResult<EzListStatusesResult> result = null;
                yield return client.Experience.ListStatuses(
                    r =>
                    {
                        result = r;
                    },
                    session,
                    experienceNamespaceName,
                    experienceModel.Name,
                    pageToken
                );
            
                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                statuses.AddRange(result.Result.Items);

                if (result.Result.NextPageToken == null)
                {
                    break;
                }

                pageToken = result.Result.NextPageToken;
            }

            onGetStatuses.Invoke(experienceModel, statuses);
        }
        
        public static IEnumerator IncreaseExperience(
            GameSession session,
            string identifierIncreaseExperienceClientId,
            string identifierIncreaseExperienceClientSecret,
            string experienceNamespaceName,
            EzExperienceModel experienceModel,
            string propertyId,
            int value,
            IncreaseExperienceEvent onIncreaseExperience,
            ErrorEvent onError
        )
        {
            // このコードは実際にアプリケーションで使用するべきではありません。
            // アプリ内から課金通貨の残高を加算できるようにすることは、事業に多大な悪い影響を及ぼす可能性があります。
            var restSession = new Gs2RestSession(
                new BasicGs2Credential(
                    identifierIncreaseExperienceClientId,
                    identifierIncreaseExperienceClientSecret
                )
            );
            var error = false;
            yield return restSession.Open(
                r =>
                {
                    if (r.Error != null)
                    {
                        onError.Invoke(r.Error);
                        error = true;
                    }
                }
            );

            if (error)
            {
                yield return restSession.Close(() => { });
                yield break;
            }

            var restClient = new Gs2ExperienceRestClient(
                restSession
            );

            yield return restClient.AddExperienceByUserId(
                new AddExperienceByUserIdRequest()
                    .WithNamespaceName(experienceNamespaceName)
                    .WithUserId(session.AccessToken.userId)
                    .WithExperienceName(experienceModel.Name)
                    .WithPropertyId(propertyId)
                    .WithExperienceValue(value),
                r =>
                {
                    if (r.Error != null)
                    {
                        onError.Invoke(r.Error);
                        error = true;
                    }
                    else
                    {
                        onIncreaseExperience.Invoke(
                            experienceModel,
                            new EzStatus(r.Result.item),
                            value
                        );
                    }
                }
            );
            
            yield return restSession.Close(() => { });
        }
    }
}