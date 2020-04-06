using System;
using System.Collections.Generic;
using Gs2.Unity.Gs2Formation.Model;
using Gs2.Weave.Core.Watcher;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gs2.Weave.Party
{
    [Serializable]
    public class ChoicePartyEvent : UnityEvent<EzMoldModel, int, EzForm>
    {
        
    }
    
    [Serializable]
    public class CloseWidgetPartyEvent : UnityEvent
    {
        
    }

    public class PartyListWidget : MonoBehaviour
    {
        [SerializeField] 
        public ChoicePartyEvent onChoiceParty = new ChoicePartyEvent();

        [SerializeField] 
        public CloseWidgetPartyEvent onClose = new CloseWidgetPartyEvent();

        [SerializeField] 
        public PartyListRow partyListRowPrefab;
        
        [SerializeField] 
        public VerticalLayoutGroup verticalLayoutGroup;

        private InventoryWatcher _inventoryWatcher;
        private FormationWatcher _formationWatcher;

        private List<PartyListRow> _rows;

        public void Initialize(
            InventoryWatcher inventoryWatcher,
            FormationWatcher formationWatcher
        )
        {
            _inventoryWatcher = inventoryWatcher;
            _formationWatcher = formationWatcher;

            void OnDoneSetupWatcher()
            {
                for (var i = 0; i < _formationWatcher.MoldModel.MaxCapacity; i++)
                {
                    var row = Instantiate(partyListRowPrefab, verticalLayoutGroup.transform);
                    row.Initialize(
                        _inventoryWatcher,
                        _formationWatcher,
                        i,
                        false,
                        onChoiceParty,
                        new ChoiceSlotEvent()
                    );
                    row.gameObject.SetActive(true);
                    _rows.Add(row);
                }
                
                _formationWatcher.onWatchFormation.AddListener(OnChangeFormation);
            }
            
            _formationWatcher.onDoneSetup.AddListener(OnDoneSetupWatcher);
            
            _rows = new List<PartyListRow>();
        }

        public void OnChangeFormation(
            EzMoldModel moldModel,
            int index,
            EzForm form
        )
        {
            _rows[index].Initialize(
                _inventoryWatcher,
                _formationWatcher,
                index,
                false,
                onChoiceParty,
                new ChoiceSlotEvent()
            );
        }

        public void OnClose()
        {
            gameObject.SetActive(false);
            onClose.Invoke();
        }
    }
}