using EntityLib;
using EntityLib.Entities;
using FisSst.BlazorMaps;
using Microsoft.EntityFrameworkCore;
using ModelLib.ApiDTOs;
using ModelLib.ApiDTOs.Pagination;
using ModelLib.DTOs;
using ModelLib.DTOs.Authentication;
using ModelLib.DTOs.Instructors;
using System.Security.Cryptography.X509Certificates;
using static EntityLib.Entities.Enums;
using static ModelLib.DTOEnums;
using static ModelLib.Repositories.RepositoryEnums;

namespace ModelLib.Repositories
{
    public interface IDogInstructorRepository
    {
        Task<(ResponseType, int)> CreateInstructorAsync(int userId, InstructorCreateDTO createDto);
        Task<(ResponseType, InstructorDetailedDTO?)> GetInstructorAsync(int? userId, int instructorId);
        Task<DistancePaginationResult<DogTrainerListDTO>> GetInstructorsAsync(DogTrainerRequest request);
        Task<IEnumerable<DogTrainerListDTO>> GetInstructorsInAreaAsync(SearchAreaDTO searchArea);
        Task<ResponseType> UpdateInstructorAsync(int userId, InstructorUpdateDTO updateDto);

        Task<(ResponseType, int)> CreateDogTrainingAsync(int userId, DogTrainingCreateDTO createDTO);
        Task<ResponseType> UpdateDogTrainingAsync(int userId, DogTrainingUpdateDTO updateDto);
        Task<ResponseType> DeleteDogTrainingAsync(int userId, int dogTrainingId);
        Task<(ResponseType, DogTrainingDetailsDTO?)> GetDogTrainingAsync(int id);
        Task<DistancePaginationResult<DogTrainingListDTO>> GetDogTrainingsAsync(DogTrainingRequest request);
        Task<IEnumerable<DogTrainingListDTO>> GetUpcommingDogTrainingsAsync(int instructorCompanyId);
        Task<AllDogTrainingsInstructorListDTO> GetAllDogTrainingsAsync(int instructorCompanyId);
        Task<ResponseType> DeleteInstructorAsync(int userId);
        Task<Dictionary<int, string>> GetTrainerNamesAsync();
    }

    public class DogInstructorRepository : RepositoryBase, IDogInstructorRepository
    {
        private readonly IPlacesRepository _placesRepository;
        private readonly IBlobStorageRepository _blobStorageRepository;

        public DogInstructorRepository(IApplicationDbContext context, IPlacesRepository placesRepository, IBlobStorageRepository blobStorageRepository) : base(context)
        {
            _placesRepository = placesRepository;
            _blobStorageRepository = blobStorageRepository;
        }

        public async Task<(ResponseType, int)> CreateInstructorAsync(int userId, InstructorCreateDTO createDto)
        {
            var existingEntity = await _context.Places.
                FirstOrDefaultAsync(p => p.FacilityType == Enums.FacilityType.DogInstructor && p.Name == createDto.Name);

            if (existingEntity != null)
            {
                return (ResponseType.Conflict, -1);
            }

            var existingInstructor = await _context.Companies.Where(c => c.ApplicationUserId == userId).FirstOrDefaultAsync();
            if (existingInstructor != null)
            {
                return (ResponseType.Conflict, -1);
            }

            var categories = createDto.Categories
                .Select(c =>
                    new InstructorCompanyCategory()
                    {
                        InstructorCategory = c,
                    })
                .ToList();

            var facilities = createDto.InstructorFacilities
                .Select(f =>
                    new InstructorCompanyFacility()
                    {
                        InstructorFacility = f
                    })
                .ToList();

            var place = new Place()
            {
                Name = createDto.Name,
                Description = createDto.Description,
                FacilityType = Enums.FacilityType.DogInstructor,
                Location = new(x: createDto.Point.X, y: createDto.Point.Y),
                Company = new Company()
                {
                    ApplicationUserId = userId,
                    Email = createDto.Email,
                    Phone = createDto.PhoneNumber,
                    InstructorCategories = categories,
                    InstructorFacilities = facilities,
                    CompanyURL = createDto.CompanyURL
                }
            };

            _context.Places.Add(place);
            await _context.SaveChangesAsync();

            if (createDto.ProfileImage != null &&
                createDto.ProfileImage.ContainsFile())
            {
                var (response, url) = await _blobStorageRepository.UploadPublicImageAsync(
                    createDto.ProfileImage,
                    BlobStorageRepository.PlaceProfileImagePath,
                    place.Id
                );
                if (url != null)
                {
                    place.ProfileImageBlobUrl = url;
                    await _context.SaveChangesAsync();
                }
            }

            if (createDto.CoverImage != null &&
                createDto.CoverImage.ContainsFile())
            {
                var (response, url) = await _blobStorageRepository.UploadPublicImageAsync(
                    createDto.CoverImage,
                    BlobStorageRepository.CompanyCoverImagePath,
                    place.Id
                    );
                if (url != null)
                {
                    place.Company.CoverImageBlobUrl = url;
                    await _context.SaveChangesAsync();
                }
            }

            return (ResponseType.Created, place.Id);
        }

