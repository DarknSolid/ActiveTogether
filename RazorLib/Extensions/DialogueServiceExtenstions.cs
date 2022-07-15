using Microsoft.AspNetCore.Components;
using MudBlazor;
using RazorLib.Components.Dogs;

namespace RazorLib.Extensions
{
    public static class DialogueServiceExtenstions
    {
        /// <summary>
        /// A helper function to create a list view dialogue that allows the user to select multiple items.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="service"></param>
        /// <param name="title">The title of the dialogue</param>
        /// <param name="functionFetchItems">A function that fetches the list item objects to be displayed</param>
        /// <param name="functionBuildListItemContent">A function that builds the content of each list item. E.g. builds a DogCard with its parameters.</param>
        /// <param name="functionExtractItemId">A function that returns the id of the object given. E.g. the id of a DogListDTO</param>
        /// <returns>Returns the dialogue task which contains a list of ids of the selected items</returns>
        public static IDialogReference ShowListViewSelection<TComponent, TItem>(
                IDialogService service, 
                string title,
                Func<Task<List<TItem>>> functionFetchItems,
                Func<TItem, RenderFragment, RenderFragment> functionBuildListItemContent,
                Func<TItem, int> functionExtractItemId
            ) 
            where TComponent : ComponentBase
        {

            var parameters = new DialogParameters
            {
                [nameof(ItemSelectionDialogue<TItem>.ItemFetch)] = functionFetchItems,
                [nameof(ItemSelectionDialogue<TItem>.BuildListItemContent)] = functionBuildListItemContent,
                [nameof(ItemSelectionDialogue<TItem>.ExtractItemId)] = functionExtractItemId
            };

            var options = new DialogOptions { CloseOnEscapeKey = true };
            var dialog = service.Show<TComponent>(title, parameters, options);
            return dialog;
        }
    }
}
