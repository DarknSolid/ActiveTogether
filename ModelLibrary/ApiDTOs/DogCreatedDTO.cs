using ModelLib.Repositories;

namespace ModelLib.ApiDTOs
{
    public class DogCreatedDTO
    {
        public int Id { get; set; }
        public RepositoryEnums.ResponseType Response { get; set; }
    }
}
