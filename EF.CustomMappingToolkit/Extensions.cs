using System.Linq;
using System.Linq.Expressions;

namespace EF.CustomMappingToolkit
{
    public static class Extensions
    {
        public static IQueryable<T> AsCustomMappingCompatible<T>(this IQueryable<T> query)
        {
            if (query is CustomMappingCompatibleQuery<T>)
            {
                return query;
            }

            return new CustomMappingCompatibleQuery<T>(query);
        }

        public static Expression<TDelegate> MakeCustomMappingCompatible<TDelegate>(this Expression<TDelegate> expr)
        {
            return (Expression<TDelegate>)new EnumAsStringExpressionVisitor().Visit(expr);
        }

        public static Expression MakeCustomMappingCompatible(this Expression expr)
        {
            return new EnumAsStringExpressionVisitor().Visit(expr);
        }
    }
}
