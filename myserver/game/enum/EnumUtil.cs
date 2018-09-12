using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game
{
    public static class EnumUtil
    {
        /*
         * Get all enum values from a specific enum
         */
        public static IReadOnlyList<T> GetValues<T>() {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
