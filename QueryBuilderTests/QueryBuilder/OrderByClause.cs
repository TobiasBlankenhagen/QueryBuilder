using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.QueryBuilder
{
    /// <summary>
    /// Contains all Information to build a Order By
    /// </summary>
    public struct OrderByClause
    {
        /// <summary> The field to sort</summary>
        public string FieldName;
        /// <summary> The sort direction</summary>
        public Sorting SortOrder;
        
        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a OrderByClause
        /// </summary>
        /// <param name="field"> The field to sort </param>
        /// <param name="order"> The sort direction</param>
        public OrderByClause(string field, Sorting order)
        {
            FieldName = field;
            SortOrder = order;
        }
    }
}
