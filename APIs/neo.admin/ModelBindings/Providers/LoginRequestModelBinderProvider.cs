using Microsoft.AspNetCore.Mvc.ModelBinding;
using neo.admin.Models;

namespace neo.admin.ModelBindings.Providers
{
    public class LoginRequestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (typeof(ILoginRequestModel).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new LoginRequestModelBinder();
            }

            return null;
        }
    }
}
