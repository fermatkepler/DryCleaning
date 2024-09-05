using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace DryCleaning.Util
{
    public class ListDayWeekBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null || bindingContext.BindingSource == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var daysOfWeek = bindingContext.ValueProvider.GetValue("weekdays");
            var daysString = daysOfWeek.FirstValue;

            List<DayOfWeek> weekdays = [];
            if (!string.IsNullOrEmpty(daysString))
            {
                var daysArray = daysString.Split(',');

                foreach (var item in daysArray)
                {
                    if (Enum.TryParse(typeof(DayOfWeek), item, true, out object? wd))
                    {
                        weekdays.Add((DayOfWeek)wd);
                    }
                }
                bindingContext.Result = ModelBindingResult.Success(weekdays);
            }

            return Task.CompletedTask;
        }
    }
}
