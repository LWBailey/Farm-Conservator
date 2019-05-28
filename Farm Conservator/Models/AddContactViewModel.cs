using SLUIDBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Farm_Conservator.Models
{
    public class AddContactViewModel
    {
        public List<ContactSearchResult>  SearchResults {get;set;}
        public FarmModel FarmModel { get; set; }
        
    }
}