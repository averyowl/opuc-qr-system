/*
    Hunter Gambee-Iddings
    Natalya Langford
    Nathan Waggoner
    Philip Grazhdan
    Sean McCoy
    Wenkang Yu Zhen
*/
using Microsoft.AspNetCore.Components;

namespace BadAddressService.Pages
{
    public partial class BadAddress : ComponentBase
    {
        [Parameter]
        public int CustomerId { get; set; }

        // Use protected so the Razor file can access these members.
        protected string customerName = "Sean McCoy";
        protected string customerAddress = "1825 SW Broadway, Portland, OR 97201";
        protected bool badAddress = false;

        protected void HandleInvalidAddress()
        {
            badAddress = true;
        }

    }
}