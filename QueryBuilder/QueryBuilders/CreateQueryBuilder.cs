using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Library.QueryBuilder.QueryBuilders
{
    /// ============================================================================================================================
    /// <summary>
    /// Class for building a CREATE query
    /// </summary>
    public class CreateQueryBuilder : IQueryBuilder
    {

        #region Fields

        private DbProviderFactory dbProviderFactory;

        #endregion Fields

        #region Constructor

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// /// <param name="mode"> Defines what to create </param>
        /// /// <param name="name"> Name of the table or the database to create </param>
        public CreateQueryBuilder(CreateMode mode, string name)
        {
            dbProviderFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            Columns = new List<Column>();
            Constraints = new List<Constraint>();
            CreationMode = mode;
            Name = name;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode"> Defines what to create </param>
        /// <param name="name"> Name of the table or the database to create </param>
        /// <param name="factory"> Factory to create a command </param>
        public CreateQueryBuilder(CreateMode mode, string name, DbProviderFactory factory)
        {
            CreationMode = mode;
            Name = name;
            dbProviderFactory = factory;
        }

        #endregion Constructor

        #region Properties

        /// <summary> Name of the table or the database to create</summary>
        public string Name { get; set; }

        /// <summary> Defines what to create </summary>
        public CreateMode CreationMode { get; set; }

        /// <summary> List of columns to create </summary>
        public List<Column> Columns { get; set; }

        /// <summary> List of constraints for the table</summary>
        public List<Constraint> Constraints { get; set; }

        #endregion Properties

        #region Methods

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the name of the Table to create
        /// </summary>
        /// <param name="table"> Name of the table to create </param>
        public void SetTable(string table)
        {
            Name = table;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a primary key constraint to the query
        /// </summary>
        /// <param name="keyColumn"> Name of the column to be used as key </param>
        public void AddPrimaryKey(string keyColumn)
        {
            Constraint constraint = new Constraint(Constraint.ConstraintType.Primary_Key, "PK" + Constraints.Count + "_" + keyColumn);
            constraint.AddColumn(keyColumn);
            Constraints.Add(constraint);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a foreign key constraint to the query
        /// </summary>
        /// <param name="referenceColumn"> Name of the column in the actual table to link</param>
        /// <param name="referenceTable"> Name of the table to refer to</param>
        /// <param name="referencingColumn">Name of the column to refer to</param>
        public void AddForeignKey(string referenceColumn, string referenceTable, string referencingColumn)
        {
            Constraint constraint = new Constraint(Constraint.ConstraintType.Foreign_Key, "FK" + Constraints.Count + "_" + referenceTable + Name);
            constraint.ForeignKeyReference = new Constraint.Reference(referenceTable, referencingColumn);
            constraint.AddColumn(referenceColumn);
            Constraints.Add(constraint);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds check constraint to the query
        /// </summary>
        /// <param name="checkstatement"> The statement to check</param>
        public void AddCheck(WhereStatement checkstatement)
        {
            Constraint constraint = new Constraint(Constraint.ConstraintType.Check, "CHK" + Constraints.Count + "_"+Name);
            constraint.Checkstatement = checkstatement;
            Constraints.Add(constraint);
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a unique constraint to the query
        /// </summary>
        /// <param name="columns"> Names of the columns to set unique</param>
        public void AddUnique(string[] columns)
        {
            Constraint constraint = new Constraint(Constraint.ConstraintType.Unique, "UC" + Constraints.Count + "_" + Name);
            constraint.AddColumns(columns);
            Constraints.Add(constraint);
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
        /// Builds the query command
        /// </summary>
        /// <param name="useParameters"> Should parameters be used </param>
        /// <returns> The DbCommand with the execution ready query </returns>
        public DbCommand BuildCommand(bool useParameters = true)
        {
            DbCommand dbCommand = dbProviderFactory.CreateCommand();
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE " + CreationMode.ToString().ToUpper());
            sb.Append(" " + Name + " ");
            if (CreationMode == CreateMode.Table)
            {
                if (Columns != null && Columns.Count > 0)
                {
                    sb.Append("(");
                    foreach (Column column in Columns)
                    {
                        if (useParameters)
                        {
                            DbParameter paramName = AddParameterToCommand(ref dbCommand, column.Name);
                            paramName.Value = column.Name;
                            DbParameter paramType = AddParameterToCommand(ref dbCommand, column.Name);
                            paramType.Value = column.Datatype;
                            sb.Append(new SqlLiteral(paramName.ParameterName).Value + " " + new SqlLiteral(paramType.ParameterName).Value + ", ");
                        }
                        else
                            sb.Append(column.Name + " " + column.Datatype + ", ");
                    }
                    sb.Length -= 2;
                    sb.Append(")");
                    if (Constraints.Count > 0)
                    {
                        foreach (Constraint constraint in Constraints)
                        {
                            sb.Append(" " + constraint.BuildConstraint(useParameters, ref dbCommand) + ", ");
                        }
                        sb.Length -= 2;
                    }
                    sb.Append(";");
                }
                else
                {
                    throw new Exception("A CREATE TABLE query need at least on column!");
                }
            }
            else
            {
                sb.Length -= 1;
                sb.Append(";");
            }
            dbCommand.CommandText = sb.ToString();
            return dbCommand;
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
        /// Sets the dbProviderFactory to factory
        /// </summary>
        /// <param name="factory"> Database provider for command creation</param>
        public void SetDbProviderFactory(DbProviderFactory factory)
        {
            this.dbProviderFactory = factory;
        }

        #endregion Methods

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Contains all information to create a new column
        /// </summary>
        public struct Column
        {
            /// <summary> Name of the column </summary>
            public string Name;

            /// <summary> DataType of the column</summary>
            public string Datatype;

            /// -------------------------------------------------------------------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="name"> Name of the column </param>
            /// <param name="datatype"> DataType of the column</param>
            public Column(string name, string datatype)
            {
                Name = name;
                Datatype = datatype;
            }
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Enum to select between CREATE DATABASE and CREATE TABLE
        /// </summary>
        public enum CreateMode
        {
            /// <summary> TABLE </summary>
            Table,
            /// <summary> DATABASE </summary>
            Database,
        }
    }
}
