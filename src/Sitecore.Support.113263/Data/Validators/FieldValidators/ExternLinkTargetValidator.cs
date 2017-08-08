using HtmlAgilityPack;
using Sitecore.Data.Fields;
using Sitecore.Data.Validators;
using Sitecore.Globalization;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sitecore.Support.Data.Validators.FieldValidators
{
  [Serializable]
  public class ExternLinkTargetValidator : StandardValidator
  {
    // Methods
    public ExternLinkTargetValidator()
    {
    }

    public ExternLinkTargetValidator(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    protected override ValidatorResult Evaluate()
    {
      Field field = base.GetField();
      if (field == null)
      {
        return ValidatorResult.Valid;
      }
      string str = field.Value;
      if (string.IsNullOrEmpty(str))
      {
        return ValidatorResult.Valid;
      }
      if (string.Compare(str, "<a", StringComparison.InvariantCulture) < 0)
      {
        return ValidatorResult.Valid;
      }
      HtmlDocument document = new HtmlDocument();
      document.LoadHtml(str);
      HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//a");
      if (nodes == null)
      {
        return ValidatorResult.Valid;
      }
      foreach (HtmlNode node in (IEnumerable<HtmlNode>)nodes)
      {
        string attributeValue = node.GetAttributeValue("href", string.Empty);
        if (!string.IsNullOrEmpty(attributeValue) && WebUtil.IsExternalUrl(attributeValue))
        {
          string str3 = node.GetAttributeValue("target", string.Empty);
          if (!attributeValue.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase))
          {
            if (string.IsNullOrEmpty(str3) || (str3 == "_self"))
            {
              base.Errors.Add(Translate.Text("The field \"{0}\" contains an external link to \"{1}\". The link should have a target, so that it opens in a new browser.", new object[] { field.DisplayName, attributeValue }));
            }
            else if (string.IsNullOrEmpty(node.GetAttributeValue("title", string.Empty)))
            {
              base.Errors.Add(Translate.Text("The field \"{0}\" contains an external link to \"{1}\" which opens in a new browser. The title of the link is empty, but should explain that the link opens a new browser.", new object[] { field.DisplayName, attributeValue }));
            }
          }          
        }
      }
      if (base.Errors.Count == 0)
      {
        return ValidatorResult.Valid;
      }
      base.Text = base.Errors[0];
      return base.GetFailedResult(ValidatorResult.Warning);
    }

    protected override ValidatorResult GetMaxValidatorResult()
    {
      return base.GetFailedResult(ValidatorResult.Warning);
    }

    // Properties
    public override string Name
    {
      get
      {
        return "Extern Link Target";
      }
    }
  }

}