        public async Task<ResponseType> UpdateInstructorAsync(int userId, InstructorUpdateDTO updateDto)
        {

            var existingEntity = await _context.Companies
                .Include(c => c.Place)
                .Include(c => c.InstructorFacilities)
                .Include(c => c.InstructorCategories)
                .FirstOrDefaultAsync
                    (
                        c => c.ApplicationUserId == userId &&
                        c.PlaceId == updateDto.InstructorCompanyId &&
                        c.Place.FacilityType == Enums.FacilityType.DogInstructor
                    );
            if (existingEntity == null)
            {
                return (ResponseType.NotFound);
            }

            // check for existing name
            if (await _context.Places.FirstOrDefaultAsync(p => p.Name == updateDto.Name && p.Id != updateDto.InstructorCompanyId) is not null)
            {
                return ResponseType.Conflict;
            }
            existingEntity.Place.Name = updateDto.Name;

            existingEntity.Place.Description = updateDto.Description;

            existingEntity.CompanyURL = updateDto.CompanyURL;

            existingEntity.Email = updateDto.Email;

            existingEntity.Phone = updateDto.PhoneNumber;

            //Facilities
            // add new facilities to the DB
            foreach (var facility in updateDto.InstructorFacilities)
            {
                if (!existingEntity.InstructorFacilities.Any(f => f.InstructorFacility == facility))
                {
                    _context.InstructorCompanyFacilities.Add(new InstructorCompanyFacility { InstructorCompany = existingEntity, InstructorFacility = facility });
                }
            }
            // remove existing facilities in the DB
            foreach (var facility in existingEntity.InstructorFacilities)
            {
                if (!updateDto.InstructorFacilities.Contains(facility.InstructorFacility))
                {
                    _context.InstructorCompanyFacilities.Remove(facility);
                }
            }

            // Categories
            // add new categories to the DB
            foreach (var category in updateDto.Categories)
            {
                if (!existingEntity.InstructorCategories.Any(f => f.InstructorCategory == category))
                {
                    _context.InstructorCompanyCategories.Add(new InstructorCompanyCategory { InstructorCompany = existingEntity, InstructorCategory = category });
                }
            }
            // remove existing categories in the DB
            foreach (var category in existingEntity.InstructorCategories)
            {
                if (!updateDto.Categories.Contains(category.InstructorCategory))
                {
                    _context.InstructorCompanyCategories.Remove(category);
                }
            }

            if (updateDto.CoverImage != null)
            {
                var path = BlobStorageRepository.CompanyCoverImagePath;
                if (updateDto.CoverImage.IsDeleteCommand)
                {
                    await _blobStorageRepository.DeletePublicImageAsync(path, existingEntity.PlaceId);
                    existingEntity.CoverImageBlobUrl = null;
                }
                else
                {
                    existingEntity.CoverImageBlobUrl = (await _blobStorageRepository.UploadPublicImageAsync(updateDto.CoverImage, path, existingEntity.PlaceId)).Item2;
                }
            }

            if (updateDto.ProfileImage != null)
            {
                var path = BlobStorageRepository.PlaceProfileImagePath;
                if (updateDto.ProfileImage.IsDeleteCommand)
                {
                    await _blobStorageRepository
                        .DeletePublicImageAsync(path, existingEntity.PlaceId);
                    existingEntity.Place.ProfileImageBlobUrl = null;
                }
                else
                {
                    var (_, url) = await _blobStorageRepository
                        .UploadPublicImageAsync(updateDto.ProfileImage, path, existingEntity.PlaceId);
                    existingEntity.Place.ProfileImageBlobUrl = url;
                }
            }

            existingEntity.Place.Location = new(x: updateDto.Point.X, y: updateDto.Point.Y);

            await _context.SaveChangesAsync();
            return ResponseType.Updated;

        }

