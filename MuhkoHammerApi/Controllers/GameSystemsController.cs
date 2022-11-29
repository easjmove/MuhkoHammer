using Microsoft.AspNetCore.Mvc;
using MuhkoHammerApi.Managers;
using MuhkoHammerApi.ModelClasses;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MuhkoHammer.Controllers
{
    [Route("api")]
    [ApiController]
    public class GameSystemsController : ControllerBase
    {
        private GameSystemsManager _manager = new GameSystemsManager();

        [HttpGet("Reset")]
        public void Reset()
        {
            GameSystemsManager.ReloadData();
            //_manager.CreateThumbs();
        }

        // GET: api/GameSystems
        [HttpGet("GameSystems")]
        public IEnumerable<GameSystem> GetGameSystems([FromQuery] bool includeAll = false)
        {
            return _manager.GetAll(includeAll);
        }

        // GET api/GameSystems/5
        [HttpGet("GameSystems/{id}")]
        public GameSystem Get(int id, [FromQuery] bool includeAll = false)
        {
            return _manager.GetByID(id, includeAll);
        }

        // GET: api/GameSystems/1/Factions
        [HttpGet("GameSystems/{gsid}/Factions")]
        public IEnumerable<Faction> GetFactions(int gsid, [FromQuery] bool includeAll = false)
        {
            return _manager.GetFactions(gsid, includeAll);
        }

        // GET: api/GameSystems/1/Factions/1/Units
        [HttpGet("GameSystems/{gsid}/Factions/{fid}/Units")]
        public IEnumerable<Unit> GetUnits(int gsid, int fid, [FromQuery] bool includeAll = false)
        {
            return _manager.GetUnits(gsid, fid, includeAll);
        }

        // GET: api/GameSystems/1/Factions/1/Units/1/Images
        [HttpGet("GameSystems/{gsid}/Factions/{fid}/Units/{uid}/Images")]
        public IEnumerable<UnitImage> GetImages(int gsid, int fid, int uid, [FromQuery] bool includeAll = false)
        {
            return _manager.GetUnitImages(gsid, fid, uid, includeAll);
        }

        [HttpGet("Images/{id}")]
        public ActionResult GetImage(int id)
        {
            byte[] image = _manager.GetImageByID(id, false);
            return File(image, "image/jpg");
        }

        [HttpGet("Thumbnail/{id}")]
        public ActionResult GetThumbnail(int id)
        {
            byte[] image = _manager.GetImageByID(id, true);
            return File(image, "image/jpg");
        }

        // POST api/Images
        [HttpPost("Images")]
        public void Post([FromForm] IFormFile data, [FromQuery] int? unitId)
        {

            _manager.AddImage(data, unitId);
        }

        // PUT api/Images/id
        [HttpPut("Images/{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
    }
}
