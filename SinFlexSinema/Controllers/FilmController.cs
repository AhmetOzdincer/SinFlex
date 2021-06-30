using SinFlexSinema.DB;
using SinFlexSinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinFlexSinema.Controllers
{
    public class FilmController : Controller
    {
        SinFlexSinemaEntities1 db = new SinFlexSinemaEntities1();
        // GET: Film
        public ActionResult Anasayfa(int id=0)
        {
            IQueryable<DB.Film> films = db.Films;
            if (id>0)
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
        public ActionResult SepeteEkle(int id,bool remove = false)
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
            if (basket.Any(x=>x.Film.Film_ID == id))
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
                else if(pro != null && pro.SatisDevamEdiyoMu == false)
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
            List<Models.Film.BasketModels> model = (List<Models.Film.BasketModels>)Session["Basket"];
            if(model==null)
            {
                model = new List<Models.Film.BasketModels>();
            }
           
            ViewBag.TotalPrice = model.Select(x => x.Film.Fiyat * x.Count).Sum();
            return View(model);
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
    }
}