        public async Task<(ResponseType, InstructorDetailedDTO?)> GetInstructorAsync(int? userId, int instructorId)
        {
            var reviews = _context.Reviews.Where(r => r.PlaceId == instructorId);

            var reviewStatus = ReviewStatus.CanReview;
            if (userId is not null)
            {
                if (await reviews.AnyAsync(r => r.UserId == userId.Value))
                {
                    reviewStatus = ReviewStatus.CanUpdateReview;
                }
            }

            var instructor = await _context.Companies
                .Include(c => c.Place)
                .Include(c => c.InstructorCategories)
                .Include(c => c.InstructorFacilities)
                .Include(c => c.User)
                .Where(c => c.PlaceId == instructorId && c.Place.FacilityType == FacilityType.DogInstructor)
                .Select(c => new InstructorDetailedDTO
                {
                    Categories = c.InstructorCategories.Select(c => c.InstructorCategory),
                    Facilities = c.InstructorFacilities.Select(c => c.InstructorFacility),
                    Rating = reviews.Average(r => r.Rating),
                    RatingCount = reviews.Count(),
                    Email = c.Email,
                    Name = c.Place.Name,
                    Description = c.Place.Description,
                    ActiveCategories = _context.DogTrainings
                                .Where(c => c.InstructorCompanyId == instructorId && c.TrainingTimes[0].Date >= DateTime.UtcNow)
                                .Select(c => c.Category)
                                .Distinct().ToList(),
                    Phone = c.Phone,
                    Location = new(c.Place.Location.X, c.Place.Location.Y),
                    OwnerId = c.ApplicationUserId,
                    CompanyURL = c.CompanyURL,
                    ProfileImgUrl = c.Place.ProfileImageBlobUrl,
                    CoverImgUrl = c.CoverImageBlobUrl,
                    CurrentReviewStatus = reviewStatus,
                    Id = instructorId,
                    Facility = c.Place.FacilityType,
                    OwnerName = c.User.FirstName + " " + c.User.LastName,
                    OwnerProfilePictureUrl = c.User.ProfileImageUrl

                })
                .FirstOrDefaultAsync();

            if (instructor == null)
            {
                return (ResponseType.NotFound, null);
            }

            instructor.ProfileImgUrl = (await _blobStorageRepository.GetPublicImageUrl(instructor.ProfileImgUrl)).Item2?.ToString();

            instructor.CoverImgUrl = (await _blobStorageRepository.GetPublicImageUrl(instructor.CoverImgUrl)).Item2?.ToString();

            instructor.OwnerProfilePictureUrl = (await _blobStorageRepository.GetPublicImageUrl(instructor.OwnerProfilePictureUrl)).Item2?.ToString();

            return (ResponseType.Ok, instructor);
        }

        public async Task<IEnumerable<DogTrainerListDTO>> GetInstructorsInAreaAsync(SearchAreaDTO searchArea)
        {
            var query = _context.Places
                .Where(p => p.FacilityType == FacilityType.DogInstructor);

            var places = _placesRepository.GetPlacesInArea(query, searchArea: searchArea);
            var placeDTOs = await places
                .Select(p => new DogTrainerListDTO
                {
                    Name = p.Name,
                    Location = new(p.Location.X, p.Location.Y),
                    Rating = _context.Reviews.Where(r => r.PlaceId == p.Id).Average(p => p.Rating),
                    RatingCount = _context.Reviews.Where(r => r.PlaceId == p.Id).Count(),
                    Id = p.Id,
                    Facility = p.FacilityType,
                    ProfileImgUrl = p.ProfileImageBlobUrl,
                })
                .ToListAsync();

            foreach (var trainer in placeDTOs)
            {
                trainer.ProfileImgUrl = (await _blobStorageRepository.GetPublicImageUrl(trainer.ProfileImgUrl)).Item2?.ToString();
            }

            return placeDTOs;
        }

