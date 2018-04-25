using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Vendors;
using Nop.Core.Domain.ZhiXiao;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;

namespace Nop.Services.Installation
{
    public partial class CodeFirstInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<CustomerTeam> _customerTeamRepository;
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerPassword> _customerPasswordRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<ActivityLog> _activityLogRepository;private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<SearchTerm> _searchTermRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CodeFirstInstallationService(
            IRepository<CustomerTeam>  customerTeamRepository,
            IRepository<Store> storeRepository,
            IRepository<Language> languageRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerPassword> customerPasswordRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<UrlRecord> urlRecordRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<Address> addressRepository,
            IRepository<Vendor> vendorRepository,
            IRepository<SearchTerm> searchTermRepository,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper)
        {
            this._customerTeamRepository = customerTeamRepository;
            this._storeRepository = storeRepository;
            this._languageRepository = languageRepository;
            this._customerRepository = customerRepository;
            this._customerPasswordRepository = customerPasswordRepository;
            this._customerRoleRepository = customerRoleRepository;
            this._urlRecordRepository = urlRecordRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._activityLogRepository = activityLogRepository;
            this._addressRepository = addressRepository;
            this._vendorRepository = vendorRepository;
            this._searchTermRepository = searchTermRepository;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Utilities

        protected virtual void InstallStores()
        {
            //var storeUrl = "http://www.yourStore.com/";
            var storeUrl = _webHelper.GetStoreLocation(false);
            var stores = new List<Store>
            {
                new Store
                {
                    Name = "七上八下直销系统",
                    Url = storeUrl,
                    SslEnabled = false,
                    Hosts = "yourstore.com,www.yourstore.com",
                    DisplayOrder = 1,
                    //should we set some default company info?
                    CompanyName = "伊佳宜",
                    CompanyAddress = "your company country, state, zip, street, etc",
                    CompanyPhoneNumber = "(123) 456-78901",
                    CompanyVat = null,
                },
            };

            _storeRepository.Insert(stores);
        }

        protected virtual void InstallLanguages()
        {
            var language = new Language
            {
                Name = "English",
                LanguageCulture = "en-US",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 2
            };

            var language2 = new Language
            {
                Name = "中文",
                LanguageCulture = "zh-CN",
                UniqueSeoCode = "cn",
                FlagImageFileName = "cn.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageRepository.Insert(language);
            _languageRepository.Insert(language2);
        }

        protected virtual void InstallLocaleResources()
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            //'Chinese' language
            var language = _languageRepository.Table.Single(l => l.Name == "中文");
            //save resources
            var localesXml = File.ReadAllText(CommonHelper.MapPath("~/App_Data/Localization/zhs.nopres.xml"));
            localizationService.ImportResourcesFromXml(language, localesXml);

            //'English' language
            language = _languageRepository.Table.Single(l => l.Name == "English");

            localesXml = File.ReadAllText(CommonHelper.MapPath("~/App_Data/Localization/defaultResources.nopres.xml"));
            localizationService.ImportResourcesFromXml(language, localesXml);
        }

        protected virtual void InstallCustomerTeams()
        {
            var teams = new List<CustomerTeam>
            {
                new CustomerTeam
                {
                    TeamNumber = DateTime.UtcNow.ToString("yyyymmdd"),
                    UserCount = 0,
                    CreatedOnUtc = DateTime.UtcNow
                }
            };

            _customerTeamRepository.Insert(teams);
        }

        protected virtual void InstallCustomersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new CustomerRole
            {
                Name = "系统管理员",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Administrators,
            };
            var crManagers = new CustomerRole
            {
                Name = "管理员",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Managers,
            };
            var crForumModerators = new CustomerRole
            {
                Name = "Forum Moderators",
                Active = false,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.ForumModerators,
            };
            var crRegistered_Normal = new CustomerRole
            {
                Name = "普通用户",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Registered,
            };
            var crRegistered_Normal_Advanced = new CustomerRole
            {
                Name = "高级用户",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Registered_Advanced,
            };
            var crGuests = new CustomerRole
            {
                Name = "匿名用户(未注册用户)",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Guests,
            };
            var crVendors = new CustomerRole
            {
                Name = "Vendors",
                Active = false,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Vendors,
            };

            var customerRoles = new List<CustomerRole>
                                {
                                    crAdministrators,
                                    crForumModerators,
                                    crRegistered_Normal,
                                    crRegistered_Normal_Advanced,
                                    crGuests,
                                    crVendors
                                };
            _customerRoleRepository.Insert(customerRoles);

            //default store 
            var defaultStore = _storeRepository.Table.FirstOrDefault();

            if (defaultStore == null)
                throw new Exception("No default store could be loaded");

            var storeId = defaultStore.Id;

            //default team
            var defaultTeam = _customerTeamRepository.Table.FirstOrDefault();

            if (defaultTeam == null)
                throw new Exception("No default team could be loaded");

            var teamId = defaultTeam.Id;

            //admin user
            var adminUser = new Customer
            {
                CustomerTeamId = teamId,
                CustomerGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };

            var defaultAdminUserAddress = new Address
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "12345678",
                Email = defaultUserEmail,
                FaxNumber = "",
                Company = "Nop Solutions Ltd",
                Address1 = "21 West 52nd Street",
                Address2 = "",
                City = "New York",
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow,
            };
            adminUser.Addresses.Add(defaultAdminUserAddress);

