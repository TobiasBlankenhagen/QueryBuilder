using Library.QueryBuilder;
using Library.QueryBuilder.QueryBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlQueryFrameworkTests
{
    [TestClass()]    
    public class DeleteBuildQueryTests
    {
        [TestMethod]
        public void DeleteBuildQueryTest()
        {
            string sql = "DELETE FROM Text";
            DeleteQueryBuilder dqb = new DeleteQueryBuilder();
            dqb.SetTable("Text");
            dqb.DeleteAll = true;            
            Assert.AreEqual(sql, dqb.BuildQuery());
        }

        [TestMethod]
        public void DeleteBuildQueryWhereTest()
        {
            string sql = "DELETE FROM Text WHERE Id = 1";
            DeleteQueryBuilder dqb = new DeleteQueryBuilder();
            dqb.SetTable("Text");
            dqb.AddWhere("Id", Comparison.Equals, 1);

            Assert.AreEqual(sql, dqb.BuildQuery());
        }

        [TestMethod]
        public void DeleteBuildQueryWhereType()
        {
            string sql = "DELETE FROM Codes WHERE Id = 1 OR Id = 2";
            DeleteQueryBuilder dqb = new DeleteQueryBuilder();
            dqb.SetTable("Codes");
            dqb.WhereStatement.StatementType = LogicOperator.OR;
            dqb.AddWhere("Id", Comparison.Equals, 1);
            dqb.AddWhere("Id", Comparison.Equals, 2);

            Assert.AreEqual(sql, dqb.BuildQuery());
        }
    }
}
