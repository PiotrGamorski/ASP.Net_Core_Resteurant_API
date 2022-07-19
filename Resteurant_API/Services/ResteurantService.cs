using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resteurant_API.Authorization;
using Resteurant_API.DataContext;
using Resteurant_API.Dtos;
using Resteurant_API.Entities;
using Resteurant_API.Exceptions;
using Resteurant_API.Interfaces;
using Resteurant_API.Services.ContextServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resteurant_API.Services
{
    public class ResteurantService : IResteurantService
    {
        private readonly ResteurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ResteurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private const string databaseErrorMessage = "Database operation error occured";

        public ResteurantService(ResteurantDbContext dbContext, IMapper mapper, ILogger<ResteurantService> logger,
            IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }
        
        public async Task<IEnumerable<ResteurantDto>> GetAll()
        {
            try
            {
                var resteurants = await _dbContext.Resteurants
                .Include(r => r.Adress)
                .Include(r => r.Dishes)
                .ToListAsync();

                return _mapper.Map<List<ResteurantDto>>(resteurants);
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException("Database operation error occured");
            }
        }

        public async Task<ResteurantDto> GetById(int id)
        {
            try
            {
                var resteurant = await _dbContext.Resteurants
                .Include(r => r.Adress)
                .Include(r => r.Dishes)
                .FirstOrDefaultAsync(r => r.Id == id);

                if (resteurant is null)
                    throw new NotFoundException("Resteurant not found");

                return _mapper.Map<ResteurantDto>(resteurant);
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }

        public async Task<Resteurant> Create(CreateResteurantDto dto)
        {
            var isAddressUnique = !(await _dbContext.Resteurants
                .AnyAsync(r => 
                r.Adress.Street == dto.Street && 
                r.Adress.City == dto.City && 
                r.Adress.PostalCode == dto.PostalCode));

            if (!isAddressUnique) return null;

            var resteurant = _mapper.Map<Resteurant>(dto);
            resteurant.CreatedById = _userContextService.GetUserId;
            try
            {
                await _dbContext.Resteurants.AddAsync(resteurant);
                await _dbContext.SaveChangesAsync();

                return resteurant;
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogWarning($"Resteurant with id: {id} DELETE action invoked");

            var resteurant = await _dbContext.Resteurants
                .FirstOrDefaultAsync(r => r.Id == id);
            if (resteurant is null)
                throw new NotFoundException("Resteurant not found");

            var authorizationResult = await _authorizationService.AuthorizeAsync(_userContextService.User, resteurant,
                new ResourceOperationRequirement(ResourceOperation.Delete));
            if (!authorizationResult.Succeeded)
                throw new ForbidException();

            try
            {
                _dbContext.Resteurants.Remove(resteurant);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Resteurant with id: {id} successfully deleted");
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }

        public async Task Update(int id, UpdateResteurantDto dto)
        {
            var resteurant = await _dbContext.Resteurants
                .FirstOrDefaultAsync(r => r.Id == id);
            if (resteurant is null) 
                throw new NotFoundException(databaseErrorMessage);

            var authorizationResult = await _authorizationService.AuthorizeAsync(_userContextService.User, resteurant,
                new ResourceOperationRequirement(ResourceOperation.Update));
            if (!authorizationResult.Succeeded)
                throw new ForbidException();

            resteurant.Name = dto.Name;
            resteurant.Description = dto.Description;
            resteurant.HasDelivery = dto.HasDelivery;

            try
            {
                _dbContext.Resteurants.Update(resteurant);
                await _dbContext.SaveChangesAsync();
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }
    }
}
