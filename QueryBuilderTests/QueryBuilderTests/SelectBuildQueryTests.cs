using System;
using System.Data.Common;
using Library.QueryBuilder;
using Library.QueryBuilder.QueryBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlQueryFrameworkTests
{
    [TestClass()]
    public class SelectQueryBuilderTests
    {
        [TestMethod()]
        public void BuildQueryTestSelectAll()
        {
            string sql = "SELECT * FROM Customer";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customer");

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod()]
        public void BuildQueryTestSelectOneColumn()
        {
            string sql = "SELECT Name FROM Customer";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectColumn("Name");
            sqb.SelectFromTable("Customer");

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod()]
        public void BuildQueryTestSelectTwoColumns()
        {
            string sql = "SELECT Name, Town FROM Customer";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectColumns(new[] { "Name", "Town" });
            sqb.SelectFromTable("Customer");

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod()]
        public void BuildQueryTestSelectAllDistinct()
        {
            string sql = "SELECT DISTINCT * FROM Customer";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.Distinct = true;
            sqb.SelectFromTable("Customer");

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod()]
        public void BuildQueryTest()
        {
            string sql = "SELECT Id FROM Customer";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            //sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));            
            sqb.SelectColumn("Id");
            sqb.SelectFromTables("Customer");
            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod()]
        public void BuildQueryTestSelectAllWhere()
        {
            string sql = "SELECT * FROM Customer WHERE (Id = 1 OR Name LIKE 'hans') AND Kunde LIKE 'Stamm'";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customer");
            WhereStatement where = new WhereStatement();
            WhereStatement.Clause clauseId = where.Add("Id", Comparison.Equals, 1);
            WhereStatement.Clause clauseName = where.Add("Name", Comparison.Like, "hans");
            where.CombineClauses(clauseId, LogicOperator.OR, clauseName);
            sqb.WhereStatement = where;
            sqb.AddWhere("Kunde", Comparison.Like, "Stamm");
            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod()]
        public void BuildQueryTestSelectAllWhereCombined()
        {
            string sql = "SELECT * FROM Customer WHERE (Id <= 100 AND Kunde LIKE 'Stamm') AND (FirstName LIKE 'V%' OR LastName LIKE 'V%') AND Town = 'Berlin'";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customer");
            WhereStatement.Clause clauseId = sqb.WhereStatement.Add("Id", Comparison.LessOrEquals, 100);
            WhereStatement.Clause clauseStamm = sqb.WhereStatement.Add("Kunde", Comparison.Like, "Stamm");
            WhereStatement.Clause clauseFirstName = sqb.WhereStatement.Add("FirstName", Comparison.Like, "V%");
            WhereStatement.Clause clauseLastName = sqb.WhereStatement.Add("LastName", Comparison.Like, "V%");
            WhereStatement.Clause clauseTown = sqb.WhereStatement.Add("Town", Comparison.Equals, "Berlin");
            sqb.WhereStatement.CombineClauses(clauseId, LogicOperator.AND, clauseStamm);
            sqb.WhereStatement.CombineClauses(clauseFirstName, LogicOperator.OR, clauseLastName);

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQuerySelectAllWhereCombinedCombinedTest()
        {
            string sql = "SELECT * FROM WfMS_Messages WHERE ( (Receiver = 10 AND ReceiverTyp = 0) OR (Receiver = 2 AND ReceiverTyp = 1))";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("WfMS_Messages");
            sqb.SelectAllColumns();
            WhereStatement.CombinedClause userclause = sqb.WhereStatement.CombineClauses(sqb.AddWhere("Receiver", Comparison.Equals, 10), LogicOperator.AND, sqb.AddWhere("ReceiverTyp", Comparison.Equals, 0));
            WhereStatement.CombinedClause workstationclause = sqb.WhereStatement.CombineClauses(sqb.AddWhere("Receiver", Comparison.Equals, 2), LogicOperator.AND, sqb.AddWhere("ReceiverTyp", Comparison.Equals, 1));
            sqb.WhereStatement.CombineClauses(userclause, LogicOperator.OR, workstationclause);
            string sqbquery = sqb.BuildQuery();
            Assert.AreEqual(sql, sqbquery);
        }
        [TestMethod()]
        public void BuildQueryTestSelectAllJoinId()
        {
            string sql = "SELECT * FROM Customers JOIN Installog ON Installog.Id = Customers.CustomerId WHERE Town = 'Berlin'";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customers");
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, "Installog", "Id", Comparison.Equals, "Customers", "CustomerId"));
            WhereStatement.Clause clauseTown = sqb.WhereStatement.Add("Town", Comparison.Equals, "Berlin");

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQueryTestSelectCountGroupHaving()
        {
            string sql = "SELECT COUNT(1), Country FROM Customers GROUP BY Country HAVING COUNT(CustomerId) > 5";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectColumns("COUNT(1)", "Country");
            sqb.AddGroupBy("Country");
            sqb.HavingStatement = new WhereStatement("COUNT(CustomerId)", Comparison.GreaterThan, 5);
            sqb.SelectFromTable("Customers");

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQuerySelectAllTop()
        {
            string sql = "SELECT TOP 3 PERCENT * FROM Customers";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.TopClause = new TopClause(3, TopUnit.Percent);
            sqb.SelectFromTable("Customers");

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQuerySelectAllTop2()
        {
            string sql = "SELECT TOP 3 * FROM Customers";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SetDbProviderFactory(DbProviderFactories.GetFactory("System.Data.SqlClient"));
            sqb.SelectAllColumns();
            sqb.TopClause = new TopClause(3);
            sqb.SelectFromTable("Customers");

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQuerySelectAllWhereIn()
        {
            string sql = "SELECT * FROM Customers WHERE Country IN ('Germany', 'France')";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectAllColumns();
            sqb.SelectFromTable("Customers");
            sqb.AddWhere("Country", Comparison.In, new string[] { "Germany", "France" });

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQuerySelectColumnRightJoin()
        {
            string sql = "SELECT COUNT(1) FROM Customers RIGHT JOIN Towns ON Towns.Id LIKE (SELECT * FROM Customers).Id";

            SubSelectQueryBuilder sub = new SubSelectQueryBuilder("Subselect");
            sub.SelectAllColumns();
            sub.SelectFromTable("Customers");

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectCount();
            sqb.SelectFromTable("Customers");
            sqb.AddJoin(new JoinClause(JoinType.RightJoin, "Towns", "Id", Comparison.Like, sub, "Id"));

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQuerySelectColumnLeftJoin()
        {
            string sql = "SELECT COUNT(1) FROM Customers LEFT JOIN Towns ON Towns.Id NOT LIKE (SELECT * FROM Customers).Id";

            SubSelectQueryBuilder sub = new SubSelectQueryBuilder("Subselect");
            sub.SelectAllColumns();
            sub.SelectFromTable("Customers");

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectCount();
            sqb.SelectFromTable("Customers");
            sqb.AddJoin(new JoinClause(JoinType.LeftJoin, "Towns", "Id", Comparison.NotLike, sub, "Id"));

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQuerySelectColumnFullJoin()
        {
            string sql = "SELECT COUNT(1) FROM Customers FULL OUTER JOIN Towns ON Towns.Id != (SELECT * FROM Customers).Id";

            SubSelectQueryBuilder sub = new SubSelectQueryBuilder("Subselect");
            sub.SelectAllColumns();
            sub.SelectFromTable("Customers");

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectCount();
            sqb.SelectFromTable("Customers");
            sqb.AddJoin(new JoinClause(JoinType.FullJoin, "Towns", "Id", Comparison.NotEquals, sub, "Id"));

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQueryJoinAlias()
        {
            string sql = "SELECT p.ID, p.DOB, n.GivenName, a.AddressLine1 " +
                            "FROM Patient_1 p " +
                            "JOIN PatientNames_1 n ON n.PatientID = p.ID " +
                            "JOIN AddressInfo_1 a ON a.PatientID = p.ID " +
                            "WHERE p.ID IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10) " +
                            "AND n.Language = 'US' " +
                            "AND a.AddressType = 2";

            string patients = "Patient_1";
            string patientNames = "PatientNames_1";
            string patientadresses = "AddressInfo_1";
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable(patients, "p");
            sqb.SelectColumns("p.ID", "p.DOB", "n.GivenName", "a.AddressLine1");
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, patientNames, "n", "PatientID", Comparison.Equals, "p", "ID"));
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, patientadresses, "a", "PatientID", Comparison.Equals, "p", "ID"));
            int[] wherValues = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            sqb.AddWhere("p.ID", Comparison.In, wherValues);
            sqb.AddWhere("n.Language", Comparison.Equals, "US");
            sqb.AddWhere("a.AddressType", Comparison.Equals, 2);

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQueryJoinSubQuery()
        {
            string sql = "SELECT p.ID, p.DOB, n.GivenName " +
                            "FROM Patient_1 p " +
                            "FULL OUTER JOIN PatientNames_1 n ON n.PatientID > (SELECT * FROM Patient_1).ID " +
                            "JOIN (SELECT * FROM Patient_1)sub ON sub.ID >= (SELECT * FROM Patient_1).ID";

            string patients = "Patient_1";
            string patientNames = "PatientNames_1";
            SubSelectQueryBuilder subqb = new SubSelectQueryBuilder("sub");
            subqb.SelectFromTable(patients);
            subqb.SelectAllColumns();
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable(patients, "p");
            sqb.SelectColumns("p.ID", "p.DOB", "n.GivenName");
            sqb.AddJoin(new JoinClause(JoinType.FullJoin, patientNames, "n", "PatientID", Comparison.GreaterThan, subqb, "ID"));
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, subqb, "ID", Comparison.GreaterOrEquals, subqb, "ID"));

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQueryJoin()
        {
            string sql = "SELECT * FROM Patient " +
                            "JOIN Names ON Names.Id < Patient.Id " +
                            "JOIN Names ON Names.Id <= Patient.Id " +
                            "JOIN Names ON Names.Id IN Patient.Id " +
                            "JOIN Names ON Names.Id NOT IN Patient.Id " +
                            "JOIN Names ON Names.Id BETWEEN Patient.Id " +
                            "JOIN Names ON Names.Id NOT BETWEEN Patient.Id";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Patient");
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, "Names", "Id", Comparison.LessThan, "Patient", "Id"));
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, "Names", "Id", Comparison.LessOrEquals, "Patient", "Id"));
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, "Names", "Id", Comparison.In, "Patient", "Id"));
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, "Names", "Id", Comparison.NotIn, "Patient", "Id"));
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, "Names", "Id", Comparison.Between, "Patient", "Id"));
            sqb.AddJoin(new JoinClause(JoinType.InnerJoin, "Names", "Id", Comparison.NotBetween, "Patient", "Id"));

            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQuerySqlLiteralsTest()
        {
            string sql = "SELECT p.ID FROM Patient_1 p WHERE CONVERT(date, p.CDate) = CAST(GETDATE() AS DATE)";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Patient_1", "p");
            sqb.SelectColumn("p.ID");
            sqb.AddWhere(new SqlLiteral("CONVERT(date, p.CDate)"), Comparison.Equals, new SqlLiteral("CAST(GETDATE() AS DATE)"));
            string sqbSql = sqb.BuildQuery();

            Assert.AreEqual(sql, sqbSql);
        }

        [TestMethod]
        public void BuildQueryBetweenDateTime()
        {
            string sql = "SELECT * FROM Test T WHERE CDate BETWEEN '1990-12-30 20:01:02' AND '1991-03-14 13:14:14'";

            SelectQueryBuilder sqb = new SelectQueryBuilder();

            sqb.SelectFromTable("Test", "T");
            DateTime?[] dateArr = new DateTime?[] { new DateTime(1990, 12, 30, 20, 01, 02), new DateTime(1991, 03, 14, 13, 14, 14) };
            sqb.AddWhere("CDate", Comparison.Between, dateArr);
            string query = sqb.BuildQuery();
            Assert.AreEqual(sql, query);
        }

        [TestMethod]
        public void BuildQueryComplicatedWhere()
        {
            string sql = "SELECT * FROM Table WHERE ( ( ( (COndi1 = '2' AND COndi2 = '2') AND COndi3 = '2') AND NOT (Condi4 = 'as' AND Condi5 = 'as')) OR (Condi6 = 'llkjkl' OR Condi7 = '54235'))";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Table");
            WhereStatement inclusive = new WhereStatement();
            inclusive.Add("COndi1", Comparison.Equals, "2");
            inclusive.Add("COndi2", Comparison.Equals, "2");
            inclusive.Add("COndi3", Comparison.Equals, "2");


            WhereStatement exclusive = new WhereStatement();
            exclusive.Add("Condi4", Comparison.Equals, "as");
            exclusive.Add("Condi5", Comparison.Equals, "as");


            WhereStatement optional = new WhereStatement();
            optional.Add("Condi6", Comparison.Equals, "llkjkl");
            optional.Add("Condi7", Comparison.Equals, "54235");


            WhereStatement.CombinedClause inclusiveCOmbined = inclusive.CombineAllClauses(LogicOperator.AND);
            WhereStatement.CombinedClause optionalCombined = optional.CombineAllClauses(LogicOperator.OR);
            WhereStatement.CombinedClause exclusiveCombined = exclusive.CombineAllClauses(LogicOperator.AND);
            WhereStatement.CombinedClause combined = sqb.WhereStatement.CombineClauses(inclusiveCOmbined, LogicOperator.ANDNOT, exclusiveCombined);
            sqb.WhereStatement.CombineClauses(combined, LogicOperator.OR, optionalCombined);
            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQueryNoInclusive()
        {
            string sql = "SELECT * FROM Table WHERE ( ( NOT (exc1 = '2' AND exc2 = '2')) OR (opt1 = '2' OR opt2 = '2'))";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Table");
            WhereStatement inclusiveAnd = new WhereStatement();
            inclusiveAnd.StatementType = LogicOperator.AND;

            WhereStatement inclusiveOr = new WhereStatement();
            inclusiveOr.StatementType = LogicOperator.OR;

            WhereStatement exclusive = new WhereStatement();
            exclusive.StatementType = LogicOperator.AND;
            exclusive.Add("exc1", Comparison.Equals, "2");
            exclusive.Add("exc2", Comparison.Equals, "2");


            WhereStatement optional = new WhereStatement();
            optional.StatementType = LogicOperator.OR;
            optional.Add("opt1", Comparison.Equals, "2");
            optional.Add("opt2", Comparison.Equals, "2");

            WhereStatement.CombinedClause inclusiveCombined = new WhereStatement.CombinedClause();
            WhereStatement.CombinedClause combined = new WhereStatement.CombinedClause();

            if (inclusiveOr.Count > 0)
            {
                inclusiveCombined = sqb.WhereStatement.CombineClauses(inclusiveAnd, LogicOperator.OR, inclusiveOr);
                combined = sqb.WhereStatement.CombineClauses(inclusiveCombined, LogicOperator.ANDNOT, exclusive);
                sqb.WhereStatement.CombineClauses(combined, LogicOperator.OR, optional);
            }
            else if (inclusiveAnd.Count > 0)
            {
                combined = sqb.WhereStatement.CombineClauses(inclusiveAnd, LogicOperator.ANDNOT, exclusive);
                sqb.WhereStatement.CombineClauses(combined, LogicOperator.OR, optional);
            }
            else
            {
                combined = sqb.WhereStatement.CombineClauses(null, LogicOperator.NOT, exclusive);
                sqb.WhereStatement.CombineClauses(combined, LogicOperator.OR, optional);
            }

            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, sqb.BuildQuery());
        }

        [TestMethod]
        public void BuildQueryOnlyOneIncOr()
        {
            string sql = "SELECT * FROM Table WHERE ( ( ( ( (incAnd1 = '2' AND incAnd2 = '2') AND incAnd3 = '2') OR incOr1 = '2') AND NOT (exc1 = '2' AND exc2 = '2')) OR (opt1 = '2' OR opt2 = '2'))";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Table");
            WhereStatement inclusiveAnd = new WhereStatement();
            inclusiveAnd.StatementType = LogicOperator.AND;
            inclusiveAnd.Add("incAnd1", Comparison.Equals, "2");
            inclusiveAnd.Add("incAnd2", Comparison.Equals, "2");
            inclusiveAnd.Add("incAnd3", Comparison.Equals, "2");
            //inclusiveAnd.Add("incAnd4", Comparison.Equals, "2");


            WhereStatement inclusiveOr = new WhereStatement();
            inclusiveOr.StatementType = LogicOperator.OR;
            inclusiveOr.Add("incOr1", Comparison.Equals, "2");

            WhereStatement exclusive = new WhereStatement();
            exclusive.StatementType = LogicOperator.AND;
            exclusive.Add("exc1", Comparison.Equals, "2");
            exclusive.Add("exc2", Comparison.Equals, "2");


            WhereStatement optional = new WhereStatement();
            optional.StatementType = LogicOperator.OR;
            optional.Add("opt1", Comparison.Equals, "2");
            optional.Add("opt2", Comparison.Equals, "2");

            WhereStatement.CombinedClause inclusiveCombined = new WhereStatement.CombinedClause();
            WhereStatement.CombinedClause combined = new WhereStatement.CombinedClause();

            if (inclusiveOr.Count > 0)
            {
                inclusiveCombined = sqb.WhereStatement.CombineClauses(inclusiveAnd, LogicOperator.OR, inclusiveOr);
                combined = sqb.WhereStatement.CombineClauses(inclusiveCombined, LogicOperator.ANDNOT, exclusive);
                sqb.WhereStatement.CombineClauses(combined, LogicOperator.OR, optional);
            }
            else if (inclusiveAnd.Count > 0)
            {
                combined = sqb.WhereStatement.CombineClauses(inclusiveAnd, LogicOperator.ANDNOT, exclusive);
                sqb.WhereStatement.CombineClauses(combined, LogicOperator.OR, optional);
            }
            else
            {
                combined = sqb.WhereStatement.CombineClauses(null, LogicOperator.NOT, exclusive);
                sqb.WhereStatement.CombineClauses(combined, LogicOperator.OR, optional);
            }

            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }

        [TestMethod]
        public void BuildQueryRealLifeTest1()
        {
            string sql = "SELECT * FROM Table WHERE (preCond1 = '1' AND NOT ( ( (incAnd1 = '2' AND incAnd2 = '2') AND incAnd3 = '2') AND incAnd4 = '2'))";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Table");
            sqb.AddWhere("preCond1", Comparison.Equals, "1");

            WhereStatement inclusiveAnd = new WhereStatement();
            inclusiveAnd.StatementType = LogicOperator.AND;
            inclusiveAnd.Add("incAnd1", Comparison.Equals, "2");
            inclusiveAnd.Add("incAnd2", Comparison.Equals, "2");
            inclusiveAnd.Add("incAnd3", Comparison.Equals, "2");
            inclusiveAnd.Add("incAnd4", Comparison.Equals, "2");


            sqb.WhereStatement.CombineClauses(sqb.WhereStatement, LogicOperator.ANDNOT, inclusiveAnd);


            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }

        [TestMethod]
        public void BuildQueryRealLifeTest2()
        {
            //string sql = "SELECT * FROM Table WHERE (preCond1 = '1' AND ( ( (incAnd1 = '2' AND incAnd2 = '2') AND incAnd3 = '2') AND incAnd4 = '2'))";
            string sql= "SELECT * FROM Table WHERE ( ( (incAnd1 = '2' AND incAnd2 = '2') AND incAnd3 = '2') AND incAnd4 = '2') AND preCond1 = '1'";
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Table");
            sqb.AddWhere("preCond1", Comparison.Equals, "1");

            WhereStatement inclusiveAnd = new WhereStatement();
            inclusiveAnd.StatementType = LogicOperator.AND;
            inclusiveAnd.Add("incAnd1", Comparison.Equals, "2");
            inclusiveAnd.Add("incAnd2", Comparison.Equals, "2");
            inclusiveAnd.Add("incAnd3", Comparison.Equals, "2");
            inclusiveAnd.Add("incAnd4", Comparison.Equals, "2");

            sqb.WhereStatement.AddCombinedClause(inclusiveAnd.CombineAllClauses(LogicOperator.AND));
            
            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }

        [TestMethod]
        public void BuildQueryCrossApplyTest()
        {
            string sql = "SELECT * FROM Customers C CROSS APPLY (SELECT * FROM Orders O WHERE O.CustomerId = C.CustomerId) ORD";
            

            SubSelectQueryBuilder subSqb = new SubSelectQueryBuilder("ORD");
            subSqb.SelectFromTable("Orders", "O");
            subSqb.AddWhere("O.CustomerId", Comparison.Equals, new SqlLiteral("C.CustomerId"));

            ApplyClause apply = new ApplyClause(ApplyType.CROSSAPPLY, subSqb);

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Customers", "C");
            sqb.Applies.Add(apply);

            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }

        [TestMethod]
        public void BuildQueryOuterApplyTest()
        {
            string sql = "SELECT * FROM Customers C OUTER APPLY (SELECT * FROM Orders O WHERE O.CustomerId = C.CustomerId) ORD";


            SubSelectQueryBuilder subSqb = new SubSelectQueryBuilder("ORD");
            subSqb.SelectFromTable("Orders", "O");
            subSqb.AddWhere("O.CustomerId", Comparison.Equals, new SqlLiteral("C.CustomerId"));

            ApplyClause apply = new ApplyClause(ApplyType.OUTERAPPLY, subSqb);

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Customers", "C");
            sqb.Applies.Add(apply);

            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }

        [TestMethod]
        public void BuildQueryMultiOuterApply()
        {
            string sql = "SELECT R.*, C.AvgDistance, C.TotalDistance, D.DistanceLeft FROM Runners R OUTER APPLY (SELECT Avg(Distance) AS AvgDistance, Sum(Distance) AS TotalDistance FROM RunnersRecord RR WHERE R.RunnerID = R.RunnerID) C OUTER APPLY (SELECT 500 - C.TotalDistance as 'DistanceLeft') D";


            SubSelectQueryBuilder subSqb = new SubSelectQueryBuilder("C");
            subSqb.SelectFromTable("RunnersRecord", "RR");
            subSqb.SelectColumns("Avg(Distance) AS AvgDistance, Sum(Distance) AS TotalDistance");
            subSqb.AddWhere("R.RunnerID", Comparison.Equals, new SqlLiteral("R.RunnerID"));
            ApplyClause apply1 = new ApplyClause(ApplyType.OUTERAPPLY, subSqb);

            SubSelectQueryBuilder subSqb2 = new SubSelectQueryBuilder("D");
            subSqb2.SelectColumns("500 - C.TotalDistance as 'DistanceLeft'");
            ApplyClause apply2 = new ApplyClause(ApplyType.OUTERAPPLY, subSqb2);

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Runners", "R");
            sqb.SelectColumns("R.*, C.AvgDistance, C.TotalDistance, D.DistanceLeft");
            sqb.Applies.Add(apply1);
            sqb.Applies.Add(apply2);

            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }

        [TestMethod]
        public void BuildQueryDateTimeMillisecond()
        {
            string sql = "SELECT * FROM Dates WHERE Date BETWEEN '2018-06-25 07:51:20' AND '2018-06-25 07:51:20.999'";

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Dates");

            DateTime firstDate = new DateTime(2018, 06, 25, 07, 51, 20, 0);
            DateTime secondDate = new DateTime(2018, 06, 25, 07, 51, 20, 999);
            DateTime[] dates = { firstDate, secondDate };
            sqb.AddWhere("Date", Comparison.Between, dates);

            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }

        [TestMethod]
        public void BuildQeryJoinWhere()
        {
            string sql = "SELECT p.ID FROM Patient_1 p " +
                "LEFT JOIN MedRec_1 m1 ON m1.PatID = p.ID AND (m1.CUID = 1 OR m1.CUID = 2) AND m1.RecType = 2 AND m1.Data.value('(/ReasonforVisit/ReasonForVisit)[1]', 'BIGINT') IN (8, 27290, 55935) " +
                "LEFT JOIN MedRec_1 m2 ON m2.PatID = p.ID AND m2.RecType = 2 AND m2.Data.value('(/ReasonforVisit/ReasonForVisit)[1]', 'BIGINT') IN (2, 27284, 55929) " +
                "LEFT JOIN MedRec_1 m3 ON m3.PatID = p.ID AND m3.RecType = 2 AND m3.Data.value('(/ReasonforVisit/ReferredBy)[1]', 'BIGINT') IN (15883) " +
                "WHERE m1.PatID IS NULL AND m2.PatID IS NULL AND m3.PatID IS NULL " +
                "ORDER BY p.ID ASC";
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Patient_1", "p");
            sqb.SelectColumn("p.ID");
            JoinClause firstJoin = new JoinClause(JoinType.LeftJoin, "MedRec_1", "m1", "PatID", Comparison.Equals, "p", "ID");
            firstJoin.Where.StatementType = LogicOperator.AND;
            firstJoin.Where.Add("m1.RecType", Comparison.Equals, 2);
            firstJoin.Where.Add("m1.Data.value('(/ReasonforVisit/ReasonForVisit)[1]', 'BIGINT')", Comparison.In, new int[] { 8, 27290, 55935 });
            firstJoin.Where.AddCombinedClause(new WhereStatement.CombinedClause(new WhereStatement.Clause("m1.CUID", Comparison.Equals, 1), LogicOperator.OR, new WhereStatement.Clause("m1.CUID", Comparison.Equals, 2)));


            JoinClause secondJoin = new JoinClause(JoinType.LeftJoin, "MedRec_1", "m2", "PatID", Comparison.Equals, "p", "ID");
            secondJoin.Where.StatementType = LogicOperator.AND;
            secondJoin.Where.Add("m2.RecType", Comparison.Equals, 2);
            secondJoin.Where.Add("m2.Data.value('(/ReasonforVisit/ReasonForVisit)[1]', 'BIGINT')", Comparison.In, new int[] { 2, 27284, 55929 });
            JoinClause thirdJoin = new JoinClause(JoinType.LeftJoin, "MedRec_1", "m3", "PatID", Comparison.Equals, "p", "ID");
            thirdJoin.Where.StatementType = LogicOperator.AND;
            thirdJoin.Where.Add("m3.RecType", Comparison.Equals, 2);
            thirdJoin.Where.Add("m3.Data.value('(/ReasonforVisit/ReferredBy)[1]', 'BIGINT')", Comparison.In, new int[] { 15883 });

            sqb.AddJoin(firstJoin);
            sqb.AddJoin(secondJoin);
            sqb.AddJoin(thirdJoin);
            sqb.AddWhere("m1.PatID", Comparison.ISNULL, null);
            sqb.AddWhere("m2.PatID", Comparison.ISNULL, null);
            sqb.AddWhere("m3.PatID", Comparison.ISNULL, null);

            sqb.AddOrderBy("p.ID");
            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }

        [TestMethod]
        public void BuildQueryJoinWhereCombined()
        {
            string sql = "SELECT p.ID FROM Patient_1 p " +
    "LEFT JOIN MedRec_1 m1 ON m1.PatID = p.ID AND (m1.CUID = 1 OR m1.CUID = 2) AND m1.RecType = 2 AND m1.Data.value('(/ReasonforVisit/ReasonForVisit)[1]', 'BIGINT') IN (8, 27290, 55935) " +
    "WHERE m1.PatID IS NULL " +
    "ORDER BY p.ID ASC";
            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Patient_1", "p");
            sqb.SelectColumn("p.ID");
            JoinClause firstJoin = new JoinClause(JoinType.LeftJoin, "MedRec_1", "m1", "PatID", Comparison.Equals, "p", "ID");
            firstJoin.Where.StatementType = LogicOperator.AND;
            firstJoin.Where.Add("m1.RecType", Comparison.Equals, 2);
            firstJoin.Where.Add("m1.Data.value('(/ReasonforVisit/ReasonForVisit)[1]', 'BIGINT')", Comparison.In, new int[] { 8, 27290, 55935 });
            firstJoin.Where.AddCombinedClause(new WhereStatement.CombinedClause(new WhereStatement.Clause("m1.CUID", Comparison.Equals, 1), LogicOperator.OR, new WhereStatement.Clause("m1.CUID", Comparison.Equals, 2)));
            
            sqb.AddJoin(firstJoin);
            sqb.AddWhere("m1.PatID", Comparison.ISNULL, null);
            sqb.AddOrderBy("p.ID");
            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }

        [TestMethod]
        public void BuildQueryWhereSub()
        {
            string sql = "SELECT * FROM Codes WHERE CodeId IN ((SELECT TOP 1 CodeId FROM Codes GROUP BY CodeId HAVING COUNT(CodeId) > 1)) ORDER BY CodeId ASC";

            SelectQueryBuilder sub = new SelectQueryBuilder();
            sub.SelectFromTable("Codes");
            sub.SelectColumn("CodeId");
            sub.TopClause = new TopClause(1, TopUnit.Records);
            sub.AddGroupBy("CodeId");
            sub.AddHavingClause(new SqlLiteral("COUNT(CodeId)"), Comparison.GreaterThan, 1);

            SelectQueryBuilder sqb = new SelectQueryBuilder();
            sqb.SelectFromTable("Codes");
            sqb.AddWhere("CodeId", Comparison.In, sub);
            sqb.AddOrderBy("CodeId");

            string select = sqb.BuildQuery();
            Assert.AreEqual(sql, select);
        }
    }
}