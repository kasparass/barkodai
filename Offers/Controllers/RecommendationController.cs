
using Barkodai.Core;
using Microsoft.AspNetCore.Mvc;
using Barkodai.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Barkodai.ViewModels;
using System.Linq;

namespace Barkodai.Offers.Controllers
{
    public class RecommendationController : ExtendedController
    {
        [ActionName("Index")]
        public async Task<IActionResult> openRecommendationList()
        {
            RecommendedListVM vm = getPersistantData<RecommendedListVM>();
            // vm.recommendedList = await RecommendedList.getList(Models.User.current);
            // if(vm.recommendedList.items.Count == 0)
            //     await Recommendation();
            await Recommendation();                                                     // kol kas be timerio
            vm.recommendedList = await RecommendedList.getList(Models.User.current);
            return View("~/Offers/Views/RecommendedItemList.cshtml", vm);
        }

        private async Task Recommendation()
        {
            await getItems();
            RecommendedListVM vm = getPersistantData<RecommendedListVM>();
            vm = applyDefaultFilters(vm);
            await getBlockList(Models.User.current);
            if(vm.blockList != null)
            {
                vm = applyBlockList(vm);
            }
            await getRating(vm);
            if(vm.ratings != null)
                vm = applyRatingList(vm);
            await addItems(vm);
        }

        // public async static void timeEventRecommendation()
        // {
        // }

        private async Task getItems()
        {
            RecommendedListVM vm = getPersistantData<RecommendedListVM>();
            vm.items = await ItemsAPI.getItems();
            setPersistantData(vm);
        }

        private RecommendedListVM applyDefaultFilters(RecommendedListVM vm)
        {
            vm.items = from item in vm.items
                where !item.category.ToLower().Equals("sex")
                select item;
            return vm;
        }

        private async Task getBlockList(User user)
        {
            RecommendedListVM vm = getPersistantData<RecommendedListVM>();
            vm.blockList = await BlockList.getList(user);
            setPersistantData(vm);
        }

        private RecommendedListVM applyBlockList(RecommendedListVM vm)
        {
            vm.items = vm.items.Where(i => !vm.blockList.items.Any(bi => bi.id == i.id));
            return vm;
        }

        private async Task getRating(RecommendedListVM vm)
        {
            vm.ratings = new List<Rating>();
            foreach(var item in vm.items)
            {
                vm.ratings.Append(await Rating.getAverageRating(item.id));
            }
        }

        private RecommendedListVM applyRatingList(RecommendedListVM vm)
        {
            List<Item> items = new List<Item>();
            foreach(var item in vm.items)
            {
                vm.ratings.ForEach(delegate(Rating rating) {
                    if(rating.item_id == item.id && rating.price >= 4 && rating.quality >= 4.5 && rating.use >= 4.8)
                        items.Append(item);
                });
            }
            vm.items = items;
            return vm;
        }

        private async Task addItems(RecommendedListVM vm) 
        {
            RecommendedList rl = await RecommendedList.getList(Models.User.current);
            if(rl.items != null)
            {
                foreach(var item in rl.items)
                {
                    await RecommendedList.deleteItem(Models.User.current, item.id);
                }
            }
            foreach(var item in vm.items)
            {
                await RecommendedList.addItem(Models.User.current, item.id);
            }
        }

    }
}