        public async Task<DistancePaginationResult<DogTrainerListDTO>> GetInstructorsAsync(DogTrainerRequest request)
        {
            var query = _context.Places
                .Include(p => p.Company)
                .Include(p => p.Company.InstructorCategories)
                .Where(p => p.FacilityType == FacilityType.DogInstructor);

            //filters
            if (request.SearchFilter is not null)
            {
                if (request.SearchFilter.Category is not null)
                {
                    query = query.Where(p => p.Company.InstructorCategories.Any(c => c.InstructorCategory == request.SearchFilter.Category.Value));
                }
            }

            var point = new NetTopologySuite.Geometries.Point(request.SearchArea.Center.Lng, request.SearchArea.Center.Lat);
            var select = (IQueryable<Place> q) => q.Select(p => new DogTrainerListDTO
            {
                Name = p.Name,
                Location = new(p.Location.X, p.Location.Y),
                Rating = _context.Reviews.Where(r => r.PlaceId == p.Id).Average(p => p.Rating),
                RatingCount = _context.Reviews.Where(r => r.PlaceId == p.Id).Count(),
                ProfileImgUrl = p.ProfileImageBlobUrl,
                CoverImgUrl = p.Company.CoverImageBlobUrl,
                Id = p.Id,
                Facility = p.FacilityType,
                DistanceMeters = (float)EF.Functions.Distance(p.Location, point, true),
                Categories = p.Company.InstructorCategories.Select(c => c.InstructorCategory).ToArray()
            });

            var paginationResult = await _placesRepository.GetPlacesByDistance(query, select, request);

            foreach (var instructor in paginationResult.Result)
            {
                instructor.ProfileImgUrl = (await _blobStorageRepository.GetPublicImageUrl(instructor.ProfileImgUrl)).Item2?.ToString();
                instructor.CoverImgUrl = (await _blobStorageRepository.GetPublicImageUrl(instructor.CoverImgUrl)).Item2?.ToString();
            }

            return paginationResult;
        }

        public async Task<(ResponseType, int)> CreateDogTrainingAsync(int userId, DogTrainingCreateDTO createDTO)
        {
            //only instructor admin can create:
            var admin = await _context.Companies.Where(c => c.ApplicationUserId == userId).FirstOrDefaultAsync();
            if (admin == null)
            {
                return (ResponseType.Unauthorized, -1);
            }
            var dogTraining = new DogTraining
            {
                Title = createDTO.Title,
                Description = createDTO.Description,
                OriginalTrainingWebsiteUri = createDTO.OriginalDogWebsiteUrl,
                Category = createDTO.Category,
                AssignedInstructorId = userId,
                Location = new NetTopologySuite.Geometries.Point(x: createDTO.Location.Lng, y: createDTO.Location.Lat),
                InstructorCompanyId = admin.PlaceId,
                MaxParticipants = createDTO.MaxParticipants,
                Price = createDTO.Price,
                RegistrationDeadline = createDTO.RegistrationDeadline.ToUniversalTime(),
                TrainingTimes = createDTO.TrainingTimes
                                .SelectMany(t => new DateTime[] { t.Start.ToUniversalTime(), t.End.ToUniversalTime() })
                                .ToArray(),
            };
            _context.DogTrainings.Add(dogTraining);
            await _context.SaveChangesAsync();

            if (createDTO.CoverImage is not null && createDTO.CoverImage.ContainsFile())
            {
                var (_, url) = await _blobStorageRepository.UploadPublicImageAsync(
                    createDTO.CoverImage,
                    BlobStorageRepository.TrainingCoverImagePath,
                    dogTraining.Id);

                if (url != null)
                {
                    dogTraining.CoverImgageBlobUrl = url;
                    await _context.SaveChangesAsync();
                }
            }

            return (ResponseType.Created, dogTraining.Id);
        }

