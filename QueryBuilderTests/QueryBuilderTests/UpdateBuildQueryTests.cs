using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Library.QueryBuilder;
using Library.QueryBuilder.QueryBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlQueryFrameworkTests
{
    [TestClass]
    public class UpdateBuildQueryTests
    {
        [TestMethod]
        public void UpdateQuery()
        {
            string sql = "UPDATE Customers SET ContactName = 'Alfred Schmidt', City = 'Frankfurt'";
            UpdateQueryBuilder uqb = new UpdateQueryBuilder();
            uqb.SetTable("Customers");
            uqb.SetField("ContactName", "Alfred Schmidt");
            uqb.SetField("City", "Frankfurt");

            Assert.AreEqual(sql, uqb.BuildQuery());
        }

        [TestMethod]
        public void UpdateQueryWhere()
        {
            string sql = "UPDATE Customers SET ContactName = 'Alfred Schmidt', City = 'Frankfurt' WHERE CustomerID = 1";
            UpdateQueryBuilder uqb = new UpdateQueryBuilder();
            uqb.SetTable("Customers");
            uqb.SetField("ContactName", "Alfred Schmidt");
            uqb.SetField("City", "Frankfurt");
            uqb.AddWhere("CustomerID", Comparison.Equals, 1);

            Assert.AreEqual(sql, uqb.BuildQuery());
        }

        [TestMethod]
        public void UpdateCommand()
        {
            DbCommand dbCommand = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            dbCommand.CommandText = "UPDATE Customers SET ContactName = @p1_ContactName, Age = @p2_Age WHERE CustomerId = @p3_CustomerId";
            DbParameter param = dbCommand.CreateParameter();
            param.ParameterName = "@p1_ContactName";
            param.Value = "Alfred Schmidt";
            dbCommand.Parameters.Add(param);
            param = dbCommand.CreateParameter();
            param.ParameterName = "@p2_Age";
            param.Value = 40;
            dbCommand.Parameters.Add(param);
            param = dbCommand.CreateParameter();
            param.ParameterName = "@p3_CustomerId";
            param.Value = 1;
            dbCommand.Parameters.Add(param);

            UpdateQueryBuilder uqb = new UpdateQueryBuilder(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            uqb.SetTable("Customers");
            uqb.SetField("ContactName", "Alfred Schmidt");
            uqb.SetField("Age", 40);            
            uqb.AddWhere("CustomerId", Comparison.Equals, 1);
            DbCommand command = uqb.BuildCommand();


            bool same = true;
            if (command.CommandText.Equals(dbCommand.CommandText))
            {
                for (int index = 0; index < dbCommand.Parameters.Count; index++)
                {
                    if (dbCommand.Parameters[index].ParameterName.Equals(command.Parameters[index].ParameterName))
                        if (dbCommand.Parameters[index].Value.Equals(command.Parameters[index].Value))
                            same = true;
                        else
                            same = false;
                    else
                        same = false;
                }
            }
            else
                same = false;
            Assert.IsTrue(same);
        }

        [TestMethod]
        public void MyTestMethod()
        {
            string sql = "UPDATE MessageRecipients SET Received = 1, Modified = '2018-09-18 16:54:27.312', ModifierId = 1 WHERE Id = 25 OR Id = 26";

            List<int> recipientsGuids = new List<int>();
            recipientsGuids.Add(25); recipientsGuids.Add(26);
            UpdateQueryBuilder uqb = new UpdateQueryBuilder(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            uqb.SetTable("MessageRecipients");
            uqb.SetField("Received", true);
            uqb.SetField("Modified", "2018-09-18 16:54:27.312");
            uqb.SetField("ModifierId", 1);
            WhereStatement where = new WhereStatement();
            where.StatementType = LogicOperator.OR;
            foreach (int id in recipientsGuids)
            {
                where.Add("Id", Comparison.Equals, id);
            }
            uqb.WhereStatement = where;
            
            string update = uqb.BuildQuery();
            SqlCommand sqlCommand = uqb.BuildCommand() as SqlCommand;
            Assert.AreEqual(sql, update);
        }

    }
}
