using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Resteurant_API.DataContext;
using Resteurant_API.Dtos;
using Resteurant_API.Entities;
using Resteurant_API.Exceptions;
using Resteurant_API.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resteurant_API.Services
{
    public class DishService : IDishService
    {
        private readonly ResteurantDbContext _context;
        private readonly IMapper _mapper;
        private const string databaseErrorMessage = "Database operation error occured";

        public DishService(ResteurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Private Methods
        private async Task<Resteurant> GetResteurantById(int resteurantId, bool includeDishes = false)
        {
            Resteurant resteurant;
            if (includeDishes)
            {
                resteurant = await _context.Resteurants
                    .Include(r => r.Dishes)
                    .FirstOrDefaultAsync(r => r.Id == resteurantId);
            }
            else resteurant = await _context.Resteurants.FirstOrDefaultAsync(r => r.Id == resteurantId);

            if (resteurant is null)
                throw new NotFoundException("Resteurant Not Found");

            return resteurant;
        }
        #endregion

        public async Task Update(int resteurantId, int dishId, UpdateDishDto dto)
        {
            try
            {
                var resteurant = await GetResteurantById(resteurantId);

                var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
                if (dish is null || dish.ResteurantId != resteurantId)
                    throw new NotFoundException("Dish not found");

                if (dto.Name != null) dish.Name = dto.Name;
                if (dto.Description != null) dish.Description = dto.Description;
                if (dto.Price != 0) dish.Price = dto.Price;

                _context.Dishes.Update(dish);
                await _context.SaveChangesAsync();
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }

        public async Task Delete(int resteurantId, int dishId)
        {
            try
            {
                var resteurant = await GetResteurantById(resteurantId, true);

                var dish = resteurant.Dishes.FirstOrDefault(d => d.Id == dishId);
                if (dish is null)
                    throw new NotFoundException("Dish not found");

                _context.Dishes.Remove(dish);
                await _context.SaveChangesAsync();
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }

        public async Task DeleteAll(int resteurantId)
        {
            try
            {
                var resteurant = await GetResteurantById(resteurantId, true);
                var dishes = resteurant.Dishes;

                _context.RemoveRange(dishes);
                await _context.SaveChangesAsync();
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }

        public async Task<Dish> Create(int resteurantId, CreateDishDto dto)
        {
            var resteurant = await GetResteurantById(resteurantId, true);
            if (resteurant.Dishes.Any(d => d.Name == dto.Name))
                throw new NotUniqueItemException("Dish already exists");

            var dish = _mapper.Map<Dish>(dto);
            dish.ResteurantId = resteurant.Id;

            try
            {
                await _context.Dishes.AddAsync(dish);
                await _context.SaveChangesAsync();

                return dish;
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }

        public async Task<DishDto> GetById(int resteurantId, int dishId)
        {
            try
            {
                var resteurant = await GetResteurantById(resteurantId);

                var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
                if (dish is null || dish.ResteurantId != resteurantId)
                    throw new NotFoundException("Dish not found");

                return _mapper.Map<DishDto>(dish);
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }

        public async Task<IEnumerable<DishDto>> GetAll(int resteurantId)
        {
            try
            {
                var resteurant = await GetResteurantById(resteurantId, true);

                var dishDtos = _mapper.Map<List<DishDto>>(resteurant.Dishes);
                return dishDtos;
            }
            catch (DatabaseOperationException)
            {
                throw new DatabaseOperationException(databaseErrorMessage);
            }
        }
    }
}
