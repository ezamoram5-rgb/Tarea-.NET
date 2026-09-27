using Microsoft.AspNetCore.Components.Forms;

namespace ClientesBlazor.Services;

public sealed class BootstrapValidation : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        if (editContext.GetValidationMessages(fieldIdentifier).Any()) return "is-invalid";
        return editContext.IsModified(fieldIdentifier) ? "is-valid" : "";
    }
}
