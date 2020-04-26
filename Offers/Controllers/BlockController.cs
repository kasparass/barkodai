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
    public class BlockController : Controller
    {
        [ActionName("Index")]
        public async Task<IActionResult> openBlockedList()
        {
            BlockList list = await BlockList.getList(Models.User.current);
            return View("~/Offers/Views/BlockedItemCategoryList.cshtml", new BlockListVM { blockList = list });
        }

        [ActionName("ListItems")]
        public async Task<IActionResult> openItemList()
        {
            var vm = new BlockListVM
            {
                blockList = await BlockList.getList(Models.User.current),
                items = await ItemsAPI.getItems()
            };
            applyDefaultFilters(vm);
            return View("~/Offers/Views/BlockedItemCategoryList.cshtml", vm);
        }

        [ActionName("Filter")]
        public async Task<IActionResult> filter(List<string> filter_categories)
        {
            var vm = new BlockListVM
            {
                blockList = await BlockList.getList(Models.User.current),
                items = await ItemsAPI.getItems()
            };
            vm.hiddenCategories = new HashSet<string>(vm.items.Select(i => i.category.ToLower()).Distinct().Except(filter_categories));
            return View("~/Offers/Views/BlockedItemCategoryList.cshtml", vm);
        }

        [ActionName("AddItem")]
        public async Task<IActionResult> addItem(int id)
        {
            await BlockList.addItem(Models.User.current, id);

            return RedirectToAction("Index");
        }

        [ActionName("RemoveItem")]
        public async Task<IActionResult> removeItem(int id)
        {
            await BlockList.deleteItem(Models.User.current, id);

            return RedirectToAction("Index");
        }

        private void applyDefaultFilters(BlockListVM vm)
        {
            if (vm.items.Any(i => i.category.ToLower().CompareTo("sex") == 0))
            {
                vm.hiddenCategories = new HashSet<string> { "sex" };
            }
        }
    }
}