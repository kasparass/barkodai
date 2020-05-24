using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barkodai.Core;
using Barkodai.Models;
using Barkodai.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Barkodai.Orders.Controllers
{
    public class CartController : ExtendedController
    {
        [ActionName("AddToCart")]
        public async Task<IActionResult> addItemToCart(int id)
        {
            Cart cart = await Cart.getCart(Models.User.current.id);
            if (cart.id != -1)
            {
                bool exists = await Cart.isItemInCart(cart.id, id);
                if (!exists)
                {
                    await Cart.addItemToCart(cart.id, id);
                }
            }
            else
            {
                cart.id = await Cart.create(Models.User.current.id);
                await Cart.addItemToCart(cart.id, id);
            }

            return RedirectToAction("Index", "Information", new { message = "Item was added to the cart." });
        }

        [ActionName("Index")]
        public async Task<IActionResult> getCart(string message, string type)
        {
            Cart cart = await Cart.getCart(Models.User.current.id);

            return View("~/Orders/Views/CartView.cshtml", new CartVM { cart = cart, message = message, message_type = type });
        }

        [ActionName("Remove")]
        public async Task<IActionResult> removeItem(int id)
        {
            await Cart.deleteFromCart(Models.User.current.id, id);
            return RedirectToAction("Index");
        }
        
        [ActionName("Order")]
        public async Task<IActionResult> orderCart(int id)
        {
            Cart cart = await Cart.getCart(Models.User.current.id);
            string url = await PayseraAPI.pay(cart);
            
            return Redirect(url);
        }
        
        [ActionName("OrderSuccess")]
        public async Task<IActionResult> orderSuccess(int id)
        {
            await Order.create(Models.User.current.id, id);
            await Cart.changeToOrdered(id);
            
            return RedirectToAction("Index", new { message = "Payment Successful", type = "success" } );
        }
        
        [ActionName("OrderFailed")]
        public async Task<IActionResult> orderFailed(int id)
        {
            return RedirectToAction("Index", new { message = "Order Canceled", type = "error" } );
        }
        
    }
}