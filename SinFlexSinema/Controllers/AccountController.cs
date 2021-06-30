using SinFlexSinema.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SinFlexSinema.Controllers
{
    public class AccountController : Controller
    {
        SinFlexSinemaEntities1 db = new SinFlexSinemaEntities1();
        // GET: Account
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(DB.User user)
        {
            if (ModelState.IsValid)
            {
                var user1 = new User
                {
                    Adi = user.Adi,
                    Soyadi = user.Soyadi,
                    Email = user.Email,
                    Sifre = user.Sifre,
                    KullaniciAdi = user.KullaniciAdi
            };
                    db.Users.Add(user1);
                    db.SaveChanges();
                    return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Yanlış Kullanıcı Adı veya Şifre");
                return View(user);
            }
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(DB.User user)
        {
            if(db.Users.Any(x=>x.KullaniciAdi == user.KullaniciAdi && x.Sifre == user.Sifre ))
            {
                var SessionUser = db.Users.FirstOrDefault(x => x.KullaniciAdi == user.KullaniciAdi && x.Sifre == user.Sifre);
                Session["LogonUser"] = SessionUser;
                Session["KullaniciAdi"] = user.KullaniciAdi;

                var permissionDescription = (from u in db.Users
                                             join ut in db.UserTypes on u.UserTypeId equals ut.Id
                                             where u.KullaniciAdi == user.KullaniciAdi && u.Sifre == user.Sifre
                                             select ut.Description).FirstOrDefault();

                Session["PermissionType"] = permissionDescription;

                return RedirectToAction("Profil", "Account");
            }
            else
            {
                ViewBag.Message = "Hata";
                return View();
            }
            
        }
        public ActionResult Logout()
        {
            Session["LogonUser"] = null;
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
        public ActionResult Profil(DB.User user)
        {
            if (Session["LogonUser"] == null)
            {
                return RedirectToAction("Anasayfa", "Film");
            }
            var user1 = db.Users.Where(model => model.User_ID == user.User_ID).SingleOrDefault();
               return View(user);
        }
        [AllowAnonymous]
        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult SifremiUnuttum(string email)
        {
            var member = db.Users.FirstOrDefault(x => x.Email == email);
            if (member == null)
            {
                ViewBag.MyError = "Böyle bir hesap bulunamadı.";
                return View();
            }
            else
            {
                var body = "Şifreniz:" + member.Sifre;
                MyMail mail = new MyMail(member.Email,"Şifremi Unuttum",body);
                mail.SendMail();
                TempData["Info"] = email + "mail adresinize şifreniz gönderilmiştir.";
                return RedirectToAction("Login");

            }
            
    }
    }
}