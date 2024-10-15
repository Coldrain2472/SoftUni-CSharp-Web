namespace SeminarHub.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SeminarHub.Data;
    using SeminarHub.Models;
    using System;
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
            var model = await context.Seminars
                .Where(s => s.IsDeleted == false)
                .Select(s => new SeminarInfoViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Category = s.Category.Name,
                    DateAndTime = s.DateAndTime.ToString(DateTimeFormat),
                    Organizer = s.Organizer.UserName!
                })
                .AsNoTracking()
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarViewModel();

            model.Categories = await GetCategories();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            DateTime dateAndTime;

            if (!DateTime.TryParseExact(model.DateAndTime, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateAndTime))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be {DateTimeFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();
                return View(model);
            }

            Seminar seminar = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = dateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                OrganizerId = GetUserById()
            };

            await context.Seminars.AddAsync(seminar);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var currentUserId = GetUserById();

            var currentUserSeminars = await context.SeminarsParticipants
                .Where(sp => sp.ParticipantId == currentUserId)
                .Select(sp => new SeminarJoinedViewModel()
                {
                    Id = sp.Seminar.Id,
                    Topic = sp.Seminar.Topic,
                    Lecturer = sp.Seminar.Lecturer,
                    Category = sp.Seminar.Category.Name,
                    DateAndTime = sp.Seminar.DateAndTime.ToString(DateTimeFormat),
                    Organizer = sp.Seminar.Organizer.UserName ?? string.Empty
                })
                .ToListAsync();

            return View(currentUserSeminars);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var currentUserId = GetUserById();

            if (!await context.Seminars
              .AsNoTracking()
              .AnyAsync(e => e.Id == id))
            {
                return BadRequest();
            }

            if (await context.SeminarsParticipants
                .AsNoTracking()
                .AnyAsync(ep => ep.SeminarId == id && ep.ParticipantId == currentUserId))
            {
                return RedirectToAction(nameof(All));
            }

            var seminarParticipant = new SeminarParticipant()
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
                .Where(s => s.Id == id)
                .Select(s => new SeminarDetailsViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    DateAndTime = s.DateAndTime.ToString(DateTimeFormat),
                    Duration = s.Duration,
                    Lecturer = s.Lecturer,
                    Category = s.Category.Name,
                    Details = s.Details,
                    Organizer = s.Organizer.UserName ?? string.Empty
                })
                .FirstOrDefaultAsync();

            if (seminar == null)
            {
                return BadRequest();
            }

            return View(seminar);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var seminar = await context.Seminars
                .AsNoTracking()
                .Where(s=>s.Id == id)
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
                .AsNoTracking()
                .Where(s=>s.Id == id)
                .Select(s=>new SeminarEditViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Details = s.Details,
                    DateAndTime = s.DateAndTime.ToString(DateTimeFormat),
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

            if (!DateTime.TryParseExact(model.DateAndTime, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateAndTime))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be {DateTimeFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();
                return View(model);
            }

            var editEvent = await context.Seminars
                .Where(e => e.Id == model.Id)
                .FirstOrDefaultAsync();

            if (editEvent == null)
            {
                return BadRequest();
            }

            editEvent.Topic = model.Topic;
            editEvent.Lecturer = model.Lecturer;
            editEvent.Details = model.Details;
            editEvent.DateAndTime = dateAndTime;
            editEvent.Duration = model.Duration;
            editEvent.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = GetUserById();

            var searchedSeminar = await context.Seminars
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

            if (searchedSeminar == null)
            {
                return BadRequest();
            }

            if (searchedSeminar.OrganizerId != GetUserById())
            {
                return Unauthorized();
            }

            var seminar = new SeminarDeleteViewModel()
            {
                Id = id,
                Topic = searchedSeminar.Topic,
                DateAndTime = searchedSeminar.DateAndTime
            };

            return View(seminar);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentSeminar = await context.Seminars
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

            var seminarParticipants = await context.SeminarsParticipants
                .Where(sp => sp.SeminarId == id)
                .ToListAsync();

            if (currentSeminar == null)
            {
                return BadRequest();
            }

            if (currentSeminar.OrganizerId != GetUserById())
            {
                return Unauthorized();
            }

            if (seminarParticipants != null && seminarParticipants.Any())
            {
                context.SeminarsParticipants.RemoveRange(seminarParticipants);
            }

            context.Seminars.Remove(currentSeminar);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            var currentUserId = GetUserById();

            if (!await context.Seminars.AsNoTracking().AnyAsync(s=>s.Id == id))
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

        private string GetUserById()
        {
            string id = string.Empty;

            if (User != null)
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
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
