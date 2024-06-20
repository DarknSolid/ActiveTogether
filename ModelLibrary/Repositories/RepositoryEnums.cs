using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.Repositories
{
    public class RepositoryEnums
    {
        public enum ResponseType
        {
            Ok,
            Conflict,
            Updated,
            Created,
            NotFound,
            Deleted,
            Unauthorized,
            Duplicate,
        }

        public enum FriendShipStatus
        {
            NotFriends,
            Friends,
            PendingRequest,
            CanAcceptRequest
        }
    }
}
