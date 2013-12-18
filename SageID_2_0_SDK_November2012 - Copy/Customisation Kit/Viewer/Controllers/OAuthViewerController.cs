namespace WebSignOn.Application.Controllers
{
    using System.Web.Mvc;

    using WebSignOn.Application.OAuth;
    using WebSignOn.Application.ActionFilter;
    using Sage.WebSignOn.Application.ActionFilter;
    using Sage.WebSignOn.Application.Infrastructure;
    using System;

    [HandleError]
    [SetCulture]
    public class OAuthViewerController : SSOBaseController
    {
        /// <summary>
        /// 
        /// NOTICE TO DEVELOPERS
        /// 
        /// Change template name in GlobalSettings.cs file
        /// 
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult Authorise(string id)
        {
            AuthoriseModel model = new AuthoriseModel();
            model.WebApplicationDisplayName = "Web SSO OAuth Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;
            model.CaptchaRequired = true;
            model.AdditionalSecurityRequired = true;
            model.ResourceDisplayName = "Your Resource";
            model.WebApplicationDisplayName = "Your Client Name";
            model.EffectiveValidityPeriod = TimeSpan.FromDays(7);

            model.PermissionSetDescriptions = new string[] {
                "This is a sample permission",
                "This is also a sample permission"
            };

            model.CaptchaTheme = CaptchaThemes.clean.ToString();

            return View(model);
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult Authenticate(string id)
        {
            AuthenticateModel model = new AuthenticateModel();
            model.WebApplicationDisplayName = "Your Client Name";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;
            model.CaptchaRequired = true;
            model.CaptchaTheme = CaptchaThemes.clean.ToString();

            return View(model);
        }

        [HttpPost]
        [Templated(GlobalSettings.TemplateName)]
        public ActionResult Authorise(string id, string authoriseChoice)
        {
            ButtonValue submitValue = HtmlHelperExtensions.EvalAuthoriseButton(this, authoriseChoice);

            return Redirect("/?" + submitValue.ToString());
        }

        public ActionResult Cancel(string id)
        {
            return Redirect("/");
        }
    }
}
