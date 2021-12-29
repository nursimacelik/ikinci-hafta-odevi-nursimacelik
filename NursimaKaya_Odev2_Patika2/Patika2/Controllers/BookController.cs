using Data;
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
    public class BookController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogger<WeatherForecastController> _logger;

        public BookController(ILogger<WeatherForecastController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var listofbooks = await unitOfWork.Book.GetAll();
            return Ok(listofbooks);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await unitOfWork.Book.GetById(id);

            if (book is null)
            {
                return NotFound();
            }

            return Ok(book);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book entity)
        {
            var response = await unitOfWork.Book.Add(entity);
            unitOfWork.Complete();

            return Ok();
        }
    }

}
