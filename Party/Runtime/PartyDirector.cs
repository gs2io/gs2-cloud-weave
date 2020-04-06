using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gs2.Gs2Inventory.Model;
using Gs2.Unity;
using Gs2.Unity.Gs2Distributor.Result;
using Gs2.Unity.Gs2Formation.Model;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Unity.Gs2JobQueue.Model;
using Gs2.Unity.Util;
using Gs2.Weave.Core.Controller;
using Gs2.Weave.Core.Watcher;
using Gs2.Weave.Unit;
using UnityEngine;
using UnityEngine.Events;

namespace Gs2.Weave.Party
{
    public class PartyDirector : MonoBehaviour
    {
        [SerializeField]
        public UnitSetting unitSetting;

        [SerializeField]
        public PartyListWidget partyListWidget;

        [SerializeField]
        public PartyInformationWidget partyInformationWidget;

        [SerializeField]
        public PartyUnitListWidget partyUnitListWidget;

        private Gs2.Unity.Client _client;
        private GameSession _session;
        private Dictionary<string, string> _config;

        private FormationWatcher _formationWatcher;
        private InventoryWatcher _unitInventoryWatcher;
        private PartySetting _partySetting;

        public FormationWatcher FormationWatcher => _formationWatcher;
        public InventoryWatcher UnitInventoryWatcher => _unitInventoryWatcher;

        private EzMoldModel _selectedMoldModel;
        private int _selectedMoldIndex;
        private EzForm _selectedForm;
        private int _selectedSlotIndex;

