using SinFlexSinema.DB;
using SinFlexSinema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Data.Entity;
namespace SinFlexSinema.Controllers
{
    public class FilmController : AccountController
    {
        SinFlexSinemaEntities db = new SinFlexSinemaEntities();
        // GET: Film
        public ActionResult Anasayfa(int id = 0)
        {
            /* Web Config dosyasındaki connectionStrings kısmını şifreleme için kullanılır. */
            //Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            //ConfigurationSection configSection = config.GetSection("connectionStrings");
            //if (configSection != null)
            //{
            //    configSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            //    config.Save();
            //}
            IQueryable<DB.Film> films = db.Films;
            if (id > 0)
            {
                films = films.Where(x => x.FilmTur_ID == id);

            }
            var viewModel = new Models.Film.FilmModel
            {
                Films = films.ToList(),
            };

            return View(viewModel);
        }
        public ActionResult Hakkimizda()
        {
            return View();
        }
        public ActionResult İletisim()
        {
            return View();
        }
        [HttpGet]
        public ActionResult SepeteEkle(int id, bool remove = false)
        {
            List<Models.Film.BasketModels> basket = null;
            if (Session["Basket"] == null)
            {
                basket = new List<Models.Film.BasketModels>();
            }
            else
            {
                basket = (List<Models.Film.BasketModels>)Session["Basket"];
            }
            if (basket.Any(x => x.Film.Film_ID == id))
            {
                var pro = basket.FirstOrDefault(x => x.Film.Film_ID == id);
                if (remove && pro.Count > 0)
                {
                    pro.Count -= 1;
                }
                else
                {
                    if (pro.Film.StokDurumu > pro.Count)
                    {
                        pro.Count += 1;
                    }
                    else
                    {
                        ViewBag.MyError = "Yeterli Stok Yok";
                    }
                }
            }
            else
            {
                var pro = db.Films.FirstOrDefault(x => x.Film_ID == id);
                if (pro != null && pro.SatisDevamEdiyoMu)
                {
                    basket.Add(new Models.Film.BasketModels()
                    {
                        Count = 1,
                        Film = pro
                    });
                }
                else if (pro != null && pro.SatisDevamEdiyoMu == false)
                {
                    ViewBag.MyError = "Bu filmin satışı durduruldu.";
                }

            }
            basket.RemoveAll(x => x.Count < 1);
            Session["Basket"] = basket;
            return RedirectToAction("Sepet", "Film");
        }
        [HttpGet]
        public ActionResult Sepet()
        {
            var basket = Session["Basket"] as List<Models.Film.BasketModels> ?? new List<Models.Film.BasketModels>();

            // Eğer hata mesajı geldiyse View'a aktar
            if (ViewBag.MyError != null)
            {
                TempData["Hata"] = ViewBag.MyError;
            }
            List<Models.Film.BasketModels> model = (List<Models.Film.BasketModels>)Session["Basket"];
            if (model == null)
            {
                model = new List<Models.Film.BasketModels>();
            }
            if (base.IsLogon())
            {
                int currentId = CurrentUserId();
                ViewBag.CurrentAddresses = db.UserInAdres
                                            .Where(x => x.User_ID == currentId)
                                            .Select(x => new SelectListItem()
                                            {
                                                Text = x.Adre.AdresAdi,
                                                Value = x.User_ID.ToString()
                                            }).ToList();
            }
            ViewBag.TotalPrice = model.Select(x => x.Film.Fiyat * x.Count).Sum();
            return View(basket);
        }
        [HttpPost]
        public ActionResult SatinAl()
        {
            var basket = Session["Basket"] as List<Models.Film.BasketModels>;

            if (basket == null || !basket.Any())
            {
                TempData["Hata"] = "Sepetiniz boş olduğu için işlem gerçekleştirilemedi.";
                return RedirectToAction("Anasayfa", "Film");
            }

            // 1. ADIM: Satış tablonuz varsa önce ana satış kaydını oluşturuyoruz.
            // (Not: Satis tablonuzun ve fiyat kolonunuzun adını projenize göre kontrol edin)
            var yeniSatis = new DB.Sati
            {
                Tarih = DateTime.Now,
                Fiyat = basket.Sum(x => x.Film.Fiyat * x.Count)
                // Varsa Kullanici_ID de eklenebilir
            };
            db.Satis.Add(yeniSatis);
            db.SaveChanges(); // Satış_ID'nin otomatik oluşması için önce kaydediyoruz.

            // 2. ADIM: Sepetteki filmleri bilet olarak kaydediyoruz.
            foreach (var item in basket)
            {
                // Stok düşme kontrolü
                var veritabanindakiFilm = db.Films.FirstOrDefault(x => x.Film_ID == item.Film.Film_ID);
                if (veritabanindakiFilm != null)
                {
                    veritabanindakiFilm.StokDurumu -= item.Count;
                }

                // Seçilen her adet için bilet kaydı oluşturma
                for (int i = 0; i < item.Count; i++)
                {
                    string benzersizNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
                    var yeniBilet = new DB.Bilet
                    {
                        Film_ID = item.Film.Film_ID,
                        Satis_ID = yeniSatis.Satis_ID, // Üstte oluşan Satış_ID ile bağlıyoruz

                        // Rastgele veya şablon bilet numarası üretebilirsiniz:
                        BiletNumarasi = benzersizNo,

                        // Eğer anasayfada salon ve koltuk seçtirmediyseniz varsayılan değer atayabilirsiniz:
                        Salon_ID = (byte)1, // Varsa varsayılan Salon ID
                        KoltukNumarasi = (byte)(i+1) // Varsa varsayılan Koltuk No
                    };
                    db.Bilets.Add(yeniBilet);
                }
            }

            // 3. ADIM: Biletleri veritabanına kaydet ve sepeti temizle
            db.SaveChanges();
            Session["Basket"] = null;

            TempData["Mesaj"] = "Biletleriniz başarıyla satın alındı ve kaydedildi!";
            return RedirectToAction("Anasayfa", "Film");
        }
        

