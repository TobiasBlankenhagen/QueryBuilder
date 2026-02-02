using System.Collections.Generic;
using System.Data.Common;
using Library.QueryBuilder;
using Library.QueryBuilder.QueryBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlQueryFrameworkTests
{
    [TestClass]
    public class SelectBuildCommandTests
    {
        [TestMethod]
        public void BuildCommandTest()
        {
            DbCommand dbCommand = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            dbCommand.CommandText = "SELECT * FROM Customer WHERE Id = @p1_Id AND Town = @p2_Town";
            DbParameter param = dbCommand.CreateParameter();
            param.ParameterName = "@p1_Id";
            param.Value = 1;
            dbCommand.Parameters.Add(param);
            param = dbCommand.CreateParameter();
            param.ParameterName = "@p2_Town";
            param.Value = "Berlin";
            dbCommand.Parameters.Add(param);

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customer");
            sqb.AddWhere("Id", Comparison.Equals, 1);
            sqb.AddWhere("Town", Comparison.Equals, "Berlin");
            DbCommand command = sqb.BuildCommand();


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
        public void BuildCommandJoinSubTest()
        {
            string sql = "SELECT Customers.CustomerId, Customers.CustomerName, Customers.CustomerAdressStreet, Customers.CustomerAdressTown FROM Customers JOIN (SELECT CustomerName, CustomerAdressStreet, CustomerAdressTown FROM Customers GROUP BY CustomerName, CustomerAdressStreet, CustomerAdressTown HAVING COUNT(CustomerName) > 1)SubSelect ON SubSelect.CustomerName = Customers.CustomerName ORDER BY CustomerName DESC";

            SelectQueryBuilder subselect = new SelectQueryBuilder(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            subselect.SelectColumns("CustomerName", "CustomerAdressStreet", "CustomerAdressTown");
            subselect.SelectFromTable("Customers");
            subselect.AddGroupBy("CustomerName", "CustomerAdressStreet", "CustomerAdressTown");
            subselect.AddHavingClause("COUNT(CustomerName)", Comparison.GreaterThan, 1);

            SelectQueryBuilder sqb = new SelectQueryBuilder(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectColumns("Customers.CustomerId", "Customers.CustomerName", "Customers.CustomerAdressStreet", "Customers.CustomerAdressTown");
            sqb.SelectFromTable("Customers");

            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, new SubSelectQueryBuilder(subselect, "SubSelect"), "CustomerName", Comparison.Equals, "Customers", "CustomerName"));
            sqb.AddOrderBy("CustomerName", Sorting.Descending);
            DbCommand dbCommand = sqb.BuildCommand();

            Assert.AreEqual(sql, dbCommand.CommandText);
        }

        [TestMethod]
        public void BuildCommandWhereCombinedBetweenTest()
        {
            DbCommand dbCommand = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            dbCommand.CommandText = "SELECT * FROM Customer WHERE (Id BETWEEN @p1_Id AND @p2_Id OR Id NOT BETWEEN @p3_Id AND @p4_Id)";
            DbParameter param = dbCommand.CreateParameter();
            param.ParameterName = "@p1_Id";
            param.Value = 1;
            dbCommand.Parameters.Add(param);
            param = dbCommand.CreateParameter();
            param.ParameterName = "@p2_Id";
            param.Value = 10;
            dbCommand.Parameters.Add(param);
            param = dbCommand.CreateParameter();
            param.ParameterName = "@p3_Id";
            param.Value = 11;
            dbCommand.Parameters.Add(param);
            param = dbCommand.CreateParameter();
            param.ParameterName = "@p4_Id";
            param.Value = 20;
            dbCommand.Parameters.Add(param);
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customer");
            sqb.WhereStatement.CombineClauses(sqb.AddWhere("Id", Comparison.Between, new int[2] { 1, 10 }), LogicOperator.OR, sqb.AddWhere("Id", Comparison.NotBetween, new int[2] { 11, 20 }));
            DbCommand command = sqb.BuildCommand();


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
        public void BuildCommandWhereCombinedequalTest()
        {
            DbCommand dbCommand = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            dbCommand.CommandText = "SELECT * FROM Customer WHERE (Id >= @p1_Id OR Id <= @p2_Id)";
            DbParameter param = dbCommand.CreateParameter();
            param.ParameterName = "@p1_Id";
            param.Value = 10;
            dbCommand.Parameters.Add(param);
            param = dbCommand.CreateParameter();
            param.ParameterName = "@p2_Id";
            param.Value = 20;
            dbCommand.Parameters.Add(param);
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customer");
            sqb.WhereStatement.CombineClauses(sqb.AddWhere("Id", Comparison.GreaterOrEquals, 10), LogicOperator.OR, sqb.AddWhere("Id", Comparison.LessOrEquals, 20));
            DbCommand command = sqb.BuildCommand();


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
        public void BuildCommandMultipleJoins()
        {
            string sql = "SELECT sub10_19.Sex, Age10_19, Age20_29 FROM (SELECT Sex, Count(Age) AS Age10_19 FROM Patients WHERE Age BETWEEN 10 AND 19 GROUP BY Sex) sub10_19 FULL OUTER JOIN (SELECT Sex, Count(Age) AS Age20_29 FROM Patients WHERE Age BETWEEN 20 AND 29 GROUP BY Sex)sub20_29 ON sub20_29.Sex = sub10_19.Sex FULL OUTER JOIN (SELECT Sex, Count(Age) AS Age30_39 FROM Patients WHERE Age BETWEEN 30 AND 39 GROUP BY Sex)sub30_39 ON sub30_39.Sex = sub20_29.Sex";

            SubSelectQueryBuilder sub10_19 = new SubSelectQueryBuilder("sub10_19");
            sub10_19.SelectColumns("Sex", "Count(Age) AS Age10_19");
            sub10_19.SelectFromTable("Patients");
            sub10_19.AddWhere("Age", Comparison.Between, new int[] { 10, 19 });
            sub10_19.AddGroupBy("Sex");

            SubSelectQueryBuilder sub20_29 = new SubSelectQueryBuilder("sub20_29");
            sub20_29.SelectColumns("Sex", "Count(Age) AS Age20_29");
            sub20_29.SelectFromTable("Patients");
            sub20_29.AddWhere("Age", Comparison.Between, new int[] { 20, 29 });
            sub20_29.AddGroupBy("Sex");

            SubSelectQueryBuilder sub30_39 = new SubSelectQueryBuilder("sub30_39");
            sub30_39.SelectColumns("Sex", "Count(Age) AS Age30_39");
            sub30_39.SelectFromTable("Patients");
            sub30_39.AddWhere("Age", Comparison.Between, new int[] { 30, 39 });
            sub30_39.AddGroupBy("Sex");

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectColumns(sub10_19.Alias + ".Sex", "Age10_19", "Age20_29");
            sqb.SelectFromTable(sub10_19);
            sqb.AddJoin(new JoinClause(JoinType.FullJoin, sub20_29, "Sex", Comparison.Equals, sub10_19.Alias, "Sex"));
            sqb.AddJoin(new JoinClause(JoinType.FullJoin, sub30_39, "Sex", Comparison.Equals, sub20_29.Alias, "Sex"));

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildCommandInInt()
        {
            DbCommand dbCommand = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            dbCommand.CommandText = "SELECT * FROM Customer WHERE Id IN (@p1_Id) AND Id IN (@p2_Id)";
            DbParameter param = dbCommand.CreateParameter();
            param.ParameterName = "@p1_Id";
            param.Value = "1,2,3,4";
            dbCommand.Parameters.Add(param);
            param = dbCommand.CreateParameter();
            param.ParameterName = "@p2_Id";
            param.Value = "1,2,3,4";
            dbCommand.Parameters.Add(param);
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customer");
            sqb.AddWhere("Id", Comparison.In, new int[] { 1, 2, 3, 4 });
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            sqb.AddWhere("Id", Comparison.In, list.ToArray());
            DbCommand command = sqb.BuildCommand();


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
        public void BuildCommandInString()
        {
            DbCommand dbCommand = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            dbCommand.CommandText = "SELECT * FROM Customer WHERE Id IN (@p1_Id)";
            DbParameter param = dbCommand.CreateParameter();
            param.ParameterName = "@p1_Id";
            param.Value = "'622f71eb-c5cb-4eb4-aa46-002b065a88f3','a38ce570-73d3-4d67-9b39-007bc5e30688','c64e6c1d-8d04-4729-91e8-00d97ef76bd9','67afe86e-6b67-4b3c-b792-00e7bda6d803'";
            dbCommand.Parameters.Add(param);
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customer");
            sqb.AddWhere("Id", Comparison.In, new string[] { "622f71eb-c5cb-4eb4-aa46-002b065a88f3", "a38ce570-73d3-4d67-9b39-007bc5e30688", "c64e6c1d-8d04-4729-91e8-00d97ef76bd9", "67afe86e-6b67-4b3c-b792-00e7bda6d803" });
            DbCommand command = sqb.BuildCommand();


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
        public void BuildCommandCombinedInInt()
        {
            DbCommand dbCommand = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            dbCommand.CommandText = "SELECT * FROM Customer WHERE (Id IN (@p1_Id) OR Id = @p2_Id)";
            DbParameter param = dbCommand.CreateParameter();
            param.ParameterName = "@p1_Id";
            param.Value = "1,2,3,4";
            dbCommand.Parameters.Add(param);
            param = dbCommand.CreateParameter();
            param.ParameterName = "@p2_Id";
            param.Value = 1;
            dbCommand.Parameters.Add(param);
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customer");
            sqb.WhereStatement.CombineClauses(sqb.AddWhere("Id", Comparison.In, new int[] { 1, 2, 3, 4 }), LogicOperator.OR, sqb.AddWhere("Id", Comparison.Equals, 1));
            DbCommand command = sqb.BuildCommand();


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
        public void BuildCommandSqlLIteral()
        {
            DbCommand dbCommand = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            dbCommand.CommandText = "SELECT p.ID FROM Patient_1 p WHERE CONVERT(date, p.CDate) = @p1_CONVERT(date, p_CDate)";
            DbParameter param = dbCommand.CreateParameter();
            param.ParameterName = "@p1_CONVERT(date, p_CDate)";
            param.Value = "CAST(GETDATE() AS DATE)";
            dbCommand.Parameters.Add(param);

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Patient_1", "p");
            sqb.SelectColumn("p.ID");
            sqb.AddWhere(new SqlLiteral("CONVERT(date, p.CDate)"), Comparison.Equals, new SqlLiteral("CAST(GETDATE() AS DATE)"));
            DbCommand command = sqb.BuildCommand();

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
