using MSTestUnit.Services;

namespace MSTestUnit
{
    [TestClass]
    public class UnitTest1
    {
        private readonly PrimeService primeService = new PrimeService();
        public UnitTest1()
        {
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
        [TestMethod]
        [DataRow(1, "message", true, 2.0)]
        public void TestMethod1(int i, string s, bool b, float f) 
        {

        }

        [TestMethod]
        [DataRow(new string[] { "abne1", "line2" })]
        public void TestMethod2(string[] lines) 
        {
            var res = primeService.CheckInt(lines);
            Assert.IsTrue(res == 1);
        }

        [TestMethod]
        [DataRow(null)]
        public void TestMethod3(object o) { }

        [TestMethod]
        //[DataRow(new string[] { "line1", "line2" }, new string[] { "line1.", "line2." })]
        public void TestMethod4(string[] input, string[] expectedOutput) { }

    }
}