using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;

namespace WTA.Infrastructure.Web.ModelBinding;

public class CustomDisplayMetadataProvider : IDisplayMetadataProvider
{
  public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
  {
    var attributes = context.Attributes;
    var displayAttribute = attributes.OfType<DisplayAttribute>().FirstOrDefault();
    if (displayAttribute != null && string.IsNullOrEmpty(displayAttribute.Name))
    {
      displayAttribute.Name = context.Key.Name;
    }
  }
}
