using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.QueryBuilder
{
    /// ============================================================================================================================
    /// <summary>
    /// Class for tables
    /// </summary>
    public class Table
    {

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor for a Table
        /// </summary>
        /// <param name="name"> Name of the table</param>
        /// <param name="alias"> Alias of the table </param>
        public Table(string name, string alias)
        {
            TableName = name;
            TableAlias = alias;
        }

        #endregion Constructor

        #region Properties

        /// <summary> Name of the table</summary>
        public string TableName { get; set; }

        /// <summary> Alias of the table</summary>
        public string TableAlias { get; set; } 

        #endregion Properties

    }
}
