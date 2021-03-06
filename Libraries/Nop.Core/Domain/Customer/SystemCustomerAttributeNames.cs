
namespace Nop.Core.Domain.Customers
{
    public static partial class SystemCustomerAttributeNames
    {
        //Form fields
        public static string FirstName { get { return "FirstName"; } }
        public static string LastName { get { return "LastName"; } }
        public static string Gender { get { return "Gender"; } }
        public static string DateOfBirth { get { return "DateOfBirth"; } }
        public static string Company { get { return "Company"; } }
        public static string StreetAddress { get { return "StreetAddress"; } }
        public static string StreetAddress2 { get { return "StreetAddress2"; } }
        public static string ZipPostalCode { get { return "ZipPostalCode"; } }
        public static string District { get { return "District"; } }
        public static string City { get { return "City"; } }
        // public static string Country { get { return "CountryId"; } }
        public static string StateProvince { get { return "StateProvince"; } }
        public static string Phone { get { return "Phone"; } }
        public static string Fax { get { return "Fax"; } }
        public static string VatNumber { get { return "VatNumber"; } }
        public static string VatNumberStatusId { get { return "VatNumberStatusId"; } }
        public static string TimeZoneId { get { return "TimeZoneId"; } }
        public static string CustomCustomerAttributes { get { return "CustomCustomerAttributes"; } }

        //Other attributes
        public static string DiscountCouponCode { get { return "DiscountCouponCode"; } }
        public static string GiftCardCouponCodes { get { return "GiftCardCouponCodes"; } }
        public static string AvatarPictureId { get { return "AvatarPictureId"; } }
        public static string ForumPostCount { get { return "ForumPostCount"; } }
        public static string Signature { get { return "Signature"; } }
        public static string PasswordRecoveryToken { get { return "PasswordRecoveryToken"; } }
        public static string PasswordRecoveryTokenDateGenerated { get { return "PasswordRecoveryTokenDateGenerated"; } }
        public static string AccountActivationToken { get { return "AccountActivationToken"; } }
        public static string EmailRevalidationToken { get { return "EmailRevalidationToken"; } }
        public static string LastVisitedPage { get { return "LastVisitedPage"; } }
        public static string ImpersonatedCustomerId { get { return "ImpersonatedCustomerId"; } }
        public static string AdminAreaStoreScopeConfiguration { get { return "AdminAreaStoreScopeConfiguration"; } }



        //depends on store
        public static string CurrencyId { get { return "CurrencyId"; } }
        public static string LanguageId { get { return "LanguageId"; } }
        public static string LanguageAutomaticallyDetected { get { return "LanguageAutomaticallyDetected"; } }
        public static string SelectedPaymentMethod { get { return "SelectedPaymentMethod"; } }
        public static string SelectedShippingOption { get { return "SelectedShippingOption"; } }
        public static string SelectedPickupPoint { get { return "SelectedPickupPoint"; } }
        public static string CheckoutAttributes { get { return "CheckoutAttributes"; } }
        public static string OfferedShippingOptions { get { return "OfferedShippingOptions"; } }
        public static string LastContinueShoppingPage { get { return "LastContinueShoppingPage"; } }
        public static string NotifiedAboutNewPrivateMessages { get { return "NotifiedAboutNewPrivateMessages"; } }
        public static string WorkingThemeName { get { return "WorkingThemeName"; } }
        public static string TaxDisplayTypeId { get { return "TaxDisplayTypeId"; } }
        public static string UseRewardPointsDuringCheckout { get { return "UseRewardPointsDuringCheckout"; } }
        public static string EuCookieLawAccepted { get { return "EuCookieLaw.Accepted"; } }

        // zhixiao
        public static string ZhiXiao_NickName { get { return "ZhiXiao.NickName"; } }              // 性别
        public static string ZhiXiao_IdCardNum { get { return "ZhiXiao.IdCardNum"; } }          // 身份证号
        public static string ZhiXiao_YinHang { get { return "ZhiXiao.YinHang"; } }              // 银行
        public static string ZhiXiao_KaiHuHang { get { return "ZhiXiao.KaiHuHang"; } }          // 开户行
        public static string ZhiXiao_KaiHuMing { get { return "ZhiXiao.KaiHuMing"; } }          // 开户名
        public static string ZhiXiao_BandNum { get { return "ZhiXiao.BandNum"; } }              // 银行卡号


        //public static string ZhiXiao_TeamId { get { return "ZhiXiao.TeamId"; } }                // 当前小组id
        public static string ZhiXiao_InTeamOrder { get { return "ZhiXiao.InTeamOrder"; } }      // 在小组中排序id(按加入时间)
        public static string ZhiXiao_InTeamTime { get { return "ZhiXiao.InTeamTime"; } }        // 加入该小组的时间
        public static string ZhiXiao_Password2 { get { return "ZhiXiao.Password2"; } }          // 二级密码
        public static string ZhiXiao_LevelId { get { return "ZhiXiao.LevelId"; } }              // 级别id
        public static string ZhiXiao_ParentId { get { return "ZhiXiao.ParentId"; } }            // 推荐人id
        public static string ZhiXiao_ChildCount { get { return "ZhiXiao.ChildCount"; } }        // 下线个数
        public static string ZhiXiao_MoneyNum { get { return "ZhiXiao.MoneyNum"; } }            // 当前金币
        public static string ZhiXiao_MoneyHistory { get { return "ZhiXiao.MoneyHistory"; } }    // 历史金币(只计算增加)
        public static string ZhiXiao_SendProductStatus { get { return "ZhiXiao.SendProductStatus"; } }    // 收货状态
        public static string ZhiXiao_SendProductLogId { get { return "ZhiXiao.SendProductLodId"; } }    // 发货信息的log id用于显示
        public static string ZhiXiao_ReceiveProductLogId { get { return "ZhiXiao.ReceiveProductLodId"; } }    // 收货信息的log id用于显示
    }
}