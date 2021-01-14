using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Gehtsoft.Measurements
{
    internal static class CodeGenerator
    {
        public static Func<int, string> GenerateGetUnitName(Type type)
        {
            var param = Expression.Parameter(typeof(int), "unit");
            var returnTarget = Expression.Label(typeof(string));

            //get fields and prepare cases for switch
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var cases = new SwitchCase[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                UnitAttribute attribute = fields[i].GetCustomAttribute<UnitAttribute>();
                var returnStatement = Expression.Return(returnTarget, Expression.Constant(attribute.Name));
                var value = fields[i].GetRawConstantValue();
                cases[i] = Expression.SwitchCase(returnStatement, new Expression[] { Expression.Constant(value)});
            }

            //prepare default method throwing exception
            var constructorInfo = typeof(ArgumentException).GetConstructor(new Type[] { typeof(string), typeof(string) });
            var argumentException = Expression.New(constructorInfo, new Expression[] { Expression.Constant("Unknown unit"), Expression.Constant("unit") });
            var defaultBody = Expression.Throw(argumentException);

            //switch statement
            var switchStmt = Expression.Switch(param, defaultBody, cases);

            //method body
            var body = Expression.Block(typeof(string), new Expression[] { switchStmt, Expression.Label(returnTarget, Expression.Constant("")) });

            //function 
            var expression = Expression.Lambda(typeof(Func<int, string>), body, new ParameterExpression[] { param });
            var functionDelegate = expression.Compile();
            return (Func<int, string>)functionDelegate;
        }

        public static Func<int, int> GenerateGetDefaultUnitAccuracy(Type type)
        {
            var param = Expression.Parameter(typeof(int), "unit");
            var returnTarget = Expression.Label(typeof(int));

            //get fields and prepare cases for switch
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var cases = new SwitchCase[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                UnitAttribute attribute = fields[i].GetCustomAttribute<UnitAttribute>();
                var returnStatement = Expression.Return(returnTarget, Expression.Constant(attribute.DefaultAccuracy));
                var value = fields[i].GetRawConstantValue();
                cases[i] = Expression.SwitchCase(returnStatement, new Expression[] { Expression.Constant(value) });
            }

            //prepare default method throwing exception
            var constructorInfo = typeof(ArgumentException).GetConstructor(new Type[] { typeof(string), typeof(string) });
            var argumentException = Expression.New(constructorInfo, new Expression[] { Expression.Constant("Unknown unit"), Expression.Constant("unit") });
            var defaultBody = Expression.Throw(argumentException);

            //switch statement
            var switchStmt = Expression.Switch(param, defaultBody, cases);

            //method body
            var body = Expression.Block(typeof(int), new Expression[] { switchStmt, Expression.Label(returnTarget, Expression.Constant(0)) });

            //function 
            var expression = Expression.Lambda(typeof(Func<int, int>), body, new ParameterExpression[] { param });
            var functionDelegate = expression.Compile();
            return (Func<int, int>)functionDelegate;
        }

        public static Func<string, int> GenerateParseUnitName(Type type)
        {
            var param = Expression.Parameter(typeof(string), "unit");
            var returnTarget = Expression.Label(typeof(int));

            //get fields and prepare cases for switch
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var cases = new SwitchCase[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                UnitAttribute attribute = fields[i].GetCustomAttribute<UnitAttribute>();
                var value = fields[i].GetRawConstantValue();
                
                var returnStatement = Expression.Return(returnTarget, Expression.Constant(value));
                Expression[] args;
                if (attribute.HasAlternativeName)
                    args = new Expression[] { Expression.Constant(attribute.Name), Expression.Constant(attribute.AlterantiveName) };
                else
                    args = new Expression[] { Expression.Constant(attribute.Name) };

                cases[i] = Expression.SwitchCase(returnStatement, args);
            }

            //prepare default method throwing exception
            var constructorInfo = typeof(ArgumentException).GetConstructor(new Type[] { typeof(string), typeof(string) });
            var argumentException = Expression.New(constructorInfo, new Expression[] { Expression.Constant("Unknown unit"), Expression.Constant("unit") });
            var defaultBody = Expression.Throw(argumentException);

            //switch statement
            var switchStmt = Expression.Switch(param, defaultBody, cases);

            //method body
            var body = Expression.Block(typeof(int), new Expression[] { switchStmt, Expression.Label(returnTarget, Expression.Constant(0)) });

            //function 
            var expression = Expression.Lambda(typeof(Func<string, int>), body, new ParameterExpression[] { param });
            var functionDelegate = expression.Compile();
            return (Func<string, int>)functionDelegate;
        }

    }
}
