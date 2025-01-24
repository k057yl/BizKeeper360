using BizKeeper360.Servises;
using Microsoft.AspNetCore.Mvc;

namespace BizKeeper360.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly CaptchaService _captchaService;

        public CaptchaController(CaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        [HttpGet]
        public IActionResult GetCaptcha()
        {
            string captchaText = _captchaService.GenerateCaptchaText();
            HttpContext.Session.SetString("Captcha", captchaText);

            byte[] captchaImage = _captchaService.GenerateCaptchaImage(captchaText);
            return File(captchaImage, "image/png");
        }

        [HttpPost]
        public JsonResult ValidateCaptcha(string captcha)
        {
            string correctCaptcha = HttpContext.Session.GetString("Captcha");

            bool isValid = captcha == correctCaptcha;
            return Json(new { isValid });
        }
    }
}
