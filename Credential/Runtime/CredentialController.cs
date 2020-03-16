using System;
using System.Collections;
using Gs2.Core;
using Gs2.Unity.Util;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gs2.Weave.Credential
{
    [Serializable]
    public class CredentialController: MonoBehaviour
    {
        public static IEnumerator InitializeGs2(
            string clientId,
            string clientSecret,
            InitializeGs2AccountEvent onInitializeGs2,
            ErrorEvent onError
        )
        {
            Assert.IsFalse(string.IsNullOrEmpty(clientId), "string.IsNullOrEmpty(clientId)");
            Assert.IsFalse(string.IsNullOrEmpty(clientSecret), "string.IsNullOrEmpty(clientSecret)");
            Assert.IsNotNull(onInitializeGs2, "onInitializeGs2 != null");
            Assert.IsNotNull(onError, "onError != null");
            
            var profile = new Profile(
                clientId,
                clientSecret,
                new Gs2BasicReopener()
            );
            
            AsyncResult<object> result = null;
            yield return profile.Initialize(
                r => { result = r; }
            );
            
            if (result.Error != null)
            {
                onError.Invoke(
                    result.Error
                );
                yield break;
            }

            var client = new Gs2.Unity.Client(profile);

            onInitializeGs2.Invoke(profile, client);
        }

        public static IEnumerator FinalizeGs2(
            Gs2.Unity.Util.Profile profile,
            FinalizeGs2AccountEvent onFinalizeGs2
        )
        {
            Assert.IsNotNull(profile, "profile != null");
            Assert.IsNotNull(onFinalizeGs2, "onFinalizeGs2 != null");
            
            yield return profile.Finalize();

            onFinalizeGs2.Invoke(profile);
        }
    }
}