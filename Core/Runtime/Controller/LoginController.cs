using System;
using System.Collections;
using Gs2.Core;
using Gs2.Core.Exception;
using Gs2.Gs2Auth.Model;
using Gs2.Unity;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Account.Result;
using Gs2.Unity.Gs2Auth.Result;
using Gs2.Unity.Util;
using Gs2.Weave.Core.CallbackEvent;
using Gs2.Weave.Login;
using UnityEngine;

namespace Gs2.Weave.Core.Controller
{
    [Serializable]
    public class LoginController: MonoBehaviour
    {
        public static IEnumerator AutoLogin(
            Client client,
            IAccountRepository repository,
            string accountNamespaceName,
            string accountEncryptionKeyId,
            CreateAccountEvent onCreateAccount,
            LoginEvent onLogin,
            ErrorEvent onError
        )
        {
            var error = false;
            
            void OnCreateAccount(EzAccount account)
            {
                repository.SaveAccount(
                    new PersistAccount
                    {
                        UserId = account.UserId,
                        Password = account.Password,
                    }
                );
            }

            void OnError(Gs2Exception e)
            {
                error = true;
            }

            onCreateAccount.AddListener(OnCreateAccount);
            onError.AddListener(OnError);

            try
            {
                if (!repository.IsExistsAccount())
                {
                    yield return CreateAccount(
                        client,
                        accountNamespaceName,
                        onCreateAccount,
                        onError
                    );
                }

                if (error)
                {
                    yield break;
                }
            
                var account = repository.LoadAccount();
                yield return Login(
                    client,
                    account?.UserId,
                    account?.Password,
                    accountNamespaceName,
                    accountEncryptionKeyId,
                    onLogin,
                    onError
                );
            }
            finally
            {
                onError.RemoveListener(OnError);
                onCreateAccount.RemoveListener(OnCreateAccount);
            }
        }
        
        public static IEnumerator CreateAccount(
            Client client,
            string accountNamespaceName,
            CreateAccountEvent onCreateAccount,
            ErrorEvent onError
        )
        {
            AsyncResult<EzCreateResult> result = null;
            yield return client.Account.Create(
                r =>
                {
                    result = r;
                },
                accountNamespaceName
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var account = result.Result.Item;

            onCreateAccount.Invoke(account);
        }
        
        public static IEnumerator Login(
            Client client,
            string userId,
            string password,
            string accountNamespaceName,
            string accountEncryptionKeyId,
            LoginEvent onLogin,
            ErrorEvent onError
        )
        {
            AsyncResult<EzAuthenticationResult> result = null;
            yield return client.Account.Authentication(
                r =>
                {
                    result = r;
                },
                accountNamespaceName,
                userId,
                accountEncryptionKeyId,
                password
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var account = result.Result.Item;

            AsyncResult<EzLoginResult> result2 = null;
            yield return client.Auth.Login(
                r =>
                {
                    result2 = r;
                },
                userId,
                accountEncryptionKeyId,
                result.Result.Body,
                result.Result.Signature
            );

            var session = new GameSession(
                new AccessToken()
                    .WithToken(result2.Result.Token)
                    .WithExpire(result2.Result.Expire)
                    .WithUserId(result2.Result.UserId)
            );

            onLogin.Invoke(account, session);
        }
    }
}