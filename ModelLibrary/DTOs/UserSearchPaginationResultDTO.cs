using ModelLib.ApiDTOs;
using ModelLib.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.DTOs
{
    public class UserSearchPaginationResultDTO
    {
        public PaginationResult Pagination { get; set; }
        public List<UserListDTO> Users { get; set; }
    }
}
