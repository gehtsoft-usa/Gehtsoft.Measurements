using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Gehtsoft.Measurements
{
    internal static class CodeGenerator
    {
        public static Func<T, string> GenerateGetUnitName<T>()
        {
            Type type = typeof(T);
            var param = Expression.Parameter(type, "unit");
            var returnTarget = Expression.Label(typeof(string));

            //get fields and prepare cases for switch
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var cases = new SwitchCase[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                UnitAttribute attribute = fields[i].GetCustomAttribute<UnitAttribute>();
                var returnStatement = Expression.Return(returnTarget, Expression.Constant(attribute.Name));
                var value = Enum.ToObject(type, fields[i].GetRawConstantValue());
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
            var expression = Expression.Lambda(typeof(Func<T, string>), body, new ParameterExpression[] { param });
            var functionDelegate = expression.Compile();
            return (Func<T, string>)functionDelegate;
        }

        public static Func<T, int> GenerateGetDefaultUnitAccuracy<T>()
        {
            Type type = typeof(T);
            var param = Expression.Parameter(type, "unit");
            var returnTarget = Expression.Label(typeof(int));

            //get fields and prepare cases for switch
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var cases = new SwitchCase[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                UnitAttribute attribute = fields[i].GetCustomAttribute<UnitAttribute>();
                var returnStatement = Expression.Return(returnTarget, Expression.Constant(attribute.DefaultAccuracy));
                var value = Enum.ToObject(type, fields[i].GetRawConstantValue());
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
            var expression = Expression.Lambda(typeof(Func<T, int>), body, new ParameterExpression[] { param });
            var functionDelegate = expression.Compile();
            return (Func<T, int>)functionDelegate;
        }

        public static Func<string, T> GenerateParseUnitName<T>()
        {
            Type type = typeof(T);
            var param = Expression.Parameter(typeof(string), "unit");
            var returnTarget = Expression.Label(type);

            //get fields and prepare cases for switch
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var cases = new SwitchCase[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                UnitAttribute attribute = fields[i].GetCustomAttribute<UnitAttribute>();
                var value = Enum.ToObject(type, fields[i].GetRawConstantValue());
                
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
            var body = Expression.Block(type, new Expression[] { switchStmt, Expression.Label(returnTarget, Expression.Constant(default(T))) });

            //function 
            var expression = Expression.Lambda(typeof(Func<string, T>), body, new ParameterExpression[] { param });
            var functionDelegate = expression.Compile();
            return (Func<string, T>)functionDelegate;
        }

        public static Func<double, T, double> GenerateToBaseConversion<T>()
        {
            Type type = typeof(T);
            var valueParameter = Expression.Parameter(typeof(double), "value");
            var unitParameter = Expression.Parameter(type, "unit");
            var returnTarget = Expression.Label(typeof(double));

            //get fields and prepare cases for switch
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var cases = new SwitchCase[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                ConversionAttribute attribute = fields[i].GetCustomAttribute<ConversionAttribute>();
                Expression unit = Expression.Constant(Enum.ToObject(type, fields[i].GetRawConstantValue()));
                Expression convertor = ToBaseExpression(attribute, valueParameter);
                var returnStatement = Expression.Return(returnTarget, convertor);
                cases[i] = Expression.SwitchCase(returnStatement, new Expression[] { unit });
            }

            //prepare default method throwing exception
            var constructorInfo = typeof(ArgumentException).GetConstructor(new Type[] { typeof(string), typeof(string) });
            var argumentException = Expression.New(constructorInfo, new Expression[] { Expression.Constant("Unknown unit"), Expression.Constant("unit") });
            var defaultBody = Expression.Throw(argumentException);

            //switch statement
            var switchStmt = Expression.Switch(unitParameter, defaultBody, cases);

            //method body
            var body = Expression.Block(typeof(double), new Expression[] { switchStmt, Expression.Label(returnTarget, Expression.Constant(0.0)) });

            //function 
            var expression = Expression.Lambda(typeof(Func<double, T, double>), body, new ParameterExpression[] { valueParameter, unitParameter });
            var functionDelegate = expression.Compile();
            return (Func<double, T, double>)functionDelegate;
        }



        private static Expression ToBaseExpression(ConversionAttribute attribute, Expression value)
        {
            if (attribute.Operation == ConversionOperation.Base)
                return value;

            Expression firstOperation = OperationToExpression(value, attribute.Operation, attribute.Factor);
            if (attribute.SecondOperation == ConversionOperation.None)
                return firstOperation;
            else
                return OperationToExpression(firstOperation, attribute.SecondOperation, attribute.SecondFactor);
        }

        private static Expression FromBaseExpression(ConversionAttribute attribute, Expression value)
        {
            if (attribute.Operation == ConversionOperation.Base)
                return value;

            Expression secondExpression;
            if (attribute.SecondOperation == ConversionOperation.None)
                secondExpression = value;
            else
                secondExpression = OperationToReverseExpression(value, attribute.SecondOperation, attribute.SecondFactor);

            return OperationToReverseExpression(value, attribute.Operation, attribute.Factor);
        }

        private static Expression OperationToExpression(Expression value, ConversionOperation operation, double factor)
        {
            Expression r = Expression.Constant(0);
            switch (operation)
            {
                case ConversionOperation.Base:
                    r = value;
                    break;
                case ConversionOperation.Add:
                    r = Expression.Add(value, Expression.Constant(factor));
                    break;
                case ConversionOperation.Subtract:
                    r = Expression.Subtract(value, Expression.Constant(factor));
                    break;
                case ConversionOperation.SubtractFromFactor:
                    r = Expression.Subtract(Expression.Constant(factor), value);
                    break;
                case ConversionOperation.Multiply:
                    r = Expression.Multiply(value, Expression.Constant(factor));
                    break;
                case ConversionOperation.Divide:
                    r = Expression.Divide(value, Expression.Constant(factor));
                    break;
                case ConversionOperation.DivideFactor:
                    r = Expression.Divide(Expression.Constant(factor), value);
                    break;
                case ConversionOperation.Negate:
                    r = Expression.Negate(value);
                    break;
            }
            return r;
        }

        private static Expression OperationToReverseExpression(Expression value, ConversionOperation operation, double factor)
        {
            Expression r = Expression.Constant(0);
            switch (operation)
            {
                case ConversionOperation.Base:
                    r = value;
                    break;
                case ConversionOperation.Add:
                    r = Expression.Subtract(value, Expression.Constant(factor));
                    break;
                case ConversionOperation.Subtract:
                case ConversionOperation.SubtractFromFactor:
                    r = Expression.Add(value, Expression.Constant(factor));
                    break;
                case ConversionOperation.Multiply:
                    r = Expression.Divide(value, Expression.Constant(factor));
                    break;
                case ConversionOperation.Divide:
                case ConversionOperation.DivideFactor:
                    r = Expression.Multiply(value, Expression.Constant(factor));
                    break;
                case ConversionOperation.Negate:
                    r = Expression.Negate(value);
                    break;
            }
            return r;
        }
    }
}
