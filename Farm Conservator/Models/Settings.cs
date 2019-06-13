using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Farm_Conservator.Models
{
    public class Settings
    {
        public string FarmAPI { get; set; }
        public string ContactAPI { get; set; }
        public string ObjectAPI { get; set; }
        public string IRISAPI { get; set; }

        public Settings()
        {
            

            var SettingsFile = XElement.Load(Path.GetFullPath(Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Settings"))[0]));


            var FarmAPIElement = from el in SettingsFile.Elements("APISettings").Elements("API")
                            where (string)el.Attribute("Name") == "FarmAPI"
                            select el.Element("BaseURL").Value;

            FarmAPI = FarmAPIElement.SingleOrDefault().ToString().Trim();


            var ContactAPIElement = from el in SettingsFile.Elements("APISettings").Elements("API")
                          where (string)el.Attribute("Name") == "ContactAPI"
                          select el.Element("BaseURL").Value;


            ContactAPI = ContactAPIElement.SingleOrDefault().ToString().Trim();

            var ObjectAPIElement = from el in SettingsFile.Elements("APISettings").Elements("API")
                                    where (string)el.Attribute("Name") == "ObjectAPI"
                                    select el.Element("BaseURL").Value;


            ObjectAPI = ObjectAPIElement.SingleOrDefault().ToString().Trim();

            var IRISAPIElement = from el in SettingsFile.Elements("APISettings").Elements("API")
                                    where (string)el.Attribute("Name") == "IRISAPI"
                                    select el.Element("BaseURL").Value;


          IRISAPI = IRISAPIElement.SingleOrDefault().ToString().Trim();



        }

    }

    
}