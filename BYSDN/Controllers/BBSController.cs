using BYSDN.Common;
using BYSDN.Entity;
using BYSDN.Lucene;
using BYSDN.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BYSDN.Controllers
{
    public class BBSController : Controller
    {
        BYSDNEntities entities = new BYSDNEntities();
        //
        // GET: /BBS/
        public ActionResult Index()
        {
            BBSHomePageDataModel model = new BBSHomePageDataModel();

            Table_User user = (from a in entities.Table_User
                               where a.Name == Request.LogonUserIdentity.Name
                               select a).FirstOrDefault();
            if (user != null)
            {
                model.QuestionCount = (from a in entities.Table_Question
                                       where a.Publisher == user.ID
                                       select a.ID).Count();
                model.AnswersCount = (from a in entities.Table_Answer
                                      where a.Publisher == user.ID
                                      select a.ID).Count();
            }
            else
            {
                model.QuestionCount = model.AnswersCount = 0;
            }

            int bbsCount = (from a in entities.Table_Question
                            select a.ID).Count();
            model.PageCount = (bbsCount / 15) + (bbsCount % 15 > 0 ? 1 : 0);

            return View(model);
        }

        public ActionResult NewQuestion()
        {
            return View();
        }

        public ActionResult Details(string ID)
        {
            Guid Id = new Guid(ID);
            Table_Question question = (from a in entities.Table_Question
                                       where a.ID == Id
                                       select a).FirstOrDefault();
            if(question != null)
            {
                BBSContentModel result = new BBSContentModel();
                result.BBSId = question.ID.ToString();
                result.Content = question.Content;
                result.Date = GetStringData(question.Date);
                result.PublisherEmail = GetEmail(question.Table_User.Name);
                result.PublisherImg = "../"+question.Table_User.Photo;
                result.PublisherName = question.Table_User.Name;
                result.Tags = question.Tags;
                result.Title = question.Tittle;
                return View(result);
            }
            return RedirectToAction("Index", "Page");
        }

        public JsonResult RequestQuestionList(int page)
        {
            int start = (page -1) * 10;

            List<BBSTitleModel> bbs = (from a in entities.Table_Question
                                       orderby a.Date descending
                                       select a)
                                       .Skip(start)
                                       .Take(10)
                                       .AsEnumerable()
                                       .Select(c =>
                                       new BBSTitleModel()
                                       {
                                           BBSId = c.ID.ToString(),
                                           PublishDate = GetStringData(c.Date),
                                           Title = c.Tittle,
                                           UserImage = c.Table_User.Photo,
                                           Brief = GetBrief(c.Content)
                                       }).ToList();

            return Json(new { success = true, retData = bbs }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Test()
        {
            return View();
        }

        public JsonResult GetReplyInfos(string Id)
        {
            Guid id = new Guid(Id);

            int result = (from a in entities.Table_Answer
                          where a.QuestionID == id
                          select a.ID).Count();

            return Json(new { success = true, retData = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReplyDetails(string Id,int Page)
        {
            int offSet = (Page -1 ) * 10;

            Guid id = new Guid(Id);

            var result = (from a in entities.Table_Answer
                          where a.QuestionID == id
                          orderby a.Date ascending
                          select new ReplyDetailsModel
                          {
                              ReplyContent = a.Content,
                              UserName = a.Table_User.Name,
                              UserImg = a.Table_User.Photo
                          }).Skip(offSet).Take(10).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                result[i].Order = offSet + i + 1;
                result[i].UserEmail = GetEmail(result[i].UserName);
            }

            return Json(new { success = true, retData = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAttachments(string Id)
        {
            Guid ID = new Guid(Id);

            var result = (from a in entities.Table_Attachments
                          where a.QuestionID == ID
                          select new
                          {
                              ID = a.ID,
                              Name = a.FileName,
                              Size = a.FileSize
                          }).ToList();

            return Json(new { success = true, retData = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RequestPublish()
        {
            string title, tags, content;

            title = Request.Params["title"];
            tags = Request.Params["tags"];
            content = htmlDeCode(Request.Params["bbsContent"]);
            List<AttachmentModel> attachments  = null;

            if (Request.Params["attachments"] != null)
            {
                attachments = JsonConvert.DeserializeObject<List<AttachmentModel>>(Request.Params["attachments"]);
            }
            else
            {
                attachments = new List<AttachmentModel>();
            }

            try
            {
                Table_User user = (from a in entities.Table_User
                                   where a.Name == Request.LogonUserIdentity.Name
                                   select a).FirstOrDefault();
                if (user != null)
                {
                    Table_Question newQuestion = new Table_Question();
                    newQuestion.ID = Guid.NewGuid();
                    newQuestion.Content = content;
                    newQuestion.Date = DateTime.Now.ToUniversalTime();
                    newQuestion.Publisher = user.ID;
                    newQuestion.Tags = tags;
                    newQuestion.Tittle = title;

                    entities.Table_Question.Add(newQuestion);
                    List<Table_Attachments> files = new List<Table_Attachments>();
                    foreach (var obj in attachments)
                    {
                        files.Add(new Table_Attachments()
                        {
                            ID = Guid.NewGuid(),
                            QuestionID = newQuestion.ID,
                            FileName = obj.FileName,
                            FileSize = obj.FileSize,
                            Table_Question = newQuestion
                        });
                    }
                    
                    entities.Table_Attachments.AddRange(files);

                    entities.SaveChanges();

                    Lucener lucener = new Lucener();
                    lucener.AddData(newQuestion.ID.ToString(), newQuestion.Content, newQuestion.Tags, newQuestion.Tittle,"BBS");

                    return Json(new { success = true, retData = newQuestion.ID }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, retData = "The user is not registe" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbEntityValidationException e)
            {
                return Json(new { success = false , retData = e.Message }, JsonRequestBehavior.AllowGet);             
            }
        }
        [HttpPost]
        public JsonResult SubmitReply() 
        {
            Guid id = new Guid(Request.Params["Id"]);
            string replyContent = htmlDeCode(Request.Params["replyContent"]);
            try
            {
                Table_User user = (from a in entities.Table_User
                                   where a.Name == Request.LogonUserIdentity.Name
                                   select a).FirstOrDefault();
                if (user != null)
                {
                    Table_Answer answer = new Table_Answer();
                    answer.ID = Guid.NewGuid();
                    answer.Content = replyContent;
                    answer.Publisher = user.ID;
                    answer.Table_User = user;
                    answer.QuestionID = id;
                    answer.Date = DateTime.Now.ToUniversalTime();

                    entities.Table_Answer.Add(answer);
                    entities.SaveChanges();

                    Lucener lucener = new Lucener();
                    lucener.AddData(id.ToString(), answer.Content, string.Empty, string.Empty , "BBS");

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, retData = "The user is not registe" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbEntityValidationException e)
            {
                return Json(new { success = false, retData = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSearchResult(string keyword)
        {
            Lucener lucene = new Lucener();

            List<SearchResult> searchResult = lucene.Search(keyword, 10).ToList();

            List<Guid> bbsIds = searchResult.Select(c => new Guid(c.Id)).ToList();

            var result = (from a in entities.Table_Question
                         where bbsIds.Contains(a.ID)
                         select a)
                        .Take(10)
                        .AsEnumerable()
                        .Select(c =>
                        new SearchResultModel()
                        {
                            Id = c.ID.ToString(),
                            Title = c.Tittle,
                            UserImage = c.Table_User.Photo,
                        }).ToList();

            foreach (var obj in result)
            {
                SearchResult sr = searchResult.Find(c => c.Id == obj.Id);
                if (sr != null)
                {
                    obj.Sample = sr.Sample;
                    obj.Score = sr.Score;
                }
            }

            result.Sort((x, y) => 
            {
                if (x.Score > y.Score)
                    return -1;
                else
                    return 1;
            });

            return Json(new { success = true, retData = result }, JsonRequestBehavior.AllowGet);     
        }

        public JsonResult UploadAttachments(HttpPostedFileBase file)
        {
            try
            {
                string fileName = string.Empty;
                if (file != null && file.ContentLength > 0)
                {
                    fileName = DateTime.Now.Ticks + "%%" + file.FileName;
                    var path = Path.Combine(Server.MapPath("~/Attachments/"), fileName);
                    file.SaveAs(path);
                    return Json(new { success = true, serverFileName = fileName }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception error)
            {
                return Json(new { success = false, retData = error.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadFile(string Id)
        {
            Guid ID = new Guid(Id);

            var fileName = (from a in entities.Table_Attachments
                            where a.ID == ID
                            select a.FileName).FirstOrDefault();

            var path = Path.Combine(Server.MapPath("~/Attachments/"), fileName);

            var outPutName = path.Split('%')[2];

            return File(path, MIMETypeHelper.GetMimeType(Path.GetExtension(path)),Server.UrlPathEncode(outPutName));
        }

        private string GetStringData(DateTime dt)
        {
            var span = DateTime.Now.ToUniversalTime() - dt;
            if (span.TotalDays > 60)
            {
                return "long time ago";
            }
            else
            {
                if (span.TotalDays > 30)
                {
                    return "1 Months ago";
                }
                else
                {
                    if (span.TotalDays > 14)
                    {
                        return "2 weeks ago";
                    }
                    else
                    {
                        if (span.TotalDays > 7)
                        {
                            return "1 weeks ago";
                        }
                        else
                        {
                            if (span.TotalDays > 1)
                            {
                                return string.Format("{0} days ago", (int)Math.Floor(span.TotalDays));
                            }
                            else
                            {
                                if (span.TotalHours > 1)
                                {
                                    return string.Format("{0} hours ago", (int)Math.Floor(span.TotalHours));
                                }
                                else
                                {
                                    if (span.TotalMinutes > 1)
                                    {
                                        return string.Format("{0} minutes ago", (int)Math.Floor(span.TotalMinutes));
                                    }
                                    else
                                    {
                                        if (span.TotalSeconds >= 1)
                                        {
                                            return string.Format("{0} second ago", (int)Math.Floor(span.TotalSeconds));
                                        }
                                        else
                                        {
                                            return "just now";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

        private string GetEmail(string userName)
        {
            string result = string.Empty;
            for (int i = userName.Length-1; i >= 0; i--)
            {
                if (userName[i] != '\\')
                {
                    result = userName[i] + result;
                }
                else
                {
                    break;
                }
            }
            return result + "@Microsoft.com";
        }

        private string htmlDeCode(string inputs)
        {
            if (inputs.Length == 0)
                return string.Empty;

            string result = string.Empty;
            result = inputs.Replace("&namp;", "&");
            result = result.Replace("&nlt;", "<");
            result = result.Replace("&ngt;", ">");
            result = result.Replace("&nquot;","\"");

            return result;
        }
	}
}