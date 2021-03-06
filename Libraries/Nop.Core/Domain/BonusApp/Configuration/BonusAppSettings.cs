using System.ComponentModel;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.BonusApp.Configuration
{
    /// <summary>
    /// Represents an bonus app status.
    /// </summary>
    public class BonusAppSettings : IBonusApp_Settings
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public string SiteTitle { get; set; }

        /// <summary>
        /// 用户返回金额比例 目前返还100%
        /// </summary>
        public double UserReturnMoneyPercent { get; set; } = 1;

        /// <summary>
        /// 用户充值后按照百分比存入奖金池
        /// </summary>
        public double SaveToAppMoneyPercent { get; set; } = 0.2;

        /// <summary>
        /// 提现比例
        /// </summary>
        public double Withdraw_Rate { get; set; } = 0.95;

        public string AuthCookieName { get; set; }  // YiJiaYi.BONUS

        /// <summary>
        /// mdt salt
        /// </summary>
        public string HashedPasswordFormat { get; set; } = "MD5";
        /// <summary>
        /// mdt salt
        /// </summary>
        public string CustomerPasswordSalt { get; set; } = "Z3GP1bc=";
    }
}