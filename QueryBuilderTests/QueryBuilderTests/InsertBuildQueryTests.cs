using System.Data.Common;
using Library.QueryBuilder.QueryBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlQueryFrameworkTests
{
    [TestClass]
    public class InsertBuildQueryTests
    {
        [TestMethod]
        public void InsertQueryTest()
        {
            string sql = "INSERT INTO Customers (CustomerName, Address) VALUES ('Cardinal', 'Skagen 21')";
            InsertQueryBuilder iqb = new InsertQueryBuilder();
            iqb.SetTable("Customers");
            iqb.SetField("CustomerName", "Cardinal");
            iqb.SetField("Address", "Skagen 21");


            Assert.AreEqual(sql, iqb.BuildQuery());
        }

        [TestMethod]
        public void InsertCommandTest()
        {
            DbCommand dbCommand = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            dbCommand.CommandText= "INSERT INTO Customers (CustomerName, Address) VALUES (@p1_CustomerName, @p2_Address)";
            DbParameter param1= dbCommand.CreateParameter();
            param1.ParameterName = "@p1_CustomerName";
            param1.Value = "Cardinal";
            dbCommand.Parameters.Add(param1);
            DbParameter param2 = dbCommand.CreateParameter();
            param2.ParameterName = "@p2_Address";
            param2.Value = "Skagen 21";
            dbCommand.Parameters.Add(param2);

            InsertQueryBuilder iqb = new InsertQueryBuilder("Customers");
            iqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            iqb.SetField("CustomerName", "Cardinal");
            iqb.SetField("Address", "Skagen 21");

            DbCommand command = iqb.BuildCommand();
            
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
    }
}
