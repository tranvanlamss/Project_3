using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using RechargeOnline.Models;

namespace Project_3.Controllers
{
    public class HomeController : Controller
    {
        private DataContext db = new DataContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact([Bind(Include = "in,name,email,subject,content")] feedback model)
        {
            if (ModelState.IsValid)
            {
                db.feedbacks.Add(model);
                db.SaveChanges();
                ViewBag.capt11 = "gửi phản hồi thành công";
                return View();
            }
            ViewBag.capt11 = "Không gửi được phản hồi";
            return View();
        }

        public string RandomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }

        public ActionResult Sendmail(int? idb, int? idp)
        {
            Email model = new Email();
            model.idb = (int)idb;
            model.idp = (int)idp;
            return View();
        }
        [HttpPost]
        public ActionResult Sendmail(Email model)
        {

            var capt = RandomDigits(10);
            using (MailMessage mm = new MailMessage(model.email, model.to))
            {
                mm.Subject = model.subject;
                mm.Body = capt;
                mm.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential cred = new NetworkCredential(model.email, model.password);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = cred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                    @ViewBag.Message = capt;
                }
            }
            HttpCookie cookie = new HttpCookie("Captcha");
            cookie.Values["captcha"] = capt;
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Check_Capt", "Home", new { ca = model.idb, pro = model.idp, pho = model.phone });
        }

        public ActionResult Check_Capt(int? ca, int? pro, int? pho)
        {

            HttpCookie cookie = Request.Cookies["Captcha"];

            if ((cookie.Values["captcha"] != null) && (cookie.Values["captcha"] != ""))
            {
                ViewBag.capt1 = cookie.Values["captcha"];
            }

            ViewBag.capt2 = ca;
            ViewBag.capt3 = pro;
            ViewBag.capt4 = pho;
            return View();
        }
        [HttpPost]
        public ActionResult Check_Capt(Captch model)
        {
            if (model.captcheck == null)
            {
                return RedirectToAction("Check_Capt", "Home", new { ca = model.idb, pro = model.idp, pho = model.phone });
            }
            if (model.captcha == model.captcheck)
            {
                return RedirectToAction("Create", "Oders", new { br = model.idb, pro = model.idp, pho = model.captcheck });
            }
            else
            {
                ViewBag.capt11 = "Ma xac thuc khong dung";
                return RedirectToAction("Check_Capt", "Home", new { ca = model.idb, pro = model.idp, pho = model.phone });
            }
        }
    }
}