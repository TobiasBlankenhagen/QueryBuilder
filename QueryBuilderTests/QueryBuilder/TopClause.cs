using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.QueryBuilder
{
    /// ============================================================================================================================
    /// <summary>
    /// Contains all Information for building a Top clause
    /// </summary>
    public struct TopClause
    {
        /// <summary> The top ? number of entries are selected</summary>
        public int Quantity;
        /// <summary> Percent or Records</summary>
        public TopUnit Unit;

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a TopClause
        /// </summary>
        /// <param name="nr"> The Top ? number of entries are selected </param>
        public TopClause(int nr)
        {
            Quantity = nr;
            Unit = TopUnit.Records;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a TopClause
        /// </summary>
        /// <param name="nr"> The Top ? number of entries are selected </param>
        /// <param name="unit"> Percent Or Records</param>
        public TopClause(int nr , TopUnit unit)
        {
            Quantity = nr;
            Unit = unit;
        }
    }
}
