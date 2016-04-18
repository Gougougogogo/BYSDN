using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BYSDN.Models
{
    public class HomePageModel
    {
        public int UserCount { get; set; }
        public int QuestionCount { get; set; }
        public int AnswerCount { get; set; }
        public List<QuestionBriefModel> LatestQuestion { get; set; }
    }
}