        public async Task<(ResponseType, DogTrainingDetailsDTO?)> GetDogTrainingAsync(int id)
        {
            var entity = await _context.DogTrainings
                .Include(d => d.InstructorCompany)
                .Include(d => d.InstructorCompany.Place)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (entity is null)
            {
                return (ResponseType.NotFound, null);
            }
            var dto = new DogTrainingDetailsDTO
            {
                DogTrainingId = id,
                Title = entity.Title,
                InstructorRating = _context.Reviews.Where(r => r.PlaceId == entity.InstructorCompanyId).Sum(r => r.Rating) / Math.Max(1, _context.Reviews.Count(r => r.PlaceId == entity.InstructorCompanyId)),
                InstructorRatingCount = _context.Reviews.Where(r => r.PlaceId == entity.InstructorCompanyId).Count(),
                Description = entity.Description,
                Category = entity.Category,
                InstructorCompanyLogoUri = (await _blobStorageRepository.GetPublicImageUrl(entity.InstructorCompany.Place.ProfileImageBlobUrl)).Item2?.ToString(),
                InstructorCompanyName = entity.InstructorCompany.Place.Name,
                Location = new LatLng(lat: entity.Location.Y, lng: entity.Location.X),
                MaxParticipants = entity.MaxParticipants,
                Price = entity.Price,
                RegistrationDeadline = entity.RegistrationDeadline,
                OriginalDogTrainingWebsiteUri = entity.OriginalTrainingWebsiteUri,
                InstructorCompanyId = entity.InstructorCompanyId,
                CoverImgUrl = (await _blobStorageRepository.GetPublicImageUrl(entity.CoverImgageBlobUrl)).Item2?.ToString()
            };

            dto.TrainingTimes = entity.TrainingTimes
                .Chunk(2)
                .Select(x => new TrainingTime
                {
                    Start = x[0],
                    End = x[1],
                }).ToArray();

            return (ResponseType.Ok, dto);
        }

        public async Task<DistancePaginationResult<DogTrainingListDTO>> GetDogTrainingsAsync(DogTrainingRequest request)
        {
            var query = _context.DogTrainings
                .Include(d => d.InstructorCompany)
                .Include(d => d.InstructorCompany.Place)
                .Where(d => d.RegistrationDeadline > DateTime.UtcNow); // the order of where clauses does not matter, as SQL is declarative and the optimizer will figure out how to make the query the most efficient.

            // filters
            if (request.SearchFilter is not null)
            {
                if (request.SearchFilter?.Category is not null)
                {
                    query = query.Where(d => request.SearchFilter.Category == d.Category);
                }

                if (request.SearchFilter?.AfterTime is not null)
                {
                    query = query.Where(t => t.TrainingTimes[0].Hour > request.SearchFilter.AfterTime.Value.Hours || (t.TrainingTimes[0].Hour == request.SearchFilter.AfterTime.Value.Hours && t.TrainingTimes[0].Minute >= request.SearchFilter.AfterTime.Value.Minutes));
                }

                if (request.SearchFilter?.BeforeTime is not null)
                {
                    query = query.Where(t => t.TrainingTimes[1].Hour < request.SearchFilter.BeforeTime.Value.Hours || (t.TrainingTimes[1].Hour == request.SearchFilter.BeforeTime.Value.Hours && t.TrainingTimes[1].Minute <= request.SearchFilter.BeforeTime.Value.Minutes));
                }

                if (request.SearchFilter?.Day is not null)
                {
                    query = query.Where(t => t.TrainingTimes[0].DayOfWeek == request.SearchFilter.Day);
                }

                if (request.SearchFilter?.TrainerId is not null)
                {
                    query = query.Where(t => t.InstructorCompanyId == request.SearchFilter.TrainerId);
                }
            }

            var point = new NetTopologySuite.Geometries.Point(request.SearchArea.Center.Lng, request.SearchArea.Center.Lat);
            var select = (IQueryable<DogTraining> q) => q.Select(d => new DogTrainingListDTO
            {
                TrainingTimes = d.TrainingTimes.Length / 2,
                FirstTrainingDate = d.TrainingTimes[0],
                InstructorCompanyId = d.InstructorCompanyId,
                Category = d.Category,
                Id = d.Id,
                InstructorCompanyLogoUri = d.InstructorCompany.Place.ProfileImageBlobUrl, //TODO
                InstructorCompanyName = d.InstructorCompany.Place.Name,
                MaxParticipants = d.MaxParticipants,
                Price = d.Price,
                RegistrationDeadline = d.RegistrationDeadline,
                Title = d.Title,
                DistanceMeters = (float)EF.Functions.Distance(d.Location, point, true),
                CoverImgUrl = d.CoverImgageBlobUrl,
                Location = new(d.Location.X, d.Location.Y),
            });

            var paginationResult = await _placesRepository.GetPlacesByDistance(query, select, request);



            foreach (var training in paginationResult.Result)
            {
                training.CoverImgUrl = (await _blobStorageRepository.GetPublicImageUrl(training.CoverImgUrl)).Item2?.ToString();
                training.InstructorCompanyLogoUri = (await _blobStorageRepository.GetPublicImageUrl(training.InstructorCompanyLogoUri)).Item2?.ToString();
            }

            return paginationResult;
        }

