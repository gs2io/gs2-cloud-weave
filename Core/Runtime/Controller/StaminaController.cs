using System;
using System.Collections;
using Gs2.Core;
using Gs2.Gs2Realtime.Message;
using Gs2.Unity;
using Gs2.Unity.Gs2Stamina.Model;
using Gs2.Unity.Gs2Stamina.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class StaminaController: MonoBehaviour
    {
        public static IEnumerator GetStaminaModel(
            Client client,
            string staminaNamespaceName,
            string staminaModelName,
            GetStaminaModelEvent onGetStaminaModel,
            ErrorEvent onError
        )
        {
            AsyncResult<EzGetStaminaModelResult> result = null;
            yield return client.Stamina.GetStaminaModel(
                r =>
                {
                    result = r;
                },
                staminaNamespaceName,
                staminaModelName
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var staminaModel = result.Result.Item;

            onGetStaminaModel.Invoke(staminaModelName, staminaModel);
        }

        public static IEnumerator GetStamina(
            Client client,
            GameSession session,
            string staminaNamespaceName,
            EzStaminaModel staminaModel,
            GetStaminaEvent onGetStamina,
            ErrorEvent onError
        )
        {
            AsyncResult<EzGetStaminaResult> result = null;
            yield return client.Stamina.GetStamina(
                r =>
                {
                    result = r;
                },
                session,
                staminaNamespaceName,
                staminaModel.Name
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var stamina = result.Result.Item;

            onGetStamina.Invoke(staminaModel, stamina);
        }

        public static IEnumerator ConsumeStamina(
            Client client,
            GameSession session,
            string staminaNamespaceName,
            EzStaminaModel staminaModel,
            int consumeValue,
            ConsumeStaminaEvent onConsumeStamina,
            GetStaminaEvent onGetStamina,
            ErrorEvent onError
        )
        {
            AsyncResult<EzConsumeResult> result = null;
            yield return client.Stamina.Consume(
                r =>
                {
                    result = r;
                },
                session,
                staminaNamespaceName,
                staminaModel.Name,
                consumeValue
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var stamina = result.Result.Item;

            onConsumeStamina.Invoke(staminaModel, stamina, consumeValue);
            onGetStamina.Invoke(staminaModel, stamina);
        }
    }
}