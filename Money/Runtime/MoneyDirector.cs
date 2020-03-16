using System.Collections;
using System.Collections.Generic;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Money
{
    public class MoneyDirector : MonoBehaviour
    {
        [SerializeField]
        public WalletWidget walletWidget;

        [SerializeField]
        public DebugWalletControlWidget debugWalletControlWidget;

        private Client _client;
        private GameSession _session;
        private int _slot;
        private Dictionary<string, string> _config;
        
        private MoneyWatcher _moneyWatcher;
        private MoneySetting _moneySetting;

        public void Start()
        {
            _moneySetting = GetComponent<MoneySetting>();
            _moneyWatcher = new MoneyWatcher();
        }

        public void OnDestroy()
        {
            _moneyWatcher.Stop();
            if (debugWalletControlWidget != null)
            {
                debugWalletControlWidget.onClickDepositButton.RemoveListener(OnClickDepositButton);
                debugWalletControlWidget.onClickWithdrawButton.RemoveListener(OnClickWithdrawButton);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="session"></param>
        /// <param name="slot"></param>
        /// <param name="config"></param>
        public IEnumerator Run(
            Client client,
            GameSession session,
            int slot,
            Dictionary<string, string> config
        )
        {
            _client = client;
            _session = session;
            _slot = slot;
            _config = config;
        
            yield return _moneyWatcher.Run(
                client,
                session,
                _moneySetting.moneyNamespaceName,
                slot,
                _moneySetting.onGetWallet,
                _moneySetting.onDeposit,
                _moneySetting.onWithdraw,
                _moneySetting.onError
            );

            walletWidget.Initialize(
                _moneyWatcher
            );
            walletWidget.gameObject.SetActive(true);
            if (debugWalletControlWidget != null)
            {
                debugWalletControlWidget.Initialize();
                debugWalletControlWidget.onClickDepositButton.AddListener(OnClickDepositButton);
                debugWalletControlWidget.onClickWithdrawButton.AddListener(OnClickWithdrawButton);
                debugWalletControlWidget.gameObject.SetActive(true);
            }
        }

        public void OnClickDepositButton(
            float price,
            int value
        )
        {
            if (string.IsNullOrEmpty(_moneySetting.identifierDepositClientId) ||
                string.IsNullOrEmpty(_moneySetting.identifierDepositClientSecret))
            {
                Debug.LogError("インストーラーの設定でデバッグ機能が無効化されています");
            }

            StartCoroutine(
                MoneyController.Deposit(
                    _moneySetting.identifierDepositClientId,
                    _moneySetting.identifierDepositClientSecret,
                    _moneySetting.moneyNamespaceName,
                    _session.AccessToken.userId,
                    _slot,
                    price,
                    value,
                    _moneySetting.onDeposit,
                    _moneySetting.onError
                )
            );
        }
        
        public void OnClickWithdrawButton(int value)
        {
            StartCoroutine(
                MoneyController.Withdraw(
                    _client,
                    _session,
                    _moneySetting.moneyNamespaceName,
                    _moneyWatcher.Wallet,
                    value,
                    false,
                    _moneySetting.onWithdraw,
                    _moneySetting.onGetWallet,
                    _moneySetting.onError
                )
            );
        }
        
        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("MoneyDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("MoneyDirector::StateMachineOnDoneStampTask");

                if (task.Action == "Gs2Money:WithdrawByUserId")
                {
                    StartCoroutine(
                        _moneyWatcher.Refresh()
                    );
                }
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("MoneyDirector::StateMachineOnCompleteStampSheet");

                if (sheet.Action == "Gs2Money:DepositByUserId")
                {
                    StartCoroutine(
                        _moneyWatcher.Refresh()
                    );
                }
            };
        }
    }
}