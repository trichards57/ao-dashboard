// -----------------------------------------------------------------------
// <copyright file="VehiclesController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Dashboard.Client.Model;
using Dashboard.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers
{
    [Route("api/vehicles")]
    [ApiController]
    public class VehiclesController(IVehicleService vehicleService) : ControllerBase
    {
        private IVehicleService vehicleService = vehicleService;

        [HttpGet]
        public IAsyncEnumerable<VehicleSettings> GetVehiclesAsync([FromQuery] Place place) => vehicleService.GetSettingsAsync(place);

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleSettings>> GetVehicleAsync(Guid id)
        {
            var vehicle = await vehicleService.GetSettingsAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return vehicle;
        }

        [HttpPost]
        public async Task<IActionResult> PostVehicleAsync(UpdateVehicleSettings settings)
        {
            await vehicleService.PutSettingsAsync(settings);

            return NoContent();
        }
    }
}
