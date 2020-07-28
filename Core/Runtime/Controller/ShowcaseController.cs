using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Unity;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Unity.Gs2Showcase.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;
#if UNITY_PURCHASING
using UnityEngine.Purchasing;
#endif

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class ShowcaseController: MonoBehaviour
    {
        public static IEnumerator GetShowcase(
            Client client,
            GameSession session,
            string showcaseNamespaceName,
            string showcaseName,
            GetShowcaseEvent onGetShowcase,
            ErrorEvent onError
        )
        {
            AsyncResult<EzGetShowcaseResult> result = null;
            yield return client.Showcase.GetShowcase(
                r =>
                {
                    result = r;
                },
                session,
                showcaseNamespaceName,
                showcaseName
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var showcase = result.Result.Item;

            onGetShowcase.Invoke(showcase);
        }

        public static IEnumerator Buy(
            Client client,
            GameSession session,
            string showcaseNamespaceName,
            string showcaseName,
            string displayItemId,
            IssueBuyStampSheetEvent onIssueBuyStampSheet,
            ErrorEvent onError,
            List<EzConfig> config,
            string contentsId = null
        )
        {
            var tempConfig = new List<EzConfig>(config);
#if UNITY_PURCHASING
            IStoreController controller = null;
            Product product = null;
            string receipt = null;
            if (contentsId != null)
            {
                AsyncResult<Gs2.Unity.Util.PurchaseParameters> result = null;
                yield return new IAPUtil().Buy(
                    r => { result = r; },
                    contentsId
                );

                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                receipt = result.Result.receipt;
                controller = result.Result.controller;
                product = result.Result.product;
            }

            
            if (receipt != null)
            {
                tempConfig.Add(
                    new EzConfig
                    {
                        Key = "receipt", 
                        Value = receipt,
                    }
                );
            }
#else
            Debug.LogError("Unity Purchasing を有効にしてください。");
            throw new InvalidProgramException("Unity Purchasing を有効にしてください。");
#endif

            string stampSheet;
            {
                AsyncResult<EzBuyResult> result = null;
                yield return client.Showcase.Buy(
                    r => { result = r; },
                    session,
                    showcaseNamespaceName,
                    showcaseName,
                    displayItemId,
                    tempConfig
                );

                if (result.Error != null)
                {
                    onError.Invoke(
                        result.Error
                    );
                    yield break;
                }

                stampSheet = result.Result.StampSheet;
            }

            onIssueBuyStampSheet.Invoke(stampSheet);

#if UNITY_PURCHASING
            controller.ConfirmPendingPurchase(product);
#endif
        }
    }
}