        public async Task<IEnumerable<DogTrainingListDTO>> GetUpcommingDogTrainingsAsync(int instructorCompanyId)
        {
            var result = await _context.DogTrainings
                .Include(d => d.InstructorCompany)
                .Include(d => d.InstructorCompany.Place)
                .Where(d => d.InstructorCompanyId == instructorCompanyId && d.RegistrationDeadline >= DateTime.UtcNow)
                .Select(d => new DogTrainingListDTO
                {
                    InstructorCompanyId = instructorCompanyId,
                    FirstTrainingDate = d.TrainingTimes[0],
                    InstructorCompanyLogoUri = d.InstructorCompany.Place.ProfileImageBlobUrl,
                    InstructorCompanyName = d.InstructorCompany.Place.Name,
                    TrainingTimes = d.TrainingTimes.Length / 2,
                    Id = d.Id,
                    CoverImgUrl = d.CoverImgageBlobUrl,
                    Title = d.Title,
                    Category = d.Category,
                    RegistrationDeadline = d.RegistrationDeadline,
                    MaxParticipants = d.MaxParticipants,
                    Price = d.Price,
                }
                )
                .OrderBy(d => d.RegistrationDeadline)
                .ToListAsync();


            foreach (var training in result)
            {
                training.CoverImgUrl = (await _blobStorageRepository.GetPublicImageUrl(training.CoverImgUrl)).Item2?.ToString();
                training.InstructorCompanyLogoUri = (await _blobStorageRepository.GetPublicImageUrl(training.InstructorCompanyLogoUri)).Item2?.ToString();
            }

            return result;
        }

        public async Task<ResponseType> UpdateDogTrainingAsync(int userId, DogTrainingUpdateDTO updateDto)
        {
            // only admin
            var training = await _context.DogTrainings
                .Include(d => d.InstructorCompany)
                .Where(d => d.Id == updateDto.DogTrainingId)
                .FirstOrDefaultAsync();
            if (training is null)
            {
                return ResponseType.NotFound;
            }
            else if (training.InstructorCompany.ApplicationUserId != userId)
            {
                return ResponseType.Unauthorized;
            }

            training.Category = updateDto.Category;
            training.Description = updateDto.Description;
            training.MaxParticipants = updateDto.MaxParticipants;
            training.Location = new NetTopologySuite.Geometries.Point(x: updateDto.Location.Lng, y: updateDto.Location.Lat);
            training.Price = updateDto.Price;
            training.RegistrationDeadline = updateDto.RegistrationDeadline.ToUniversalTime();
            training.OriginalTrainingWebsiteUri = updateDto.OriginalDogWebsiteUrl;
            training.Title = updateDto.Title;
            training.TrainingTimes = updateDto.TrainingTimes.SelectMany(t => new DateTime[] { t.Start.ToUniversalTime(), t.End.ToUniversalTime() }).ToArray();

            if (updateDto.CoverImage is not null)
            {
                var path = BlobStorageRepository.TrainingCoverImagePath;
                if (updateDto.CoverImage.IsDeleteCommand)
                {
                    await _blobStorageRepository.DeletePublicImageAsync(path, training.Id);
                }
                else if (updateDto.CoverImage.ContainsFile())
                {
                    training.CoverImgageBlobUrl = (await _blobStorageRepository.UploadPublicImageAsync(updateDto.CoverImage, path, training.Id)).Item2;
                }
            }

            await _context.SaveChangesAsync();

            return ResponseType.Updated;
        }

        public async Task<ResponseType> DeleteDogTrainingAsync(int userId, int dogTrainingId)
        {
            // only admin
            var training = await _context.DogTrainings
                .Include(d => d.InstructorCompany)
                .FirstOrDefaultAsync(d => d.Id == dogTrainingId);

            if (training == null)
            {
                return ResponseType.NotFound;
            }
            else if (training.InstructorCompany.ApplicationUserId != userId)
            {
                return ResponseType.Unauthorized;
            }

            _context.DogTrainings.Remove(training);
            await _context.SaveChangesAsync();

            if (training.CoverImgageBlobUrl != null)
            {
                await _blobStorageRepository.DeletePublicImageAsync(training.CoverImgageBlobUrl);
            }

            return ResponseType.Deleted;

        }

