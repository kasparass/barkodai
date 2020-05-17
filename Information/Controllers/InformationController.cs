﻿﻿using System;
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
        public async Task<IActionResult> openItemList()
        {
            BlockListVM vm = getPersistantData<BlockListVM>();
            vm.blockList = await BlockList.getList(Models.User.current);
            vm.items = await ItemsAPI.getItems();
            if (vm.hiddenCategories == null)
            {
                vm = applyDefaultFilters(vm);
            }
            setPersistantData(vm);
            return View("~/Information/Views/InformationList.cshtml", vm);
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