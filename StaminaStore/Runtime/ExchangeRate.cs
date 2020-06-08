using System.Linq;
using Gs2.Gs2Money.Request;
using Gs2.Gs2Stamina.Request;
using Gs2.Unity.Gs2Exchange.Model;
using Gs2.Util.LitJson;

namespace Gs2.Weave.StaminaStore
{
    public class ExchangeRate
    {
        private EzRateModel _item;
        
        public ExchangeRate(EzRateModel item)
        {
            _item = item;
        }

        public int? WithdrawCurrencyValue
        {
            get { return WithdrawByUserIdRequest.FromDict(JsonMapper.ToObject(_item.AcquireActions.First(action => action.Action == "Gs2Money:WithdrawByUserId").Request)).count; }
        }

        public int? RecoverValue
        {
            get { return RecoverStaminaByUserIdRequest.FromDict(JsonMapper.ToObject(_item.AcquireActions.First(action => action.Action == "Gs2Stamina:RecoverStaminaByUserId").Request)).recoverValue; }
        }
    }

}