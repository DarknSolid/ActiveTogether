using ModelLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace UnitTests
{
    public class PlacesRepositoryTests : TestBase
    {
        private readonly IPlacesRepository _placesRepository;

        public PlacesRepositoryTests()
        {
            _placesRepository = new PlacesRepository(_context);
        }

    }
}
