﻿using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Vendors;
    
namespace Nop.Core
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the bonus app current customer
        /// </summary>
        Nop.Core.Domain.BonusApp.Customers.BonusApp_Customer CurrentBonusAppCustomer { get; set; }

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        Customer CurrentCustomer { get; set; }
        /// <summary>
        /// Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        Customer OriginalCustomerIfImpersonated { get; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }
        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        //Currency WorkingCurrency { get; set; }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }
    }
}
