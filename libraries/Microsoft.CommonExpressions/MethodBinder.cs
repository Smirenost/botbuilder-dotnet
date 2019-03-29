﻿using System;
using System.Collections.Generic;

namespace Microsoft.Expressions
{

    /// Delegate which evaluates operators operands (aka the paramters) to the result
    /// </summary>
    public delegate object EvaluationDelegate(IReadOnlyList<object> parameters);
    /// <summary>
    /// Get a method by name.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public delegate EvaluationDelegate GetMethodDelegate(string name);

    /// <summary>
    /// Implementations of <see cref="GetMethodDelegate"/>.
    /// </summary>
    public static class MethodBinder
    {
        /// <summary>
        /// reference: https://docs.microsoft.com/en-us/azure/logic-apps/workflow-definition-language-functions-reference
        /// </summary>

        private static readonly Dictionary<string, EvaluationDelegate> FunctionMap = new Dictionary<string, EvaluationDelegate>
        {
            {"div", BuildinFunctions.Div},
            {"mul", BuildinFunctions.Mul},
            {"add", BuildinFunctions.Add},
            {"sub", BuildinFunctions.Sub},
            {"equals", BuildinFunctions.Equal},
            {"notEquals", BuildinFunctions.NotEqual},
            {"max", BuildinFunctions.Max},
            {"min", BuildinFunctions.Min},
            {"less", BuildinFunctions.LessThan},
            {"lessOrEquals", BuildinFunctions.LessThanOrEqual},
            {"greater", BuildinFunctions.GreaterThan},
            {"greaterOrEquals", BuildinFunctions.GreaterThanOrEqual},
            {"exp", BuildinFunctions.Pow},
            {"and", BuildinFunctions.And},
            {"or", BuildinFunctions.Or},
            {"not", BuildinFunctions.Not},
            {"exist", BuildinFunctions.Exist},
            {"mod", BuildinFunctions.Mod},
            {"concat", BuildinFunctions.Concat},
            {"length", BuildinFunctions.Length},
            {"replace", BuildinFunctions.Replace},
            {"replaceIgnoreCase", BuildinFunctions.ReplaceIgnoreCase},
            {"split", BuildinFunctions.Split},
            {"substring", BuildinFunctions.SubString},
            {"toLower", BuildinFunctions.ToLower},
            {"toUpper", BuildinFunctions.ToUpper},
            {"trim", BuildinFunctions.Trim},
            {"if", BuildinFunctions.If},//Similar to a ? b:c;
            {"rand", BuildinFunctions.Rand},
            {"sum", BuildinFunctions.Sum},
            {"average", BuildinFunctions.Average},
            {"addDays", BuildinFunctions.AddDays},
            {"addHours", BuildinFunctions.AddHours},
            {"addMinutes", BuildinFunctions.AddMinutes},
            {"addSeconds", BuildinFunctions.AddSeconds},
            {"dayOfWeek", BuildinFunctions.DayOfWeek},// sunday is 0
            {"dayOfMonth", BuildinFunctions.DayOfMonth},
            {"dayOfYear", BuildinFunctions.DayOfYear},
            {"month", BuildinFunctions.Month},
            {"date", BuildinFunctions.Date},
            {"year", BuildinFunctions.Year},
            {"utcNow", BuildinFunctions.UtcNow},
            {"formatDateTime", BuildinFunctions.FormatDateTime},
            {"subtractFromTime", BuildinFunctions.SubtractFromTime},//Subtract a number of time units from a timestamp
            {"dateReadBack", BuildinFunctions.DateReadBack},
            {"getTimeOfDay", BuildinFunctions.GetTimeOfDay},
            {"float", BuildinFunctions.ConvertToFloat},
            {"int", BuildinFunctions.ConvertToInt},
            {"string", BuildinFunctions.ConvertToString},
            {"bool", BuildinFunctions.ConvertToBool},
            {"createArray", BuildinFunctions.CreateArray},
            {"contains", BuildinFunctions.CheckContains},
            {"empty", BuildinFunctions.CheckEmpty},
            {"first", BuildinFunctions.First},
            {"join", BuildinFunctions.Join},
            {"last", BuildinFunctions.Last},
            {"count", BuildinFunctions.Count},
        };

        /// <summary>
        /// Default list of methods.
        /// </summary>
        public static GetMethodDelegate All { get; } = (string name) =>
         {
             if (FunctionMap.ContainsKey(name))
                 return FunctionMap[name];

             throw new Exception($"Operation {name} is invalid.");
         };
    }

    /// <summary>
    /// Wrap a GetMethodDelegate, returns a new delegate that throw the right exceptions
    /// 1st and 3rd party GetMethodDelegate needs to be wrapped into this, to work best with the rest
    /// </summary>
    class GetMethodDelegateWrapper
    {
        // this is a wrapper to help throw proper exceptions
        private readonly GetMethodDelegate _getMethod = null;
        public GetMethodDelegateWrapper(GetMethodDelegate getMethod)
        {
            _getMethod = getMethod;
        }

        public EvaluationDelegate GetMethod(string name)
        {
            try
            {
                return _getMethod(name);
            }
            catch (Exception e)
            {
                throw new NoSuchFuntionException($"No such function {name}, error: {e.Message}");
            }
        }
    }
}
