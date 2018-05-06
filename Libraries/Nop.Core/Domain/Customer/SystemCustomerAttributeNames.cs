
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
        public static string ZhiXiao_NickName { get { return "ZhiXiao.NickName"; } }              // �Ա�
        public static string ZhiXiao_IdCardNum { get { return "ZhiXiao.IdCardNum"; } }          // ���֤��
        public static string ZhiXiao_YinHang { get { return "ZhiXiao.YinHang"; } }              // ����
        public static string ZhiXiao_KaiHuHang { get { return "ZhiXiao.KaiHuHang"; } }          // ������
        public static string ZhiXiao_KaiHuMing { get { return "ZhiXiao.KaiHuMing"; } }          // ������
        public static string ZhiXiao_BandNum { get { return "ZhiXiao.BandNum"; } }              // ���п���


        //public static string ZhiXiao_TeamId { get { return "ZhiXiao.TeamId"; } }                // ��ǰС��id
        public static string ZhiXiao_InTeamOrder { get { return "ZhiXiao.InTeamOrder"; } }      // ��С��������id(������ʱ��)
        public static string ZhiXiao_InTeamTime { get { return "ZhiXiao.InTeamTime"; } }        // �����С���ʱ��
        public static string ZhiXiao_Password2 { get { return "ZhiXiao.Password2"; } }          // ��������
        public static string ZhiXiao_LevelId { get { return "ZhiXiao.LevelId"; } }              // ����id
        public static string ZhiXiao_ParentId { get { return "ZhiXiao.ParentId"; } }            // �Ƽ���id
        public static string ZhiXiao_ChildCount { get { return "ZhiXiao.ChildCount"; } }        // ���߸���
        public static string ZhiXiao_MoneyNum { get { return "ZhiXiao.MoneyNum"; } }            // ��ǰ���
        public static string ZhiXiao_MoneyHistory { get { return "ZhiXiao.MoneyHistory"; } }    // ��ʷ���(ֻ��������)
        public static string ZhiXiao_SendProductStatus { get { return "ZhiXiao.SendProductStatus"; } }    // �ջ�״̬
        public static string ZhiXiao_SendProductLogId { get { return "ZhiXiao.SendProductLodId"; } }    // �ջ���Ϣ��log id������ʾ
    }
}