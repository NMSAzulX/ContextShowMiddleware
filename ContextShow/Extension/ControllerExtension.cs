using ContextShow;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerExtension
    {
        public static async Task Recoder(this ControllerBase controller,string content)
        {
            ContextShowMiddleware._response_contents[controller.Response] = content;
        }
    }
}
