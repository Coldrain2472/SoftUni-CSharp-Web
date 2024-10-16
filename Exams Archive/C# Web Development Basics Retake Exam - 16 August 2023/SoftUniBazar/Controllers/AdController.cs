namespace SoftUniBazar.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualBasic;
    using SoftUniBazar.Data;
    using SoftUniBazar.Models.Ad;
    using SoftUniBazar.Models.Category;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security.Claims;
    using static SoftUniBazar.Constants.CommonConstants;

    [Authorize]
    public class AdController : Controller
    {
        private readonly BazarDbContext context;

        public AdController(BazarDbContext _context)
        {
            context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await context.Ads
                .AsNoTracking()
                .Where(a => a.IsDeleted == false)
                .Select(a => new AdAllViewModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    ImageUrl = a.ImageUrl,
                    CreatedOn = a.CreatedOn.ToString(DefaultDateFormat),
                    Category = a.Category.Name,
                    Description = a.Description,
                    Price = a.Price,
                    Owner = a.Owner.UserName ?? string.Empty
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AdAddViewModel();

            model.Categories = await GetCategories();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AdAddViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();
                return View(model);
            }

            var adToAdd = new Ad()
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                Price = model.Price,
                CategoryId = model.CategoryId,
                OwnerId = GetUserById()
            };

            await context.Ads.AddAsync(adToAdd);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var currentUserId = GetUserById();

            var adBuyers = await context.AdsBuyers
                .Where(ab => ab.BuyerId == currentUserId)
                .Select(ab => new AdCartViewModel()
                {
                    Id = ab.Ad.Id,
                    Name = ab.Ad.Name,
                    Description = ab.Ad.Description,
                    ImageUrl = ab.Ad.ImageUrl,
                    Price = ab.Ad.Price,
                    Category = ab.Ad.Category.Name,
                    CreatedOn = ab.Ad.CreatedOn.ToString(DefaultDateFormat),
                    Owner = ab.Ad.Owner.UserName ?? string.Empty
                })
                .ToListAsync();

            return View(adBuyers);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var currentUserId = GetUserById();

            if (!await context.Ads.AsNoTracking().AnyAsync(a => a.Id == id))
            {
                return BadRequest();
            }

            if (await context.AdsBuyers.AsNoTracking().AnyAsync(ab => ab.AdId == id && ab.BuyerId == currentUserId))
            {
                return RedirectToAction(nameof(All));
            }

            var adBuyer = new AdBuyer()
            {
                AdId = id,
                BuyerId = currentUserId
            };

            await context.AdsBuyers.AddAsync(adBuyer);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Cart));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentAd = await context.Ads
                .AsNoTracking()
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (currentAd == null)
            {
                return BadRequest();
            }

            if (currentAd.OwnerId != GetUserById())
            {
                return Unauthorized();
            }

            var adToEdit = await context.Ads
               .AsNoTracking()
               .Where(a => a.Id == id)
               .Select(a => new AdEditViewModel()
               {
                   Id = a.Id,
                   Name = a.Name,
                   ImageUrl = a.ImageUrl,
                   Description = a.Description,
                   Price = a.Price,
               })
               .FirstOrDefaultAsync();

            if (adToEdit == null)
            {
                return BadRequest();
            }

            adToEdit.Categories = await GetCategories();

            return View(adToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdEditViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();
                return View(model);
            }

            var editEvent = await context.Ads
                .Where(e => e.Id == model.Id)
                .FirstOrDefaultAsync();

            if (editEvent == null)
            {
                return BadRequest();
            }

            editEvent.Name = model.Name;
            editEvent.Description = model.Description;
            editEvent.ImageUrl = model.ImageUrl;
            editEvent.Price = model.Price;
            editEvent.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var currentUserId = GetUserById();

            if (!await context.Ads.AsNoTracking().AnyAsync(a => a.Id == id))
            {
                return BadRequest();
            }

            var adBuyer = await context.AdsBuyers
                .Where(ab => ab.AdId == id && ab.BuyerId == currentUserId)
                .FirstOrDefaultAsync();

            if (adBuyer == null)
            {
                return BadRequest();
            }

            context.AdsBuyers.Remove(adBuyer);
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
                .Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}
