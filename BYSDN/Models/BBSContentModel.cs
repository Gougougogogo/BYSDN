using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BYSDN.Models
{
    public class BBSContentModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string PublisherName { get; set; }
        public string PublisherEmail { get; set; }
        public string PublisherImg { get; set; }
        public string Date { get; set; }
        public string Tags { get; set; }
        public string BBSId { get; set; }
    }
}