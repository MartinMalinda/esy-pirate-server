using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace WebApplication22.Controllers
{
    [Route("api/pirates")]
    [ApiController]
    public class PirateController : Controller
    {
        private static List<Pirate> pirates;

        public static List<Pirate> Pirates
        {
            get
            {
                if (pirates == null)
                {
                    pirates = new List<Pirate>()
                {
                    new Pirate(1,"Esy"),
                    new Pirate(2,"johny")
                };
                }
                return pirates;
            }
            set
            {
                pirates = value;
            }
        }

        [HttpGet]
        [Route("")]
        public List<Pirate> FindAll()
        {
            return Pirates;
        }

        public class JsonObject
        {
            public string What { get; set; }
            public int[] Numbers { get; set; }
            public JsonObject(string what, int[] numbers)
            {
                What = what;
                Numbers = numbers;
            }
        }

        [HttpPost("arrays")]
        public IActionResult Arrays([FromBody]JsonObject obj)
        {
            if (obj.Numbers == null && obj.What != null)
            {
                StatusCode(400, new { message = "Please provide what to do with the numbers!" });
            }

            Object result = null;
            switch (obj.What)
            {
                case "sum":
                    result = obj.Numbers.Select(n => n).Sum();
                    break;
                case "multiply":
                    result = obj.Numbers.Aggregate((a, b) => a * b);
                    break;
                case "double":
                    result = obj.Numbers.Select(n => (n * 2)).ToArray();
                    break;
            }

            return Json(new { result = result });
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult FindOne([FromRoute] int id)
        {

            if (id == 0)
            {
                // Pirate with id 0 is forbidden, he's mysterious
                return StatusCode(403, new { message = "Cannot give you this pirate, srry" });
            }

            //return Pirates.First(p => p.Id == id); // Crash if id is not present
            var maybePirate = Pirates.Find(p => p.Id == id); // null => 204 No Content
            if (maybePirate == null)
            {
                // return 404
                return NotFound(new { message = "Pirate not found!" });
            }

            return Ok(maybePirate);
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
           var maybePirate = Pirates.Find(p => p.Id == id);

            if (maybePirate == null)
            {
                // return 404
                return NotFound(new { message = "Pirate not found!" });
            }

            Pirates.Remove(maybePirate);
            return StatusCode(204);
        }

        [HttpPost]
        [Route("")]
        public ActionResult Create(Pirate pirate)
        {
            pirate.Id = Pirates.Count + 1; // should be Max()!
            Pirates.Add(pirate);
            return StatusCode(201, pirate);
        }


        [HttpPut]
        [Route("{id}")]
        public ActionResult Update([FromRoute] int id, string name)
        {
            var maybePirate = Pirates.Find(p => p.Id == id);

            if (maybePirate == null)
            {
                // return 404
                return NotFound(new { message = "Pirate not found!" });
            }

            maybePirate.Name = name;

            return Ok(maybePirate);
        }

    }
}