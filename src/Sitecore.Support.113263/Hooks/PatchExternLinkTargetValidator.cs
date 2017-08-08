using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Events.Hooks;
using Sitecore.SecurityModel;

namespace Sitecore.Support.Hooks
{
  public class PatchExternLinkTargetValidator : IHook
  {
    public void Initialize()
    {
      using (new SecurityDisabler())
      {
        string dbName = "master";
        var db = Factory.GetDatabase(dbName);
        if (db != null)
        {
          string itemPath = "/sitecore/system/Settings/Validation Rules/Field Rules/System/Extern Link Target";
          var itemID = ID.Parse("{00A05EC0-AECB-4DF3-BEA5-D5FCCC13F2D1}");
          var item = db.GetItem(itemPath);
          item = (item == null) ? db.GetItem(itemID) : item;
          if (item != null)
          {
            var type = typeof(Sitecore.Support.Data.Validators.FieldValidators.ExternLinkTargetValidator);
            string fieldValue = type.FullName + "," + type.Assembly.GetName().Name;
            if (!item["Type"].Equals(fieldValue))
            {
              using (new EditContext(item, false, true))
              {
                item["Type"] = fieldValue;
              }
              CacheManager.GetItemCache(db).RemoveItem(item.ID);
            }            
          }          
        }
      }
    }
  }
}