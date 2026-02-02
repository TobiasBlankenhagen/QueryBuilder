using System.ComponentModel;
using System.Linq;

namespace Library.QueryBuilder
{
    /// ============================================================================================================================
    /// <summary> 
    /// Enum for the logical operators
    /// </summary>
    public enum LogicOperator
    {
        /// <summary> AND</summary>
        [Description("AND")]
        AND,
        /// <summary> OR </summary>
        [Description("OR")]
        OR,
        /// <summary> AND NOT</summary>
        [Description("AND NOT")]
        ANDNOT,
        /// <summary>NOT</summary>
        [Description("NOT")]
        NOT,
    }

    /// ============================================================================================================================
    /// <summary>
    /// The avaiable Comparison operators
    /// </summary>
    public enum Comparison
    {
        /// <summary> = </summary>
        Equals,
        /// <summary> != </summary>
        NotEquals,
        /// <summary> LIKE </summary>
        Like,
        /// <summary> NOT field LIKE </summary>
        NotLike,
        /// <summary> > </summary>
        GreaterThan,    
        /// <summary> >= </summary>
        GreaterOrEquals,
        /// <summary> <![CDATA[<]]> </summary>
        LessThan,
        /// <summary> <![CDATA[<]]>=</summary>
        LessOrEquals,
        /// <summary> IN </summary>
        In,
        /// <summary> NOT field IN</summary>
        NotIn,
        /// <summary> BETWEEN </summary>
        Between,
        /// <summary> NOT field BETWEEN </summary>
        NotBetween,
        /// <summary> IS NULL, use null as value</summary>
        ISNULL,
        /// <summary> IS NOT NULL, use null as value</summary>
        ISNOTNULL,
    }

    /// ============================================================================================================================
    /// <summary>
    /// Enum for the join types
    /// </summary>
    public enum JoinType
    {
        /// <summary> INNER JOIN </summary>
        [Description("JOIN")]
        InnerJoin,
        /// <summary> OUTER JOIN</summary>
        [Description("FULL OUTER JOIN")]
        FullJoin,
        /// <summary> LEFT JOIN </summary>
        [Description("LEFT JOIN")]
        LeftJoin,
        /// <summary> RIGHT JOIN</summary>
        [Description("RIGHT JOIN")]
        RightJoin,
    }

    /// ============================================================================================================================
    /// <summary>
    /// Enum for the apply types
    /// </summary>
    public enum ApplyType
    {
        /// <summary>CROSS APPLY</summary>
        [Description("CROSS APPLY")]
        CROSSAPPLY,
        /// <summary> OUTER APPLY</summary>
        [Description("OUTER APPLY")]
        OUTERAPPLY
    }

    /// ============================================================================================================================
    /// <summary>
    /// Enum for the sort directions
    /// </summary>
    public enum Sorting
    {
        /// <summary> Ascending sort direction </summary>
        Ascending,
        /// <summary> Descending sort direction </summary>
        Descending,
    }

    /// ============================================================================================================================
    /// <summary>
    /// Enum for the Unit for the Top clause
    /// </summary>
    public enum TopUnit
    {
        /// <summary> Selects the Top Records </summary>
        Records,
        /// <summary> Selects the Top Percent of Records</summary>
        Percent,
    }

    /// ============================================================================================================================
    /// <summary>
    /// Extends the enums with functions
    /// </summary>
    public static class EnumExtensions
    {
        /// -------------------------------------------------------------------------------------------------------------------------
        public static string GetDescriptionFromEnumValue<T>(this T value)
        {
            System.Type type = value.GetType();
            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());
            System.Collections.Generic.IEnumerable<object> vs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            DescriptionAttribute attribute = vs.SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
