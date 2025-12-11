using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace MyRecipeBook.API.Filters
{
    public class CultureActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var lang = context.HttpContext.Request.Headers["Accept-Language"].ToString();

            CultureInfo culture = new("en");

            if (!string.IsNullOrWhiteSpace(lang))
            {
                try
                {
                    culture = new CultureInfo(lang);
                }
                catch
                {
                    // fallback para 'en' se cultura for inválida
                    culture = new CultureInfo("en");
                }
            }

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}

