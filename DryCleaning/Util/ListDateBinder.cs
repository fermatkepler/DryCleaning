using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace DryCleaning.Util
{
    public class ListDateBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null || bindingContext.BindingSource == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var dates = bindingContext.ValueProvider.GetValue("dates");
            var daysString = dates.FirstValue;

            List<DateOnly> datesArray = [];
            if (!string.IsNullOrEmpty(daysString))
            {
                var daysArray = daysString.Split(',');
                
                foreach (var item in daysArray)
                {
                    if (DateOnly.TryParseExact(item, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateOnly result))
                    {
                        datesArray.Add(result);
                    }
                }

                bindingContext.Result = ModelBindingResult.Success(datesArray);
            }

            return Task.CompletedTask;
        }
    }
}
