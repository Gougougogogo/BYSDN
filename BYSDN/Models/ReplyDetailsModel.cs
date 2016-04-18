using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BYSDN.Models
{
    public class ReplyDetailsModel
    {
        public int Order { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserImg { get; set; }
        public string ReplyContent { get; set; }
    }
}