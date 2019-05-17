using Newtonsoft.Json;
using SLUIDBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Farm_Conservator.Controllers
{
    public class FarmController : Controller
    {
        private static readonly HttpClient Client = new HttpClient();



        //Get All Farms and display as list
        public async Task<ActionResult> Farms()
        {
            return View(await GetFarmList());

        }

        [Route("farm/edit/{id}")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<FarmModel> Farms = await GetFarmList();

            FarmModel FarmToEdit = Farms.FirstOrDefault(f => f.ID == id);

            if (FarmToEdit == null)
            {
                return HttpNotFound();
            }

            ViewBag.PriorityList = new List<string> { "Very High", "High", "Medium", "Low" };


            return View(FarmToEdit);
        }


        public async Task<List<FarmModel>> GetFarmList()
        {

            string API = "http://localhost:54082/API/Farm/";

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(API);
                Request.Method = HttpMethod.Get;


                using (var response = await Client.SendAsync(Request))
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var FarmList = JsonConvert.DeserializeObject<List<FarmModel>>(content);

                    return FarmList;

                }






            }



        }

        public async Task<int> GetFarmObjectID(int farmID)
        {

            string API = "http://localhost:54082/API/Farm/";

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(API + $"object/{farmID}");
                Request.Method = HttpMethod.Get;


                using (var response = await Client.SendAsync(Request))
                {
                    string content = await response.Content.ReadAsStringAsync();
                    
                    int.TryParse(content, out int FarmID);

                    return FarmID;

                }




            }

        }
    }
}
