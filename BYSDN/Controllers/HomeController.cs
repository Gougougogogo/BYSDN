using BYSDN.Entity;
using BYSDN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BYSDN.Controllers
{
    public class HomeController : Controller
    {
        BYSDNEntities entities = new BYSDNEntities();
        public ActionResult Index()
        {
            var person = (from a in entities.Table_User
                          where a.Name == Request.LogonUserIdentity.Name
                          select a.ID).FirstOrDefault();

            if (person == Guid.Empty)
            {
                return RedirectToAction("Registe", "User");
            }


            DateTime yesterday = DateTime.Now.AddDays(-1).ToUniversalTime();

            var userCount = (from a in entities.Table_User
                             select a.ID).Count();

            var questionCount = (from a in entities.Table_Question
                                 where a.Date > yesterday
                                 select a.ID).Count();

            var answersCount = (from a in entities.Table_Answer
                                where a.Date > yesterday
                                select a.ID).Count();

            List<QuestionBriefModel> latestQuestion = (from a in entities.Table_Question
                                                       orderby a.Date descending
                                                       select new QuestionBriefModel
                                                       {
                                                           BBSId = a.ID,
                                                           Brief = a.Content,
                                                           Title = a.Tittle,
                                                           Tags = a.Tags,
                                                           UserImage = a.Table_User.Photo
                                                       }).Take(4).ToList();

            foreach (var obj in latestQuestion)
            {
                obj.Brief = GetBrief(obj.Brief);
            }

            HomePageModel model = new HomePageModel();
            model.UserCount = userCount;
            model.AnswerCount = answersCount;
            model.QuestionCount = questionCount;
            model.LatestQuestion = latestQuestion;
            return View(model);
        }

        public JsonResult GetSummaryData()
        {
            DateTime time = new DateTime(DateTime.Now.ToUniversalTime().Year, DateTime.Now.ToUniversalTime().Month, 1);
            
            var question = (from a in entities.Table_Question
                            select new
                            {
                                Data = a.Date
                            }).ToList();

            var answer = (from a in entities.Table_Answer
                          select new
                          {
                              Data = a.Date
                          }).ToList();

            List<SummaryModel> summary = new List<SummaryModel>();

            for (int i = 1; i <= DateTime.DaysInMonth(time.Year, time.Month); i++)
            {
                SummaryModel model = new SummaryModel();
                model.Day = i;
                model.Question = question.FindAll(C => (C.Data > time.AddDays(i - 1) && C.Data < time.AddDays(i))).Count;
                model.Answer = answer.FindAll(C => (C.Data > time.AddDays(i - 1) && C.Data < time.AddDays(i))).Count;
                summary.Add(model);
            }

            return Json(new { success = true, retData = summary }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserInfo()
        {
            var person = (from a in entities.Table_User
                          where a.Name == Request.LogonUserIdentity.Name
                          select new { 
                            Img = a.Photo,
                            Comment = a.Comments
                          }).FirstOrDefault();
            return Json(person, JsonRequestBehavior.AllowGet);
        }

        private string GetBrief(string htmlContent)
        {
            string temp = Regex.Replace(htmlContent, "<[^>]*>", "");
            if (temp.Length > 360)
            {
                return temp.Replace("&nbsp;", " ").Substring(0, 360) + "...";
            }
            return temp.Replace("&nbsp;", " ");
        }
    }
}

