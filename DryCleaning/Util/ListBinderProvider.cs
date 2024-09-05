using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DryCleaning.Util
{
    public class ListBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(List<DayOfWeek>))
            {
                return new ListDayWeekBinder();
            }

            if (context.Metadata.ModelType == typeof(List<DateOnly>))
            {
                return new ListDateBinder();
            }

            return null;
        }
    }
}
