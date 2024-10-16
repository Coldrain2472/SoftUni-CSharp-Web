namespace Homies.Controllers
{
    using Homies.Data;
    using Homies.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualBasic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security.Claims;
    using static Homies.Constants.CommonConstants;

    [Authorize]
    public class EventController : Controller
    {
        private readonly HomiesDbContext context;

        public EventController(HomiesDbContext _context)
        {
            context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await context.Events
                .AsNoTracking()
                .Where(e => e.IsDeleted == false)
                .Select(e => new EventAllViewModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Start = e.Start.ToString(DefaultDateFormat),
                    Organiser = e.Organiser.UserName ?? string.Empty,
                    Type = e.Type.Name
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new EventAddViewModel();

            model.Types = await GetTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventAddViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParseExact(model.Start, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                ModelState.AddModelError(nameof(model.Start), $"Invalid date! Format must be {DefaultDateFormat}");
            }
            if (!DateTime.TryParseExact(model.End, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                ModelState.AddModelError(nameof(model.End), $"Invalid date! Format must be {DefaultDateFormat}");
            }

            if (!ModelState.IsValid)
            {
                // we can check what kind of errors we have (if any)
                List<string> errorMessages = new List<string>();
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }

                model.Types = await GetTypes();
                return View(model);
            }

            Event newEvent = new Event()
            {
                Name = model.Name,
                Description = model.Description,
                Start = startDate,
                End = endDate,
                TypeId = model.TypeId,
                CreatedOn = DateTime.Now,
                OrganiserId = GetUserById()
            };

            await context.AddAsync(newEvent);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentEvent = await context.Events
                .AsNoTracking()
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (currentEvent == null)
            {
                return BadRequest();
            }

            if (currentEvent.OrganiserId != GetUserById())
            {
                return Unauthorized();
            }

            var eventToEdit = await context.Events
                .Where(e => e.Id == id)
                .Select(e => new EventEditViewModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start.ToString(DefaultDateFormat),
                    End = e.End.ToString(DefaultDateFormat),
                    TypeId = e.TypeId
                })
                .FirstOrDefaultAsync();

            if (eventToEdit == null)
            {
                return BadRequest();
            }

            eventToEdit.Types = await GetTypes();

            return View(eventToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventEditViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParseExact(model.Start, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                ModelState.AddModelError(nameof(model.Start), $"Invalid date! Format must be {DefaultDateFormat}");
            }

            if (!DateTime.TryParseExact(model.End, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                ModelState.AddModelError(nameof(model.End), $"Invalid date! Format must be {DefaultDateFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Types = await GetTypes();
                return View(model);
            }

            var eventToEdit = await context.Events
                .Where(e => e.Id == model.Id)
                .FirstOrDefaultAsync();

            if (eventToEdit == null)
            {
                return BadRequest();
            }

            eventToEdit.Name = model.Name;
            eventToEdit.Description = model.Description;
            eventToEdit.Start = startDate;
            eventToEdit.End = endDate;
            eventToEdit.TypeId = model.TypeId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var currentUserId = GetUserById();

            if (!await context.Events.AsNoTracking().AnyAsync(e => e.Id == id))
            {
                return BadRequest();
            }

            if (await context.EventsParticipants.AsNoTracking().AnyAsync(e => e.EventId == id && e.HelperId == currentUserId))
            {
                return RedirectToAction(nameof(All));
            }

            var eventParticipant = new EventParticipant()
            {
                EventId = id,
                HelperId = currentUserId
            };

            await context.EventsParticipants.AddAsync(eventParticipant);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var currentUserId = GetUserById();

            var currentUserEvents = await context.EventsParticipants
                .Where(ep => ep.HelperId == currentUserId)
                .Select(ep => new EventJoinedViewModel()
                {
                    Id = ep.EventId,
                    Name = ep.Event.Name,
                    Start = ep.Event.Start.ToString(DefaultDateFormat),
                    Organiser = ep.Event.Organiser.UserName ?? string.Empty,
                    Type = ep.Event.Type.Name
                })
                .ToListAsync();

            return View(currentUserEvents);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var currentEvent = await context.Events
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EventDetailsViewModel()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start.ToString(DefaultDateFormat),
                    End = e.End.ToString(DefaultDateFormat),
                    Organiser = e.Organiser.UserName ?? string.Empty,
                    CreatedOn = e.CreatedOn.ToString(DefaultDateFormat),
                    Type = e.Type.Name
                })
                .FirstOrDefaultAsync();

            return View(currentEvent);
        }
        
        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            var currentUserId = GetUserById();

            if (!await context.Events
               .AsNoTracking()
               .AnyAsync(e => e.Id == id))
            {
                return BadRequest();
            }

            var userEvent = await context.EventsParticipants
               .Where(ep => ep.EventId == id && ep.HelperId == currentUserId)
               .FirstOrDefaultAsync();

            if (userEvent == null)
            {
                return BadRequest();
            }

            context.EventsParticipants.Remove(userEvent);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        private string GetUserById()
        {
            string id = string.Empty;

            if (User != null)
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            }

            return id;
        }

        private async Task<ICollection<TypeViewModel>> GetTypes()
        {
            return await context.Types
                .Select(t => new TypeViewModel()
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }
    }
}