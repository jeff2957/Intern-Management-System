using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project2.Models
{
    public class AdminViewModel
    {
        public IEnumerable<Azure_DB.Task> PartialTask { get; set; }

        public IEnumerable<Azure_DB.Signin> PartialSignin { get; set; }

        
    }
}