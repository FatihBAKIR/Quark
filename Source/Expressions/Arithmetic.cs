using Quark.Contexts;
using UnityEngine;

namespace Quark.Expressions
{
    /// <summary>
    /// This class provides basic operations on Quark expressions
    /// </summary>
    public class Arithmetic
    {
        /// <summary>
        /// Returns an expression that adds the values of two expressions
        /// </summary>
        /// <typeparam name="T">Context requirement of the expression</typeparam>
        /// <param name="exp1">First expression</param>
        /// <param name="exp2">Second expression</param>
        /// <returns>Summing expression</returns>
        public static IExpression<T> Add<T>(IExpression<T> exp1, IExpression<T> exp2)
            where T : class, IContext
        {
            return new BinaryOp<T>(exp1, exp2, (x, y) => x + y);
        }

        /// <summary>
        /// Returns an expression that subtracts the values of two expressions
        /// </summary>
        /// <typeparam name="T">Context requirement of the expression</typeparam>
        /// <param name="exp1">First expression</param>
        /// <param name="exp2">Second expression</param>
        /// <returns>Subtracting expression</returns>
        public static IExpression<T> Sub<T>(IExpression<T> exp1, IExpression<T> exp2)
            where T : class, IContext
        {
            return new BinaryOp<T>(exp1, exp2, (x, y) => x - y);
        }

        /// <summary>
        /// Returns an expression that multiplies the values of two expressions
        /// </summary>
        /// <typeparam name="T">Context requirement of the expression</typeparam>
        /// <param name="exp1">First expression</param>
        /// <param name="exp2">Second expression</param>
        /// <returns>Multiplying expression</returns>
        public static IExpression<T> Mul<T>(IExpression<T> exp1, IExpression<T> exp2)
            where T : class, IContext
        {
            return new BinaryOp<T>(exp1, exp2, (x, y) => x * y);
        }

        /// <summary>
        /// Returns an expression that divides the values of two expressions
        /// </summary>
        /// <typeparam name="T">Context requirement of the expression</typeparam>
        /// <param name="exp1">First expression</param>
        /// <param name="exp2">Second expression</param>
        /// <returns>Dividing expression</returns>
        public static IExpression<T> Div<T>(IExpression<T> exp1, IExpression<T> exp2)
            where T : class, IContext
        {
            return new BinaryOp<T>(exp1, exp2, (x, y) => x / y);
        }

        /// <summary>
        /// Returns an expression that calculates the minimum of two expressions
        /// </summary>
        /// <typeparam name="T">Context requirement of the expression</typeparam>
        /// <param name="exp1">First expression</param>
        /// <param name="exp2">Second expression</param>
        /// <returns>Minimum expression</returns>
        public static IExpression<T> Min<T>(IExpression<T> exp1, IExpression<T> exp2)
            where T : class, IContext
        {
            return new BinaryOp<T>(exp1, exp2, Mathf.Min);
        }

        /// <summary>
        /// Returns an expression that calculates the maximum of two expressions
        /// </summary>
        /// <typeparam name="T">Context requirement of the expression</typeparam>
        /// <param name="exp1">First expression</param>
        /// <param name="exp2">Second expression</param>
        /// <returns>Maximum expression</returns>
        public static IExpression<T> Max<T>(IExpression<T> exp1, IExpression<T> exp2)
            where T : class, IContext
        {
            return new BinaryOp<T>(exp1, exp2, Mathf.Max);
        }
    }
}
