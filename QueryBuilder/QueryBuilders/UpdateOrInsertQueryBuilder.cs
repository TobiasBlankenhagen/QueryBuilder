using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Library.QueryBuilder.QueryBuilders
{
    /// ============================================================================================================================
    /// <summary>
    /// Base Class for Update and Insert QueryBuilders
    /// </summary>
    public class UpdateOrInsertQueryBuilder
    {
        /// <summary> Has the Fields and Values to Insert, Update</summary>
        protected List<FieldValuePair> fieldValuePairs = new List<FieldValuePair>();
        /// <summary> Has the Table to Update, in which to Insert</summary>
        protected string table;
        /// <summary> Database provider for command creation</summary>
        protected DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the dbProviderFactory to factory
        /// </summary>
        /// <param name="factory"> Database provider for command creation</param>
        public void SetDbProviderFactory(DbProviderFactory factory)
        {
            dbProviderFactory = factory;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the value of a existing field in the list or adds it.
        /// </summary>
        /// <param name="fieldName"> The field to update or add to the list</param>
        /// <param name="fieldValue"> The value for the field</param>
        public void SetField(string fieldName, object fieldValue)
        {
            bool flag = false;
            foreach (FieldValuePair fieldValuePair in this.fieldValuePairs)
            {
                if (fieldValuePair.FieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                {
                    fieldValuePair.Value = fieldValue;
                    flag = true;
                    break;
                }
            }
            if (flag)
                return;
            this.fieldValuePairs.Add(new FieldValuePair(fieldName, fieldValue));
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the table for the update or insert query
        /// </summary>
        /// <param name="table"> Tablename for the query</param>
        public void SetTable(string table)
        {
            this.table = table;
        }

        /// -------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a Parameter to a DbCommand
        /// </summary>
        /// <param name="dbCommand"> The command to add the parameter to</param>
        /// <param name="field"> The field the parameter stands for</param>
        /// <returns> The added parameter</returns>
        protected DbParameter AddParameterToCommand(ref DbCommand dbCommand, string field)
        {
            string str = string.Format("@p{0}_{1}", (object)(dbCommand.Parameters.Count + 1), (object)field.Replace('.', '_'));
            DbParameter parameter = dbCommand.CreateParameter();
            parameter.ParameterName = str;
            dbCommand.Parameters.Add((object)parameter);
            return parameter;
        }

        /// ============================================================================================================================
        /// <summary>
        /// Informationsclass for Insert and UpdateQueryBuilders
        /// </summary>
        protected class FieldValuePair
        {
            /// <summary> Name of the Database field</summary>
            public string FieldName;
            /// <summary> Value for the Database field</summary>
            public object Value;

            /// -------------------------------------------------------------------------------------------------------------------------
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="fieldName"> Name of the Database field</param>
            /// <param name="fieldValue"> Value for the Database field</param>
            public FieldValuePair(string fieldName, object fieldValue)
            {
                this.FieldName = fieldName;
                this.Value = fieldValue;
            }
        }
    }
}
