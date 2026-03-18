using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webcvone.Models.entitty; // Model yolunun doğruluğundan emin ol

namespace webcvone.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private webcvrepoEntities db = new webcvrepoEntities();

        public ActionResult AdminP()
        {
            // Veritabanındaki toplam sayıları çekiyoruz
            ViewBag.v1 = db.work_flow.Count(); // Toplam Proje Sayısı
            ViewBag.v2 = db.Iletisim.Count();  // Toplam Mesaj Sayısı
            ViewBag.v3 = db.tbl_sertifika.Count(); // Toplam Sertifika Sayısı
            ViewBag.v4 = db.detay_profil.Count(); // Toplam Deneyim Sayısı

            // Opsiyonel: Sadece "Devam Eden" projelerin sayısını çekmek istersen:
            // ViewBag.v5 = db.work_flow.Where(x => x.Hazır == 2).Count();

            return View();
        }

        // --- EĞİTİM BÖLÜMÜ ---

        // 1. Eğitimleri Listeleme Sayfası
        public ActionResult EgitimListesi()
        {
            var degerler = db.tbl_egitim.ToList();
            return View(degerler);
        }

        // 2. Yeni Eğitim Ekleme Sayfası (Sayfayı İlk Açan Kısım)
        [HttpGet]
        public ActionResult EgitimEkle()
        {
            return View();
        }

        // 3. Yeni Eğitim Ekleme İşlemi (Butona Basınca Çalışan Kısım)
        [HttpPost]
        public ActionResult EgitimEkle(tbl_egitim p)
        {
            if (ModelState.IsValid)
            {
                db.tbl_egitim.Add(p);
                db.SaveChanges(); // Veritabanına asıl kaydı bu yapar
                return RedirectToAction("EgitimListesi"); // Kayıt sonrası listeye dön
            }
            return View(p);
        }
        [HttpGet]
        public ActionResult EgitimGetir(int id)
        {
            // ID'ye göre ilgili eğitim verisini buluyoruz
            var egitim = db.tbl_egitim.Find(id);
            return View(egitim);
        }

        [HttpPost]
        public ActionResult EgitimGuncelle(tbl_egitim p)
        {
            // Veritabanındaki eski veriyi bulup yenisiyle değiştiriyoruz
            var deger = db.tbl_egitim.Find(p.ID);
            deger.OkulAd = p.OkulAd;
            deger.Bolum = p.Bolum;
            deger.GPA = p.GPA;
            deger.EgitimDetay = p.EgitimDetay;
            deger.MezuniyetYili = p.MezuniyetYili;

            db.SaveChanges();
            return RedirectToAction("EgitimListesi");
        }

        // 4. Eğitim Silme İşlemi
        public ActionResult EgitimSil(int id)
        {
            var egitim = db.tbl_egitim.Find(id);
            if (egitim != null)
            {
                db.tbl_egitim.Remove(egitim);
                db.SaveChanges();
            }
            return RedirectToAction("EgitimListesi");
        }
        // --- SERTİFİKA BÖLÜMÜ ---

        public ActionResult SertifikaListesi()
        {
            var degerler = db.tbl_sertifika.ToList();
            return View(degerler);
        }

        [HttpGet]
        public ActionResult SertifikaEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SertifikaEkle(tbl_sertifika p)
        {
            db.tbl_sertifika.Add(p);
            db.SaveChanges();
            return RedirectToAction("SertifikaListesi");
        }

        public ActionResult SertifikaSil(int id)
        {
            var deger = db.tbl_sertifika.Find(id);
            db.tbl_sertifika.Remove(deger);
            db.SaveChanges();
            return RedirectToAction("SertifikaListesi");
        }

        [HttpGet]
        public ActionResult SertifikaGetir(int? id)
        {
            if (id == null) return RedirectToAction("SertifikaListesi");
            var sertifika = db.tbl_sertifika.Find(id);
            return View(sertifika);
        }

        [HttpPost]
        public ActionResult SertifikaGuncelle(tbl_sertifika p)
        {
            var deger = db.tbl_sertifika.Find(p.ID);
            deger.SertifikaAd = p.SertifikaAd;
            deger.Kurum = p.Kurum;
            deger.Tarih = p.Tarih;
            deger.SertifikaLink = p.SertifikaLink;
            deger.Ikon = p.Ikon;
            db.SaveChanges();
            return RedirectToAction("SertifikaListesi");
        }
        // --- KARİYER (DENEYİM) BÖLÜMÜ ---

        public ActionResult KariyerListesi()
        {
            var veriler = db.detay_profil.ToList();
            return View(veriler);
        }

        [HttpGet]
        public ActionResult KariyerEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult KariyerEkle(detay_profil p)
        {
            db.detay_profil.Add(p);
            db.SaveChanges();
            return RedirectToAction("KariyerListesi");
        }

        public ActionResult KariyerSil(int id)
        {
            var deger = db.detay_profil.Find(id);
            db.detay_profil.Remove(deger);
            db.SaveChanges();
            return RedirectToAction("KariyerListesi");
        }

        [HttpGet]
        public ActionResult KariyerGetir(int id)
        {
            var kariyer = db.detay_profil.Find(id);
            return View(kariyer);
        }

        [HttpPost]
        public ActionResult KariyerGuncelle(detay_profil p)
        {
            var deger = db.detay_profil.Find(p.ıd);
            deger.Sırket = p.Sırket;
            deger.Pozisyon = p.Pozisyon;
            deger.tarih = p.tarih;
            deger.deneyim_notu = p.deneyim_notu;

            db.SaveChanges();
            return RedirectToAction("KariyerListesi");
        }
        // --- TEKNİK PROJELER (WORK FLOW) ---

        public ActionResult ProjeListesi()
        {
            var projeler = db.work_flow.ToList();
            return View(projeler);
        }

        [HttpGet]
        public ActionResult ProjeGetir(int id)
        {
            // Küçük 'id' parametresi hata almanı engeller
            var proje = db.work_flow.Find(id);
            return View(proje);
        }

        [HttpPost]
        public ActionResult ProjeGuncelle(work_flow p)
        {
            // Veritabanındaki Id kolonun küçük harf 'Id' ise p.Id kullanmalısın
            var deger = db.work_flow.Find(p.Id);

            deger.ProjeAd = p.ProjeAd;
            deger.Category = p.Category;
            deger.Technology = p.Technology;
            deger.Hazır = p.Hazır; // Checkbox veya string durumuna göre güncellenir

            db.SaveChanges();
            return RedirectToAction("ProjeListesi");
        }
        // --- GELEN MESAJLAR BÖLÜMÜ ---

        public ActionResult Mesajlar()
        {
            var mesajlar = db.Iletisim.ToList();
            return View(mesajlar);
        }

        public ActionResult MesajSil(int id)
        {
            var mesaj = db.Iletisim.Find(id);
            if (mesaj != null)
            {
                db.Iletisim.Remove(mesaj);
                db.SaveChanges();
            }
            return RedirectToAction("Mesajlar");
        }

        public ActionResult MesajDetay(int id)
        {
            var mesaj = db.Iletisim.Find(id);
            return View(mesaj);
        }
    }
}