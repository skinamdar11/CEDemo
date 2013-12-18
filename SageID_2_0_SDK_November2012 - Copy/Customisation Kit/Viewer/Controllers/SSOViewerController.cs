namespace WebSignOn.Application.Controllers
{
    using System.Web.Mvc;

    using WebSignOn.Application.ActionFilter;
    using WebSignOn.Application;
    using Sage.WebSignOn.Application.ActionFilter;
    using Sage.WebSignOn.Application.Infrastructure;
    using System;
    using System.Collections.Generic;

    [HandleError]
    [SetCulture]
    public class SSOViewerController : SSOBaseController
    {
        /// <summary>
        /// 
        /// NOTICE TO DEVELOPERS
        /// 
        /// Change template name in GlobalSettings.cs file
        /// 
        /// </summary>
        private const string DemoUser = "ssoviewer@sage.com";
        private const string DemoDisplayName = "Demonstration User";

        public ActionResult Index()
        {
            return View();
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult SignOn(string id)
        {
            SignOnModel model = new SignOnModel();
            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;
            model.CaptchaRequired = true;
            model.CaptchaTheme = CaptchaThemes.clean.ToString();

            return View(model);
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult SignOnAdditional(string id)
        {
            SignOnModel model = new SignOnModel();
            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignedInApplications = new string[] { "Web Application A", "Web Application B" };
            model.SignOnAttemptId = id;
            model.User = DemoUser;
            model.DisplayName = DemoDisplayName;
            model.CancelAllowed = true;
            model.CaptchaRequired = true;
            model.CaptchaTheme = CaptchaThemes.clean.ToString();

            return View(model);
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult SignOnExpress(string id)
        {
            SignOnModel model = new SignOnModel();
            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignedInApplications = new string[] { "Web Application A", "Web Application B" };
            model.SignOnAttemptId = id;
            model.User = DemoUser;
            model.DisplayName = DemoDisplayName;
            model.CancelAllowed = true;
            model.AutoSignOnAllowed = true;

            return View(model);
        }

        public ActionResult Cancel(string id)
        {
            return Redirect("/");
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult Register(string id)
        {
            RegisterModel model = new RegisterModel();
            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;
            model.CaptchaRequired = true;
            model.CaptchaTheme = CaptchaThemes.clean.ToString();
            model.SecurityQuestions = SecurityQuestions.Items;

            return View(model);
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult RegisterCustomCaptchaDemo(string id)
        {
            RegisterModel model = new RegisterModel();
            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;
            model.CaptchaRequired = true;
            model.CaptchaTheme = CaptchaThemes.custom.ToString();
            model.SecurityQuestions = SecurityQuestions.Items;

            return View(model);
        }

        public ActionResult CancelRegistration(string id)
        {
            return Redirect("/");
        }


        [Templated(GlobalSettings.TemplateName)]
        public ActionResult RecoveryStage1(string id)
        {
            RecoveryModel model = new RecoveryModel(WebSSOMode.PasswordRecoveryStage1);
            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;
            model.CaptchaRequired = true;
            model.CaptchaTheme = CaptchaThemes.clean.ToString();

            return View(model);
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult RecoveryStage2(string id)
        {
            RecoveryModel model = new RecoveryModel(WebSSOMode.PasswordRecoveryStage2);
            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;

            model.RecoveryQuestion1 = "This would be the first security question?";
            model.RecoveryQuestion2 = "This would be the second security question?";

            return View(model);
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult RecoveryStage3(string id)
        {
            RecoveryModel model = new RecoveryModel(WebSSOMode.PasswordRecoveryStage3);
            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;

            return View(model);
        }

        public ActionResult CancelRecovery(string id)
        {
            return Redirect("/");
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult Change(string id)
        {

            // Change password and security questions
            ChangeModel model = new ChangeModel(false);

            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;
            model.CaptchaRequired = false;

            model.UserSecurityQuestions1 = SecurityQuestions.Items;
            model.UserSecurityQuestions2 = SecurityQuestions.Items;
            model.UserSecurityQuestions3 = SecurityQuestions.Items;

            model.SecurityAnswer1 = "●●●●●●●●";
            model.SecurityAnswer2 = "●●●●●●●●";
            model.SecurityAnswer3 = "●●●●●●●●";

            return View(model);
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult ChangePasswordOnly(string id)
        {
            // Change password only
            ChangeModel model = new ChangeModel(true);

            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;
            model.CaptchaRequired = false;

            return View(model);
        }

        [Templated(GlobalSettings.TemplateName)]
        public ActionResult ManageAuthorisations(string id)
        {
            // Change password only
            ManageAuthorisationModel model = new ManageAuthorisationModel();

            model.WebApplicationDisplayName = "Web SSO Viewer";
            model.SignOnAttemptId = id;
            model.CancelAllowed = true;
            model.CaptchaRequired = false;

            model.Authorisations = new ManageAuthorisationModel.UserAuthorizations()
            {
                ResourceAuthorizations = new List<ManageAuthorisationModel.ResourceAuthorization>{
                    new ManageAuthorisationModel.ResourceAuthorization()
                    { 
                        ResourceApplicationNames = new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Sage CRM" } },
                        ClientAuthorizations = new List<ManageAuthorisationModel.ClientAuthorization>
                        {
                            new ManageAuthorisationModel.ClientAuthorization()
                            { 
                                ClientApplicationNames = new List<ManageAuthorisationModel.CultureSpecificValue> {new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Sage 100" }},
                                ClientType = ManageAuthorisationModel.ClientType.Desktop,
                                DefaultClientApplicationName = "Sage 100",
                                ClientInstanceAuthorizations = new List<ManageAuthorisationModel.ClientInstanceAuthorization> {
                                    new ManageAuthorisationModel.ClientInstanceAuthorization() {
                                         Authorizations = new List<ManageAuthorisationModel.Authorization>
                                        {
                                            new ManageAuthorisationModel.Authorization()
                                            {
                                                    ExpiresAt = DateTime.ParseExact("30/06/2012", "dd/MM/yyyy", null),
                                                    GrantedAt = DateTime.ParseExact("29/04/2012", "dd/MM/yyyy", null),
                                                    Id = Guid.NewGuid(),
                                                    PermissionSets = new List<ManageAuthorisationModel.AuthorizationPermissionSet> {
                                                        new ManageAuthorisationModel.AuthorizationPermissionSet() {
                                                            PermissionSetDisplayDescriptions = new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Identify you to the resource" },
                                                                                                                                                    new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Upload data to be processed"}}
                                                        }
                                                    }
                                            },
                                            new ManageAuthorisationModel.Authorization()
                                            {
                                                    ExpiresAt = DateTime.ParseExact("15/06/2012", "dd/MM/yyyy", null),
                                                    GrantedAt = DateTime.ParseExact("29/04/2012", "dd/MM/yyyy", null),
                                                    Id = Guid.NewGuid(),
                                                    PermissionSets = new List<ManageAuthorisationModel.AuthorizationPermissionSet> {
                                                        new ManageAuthorisationModel.AuthorizationPermissionSet() {
                                                            PermissionSetDisplayDescriptions = new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Read contact list" },
                                                                                                                                                    new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Spam your friends"}}
                                                        }
                                                    }
                                            }
                                        },
                                        DeviceName = "Home"
                                    }
                                }
                            },
                            new ManageAuthorisationModel.ClientAuthorization()
                            { 
                                ClientApplicationNames = new List<ManageAuthorisationModel.CultureSpecificValue> {new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Sage Ciel" }},
                                ClientType = ManageAuthorisationModel.ClientType.Desktop,
                                DefaultClientApplicationName = "Sage Ciel",
                                ClientInstanceAuthorizations = new List<ManageAuthorisationModel.ClientInstanceAuthorization> {
                                    new ManageAuthorisationModel.ClientInstanceAuthorization() {
                                         Authorizations = new List<ManageAuthorisationModel.Authorization>
                                        {
                                            new ManageAuthorisationModel.Authorization()
                                            {
                                                    ExpiresAt = DateTime.ParseExact("30/06/2012", "dd/MM/yyyy", null),
                                                    GrantedAt = DateTime.ParseExact("29/04/2012", "dd/MM/yyyy", null),
                                                    Id = Guid.NewGuid(),
                                                    PermissionSets = new List<ManageAuthorisationModel.AuthorizationPermissionSet> {
                                                        new ManageAuthorisationModel.AuthorizationPermissionSet() {
                                                            PermissionSetDisplayDescriptions = new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Identify you to the resource" },
                                                                                                                                                    new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Upload data to be processed"}}
                                                        }
                                                    }
                                            },
                                            new ManageAuthorisationModel.Authorization()
                                            {
                                                    ExpiresAt = DateTime.ParseExact("15/06/2012", "dd/MM/yyyy", null),
                                                    GrantedAt = DateTime.ParseExact("29/04/2012", "dd/MM/yyyy", null),
                                                    Id = Guid.NewGuid(),
                                                    PermissionSets = new List<ManageAuthorisationModel.AuthorizationPermissionSet> {
                                                        new ManageAuthorisationModel.AuthorizationPermissionSet() {
                                                            PermissionSetDisplayDescriptions = 
                                                            new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Read contact list" },
                                                                                                                                                    new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Spam your friends"}}
                                                        }
                                                    }
                                            }
                                        },
                                        DeviceName = "Work"
                                    }
                                }
                            }
                        }
                    }, 
                    new ManageAuthorisationModel.ResourceAuthorization()
                    { 
                        ResourceApplicationNames = new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "WebApplication B" } },
                        ClientAuthorizations = new List<ManageAuthorisationModel.ClientAuthorization>
                        {
                            new ManageAuthorisationModel.ClientAuthorization()
                            { 
                                ClientApplicationNames = new List<ManageAuthorisationModel.CultureSpecificValue> {new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "WebApplication B client" }},
                                ClientType = ManageAuthorisationModel.ClientType.Desktop,
                                DefaultClientApplicationName = "WebApplication B client",
                                ClientInstanceAuthorizations = new List<ManageAuthorisationModel.ClientInstanceAuthorization> {
                                    new ManageAuthorisationModel.ClientInstanceAuthorization() {
                                         Authorizations = new List<ManageAuthorisationModel.Authorization>
                                        {
                                            new ManageAuthorisationModel.Authorization()
                                            {
                                                    ExpiresAt = DateTime.ParseExact("30/06/2012", "dd/MM/yyyy", null),
                                                    GrantedAt = DateTime.ParseExact("29/04/2012", "dd/MM/yyyy", null),
                                                    Id = Guid.NewGuid(),
                                                    PermissionSets = new List<ManageAuthorisationModel.AuthorizationPermissionSet>{
                                                        new ManageAuthorisationModel.AuthorizationPermissionSet() {
                                                            PermissionSetDisplayDescriptions = new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Identify you to the resource" },
                                                                                                                                                    new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Upload data to be processed"}}
                                                        }
                                                    }
                                            },
                                            new ManageAuthorisationModel.Authorization()
                                            {
                                                    ExpiresAt = DateTime.ParseExact("15/06/2012", "dd/MM/yyyy", null),
                                                    GrantedAt = DateTime.ParseExact("29/04/2012", "dd/MM/yyyy", null),
                                                    Id = Guid.NewGuid(),
                                                    PermissionSets = new List<ManageAuthorisationModel.AuthorizationPermissionSet> {
                                                        new ManageAuthorisationModel.AuthorizationPermissionSet() {
                                                            PermissionSetDisplayDescriptions = new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Read contact list" },
                                                                                                                                                    new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Spam your friends"}}
                                                        }
                                                    }
                                            }
                                        },
                                        DeviceName = "Home"
                                    }
                                }
                            },
                            new ManageAuthorisationModel.ClientAuthorization()
                            { 
                                ClientApplicationNames = new List<ManageAuthorisationModel.CultureSpecificValue> { new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "WebApplication B client" }},
                                ClientType = ManageAuthorisationModel.ClientType.Desktop,
                                DefaultClientApplicationName = "WebApplication B client",
                                ClientInstanceAuthorizations = new List<ManageAuthorisationModel.ClientInstanceAuthorization> {
                                    new ManageAuthorisationModel.ClientInstanceAuthorization() {
                                         Authorizations = new List<ManageAuthorisationModel.Authorization>
                                        {
                                            new ManageAuthorisationModel.Authorization()
                                            {
                                                    ExpiresAt = DateTime.ParseExact("30/06/2012", "dd/MM/yyyy", null),
                                                    GrantedAt = DateTime.ParseExact("29/04/2012", "dd/MM/yyyy", null),
                                                    Id = Guid.NewGuid(),
                                                    PermissionSets = new List<ManageAuthorisationModel.AuthorizationPermissionSet>{
                                                        new ManageAuthorisationModel.AuthorizationPermissionSet() {
                                                            PermissionSetDisplayDescriptions = new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Identify you to the resource" },
                                                                                                                                                    new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Upload data to be processed"}}
                                                        }
                                                    }
                                            },
                                            new ManageAuthorisationModel.Authorization()
                                            {
                                                    ExpiresAt = DateTime.ParseExact("15/06/2012", "dd/MM/yyyy", null),
                                                    GrantedAt = DateTime.ParseExact("29/04/2012", "dd/MM/yyyy", null),
                                                    Id = Guid.NewGuid(),
                                                    PermissionSets = new List<ManageAuthorisationModel.AuthorizationPermissionSet> {
                                                        new ManageAuthorisationModel.AuthorizationPermissionSet() {
                                                            PermissionSetDisplayDescriptions = new List<ManageAuthorisationModel.CultureSpecificValue>{ new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Read contact list" },
                                                                                                                                                    new ManageAuthorisationModel.CultureSpecificValue() { Culture = "", Value = "Spam your friends"}}
                                                        }
                                                    }
                                            }
                                        },
                                        DeviceName = "Work"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return View(model);
        }

        public ActionResult CancelManageAuthorisations(string id)
        {
            return Redirect("/");
        }

        public ActionResult CancelChange(string id)
        {
            return Redirect("/");
        }

        public ActionResult ForgotPassword(string id)
        {
            ulong i = ulong.Parse(id);

            return Redirect("/RecoveryStage1/" + i.ToString());
        }
    }
}
