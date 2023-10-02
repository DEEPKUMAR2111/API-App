using Data.DataAccess.Data;
using Data.DataAccess.Data.Interface.IData;
using Data.Models;
using Data.Models.ViewModels;
using Data.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace API_App.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _db;
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context, IUnitOfWork db)
        {
            _db = db;
            _context = context;
        }
        [HttpGet]
        public IActionResult CartItem()
        {
            var cliamsIdentity = (ClaimsIdentity)User.Identity;
            var user = _db.ApplicationUserRepository.GetFirstOrDefault(x => x.UserName == cliamsIdentity.Name);
            CartVM cartVM = new()
            {
                ListCart = _db.CartRepository.GetAll(x => x.ApplicationUserId == user.Id, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            foreach (var cart in cartVM.ListCart)
            {
                cartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
            }
            return Ok(cartVM);
        }
        [HttpGet]
        public IActionResult Count()
        {
            var cliamsIdentity = (ClaimsIdentity)User.Identity;
            var user = _db.ApplicationUserRepository.GetFirstOrDefault(x => x.UserName == cliamsIdentity.Name);
            var cartVM = new
            {
                number = _db.CartRepository.GetAll(x => x.ApplicationUserId == user.Id).Count()
            };
            return Ok(cartVM);
        }



            [HttpGet("{CartId}")]
        public IActionResult Plus(int CartId)
        {
            var Cart = _db.CartRepository.GetFirstOrDefault(u => u.Id == CartId);
            _db.CartRepository.IncrementCartItem(Cart, 1);
            _db.Save();
            var text = new
            {
                meassge = "Cart Updated"
            };
            return Ok(text);
        }

        [HttpGet("{CartId}")]
        public IActionResult Minus(int CartId)
        {
            var Cart = _db.CartRepository.GetFirstOrDefault(u => u.Id == CartId);

            if (Cart.Count <= 1)
            {
                _db.CartRepository.Remove(Cart);
             /*   var Count = _db.CartRepository.GetAll(u => u.ApplicationUserId == Cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32("SessionCart", Count);*/
            }
            else
            {
                _db.CartRepository.DecrementCartItem(Cart, 1);
            }

            _db.Save();
            var text = new
            {
                meassge = "Cart Updated"
            };
            return Ok(text);

        }

        [HttpDelete("{CartId}")]
        public IActionResult Remove(int CartId)
        {
            var cart = _db.CartRepository.GetFirstOrDefault(u => u.Id == CartId);
            _db.CartRepository.Remove(cart);
            _db.Save();
            var text = new
            {
                meassge = "Cart Item Removed."
            };
            /*HttpContext.Session.SetInt32("SessionCart", _db.CartRepository.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count);*/
            return Ok(text);
        }

        [HttpGet]
        public IActionResult Summary()
        {
            var cliamsIdentity = (ClaimsIdentity)User.Identity;
            var user = _db.ApplicationUserRepository.GetFirstOrDefault(x => x.UserName == cliamsIdentity.Name);
            CartVM cartVM = new()
            {
                ListCart = _db.CartRepository.GetAll(x => x.ApplicationUserId == user.Id, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            cartVM.OrderHeader.ApplicationUserId = user.Id;
            cartVM.OrderHeader.ApplicationUser = _db.ApplicationUserRepository.GetFirstOrDefault(x => x.Id == user.Id);
            cartVM.OrderHeader.Name = cartVM.OrderHeader.ApplicationUser.Name;
            cartVM.OrderHeader.PhoneNumber = cartVM.OrderHeader.ApplicationUser.PhoneNumber;
            cartVM.OrderHeader.State = cartVM.OrderHeader.ApplicationUser.State;
            cartVM.OrderHeader.City = cartVM.OrderHeader.ApplicationUser.City;
            cartVM.OrderHeader.Address = cartVM.OrderHeader.ApplicationUser.Address;
            cartVM.OrderHeader.PostalCode = cartVM.OrderHeader.ApplicationUser.PinCode.ToString();
            foreach (var cart in cartVM.ListCart)
            {
                cartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
            }
            if (cartVM.OrderHeader.OrderTotal <= 0)
            {
                return Ok("Please add item to shop.");
            }
            return Ok(cartVM);
        }

        [HttpPost]
        public IActionResult PlaceOrder(CartVM cartVM)
        {
            

                var cliamsIdentity = (ClaimsIdentity)User.Identity;
                var user = _db.ApplicationUserRepository.GetFirstOrDefault(x => x.UserName == cliamsIdentity.Name);
                cartVM.ListCart = _db.CartRepository.GetAll(x => x.ApplicationUserId == user.Id, includeProperties: "Product");
                cartVM.OrderHeader.OrderStatus = OrderStatus.StatusPending;
                cartVM.OrderHeader.PaymentStatus = PaymentStatus.PaymentStatusPending;
                cartVM.OrderHeader.DateOfOrder = DateTime.Now;
                cartVM.OrderHeader.ApplicationUserId = user.Id;
                cartVM.OrderHeader.OrderTotal = 0;
                cartVM.OrderHeader.Carrier = "";
                cartVM.OrderHeader.TrackingNumber = "";
                cartVM.OrderHeader.SessionId = "";
                cartVM.OrderHeader.PaymentIntentId = "";
                cartVM.OrderHeader.ApplicationUser= null;
                foreach (var cart in cartVM.ListCart)
                {
                    cartVM.OrderHeader.OrderTotal += (cart.Product.Price * cart.Count);
                }

                _db.OrderHeaderRepository.Add(cartVM.OrderHeader);
                _db.Save();
                foreach (var cart in cartVM.ListCart)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        ProductId = cart.ProductId,
                        OrderId = cartVM.OrderHeader.Id,
                        Count = cart.Count,
                        Price = cart.Product.Price,
                    };
                    _db.OrderDetailRepository.Add(orderDetail);
                    _db.Save();
                }
                StripeConfiguration.ApiKey = "sk_test_51N13aLSFAfjmjNoLp87Em7G31PHhRwYsJYIPfCooiJWgJq18grhRYaijLKxm9NIdxGf9fIpcQ6TvsXsrVdeSKOaE00rgq15NYx";

            var options = new PaymentIntentCreateOptions
            {
                Amount = 2000,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            var service = new PaymentIntentService();
           var response= service.Create(options);

            _db.CartRepository.RemoveRange(cartVM.ListCart);
            _db.Save();
            return Ok(response);

        }

     



        }
    }
