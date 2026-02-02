using Library.QueryBuilder;
using Library.QueryBuilder.QueryBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlQueryFrameworkTests
{
    [TestClass]
    public class CreateBuildQueryTest
    {
        [TestMethod]
        public void CreateDatabaseTest()
        {
            string sql = "CREATE DATABASE test_Create;";
            CreateQueryBuilder cqb = new CreateQueryBuilder(CreateQueryBuilder.CreateMode.Database, "test_Create");
            
            Assert.AreEqual(sql, cqb.BuildCommand().CommandText);
        }

        [TestMethod]
        public void CreateTableTest()
        {
            string sql = "CREATE TABLE Table (Id char(32));";
            CreateQueryBuilder cqb = new CreateQueryBuilder(CreateQueryBuilder.CreateMode.Table, "Table");
            cqb.Columns.Add(new CreateQueryBuilder.Column("Id", "char(32)"));

            Assert.AreEqual(sql, cqb.BuildQuery());
        }

        [TestMethod]
        public void CreateTablePrimaryKeyTest()
        {
            string sql = "CREATE TABLE Table (Id char(32)) CONSTRAINT PK0_Id PRIMARY KEY (Id);";
            CreateQueryBuilder cqb = new CreateQueryBuilder(CreateQueryBuilder.CreateMode.Table, "Table");
            cqb.Columns.Add(new CreateQueryBuilder.Column("Id", "char(32)"));
            cqb.AddPrimaryKey("Id");

            Assert.AreEqual(sql, cqb.BuildQuery());
        }

        [TestMethod]
        public void CreateTableForeignKeyTest()
        {
            string sql = "CREATE TABLE Table (Id char(32)) CONSTRAINT FK0_PersonsTable FOREIGN KEY (Id) REFERENCES Persons(PersonId);";
            CreateQueryBuilder cqb = new CreateQueryBuilder(CreateQueryBuilder.CreateMode.Table, "Table");
            cqb.Columns.Add(new CreateQueryBuilder.Column("Id", "char(32)"));
            cqb.AddForeignKey("Id","Persons", "PersonId");
            

            Assert.AreEqual(sql, cqb.BuildQuery());
        }

        [TestMethod]
        public void CreateTableCheckTest()
        {
            string sql = "CREATE TABLE Table (Id char(32)) CONSTRAINT CHK0_Table CHECK ( (Age >= 18 AND City = 'Sandnes'));";
            CreateQueryBuilder cqb = new CreateQueryBuilder(CreateQueryBuilder.CreateMode.Table, "Table");
            cqb.Columns.Add(new CreateQueryBuilder.Column("Id", "char(32)"));
            WhereStatement where = new WhereStatement();            
            where.CombineClauses(where.Add("Age", Comparison.GreaterOrEquals, 18), LogicOperator.AND, where.Add("City", Comparison.Equals, "Sandnes"));
            cqb.AddCheck(where);

            Assert.AreEqual(sql, cqb.BuildQuery());
        }

        [TestMethod]
        public void CreateTableCheck2Test()
        {
            string sql = "CREATE TABLE Table (Id char(32)) CONSTRAINT CHK0_Table CHECK ( Age >= 18);";
            CreateQueryBuilder cqb = new CreateQueryBuilder(CreateQueryBuilder.CreateMode.Table, "Table");
            cqb.Columns.Add(new CreateQueryBuilder.Column("Id", "char(32)"));
            WhereStatement where = new WhereStatement();
            where.Add("Age", Comparison.GreaterOrEquals, 18);
            cqb.AddCheck(where);

            Assert.AreEqual(sql, cqb.BuildQuery());
        }

        [TestMethod]
        public void CreateTableUnique()
        {
            string sql = "CREATE TABLE Table (Id char(32), Name nvarchar(255)) CONSTRAINT UC0_Table UNIQUE (Id, Name);";
            CreateQueryBuilder cqb = new CreateQueryBuilder(CreateQueryBuilder.CreateMode.Table, "Table");
            cqb.Columns.Add(new CreateQueryBuilder.Column("Id", "char(32)"));
            cqb.Columns.Add(new CreateQueryBuilder.Column("Name", "nvarchar(255)"));
            cqb.AddUnique(new string[] { "Id", "Name" });

            Assert.AreEqual(sql, cqb.BuildQuery());
        }
    }
}
