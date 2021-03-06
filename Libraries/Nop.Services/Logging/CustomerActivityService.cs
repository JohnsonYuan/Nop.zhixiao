﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;

namespace Nop.Services.Logging
{
    /// <summary>
    /// Customer activity service
    /// </summary>
    public class CustomerActivityService : ICustomerActivityService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string ACTIVITYTYPE_ALL_KEY = "Nop.activitytype.all";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string ACTIVITYTYPE_PATTERN_KEY = "Nop.activitytype.";

        #endregion

        #region Fields

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<MoneyLog> _moneyLogRepository;
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
        private readonly IRepository<WithdrawLog> _withdrawLogRepository;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IDataProvider _dataProvider;
        private readonly CommonSettings _commonSettings;
        private readonly IWebHelper _webHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="activityLogRepository">Activity log repository</param>
        /// <param name="moneyLogRepository">Money log repository</param>
        /// <param name="activityLogTypeRepository">Activity log type repository</param>
        /// <param name="workContext">Work context</param>
        /// <param name="dbContext">DB context</param>>
        /// <param name="dataProvider">WeData provider</param>
        /// <param name="commonSettings">Common settings</param>
        /// <param name="webHelper">Web helper</param>
        public CustomerActivityService(ICacheManager cacheManager,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<MoneyLog> moneyLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IRepository<WithdrawLog> withdrawLogRepository,
            IWorkContext workContext,
            IDbContext dbContext, IDataProvider dataProvider,
            CommonSettings commonSettings,
            IWebHelper webHelper,
            IGenericAttributeService genericAttributeService)
        {
            this._cacheManager = cacheManager;
            this._activityLogRepository = activityLogRepository;
            this._moneyLogRepository = moneyLogRepository;
            this._activityLogTypeRepository = activityLogTypeRepository;
            this._withdrawLogRepository = withdrawLogRepository;
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
            this._commonSettings = commonSettings;
            this._webHelper = webHelper;
            this._genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Nested classes

        [Serializable]
        public class ActivityLogTypeForCaching
        {
            public int Id { get; set; }
            public string SystemKeyword { get; set; }
            public string Name { get; set; }
            public bool Enabled { get; set; }
        }

        #endregion

        #region Utitlies

        /// <summary>
        /// Gets all activity log types (class for caching)
        /// </summary>
        /// <returns>Activity log types</returns>
        protected virtual IList<ActivityLogTypeForCaching> GetAllActivityTypesCached()
        {
            //cache
            string key = string.Format(ACTIVITYTYPE_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var result = new List<ActivityLogTypeForCaching>();
                var activityLogTypes = GetAllActivityTypes();
                foreach (var alt in activityLogTypes)
                {
                    var altForCaching = new ActivityLogTypeForCaching
                    {
                        Id = alt.Id,
                        SystemKeyword = alt.SystemKeyword,
                        Name = alt.Name,
                        Enabled = alt.Enabled
                    };
                    result.Add(altForCaching);
                }
                return result;
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        public virtual void InsertActivityType(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            _activityLogTypeRepository.Insert(activityLogType);
            _cacheManager.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        public virtual void UpdateActivityType(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            _activityLogTypeRepository.Update(activityLogType);
            _cacheManager.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type</param>
        public virtual void DeleteActivityType(ActivityLogType activityLogType)
        {
            if (activityLogType == null)
                throw new ArgumentNullException("activityLogType");

            _activityLogTypeRepository.Delete(activityLogType);
            _cacheManager.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }

        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>Activity log type items</returns>
        public virtual IList<ActivityLogType> GetAllActivityTypes()
        {
            var query = from alt in _activityLogTypeRepository.Table
                        orderby alt.Name
                        select alt;
            var activityLogTypes = query.ToList();
            return activityLogTypes;
        }

        /// <summary>
        /// Gets an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <returns>Activity log type item</returns>
        public virtual ActivityLogType GetActivityTypeById(int activityLogTypeId)
        {
            if (activityLogTypeId == 0)
                return null;

            return _activityLogTypeRepository.GetById(activityLogTypeId);
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog InsertActivity(string systemKeyword, string comment, params object[] commentParams)
        {
            return InsertActivity(_workContext.CurrentCustomer, systemKeyword, comment, commentParams);
        }


        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="customer">The customer</param>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog InsertActivity(Customer customer, string systemKeyword, string comment, params object[] commentParams)
        {
            if (customer == null)
                return null;

            var activityTypes = GetAllActivityTypesCached();
            var activityType = activityTypes.ToList().Find(at => at.SystemKeyword == systemKeyword);
            if (activityType == null || !activityType.Enabled)
                return null;

            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);



            var activity = new ActivityLog();
            activity.ActivityLogTypeId = activityType.Id;
            activity.Customer = customer;
            activity.Comment = comment;
            activity.CreatedOnUtc = DateTime.UtcNow;
            activity.IpAddress = _webHelper.GetCurrentIpAddress();

            _activityLogRepository.Insert(activity);

            return activity;
        }

        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLog">Activity log type</param>
        public virtual void DeleteActivity(ActivityLog activityLog)
        {
            if (activityLog == null)
                throw new ArgumentNullException("activityLog");

            _activityLogRepository.Delete(activityLog);
        }

        /// <summary>
        /// Gets all activity log items
        /// </summary>
        /// <param name="createdOnFrom">Log item creation from; null to load all activities</param>
        /// <param name="createdOnTo">Log item creation to; null to load all activities</param>
        /// <param name="customerId">Customer identifier; null to load all activities</param>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ipAddress">IP address; null or empty to load all activities</param>
        /// <returns>Activity log items</returns>
        public virtual IPagedList<ActivityLog> GetAllActivities(DateTime? createdOnFrom = null,
            DateTime? createdOnTo = null, int? customerId = null, int activityLogTypeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, string ipAddress = null)
        {
            var query = _activityLogRepository.Table;
            if (!String.IsNullOrEmpty(ipAddress))
                query = query.Where(al => al.IpAddress.Contains(ipAddress));
            if (createdOnFrom.HasValue)
                query = query.Where(al => createdOnFrom.Value <= al.CreatedOnUtc);
            if (createdOnTo.HasValue)
                query = query.Where(al => createdOnTo.Value >= al.CreatedOnUtc);
            if (activityLogTypeId > 0)
                query = query.Where(al => activityLogTypeId == al.ActivityLogTypeId);
            if (customerId.HasValue)
                query = query.Where(al => customerId.Value == al.CustomerId);

            query = query.OrderByDescending(al => al.CreatedOnUtc);

            var activityLog = new PagedList<ActivityLog>(query, pageIndex, pageSize);
            return activityLog;
        }

        /// <summary>
        /// Gets all activity log items by types.
        /// </summary>
        public virtual IPagedList<ActivityLog> GetAllActivitiesByTypes(string[] logTypeSystemNames,
            DateTime? createdOnFrom = null,
            DateTime? createdOnTo = null,
            int? customerId = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string ipAddress = null)
        {
            var query = _activityLogRepository.Table;

            // filter by log type
            if (logTypeSystemNames != null)
                query = query.Join(_activityLogTypeRepository.Table, x => x.ActivityLogTypeId, y => y.Id, (x, y) => new { Log = x, LogType = y })
                    .Where(z => logTypeSystemNames.Contains(z.LogType.SystemKeyword))
                    .Select(z => z.Log);
            if (createdOnFrom.HasValue)
                query = query.Where(al => createdOnFrom.Value <= al.CreatedOnUtc);
            if (createdOnTo.HasValue)
                query = query.Where(al => createdOnTo.Value >= al.CreatedOnUtc);
            if (customerId.HasValue)
                query = query.Where(al => customerId.Value == al.CustomerId);

            if (!String.IsNullOrEmpty(ipAddress))
                query = query.Where(al => al.IpAddress.Contains(ipAddress));

            query = query.OrderByDescending(al => al.CreatedOnUtc);

            var activityLog = new PagedList<ActivityLog>(query, pageIndex, pageSize);
            return activityLog;
        }

        /// <summary>
        /// Gets an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog GetActivityById(int activityLogId)
        {
            if (activityLogId == 0)
                return null;

            return _activityLogRepository.GetById(activityLogId);
        }

        /// <summary>
        /// Clears activity log
        /// </summary>
        public virtual void ClearAllActivities()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //although it's not a stored procedure we use it to ensure that a database supports them
                //we cannot wait until EF team has it implemented - http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/suggestions/1015357-batch-cud-support


                //do all databases support "Truncate command"?
                string activityLogTableName = _dbContext.GetTableName<ActivityLog>();
                _dbContext.ExecuteSqlCommand(String.Format("TRUNCATE TABLE [{0}]", activityLogTableName));
            }
            else
            {
                var activityLog = _activityLogRepository.Table.ToList();
                foreach (var activityLogItem in activityLog)
                    _activityLogRepository.Delete(activityLogItem);
            }
        }

        /// <summary>
        /// Inserts an money log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="moneyDelta"></param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Money log item</returns>
        public virtual MoneyLog InsertMoneyLog(string systemKeyword, long moneyDelta, string comment, params object[] commentParams)
        {
            return InsertMoneyLog(_workContext.CurrentCustomer, systemKeyword, moneyDelta, comment, commentParams);
        }

        /// <summary>
        /// Insert money log
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="systemKeyword"></param>
        /// <param name="comment"></param>
        /// <param name="commentParams"></param>
        /// <returns></returns>
        public virtual MoneyLog InsertMoneyLog(Customer customer, string systemKeyword, string comment, params object[] commentParams)
        {
            return InsertMoneyLog(customer, systemKeyword, 0, comment, commentParams);
        }

        /// <summary>
        /// Inserts an monety log item
        /// </summary>
        /// <param name="customer">The customer</param>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Money log item</returns>
        public virtual MoneyLog InsertMoneyLog(Customer customer, string systemKeyword, long moneyDelta, string comment, params object[] commentParams)
        {
            if (customer == null)
                return null;

            var activityTypes = GetAllActivityTypesCached();
            var activityType = activityTypes.ToList().Find(at => at.SystemKeyword == systemKeyword);
            if (activityType == null || !activityType.Enabled)
                return null;

            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);

            var moneyLog = new MoneyLog();
            moneyLog.ActivityLogTypeId = activityType.Id;
            moneyLog.Customer = customer;
            moneyLog.Comment = comment;
            moneyLog.CreatedOnUtc = DateTime.UtcNow;
            moneyLog.IpAddress = _webHelper.GetCurrentIpAddress();

            var currentMoney = customer.GetMoneyNum();
            moneyLog.MoneyBefore = currentMoney;
            moneyLog.MoneyDelta = moneyDelta;
            moneyLog.MoneyAfter = currentMoney + moneyDelta;

            _moneyLogRepository.Insert(moneyLog);

            // update customer's money
            if (moneyDelta != 0)
            {
                // 实际扣除输入的金额
                _genericAttributeService.SaveAttribute(
                    customer,
                    SystemCustomerAttributeNames.ZhiXiao_MoneyNum,
                    currentMoney + moneyDelta);

                // money history只记录金额增加
                if (moneyDelta > 0)
                {
                    // 钱历史记录
                    _genericAttributeService.SaveAttribute(
                        customer,
                        SystemCustomerAttributeNames.ZhiXiao_MoneyHistory,
                        currentMoney + moneyDelta);
                }
            }

            return moneyLog;
        }

        /// <summary>
        /// Deletes an money log item
        /// </summary>
        /// <param name="moneyLog">Money log</param>
        public virtual void DeleteMoneyLog(MoneyLog moneyLog)
        {
            if (moneyLog == null)
                throw new ArgumentNullException("moneyLog");

            _moneyLogRepository.Delete(moneyLog);
        }

        /// <summary>
        /// Gets all money log items
        /// </summary>
        /// <param name="createdOnFrom">Log item creation from; null to load all activities</param>
        /// <param name="createdOnTo">Log item creation to; null to load all activities</param>
        /// <param name="customerId">Customer identifier; null to load all activities</param>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ipAddress">IP address; null or empty to load all activities</param>
        /// <returns>Money log items</returns>
        public virtual IPagedList<MoneyLog> GetAllMoneyLogs(DateTime? createdOnFrom = null,
            DateTime? createdOnTo = null, int? customerId = null, int activityLogTypeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, string ipAddress = null)
        {
            var query = _moneyLogRepository.Table;
            if (!String.IsNullOrEmpty(ipAddress))
                query = query.Where(al => al.IpAddress.Contains(ipAddress));
            if (createdOnFrom.HasValue)
                query = query.Where(al => createdOnFrom.Value <= al.CreatedOnUtc);
            if (createdOnTo.HasValue)
                query = query.Where(al => createdOnTo.Value >= al.CreatedOnUtc);
            if (activityLogTypeId > 0)
                query = query.Where(al => activityLogTypeId == al.ActivityLogTypeId);
            if (customerId.HasValue)
                query = query.Where(al => customerId.Value == al.CustomerId);

            query = query.OrderByDescending(al => al.CreatedOnUtc);

            var moneyLog = new PagedList<MoneyLog>(query, pageIndex, pageSize);
            return moneyLog;
        }

        #region Withdraw log

        /// <summary>
        /// Inserts an withdraw log item
        /// </summary>
        /// <param name="amount">The withdraw amount</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Activity log item</returns>
        public virtual WithdrawLog InsertWithdraw(int amount, string comment, params object[] commentParams)
        {
            return InsertWithdraw(_workContext.CurrentCustomer, amount, comment, commentParams);
        }

        /// <summary>
        /// Inserts an withdraw log item
        /// </summary>
        /// <param name="customer">The customer</param>
        /// <param name="amount">The withdraw amount</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Withdraw log item</returns>
        public virtual WithdrawLog InsertWithdraw(Customer customer, int amount, string comment, params object[] commentParams)
        {
            if (customer == null)
                return null;

            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);

            var withdraw = new WithdrawLog();
            withdraw.Amount = amount;
            withdraw.IsDone = false;
            withdraw.Customer = customer;
            withdraw.Comment = comment;
            withdraw.CreatedOnUtc = DateTime.UtcNow;
            withdraw.IpAddress = _webHelper.GetCurrentIpAddress();

            _withdrawLogRepository.Insert(withdraw);

            return withdraw;
        }

        /// <summary>
        /// Update withdraw log item
        /// </summary>
        /// <param name="withdrawLog"></param>
        public virtual void UpdateWithdrawLog(WithdrawLog withdrawLog)
        {
            if (withdrawLog == null)
                throw new ArgumentNullException("withdrawLog");

            _withdrawLogRepository.Update(withdrawLog);
        }

        /// <summary>
        /// Deletes an withdraw log item
        /// </summary>
        /// <param name="withdrawLog">Activity log type</param>
        public virtual void DeleteWithdraw(WithdrawLog withdrawLog)
        {
            if (withdrawLog == null)
                throw new ArgumentNullException("activityLog");

            _withdrawLogRepository.Delete(withdrawLog);
        }

        /// <summary>
        /// Gets all activity log items
        /// </summary>
        /// <param name="createdOnFrom">Log item creation from; null to load all activities</param>
        /// <param name="createdOnTo">Log item creation to; null to load all activities</param>
        /// <param name="customerId">Customer identifier; null to load all activities</param>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="ipAddress">IP address; null or empty to load all activities</param>
        /// <returns>Activity log items</returns>
        public virtual IPagedList<WithdrawLog> GetAllWithdraws(DateTime? createdOnFrom = null,
            DateTime? createdOnTo = null, int? customerId = null, bool? isDone = false,
            int pageIndex = 0, int pageSize = int.MaxValue, string ipAddress = null)
        {
            var query = _withdrawLogRepository.Table;
            if (!String.IsNullOrEmpty(ipAddress))
                query = query.Where(al => al.IpAddress.Contains(ipAddress));
            if (createdOnFrom.HasValue)
                query = query.Where(al => createdOnFrom.Value <= al.CreatedOnUtc);
            if (createdOnTo.HasValue)
                query = query.Where(al => createdOnTo.Value >= al.CreatedOnUtc);
            if (isDone.HasValue)
                query = query.Where(al => al.IsDone == isDone.Value);
            if (customerId.HasValue)
                query = query.Where(al => customerId.Value == al.CustomerId);

            query = query.OrderByDescending(al => al.CreatedOnUtc);

            var withdrawLog = new PagedList<WithdrawLog>(query, pageIndex, pageSize);
            return withdrawLog;
        }

        /// <summary>
        /// Gets an withdraw log item
        /// </summary>
        /// <param name="withdrawLogId">withdrawLog log identifier</param>
        /// <returns>Activity log item</returns>
        public virtual WithdrawLog GetWithdrawById(int withdrawLogId)
        {
            if (withdrawLogId == 0)
                return null;

            return _withdrawLogRepository.GetById(withdrawLogId);
        }

        /// <summary>
        /// Clears Withdraw log
        /// </summary>
        public virtual void ClearAllWithdraws()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //although it's not a stored procedure we use it to ensure that a database supports them
                //we cannot wait until EF team has it implemented - http://data.uservoice.com/forums/72025-entity-framework-feature-suggestions/suggestions/1015357-batch-cud-support


                //do all databases support "Truncate command"?
                string WithdrawTableName = _dbContext.GetTableName<WithdrawLog>();
                _dbContext.ExecuteSqlCommand(String.Format("TRUNCATE TABLE [{0}]", WithdrawTableName));
            }
            else
            {
                var withdrawLogs = _withdrawLogRepository.Table.ToList();
                foreach (var withdrawLogItem in withdrawLogs)
                    _withdrawLogRepository.Delete(withdrawLogItem);
            }
        }

        #endregion

        #endregion
    }
}
