using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EF.CustomMappingToolkit
{
    class EnumAsStringExpressionVisitor : ExpressionVisitor
    {
        public Expression Modify(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            if (b.Left.NodeType == ExpressionType.Convert)
            {
                var leftUnaryExpressionOperandMemberExpression = (b.Left as UnaryExpression)?.Operand as MemberExpression;
                var mapEnumAsStringAttr = leftUnaryExpressionOperandMemberExpression?.Member.GetCustomAttribute(typeof(MapEnumAsStringAttribute)) as MapEnumAsStringAttribute;
                var rightConstantExpression = b.Right as ConstantExpression;
                if (leftUnaryExpressionOperandMemberExpression != null && mapEnumAsStringAttr != null && rightConstantExpression != null)
                {
                    Expression left = Expression.MakeMemberAccess(leftUnaryExpressionOperandMemberExpression.Expression, leftUnaryExpressionOperandMemberExpression.Member.ReflectedType.GetProperty(mapEnumAsStringAttr.StringPropertyName));
                    Expression right = Expression.Constant(Enum.ToObject(((PropertyInfo)leftUnaryExpressionOperandMemberExpression.Member).PropertyType, rightConstantExpression.Value).ToString());

                    return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
                }
            }

            return base.VisitBinary(b);
        }
    }
}
