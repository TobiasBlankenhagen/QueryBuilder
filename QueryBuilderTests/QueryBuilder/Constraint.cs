using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Library.QueryBuilder
{
    /// ============================================================================================================================
    /// <summary>
    /// Class for constraints
    /// </summary>
    public class Constraint
    {

        #region Properties

        /// <summary> Name of the constraint </summary>
        public string Name { get; set; }

        /// <summary> Defines the type of the constraint </summary>
        internal ConstraintType Type { get; set; }

        /// <summary> List of Columns for which the constraint applies </summary>
        public List<string> Columns { get; set; }

        /// <summary> Reference for a foreign key constraint </summary>
        public Reference ForeignKeyReference { get; set; }

        /// <summary> Wherestatement of a check constraint </summary>
        public WhereStatement Checkstatement { get; set; }

        #endregion Properties

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"> Defines the type of the constraint </param>
        /// <param name="name"> Name of the constraint </param>
        public Constraint(ConstraintType type, string name)
        {
            Type = type;
            Name = name;
            Columns = new List<string>();
        }

        #endregion Constructor

        #region Methods

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a column to the constraint
        /// </summary>
        /// <param name="column"> Name of the column to add</param>
        public void AddColumn(string column)
        {
            Columns.Add(column);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds multiple column to the constraint
        /// </summary>
        /// <param name="columns"> Names of the columns to add </param>
        public void AddColumns(string[] columns)
        {
            Columns.AddRange(columns);
        }
        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Builds the constraint string
        /// </summary>
        /// <param name="useParameters"> Should DbParameters be used </param>
        /// <param name="dbCommand"> The DbCommand to be used</param>
        /// <returns> The constraint as string</returns>
        public string BuildConstraint(bool useParameters, ref DbCommand dbCommand)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("CONSTRAINT " + Name);
            sb.Append(" " + Type.ToString().Replace('_', ' ').ToUpper() + " (");
            
                if (Type != ConstraintType.Check)
                {
                    if (Type == ConstraintType.Primary_Key && Columns == null && Columns.Count != 1)
                        throw new Exception("A " + Type + " Constraint needs at least/only one Column");
                    if (Columns == null || Columns.Count < 1)
                        throw new Exception("A " + Type + " Constraint needs at least one column");
                    foreach (string column in Columns)
                    {
                        sb.Append(column + ", ");
                    }
                    sb.Length -= 2;
                }
                else
                {
                    sb.Append(Checkstatement.BuildStatement(useParameters, ref dbCommand));
                }
                sb.Append(")");
            if (Type == ConstraintType.Foreign_Key)
            {
                if (ForeignKeyReference != null)
                    sb.Append(" REFERENCES " + ForeignKeyReference.Table + "(" + ForeignKeyReference.Column + ")");
                else
                    throw new Exception("For a Foreign Key Constraint the Reference needs to be set.");

            }
            return sb.ToString();
        }

        #endregion Methods

        /// ============================================================================================================================
        /// <summary>
        /// Class for a reference to a foreign key
        /// </summary>
        public class Reference
        {
            /// <summary> Name of the table to refence </summary>
            public string Table { get; set; }

            /// <summary> Name of the column to reference </summary>
            public string Column { get; set; }

            /// -------------------------------------------------------------------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="table"> Name of the table to reference </param>
            /// <param name="column"> Name of the column to reference </param>
            public Reference(string table, string column)
            {
                Table = table;
                Column = column;
            }
        }

        /// ============================================================================================================================
        /// <summary>
        /// Enum for defining the constraint type
        /// </summary>
        public  enum ConstraintType
        {
            /// <summary> UNIQUE </summary>
            Unique,
            /// <summary> PRIMAY KEY </summary>
            Primary_Key,
            /// <summary> FOREIGN KEY </summary>
            Foreign_Key,
            /// <summary> CHECK </summary>
            Check,
        }
    }
}
