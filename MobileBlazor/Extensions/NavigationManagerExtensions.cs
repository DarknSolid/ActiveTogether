using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileBlazor.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static void GoBack(this NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            jsRuntime.InvokeVoidAsync("history.back");
        }
    }
}
