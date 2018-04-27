using Microsoft.AspNetCore.Mvc;
using ConferenceApi.Models;
using System.Linq;

namespace ConferenceApi.Controllers
{
    /// <summary>
    /// App main controller.
    /// </summary>
    [Route("conference")]
    public class ConferenceController : Controller
    {
        private readonly ConferenceContext _context;

        /// <summary>
        /// Controller default constructor. Db initialize.
        /// </summary>
        /// <param name="context"></param>
        public ConferenceController(ConferenceContext context)
        {
            _context = context;

            if (_context.ConferenceItems.Any()) return;

            #region Start Db Initialize

            _context.ConferenceItems.Add(new ConferenceItem
            {
                Section = "GIS",
                Info = new InfoItem
                {
                    Name = "Geoinformation Systems",
                    City = "Tomsk",
                    Location = "Lenina 2, 404"
                }
            });
            _context.ConferenceItems.Add(new ConferenceItem
            {
                Section = "CS",
                Info = new InfoItem
                {
                    Name = "Computer Science",
                    City = "Tomsk",
                    Location = "Lenina 30, 206"
                }
            });
            _context.ConferenceItems.Add(new ConferenceItem
            {
                Section = "CF",
                Info = new InfoItem
                {
                    Name = "CodeFest",
                    City = "Novosibirsk",
                    Location = "Expocenter"
                }
            });
            _context.SaveChanges();

            #endregion
        }

        [HttpGet("info")]
        public IActionResult GetAll()
        {
            return Ok(_context.ConferenceItems.ToList());
        }

        [HttpGet("{section}/info", Name = "GetConference")]
        public IActionResult GetById(string section)
        {
            var item = _context.ConferenceItems.FirstOrDefault(t => t.Section == section);
            if (item == null)
                return NotFound();

            return Ok(item.Info);
        }

        [HttpPost("{section}/info")]
        public IActionResult Create(string section, [FromBody] InfoItem infoItem)
        {
            if (section == string.Empty)
                ModelState.AddModelError("Section", "The Section field is required.");
            if (section.Length > 5)
                ModelState.AddModelError("Section",
                    "The field Section must be a string with a maximum length of 5.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ConferenceItem item = _context.ConferenceItems.FirstOrDefault(t => t.Section == section);
            if (item != null)
            {
                item.Section = section;
                item.Info = infoItem;
                _context.ConferenceItems.Update(item);
            }
            else
            {
                item = new ConferenceItem
                {
                    Section = section,
                    Info = infoItem
                };
                _context.ConferenceItems.Add(item);
            }

            _context.SaveChanges();

            return CreatedAtRoute("GetConference", new {item.Section}, item);
        }
    }
}