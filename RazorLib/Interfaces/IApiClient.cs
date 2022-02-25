﻿using ModelLib.ApiDTOs;
using ModelLib.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DTOs.Authentication;

namespace RazorLib.Interfaces
{
    public interface IApiClient
    {
        Task<MapSearchResultDTO> FetchMapMarkers(MapSearchDTO mapSearchDTO);
    }
}
