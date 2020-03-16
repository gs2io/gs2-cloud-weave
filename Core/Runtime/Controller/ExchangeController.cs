using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Unity;
using Gs2.Unity.Gs2Exchange.Model;
using Gs2.Unity.Gs2Exchange.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class ExchangeController : MonoBehaviour
    {
        public static IEnumerator GetExchangeRate(
            Client client,
            string exchangeNamespaceName,
            GetExchangeRateEvent onGetExchangeRate,
            ErrorEvent onError
        )
        {
            AsyncResult<EzListRateModelsResult> result = null;
            yield return client.Exchange.ListRateModels(
                r => { result = r; },
                exchangeNamespaceName
            );

            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var rateModels = result.Result.Items;

            onGetExchangeRate.Invoke(exchangeNamespaceName, rateModels);
        }
        
        public static IEnumerator Exchange(
            Client client,
            GameSession session,
            string exchangeNamespaceName,
            string rateName,
            int count,
            IssueExchangeStampSheetEvent onIssueBuyStampSheet,
            ErrorEvent onError,
            List<EzConfig> configs = null
        )
        {
            AsyncResult<EzExchangeResult> result = null;
            yield return client.Exchange.Exchange(
                r => { result = r; },
                session,
                exchangeNamespaceName,
                rateName,
                count,
                configs
            );

            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var stampSheet = result.Result.StampSheet;

            onIssueBuyStampSheet.Invoke(stampSheet);
        }
    }
}