        public void Start()
        {
            _formationWatcher = new FormationWatcher();
            _partySetting = GetComponent<PartySetting>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="session"></param>
        /// <param name="unitInventoryWatcher"></param>
        /// <param name="config"></param>
        public IEnumerator Run(
            Gs2.Unity.Client client,
            GameSession session,
            InventoryWatcher unitInventoryWatcher,
            Dictionary<string, string> config
        )
        {
            _client = client;
            _session = session;
            _unitInventoryWatcher = unitInventoryWatcher;
            _config = config;

            partyListWidget.Initialize(
                _unitInventoryWatcher,
                _formationWatcher
            );
            partyListWidget.onChoiceParty.AddListener(OnChoiceParty);

            yield return _formationWatcher.Run(
                client,
                session,
                _partySetting.formationNamespaceName,
                _partySetting.formModelName,
                _partySetting.onGetMoldModelModel,
                _partySetting.onGetForm,
                _partySetting.onUpdateForm,
                _partySetting.onError
            );
        }

        private IEnumerator UpdateParty(
            int index,
            IEnumerable<EzSlot> units,
            string signatureKeyId
        )
        {
            var signedSlots = new List<EzSlotWithSignature>();

            foreach (var unit in units)
            {
                void OnGetItemSetWithSignature(
                    string itemSetId,
                    string signatureKeyIdTmp,
                    string body,
                    string signature
                )
                {
                    if (itemSetId != unit.PropertyId || signatureKeyIdTmp != signatureKeyId)
                    {
                        return;
                    }
                
                    signedSlots.Add(
                        new EzSlotWithSignature
                        {
                            Name = unit.Name,
                            PropertyType = "gs2_inventory",
                            Body = body,
                            Signature = signature,
                        }
                    );
                }
                
                if (unit.PropertyId != null)
                {
                    unitSetting.onGetItemSetWithSignature.AddListener(OnGetItemSetWithSignature);

                    yield return InventoryController.GetItemSetWithSignature(
                        _client,
                        _session,
                        unitSetting.inventoryNamespaceName,
                        unitSetting.inventoryModelName,
                        ItemSet.GetItemNameFromGrn(unit.PropertyId),
                        ItemSet.GetItemSetNameFromGrn(unit.PropertyId),
                        signatureKeyId,
                        unitSetting.onGetItemSetWithSignature,
                        _partySetting.onError
                    );

                    unitSetting.onGetItemSetWithSignature.RemoveListener(OnGetItemSetWithSignature);
                }
            }

            if (signedSlots.Count > 0)
            {
                yield return FormationController.SetForm(
                    _client,
                    _session,
                    _partySetting.formationNamespaceName,
                    _formationWatcher.MoldModel,
                    index,
                    signedSlots,
                    signatureKeyId,
                    _partySetting.onGetForm,
                    _partySetting.onUpdateForm,
                    _partySetting.onError
                );
            }
        }

        public void OnChoiceParty(
            EzMoldModel moldModel,
            int index,
            EzForm form
        )
        {
            Debug.Log("PartyDirector::OnChoiceParty");

            _selectedMoldModel = moldModel;
            _selectedMoldIndex = index;
            if (form == null)
            {
                var slots = new List<EzSlot>();
                foreach (var slot in _selectedMoldModel.FormModel.Slots)
                {
                    slots.Add(
                        new EzSlot
                        {
                            Name = slot.Name,
                        }
                    );
                }
                _selectedForm = new EzForm
                {
                    Index = index,
                    Slots = slots,
                };
            }
            else
            {
                _selectedForm = form;
            }

            void OnChoiceSlot(
                EzForm _,
                int slotIndex,
                EzSlot __
            )
            {
                Debug.Log("PartyDirector::OnChoiceSlot");
                
                _selectedSlotIndex = slotIndex;
            }

            var onChoiceSlot = new ChoiceSlotEvent();
            onChoiceSlot.AddListener(OnChoiceSlot);
            partyInformationWidget.Initialize(
                _unitInventoryWatcher,
                _formationWatcher,
                index,
                onChoiceSlot
            );
            
            partyUnitListWidget.Initialize(
                _unitInventoryWatcher,
                _formationWatcher,
                _selectedMoldIndex,
                _selectedForm
            );

            partyListWidget.gameObject.SetActive(false);
            partyUnitListWidget.gameObject.SetActive(true);
        }

        public void OnCloseFormationChoiceUnitWidget()
        {
            partyInformationWidget.Close();
            partyUnitListWidget.gameObject.SetActive(false);
            partyListWidget.gameObject.SetActive(true);
        }

        public void OnChoiceUnit(EzItemSet unit)
        {
            Debug.Log("PartyDirector::OnChoiceUnit");

            var slot = _selectedForm.Slots.FirstOrDefault(item => item.Name == _selectedSlotIndex.ToString());
            if (slot == null)
            {
                _selectedForm.Slots.Add(new EzSlot
                {
                    Name = _selectedSlotIndex.ToString(),
                    PropertyId = unit.ItemSetId,
                });
            }
            else
            {
                slot.PropertyId = unit.ItemSetId;
            }

            _formationWatcher.onWatchFormation.Invoke(
                _selectedMoldModel,
                _selectedMoldIndex,
                _selectedForm
            );
            
            _partySetting.onChangeFormation.Invoke(
                _selectedMoldModel,
                _selectedMoldIndex,
                _selectedSlotIndex,
                unit
            );
        }

        public void OnCommitChangeParty()
        {
            Debug.Log("PartyDirector::OnCommitChangeParty");

            StartCoroutine(
                UpdateParty(
                    _selectedMoldIndex,
                    _selectedForm.Slots,
                    _partySetting.partyKeyId
                )
            );
        }

        public UnityAction<EzJob, EzJobResultBody> GetJobQueueAction()
        {
            return (job, jobResult) =>
            {
                Debug.Log("PartyDirector::GetJobQueueAction");
            };
        }

        public UnityAction<EzStampTask, EzRunStampTaskResult> GetTaskCompleteAction()
        {
            return (task, taskResult) =>
            {
                Debug.Log("PartyDirector::StateMachineOnDoneStampTask");
            };
        }

        public UnityAction<EzStampSheet, EzRunStampSheetResult> GetSheetCompleteAction()
        {
            return (sheet, sheetResult) =>
            {
                Debug.Log("PartyDirector::StateMachineOnCompleteStampSheet");
            };
        }

        public void OnShowPartyWidget()
        {
            partyListWidget.gameObject.SetActive(true);
        }
    }
}