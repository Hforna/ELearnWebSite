using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sqids;

namespace User.Api.Binder
{
    public class BinderId : IModelBinder
    {
        private readonly SqidsEncoder<long> _sqIds;

        public BinderId(SqidsEncoder<long> sqIds) => _sqIds = sqIds;

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;

            var valueBinder = bindingContext.ValueProvider.GetValue(modelName);

            if (valueBinder == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueBinder);

            var value = valueBinder.FirstValue;

            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;

            var decodeId = _sqIds.Decode(value).Single();

            bindingContext.Result = ModelBindingResult.Success(decodeId);

            return Task.CompletedTask;
        }
    }
}
