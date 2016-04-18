using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BYSDN.Models
{
    public class QuestionBriefModel
    {
        public string Title { get; set; }
        public Guid BBSId { get; set; }
        public string PublishDate { get; set; }
        public string UserImage { get; set; }
        public string Brief { get; set; }
        public string Tags { get; set; }
    }
}