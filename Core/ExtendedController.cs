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
            setPersistantData(GetType().ToString() + "_PersistantData", data);
        }

        protected void setPersistantData(string name, object data)
        {
            if(data is null)
            {
                TempData.Remove(name);
            }
            else
            {
                TempData[name] = JsonConvert.SerializeObject(data);
            }
        }

        protected T getPersistantData<T>() where T : struct => getPersistantData<T>(GetType().ToString() + "_PersistantData");


        protected T getPersistantData<T>(string name) where T : struct
        {
            if (TempData[name] == null || (TempData[name] as string) == null) return default(T);
            return JsonConvert.DeserializeObject<T>(TempData[name] as string);
        }

        protected void keepPersistantData()
        {
            TempData.Keep();
        }

        protected bool hasPersistantData()
        {
            return hasPersistantData(GetType().ToString() + "_PersistantData");
        }

        protected bool hasPersistantData(string name)
        {
            return TempData[name] != null;
        }
    }
}
