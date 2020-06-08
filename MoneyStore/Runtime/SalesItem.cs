using System.Linq;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Util.LitJson;

namespace Gs2.Weave.MoneyStore
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

        public string ContentsId
        {
            get { return JsonMapper.ToObject(_item.ConsumeActions.First(action => action.Action == "Gs2Money:RecordReceipt").Request)["contentsId"].ToString(); }
        }

        public string Count
        {
            get { return JsonMapper.ToObject(_item.AcquireActions.First(action => action.Action == "Gs2Money:DepositByUserId").Request)["count"].ToString(); }
        }

        public string Price
        {
            get { return JsonMapper.ToObject(_item.AcquireActions.First(action => action.Action == "Gs2Money:DepositByUserId").Request)["price"].ToString(); }
        }
    }
}