using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCPopupCRUD
{
    [MetadataType(typeof(ContactMetaData))]
    public partial class Contact
    {
        public string CountryName { get; set; }
        public string StateName { get; set; }
    }

    public class ContactMetaData
    {
        [Required(ErrorMessage= "Contact Name required", AllowEmptyStrings = false)]
        [Display(Name="Contact Person")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage="Contact No required", AllowEmptyStrings=false)]
        [Display(Name="Contact No")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage="Country required")]
        [Display(Name="Country")]
        public int CountryID { get; set; }

        [Required(ErrorMessage="State required")]
        [Display(Name="State")]
        public int StateID { get; set; }
    }
}