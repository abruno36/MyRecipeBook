using Microsoft.AspNetCore.Mvc;

namespace MyRecipeBook.API.Communication.Requests
{
    public class UpdateImageForm
    {
        [FromForm(Name = "file")]
        public IFormFile? File { get; set; }
    }
}
