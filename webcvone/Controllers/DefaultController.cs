using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webcvone.Models.entitty;

namespace webcvone.Controllers
{
    public class DefaultController : Controller
    {
        private webcvrepoEntities db = new webcvrepoEntities();

        [HttpGet]
        public ActionResult Index()
        {
            // Mevcut verileri çekme işlemleri
            var aciklamalar = db.aciklamalar.ToList();
            ViewBag.KariyerVerileri = db.detay_profil.OrderByDescending(x => x.tarih).ToList();
            ViewBag.SertifikaVerileri = db.tbl_sertifika.ToList();
            ViewBag.EgitimVerileri = db.tbl_egitim.ToList();
            ViewBag.WorkFlowVerileri = db.work_flow.ToList();
            ViewBag.IletisimVerileri = db.Iletisim.ToList();

            return View(aciklamalar);
        }

        [HttpPost]
        public ActionResult Index(Iletisim p)
        {
            try
            {
                db.Iletisim.Add(p);
                db.SaveChanges();
                return Json(new { success = true }); // AJAX'a başarı dön
            }
            catch
            {
                return Json(new { success = false }); // AJAX'a hata dön
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}