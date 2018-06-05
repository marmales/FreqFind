using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreqFind.Common.Extensions
{
    public static class LinqExtensions
    {
        public static Task<List<T>> ToListAsync<T>(this IEnumerable<T> list)
        {
            return Task.Run(() => list.ToList());
        }
    }
}
