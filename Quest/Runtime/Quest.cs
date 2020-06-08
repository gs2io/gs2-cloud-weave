using System.Linq;
using Gs2.Gs2Stamina.Request;
using Gs2.Unity.Gs2Quest.Model;
using Gs2.Util.LitJson;

namespace Gs2.Weave.Quest
{
    public class Quest
    {
        private EzQuestModel _item;
        
        public Quest(EzQuestModel item)
        {
            _item = item;
        }

        public int? ConsumeStamina
        {
            get { return ConsumeStaminaByUserIdRequest.FromDict(JsonMapper.ToObject(_item.ConsumeActions.First(action => action.Action == "Gs2Stamina:ConsumeStaminaByUserId").Request)).consumeValue; }
        }

        public string Name => _item.Name;
    }

}