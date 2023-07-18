using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using YPrime.Data.Study;
using YPrime.BusinessLayer.Interfaces;

namespace YPrime.BusinessLayer.BaseClasses
{
    public abstract class BaseRepository : IDisposable, IBaseRepository
    {
        protected const string DefaultCultureCode = "en-US";

        protected readonly IStudyDbContext _db;

        protected BaseRepository(IStudyDbContext db)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            _db = db;
        }

        public void Dispose()
        {
        }

        protected Expression<Func<T, U>> GetGroupKey<T, U>(string GroupProperty)
        {
            var parameter = Expression.Parameter(typeof(T));
            var body = Expression.Property(parameter, GroupProperty);
            return Expression.Lambda<Func<T, U>>(body, parameter);
        }

        protected Expression<Func<T, TReturn>> CreateSelector<T, TReturn>(string FieldName)
            where T : class
            where TReturn : class
        {
            ParameterExpression p = Expression.Parameter(typeof(T), "t");
            Expression body;
            var fieldNameSplit = FieldName.Split('.');
            //need to handle embedded objects, only going 1 deep for now
            switch (fieldNameSplit.Length)
            {
                case 2:
                    body = Expression.Property(Expression.Property(p, fieldNameSplit[0]), fieldNameSplit[1]);
                    break;
                default:
                    //only one level object
                    body = Expression.Property(p, fieldNameSplit[0]);
                    break;
            }

            Expression conversion = Expression.Convert(body, typeof(TReturn));
            return Expression.Lambda<Func<T, TReturn>>(conversion, p);
        }

        public static SqlParameter CreateNullableSqlParameter(SqlDbType sqlType, object Value, string ParameterName)
        {
            var parameter = new SqlParameter(ParameterName, sqlType);
            parameter.Value = Value;
            if (parameter.Value == null)
            {
                parameter.Value = SqlString.Null;
            }

            return parameter;
        }
    }
}