﻿using Data;
using Data.Uow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patika2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogger<VehicleController> _logger;

        public VehicleController(ILogger<VehicleController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vehicleList = await unitOfWork.Vehicle.GetAll();
            return Ok(vehicleList);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var vehicle = await unitOfWork.Vehicle.GetById(id);

            if (vehicle is null)
            {
                return NotFound();
            }

            return Ok(vehicle);
        }

        [HttpGet("{id}/Containers")]
        public async Task<IActionResult> GetContainers(long id)
        {
            var vehicle = await unitOfWork.Vehicle.GetById(id);

            if (vehicle is null)
            {
                return NotFound();
            }

            var containerList = await unitOfWork.Vehicle.GetContainers(id);
            return Ok(containerList);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Vehicle entity)
        {
            var response = await unitOfWork.Vehicle.Add(entity);
            unitOfWork.Complete();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Vehicle entity)
        {
            var response = unitOfWork.Vehicle.Update(entity);
            unitOfWork.Complete();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            var vehicle = await unitOfWork.Vehicle.GetById(id);

            if (vehicle is null)
            {
                return NotFound();
            }
            await unitOfWork.Vehicle.Delete(id);
            unitOfWork.Complete();

            return Ok();
        }
    }

}
