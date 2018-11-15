using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboChat.Library
{
    static class CollectionExtensions
    {
        public static IEnumerable<T> withCollection<T>(this List<T> listFirst, List<T> listSecond)
        {
            var x = listFirst;
            x.AddRange(listSecond);
            return x;
        }
    }
}
