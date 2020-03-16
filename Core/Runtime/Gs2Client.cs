using System;
using UnityEngine;

namespace Weave.Core.Runtime
{
    public class Gs2Client : MonoBehaviour
    {
        public Gs2.Unity.Util.Profile Profile;
        public Gs2.Unity.Client Client;

        public void Start()
        {
            DontDestroyOnLoad (this);
        }

        public static Gs2Client GetGs2Client()
        {
            var gs2ClientTransform = GameObject.Find("Gs2Client");
            if (gs2ClientTransform == null)
            {
                throw new InvalidProgramException("io.gs2.Core: GS2 SDKクライアントが見つかりませんでした。 io.gs2.Credential のプレハブがシーンに配置出来ていないかもしれません。");
            }

            var gs2Client = gs2ClientTransform.gameObject.GetComponent<Gs2Client>();
            if (gs2Client == null)
            {
                throw new InvalidProgramException("io.gs2.Core: GS2 SDKクライアントが見つかりませんでした。 io.gs2.Credential のプレハブがシーンに配置出来ていないかもしれません。");
            }

            return gs2Client;
        }
    }
}