using Microsoft.AspNetCore.Mvc.ModelBinding;
using neo.admin.Models;
using System.Text.Json;

namespace neo.admin.ModelBindings
{
    public class LoginRequestModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;
            var clientType = request.Headers["X-Client-Type"].FirstOrDefault()?.ToLower();

            request.EnableBuffering(); // Important: allows reading the body multiple times

            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0; // Reset stream for downstream components

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            ILoginRequestModel? model = clientType switch
            {
                "web" => JsonSerializer.Deserialize<LoginRequestWebModel>(body, options),
                "mobile" => JsonSerializer.Deserialize<LoginRequestModelBase>(body, options),
                _ => null
            };

            if (model == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}