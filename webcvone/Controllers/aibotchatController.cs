using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using webcvone.Models.entitty;

namespace webcvone.Controllers
{
    public class aibotchatController : Controller
    {
        private readonly webcvrepoEntities db = new webcvrepoEntities();
        private readonly string _apiKey = "AIzaSyDJg02ue_1hx1z_QI7b7QKbIn240uVsLEA".Trim();

        [HttpPost]
        public async Task<JsonResult> AskAI(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { success = false, reply = "Mesaj boş olamaz." });

            try
            {
                var bio = db.aciklamalar.FirstOrDefault();
                var egitimList = db.tbl_egitim.ToList();
                var yetenekList = db.work_flow.ToList();
                var deneyimList = db.detay_profil.ToList();

                string egitim = egitimList.Any() ? string.Join(" | ", egitimList.Select(x => x.OkulAd + " " + x.Bolum)) : "Bilgi yok";
                string yetenek = yetenekList.Any() ? string.Join(", ", yetenekList.Select(x => x.Technology)) : "C#, SQL";
                string deneyim = deneyimList.Any() ? string.Join(" | ", deneyimList.Select(x => x.Sırket + " " + x.Pozisyon)) : "Deneyim bilgisi yok";

                string systemPrompt = $"Sen Onur'un asistanısın. İsim: {bio?.ad} {bio?.soyad}. Eğitim: {egitim}. Yetenekler: {yetenek}. Deneyim: {deneyim}. Kısa cevap ver.";

                using (var client = new HttpClient())
                {
                    // KRİTİK NOKTA: "models/" takısını ekledik!
                    var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
                    var requestBody = new
                    {
                        contents = new[] {
                            new { parts = new[] { new { text = systemPrompt + "\n\nSoru: " + message } } }
                        }
                    };

                    var jsonRequest = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(requestUrl, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, reply = $"API Hatası ({response.StatusCode}): {responseString}" });
                    }

                    dynamic result = JsonConvert.DeserializeObject(responseString);

                    if (result?.candidates != null && result.candidates.Count > 0)
                    {
                        string aiResponse = result.candidates[0].content.parts[0].text;
                        return Json(new { success = true, reply = aiResponse });
                    }
                    else
                    {
                        return Json(new { success = false, reply = "Cevap üretilemedi, lütfen tekrar deneyin." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, reply = "Sistem Hatası: " + ex.Message });
            }
        }
    }
}