using BahamutCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public class Program
    {
        public void Main(string[] args)
        {
            while (true)
            {
                var name = Console.ReadLine();
                var key = AppkeyUtil.GenerateAppkey(name);
                Console.WriteLine(key);
            }
        }
    }
}
