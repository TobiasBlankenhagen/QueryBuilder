using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Library.QueryBuilder.QueryBuilders
{
    /// <summary>
    /// Query builder for select queries
    /// </summary>
    public class SelectQueryBuilder : IQueryBuilder
    {

        #region Fields

        private DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");

        #endregion Fields

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public SelectQueryBuilder()
        {
            SelectedColumns = new List<string>();
            SelectedTables = new List<Table>();
            Joins = new List<JoinClause>();
            Applies = new List<ApplyClause>();
            OrderBy = new List<OrderByClause>();
            this.WhereStatement = new WhereStatement();
            GroupByColumns = new List<string>();
            HavingStatement = new WhereStatement();
            this.TopClause = new TopClause(100, TopUnit.Percent);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="factory"> The database provider for command creation</param>
        public SelectQueryBuilder(DbProviderFactory factory)
            : this()
        {
            SetDbProviderFactory(factory);
        }

        #endregion Constructor

        #region Properties

        /// <summary> Should the query contain the DISTICT keyword</summary>
        public bool Distinct { get; set; }

        /// <summary> Top clause of the query</summary>
        public TopClause TopClause { get; set; }

        /// <summary> List of columns to select</summary>
        public List<string> SelectedColumns { get; set; }

        /// <summary> List of table to select</summary>
        public List<Table> SelectedTables { get; set; }

        /// <summary> Where of the query</summary>
        public WhereStatement WhereStatement { get; set; }

        /// <summary> Having of the query</summary>
        public WhereStatement HavingStatement { get; set; }

        /// <summary> List of Joins</summary>
        public List<JoinClause> Joins { get; set; }

        /// <summary> List of Applies </summary>
        public List<ApplyClause> Applies { get; set; }

        /// <summary> List of order clauses</summary>
        public List<OrderByClause> OrderBy { get; set; }
        /// <summary> List of group by columns </summary>
        public List<string> GroupByColumns { get; set; }

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
        /// Clears the SelectedColumns and selects "*"
        /// </summary>
        public void SelectAllColumns()
        {
            SelectedColumns.Clear();
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Select the specified column
        /// </summary>
        /// <param name="column"> Name of the Column to select</param>
        public void SelectColumn(string column)
        {
            SelectedColumns.Clear();
            SelectedColumns.Add(column);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Selects the specified columns
        /// </summary>
        /// <param name="columns"> Name of the columns to select </param>
        public void SelectColumns(params string[] columns)
        {
            SelectedColumns.Clear();
            SelectedColumns.AddRange(columns);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Selects COUNT(1) as column
        /// </summary>
        public void SelectCount()
        {
            SelectColumn("COUNT(1)");
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Selects the specified table
        /// </summary>
        /// <param name="name"> Name of the table to select</param>
        public void SelectFromTable(string name)
        {
            SelectedTables.Clear();
            SelectedTables.Add(new Table(name, String.Empty));
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Selects the specified table with an alias
        /// </summary>
        /// <param name="name"> Name of the table to select </param>
        /// <param name="alias"> Alias of the table </param>
        public void SelectFromTable(string name, string alias)
        {
            SelectedTables.Clear();
            SelectedTables.Add(new Table(name, alias));
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Selects the subselect query as Table
        /// </summary>
        /// <param name="subselect"> subquery to use as Table</param>
        public void SelectFromTable(SubSelectQueryBuilder subselect)
        {
            SelectedTables.Clear();
            SelectedTables.Add(new Table("(" + subselect.BuildQuery() + ")", subselect.Alias));
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Selects the specified tables
        /// </summary>
        /// <param name="tables"> Names of the tables to select</param>
        public void SelectFromTables(params string[] tables)
        {
            SelectedTables.Clear();
            foreach (string name in tables)
            {
                SelectedTables.Add(new Table(name, string.Empty));
            }
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a joinclause to the list
        /// </summary>
        /// <param name="join"> Joinclause to add</param>
        public void AddJoin(JoinClause join)
        {
            Joins.Add(join);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates and adds a where clause to the list
        /// </summary>
        /// <param name="field"> The field to check</param>
        /// <param name="comparison"> How to compare</param>
        /// <param name="value"> The value to compare to</param>
        /// <returns> The added clause</returns>
        public WhereStatement.Clause AddWhere(object field, Comparison comparison, object value)
        {
            return WhereStatement.Add(field, comparison, value);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds the specified columns to the group by list
        /// </summary>
        /// <param name="columns"> Column to group by</param>
        public void AddGroupBy(params string[] columns)
        {
            foreach (string column in columns)
            {
                GroupByColumns.Add(column);
            }
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates and adds a new clause to the having
        /// </summary>
        /// <param name="field"> The field to check</param>
        /// <param name="comparison"> How to compare</param>
        /// <param name="value"> The value to compare to</param>
        public void AddHavingClause(object field, Comparison comparison, object value)
        {
            HavingStatement.Add(field, comparison, value);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates and adds a order by clause
        /// </summary>
        /// <param name="field"> The field to sort </param>
        /// <param name="order"> The sort direction</param>
        public void AddOrderBy(string field, Sorting order = Sorting.Ascending)
        {
            OrderBy.Add(new OrderByClause(field, order));
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
        /// Builds the query string as subquery with an alias
        /// </summary>
        /// <param name="alias"> The alias identifier of the subquery</param>
        /// <returns> The query </returns>
        public string BuildSubQuery(string alias)
        {
            return "(" + BuildCommand(false).CommandText + ") " + alias;
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

            StringBuilder sb = new StringBuilder("SELECT");
            // Distinct
            if (Distinct)
                sb.Append(" DISTINCT");
            // Top
            if (!(TopClause.Quantity == 100 && TopClause.Unit == TopUnit.Percent))
            {
                sb.Append(" TOP " + TopClause.Quantity);
                if (TopClause.Unit == TopUnit.Percent)
                    sb.Append(" PERCENT");
            }
            // Columns
            if (SelectedColumns.Count < 1)
                sb.Append(" *");
            else
            {
                foreach (string column in SelectedColumns)
                {
                    sb.Append(" " + column + ",");
                }
                sb.Length -= 1;
            }
            // From Table
            if (SelectedTables.Count > 0)
            {
                sb.Append(" FROM");
                foreach (Table table in SelectedTables)
                {
                    sb.Append(" " + table.TableName);
                    if (table.TableAlias != string.Empty)
                        sb.Append(" " + table.TableAlias + ",");
                    else
                        sb.Append(",");
                }
                sb.Length -= 1;
            }
            // JOIN
            if (Joins.Count > 0)
            {
                foreach (JoinClause join in Joins)
                {
                    sb.Append(" " + join.BuildJoinClause(useParameters, ref dbCommand));  //" " + join.ToTable + " ON " + WhereStatement.CreateComparisonClause(join.FromTable + "." + join.FromColumn, join.ComparisonOperator, new SqlLiteral(join.ToTable + "." + join.ToColumn)));
                }
            }
            //APPLY
            if (Applies.Count > 0)
            {
                foreach (ApplyClause apply in Applies)
                {
                    sb.Append(" " + apply.BuildApplyClause());
                }
            }
            // WHERE
            if (WhereStatement.Count > 0)
            {
                sb.Append(" WHERE");
                sb.Append(WhereStatement.BuildStatement(useParameters, ref dbCommand));
            }
            // GROUP BY
            if (GroupByColumns.Count > 0)
            {
                sb.Append(" GROUP BY");
                foreach (string groupByColumn in GroupByColumns)
                {
                    sb.Append(" " + groupByColumn + ",");
                }
                sb.Length -= 1;
            }
            // HAVING
            if (HavingStatement.Count > 0)
            {
                if (GroupByColumns.Count == 0)
                    throw new Exception("Having statement was set without Group By");
                sb.Append(" HAVING");
                sb.Append(HavingStatement.BuildStatement(useParameters, ref dbCommand));
            }
            // ORDER BY
            if (OrderBy.Count > 0)
            {
                sb.Append(" ORDER BY");
                foreach (OrderByClause orderByClause in OrderBy)
                {
                    switch (orderByClause.SortOrder)
                    {
                        case Sorting.Ascending:
                            sb.Append(" " + orderByClause.FieldName + " ASC, ");
                            break;
                        case Sorting.Descending:
                            sb.Append(" " + orderByClause.FieldName + " DESC, ");
                            break;
                    }
                }
                sb.Length -= 2;
            }

            dbCommand.CommandText = sb.ToString().Replace("  ", " ");
            return dbCommand;
        }

        #endregion Methods

    }

    /// ============================================================================================================================
    /// <summary>
    /// Class for subqueries
    /// </summary>
    public class SubSelectQueryBuilder : SelectQueryBuilder
    {

        #region Properties

        /// <summary> Alias of the query</summary>
        public string Alias;

        #endregion Properties

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="alias"> Alias of the query</param>
        public SubSelectQueryBuilder(string alias)
            : base()
        {
            Alias = alias;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="selectQueryBuilder"> Selectquery to use as subquery </param>
        /// <param name="alias"> Alias of the query </param>
        public SubSelectQueryBuilder(SelectQueryBuilder selectQueryBuilder, string alias)
        {
            this.Distinct = selectQueryBuilder.Distinct;
            this.HavingStatement = selectQueryBuilder.HavingStatement;
            this.Joins = selectQueryBuilder.Joins;
            this.OrderBy = selectQueryBuilder.OrderBy;
            this.GroupByColumns = selectQueryBuilder.GroupByColumns;
            this.SelectedColumns = selectQueryBuilder.SelectedColumns;
            this.SelectedTables = selectQueryBuilder.SelectedTables;
            this.TopClause = selectQueryBuilder.TopClause;
            this.WhereStatement = selectQueryBuilder.WhereStatement;
            Alias = alias;
        }

        #endregion Constructor

    }
}