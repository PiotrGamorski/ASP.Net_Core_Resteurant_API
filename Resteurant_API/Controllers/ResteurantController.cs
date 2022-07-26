﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resteurant_API.Authentication;
using Resteurant_API.Dtos;
using Resteurant_API.Exceptions;
using Resteurant_API.Interfaces;
using Resteurant_API.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Resteurant_API.Controllers
{
    [Route("api/resteurant")]
    [ApiController]
    [Authorize]
    public class ResteurantController : ControllerBase
    {
        private readonly IResteurantService _service;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly AuthenticationSettings _authenticationSettings;

        public ResteurantController(IResteurantService service, AuthenticationStateProvider authenticationStateProvider, 
            AuthenticationSettings authenticationSettings)
        {
            _service = service;
            _authenticationStateProvider = authenticationStateProvider;
            _authenticationSettings = authenticationSettings;
        }

        #region HttpPut Methods
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateResteurantDto dto)
        {
            await _service.Update(id, dto);
            return Ok();
        }
        #endregion

        #region HttpDelete Methods
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _service.Delete(id);
            return NoContent();
        }
        #endregion

        #region HttpPost Methods
        [HttpPost]
        [Authorize("Admin,Manager")]
        public async Task<ActionResult> CreateResteurant([FromBody] CreateResteurantDto dto)
        {
            var resteurant = await _service.Create(dto);
            if (resteurant is null)
                throw new NotUniqueItemException("Resteurant already exists");

            return Created($"/api/resteurant/{resteurant.Id}", null);
        }
        #endregion

        #region HttpGet Methods
        [HttpGet]
        [Authorize(Policy = "HasNationality")]
        [Authorize(Policy ="AtLeast18")]
        [Authorize(Policy = "CreatedAtLeastOneResteurant")]
        public async Task<ActionResult<IEnumerable<ResteurantDto>>> GetAll([FromQuery]ResteurantQuery query)
        {
            var resteurants = await _service.GetAll(query);
            return Ok(resteurants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResteurantDto>> Get([FromRoute] int id)
        {
            
            var resteurantDto = await _service.GetById(id);
            if (resteurantDto is null) return NotFound();

            if (_authenticationSettings.AuthenticationScheme == "Bearer")
            {
                var authState = await ((CustomAuthStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync();
                var userId = authState?.User?.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            }
            
            return Ok(resteurantDto);
        }
        #endregion
    }
}
