using System;
using System.Linq.Expressions;
using System.Reflection;
using W4k.SumoLogic.Logging.Serilog.Config;

namespace W4k.SumoLogic.Logging.Serilog.Extensions
{
    internal static class SumoLogicConnectionExtensions
    {
        public static SumoLogicConnection SetTimeSpanIfNotEmpty(
            this SumoLogicConnection target,
            Expression<Func<SumoLogicConnection, TimeSpan>> member,
            long? value)
        {
            if (value.HasValue
                && member.Body is MemberExpression memberExpression
                && memberExpression.Member is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(target, TimeSpan.FromMilliseconds(value.Value), null);
            }

            return target;
        }
    }
}
