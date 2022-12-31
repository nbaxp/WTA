using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace WTA.Infrastructure.Web.DataAnnotations;

public class LocalizedValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly ValidationAttributeAdapterProvider _originalProvider = new();

    public IAttributeAdapter? GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer? stringLocalizer)
    {
        if (string.IsNullOrEmpty(attribute.ErrorMessage))
        {
            attribute.ErrorMessage = attribute.GetType().Name;
        }
        return _originalProvider.GetAttributeAdapter(attribute, stringLocalizer);
    }
}
