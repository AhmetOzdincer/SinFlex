using SinFlexSinema.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace SinFlexSinema.Controllers
{
    public class AccountController : Controller
    {

        SinFlexSinemaEntities db = new SinFlexSinemaEntities();
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
                //if (SessionUser != null)
                //{
                //    FormsAuthentication.SetAuthCookie(SessionUser.KullaniciAdi, user.BeniHatirla);

                //}
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
            /*Burası Web Config dosyasındaki connectionStrings kısmını şifrelendikten sonra çözmeye yarar. */
            //{
            //    Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            //    ConfigurationSection configSection = config.GetSection("connectionStrings");
            //    if (configSection.SectionInformation.IsProtected)
            //    {
            //        configSection.SectionInformation.UnprotectSection();
            //        config.Save();
            //    }

            //}
            Session["LogonUser"] = null;
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            public ActionResult Profil(/*DB.User user*/ int id =0)
            {
                if (id == 0)
                {
                    id = CurrentUserId();
                }
                var user1 = db.Users.FirstOrDefault(x => x.User_ID == id);
                if (user1 == null)
                {
                    return RedirectToAction("Anasayfa", "Film");
                }
                User model = new User
                {
                    User_ID = user1.User_ID,
                    UserType = user1.UserType,
                    KullaniciAdi = user1.KullaniciAdi,
                    Yas = user1.Yas,
                    Cinsiyet = user1.Cinsiyet,
                    Meslek = user1.Meslek,
                    Email = user1.Email,
                    Adi = user1.Adi,
                    Soyadi = user1.Soyadi,
                    DogumTarihi = user1.DogumTarihi,
                    TcKimlikNo = user1.TcKimlikNo,
                    KayitTarihi = user1.KayitTarihi
                };

                   return View(model);
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
                    TempData["Info"] = email + " mail adresinize şifreniz gönderilmiştir.";
                    return RedirectToAction("Login");

                }

        }
            protected DB.User CurentUser()
            {
                if (Session["LogonUser"] == null) return null;
                return (DB.User)Session["LogonUser"];
            }
            protected int CurrentUserId()
            {
                if (Session["LogonUser"] == null) return 0;
                return ((DB.User)Session["LogonUser"]).User_ID;
            }
            protected bool IsLogon()
            {
                if(Session["LogonUser"] == null)
                    return false;
                else
                    return true;
            }
            //[HttpPost]
            //public ActionResult Address(DB.Adre address)
            //{
            //    DB.Adre _address = null;
            //    if (address.Adres_ID.ToString() == Guid.Empty.ToString())
            //    {
            //        address.Adres_ID = Guid.NewGuid().GetHashCode();
            //        address.Adres_ID = CurrentUserId(); /* address.UserInAdres.Select(x=>x.User_ID == CurrentUserId());*/     /*address.UserInAdres.Where(x=>x.User_ID == CurrentUserId);*/
                                                                                                                                //        db.Adres.Add(address);
                                                                                                                                //    }
                                                                                                                                //    else
                                                                                                                                //    {
                                                                                                                                //        _address = db.Adres.FirstOrDefault(x => x.Adres_ID == address.Adres_ID);
                                                                                                                                //        _address.AdresAdi = address.AdresAdi;

                //    }
                //    db.SaveChanges();
                //    return RedirectToAction("Profil", "Account");
                //}
            }

        }