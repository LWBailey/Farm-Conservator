﻿using Farm_Conservator.Models;
using Newtonsoft.Json;
using NLog;
using SLUIDBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Farm_Conservator.Controllers
{
    public class ContactController : Controller
    {
        public static Settings Settings;
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public ContactController()
        {

            Settings = new Settings();
        }

        private static readonly HttpClient Client = new HttpClient();

        // GET: Contacts
        public async Task<ActionResult> RemoveContact(int farmID, int ContactIRISID)
        {
            int FarmObjectID = await new FarmController().GetFarmObjectID(farmID);

            
            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(Settings.ContactAPI +"/Remove");
                Request.Method = HttpMethod.Put;
                Request.Content = new StringContent(JsonConvert.SerializeObject(new RemoveContactRequest
                {
                    ObjectID = FarmObjectID,
                    IRISID = ContactIRISID


                }), Encoding.UTF8, "application/json");
                

                using (var response = await Client.SendAsync(Request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        logger.Debug($"Contact {ContactIRISID} removed from object {FarmObjectID} by {System.Security.Principal.WindowsIdentity.GetCurrent().Name}");

                        return RedirectToAction("Edit", "Farm", new { id = farmID });

                    }


                    else
                    {
                        logger.Debug($"Failure when removing {ContactIRISID} from object {FarmObjectID} by {System.Security.Principal.WindowsIdentity.GetCurrent().Name}");

                        return RedirectToAction("Edit", "Farm", new { id = farmID });
                    }

                }





            }
        }



        public async Task<ActionResult> AddContact(AddContactRequest contactRequest)
        {


            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(Settings.ContactAPI + "/Add");
                Request.Method = HttpMethod.Put;
                Request.Content = new StringContent(JsonConvert.SerializeObject(new AddContactRequest
                {
                    ObjectID = contactRequest.ObjectID,
                    IRISID = contactRequest.IRISID,
                    RelationshipTypeID = contactRequest.RelationshipTypeID

                }), Encoding.UTF8, "application/json");

                int FarmID = await GetIDFromObject(contactRequest.ObjectID);

                using (var response = await Client.SendAsync(Request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        logger.Debug($"Contact {contactRequest.IRISID} added to object {contactRequest.ObjectID} by {System.Security.Principal.WindowsIdentity.GetCurrent().Name}");

                        return RedirectToAction("Edit", "Farm", new { id = FarmID });

                    }


                    else
                    {
                        return RedirectToAction("Edit", "Farm", new { id = FarmID });
                    }

                }
            }
        }


        public async Task<List<ContactModel>> GetContactListForFarm(int objectID)
        {


            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(Settings.ContactAPI+ "/{objectID}");
                Request.Method = HttpMethod.Get;


                using (var response = await Client.SendAsync(Request))
                {
                   string content = await response.Content.ReadAsStringAsync();

                    var ContactList = JsonConvert.DeserializeObject<List<ContactModel>>(content);

                    return ContactList;

                }
                                             

            }



        }

        public async Task<ActionResult> StartSearch(int ObjectID, [Optional] FarmModel model)
        {
            ViewBag.ObjectID = ObjectID;        
            ViewBag.ContactTypes = new List<string> { "Individual","Organisation" };

            return View("AddContactView");

        }

        public async Task<ActionResult> SelectRelationship(AddContactRequest contactRequest)
        {      
                                               
            return View("SelectRelationshipType", contactRequest);

        }


        public async Task<ActionResult> ContactSearch(int objectID,string ContactType, string LastName, [Optional]string FirstName)
        {

            using (var Request = new HttpRequestMessage())
            {



                Request.RequestUri = new Uri(Settings.IRISAPI + $@"/Contact?ContactType={(ContactType == "Individual"?1:2)}&Orgname={(ContactType== "Organisation"?LastName:"")}&LastName={(ContactType == "Individual" ? LastName : "")}&FirstName={FirstName}"); //This is an abuse of interpolation, I know. Should I have written this as a POST instead?
                Request.Method = HttpMethod.Get;

                using (var response = await Client.SendAsync(Request))
                {

                    string content = await response.Content.ReadAsStringAsync();

                    IEnumerable<ContactSearchResult> SearchResult = JsonConvert.DeserializeObject<List<ContactSearchResult>>(content);

                    if (SearchResult.Any())
                    {

                        ViewBag.ObjectID = objectID;
                        ViewBag.ContactTypes = new List<string> { "Individual", "Organisation" };

                        return View("AddContactView", SearchResult);

                    }

                    else
                    {
                        return null;

                    }


                }

            }
        }
        public async Task<int> GetIDFromObject(int ObjectID)
        {

            string API = Settings.ObjectAPI;

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(API + $"/object/{ObjectID}");
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
