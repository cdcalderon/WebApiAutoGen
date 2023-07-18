using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YPrime.Core.BusinessLayer.Models;
using YPrime.BusinessLayer.Models;

namespace YPrime.BusinessLayer.Interfaces
{
    /// <summary>
    /// Manages embedding PowerBI Analytics
    /// </summary>
    public interface IBIEmbedService
    {
        /// <summary>
        /// Returns the embed config with user assigned sites for site-access filtering
        /// </summary>
        /// <param name="reportId">reportId</param>
        /// /// <param name="userSiteIds">Used to filter list of reports based on sites assigned to the user</param>
        /// <returns></returns>
        Task<EmbedConfig> GetEmbedConfig(Guid reportId, string[] userSiteIds);

        /// <summary>
        /// Returns list of reports in the PowerBI workspace
        /// If applyFilter is true, also filter reports using Study Name
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ExternalReport>> GetAnalyticsInWorkspace();
    }
}
