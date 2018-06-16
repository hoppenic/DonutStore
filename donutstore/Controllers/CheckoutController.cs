using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Braintree;
using donutstore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;



namespace donutstore.Controllers
{
    public class CheckoutController : Controller
    {
        private DonutStoreDbContext _donutStoreDbContext;
        private EmailService _emailService;
        private SignInManager<DonutStoreUser> _signInManager;
        private BraintreeGateway _brainTreeGateway;

        public CheckoutController(DonutStoreDbContext donutStoreDbContext, EmailService emailService, SignInManager<DonutStoreUser> signInManager,BraintreeGateway braintreeGateway)
        {
            _donutStoreDbContext = donutStoreDbContext;
            _emailService = emailService;
            _signInManager = signInManager;
            _brainTreeGateway = braintreeGateway;

        }

        public async Task<IActionResult> Index()
        {
            CheckoutViewModel model = new CheckoutViewModel();
            await GetCurrentCart(model);
            if (model.Cart == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        private async Task GetCurrentCart(CheckoutViewModel model)
        {
            Guid cartId;
            Cart cart = null;

            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _signInManager.UserManager.GetUserAsync(User);
                model.ContactEmail = currentUser.Email;
                model.ContactPhoneNumber = currentUser.PhoneNumber;
            }

            if (Request.Cookies.ContainsKey("cartId"))
            {
                if (Guid.TryParse(Request.Cookies["cartId"], out cartId))
                {
                    cart = await _donutStoreDbContext.Carts
                        .Include(carts => carts.CartItems)
                        .ThenInclude(cartitems => cartitems.Product)
                        .FirstOrDefaultAsync(x => x.CookieIdentifier == cartId);
                }
            }
            model.Cart = cart;

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            await GetCurrentCart(model);

            if (ModelState.IsValid)
            {
                Order newOrder = new Order
                {
                    TrackingNumber = Guid.NewGuid().ToString(),
                    OrderDate = DateTime.Now,
                    OrderItems = model.Cart.CartItems.Select(x => new OrderItem
                    {
                        ProductID = x.Product.ID,
                        ProductName = x.Product.Name,
                        ProductPrice = (x.Product.Price ?? 0),
                        Quantity = x.Quantity
                    }).ToArray(),
                    AddressLine1 = model.ShippingAddressLine1,
                    AddressLine2 = model.ShippingAddressLine2,
                    Country = model.ShippingCountry,
                    Email = model.ContactEmail,
                    PhoneNumber = model.ContactPhoneNumber,
                    Locale = model.ShippingLocale,
                    PostalCode = model.ShippingPostalCode,
                    Region = model.ShippingRegion

                };

                TransactionRequest transaction = new TransactionRequest
                {
                    //Amount = 1,
                    Amount = model.Cart.CartItems.Sum(x => x.Quantity * (x.Product.Price ?? 0)),
                    CreditCard = new TransactionCreditCardRequest
                    {
                        Number = model.BillingCardNumber,
                        CardholderName = model.BillingNameOnCard,
                        CVV = model.BillingCardVerificationValue,
                        ExpirationMonth = model.BillingCardExpirationMonth.ToString().PadLeft(2, '0'),
                        ExpirationYear = model.BillingCardExpirationYear.ToString()
                    }

                };
                var transactionResult = await _brainTreeGateway.Transaction.SaleAsync(transaction);
                if (transactionResult.IsSuccess())
                {



                    _donutStoreDbContext.Orders.Add(newOrder);
                    _donutStoreDbContext.CartItems.RemoveRange(model.Cart.CartItems);
                    _donutStoreDbContext.Carts.Remove(model.Cart);
                    await _donutStoreDbContext.SaveChangesAsync();
                    //Try to checkout
                    Response.Cookies.Delete("cartId");
                    return RedirectToAction("Index", "Receipt", new { id = newOrder.TrackingNumber });
                }
                for (int i = 0; i < transactionResult.Errors.Count; i++)
                {
                    ModelState.AddModelError("BillingCardNumber" + i, transactionResult.Errors.All()[i].Message);
                }

            }


            return View(model);


        }
    }