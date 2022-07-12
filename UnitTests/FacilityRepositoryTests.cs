using ModelLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace UnitTests
{
    public class FacilityRepositoryTests : TestBase
    {
        private readonly IFacilityRepository _facilityRepository;

        public FacilityRepositoryTests()
        {
            _facilityRepository = new FacilityRepository(_context);
        }

        [Theory]
        [InlineData(1, FacilityType.DogPark, true)]
        [InlineData(0, FacilityType.DogPark, false)]
        public async Task FacilityExists_Given_Existing_And_Non_Existing_Returns_Expected(int facilityId, FacilityType facilityType, bool expected)
        {
            var actual = await _facilityRepository.FacilityExists(facilityId, facilityType);
            Assert.Equal(expected, actual);
        }
    }
}
