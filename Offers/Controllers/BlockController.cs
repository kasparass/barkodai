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
    public class BlockController : ExtendedController
    {
        [ActionName("Index")]
        public async Task<IActionResult> openBlockedList()
        {
            BlockListVM vm = getPersistantData<BlockListVM>();
            vm.blockList = await BlockList.getList(Models.User.current);
            setPersistantData(vm);
            return View("~/Offers/Views/BlockedItemCategoryList.cshtml", vm);
        }

        [ActionName("ListItems")]
        public async Task<IActionResult> openItemList()
        {
            BlockListVM vm = getPersistantData<BlockListVM>();
            vm.items = await ItemsAPI.getItems();
            if (vm.hiddenCategories == null)
            {
                vm = applyDefaultFilters(vm);
            }

            setPersistantData(vm);
            return RedirectToAction("Index");
        }

        [ActionName("Filter")]
        public IActionResult filter(List<string> filter_categories)
        {
            BlockListVM vm = getPersistantData<BlockListVM>();
            if (vm.items != null)
            {
                vm = applyFilters(vm, filter_categories);
                setPersistantData(vm);
            }

            return RedirectToAction("Index");
        }

        [ActionName("AddItem")]
        public async Task<IActionResult> submitItem(int id)
        {
            await BlockList.addItem(Models.User.current, id);
            return RedirectToAction("ListItems");
        }

        [ActionName("RemoveItem")]
        public async Task<IActionResult> removeItem(int id)
        {
            await BlockList.deleteItem(Models.User.current, id);
            if (getPersistantData<BlockListVM>().items != null)
            {
                return RedirectToAction("ListItems");
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        private BlockListVM applyFilters(BlockListVM vm, List<string> filter_categories)
        {
            vm.hiddenCategories = new HashSet<string>(vm.items.Select(i => i.category.ToLower()).Distinct().Except(filter_categories));
            return vm;
        }

        private BlockListVM applyDefaultFilters(BlockListVM vm)
        {
            if (vm.items.Any(i => i.category.ToLower().CompareTo("sex") == 0))
            {
                vm.hiddenCategories = new HashSet<string> { "sex" };
            }
            else
            {
                vm.hiddenCategories = new HashSet<string>();
            }
            return vm;
        }
    }
}