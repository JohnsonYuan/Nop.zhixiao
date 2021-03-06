﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Validators.Customers;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Core.Domain.ZhiXiao;
using Nop.Core.Domain.Customers;
using Nop.Core;

namespace Nop.Models.Customers
{
    [Validator(typeof(CustomerValidator))]
    public partial class CustomerModel : BaseNopEntityModel
    {
        public CustomerModel()
        {
            this.AvailableTimeZones = new List<SelectListItem>();
            this.SendEmail = new SendEmailModel() { SendImmediately = true };
            this.SendPm = new SendPmModel();

            this.SelectedCustomerRoleIds = new List<int>();
            this.AvailableCustomerRoles = new List<SelectListItem>();

            //this.AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            //this.AvailableCountries = new List<SelectListItem>();
            //this.AvailableStates = new List<SelectListItem>();
            //this.AvailableVendors = new List<SelectListItem>();
            this.CustomerAttributes = new List<CustomerAttributeModel>();
            this.AvailableNewsletterSubscriptionStores = new List<StoreModel>();
            //this.RewardPointsAvailableStores = new List<SelectListItem>();
        }

        public bool UsernamesEnabled { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.NickName")]
        public string NickName { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Password")]
        [AllowHtml]
        [DataType(DataType.Password)]
        [NoTrim]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [NopResourceDisplayName("Account.Fields.ConfirmPassword")]
        [AllowHtml]
        public string ConfirmPassword { get; set; }

        [NopResourceDisplayName("Account.Fields.Password2")]
        [AllowHtml]
        [DataType(DataType.Password)]
        [NoTrim]
        public string Password2 { get; set; }

        [DataType(DataType.Password)]
        [NoTrim]
        [NopResourceDisplayName("Account.Fields.ConfirmPassword2")]
        [AllowHtml]
        public string ConfirmPassword2 { get; set; }

        /// <summary>
        /// 二级密码
        /// </summary>
        public string ZhiXiao_Password_Display { get { return Password2; } set { Password2 = value; } }

        //[NopResourceDisplayName("Admin.Customers.Customers.Fields.Vendor")]
        //public int VendorId { get; set; }
        //public IList<SelectListItem> AvailableVendors { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Gender")]
        public string Gender { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FirstName")]
        [AllowHtml]
        public string FirstName { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastName")]
        [AllowHtml]
        public string LastName { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.FullName")]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the customer team number.
        /// </summary>
        public string CustomerTeamNum { get; set; }
        public CustomerTeamModel TeamInfo { get; set; }

        public bool DateOfBirthEnabled { get; set; }
        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        public bool CompanyEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Company")]
        [AllowHtml]
        public string Company { get; set; }

        public bool StreetAddressEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.StreetAddress")]
        [AllowHtml]
        public string StreetAddress { get; set; }

        //public bool ZipPostalCodeEnabled { get; set; }
        //[NopResourceDisplayName("Admin.Customers.Customers.Fields.ZipPostalCode")]
        //[AllowHtml]
        //public string ZipPostalCode { get; set; }

        // 省 市 区
        public bool StateProvinceEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.StateProvince")]
        public string StateProvince { get; set; }

        public bool CityEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.City")]
        [AllowHtml]
        public string City { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.District")]
        public string District { get; set; }

        public bool PhoneEnabled { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Phone")]
        [AllowHtml]
        public string Phone { get; set; }

        //public bool FaxEnabled { get; set; }
        //[NopResourceDisplayName("Admin.Customers.Customers.Fields.Fax")]
        //[AllowHtml]
        //public string Fax { get; set; }

        public List<CustomerAttributeModel> CustomerAttributes { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.RegisteredInStore")]
        public string RegisteredInStore { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.IsTaxExempt")]
        public bool IsTaxExempt { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Affiliate")]
        public int AffiliateId { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Affiliate")]
        public string AffiliateName { get; set; }


        //time zone
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.TimeZoneId")]
        [AllowHtml]
        public string TimeZoneId { get; set; }

        public bool AllowCustomersToSetTimeZone { get; set; }

        public IList<SelectListItem> AvailableTimeZones { get; set; }


        //EU VAT
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.VatNumber")]
        [AllowHtml]
        public string VatNumber { get; set; }

        public string VatNumberStatusNote { get; set; }

        public bool DisplayVatNumber { get; set; }


        //registration date
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        //IP adderss
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.IPAddress")]
        public string LastIpAddress { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.LastVisitedPage")]
        public string LastVisitedPage { get; set; }


        //customer roles
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
        public string CustomerRoleNames { get; set; }
        public List<SelectListItem> AvailableCustomerRoles { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCustomerRoleIds { get; set; }


        //newsletter subscriptions (per store)
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Newsletter")]
        public List<StoreModel> AvailableNewsletterSubscriptionStores { get; set; }
        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Newsletter")]
        public int[] SelectedNewsletterSubscriptionStoreIds { get; set; }



        //reward points history
        //public bool DisplayRewardPointsHistory { get; set; }
        //[NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsValue")]
        //public int AddRewardPointsValue { get; set; }
        //[NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsMessage")]
        //[AllowHtml]
        //public string AddRewardPointsMessage { get; set; }
        //[NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore")]
        //public int AddRewardPointsStoreId { get; set; }
        //[NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.AddRewardPointsStore")]
        //public IList<SelectListItem> RewardPointsAvailableStores { get; set; }

        //send email model
        public SendEmailModel SendEmail { get; set; }
        //send PM model
        public SendPmModel SendPm { get; set; }
        //send the welcome message
        public bool AllowSendingOfWelcomeMessage { get; set; }
        //re-send the activation message
        public bool AllowReSendingOfActivationMessage { get; set; }

        //[NopResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth")]
        //public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.Active")]
        public bool Active { get; set; }


        #region 直销系统需要的字段

        /// <summary>
        /// 用户是否可以升级（1. 普通小组 2. CustomerTeam == null）
        /// </summary>
        public bool CanUpgrade { get; set; }

        [Display(Name = "推荐人")]
        [AllowHtml]
        [UIHint("MultiSelect")]
        public int ZhiXiao_ParentId { get; set; }       // 推荐人id
        public Customer ParentUser { get; set; }

        [Display(Name = "小组")]
        public int ZhiXiao_TeamId { get; set; }       // 小组id
        public CustomerTeam CustomerTeam { get; set; }

        public string ZhiXiao_InTeamOrder { get; set; }  // 在小组中顺序
        public string ZhiXiao_InTeamTime { get; set; }   // 进入该小组时间
        public string ZhiXiao_ChildCount { get; set; }   // 下线个数
        [Display(Name = "用户级别")]
        public int ZhiXiao_LevelId { get; set; }        // 级别
        [Display(Name = "积分")]
        public long ZhiXiao_MoneyNum { get; set; }       // 积分(真是个数)
        [Display(Name = "历史积分")]
        public long ZhiXiao_MoneyHistory { get; set; }   // 历史积分(只记录增加, 不记录减少)

        [Display(Name = "身份证号")]
        public string ZhiXiao_IdCardNum { get; set; }       // 身份证号
        [Display(Name = "开户银行")]
        public string ZhiXiao_YinHang { get; set; }         // 银行
        [Display(Name = "开户行名称")]
        public string ZhiXiao_KaiHuHang { get; set; }       // 开户行
        [Display(Name = "开户名")]
        public string ZhiXiao_KaiHuMing { get; set; }       // 开户名
        [Display(Name = "银行卡号")]
        public string ZhiXiao_BankNum { get; set; }         // 银行卡号
        [Display(Name = "发货状态")]
        public int ProductStatusId { get; set; }

        [Display(Name = "级别")]
        public CustomerLevel CustomerLevel
        {
            get
            {
                return (CustomerLevel)this.ZhiXiao_LevelId;
            }
            set
            {
                this.ZhiXiao_LevelId = (int)value;
            }
        }

        /// <summary>
        /// 级别信息
        /// </summary>
        public string LevelDescription { get; set; }

        public SendProductStatus ProductStatus
        {
            get
            {
                return (SendProductStatus)this.ProductStatusId;
            }
            set
            {
                this.ProductStatusId = (int)value;
            }
        }

        #endregion

        #region Nested classes

        public partial class StoreModel : BaseNopEntityModel
        {
            public string Name { get; set; }
        }

        public partial class AssociatedExternalAuthModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth.Fields.Email")]
            public string Email { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth.Fields.ExternalIdentifier")]
            public string ExternalIdentifier { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.AssociatedExternalAuth.Fields.AuthMethodName")]
            public string AuthMethodName { get; set; }
        }

        public partial class RewardPointsHistoryModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Store")]
            public string StoreName { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Points")]
            public int Points { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.PointsBalance")]
            public string PointsBalance { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Message")]
            [AllowHtml]
            public string Message { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.RewardPoints.Fields.Date")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class SendEmailModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.Subject")]
            [AllowHtml]
            public string Subject { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.Body")]
            [AllowHtml]
            public string Body { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.SendImmediately")]
            public bool SendImmediately { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.SendEmail.DontSendBeforeDate")]
            [UIHint("DateTimeNullable")]
            public DateTime? DontSendBeforeDate { get; set; }
        }

        public partial class SendPmModel : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.SendPM.Subject")]
            public string Subject { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.SendPM.Message")]
            public string Message { get; set; }
        }

        public partial class OrderModel : BaseNopEntityModel
        {
            public override int Id { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.Orders.CustomOrderNumber")]
            public string CustomOrderNumber { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.OrderStatus")]
            public string OrderStatus { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.Orders.OrderStatus")]
            public int OrderStatusId { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.OrderTotal")]
            public string OrderTotal { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.Store")]
            public string StoreName { get; set; }

            [NopResourceDisplayName("Admin.Customers.Customers.Orders.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class BackInStockSubscriptionModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Customers.Customers.BackInStockSubscriptions.Store")]
            public string StoreName { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.BackInStockSubscriptions.Product")]
            public int ProductId { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.BackInStockSubscriptions.Product")]
            public string ProductName { get; set; }
            [NopResourceDisplayName("Admin.Customers.Customers.BackInStockSubscriptions.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class CustomerAttributeModel : BaseNopEntityModel
        {
            public CustomerAttributeModel()
            {
                Values = new List<CustomerAttributeValueModel>();
            }

            public string Name { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<CustomerAttributeValueModel> Values { get; set; }

        }

        public partial class CustomerAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }
}