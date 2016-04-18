using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BYSDN.Models
{
    public class SearchResultModel
    {
        public string Id { get; set; }
        public string UserImage { get; set; }
        public string Title { get; set; }
        public string Sample { get; set; }
        public float Score { get; set; }
    }
}