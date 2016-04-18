using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BYSDN.Entity;
using BYSDN.Models;

namespace BYSDN.Controllers
{
    public class UserController : Controller
    {
        BYSDNEntities entities = new BYSDNEntities();
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registe()
        {
            ViewBag.Name =  User.Identity.Name;

            return View();
        }

        public ActionResult Upload(HttpPostedFileBase file)
        {
            string fileName = string.Empty;
            if (file != null && file.ContentLength > 0)
            {
                fileName = DateTime.Now.Ticks + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                file.SaveAs(path);
                return Json(new { success = true, serverFileName = fileName }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false },JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CropperImage(string url, float x, float y, float width, float height)
        {
            string fileName = DateTime.Now.Ticks + Path.GetExtension(Server.MapPath(url));
            Bitmap image = GetImageFromUrl(url);
            Bitmap newImage = CreateImage(image, (int)x, (int)y, (int)width, (int)height);
            image.Dispose();
            if (System.IO.File.Exists(Server.MapPath(url)))
            {                
                System.IO.File.Delete(Server.MapPath(url));
                newImage.Save(Path.Combine(Server.MapPath("~/Images/"), fileName));
            }
            return Json(new { success = true, fileName = fileName }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTimeLineData(int page)
        {
            List<TimeLineBodyModel> result = new List<TimeLineBodyModel>();

            Table_User user = (from a in entities.Table_User
                               where a.Name == Request.LogonUserIdentity.Name
                               select a).FirstOrDefault();

            if (user != null)
            {
                int start = (page - 1) * 4;
                var searchResult = (from a in entities.Table_OperationLog
                              where a.UserID == user.ID
                              orderby a.Date descending
                              select new
                              {
                                  Date = a.Date,
                                  Type = a.Table_LogEntity.LogType,
                                  OrginValue = a.Table_LogEntity.OriginValue,
                                  NewValue = a.Table_LogEntity.NewValue
                              }).Skip(start).Take(4).ToList();

                foreach (var obj in searchResult)
                {
                    Guid id = obj.Type == "New User" ? user.ID : new Guid(obj.NewValue);
                    TimeLineBodyModel body = getTimeLineBody(obj.Type, id);
                    if (body != null)
                    {
                        result.Add(body);
                    }
                }
            }

            return Json(new { success = true ,retData = result }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RegisteUser(string UserName, string Sex, string Comments,string imgPath)
        {
            Table_User user = new Table_User();
            user.ID = Guid.NewGuid();
            user.Name = UserName;
            user.Photo = imgPath;
            user.Rate = 0;
            user.Sexy = Sex == "Male" ? true : false;
            user.Comments = Comments;
            user.Date = DateTime.Now.ToUniversalTime();
            try
            {
                entities.Table_User.Add(user);
                entities.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }            
        }

        private TimeLineBodyModel getTimeLineBody(string type,Guid id)
        {
            TimeLineBodyModel result = null;

            switch(type)
            {
                case "New Reply":
                    var searchResult = (from a in entities.Table_Answer
                                        where a.ID == id
                                        select new
                                        {
                                            Date = a.Date,
                                            ID = a.QuestionID,
                                            Title = a.Table_Question.Tittle,
                                            Tags = a.Table_Question.Tags
                                        }).FirstOrDefault();
                    if(searchResult != null)
                    {
                        result = new TimeLineBodyModel();
                        result.Type = type;
                        result.BodyContent = searchResult.Title + " [" + searchResult.Tags + "]";
                        result.ID = searchResult.ID.ToString();
                        result.ShortDate = searchResult.Date.ToLocalTime().ToShortDateString();
                        result.ShortTime = searchResult.Date.ToLocalTime().ToShortTimeString();
                    }
                    break;
                case "New Question": 
                    var qsearchResult = (from a in entities.Table_Question
                                        where a.ID == id
                                        select new
                                        {
                                            Date = a.Date,
                                            ID = a.ID,
                                            Title = a.Tittle,
                                            Tags = a.Tags
                                        }).FirstOrDefault();
                    if(qsearchResult != null)
                    {
                        result = new TimeLineBodyModel();
                        result.Type = type;
                        result.BodyContent = qsearchResult.Title + " [" + qsearchResult.Tags + "]";
                        result.ID = qsearchResult.ID.ToString();
                        result.ShortDate = qsearchResult.Date.ToLocalTime().ToShortDateString();
                        result.ShortTime = qsearchResult.Date.ToLocalTime().ToShortTimeString();
                    }
                    break;
                case "New User":
                    var usearchResult = (from a in entities.Table_User
                                         where a.ID == id
                                         select new
                                         {
                                             Date = a.Date,
                                             ID = a.ID,
                                         }).FirstOrDefault();
                    if (usearchResult != null)
                    {
                        result = new TimeLineBodyModel();
                        result.Type = type;
                        result.BodyContent = "First time log in BYSDN.";
                        result.ShortDate = usearchResult.Date.ToLocalTime().ToShortDateString();
                        result.ShortTime = usearchResult.Date.ToLocalTime().ToShortTimeString();
                    }
                    break;                  
            }

            return result;
        }

        private Bitmap GetImageFromUrl(string url)
        {
            var path = Server.MapPath(url);
            if (System.IO.File.Exists(path))
            {
                return Bitmap.FromFile(path) as Bitmap;
            }
            return null;
        }

        private Bitmap CreateImage(Bitmap original, int x, int y, int width, int height)
        {
            var img = new Bitmap(width, height);
            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
            }
            return img;
        }
	}
}