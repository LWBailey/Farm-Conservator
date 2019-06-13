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
using PagedList;
using Farm_Conservator.Models;
using NLog;

namespace Farm_Conservator.Controllers
{
   


    public class FarmController : Controller
    {
        public static Settings Settings;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static readonly HttpClient Client = new HttpClient();

        public FarmController()
        {

            Settings = new Settings();

        }

        //Get All Farms and display as list
        public async Task<ActionResult> Farms(int? page, string SearchFarm)
        {

            int pageSize;
            int pageNumber;
            List<FarmModel> FarmList;

            if (string.IsNullOrEmpty(SearchFarm)) // If there is no search display paged list of all farms
            {
                FarmList = await GetFarmList();

                FarmList = FarmList.OrderBy(f => f.FarmName).ToList();

                 pageSize = 50;
                 pageNumber = (page ?? 1);
            }

            else
            {

                FarmList = await GetFarmByName(SearchFarm)?? null;
                FarmList = FarmList.OrderBy(f => f.FarmName).ToList();

                if (FarmList == null)
                {
                    pageSize = 50;
                    pageNumber = (page ?? 1);
                    FarmList = await GetFarmList();
                    ViewBag.SearchResultMessage = $"No farms found matching {SearchFarm}";

                }

                else
                {

                    ViewBag.SearchResultMessage = $"{FarmList.Count.ToString()} farms found matching {SearchFarm}";

                    if (FarmList.Count == 1)
                    {
                        pageSize = 1;
                        pageNumber = (page ?? 1);


                    }

                    else
                    {
                        pageSize = 50;
                        pageNumber = (page ?? 1);

                    }

                }


            }


            return View(FarmList.ToPagedList(pageNumber, pageSize));

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

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(Settings.FarmAPI + "/" + id);
                Request.Method = HttpMethod.Get;


                using (var response = await Client.SendAsync(Request))
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var FarmToEdit = JsonConvert.DeserializeObject<FarmModel>(content);

                    if (FarmToEdit == null)
                    {
                        return HttpNotFound();
                    }

                    else
                    {
                        ViewBag.PriorityList = new List<string> { "Very High", "High", "Medium", "Low" };

                        ViewBag.Message = message;
                        return View(FarmToEdit);
                    }
                }





            }

        }


        public async Task<List<FarmModel>> GetFarmList()
        {

          
            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(Settings.FarmAPI);
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

           

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(Settings.FarmAPI + $"/{farmID}");
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



            

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(Settings.FarmAPI);
                Request.Method = HttpMethod.Put;
                Request.Content = new StringContent(JsonConvert.SerializeObject(_farm), Encoding.UTF8, "application/json");






                using (var response = await Client.SendAsync(Request))
                {
                    string content = await response.Content.ReadAsStringAsync();

                    int.TryParse(content, out int FarmID);

                    if (FarmID != 0)
                    {


                        logger.Debug($"Farm {FarmID} updated by {System.Security.Principal.WindowsIdentity.GetCurrent().Name}");

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
                


                using (var Request = new HttpRequestMessage())
                {

                    Request.RequestUri = new Uri(Settings.FarmAPI);
                    Request.Method = HttpMethod.Post;
                    Request.Content = new StringContent(JsonConvert.SerializeObject(_farm), Encoding.UTF8, "application/json");



                    using (var response = await Client.SendAsync(Request))
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        FarmModel ReturnedFarm = JsonConvert.DeserializeObject<FarmModel>(content);


                        if (ReturnedFarm.ID != 0)
                        {
                            logger.Debug($"Farm {ReturnedFarm.ID} created by {System.Security.Principal.WindowsIdentity.GetCurrent().Name}");
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
            

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(Settings.ObjectAPI + $"/object/delete/{ObjectID}");
                Request.Method = HttpMethod.Get;


                using (var response = await Client.SendAsync(Request))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        logger.Debug($"Farm {ObjectID} deleted by {System.Security.Principal.WindowsIdentity.GetCurrent().Name}");
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



        public async Task<List<FarmModel>> GetFarmByName(string FarmName)
        {

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(Settings.FarmAPI +"/" + FarmName);
                Request.Method = HttpMethod.Get;


                using (var response = await Client.SendAsync(Request))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        var FarmList = JsonConvert.DeserializeObject<List<FarmModel>>(content);

                        return FarmList;


                    }

                    else
                    {
                        return null;

                    }



                }


            }


        }


    }
    }
