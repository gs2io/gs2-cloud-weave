using System;
using System.Collections;
using Gs2.Core;
using Gs2.Core.Model;
using Gs2.Core.Net;
using Gs2.Gs2Money;
using Gs2.Gs2Money.Request;
using Gs2.Gs2Realtime.Message;
using Gs2.Unity;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Unity.Gs2Money.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class MoneyController: MonoBehaviour
    {
        public static IEnumerator GetWallet(
            Client client,
            GameSession session,
            string moneyNamespaceName,
            int slot,
            GetWalletEvent onGetWallet,
            ErrorEvent onError
        )
        {
            AsyncResult<EzGetResult> result = null;
            yield return client.Money.Get(
                r =>
                {
                    result = r;
                },
                session,
                moneyNamespaceName,
                slot
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var wallet = result.Result.Item;

            onGetWallet.Invoke(wallet);
        }

        public static IEnumerator Deposit(
            string identifierDepositClientId,
            string identifierDepositClientSecret,
            string moneyNamespaceName,
            string userId,
            int slot,
            float price,
            int value,
            DepositEvent onDeposit,
            ErrorEvent onError
        )
        {
            // このコードは実際にアプリケーションで使用するべきではありません。
            // アプリ内から課金通貨の残高を加算できるようにすることは、事業に多大な悪い影響を及ぼす可能性があります。
            var restSession = new Gs2RestSession(
                new BasicGs2Credential(
                    identifierDepositClientId,
                    identifierDepositClientSecret
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

            var restClient = new Gs2MoneyRestClient(
                restSession
            );

            yield return restClient.DepositByUserId(
                new DepositByUserIdRequest()
                    .WithNamespaceName(moneyNamespaceName)
                    .WithUserId(userId)
                    .WithSlot(slot)
                    .WithPrice(price)
                    .WithCount(value),
                r =>
                {
                    if (r.Error != null)
                    {
                        onError.Invoke(r.Error);
                        error = true;
                    }
                    else
                    {
                        onDeposit.Invoke(new EzWallet(r.Result.item), price, value);
                    }
                }
            );
            
            yield return restSession.Close(() => { });
        }

        public static IEnumerator Withdraw(
            Client client,
            GameSession session,
            string moneyNamespaceName,
            EzWallet wallet,
            int value,
            bool paidOnly,
            WithdrawEvent onWithdraw,
            GetWalletEvent onGetWallet,
            ErrorEvent onError
        )
        {
            AsyncResult<EzWithdrawResult> result = null;
            yield return client.Money.Withdraw(
                r =>
                {
                    result = r;
                },
                session,
                moneyNamespaceName,
                wallet.Slot,
                value,
                paidOnly
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            wallet = result.Result.Item;

            onWithdraw.Invoke(wallet, value);
            onGetWallet.Invoke(wallet);
        }
    }
}