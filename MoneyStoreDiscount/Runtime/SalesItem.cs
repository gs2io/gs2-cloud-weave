using System.Linq;
using Gs2.Unity.Gs2Showcase.Model;
using LitJson;

namespace Gs2.Weave.MoneyStoreDiscount
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

        public int MaxLimit
        {
            get
            {
                if (_item.ConsumeActions.FirstOrDefault(action => action.Action == "Gs2Limit:CountUpByUserId") == null)
                {
                    return 0;
                }
                return int.Parse(JsonMapper.ToObject(_item.ConsumeActions.First(action => action.Action == "Gs2Limit:CountUpByUserId").Request)["maxValue"].ToString());
            }
        }

        public string LimitCounterName
        {
            get { 
                if (_item.ConsumeActions.FirstOrDefault(action => action.Action == "Gs2Limit:CountUpByUserId") == null)
                {
                    return null;
                }
                return JsonMapper.ToObject(_item.ConsumeActions.First(action => action.Action == "Gs2Limit:CountUpByUserId").Request)["counterName"].ToString(); }
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