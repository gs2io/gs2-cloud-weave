using System.Linq;
using Gs2.Unity.Gs2Showcase.Model;
using LitJson;

namespace Gs2.Weave.Gacha
{
    public class SalesItem
    {
        private EzSalesItem _item;
        
        public SalesItem(string displayItemId, EzSalesItem item)
        {
            DisplayItemId = displayItemId;
            _item = item;
        }

        public string DisplayItemId { get; }

        public string LotteryNamespaceName
        {
            get { return JsonMapper.ToObject(_item.AcquireActions.First(action => action.Action == "Gs2Lottery:DrawByUserId").Request)["namespaceName"].ToString(); }
        }

        public string LotteryLotteryName
        {
            get { return JsonMapper.ToObject(_item.AcquireActions.First(action => action.Action == "Gs2Lottery:DrawByUserId").Request)["lotteryName"].ToString(); }
        }

        public string LotteryCount
        {
            get { return JsonMapper.ToObject(_item.AcquireActions.First(action => action.Action == "Gs2Lottery:DrawByUserId").Request)["count"].ToString(); }
        }

        public string Price
        {
            get { return JsonMapper.ToObject(_item.ConsumeActions.First(action => action.Action == "Gs2Money:WithdrawByUserId").Request)["count"].ToString(); }
        }
    }
}