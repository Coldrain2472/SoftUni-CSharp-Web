namespace SeminarHub.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SeminarHub.Data;
    using SeminarHub.Data.Models;
    using SeminarHub.Models.Category;
    using SeminarHub.Models.Seminar;
    using System.Globalization;
    using System.Security.Claims;
    using static SeminarHub.Constants.CommonConstants;

    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext context;

        public SeminarController(SeminarHubDbContext _context)
        {
            context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var seminars = await context.Seminars
                .AsNoTracking()
                .Select(s => new SeminarAllViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    DateAndTime = s.DateAndTime.ToString(DefaultDateFormat),
                    Organizer = s.Organizer.UserName ?? string.Empty,
                    Category = s.Category.Name
                })
                .ToListAsync();

            return View(seminars);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var seminar = new SeminarAddViewModel();

            seminar.Categories = await GetCategories();

            return View(seminar);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarAddViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            DateTime dateAndTime;

            if (!DateTime.TryParseExact(model.DateAndTime, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateAndTime))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be {DefaultDateFormat}");
            }

            //if (!ModelState.IsValid)
            //{
            //    // we can check what kind of errors we have (if any)
            //    List<string> errorMessages = new List<string>();
            //    foreach (var entry in ModelState)
            //    {
            //        foreach (var error in entry.Value.Errors)
            //        {
            //            errorMessages.Add(error.ErrorMessage);
            //        }
            //    }

            //    model.Categories = await GetCategories();
            //    return View(model);
            //}

            Seminar newSeminar = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = dateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                OrganizerId = GetUserById()
            };

            await context.Seminars.AddAsync(newSeminar);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var currentUserId = GetUserById();

            var seminars = await context.SeminarsParticipants
                 .AsNoTracking()
                 .Where(sp => sp.ParticipantId == currentUserId)
                 .Select(sp => new SeminarJoinedViewModel()
                 {
                     Id = sp.Seminar.Id,
                     Topic = sp.Seminar.Topic,
                     Lecturer = sp.Seminar.Lecturer,
                     DateAndTime = sp.Seminar.DateAndTime.ToString(DefaultDateFormat),
                     Organizer = sp.Seminar.Organizer.UserName ?? string.Empty,
                     Category = sp.Seminar.Category.Name
                 })
                 .ToListAsync();

            return View(seminars);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var currentUserId = GetUserById();

            if (!await context.Seminars.AnyAsync(s=>s.Id == id))
            {
                return BadRequest();
            }

            if (await context.SeminarsParticipants.AnyAsync(sp=>sp.ParticipantId == currentUserId && sp.SeminarId == id))
            {
                return RedirectToAction(nameof(All));
            }

            SeminarParticipant seminarParticipant = new SeminarParticipant()
            {
                SeminarId = id,
                ParticipantId = currentUserId
            };

            await context.SeminarsParticipants.AddAsync(seminarParticipant);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
           var seminar = await context.Seminars
                .AsNoTracking()
                .Where(s=>s.Id == id)
                .Select(s=>new SeminarDetailsViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    DateAndTime = s.DateAndTime.ToString(DefaultDateFormat),
                    Duration = s.Duration,
                    Category = s.Category.Name,
                    Details = s.Details,
                    Organizer = s.Organizer.UserName ?? string.Empty
                })
                .FirstOrDefaultAsync();

            return View(seminar);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var seminar = await context.Seminars
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

            if (seminar == null)
            {
                return BadRequest();
            }

            if (seminar.OrganizerId != GetUserById())
            {
                return Unauthorized();
            }

            var seminarToEdit = await context.Seminars
                .Where(s=>s.Id == id)
                .Select(s=>new SeminarEditViewModel()
                {
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Details = s.Details,
                    DateAndTime = s.DateAndTime.ToString(DefaultDateFormat),
                    Duration = s.Duration,
                    CategoryId = s.CategoryId
                })
                .FirstOrDefaultAsync();

            if (seminarToEdit == null)
            {
                return BadRequest();
            }

            seminarToEdit.Categories = await GetCategories();

            return View(seminarToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SeminarEditViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            DateTime dateAndTime;

            if (!DateTime.TryParseExact(model.DateAndTime, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateAndTime))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be {DefaultDateFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();
                return View(model);
            }

            var seminarToEdit = await context.Seminars
                .Where(s => s.Id == model.Id)
                .FirstOrDefaultAsync();

            if (seminarToEdit == null) 
            { 
                return BadRequest(); 
            }

            seminarToEdit.Topic = model.Topic;
            seminarToEdit.Lecturer = model.Lecturer;
            seminarToEdit.Duration = model.Duration;
            seminarToEdit.CategoryId = model.CategoryId;
            seminarToEdit.Details = model.Details;
            seminarToEdit.DateAndTime = dateAndTime;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            var currentUserId = GetUserById();

            if (!await context.Seminars.AnyAsync(s=>s.Id == id))
            {
                return BadRequest();
            }

            var seminarParticipant = await context.SeminarsParticipants
                .Where(sp => sp.SeminarId == id && sp.ParticipantId == currentUserId)
                .FirstOrDefaultAsync();

            if (seminarParticipant == null)
            {
                return BadRequest();
            }

            context.SeminarsParticipants.Remove(seminarParticipant);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = GetUserById();

            var seminar = await context.Seminars
                .Where(s=>s.Id == id)
                .FirstOrDefaultAsync();

            if (seminar == null)
            {
                return BadRequest();
            }

            if (seminar.OrganizerId != currentUserId)
            {
                return Unauthorized();
            }

            var seminarToDelete = new SeminarDeleteViewModel()
            {
                Id = id,
                Topic = seminar.Topic,
                DateAndTime = seminar.DateAndTime
            };

            return View(seminarToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seminar = await context.Seminars
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

            var seminarParticipants = await context.SeminarsParticipants
                .Where(sp => sp.SeminarId == id)
                .ToListAsync();

            if (seminar == null)
            {
                return BadRequest();
            }

            if (seminar.OrganizerId != GetUserById())
            {
                return Unauthorized();
            }

            if (seminarParticipants != null && seminarParticipants.Any())
            {
                context.SeminarsParticipants.RemoveRange(seminarParticipants);
            }

            context.Seminars.Remove(seminar);
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

        private async Task<ICollection<CategoryViewModel>> GetCategories()
        {
            return await context.Categories
                .Select(t => new CategoryViewModel()
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }
    }
}
