using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Models.Customers;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Web.ZhiXiao.Factories;

namespace Web.ZhiXiao.Areas.BunusApp.Controllers
{
    public class CommonController : BasePublicController
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly HttpContextBase _httpContext;
        private readonly IMaintenanceService _maintenanceService;

        //fields for customer login/logont
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IAuthenticationService _authenticationService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CustomerSettings _customerSettings;

        #endregion

        #region Constructors

        public CommonController(IWebHelper webHelper,
            IDateTimeHelper dateTimeHelper,
            ILogger logger,
            IWorkContext workContext,
            IPermissionService permissionService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            HttpContextBase httpContext,
            IMaintenanceService maintenanceService,

            IGenericAttributeService genericAttributeService,
            ICustomerActivityService customerActivityService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerModelFactory customerModelFactory,
            IAuthenticationService authenticationService,
            StoreInformationSettings storeInformationSettings,
            CustomerSettings customerSettings
            )
        {
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
            this._logger = logger;
            this._workContext = workContext;
            this._permissionService = permissionService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._httpContext = httpContext;
            this._maintenanceService = maintenanceService;

            this._genericAttributeService = genericAttributeService;
            this._customerActivityService = customerActivityService;
            this._customerRegistrationService = customerRegistrationService;
            this._customerModelFactory = customerModelFactory;
            this._authenticationService = authenticationService;
            this._storeInformationSettings = storeInformationSettings;
            this._customerSettings = customerSettings;
        }

        #endregion

        #region Login / logout

        // GET: Customer
        public virtual ActionResult Login(bool? checkoutAsGuest)
        {
            return Json(RouteData, JsonRequestBehavior.AllowGet);
            var model = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult =
                   _customerRegistrationService.ValidateCustomer(
                       _customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);

                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled
                                ? _customerService.GetCustomerByUsername(model.Username)
                                : _customerService.GetCustomerByEmail(model.Email);

                            // 管理员账号只能通过管理员登陆页面登陆
                            if (customer.IsAdmin())
                            {
                                var areaInfo = RouteData.DataTokens["area"] as string;
                                if (areaInfo == null || areaInfo != "YiJiaYi_Manage")
                                {
                                    ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                                    break;
                                }
                            }

                            //migrate shopping cart
                            //_shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event       
                            //_eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity(customer, "PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"));

                            if (String.IsNullOrEmpty(returnUrl)
                                || !Url.IsLocalUrl(returnUrl)
                                || returnUrl == "/")
                            {
                                if (customer.IsAdmin())
                                {
                                    return RedirectToRoute("AdminHomePage");
                                }
                                else
                                {
                                    return RedirectToRoute("HomePage");
                                }
                            }

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareLoginModel(model.CheckoutAsGuest);
            return View(model);
        }

        //available even when a store is closed
        //[StoreClosed(true)]
        //available even when navigation is not allowed
        //[PublicStoreAllowNavigation(true)]
        public virtual ActionResult Logout()
        {
            //external authentication
            //ExternalAuthorizerHelper.RemoveParameters();

            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                _customerActivityService.InsertActivity(_workContext.OriginalCustomerIfImpersonated,
                    "Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"),
                    _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id);
                _customerActivityService.InsertActivity("Impersonation.Finished",
                    _localizationService.GetResource("ActivityLog.Impersonation.Finished.Customer"),
                    _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id);

                //logout impersonated customer
                _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated,
                    SystemCustomerAttributeNames.ImpersonatedCustomerId, null);

                //redirect back to customer details page (admin area)
                return this.RedirectToAction("Edit", "Customer",
                    new { id = _workContext.CurrentCustomer.Id, area = "Admin" });

            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.Logout", _localizationService.GetResource("ActivityLog.PublicStore.Logout"));

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            //_eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

            //EU Cookie
            if (_storeInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData["nop.IgnoreEuCookieLawWarning"] = true;
            }

            return RedirectToRoute("login");
        }

        #endregion
    }
}