        public async Task<AllDogTrainingsInstructorListDTO> GetAllDogTrainingsAsync(int instructorCompanyId)
        {
            // TODO optimize by not including all fields
            var allTrainings = _context.DogTrainings
                .Include(d => d.InstructorCompany)
                .Include(d => d.InstructorCompany.Place)
                .Where(d => d.InstructorCompanyId == instructorCompanyId)
                .OrderBy(d => d.RegistrationDeadline);

            return new AllDogTrainingsInstructorListDTO()
            {
                UpcommingTrainings = await DogTrainingToListDTO(allTrainings.Where(t => DateTime.UtcNow < t.RegistrationDeadline)),
                ActiveTrainings = await DogTrainingToListDTO(allTrainings.Where(t => t.RegistrationDeadline <= DateTime.UtcNow && DateTime.UtcNow <= t.TrainingTimes[t.TrainingTimes.Length - 1])),
                FinishedTrainings = await DogTrainingToListDTO(allTrainings.Where(t => t.TrainingTimes[t.TrainingTimes.Length - 1] < DateTime.UtcNow))
            };
        }

        private async Task<IEnumerable<DogTrainingListDTO>> DogTrainingToListDTO(IQueryable<DogTraining> dogTrainings)
        {
            var result = await dogTrainings
                .Select(d => new DogTrainingListDTO
                {
                    Id = d.Id,
                    Title = d.Title,
                    Category = d.Category,
                    RegistrationDeadline = d.RegistrationDeadline,
                    FirstTrainingDate = d.TrainingTimes[d.TrainingTimes.Length - 1],
                    MaxParticipants = d.MaxParticipants,
                    Price = d.Price,
                    TrainingTimes = d.TrainingTimes.Length / 2,
                    InstructorCompanyId = d.InstructorCompanyId,
                    InstructorCompanyLogoUri = d.InstructorCompany.Place.ProfileImageBlobUrl,
                    InstructorCompanyName = d.InstructorCompany.Place.Name,
                    CoverImgUrl = d.CoverImgageBlobUrl,
                }).ToListAsync();

            foreach (var training in result)
            {
                training.CoverImgUrl = (await _blobStorageRepository.GetPublicImageUrl(training.CoverImgUrl)).Item2?.ToString();
                training.InstructorCompanyLogoUri = (await _blobStorageRepository.GetPublicImageUrl(training.InstructorCompanyLogoUri)).Item2?.ToString();

            }
            return result;
        }

        public async Task<ResponseType> DeleteInstructorAsync(int userId)
        {
            var company = await _context.Companies.Where(c => c.ApplicationUserId == userId).FirstOrDefaultAsync();
            if (company == null)
            {
                return ResponseType.NotFound;
            }

            // TODO - do we really want to delete?
            if (company.CoverImageBlobUrl != null)
            {
                await _blobStorageRepository.DeletePublicImageAsync(company.CoverImageBlobUrl);
            }
            await _blobStorageRepository.DeletePublicImageAsync(BlobStorageRepository.PlaceProfileImagePath, company.PlaceId);

            foreach (var dogTrainingCoverImgUrl in await _context.DogTrainings.Where(d => d.InstructorCompanyId == company.PlaceId).Select(d => d.CoverImgageBlobUrl).ToListAsync())
            {
                if (dogTrainingCoverImgUrl != null)
                {
                    await _blobStorageRepository.DeletePublicImageAsync(dogTrainingCoverImgUrl);
                }
            }

            _context.Places.RemoveRange(_context.Places.Where(c => c.Id == company.PlaceId));
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();


            return ResponseType.Deleted;
        }

        public async Task<Dictionary<int, string>> GetTrainerNamesAsync()
        {
            var result = new Dictionary<int, string>();
            var dtos = await _context.Companies
                .Include(c => c.Place)
                .Where(c => c.Place.FacilityType == FacilityType.DogInstructor)
                .Select(c => new { c.PlaceId, c.Place.Name })
                .ToListAsync();

            dtos.ForEach(a => result.Add(a.PlaceId, a.Name));
            return result;
        }

    }
}
