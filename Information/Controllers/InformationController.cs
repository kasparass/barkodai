using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Barkodai.Core;
using Barkodai.Models;
using Barkodai.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Barkodai.Information.Controllers
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
            if (show)
            {
                // setPersistantData("ShopsList", new ShopList { shops = await ItemsAPI.getShops(id) });
            }
            else
            {
                setPersistantData("ShopsList", null);
            }

            return RedirectToAction("Description", new { id });
        }

        [ActionName("Scan")]
        public async Task<IActionResult> openScan()
        {
            if (hasPersistantData("ScanVM"))
            {
                return View("~/Information/Views/CameraScan.cshtml", getPersistantData<ScanVM>("ScanVM"));
            }
            else
            {
                return View("~/Information/Views/CameraScan.cshtml", new ScanVM());
            }
        }

        [ActionName("ScanCapture")]
        [HttpPost]
        public async Task tryScan(string name)
        {
            Stream image = await getPhoto();
            (Item item, string message) itemResult = await ItemsAPI.getItemFromPhoto(image);

            setPersistantData("ScanVM", new ScanVM
            {
                item = itemResult.item,
                error_message = itemResult.item == null ? itemResult.message : null
            });
        }

        private async Task<Stream> getPhoto()
        {
            var files = HttpContext.Request.Form.Files;
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (!file.FileName.EndsWith("jpeg") && !file.FileName.EndsWith("jpg")) continue;

                    var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    return memoryStream;
                }
            }

            return null;
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