            adminUser.CustomerRoles.Add(crAdministrators);
            adminUser.CustomerRoles.Add(crManagers);
            adminUser.CustomerRoles.Add(crForumModerators);
            adminUser.CustomerRoles.Add(crRegistered_Normal);

            _customerRepository.Insert(adminUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.FirstName, "John");
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.LastName, "Smith");

            //set hashed admin password
            var customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));

            //second user
            var secondUserEmail = "xiaoyuan@yourStore.com";
            var secondUser = new Customer
            {
                CustomerTeamId = teamId,
                CustomerGuid = Guid.NewGuid(),
                Email = secondUserEmail,
                Username = secondUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSecondUserAddress = new Address
            {
                FirstName = "Steve",
                LastName = "Gates",
                PhoneNumber = "87654321",
                Email = secondUserEmail,
                FaxNumber = "",
                Company = "Steve Company",
                Address1 = "750 Bel Air Rd.",
                Address2 = "",
                City = "Los Angeles",
                ZipPostalCode = "90077",
                CreatedOnUtc = DateTime.UtcNow,
            };
            secondUser.Addresses.Add(defaultSecondUserAddress);
            secondUser.BillingAddress = defaultSecondUserAddress;
            secondUser.ShippingAddress = defaultSecondUserAddress;

            secondUser.CustomerRoles.Add(crManagers);
            secondUser.CustomerRoles.Add(crRegistered_Normal);

            _customerRepository.Insert(secondUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(secondUser, SystemCustomerAttributeNames.FirstName, defaultSecondUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(secondUser, SystemCustomerAttributeNames.LastName, defaultSecondUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = secondUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            #region Registered user

            //third user
            var thirdUserEmail = "xiaowang@yourStore.com";
            var thirdUser = new Customer
            {
                CustomerTeamId = teamId,
                CustomerGuid = Guid.NewGuid(),
                Email = thirdUserEmail,
                Username = thirdUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultThirdUserAddress = new Address
            {
                FirstName = "Arthur",
                LastName = "Holmes",
                PhoneNumber = "111222333",
                Email = thirdUserEmail,
                FaxNumber = "",
                Company = "Holmes Company",
                Address1 = "221B Baker Street",
                Address2 = "",
                City = "London",
                ZipPostalCode = "NW1 6XE",
                CreatedOnUtc = DateTime.UtcNow,
            };
            thirdUser.Addresses.Add(defaultThirdUserAddress);
            thirdUser.BillingAddress = defaultThirdUserAddress;
            thirdUser.ShippingAddress = defaultThirdUserAddress;

            thirdUser.CustomerRoles.Add(crRegistered_Normal);

            _customerRepository.Insert(thirdUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(thirdUser, SystemCustomerAttributeNames.FirstName, defaultThirdUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(thirdUser, SystemCustomerAttributeNames.LastName, defaultThirdUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = thirdUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fourth user
            var fourthUserEmail = "james_pan@nopCommerce.com";
            var fourthUser = new Customer
            {
                CustomerTeamId = teamId,
                CustomerGuid = Guid.NewGuid(),
                Email = fourthUserEmail,
                Username = fourthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFourthUserAddress = new Address
            {
                FirstName = "James",
                LastName = "Pan",
                PhoneNumber = "369258147",
                Email = fourthUserEmail,
                FaxNumber = "",
                Company = "Pan Company",
                Address1 = "St Katharine抯 West 16",
                Address2 = "",
                City = "St Andrews",
                ZipPostalCode = "KY16 9AX",
                CreatedOnUtc = DateTime.UtcNow,
            };
            fourthUser.Addresses.Add(defaultFourthUserAddress);
            fourthUser.BillingAddress = defaultFourthUserAddress;
            fourthUser.ShippingAddress = defaultFourthUserAddress;

            fourthUser.CustomerRoles.Add(crRegistered_Normal);

            _customerRepository.Insert(fourthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(fourthUser, SystemCustomerAttributeNames.FirstName, defaultFourthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fourthUser, SystemCustomerAttributeNames.LastName, defaultFourthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = fourthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //fifth user
            var fifthUserEmail = "brenda_lindgren@nopCommerce.com";
            var fifthUser = new Customer
            {
                CustomerTeamId = teamId,
                CustomerGuid = Guid.NewGuid(),
                Email = fifthUserEmail,
                Username = fifthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultFifthUserAddress = new Address
            {
                FirstName = "Brenda",
                LastName = "Lindgren",
                PhoneNumber = "14785236",
                Email = fifthUserEmail,
                FaxNumber = "",
                Company = "Brenda Company",
                Address1 = "1249 Tongass Avenue, Suite B",
                Address2 = "",
                City = "Ketchikan",
                ZipPostalCode = "99901",
                CreatedOnUtc = DateTime.UtcNow,
            };
            fifthUser.Addresses.Add(defaultFifthUserAddress);
            fifthUser.BillingAddress = defaultFifthUserAddress;
            fifthUser.ShippingAddress = defaultFifthUserAddress;

            fifthUser.CustomerRoles.Add(crRegistered_Normal);

            _customerRepository.Insert(fifthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(fifthUser, SystemCustomerAttributeNames.FirstName, defaultFifthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(fifthUser, SystemCustomerAttributeNames.LastName, defaultFifthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = fifthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            //sixth user
            var sixthUserEmail = "victoria_victoria@nopCommerce.com";
            var sixthUser = new Customer
            {
                CustomerTeamId = teamId,
                CustomerGuid = Guid.NewGuid(),
                Email = sixthUserEmail,
                Username = sixthUserEmail,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            var defaultSixthUserAddress = new Address
            {
                FirstName = "Victoria",
                LastName = "Terces",
                PhoneNumber = "45612378",
                Email = sixthUserEmail,
                FaxNumber = "",
                Company = "Terces Company",
                Address1 = "201 1st Avenue South",
                Address2 = "",
                City = "Saskatoon",
                ZipPostalCode = "S7K 1J9",
                CreatedOnUtc = DateTime.UtcNow,
            };
            sixthUser.Addresses.Add(defaultSixthUserAddress);
            sixthUser.BillingAddress = defaultSixthUserAddress;
            sixthUser.ShippingAddress = defaultSixthUserAddress;

            sixthUser.CustomerRoles.Add(crRegistered_Normal);

            _customerRepository.Insert(sixthUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(sixthUser, SystemCustomerAttributeNames.FirstName, defaultSixthUserAddress.FirstName);
            _genericAttributeService.SaveAttribute(sixthUser, SystemCustomerAttributeNames.LastName, defaultSixthUserAddress.LastName);

            //set customer password
            _customerPasswordRepository.Insert(new CustomerPassword
            {
                Customer = sixthUser,
                Password = "123456",
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = string.Empty,
                CreatedOnUtc = DateTime.UtcNow
            });

            #endregion

            //search engine (crawler) built-in user
            var searchEngineUser = new Customer
            {
                Email = "builtin@search_engine_record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.SearchEngine,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            searchEngineUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(searchEngineUser);

            //built-in user for background tasks
            var backgroundTaskUser = new Customer
            {
                Email = "builtin@background-task-record.com",
                CustomerGuid = Guid.NewGuid(),
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.BackgroundTask,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = storeId
            };
            backgroundTaskUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(backgroundTaskUser);
        }

        protected virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>
            {
                //admin area activities
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttribute",
                    Enabled = true,
                    Name = "Add a new address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAddressAttributeValue",
                    Enabled = true,
                    Name = "Add a new address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewAffiliate",
                    Enabled = true,
                    Name = "Add a new affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewBlogPost",
                    Enabled = true,
                    Name = "Add a new blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCampaign",
                    Enabled = true,
                    Name = "Add a new campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCategory",
                    Enabled = true,
                    Name = "Add a new category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCheckoutAttribute",
                    Enabled = true,
                    Name = "Add a new checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCountry",
                    Enabled = true,
                    Name = "Add a new country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCurrency",
                    Enabled = true,
                    Name = "Add a new currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomer",
                    Enabled = true,
                    Name = "Add a new customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerAttribute",
                    Enabled = true,
                    Name = "Add a new customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerAttributeValue",
                    Enabled = true,
                    Name = "Add a new customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewCustomerRole",
                    Enabled = true,
                    Name = "Add a new customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewDiscount",
                    Enabled = true,
                    Name = "Add a new discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewEmailAccount",
                    Enabled = true,
                    Name = "Add a new email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewGiftCard",
                    Enabled = true,
                    Name = "Add a new gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewLanguage",
                    Enabled = true,
                    Name = "Add a new language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewManufacturer",
                    Enabled = true,
                    Name = "Add a new manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureDimension",
                    Enabled = true,
                    Name = "Add a new measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewMeasureWeight",
                    Enabled = true,
                    Name = "Add a new measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewNews",
                    Enabled = true,
                    Name = "Add a new news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewProduct",
                    Enabled = true,
                    Name = "Add a new product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewProductAttribute",
                    Enabled = true,
                    Name = "Add a new product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSetting",
                    Enabled = true,
                    Name = "Add a new setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewSpecAttribute",
                    Enabled = true,
                    Name = "Add a new specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStateProvince",
                    Enabled = true,
                    Name = "Add a new state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewStore",
                    Enabled = true,
                    Name = "Add a new store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewTopic",
                    Enabled = true,
                    Name = "Add a new topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewVendor",
                    Enabled = true,
                    Name = "Add a new vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWarehouse",
                    Enabled = true,
                    Name = "Add a new warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "AddNewWidget",
                    Enabled = true,
                    Name = "Add a new widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteActivityLog",
                    Enabled = true,
                    Name = "Delete activity log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttribute",
                    Enabled = true,
                    Name = "Delete an address attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAddressAttributeValue",
                    Enabled = true,
                    Name = "Delete an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteAffiliate",
                    Enabled = true,
                    Name = "Delete an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPost",
                    Enabled = true,
                    Name = "Delete a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteBlogPostComment",
                    Enabled = true,
                    Name = "Delete a blog post comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCampaign",
                    Enabled = true,
                    Name = "Delete a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCategory",
                    Enabled = true,
                    Name = "Delete category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCheckoutAttribute",
                    Enabled = true,
                    Name = "Delete a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCountry",
                    Enabled = true,
                    Name = "Delete a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCurrency",
                    Enabled = true,
                    Name = "Delete a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomer",
                    Enabled = true,
                    Name = "Delete a customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerAttribute",
                    Enabled = true,
                    Name = "Delete a customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerAttributeValue",
                    Enabled = true,
                    Name = "Delete a customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteCustomerRole",
                    Enabled = true,
                    Name = "Delete a customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteDiscount",
                    Enabled = true,
                    Name = "Delete a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteEmailAccount",
                    Enabled = true,
                    Name = "Delete an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteGiftCard",
                    Enabled = true,
                    Name = "Delete a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteLanguage",
                    Enabled = true,
                    Name = "Delete a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteManufacturer",
                    Enabled = true,
                    Name = "Delete a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureDimension",
                    Enabled = true,
                    Name = "Delete a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMeasureWeight",
                    Enabled = true,
                    Name = "Delete a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteMessageTemplate",
                    Enabled = true,
                    Name = "Delete a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteNews",
                    Enabled = true,
                    Name = "Delete a news"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "DeleteNewsComment",
                    Enabled = true,
                    Name = "Delete a news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteOrder",
                    Enabled = true,
                    Name = "Delete an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeletePlugin",
                    Enabled = true,
                    Name = "Delete a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProduct",
                    Enabled = true,
                    Name = "Delete a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProductAttribute",
                    Enabled = true,
                    Name = "Delete a product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteProductReview",
                    Enabled = true,
                    Name = "Delete a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteReturnRequest",
                    Enabled = true,
                    Name = "Delete a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSetting",
                    Enabled = true,
                    Name = "Delete a setting"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSpecAttribute",
                    Enabled = true,
                    Name = "Delete a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStateProvince",
                    Enabled = true,
                    Name = "Delete a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteStore",
                    Enabled = true,
                    Name = "Delete a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSystemLog",
                    Enabled = true,
                    Name = "Delete system log"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteTopic",
                    Enabled = true,
                    Name = "Delete a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteVendor",
                    Enabled = true,
                    Name = "Delete a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWarehouse",
                    Enabled = true,
                    Name = "Delete a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "DeleteWidget",
                    Enabled = true,
                    Name = "Delete a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditActivityLogTypes",
                    Enabled = true,
                    Name = "Edit activity log types"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttribute",
                    Enabled = true,
                    Name = "Edit an address attribute"
                },
                 new ActivityLogType
                {
                    SystemKeyword = "EditAddressAttributeValue",
                    Enabled = true,
                    Name = "Edit an address attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditAffiliate",
                    Enabled = true,
                    Name = "Edit an affiliate"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditBlogPost",
                    Enabled = true,
                    Name = "Edit a blog post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCampaign",
                    Enabled = true,
                    Name = "Edit a campaign"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCategory",
                    Enabled = true,
                    Name = "Edit category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCheckoutAttribute",
                    Enabled = true,
                    Name = "Edit a checkout attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCountry",
                    Enabled = true,
                    Name = "Edit a country"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCurrency",
                    Enabled = true,
                    Name = "Edit a currency"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomer",
                    Enabled = true,
                    Name = "Edit a customer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerAttribute",
                    Enabled = true,
                    Name = "Edit a customer attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerAttributeValue",
                    Enabled = true,
                    Name = "Edit a customer attribute value"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditCustomerRole",
                    Enabled = true,
                    Name = "Edit a customer role"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditDiscount",
                    Enabled = true,
                    Name = "Edit a discount"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditEmailAccount",
                    Enabled = true,
                    Name = "Edit an email account"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditGiftCard",
                    Enabled = true,
                    Name = "Edit a gift card"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditLanguage",
                    Enabled = true,
                    Name = "Edit a language"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditManufacturer",
                    Enabled = true,
                    Name = "Edit a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureDimension",
                    Enabled = true,
                    Name = "Edit a measure dimension"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMeasureWeight",
                    Enabled = true,
                    Name = "Edit a measure weight"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditMessageTemplate",
                    Enabled = true,
                    Name = "Edit a message template"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditNews",
                    Enabled = true,
                    Name = "Edit a news"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditOrder",
                    Enabled = true,
                    Name = "Edit an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPlugin",
                    Enabled = true,
                    Name = "Edit a plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProduct",
                    Enabled = true,
                    Name = "Edit a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProductAttribute",
                    Enabled = true,
                    Name = "Edit a product attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditProductReview",
                    Enabled = true,
                    Name = "Edit a product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditPromotionProviders",
                    Enabled = true,
                    Name = "Edit promotion providers"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditReturnRequest",
                    Enabled = true,
                    Name = "Edit a return request"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSettings",
                    Enabled = true,
                    Name = "Edit setting(s)"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStateProvince",
                    Enabled = true,
                    Name = "Edit a state or province"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditStore",
                    Enabled = true,
                    Name = "Edit a store"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTask",
                    Enabled = true,
                    Name = "Edit a task"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditSpecAttribute",
                    Enabled = true,
                    Name = "Edit a specification attribute"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditVendor",
                    Enabled = true,
                    Name = "Edit a vendor"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWarehouse",
                    Enabled = true,
                    Name = "Edit a warehouse"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditTopic",
                    Enabled = true,
                    Name = "Edit a topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "EditWidget",
                    Enabled = true,
                    Name = "Edit a widget"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Started",
                    Enabled = true,
                    Name = "Customer impersonation session. Started"
                },
                new ActivityLogType
                {
                    SystemKeyword = "Impersonation.Finished",
                    Enabled = true,
                    Name = "Customer impersonation session. Finished"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportCategories",
                    Enabled = true,
                    Name = "Categories were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportManufacturers",
                    Enabled = true,
                    Name = "Manufacturers were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportProducts",
                    Enabled = true,
                    Name = "Products were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "ImportStates",
                    Enabled = true,
                    Name = "States were imported"
                },
                new ActivityLogType
                {
                    SystemKeyword = "InstallNewPlugin",
                    Enabled = true,
                    Name = "Install a new plugin"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UninstallPlugin",
                    Enabled = true,
                    Name = "Uninstall a plugin"
                },
                //public store activities
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewCategory",
                    Enabled = false,
                    Name = "Public store. View a category"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewManufacturer",
                    Enabled = false,
                    Name = "Public store. View a manufacturer"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ViewProduct",
                    Enabled = false,
                    Name = "Public store. View a product"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.PlaceOrder",
                    Enabled = false,
                    Name = "Public store. Place an order"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.SendPM",
                    Enabled = false,
                    Name = "Public store. Send PM"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.ContactUs",
                    Enabled = false,
                    Name = "Public store. Use contact us form"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToCompareList",
                    Enabled = false,
                    Name = "Public store. Add to compare list"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToShoppingCart",
                    Enabled = false,
                    Name = "Public store. Add to shopping cart"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddToWishlist",
                    Enabled = false,
                    Name = "Public store. Add to wishlist"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Login",
                    Enabled = false,
                    Name = "Public store. Login"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.Logout",
                    Enabled = false,
                    Name = "Public store. Logout"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddProductReview",
                    Enabled = false,
                    Name = "Public store. Add product review"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddNewsComment",
                    Enabled = false,
                    Name = "Public store. Add news comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddBlogComment",
                    Enabled = false,
                    Name = "Public store. Add blog comment"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumTopic",
                    Enabled = false,
                    Name = "Public store. Add forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumTopic",
                    Enabled = false,
                    Name = "Public store. Edit forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumTopic",
                    Enabled = false,
                    Name = "Public store. Delete forum topic"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.AddForumPost",
                    Enabled = false,
                    Name = "Public store. Add forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.EditForumPost",
                    Enabled = false,
                    Name = "Public store. Edit forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.DeleteForumPost",
                    Enabled = false,
                    Name = "Public store. Delete forum post"
                },
                new ActivityLogType
                {
                    SystemKeyword = "UploadNewPlugin",
                    Enabled = true,
                    Name = "Upload a plugin"
                },
            };
            _activityLogTypeRepository.Insert(activityLogTypes);
        }

        protected virtual void InstallActivityLog(string defaultUserEmail)
        {
            //default customer/user
            var defaultCustomer = _customerRepository.Table.FirstOrDefault(x => x.Email == defaultUserEmail);
            if (defaultCustomer == null)
                throw new Exception("Cannot load default customer");

            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditCategory")),
                Comment = "Edited a category ('Computers')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditDiscount")),
                Comment = "Edited a discount ('Sample discount with coupon code')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("EditSpecAttribute")),
                Comment = "Edited a specification attribute ('CPU Type')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("AddNewProductAttribute")),
                Comment = "Added a new product attribute ('Some attribute')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
            _activityLogRepository.Insert(new ActivityLog()
            {
                ActivityLogType = _activityLogTypeRepository.Table.First(alt => alt.SystemKeyword.Equals("DeleteGiftCard")),
                Comment = "Deleted a gift card ('bdbbc0ef-be57')",
                CreatedOnUtc = DateTime.UtcNow,
                Customer = defaultCustomer,
                IpAddress = "127.0.0.1"
            });
        }

        protected virtual void InstallSettings(bool installSampleData)
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            settingService.SaveSetting(new CommonSettings
            {
                UseSystemEmailForContactUsForm = true,
                UseStoredProceduresIfSupported = false, // 不使用存储过程 
                UseStoredProcedureForLoadingCategories = false,
                SitemapEnabled = true,
                SitemapIncludeCategories = true,
                SitemapIncludeManufacturers = true,
                SitemapIncludeProducts = false,
                DisplayJavaScriptDisabledWarning = false,
                UseFullTextSearch = false,
                FullTextMode = FulltextSearchMode.ExactMatch,
                Log404Errors = true,
                BreadcrumbDelimiter = "/",
                RenderXuaCompatible = false,
                XuaCompatibleValue = "IE=edge",
                BbcodeEditorOpenLinksInNewWindow = false
            });

            settingService.SaveSetting(new SeoSettings
            {
                PageTitleSeparator = ". ",
                PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
                DefaultTitle = "直销系统",
                DefaultMetaKeywords = "",
                DefaultMetaDescription = "",
                GenerateProductMetaDescription = true,
                ConvertNonWesternChars = false,
                AllowUnicodeCharsInUrls = true,
                CanonicalUrlsEnabled = false,
                WwwRequirement = WwwRequirement.NoMatter,
                //we disable bundling out of the box because it requires a lot of server resources
                EnableJsBundling = false,
                EnableCssBundling = false,
                TwitterMetaTags = true,
                OpenGraphMetaTags = true,
                ReservedUrlRecordSlugs = new List<string>
                {
                    "admin",
                    "install",
                    "recentlyviewedproducts",
                    "newproducts",
                    "compareproducts",
                    "clearcomparelist",
                    "setproductreviewhelpfulness",
                    "login",
                    "register",
                    "logout",
                    "cart",
                    "wishlist",
                    "emailwishlist",
                    "checkout",
                    "onepagecheckout",
                    "contactus",
                    "passwordrecovery",
                    "subscribenewsletter",
                    "blog",
                    "boards",
                    "inboxupdate",
                    "sentupdate",
                    "news",
                    "sitemap",
                    "search",
                    "config",
                    "eucookielawaccept",
                    "page-not-found",
                    //system names are not allowed (anyway they will cause a runtime error),
                    "con",
                    "lpt1",
                    "lpt2",
                    "lpt3",
                    "lpt4",
                    "lpt5",
                    "lpt6",
                    "lpt7",
                    "lpt8",
                    "lpt9",
                    "com1",
                    "com2",
                    "com3",
                    "com4",
                    "com5",
                    "com6",
                    "com7",
                    "com8",
                    "com9",
                    "null",
                    "prn",
                    "aux"
                },
                CustomHeadTags = ""
            });

            settingService.SaveSetting(new AdminAreaSettings
            {
                DefaultGridPageSize = 15,
                PopupGridPageSize = 10,
                GridPageSizes = "10, 15, 20, 50, 100",
                RichEditorAdditionalSettings = null,
                RichEditorAllowJavaScript = false,
                UseRichEditorInMessageTemplates = false,
                UseIsoDateTimeConverterInJson = true
            });

            settingService.SaveSetting(new LocalizationSettings
            {
                DefaultAdminLanguageId = _languageRepository.Table.Single(l => l.Name == "English").Id,
                UseImagesForLanguageSelection = false,
                SeoFriendlyUrlsForLanguagesEnabled = false,
                AutomaticallyDetectLanguage = false,
                LoadAllLocaleRecordsOnStartup = true,
                LoadAllLocalizedPropertiesOnStartup = true,
                LoadAllUrlRecordsOnStartup = false,
                IgnoreRtlPropertyForAdminArea = false
            });

            settingService.SaveSetting(new CustomerSettings
            {
                UsernamesEnabled = true,
                CheckUsernameAvailabilityEnabled = false,
                AllowUsersToChangeUsernames = false,
                DefaultPasswordFormat = PasswordFormat.Hashed,
                HashedPasswordFormat = "SHA1",
                PasswordMinLength = 6,
                UnduplicatedPasswordsNumber = 4,
                PasswordRecoveryLinkDaysValid = 7,
                PasswordLifetime = 90,
                FailedPasswordAllowedAttempts = 0,
                FailedPasswordLockoutMinutes = 30,
                UserRegistrationType = UserRegistrationType.Standard,
                AllowCustomersToUploadAvatars = false,
                AvatarMaximumSizeBytes = 20000,
                DefaultAvatarEnabled = true,
                ShowCustomersLocation = false,
                ShowCustomersJoinDate = false,
                AllowViewingProfiles = false,
                NotifyNewCustomerRegistration = false,
                HideDownloadableProductsTab = false,
                HideBackInStockSubscriptionsTab = false,
                DownloadableProductsValidateUser = false,
                CustomerNameFormat = CustomerNameFormat.ShowFirstName,
                GenderEnabled = true,
                DateOfBirthEnabled = true,
                DateOfBirthRequired = false,
                DateOfBirthMinimumAge = null,
                CompanyEnabled = true,
                StreetAddressEnabled = false,
                StreetAddress2Enabled = false,
                ZipPostalCodeEnabled = false,
                CityEnabled = false,
                CountryEnabled = false,
                CountryRequired = false,
                StateProvinceEnabled = false,
                StateProvinceRequired = false,
                PhoneEnabled = false,
                FaxEnabled = false,
                AcceptPrivacyPolicyEnabled = false,
                NewsletterEnabled = true,
                NewsletterTickedByDefault = true,
                HideNewsletterBlock = false,
                NewsletterBlockAllowToUnsubscribe = false,
                OnlineCustomerMinutes = 20,
                StoreLastVisitedPage = false,
                SuffixDeletedCustomers = false,
                EnteringEmailTwice = false,
                RequireRegistrationForDownloadableProducts = false,
                DeleteGuestTaskOlderThanMinutes = 1440
            });

            settingService.SaveSetting(new AddressSettings
            {
                CompanyEnabled = true,
                StreetAddressEnabled = true,
                StreetAddressRequired = true,
                StreetAddress2Enabled = true,
                ZipPostalCodeEnabled = true,
                ZipPostalCodeRequired = true,
                CityEnabled = true,
                CityRequired = true,
                CountryEnabled = true,
                StateProvinceEnabled = true,
                PhoneEnabled = true,
                PhoneRequired = true,
                FaxEnabled = true,
            });

            settingService.SaveSetting(new StoreInformationSettings
            {
                StoreClosed = false,
                DefaultStoreTheme = "DefaultClean",
                AllowCustomerToSelectTheme = false,
                DisplayMiniProfilerInPublicStore = false,
                DisplayMiniProfilerForAdminOnly = false,
                DisplayEuCookieLawWarning = false,
                FacebookLink = "http://www.facebook.com/nopCommerce",
                TwitterLink = "https://twitter.com/nopCommerce",
                YoutubeLink = "http://www.youtube.com/user/nopCommerce",
                GooglePlusLink = "https://plus.google.com/+nopcommerce",
                HidePoweredByNopCommerce = true
            });

            settingService.SaveSetting(new RewardPointsSettings
            {
                Enabled = true,
                ExchangeRate = 1,
                PointsForRegistration = 0,
                PointsForPurchases_Amount = 10,
                PointsForPurchases_Points = 1,
                ActivationDelay = 0,
                ActivationDelayPeriodId = 0,
                DisplayHowMuchWillBeEarned = true,
                PointsAccumulatedForAllStores = true,
                PageSize = 10
            });

            settingService.SaveSetting(new NewsSettings
            {
                Enabled = true,
                AllowNotRegisteredUsersToLeaveComments = true,
                NotifyAboutNewNewsComments = false,
                ShowNewsOnMainPage = true,
                MainPageNewsCount = 3,
                NewsArchivePageSize = 10,
                ShowHeaderRssUrl = false,
                NewsCommentsMustBeApproved = false,
                ShowNewsCommentsPerStore = false
            });
        }

        #endregion

        #region Methods

        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            InstallStores();
            InstallLanguages();
            InstallSettings(installSampleData);
            InstallLocaleResources();
            InstallCustomerTeams();
            InstallCustomersAndUsers(defaultUserEmail, defaultUserPassword);
            InstallActivityLogTypes();

            if (installSampleData)
            {
                // no data
                InstallActivityLog(defaultUserEmail);
            }
        }

        #endregion
    }
}