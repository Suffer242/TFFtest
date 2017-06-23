using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class totTests
    {
        [TestMethod()]
        public void TextBetweenTest()
        {
            var s = "123thethe456";
            Assert.AreEqual(s.BeforeFirst("the"),"123"  );
            Assert.AreEqual(s.BeforeLast("the"), "123the");
            Assert.AreEqual(s.AfterFirst("the"), "the456");
            Assert.AreEqual(s.AfterLast("the"), "456");

        }
    }
}