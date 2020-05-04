using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barkodai.Core
{
    public class ExtendedController : Controller
    {
        protected void setPersistantData(object data)
        {
            TempData[GetType().ToString() + "_PersistantData"] = JsonConvert.SerializeObject(data);
        }

        protected T getPersistantData<T>() where T : struct
        {
            if (TempData[GetType().ToString() + "_PersistantData"] == null) return default(T);
            return JsonConvert.DeserializeObject<T>(TempData[GetType().ToString() + "_PersistantData"] as string);
        }

        protected void keepPersistantData()
        {
            TempData.Keep();
        }

        protected bool hasPersistantData()
        {
            return TempData[GetType().ToString() + "_PersistantData"] != null;
        }
    }
}
