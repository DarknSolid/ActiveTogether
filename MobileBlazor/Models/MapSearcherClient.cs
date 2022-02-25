using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using RazorLib.AbstractClasses;
using RazorLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileBlazor.Models
{
    public class MapSearcherClient : MapSearcher
    {
        private readonly IApiClient _apiClient;

        public MapSearcherClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        protected override async Task<MapSearchResultDTO> FetchNewMapEntities(MapSearchDTO mapSearchDTO)
        {
            return await _apiClient.FetchMapMarkers(mapSearchDTO);
        }
    }
}
