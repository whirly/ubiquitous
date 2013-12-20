using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubiquitous
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() < 4)
            {
                Console.Write("USAGE : ubiquitous <server> <in_point> <out_point> <password>");
            }
            else
            {
                Icecast caster = new Icecast(args[0], args[1], args[2], args[3] );
                caster.Stream();
            }
        }
    }
}
