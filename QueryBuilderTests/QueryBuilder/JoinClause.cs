using System.Data.Common;
using System.Text;
using Library.QueryBuilder.QueryBuilders;

namespace Library.QueryBuilder
{
    /// ============================================================================================================================
    /// <summary>
    /// Contains all information to build a Join
    /// </summary>
    public struct JoinClause
    {

        #region Properties

        /// <summary> The type of Join </summary>
        public JoinType JoinType;
        /// <summary> The referencing table </summary>
        public object FromTable;
        /// <summary> The referencing column </summary>
        public string FromColumn;
        /// <summary> How are the columns compared </summary>
        public Comparison ComparisonOperator;
        /// <summary> The referenced table </summary>
        public object ToTable;
        /// <summary> The referenced column </summary>
        public string ToColumn;
        /// <summary> Contains where clauses </summary>
        public WhereStatement Where;

        #endregion Properties

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a JoinClause
        /// </summary>
        /// <param name="joinType"> Type of join </param>
        /// <param name="fromTable"> Referencing table </param>
        /// <param name="fromColumn"> Referencing column </param>
        /// <param name="comparison"> How are the columns compared </param>
        /// <param name="toTable"> Referenced table </param>
        /// <param name="toColumn"> Referenced column </param>
        public JoinClause(JoinType joinType, string fromTable, string fromColumn, Comparison comparison, string toTable, string toColumn)
        {
            JoinType = joinType;
            FromTable = fromTable;
            FromColumn = fromColumn;
            ComparisonOperator = comparison;
            ToTable = toTable;
            ToColumn = toColumn;
            Where = new WhereStatement();
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a JoinClause
        /// </summary>
        /// <param name="joinType"> Type of join </param>
        /// <param name="fromTable"> Referencing table </param>
        /// <param name="fromTableAlias"> Alias for the referencing Table</param>
        /// <param name="fromColumn"> Referencing column </param>
        /// <param name="comparison"> How are the columns compared </param>
        /// <param name="toTable"> Referenced table </param>
        /// <param name="toColumn"> Referenced column </param>
        public JoinClause(JoinType joinType, string fromTable, string fromTableAlias, string fromColumn, Comparison comparison, string toTable, string toColumn)
        {
            JoinType = joinType;
            FromTable = new Table(fromTable, fromTableAlias);
            FromColumn = fromColumn;
            ComparisonOperator = comparison;
            ToTable = new Table(toTable, string.Empty);
            ToColumn = toColumn;
            Where = new WhereStatement();
        }


        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Construcktor for a JoinClause
        /// </summary>
        /// <param name="joinType"> Type of join </param>
        /// <param name="fromTable"> Referencing table </param>
        /// <param name="fromColumn"> Referencing column </param>
        /// <param name="comparison"> How are the columns compared </param>
        /// <param name="toTable"> Referenced table </param>
        /// <param name="toColumn"> Referenced column </param>
        public JoinClause(JoinType joinType, SubSelectQueryBuilder fromTable, string fromColumn, Comparison comparison, string toTable, string toColumn)
        {
            JoinType = joinType;
            FromTable = fromTable;
            FromColumn = fromColumn;
            ComparisonOperator = comparison;
            ToTable = toTable;
            ToColumn = toColumn;
            Where = new WhereStatement();
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a JoinClause
        /// </summary>
        /// <param name="joinType"> Type of join </param>
        /// <param name="fromTable"> Referencing table </param>
        /// <param name="fromColumn"> Referencing column </param>
        /// <param name="comparison"> How are the columns compared </param>
        /// <param name="toTable"> Referenced table </param>
        /// <param name="toColumn"> Referenced column </param>
        public JoinClause(JoinType joinType, string fromTable, string fromColumn, Comparison comparison, SubSelectQueryBuilder toTable, string toColumn)
        {
            JoinType = joinType;
            FromTable = fromTable;
            FromColumn = fromColumn;
            ComparisonOperator = comparison;
            ToTable = toTable;
            ToColumn = toColumn;
            Where = new WhereStatement();
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a JoinClause
        /// </summary>
        /// <param name="joinType"> Type of join </param>
        /// <param name="fromTable"> Referencing table </param>
        /// <param name="fromTableAlias"> Alias for the referencing Table</param>
        /// <param name="fromColumn"> Referencing column </param>
        /// <param name="comparison"> How are the columns compared </param>
        /// <param name="toTable"> Referenced table </param>
        /// <param name="toColumn"> Referenced column </param>
        public JoinClause(JoinType joinType, string fromTable, string fromTableAlias, string fromColumn, Comparison comparison, SubSelectQueryBuilder toTable, string toColumn)
        {
            JoinType = joinType;
            FromTable = new Table(fromTable, fromTableAlias);
            FromColumn = fromColumn;
            ComparisonOperator = comparison;
            ToTable = toTable;
            ToColumn = toColumn;
            Where = new WhereStatement();
        }
        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a JoinClause
        /// </summary>
        /// <param name="joinType"> Type of join </param>
        /// <param name="fromTable"> Referencing table </param>
        /// <param name="fromColumn"> Referencing column </param>
        /// <param name="comparison"> How are the columns compared </param>
        /// <param name="toTable"> Referenced table </param>
        /// <param name="toColumn"> Referenced column </param>
        public JoinClause(JoinType joinType, SubSelectQueryBuilder fromTable, string fromColumn, Comparison comparison, SubSelectQueryBuilder toTable, string toColumn)
        {
            JoinType = joinType;
            FromTable = fromTable;
            FromColumn = fromColumn;
            ComparisonOperator = comparison;
            ToTable = toTable;
            ToColumn = toColumn;
            Where = new WhereStatement();
        }

        #endregion Constructor

        #region Methods

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Builds a String for a complete JoinClause
        /// </summary>
        /// <returns>The string to append on a query</returns>
        public string BuildJoinClause(bool useParameters, ref DbCommand dbCommand)
        {
            StringBuilder sb = new StringBuilder();

            switch (JoinType)
            {
                case JoinType.InnerJoin:
                    sb.Append(JoinType.InnerJoin.GetDescriptionFromEnumValue());// "JOIN");
                    break;
                case JoinType.FullJoin:
                    sb.Append(JoinType.FullJoin.GetDescriptionFromEnumValue());// "FULL OUTER JOIN");
                    break;
                case JoinType.LeftJoin:
                    sb.Append(JoinType.LeftJoin.GetDescriptionFromEnumValue());// "LEFT JOIN");
                    break;
                case JoinType.RightJoin:
                    sb.Append(JoinType.RightJoin.GetDescriptionFromEnumValue());// "RIGHT JOIN");
                    break;
            }

            if (FromTable is SubSelectQueryBuilder subqueryFrom)
            {
                sb.Append(" (" + subqueryFrom.BuildQuery() + ")" + subqueryFrom.Alias);
                sb.Append(" ON ");
                sb.Append(subqueryFrom.Alias + "." + FromColumn);
            }
            else if (FromTable is Table table)
            {
                sb.Append(" " + table.TableName + " " + table.TableAlias);
                sb.Append(" ON");
                if (table.TableAlias != string.Empty)
                    sb.Append(" " + table.TableAlias + "." + FromColumn);
                else
                    sb.Append(" " + table.TableName + "." + FromColumn);
            }
            else
            {
                sb.Append(" " + FromTable);
                sb.Append(" ON");
                sb.Append(" " + FromTable + "." + FromColumn);
            }
            switch (ComparisonOperator)
            {
                case Comparison.Equals:
                    sb.Append(" =");
                    break;
                case Comparison.NotEquals:
                    sb.Append(" !=");
                    break;
                case Comparison.Like:
                    sb.Append(" LIKE");
                    break;
                case Comparison.NotLike:
                    sb.Append(" NOT LIKE");
                    break;
                case Comparison.GreaterThan:
                    sb.Append(" >");
                    break;
                case Comparison.GreaterOrEquals:
                    sb.Append(" >=");
                    break;
                case Comparison.LessThan:
                    sb.Append(" <");
                    break;
                case Comparison.LessOrEquals:
                    sb.Append(" <=");
                    break;
                case Comparison.In:
                    sb.Append(" IN");
                    break;
                case Comparison.NotIn:
                    sb.Append(" NOT IN");
                    break;
                case Comparison.Between:
                    sb.Append(" BETWEEN");
                    break;
                case Comparison.NotBetween:
                    sb.Append(" NOT BETWEEN");
                    break;
            }

            if (ToTable is SubSelectQueryBuilder subqueryTo)
            {
                sb.Append(" (" + subqueryTo.BuildQuery() + ")");
                sb.Append("." + ToColumn);
            }
            else if (ToTable is Table table)
            {
                if (table.TableAlias != string.Empty)
                    sb.Append(" " + table.TableAlias + "." + ToColumn);
                else
                    sb.Append(" " + table.TableName + "." + ToColumn);
            }
            else
            {
                sb.Append(" " + ToTable + "." + ToColumn);
            }

            if(Where!=null && Where.Count >0)
            {
                string whereString = Where.BuildStatement(useParameters, ref dbCommand);
                sb.Append(" "+Where.StatementType);
                sb.Append(" "+whereString);
            }
            return sb.ToString();
        }

        #endregion Methods

    }

    /// ============================================================================================================================
    /// <summary>
    /// Contains all information to build an Apply
    /// </summary>
    public struct ApplyClause
    {

        #region Properties

        /// <summary>Subselect</summary>
        public SubSelectQueryBuilder SubSelect { get; set; }

        /// <summary>Type of apply</summary>
        public ApplyType ApplyType { get; set; }


        #endregion Properties

        #region Constructor

        /// ----------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applyType">Type of the apply</param>
        /// <param name="subSelect">Subselect</param>
        public ApplyClause(ApplyType applyType, SubSelectQueryBuilder subSelect)
        {
            ApplyType = applyType;
            SubSelect = subSelect;
        }

        #endregion Constructor

        #region Methods

        /// ----------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructs the apply string
        /// </summary>
        /// <returns></returns>
        public string BuildApplyClause()
        {
            StringBuilder sb = new StringBuilder();

            switch (ApplyType)
            {
                case ApplyType.CROSSAPPLY:
                    sb.Append(ApplyType.CROSSAPPLY.GetDescriptionFromEnumValue());
                    break;
                case ApplyType.OUTERAPPLY:
                    sb.Append(ApplyType.OUTERAPPLY.GetDescriptionFromEnumValue());
                    break;
            }
            sb.Append(" (");
            sb.Append(SubSelect.BuildQuery());
            sb.Append(") " + SubSelect.Alias);
            return sb.ToString();
        }

        #endregion Methods

    }

    
}