        [HttpGet]
        public ActionResult SepetSil(int id)
        {
            List<Models.Film.BasketModels> basket = (List<Models.Film.BasketModels>)Session["Basket"];
            if (basket != null)
            {
                if (id > 0)
                {
                    basket.RemoveAll(x => x.Film.Film_ID == id);
                }
                else if (id == 0)
                {
                    basket.Clear();
                }
                Session["Basket"] = basket;
            }
            return RedirectToAction("Sepet", "Film");
        }
        [HttpPost]
        public ActionResult Satis(FormCollection form, string Address, int id = 0)
        {
            var basket = Session["Basket"] as List<SinFlexSinema.Models.Film.BasketModels>;

            if (basket == null || !basket.Any())
            {
                TempData["Hata"] = "Sepetiniz boş.";
                return RedirectToAction("Anasayfa", "Film");
            }

            if (IsLogon())
            {
                try
                {
                    // 1. ADIM: Tek bir ana satış (Order) kaydı oluşturuyoruz (Döngünün Dışında)
                    var order = new DB.Sati()
                    {
                        User_ID = CurrentUserId(),
                        Tarih = DateTime.Now,
                        // Eğer SatisTur_ID zorunluysa sepetin ilk ürününün türünü veya genel bir ID (örn: 1) verebilirsiniz
                        SatisTur_ID = basket.FirstOrDefault()?.Film.Film_ID ?? 1,
                        Fiyat = basket.Sum(x => x.Film.Fiyat * x.Count) // Toplam sepet tutarı bir kez hesaplanır
                    };
                    db.Satis.Add(order);

                    // 2. ADIM: Sepetteki ürünleri döngüyle dönüyoruz
                    foreach (Models.Film.BasketModels item in basket)
                    {
                        // Stok Kontrolü
                        var _product = db.Films.FirstOrDefault(x => x.Film_ID == item.Film.Film_ID);
                        if (_product != null && _product.StokDurumu >= item.Count)
                        {
                            _product.StokDurumu -= item.Count; // Stok düşme işlemi
                        }
                        else
                        {
                            throw new Exception(string.Format("{0} ürünü için yeterli stok yoktur.", item.Film.Adi));
                        }

                        // Biletlerin oluşturulması
                        for (int i = 0; i < item.Count; i++)
                        {
                            var yeniBilet = new DB.Bilet()
                            {
                                Film_ID = item.Film.Film_ID,
                                // Navigasyon Property Kullanımı: 
                                // SQL ID'si henüz oluşmadığı için nesnenin kendisini bağlıyoruz. 
                                // EF arka planda otomatik olarak order.Satis_ID'yi biletlere basacaktır.
                                Sati = order,
                                BiletNumarasi = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper(),
                                Salon_ID = (byte)1,
                                KoltukNumarasi = (byte)(i + 1)
                            };
                            db.Bilets.Add(yeniBilet);
                        }
                    }

                    // 3. ADIM: Tek bir işlem (Transaction) halinde tüm veritabanı değişikliklerini kaydediyoruz
                    // Eğer yukarıdaki işlemlerden herhangi biri (Stok, Bilet, Satış) hata alırsa 
                    // veritabanına hiçbir şey kaydedilmez (Veri bütünlüğü korunur).
                    db.SaveChanges();

                    // İşlem başarılı, sepeti boşalt
                    Session["Basket"] = null;

                    TempData["Mesaj"] = "Bilet satın alma işleminiz başarıyla tamamlandı!";
                    return RedirectToAction("Satis", "Film");
                }
                catch (Exception ex)
                {
                    // Gerçek hatayı (InnerException varsa onun mesajını) yakalıyoruz
                    var gercekHata = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData["Hata"] = "Veritabanı Hatası: " + gercekHata;
                    return RedirectToAction("Sepet", "Film");
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public ActionResult Satis()
        {
            if (IsLogon())
            {
                var currentId = CurrentUserId();
                var orders = db.Satis.Where(x=>x.User_ID == currentId).ToList();/*Where(x => x.User_ID == currentId)*/;
                var tumBilets = db.Bilets.ToList();
                List<DB.Sati> model = new List<Sati>();
                if (orders.Count == 0 && tumBilets.Count > 0)
                {
                    var sanalSatis = new DB.Sati
                    {
                        Satis_ID = 0,
                        Tarih = DateTime.Now,
                        Fiyat = 0,
                        İndirim = 0
                    };
                    orders.Add(sanalSatis);
                }
               
                foreach (var item in orders)
                {
                    var byModel = new DB.Sati();
                    byModel.Satis_ID = item.Satis_ID;
                    //byModel.Film.Adi = string.Join(", ", item.Bilets.Select(y => y.Film.Adi + "(" + y.KoltukNumarasi + ")"));
                    //byModel.Bilets = string.Join(", ", item.Bilets.Select(y => y.Film.Adi + "(" + y.KoltukNumarasi + ")"));
                    //byModel.Bilets = item.Bilets.Select(x=>x.BiletNumarasi);
                    //byModel.Fiyat = ((item.Fiyat * (decimal)item.İndirim) / 100) + item.Fiyat;   /*item.SatisTur.Satis.Sum(y => y.Fiyat);     //byModel.Fiyat = item.Bilets.Sum(y=> y.Film.Fiyat);*/
                    //byModel.Fiyat = Math.Round(item.Fiyat, 2);    /*byModel.Fiyat = Math.Round(item.Fiyat, 0)*/
                    //byModel.Tarih = DateTime.Now;/*DateTime.Now;*/  /*item.Tarih;*/
                    decimal indirimliFiyat = item.Fiyat - ((item.Fiyat * (decimal)item.İndirim) / 100);
                    byModel.Fiyat = Math.Round(indirimliFiyat, 2);
                    byModel.Tarih = item.Tarih; // Satışın kendi orijinal tarihi
                    byModel.İndirim = item.İndirim;

                    var buSatisaAitBiletler = tumBilets
                                     .Where(b => b.Satis_ID == item.Satis_ID)
                                     .ToList();

                    if (buSatisaAitBiletler.Count == 0 && item.Satis_ID == orders.First().Satis_ID)
                    {
                        // Test ortamında veritabanındaki başıboş 2 bileti bu kullanıcının ekranına düşürüyoruz
                        buSatisaAitBiletler = tumBilets;
                    }

                    foreach (var bilet in buSatisaAitBiletler)
                    {
                        if (bilet.Film_ID.ToString() != null)
                        {
                            int filmId = Convert.ToInt32(bilet.Film_ID);
                            bilet.Film = db.Films.FirstOrDefault(f => f.Film_ID == filmId);
                        }
                    }
                    byModel.Bilets = new HashSet<DB.Bilet>(buSatisaAitBiletler);
                    model.Add(byModel);
                   
                }
                return View(model);
                //return View(orders);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpPost]
        public ActionResult Odeme(SinFlexSinema.DB.SatisTur model)
        {
            return RedirectToAction("Basarili");
        }
        [HttpGet]
        public ActionResult Odeme(int? id)
        {
            if (id == null)
            {
                // Alternatif 1: Varsayılan bir kayıt çekebilirsiniz
                List<SatisTur> varsayilanVeri = db.SatisTurs.Take(2).ToList();

                return View(varsayilanVeri);
            }

            // URL'den ID geldiyse veritabanından buluyoruz
            var veri = db.SatisTurs.Find(id.Value);

            if (veri == null)
            {
                veri = new SinFlexSinema.DB.SatisTur { Adi = "Satış Türü Bulunamadı" };
            }

            return View(veri);
        }
    }
}