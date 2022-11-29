using Microsoft.AspNetCore.Mvc;
using MuhkoHammer.Managers;
using MuhkoHammer.ModelClasses;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MuhkoHammer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameSystemsController : ControllerBase
    {
        private GameSystemsManager _manager = new GameSystemsManager();

        // GET: api/<GameSystemsController>
        [HttpGet]
        public IEnumerable<GameSystem> Get()
        {
            return _manager.GetAll();
        }

        // GET api/<GameSystemsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<GameSystemsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<GameSystemsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
    }
}
