using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barkodai.Core;
using Barkodai.Models;
using Barkodai.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Barkodai.Offers.Controllers
{
    public class InformationController : ExtendedController
    {
        [ActionName("Index")]
        public async Task<IActionResult> openItemList(string message)
        {
            ItemInformationList il = getPersistantData<ItemInformationList>();
            il.items = await ItemsAPI.getItems();
            if (il.hiddenCategories == null)
            {
                il = applyDefaultFilters(il);
            }
            setPersistantData(il);
            il.cart_count = await Models.User.getCartCount();
            il.success_message = message;
            return View("~/Information/Views/ItemInformationList.cshtml", il);
        }

        [ActionName("Filter")]
        public IActionResult filter(List<string> filter_categories)
        {
            ItemInformationList il = getPersistantData<ItemInformationList>();
            if (il.items != null)
            {
                il = applyFilters(il, filter_categories);
                setPersistantData(il);
            }

            return RedirectToAction("Index");
        }

        [ActionName("Description")]
        public async Task<IActionResult> openItemDescription(int id)
        {
            Item item = await ItemsAPI.getItem(id);
            item.averageRating = await Rating.getAverageRating(item.id);
            // item.shops = getPersistantData<ShopList>("ShopsList").shops;
            return View("~/Information/Views/ItemDescription.cshtml", item);
        }

        [ActionName("ListShops")]
        public async Task<IActionResult> showShopsList(int id, bool show)
        {
            if(show)
            {
                // setPersistantData("ShopsList", new ShopList { shops = await ItemsAPI.getShops(id) });
            }
            else
            {
                setPersistantData("ShopsList", null);
            }
            
            return RedirectToAction("Description", new { id });
        }

        private ItemInformationList applyFilters(ItemInformationList il, List<string> filter_categories)
        {
            il.hiddenCategories = new HashSet<string>(il.items.Select(i => i.category.ToLower()).Distinct().Except(filter_categories));
            return il;
        }

        private ItemInformationList applyDefaultFilters(ItemInformationList il)
        {
            if (il.items.Any(i => i.category.ToLower().CompareTo("sex") == 0))
            {
                il.hiddenCategories = new HashSet<string> { "sex" };
            }
            else
            {
                il.hiddenCategories = new HashSet<string>();
            }
            return il;
        }
    }
}