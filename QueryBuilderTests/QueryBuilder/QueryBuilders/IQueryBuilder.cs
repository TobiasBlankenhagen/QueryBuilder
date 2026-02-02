using System.Data.Common;

namespace Library.QueryBuilder.QueryBuilders
{
    /// ============================================================================================================================
    /// <summary>
    /// Interface for the QueryBuilders
    /// </summary>
    public interface IQueryBuilder
    {

        #region Methods

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the DbProviderFactory
        /// </summary>
        /// <param name="factory"> The factory to set</param>
        void SetDbProviderFactory(DbProviderFactory factory);

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Builds the DbCommand to a query
        /// </summary>
        /// <param name="useCommand">Build the query with params or not</param>
        /// <returns> The DbCommand with the execution ready query</returns>
        DbCommand BuildCommand(bool useCommand=true);

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Builds a query string
        /// </summary>
        /// <returns> The execution ready query as string</returns>
        string BuildQuery(); 

        #endregion Methods

    }
}
