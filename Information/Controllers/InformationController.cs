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
using Newtonsoft.Json;

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
            ItemDescriptionVM vm = new ItemDescriptionVM { 
                item = await ItemsAPI.getItem(id), 
                showShops = getPersistantData<bool>("description_show_shops"),
                userRating = await Rating.getRating(Models.User.current.id, id),
                userId = Models.User.current.id
            };
            vm.item.averageRating = await Rating.getAverageRating(vm.item.id);

            return View("~/Information/Views/ItemDescription.cshtml", vm);
        }

        [ActionName("ListShops")]
        public IActionResult showShopsList(int id, bool show)
        {
            setPersistantData("description_show_shops", show);
            return RedirectToAction("Description", new { id });
        }

        [ActionName("Rate"), HttpPost]
        public async Task<IActionResult> submitRating(float use, float price, float quality, int user_id, int item_id)
        {
            Rating rating = new Rating
            {
                use = use,
                price = price,
                quality = quality,
                user_id = user_id,
                item_id = item_id
            };
            keepPersistantData();
            await Rating.create(rating);
            return RedirectToAction("Description", new { id = rating.item_id });
        }

        [ActionName("Scan")]
        public IActionResult openScan()
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