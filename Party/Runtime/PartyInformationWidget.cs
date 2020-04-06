using Gs2.Weave.Core.Watcher;
using UnityEngine;

namespace Gs2.Weave.Party
{
    public class PartyInformationWidget : MonoBehaviour
    {
        
        [SerializeField] 
        public PartyListRow partyListRowPrefab;

        private FormationWatcher _formationWatcher;
        private PartyListRow _partyInformation;
        
        public void Initialize(
            InventoryWatcher inventoryWatcher,
            FormationWatcher formationWatcher,
            int index,
            ChoiceSlotEvent onChoiceSlot
        )
        {
            _formationWatcher = formationWatcher;
            
            _partyInformation = Instantiate(partyListRowPrefab, transform);
            _partyInformation.Initialize(
                inventoryWatcher,
                _formationWatcher,
                index,
                true,
                new ChoicePartyEvent(),
                onChoiceSlot
            );
            _partyInformation.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            if (_partyInformation != null)
            {
                Destroy(_partyInformation.gameObject);
            }
            gameObject.SetActive(true);
        }
    }
}