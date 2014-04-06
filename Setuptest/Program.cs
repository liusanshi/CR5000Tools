using System;
using System.Collections.Generic;
using System.Text;

using Zuken.Setup;

namespace Setuptest
{
    class Program
    {
        static void Main(string[] args)
        {
            ZukenSetup zk = new ZukenSetup();
            zk.SourceDefaultDir = @"D:\Program Files\kingdee\K3PLM\Integration\Integration Setup\Resources";
            zk.UserDefaultDir = @"D:\Program Files\Zuken_PLM";

            zk.Install();

            Console.WriteLine(zk.DirName);

            Console.Read();
        }
    }
}
