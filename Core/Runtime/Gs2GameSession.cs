using System;
using UnityEngine;

namespace Weave.Core.Runtime
{
    public class Gs2GameSession : MonoBehaviour
    {
        public Gs2.Unity.Gs2Account.Model.EzAccount Account;
        public Gs2.Unity.Util.GameSession Session;

        public void Start()
        {
            DontDestroyOnLoad (this);
        }

        public static Gs2GameSession GetGameSession()
        {
            var gameSessionTransform = GameObject.Find("GameSession");
            if (gameSessionTransform == null)
            {
                throw new InvalidProgramException("io.gs2.Core: ログイン情報が見つかりませんでした。 io.gs2.Login のプレハブがシーンに配置出来ていないかもしれません。");
            }

            var gameSession = gameSessionTransform.gameObject.GetComponent<Gs2GameSession>();
            if (gameSession == null)
            {
                throw new InvalidProgramException("io.gs2.Core: ログイン情報が見つかりませんでした。 io.gs2.Login のプレハブがシーンに配置出来ていないかもしれません。");
            }

            return gameSession;
        }
    }
}