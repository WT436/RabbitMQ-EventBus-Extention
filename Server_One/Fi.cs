using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server_One
{
    public class Fi
    {
        public int Fib(int n) => Enumerable.Range(1, n).Aggregate((t, i) => t + i);
    }
}
