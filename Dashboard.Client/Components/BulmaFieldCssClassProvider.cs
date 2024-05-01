using Microsoft.AspNetCore.Components.Forms;

namespace Dashboard.Client.Components;

public class BulmaFieldCssClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext, in FieldIdentifier fieldIdentifier)
    {
        var isValid = editContext.IsValid(fieldIdentifier);

        if (!isValid)
        {
            return "is-danger";
        }

        if (editContext.IsModified(fieldIdentifier))
        {
            return "is-success";
        }

        return "";
    }
}
