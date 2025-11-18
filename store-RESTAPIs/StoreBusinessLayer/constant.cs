using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreBusinessLayer
{
    public static class Constant
    {
        //using for paginate for any queryable
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int pageNumber, int pageSize = 10)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source), "المصدر لا يمكن أن يكون فارغًا.");

            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "رقم الصفحة يجب أن يكون أكبر من الصفر.");

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "حجم الصفحة يجب أن يكون أكبر من الصفر.");

            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
