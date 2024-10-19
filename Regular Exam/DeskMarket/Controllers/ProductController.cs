namespace DeskMarket.Controllers
{
    using DeskMarket.Data;
    using DeskMarket.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System;
    using System.Security.Claims;
    using DeskMarket.Data.Models;
    using static DeskMarket.Constants.CommonConstants;
    using Microsoft.AspNetCore.Authorization;

    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;

        public ProductController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await context.Products
                .AsNoTracking()
                .Where(p => p.IsDeleted == false)
                .Select(p => new ProductIndexViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    HasBought = false,
                    IsSeller = p.SellerId == GetUserById()
                })
                .ToListAsync();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var productForm = new ProductAddViewModel();

            productForm.Categories = await GetCategories();

            return View(productForm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(ProductAddViewModel productForm)
        {
            // null validation
            if (productForm == null)
            {
                return BadRequest();
            }
            DateTime addedOn;

            if (!DateTime.TryParseExact(productForm.AddedOn, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out addedOn))
            {
                ModelState.AddModelError(nameof(productForm.AddedOn), $"Invalid date! Format must be {DefaultDateFormat}");
            }

            if (!ModelState.IsValid)
            {
                // I used it to show me the error messages of the ModelState
                // for debugging
                List<string> errorMessages = new List<string>();
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }

                productForm.Categories = await GetCategories();
                return View(productForm);
            }

            // this will return the user to the same page if some of the edited fields are invalid
            if (!ModelState.IsValid)
            {
                productForm.Categories = await GetCategories();

                return View(productForm);
            }

            Product product = new Product()
            {
                ProductName = productForm.ProductName,
                Price = productForm.Price,
                ImageUrl = productForm.ImageUrl,
                Description = productForm.Description,
                AddedOn = addedOn,
                CategoryId = productForm.CategoryId,
                SellerId = GetUserById()
            };

            // adding the new product to the db and saving
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = GetUserById();

            var product = await context.Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailsViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    AddedOn = p.AddedOn.ToString(DefaultDateFormat),
                    Seller = p.Seller.UserName ?? string.Empty,
                    CategoryName = p.Category.Name,
                    HasBought = User.Identity.IsAuthenticated &&
                        context.ProductsClients.Any(pc => pc.ProductId == id && pc.ClientId == currentUserId)
                })
                .FirstOrDefaultAsync();

            // validation if the product exists
            if (product == null)
            {
                return BadRequest();
            }

            return View(product);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var currentUserId = GetUserById();

            var model = await context.Products
                .Where(p => p.IsDeleted == false)
                .Where(p => p.ProductsClients.Any(pc => pc.ClientId == currentUserId))
                .Select(p => new ProductCartViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    HasBought = context.ProductsClients.Any(p => p.ClientId == currentUserId && p.ProductId == p.ProductId) 
                    // check if the user has bought this product
                })
                .ToListAsync();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var currentUserId = GetUserById();

            var productExists = await context.Products
                 .AsNoTracking()
                 .AnyAsync(p => p.Id == id && !p.IsDeleted);

            if (!productExists)
            {
                return BadRequest();
            }

            var inCart = await context.ProductsClients
                 .AnyAsync(pc => pc.ProductId == id && pc.ClientId == currentUserId);

            if (inCart)
            {
                return RedirectToAction(nameof(Index));
            }

            ProductClient productClient = new ProductClient()
            {
                ProductId = id,
                ClientId = currentUserId
            };

            await context.ProductsClients.AddAsync(productClient);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Cart));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await context.Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            // null validation for product
            if (product == null)
            {
                return BadRequest();
            }

            // this won't allow for a user that is not a creator of this product to alter it
            if (product.SellerId != GetUserById())
            {
                return Unauthorized();
            }

            var productToEdit = await context.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductEditViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                    AddedOn = p.AddedOn.ToString(DefaultDateFormat),
                    CategoryId = p.CategoryId,
                    SellerId = GetUserById()
                })
                .FirstOrDefaultAsync();

            // null validation
            if (productToEdit == null)
            {
                return BadRequest();
            }

            productToEdit.Categories = await GetCategories();

            return View(productToEdit);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            // null validation
            if (model == null)
            {
                return BadRequest();
            }
            DateTime addedOn;

            if (!DateTime.TryParseExact(model.AddedOn, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out addedOn))
            {
                ModelState.AddModelError(nameof(model.AddedOn), $"Invalid date! Format must be {DefaultDateFormat}!");
            }

            if (!ModelState.IsValid)
            {
                // for debugging purposes: it lets me see the error messages
                List<string> errorMessages = new List<string>();
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }

                model.Categories = await GetCategories();

                return View(model);
            }

            var productToEdit = await context.Products
                .Where(p => p.Id == model.Id)
                .FirstOrDefaultAsync();

            // null validation
            if (productToEdit == null)
            {
                return BadRequest();
            }

            productToEdit.ProductName = model.ProductName;
            productToEdit.Description = model.Description;
            productToEdit.Price = model.Price;
            productToEdit.AddedOn = addedOn;
            productToEdit.ImageUrl = model.ImageUrl;
            productToEdit.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();
            return RedirectToAction("Details", "Product", new { id = productToEdit.Id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            Product? product = await context.Products
                    .Where(p => p.Id == id)
                    .Include(p => p.ProductsClients)
                    .FirstOrDefaultAsync();

            if (product == null || product.IsDeleted)
            {
                return BadRequest();
            }

            var currentUserId = GetUserById();

            ProductClient? productClient = product.ProductsClients.FirstOrDefault(pc => pc.ClientId == currentUserId);

            if (productClient == null)
            {
                return BadRequest();
            }
            else
            {
                product.ProductsClients.Remove(productClient);

                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Cart));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await context.Products
                .Where(p => p.Id == id)
                .Where(p=>p.IsDeleted == false)
                .AsNoTracking()
                .Select(p => new ProductDeleteViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    SellerId = p.SellerId,
                    Seller = p.Seller.UserName ?? string.Empty
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(ProductDeleteViewModel model)
        {
            Product? product = await context.Products
                .Where(p => p.Id == model.Id)
                .Where(p => p.IsDeleted == false)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return BadRequest();
            }
            else
            {
                product.IsDeleted = true;

                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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