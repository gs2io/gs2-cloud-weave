using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Gs2Realtime.Message;
using Gs2.Unity;
using Gs2.Unity.Gs2Limit.Model;
using Gs2.Unity.Gs2Limit.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class LimitController: MonoBehaviour
    {
        public static IEnumerator GetLimitModel(
            Client client,
            string limitNamespaceName,
            string limitModelName,
            GetLimitModelEvent onGetLimitModel,
            ErrorEvent onError
        )
        {
            AsyncResult<EzGetLimitModelResult> result = null;
            yield return client.Limit.GetLimitModel(
                r =>
                {
                    result = r;
                },
                limitNamespaceName,
                limitModelName
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var limitModel = result.Result.Item;

            onGetLimitModel.Invoke(limitModelName, limitModel);
        }

        public static IEnumerator ListCounters(
            Client client,
            GameSession session,
            string limitNamespaceName,
            EzLimitModel limitModel,
            GetCounterEvent onGetCounter,
            ErrorEvent onError
        )
        {
            var counters = new List<EzCounter>();
            
            string pageToken = null;
            while (true)
            {
                AsyncResult<EzListCountersResult> result = null;
                yield return client.Limit.ListCounters(
                    r =>
                    {
                        result = r;
                    },
                    session,
                    limitNamespaceName,
                    limitModel.Name,
                    pageToken,
                    30
                );
            
                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                counters.AddRange(result.Result.Items);
                pageToken = result.Result.NextPageToken;

                if (pageToken == null)
                {
                    break;
                }
            }

            foreach (var counter in counters)
            {
                onGetCounter.Invoke(limitModel, counter);
            }
        }

        public static IEnumerator GetCounter(
            Client client,
            GameSession session,
            string limitNamespaceName,
            EzLimitModel limitModel,
            string counterName,
            GetCounterEvent onGetCounter,
            ErrorEvent onError
        )
        {
            AsyncResult<EzGetCounterResult> result = null;
            yield return client.Limit.GetCounter(
                r =>
                {
                    result = r;
                },
                session,
                limitNamespaceName,
                limitModel.Name,
                counterName
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var counter = result.Result.Item;

            onGetCounter.Invoke(limitModel, counter);
        }

        public static IEnumerator CountUp(
            Client client,
            GameSession session,
            string limitNamespaceName,
            EzLimitModel limitModel,
            string counterName,
            int countUpValue,
            CountUpEvent onCountUp,
            ErrorEvent onError
        )
        {
            AsyncResult<EzCountUpResult> result = null;
            yield return client.Limit.CountUp(
                r =>
                {
                    result = r;
                },
                session,
                limitNamespaceName,
                limitModel.Name,
                counterName,
                countUpValue
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var counter = result.Result.Item;

            onCountUp.Invoke(limitModel, counter, countUpValue);
        }
    }
}