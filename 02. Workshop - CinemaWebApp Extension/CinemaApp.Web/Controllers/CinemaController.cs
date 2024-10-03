namespace CinemaApp.Web.Controllers
{
    using CinemaApp.Data;
    using CinemaApp.Data.Models;
    using CinemaApp.Web.ViewModels.Cinema;
    using CinemaApp.Web.ViewModels.Movie;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class CinemaController : BaseController
    {
        private readonly CinemaDbContext dbContext;

        public CinemaController(CinemaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<CinemaIndexViewModel> cinemas = this.dbContext
                .Cinemas
                .Select(c => new CinemaIndexViewModel()
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Location = c.Location
                })
                .OrderBy(c => c.Location)
                .ToArray();

            return this.View(cinemas);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddCinemaFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            Cinema cinema = new Cinema()
            {
                Name = model.Name,
                Location = model.Location
            };

            this.dbContext.Cinemas.Add(cinema);
            this.dbContext.SaveChanges();

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id)
        {
            Guid cinemaGuid = Guid.Empty;
            bool isIdValid = this.IsGuidValid(id, ref cinemaGuid);

            if (!isIdValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            Cinema? cinema = this.dbContext
              .Cinemas
              .Include(c => c.CinemaMovies)
              .ThenInclude(cm => cm.Movie)
              .FirstOrDefault(c => c.Id == cinemaGuid);

            if (cinema == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            CinemaDetailsViewModel viewModel = new CinemaDetailsViewModel()
            {
                Name = cinema.Name,
                Location = cinema.Location,
                Movies = cinema.CinemaMovies
                .Where(cm=>cm.IsDeleted == false)
                .Select(cm => new CinemaMovieViewModel()
                {
                    Title = cm.Movie.Title,
                    Duration = cm.Movie.Duration
                })
                .ToArray()
            };

            return this.View(viewModel);
        }
    }
}
