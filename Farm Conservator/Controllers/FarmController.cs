using Newtonsoft.Json;
using SLUIDBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        [Route("farm/create")]
        public async Task<ActionResult> Create()
        {

            FarmModel Newfarm = new FarmModel();

            ViewBag.PriorityList = new List<string> { "Very High", "High", "Medium", "Low" };
            return View(Newfarm);

        }

        [Route("farm/edit/{id}")]
        public async Task<ActionResult> Edit(int? id, string message)
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

            ViewBag.Message = message;
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

                Request.RequestUri = new Uri(API + $"/{farmID}");
                Request.Method = HttpMethod.Post;


                using (var response = await Client.SendAsync(Request))
                {
                    string content = await response.Content.ReadAsStringAsync();

                    int.TryParse(content, out int FarmID);

                    return FarmID;

                }




            }

        }




        public async Task<ActionResult> UpdateFarm(FarmModel _farm)
        {
            ViewBag.PriorityList = new List<string> { "Very High", "High", "Medium", "Low" };



            string API = "http://localhost:54082/API/Farm/";

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(API);
                Request.Method = HttpMethod.Put;
                Request.Content = new StringContent(JsonConvert.SerializeObject(_farm), Encoding.UTF8, "application/json");






                using (var response = await Client.SendAsync(Request))
                {
                    string content = await response.Content.ReadAsStringAsync();

                    int.TryParse(content, out int FarmID);

                    if (FarmID != 0)
                    {




                        return RedirectToAction("Edit", "Farm", new { id = FarmID, message = "Farm successfully updated" });
                    }

                    else return RedirectToAction("Edit", "Farm", new { id = FarmID, message = "Error updating farm" });

                }


            }




        }








        public async Task<ActionResult> CreateFarm(FarmModel _farm)
        {

            if (ModelState.IsValid)
            {


                string API = "http://localhost:54082/API/Farm/";

                using (var Request = new HttpRequestMessage())
                {

                    Request.RequestUri = new Uri(API);
                    Request.Method = HttpMethod.Post;
                    Request.Content = new StringContent(JsonConvert.SerializeObject(_farm), Encoding.UTF8, "application/json");



                    using (var response = await Client.SendAsync(Request))
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        FarmModel ReturnedFarm = JsonConvert.DeserializeObject<FarmModel>(content);


                        if (ReturnedFarm.ID != 0)
                        {

                            return RedirectToAction("Edit", "Farm", new { id = ReturnedFarm.ID, message = "Farm creation successful" });
                        }

                        else return RedirectToAction("Edit", "Farm", new { id = ReturnedFarm.ID, message = "Error with farm creation" });

                    }

                }

            }


            else return RedirectToAction("Edit", "Farm", new { id = _farm.ID, message = "Error with farm creation" });
        }

        public async Task<ActionResult> DeleteFarm(int ObjectID)
        {
            string API = "http://localhost:54082/API/";

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(API + $"/object/delete/{ObjectID}");
                Request.Method = HttpMethod.Get;


                using (var response = await Client.SendAsync(Request))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                       return RedirectToAction("Farms", "Farm");

                    }

                    else
                    {
                        return RedirectToAction("Farms", "Farm", new {message = "Could not delete farm" });

                    }



                }


            }

        }

        public ActionResult NavigateToIRISContactScreen(int IRISID)
        {

            return Redirect($"http://irislive.horizons.govt.nz/ContactDetails.aspx?ContactID={IRISID}");

        }


    }
    }
