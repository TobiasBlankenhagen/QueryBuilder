using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Library.QueryBuilder.QueryBuilders;

namespace Library.QueryBuilder
{
    /// ============================================================================================================================
    /// <summary>
    /// Class to contain and handle all where related operations
    /// </summary>
    public class WhereStatement
    {

        #region Fields

        private List<Clause> clauses;
        private List<CombinedClause> combinedClauses;

        #endregion Fields

        #region Properties

        /// <summary> Gets the count of clauses</summary>
        public int Count
        {
            get { return clauses.Count + combinedClauses.Count; }
        }

        /// <summary>LogicOperator to connect the clauses with</summary>
        public LogicOperator StatementType { get; set; }
        #endregion Properties

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public WhereStatement()
        {
            clauses = new List<Clause>();
            combinedClauses = new List<CombinedClause>();
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field"> The field to check</param>
        /// <param name="compareOperator"> How to compare</param>
        /// <param name="compareValue"> The value to compare to</param>
        public WhereStatement(string field, Comparison compareOperator, object compareValue)
        {
            clauses = new List<Clause>();
            clauses.Add(new Clause(field, compareOperator, compareValue));
            combinedClauses = new List<CombinedClause>();
        }

        #endregion Constructor

        #region Methods

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a new clause to the list
        /// </summary>
        /// <param name="field"> The field to check</param>
        /// <param name="comparison"> How to compare</param>
        /// <param name="value"> The value to compare to</param>
        /// <returns> The added clause</returns>
        public Clause Add(object field, Comparison comparison, object value)
        {
            Clause clause = new Clause(field, comparison, value);
            clauses.Add(clause);
            return clause;
        }

        /// ----------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a new combined clause to the list
        /// </summary>
        /// <param name="combinedClause">The combined clause to add</param>
        public void AddCombinedClause(CombinedClause combinedClause)
        {
            combinedClauses.Add(combinedClause);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Combines clauses with a logic operator
        /// </summary>
        /// <param name="first"> The first clause to combine</param>
        /// <param name="logic"> The Operator bewtween the clauses</param>
        /// <param name="second"> The second clause to combine</param>
        /// <returns> The combined clause</returns>
        public CombinedClause CombineClauses(object first, LogicOperator logic, object second)
        {
            first = ExtractClauses(first);

            second = ExtractClauses(second);

            CombinedClause combinedClause = new CombinedClause(first, logic, second);
            combinedClauses.Add(combinedClause);
            return combinedClause;
        }

        private object ExtractClauses(object obj)
        {
            if (obj is Clause objClause)
            {
                if (clauses.Contains(objClause))
                    clauses.Remove(objClause);
            }
            if (obj is CombinedClause objCombined)
            {
                if (combinedClauses.Contains(objCombined))
                    combinedClauses.Remove(objCombined);
            }
            if (obj is WhereStatement objStatement)
            {
                if (objStatement.Count > 1)
                {
                    CombinedClause combinedCombClause = new CombinedClause();
                    CombinedClause combClause = new CombinedClause();

                    if (objStatement.clauses.Count > 1)
                        obj = combinedCombClause = objStatement.CombineAllClauses(objStatement.StatementType);
                    else if (objStatement.clauses.Count == 1)
                        obj = combinedCombClause = objStatement.combinedClauses[0];

                    if (objStatement.combinedClauses.Count > 1)
                        obj = combClause = objStatement.CombineAllCombinedClauses(objStatement.StatementType);
                    else if (objStatement.combinedClauses.Count == 1)
                        obj = combClause = objStatement.combinedClauses[0];

                    foreach (Clause clause in objStatement.clauses)
                    {
                        if (clauses.Contains(clause))
                            clauses.Remove(clause);
                    }
                    foreach (CombinedClause combClauses in objStatement.combinedClauses)
                    {
                        if (combinedClauses.Contains(combClauses))
                            combinedClauses.Remove(combClauses);
                    }
                }
                else if (objStatement.Count == 1)
                {
                    if (objStatement.clauses.Count == 1)
                    {
                        obj = objStatement.clauses[0];
                        if (clauses.Contains(objStatement.clauses[0]))
                            clauses.Remove(objStatement.clauses[0]);
                    }
                    else
                    {
                        obj = objStatement.combinedClauses[0];
                        if (combinedClauses.Contains(objStatement.combinedClauses[0]))
                            combinedClauses.Remove(objStatement.combinedClauses[0]);
                    }
                }
                else
                    obj = null;
            }
            return obj;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Combines all clauses with the given logic operator
        /// </summary>
        /// <param name="logic"> The operator to connect the clauses</param>
        /// <returns>A combined clause with all clauses connected</returns>
        public CombinedClause CombineAllClauses(LogicOperator logic)
        {
            CombinedClause combinedClause = new CombinedClause();
            if (clauses.Count >= 2)
            {
                combinedClause = CombineClauses(clauses[0], logic, clauses[1]);
                if (clauses.Count > 0)
                {
                    for (int count = 0; 0 < clauses.Count; count++)
                    {
                        combinedClause = CombineClauses(combinedClause, logic, clauses[0]);
                    }
                }
            }
            else
                throw new Exception("More than 1 clauses are requried for combining");
            return combinedClause;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Combines all combined clauses with the given logic operator
        /// </summary>
        /// <param name="logic"> The operator to connect the clauses</param>
        /// <returns>A combined clause with all clauses connected</returns>
        public CombinedClause CombineAllCombinedClauses(LogicOperator logic)
        {
            CombinedClause combinedClause = new CombinedClause();
            if (combinedClauses.Count >= 2)
            {
                combinedClause = CombineClauses(combinedClauses[0], logic, combinedClauses[1]);
                if (combinedClauses.Count > 0)
                {
                    for (int index = 0; 0 < combinedClauses.Count; index++)
                    {
                        CombineClauses(combinedClause, logic, combinedClauses[0]);
                    }
                }
            }
            else
                throw new Exception("More than 1 clauses are requried for combining");
            return combinedClause;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Builds the where string from the combined and uncombined clauses
        /// </summary>
        /// <param name="useParameters"> Should parameters be used</param>
        /// <param name="dbCommand"> The command to be used for the parameters</param>
        /// <returns> The execution ready where string</returns>
        public string BuildStatement(bool useParameters, ref DbCommand dbCommand)
        {
            StringBuilder sb = new StringBuilder();
            int clausCount = 0;
            foreach (CombinedClause combiClause in combinedClauses)
            {
                sb.Append(BuildCombinedClause(combiClause, useParameters, dbCommand));
                sb.Append(" " + StatementType.GetDescriptionFromEnumValue() + "");
                clauses.RemoveAll(x => x.Equals(combiClause.firstClause));
                clauses.RemoveAll(x => x.Equals(combiClause.secondClause));
            }
            // Removing the last " AND" 
            if (combinedClauses.Count > 0)
                sb.Length -= 4;
            foreach (Clause clause in clauses)
            {
                if (combinedClauses.Count > 0 || clausCount > 0 && clausCount < clauses.Count)
                    sb.Append(" " + StatementType.GetDescriptionFromEnumValue() + " ");
                else
                    sb.Append(" ");
                if (useParameters)
                {
                    DbParameter parameter = AddParameterToCommand(ref dbCommand, FormatSQLField(clause.Field));
                    if (clause.ComparisonOperator == Comparison.In || clause.ComparisonOperator == Comparison.NotIn || clause.Value is SqlLiteral)
                        parameter.Value = FormatSQLValue(clause.Value);
                    else
                        parameter.Value = clause.Value;
                    sb.Append(CreateComparisonClause(FormatSQLField(clause.Field), clause.ComparisonOperator, new SqlLiteral(parameter.ParameterName)));
                }
                else
                    sb.Append(CreateComparisonClause(clause.Field, clause.ComparisonOperator, clause.Value));
                clausCount++;
            }
            return sb.ToString();
        }

        private string BuildCombinedClause(CombinedClause combiClause, bool useParameters, DbCommand dbCommand)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" (");
            if (combiClause.firstClause is Clause firstclause)
            {
                if (useParameters)
                {
                    DbParameter param;
                    if (firstclause.ComparisonOperator == Comparison.In || firstclause.ComparisonOperator == Comparison.NotIn)
                    {
                        param = AddParameterToCommand(ref dbCommand, firstclause.Field.ToString());
                        param.Value = FormatSQLValue(firstclause.Value);
                        sb.Append(CreateComparisonClause(firstclause.Field, firstclause.ComparisonOperator, param.ParameterName));
                    }
                    else if (firstclause.ComparisonOperator != Comparison.Between && firstclause.ComparisonOperator != Comparison.NotBetween)
                    {
                        param = AddParameterToCommand(ref dbCommand, firstclause.Field.ToString());
                        param.Value = firstclause.Value;
                        sb.Append(CreateComparisonClause(firstclause.Field, firstclause.ComparisonOperator, param.ParameterName));
                    }
                    else
                    {
                        DbParameter param1 = AddParameterToCommand(ref dbCommand, firstclause.Field.ToString());
                        DbParameter param2 = AddParameterToCommand(ref dbCommand, firstclause.Field.ToString());
                        object[] objArr = ConvertArrayValue(firstclause.Value);
                        param1.Value = objArr[0];
                        param2.Value = objArr[1];
                        sb.Append(CreateComparisonClause(firstclause.Field, firstclause.ComparisonOperator, new object[2] { new SqlLiteral(param1.ParameterName), new SqlLiteral(param2.ParameterName) }));
                    }
                }
                else
                    sb.Append(CreateComparisonClause(firstclause.Field, firstclause.ComparisonOperator, firstclause.Value));
                clauses.Remove(firstclause);
            }
            if (combiClause.firstClause is CombinedClause firstCombiSubClause)
                sb.Append(BuildCombinedClause(firstCombiSubClause, useParameters, dbCommand));

            sb.Append(" " + combiClause.logicOperator.GetDescriptionFromEnumValue() + " ");

            if (combiClause.secondClause is Clause secondClause)
            {
                if (useParameters)
                {
                    DbParameter param;
                    if (secondClause.ComparisonOperator != Comparison.Between && secondClause.ComparisonOperator != Comparison.NotBetween
                        && secondClause.ComparisonOperator != Comparison.In && secondClause.ComparisonOperator != Comparison.NotIn)
                    {
                        param = AddParameterToCommand(ref dbCommand, secondClause.Field.ToString());
                        param.Value = secondClause.Value;
                        sb.Append(CreateComparisonClause(secondClause.Field, secondClause.ComparisonOperator, param.ParameterName));
                    }
                    else
                    {
                        DbParameter param1 = AddParameterToCommand(ref dbCommand, secondClause.Field.ToString());
                        DbParameter param2 = AddParameterToCommand(ref dbCommand, secondClause.Field.ToString());
                        object[] objArr = ConvertArrayValue(secondClause.Value);
                        param1.Value = objArr[0];
                        param2.Value = objArr[1];
                        sb.Append(CreateComparisonClause(secondClause.Field, secondClause.ComparisonOperator, new object[2] { new SqlLiteral(param1.ParameterName), new SqlLiteral(param2.ParameterName) }));
                    }
                }
                else
                    sb.Append(CreateComparisonClause(secondClause.Field, secondClause.ComparisonOperator, secondClause.Value));
                clauses.Remove(secondClause);
            }
            if (combiClause.secondClause is CombinedClause secondCombiSubClause)
                sb.Append(BuildCombinedClause(secondCombiSubClause, useParameters, dbCommand));
            sb.Append(")");
            return sb.ToString();
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a Parameter to a DbCommand
        /// </summary>
        /// <param name="dbCommand"> The command to add the parameter to</param>
        /// <param name="field"> The field the parameter stands for</param>
        /// <returns> The added parameter</returns>
        private DbParameter AddParameterToCommand(ref DbCommand dbCommand, string field)
        {
            string str = string.Format("@p{0}_{1}", (object)(dbCommand.Parameters.Count + 1), (object)field.Replace('.', '_'));
            DbParameter parameter = dbCommand.CreateParameter();
            parameter.ParameterName = str;
            dbCommand.Parameters.Add((object)parameter);
            return parameter;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Converts an object to an object[]
        /// </summary>
        /// <param name="value"> The object with the Array </param>
        /// <returns> The value as object[] </returns>
        private static object[] ConvertArrayValue(object value)
        {
            object[] objArr;
            if (value is Array valueArr)
            {
                objArr = new object[valueArr.Length];
                if (valueArr is int[] intArr)
                {
                    for (int index = 0; index < valueArr.Length; index++)
                    {
                        objArr[index] = intArr[index];
                    }
                }
                if (valueArr is string[] strArr)
                {
                    for (int index = 0; index < valueArr.Length; index++)
                    {
                        objArr[index] = strArr[index];
                    }
                }
                if (valueArr is object[] objectArr)
                {
                    for (int index = 0; index < valueArr.Length; index++)
                    {
                        objArr[index] = objectArr[index];
                    }
                }
                if (valueArr is DateTime[] dateArr)
                {
                    for (int index = 0; index < valueArr.Length; index++)
                    {
                        objArr[index] = dateArr[index];
                    }
                }
                if (valueArr is DateTime?[] nullableDateArr)
                {
                    for (int index = 0; index < valueArr.Length; index++)
                    {
                        objArr[index] = nullableDateArr[index];
                    }
                }
            }
            else
                return new object[0];
            return objArr;
        }
        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates a single comparison clause
        /// </summary>
        /// <param name="fieldname"> The field of the clause</param>
        /// <param name="comparison"> The comparison operator of the clause</param>
        /// <param name="value"> The value of the clause</param>
        /// <returns> The comparison string</returns>
        private static string CreateComparisonClause(object fieldname, Comparison comparison, object value)
        {
            StringBuilder compClause = new StringBuilder("");
            switch (comparison)
            {
                case Comparison.Equals:

                    if (value != null && value != DBNull.Value)
                        compClause.Append(FormatSQLField(fieldname) + " = " + FormatSQLValue(value));
                    else
                        compClause.Append(FormatSQLField(fieldname) + " IS NULL");
                    break;
                case Comparison.NotEquals:
                    if (value != null && value != DBNull.Value)
                        compClause.Append(FormatSQLField(fieldname) + " <> " + FormatSQLValue(value));
                    else
                        compClause.Append(FormatSQLField(fieldname) + " IS NOT NULL");
                    break;
                case Comparison.Like:
                    compClause.Append(FormatSQLField(fieldname) + " LIKE " + FormatSQLValue(value));
                    break;
                case Comparison.NotLike:
                    compClause.Append(" NOT " + FormatSQLField(fieldname) + " LIKE " + FormatSQLValue(value));
                    break;
                case Comparison.GreaterThan:
                    compClause.Append(FormatSQLField(fieldname) + " > " + FormatSQLValue(value));
                    break;
                case Comparison.GreaterOrEquals:
                    compClause.Append(FormatSQLField(fieldname) + " >= " + FormatSQLValue(value));
                    break;
                case Comparison.LessThan:
                    compClause.Append(FormatSQLField(fieldname) + " < " + FormatSQLValue(value));
                    break;
                case Comparison.LessOrEquals:
                    compClause.Append(FormatSQLField(fieldname) + " <= " + FormatSQLValue(value));
                    break;
                case Comparison.Between:
                case Comparison.NotBetween:
                    if (!(value is Array))
                        throw new Exception("When using Comparison.Between, you have to specify a value of type object[] containing the 2 values for the BETWEEN operation.");
                    object[] objArray = ConvertArrayValue(value); //(object[])value;
                    compClause.Append(FormatSQLField(fieldname));
                    if (comparison == Comparison.NotBetween)
                        compClause.Append(" NOT BETWEEN");
                    else
                        compClause.Append(" BETWEEN");
                    compClause.Append(" " + FormatSQLValue(objArray[0]) + " AND " + FormatSQLValue(objArray[1]));
                    break;
                case Comparison.In:
                case Comparison.NotIn:
                    if (comparison == Comparison.NotIn)
                        compClause.Append("NOT ");
                    if (value is Array array)
                    {
                        if (array.Length < 1)
                            throw new Exception("When using Comparison.In or Comparison.NotIn, you have to specify a value of type object[] containing at least one value.");
                        compClause.Append(FormatSQLField(fieldname) + " IN (");
                        foreach (object someValue in array)
                            compClause.Append(FormatSQLValue(someValue) + ", ");
                        compClause.Length -= 2;
                        compClause.Append(")");
                        break;
                    }
                    if (value is string)
                    {
                        compClause.Append(FormatSQLField(fieldname) + " IN (" + value + ")");
                        break;
                    }
                    compClause.Append(FormatSQLField(fieldname) + " IN (" + FormatSQLValue(value) + ")");
                    break;
                case Comparison.ISNULL:
                    compClause.Append(FormatSQLField(fieldname) + " IS NULL");
                    break;
                case Comparison.ISNOTNULL:
                    compClause.Append(FormatSQLField(fieldname) + " IS NOT NULL");
                    break;
            }
            return compClause.ToString();
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        private static string FormatSQLField(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                switch (value.GetType().Name)
                {
                    case "String":
                    case "string":
                        return value.ToString();
                    case "SqlLiteral":
                        return ((SqlLiteral)value).Value;
                    case "SelectQueryBuilder":
                        return "(" + ((SelectQueryBuilder)value).BuildQuery() + ")";
                    default:
                        return value.ToString();
                }
            }
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Formats a object value in a valid sql value
        /// </summary>
        /// <param name="value"> The value to format</param>
        /// <returns> The formated value</returns>
        internal static string FormatSQLValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                switch (value.GetType().Name)
                {
                    case "String":
                    case "string":
                        {
                            string stringValue = value as string;
                            if (stringValue.Contains("@"))
                                return stringValue;
                            return "'" + ((string)value).Replace("'", "''") + "'";
                        }
                    case "DateTime":
                        return "'" + ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss.FFF") + "'";
                    case "DBNull":
                        return "NULL";
                    case "Boolean":
                        return (bool)value ? "1" : "0";
                    case "Guid":
                        return "'" + ((Guid)value).ToString() + "'";
                    case "SqlLiteral":
                        return ((SqlLiteral)value).Value;
                    case "SelectQueryBuilder":
                    case "SubSelectQueryBuilder":
                        return "(" + ((SelectQueryBuilder)value).BuildQuery() + ")";
                    case "Int32[]":
                        {
                            Int32[] array = value as Int32[];
                            StringBuilder stringvalue = new StringBuilder();
                            for (int index = 0; index < array.Length; index++)
                            {
                                stringvalue.Append(array[index] + ",");
                            }
                            stringvalue.Length -= 1;
                            return stringvalue.ToString();
                        }
                    case "String[]":
                        {
                            string[] array = value as string[];
                            StringBuilder stringvalue = new StringBuilder();
                            for (int index = 0; index < array.Length; index++)
                            {
                                stringvalue.Append("'" + array[index] + "',");
                            }
                            stringvalue.Length -= 1;
                            return stringvalue.ToString();
                        }
                    default:
                        return value.ToString();
                }
            }
        }

        #endregion Methods

        /// ============================================================================================================================
        /// <summary>
        /// Contains all information for a single where clause
        /// </summary>
        public struct Clause
        {
            /// <summary> Field of the clause</summary>
            public object Field;
            /// <summary> How to compare</summary>
            public Comparison ComparisonOperator;
            /// <summary> Value of the clause</summary>
            public object Value;

            /// -------------------------------------------------------------------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="field"> Field of the clause</param>
            /// <param name="comparison"> How to Compare</param>
            /// <param name="value"> Value of the clause</param>
            public Clause(object field, Comparison comparison, object value)
            {
                this.Field = field;
                this.ComparisonOperator = comparison;
                this.Value = value;
            }
        }

        /// ============================================================================================================================
        /// <summary>
        /// Contains all information for combined clauses
        /// </summary>
        public struct CombinedClause
        {
            /// <summary> The first clause to combine</summary>
            public object firstClause;
            /// <summary> The second clause to combine</summary>
            public object secondClause;
            /// <summary> The operator to combine the clauses</summary>
            public LogicOperator logicOperator;

            /// -------------------------------------------------------------------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="firstClause"> The first clause to combine</param>
            /// <param name="logicOperator"> The operator to combine the clauses</param>
            /// <param name="secondClause"> The second clause to combine</param>
            public CombinedClause(object firstClause, LogicOperator logicOperator, object secondClause)
            {
                this.firstClause = firstClause;
                this.logicOperator = logicOperator;
                this.secondClause = secondClause;
            }
        }

    }
}
