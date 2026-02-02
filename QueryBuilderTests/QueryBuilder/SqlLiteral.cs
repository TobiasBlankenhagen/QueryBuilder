using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.QueryBuilder
{
    /// ============================================================================================================================
    /// <summary>
    /// Class for wrapping of functions
    /// </summary>
    public class SqlLiteral
    {
        /// <summary> Gets the affected Rows</summary>
        public static string StatementRowsAffected = "SELECT @@ROWCOUNT";

        /// <summary> The function to wrap</summary>
        public string Value { get; set; }

        /// <summary>
        /// Constructor for a SqlLiteral
        /// </summary>
        /// <param name="value"> The function to wrap</param>
        public SqlLiteral(string value)
        {
            this.Value = value;
        }
    }

}
