using System;
using System.Data.Common;
using System.Text;

namespace Library.QueryBuilder.QueryBuilders
{
    /// ============================================================================================================================
    /// <summary>
    /// Query Builder for Delete Queries
    /// </summary>
    public class DeleteQueryBuilder: IQueryBuilder
    {

        #region Fields

        private DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");

        #endregion Fields

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a Delete query
        /// </summary>
        public DeleteQueryBuilder()
        {
            WhereStatement = new WhereStatement();
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a Delete query
        /// </summary>
        /// <param name="dbProviderFactory"> The database provider for command creation</param>
        public DeleteQueryBuilder(DbProviderFactory dbProviderFactory)
            : this()
        {
            SetDbProviderFactory(dbProviderFactory);
        }

        #endregion Constructor

        #region Properties

        /// <summary> Where of the query</summary>
        public WhereStatement WhereStatement { get; set; }

        /// <summary> Table to delete from</summary>
        public string Table { get; set; }

        /// <summary> Flag for deleting all entries </summary>
        public bool DeleteAll { get; set; }

        #endregion Properties

        #region Methods

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the dbProviderFactory to factory
        /// </summary>
        /// <param name="factory"> Database provider for command creation</param>
        public void SetDbProviderFactory(DbProviderFactory factory)
        {
            this.dbProviderFactory = factory;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the table to delete from
        /// </summary>
        /// <param name="table"> name of the table to delete from </param>
        public void SetTable(string table)
        {
            Table = table;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates and adds a where clause to the list
        /// </summary>
        /// <param name="field"> The field to check</param>
        /// <param name="comparison"> How to compare</param>
        /// <param name="value"> The value to compare to</param>
        /// <returns> The added clause</returns>
        public WhereStatement.Clause AddWhere(string field, Comparison comparison, object value)
        {
            return WhereStatement.Add(field, comparison, value);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Builds the query string
        /// </summary>
        /// <returns> The execution ready query</returns>
        public string BuildQuery()
        {
            return BuildCommand(false).CommandText;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Builds the query command
        /// </summary>
        /// <param name="useParameters"> Should parameters be used </param>
        /// <returns> The DbCommand with the execution ready query </returns>
        public DbCommand BuildCommand(bool useParameters = true)
        {
            if (useParameters && dbProviderFactory == null)
                throw new Exception("Cannot build a command when the Db Factory hasn't been specified. Call SetDbProviderFactory first.");
            DbCommand dbCommand = dbProviderFactory.CreateCommand();

            StringBuilder sb = new StringBuilder("DELETE FROM "+Table);

            // WHERE
            if (WhereStatement.Count > 0)
            {
                sb.Append(" WHERE");
                sb.Append(WhereStatement.BuildStatement(useParameters, ref dbCommand));
            }
            else if (DeleteAll == false)
                throw new Exception("You are trying to build a Delete Command without any Where Clauses. If you want to delete any content in the table set \"DeleteAll\" to true.");

            dbCommand.CommandText = sb.ToString().Replace("  ", " ");
            return dbCommand;
        } 

        #endregion Methods

    }
}
