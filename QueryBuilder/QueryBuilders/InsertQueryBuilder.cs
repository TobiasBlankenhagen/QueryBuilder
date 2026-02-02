using System;
using System.Data.Common;
using System.Text;

namespace Library.QueryBuilder.QueryBuilders
{
    /// ============================================================================================================================
    /// <summary>
    /// Query builder for insert queries
    /// </summary>
    public class InsertQueryBuilder : UpdateOrInsertQueryBuilder, IQueryBuilder
    {

        #region Fields

        

        #endregion Fields

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public InsertQueryBuilder()
        {

        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table"> Name of the database table to insert into</param>
        public InsertQueryBuilder(string table)
        {
            SetTable(table);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="factory"> The database provider for command creation</param>
        public InsertQueryBuilder(DbProviderFactory factory)
        {
            SetDbProviderFactory(factory);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table"> Name of the database table to insert into</param>
        /// <param name="factory"> The database provider for command creation</param>
        public InsertQueryBuilder(string table, DbProviderFactory factory)
        {
            SetTable(table);
            SetDbProviderFactory(factory);
        }

        #endregion Constructor

        #region Methods



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
            StringBuilder sb = new StringBuilder("INSERT INTO");
            // Table
            if (String.IsNullOrEmpty(table))
                throw new Exception("Table to update was not set.");
            sb.Append(" " + table);
            // Fields
            if (fieldValuePairs.Count == 0)
                throw new Exception("No fields set to insert");
            sb.Append(" (");
            foreach (FieldValuePair fieldValuePair in fieldValuePairs)
            {
                sb.Append(fieldValuePair.FieldName + ", ");
            }
            sb.Length -= 2;
            sb.Append(")");
            // VALUES
            sb.Append(" VALUES");
            sb.Append(" (");
            foreach (FieldValuePair fieldValuePair in fieldValuePairs)
            {
                if (useParameters)
                {
                    DbParameter parameter = AddParameterToCommand(ref dbCommand, fieldValuePair.FieldName);
                    parameter.Value = fieldValuePair.Value;
                    sb.Append(parameter.ParameterName+", ");
                }
                else
                    sb.Append(WhereStatement.FormatSQLValue(fieldValuePair.Value)+", ");
            }
            sb.Length -= 2;
            sb.Append(")");
            dbCommand.CommandText = sb.ToString();
            return dbCommand;
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

        #endregion Methods

    }
}
