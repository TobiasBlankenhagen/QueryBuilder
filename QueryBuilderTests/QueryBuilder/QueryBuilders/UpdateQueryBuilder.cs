using System;
using System.Data.Common;
using System.Text;

namespace Library.QueryBuilder.QueryBuilders
{
    /// ============================================================================================================================
    /// <summary>
    /// Query builder for update queries
    /// </summary>
    public class UpdateQueryBuilder : UpdateOrInsertQueryBuilder, IQueryBuilder
    {

        #region Fields
                

        #endregion Fields

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateQueryBuilder()
        {
            WhereStatement = new WhereStatement();
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table"> Name of the database table to update</param>
        public UpdateQueryBuilder(string table)
        {
            WhereStatement = new WhereStatement();
            SetTable(table);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="factory"> The database provider for command creation</param>
        public UpdateQueryBuilder(DbProviderFactory factory)
        {
            WhereStatement = new WhereStatement();
            SetDbProviderFactory(factory);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="table"> Name of the database table to update</param>
        /// <param name="factory"> The database provider for command creation</param>
        public UpdateQueryBuilder(string table, DbProviderFactory factory)
        {
            WhereStatement = new WhereStatement();
            SetTable(table);
            SetDbProviderFactory(factory);
        }

        #endregion Constructor

        #region Properties

        private WhereStatement whereStatement;

        public WhereStatement WhereStatement
        {
            get { return whereStatement; }
            set { whereStatement = value; }
        }


        #endregion Properties

        #region Methods

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a where clause to the statement
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
        /// Builds the query command
        /// </summary>
        /// <param name="useParameters"> Should parameters be used </param>
        /// <returns> The DbCommand with the execution ready query </returns>
        public DbCommand BuildCommand(bool useParameters = true)
        {
            if (useParameters && dbProviderFactory == null)
                throw new Exception("Cannot build a command when the Db Factory hasn't been specified. Call SetDbProviderFactory first.");
            DbCommand dbCommand = dbProviderFactory.CreateCommand();
            StringBuilder sb = new StringBuilder("UPDATE");
            // Table
            if (String.IsNullOrEmpty(table))
                throw new Exception("Table to update was not set.");
            sb.Append(" " + table);
            // SET
            if (fieldValuePairs.Count == 0)
                throw new Exception("Nothing to update.");
            sb.Append(" SET ");
            foreach (FieldValuePair fieldValuePair in fieldValuePairs)
            {
                if (useParameters)
                {
                    DbParameter parameter = AddParameterToCommand(ref dbCommand, fieldValuePair.FieldName);
                    parameter.Value = fieldValuePair.Value;
                    sb.Append(fieldValuePair.FieldName + " = " + parameter.ParameterName + ", ");
                }
                else
                    sb.Append(fieldValuePair.FieldName + " = " + WhereStatement.FormatSQLValue(fieldValuePair.Value) + ", ");
            }
            if (fieldValuePairs.Count > 0)
                sb.Length -= 2;
            // WHERE
            if (WhereStatement.Count > 0)
            {
                sb.Append(" WHERE");
                sb.Append(WhereStatement.BuildStatement(useParameters, ref dbCommand));
            }
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
