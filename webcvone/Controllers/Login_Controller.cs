using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using webcvone.Models.entitty;

namespace webcvone.Controllers
{
    public class LoginController : Controller
    {
        webcvrepoEntities db = new webcvrepoEntities();

        // 1. Giriş Sayfasını Görüntüler
        [HttpGet]
        public ActionResult Loginhome()
        {
            return View();
        }

        // 2. Giriş Bilgilerini Kontrol Eder
        [HttpPost]
        public ActionResult Loginhome(string Admeyn, string Log_keytrue)
        {
            // Sayısal dönüşüm kontrolü (int kullandığımız için)
            int key;
            bool isValidKey = int.TryParse(Log_keytrue, out key);

            if (!isValidKey)
            {
                ViewBag.Hata = "Şifre sadece rakamlardan oluşmalıdır!";
                return View();
            }

            // Veritabanı eşleşme kontrolü
            var bilgiler = db.Admin_log.FirstOrDefault(x => x.Admeyn == Admeyn && x.Log_keytrue == key);

            if (bilgiler != null)
            {
                // Kimlik Doğrulama Çerezi (Authorize etiketinin çalışması için şart)
                FormsAuthentication.SetAuthCookie(bilgiler.Admeyn, false);

                // Giriş yapan ismi her yerde kullanabilmek için Session'a atıyoruz
                Session["Kullanici"] = bilgiler.Admeyn;

                // AdminController içindeki AdminP metoduna yönlendir
                return RedirectToAction("AdminP", "Admin");
            }
            else
            {
                ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!";
                return View();
            }
        }

        // 3. Güvenli Çıkış İşlemi
        // 3. Güvenli Çıkış İşlemi
        [Authorize] // Sadece giriş yapmış olanlar çıkış yapabilsin
        public ActionResult LogOut()
        {
            // Kimlik doğrulama çerezini (cookie) kaldırır
            FormsAuthentication.SignOut();

            // Sunucu tarafındaki oturum verilerini (Session) tamamen sıfırlar
            Session.Abandon();

            // Kullanıcıyı ziyaretçilerin gördüğü ana sayfaya (Index.cshtml) gönderir
            // Controller: Default, Action:
            return RedirectToAction("Index", "Default");
        }
    }
}