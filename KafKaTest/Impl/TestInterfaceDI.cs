using KafKaTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafKaTest.Impl
{
    public class TestInterfaceDI<T>: ITestInterfaceDI<T> where T : class
    {
    }
}
