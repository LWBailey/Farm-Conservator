using Newtonsoft.Json;
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

        public static string APIURL = "http://localhost:54082/API";
        public static string IRISContactAPIURL = "http://localhost:49660/API";
        private static readonly HttpClient Client = new HttpClient();

        // GET: Contacts
        public async Task<ActionResult> RemoveContact(int farmID, int ContactIRISID)
        {
            int FarmObjectID = await new FarmController().GetFarmObjectID(farmID);


            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(APIURL + "/Contact/Remove");
                Request.Method = HttpMethod.Put;
                Request.Content = new StringContent(JsonConvert.SerializeObject(new SLUIDBModels.RemoveContactRequest
                {
                    ObjectID = FarmObjectID,
                    IRISID = ContactIRISID


                }), Encoding.UTF8, "application/json");


                using (var response = await Client.SendAsync(Request))
                {
                    if (response.IsSuccessStatusCode)
                    {

                        return RedirectToAction("Edit", "Farm", new { id = farmID });

                    }


                    else
                    {
                        return RedirectToAction("Edit", "Farm", new { id = farmID });
                    }

                }





            }
        }

        public async Task<ActionResult> AddContact(int farmID, int ContactIRISID, int ContactType)
        {
            int FarmObjectID = await new FarmController().GetFarmObjectID(farmID);


            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(APIURL + "/Contact/Remove");
                Request.Method = HttpMethod.Put;
                Request.Content = new StringContent(JsonConvert.SerializeObject(new SLUIDBModels.RemoveContactRequest
                {
                    ObjectID = FarmObjectID,
                    IRISID = ContactIRISID


                }), Encoding.UTF8, "application/json");


                using (var response = await Client.SendAsync(Request))
                {
                    if (response.IsSuccessStatusCode)
                    {

                        return RedirectToAction("Edit", "Farm", farmID);

                    }


                    else
                    {
                        return RedirectToAction("Edit", "Farm", farmID);
                    }

                }
            }
        }


        public async Task<List<ContactModel>> GetContactListForFarm(int objectID)
        {


            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(APIURL+ "/contact/{objectID}");
                Request.Method = HttpMethod.Get;


                using (var response = await Client.SendAsync(Request))
                {
                   string content = await response.Content.ReadAsStringAsync();

                    var ContactList = JsonConvert.DeserializeObject<List<ContactModel>>(content);

                    return ContactList;

                }
                                             

            }



        }

        public async Task<ActionResult> StartSearch(int ObjectID)
        {
            ViewBag.ObjectID = ObjectID;          

            return View("AddContactView");

        }

        public async Task<ActionResult> Modal(AddContactRequest contactRequest)
        {

            List<string> RelationshipTypes = new List<string> { "Primary", "Owner", "Manager", "Other" };
            ViewBag.RelationshipTypes = RelationshipTypes;
                                               
            return View("SelectRelationshipType", contactRequest);

        }


        public async Task<ActionResult> ContactSearch(int objectID,int ContactType, string LastName, [Optional]string FirstName)
        {

            using (var Request = new HttpRequestMessage())
            {

                Request.RequestUri = new Uri(IRISContactAPIURL + $@"/Contact?ContactType={ContactType}&Orgname={(ContactType==2?LastName:"")}&LastName={(ContactType == 1 ? LastName : "")}&FirstName={FirstName}"); //This is an abuse of interpolation, I know. Should I have written this as a POST instead?
                Request.Method = HttpMethod.Get;

                using (var response = await Client.SendAsync(Request))
                {

                    string content = await response.Content.ReadAsStringAsync();

                    IEnumerable<ContactSearchResult> SearchResult = JsonConvert.DeserializeObject<List<ContactSearchResult>>(content);

                    if (SearchResult.Any())
                    {

                        ViewBag.ObjectID = objectID;
                        

                        return View("AddContactView", SearchResult);

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
