using System;
using System.Collections;
using System.Collections.Generic;
using NCalc.Domain;
using Antlr.Runtime;
using System.Diagnostics;
using System.Threading;

namespace NCalc
{
    public class Expression
    {
        public EvaluateOptions Options { get; set; }

        /// <summary>
        /// Textual representation of the expression to evaluate.
        /// </summary>
        protected string OriginalExpression;

        public Expression(string expression) : this(expression, EvaluateOptions.None)
        {
        }

        public Expression(string expression, EvaluateOptions options)
        {
            if (String.IsNullOrEmpty(expression))
                throw new
                    ArgumentException("Expression can't be empty", "expression");

            OriginalExpression = expression;
            Options = options;
        }

        public Expression(LogicalExpression expression) : this(expression, EvaluateOptions.None)
        {
        }

        public Expression(LogicalExpression expression, EvaluateOptions options)
        {
            if (expression == null)
                throw new
                    ArgumentException("Expression can't be null", "expression");

            ParsedExpression = expression;
            Options = options;
        }

        #region Cache management
        private static bool _cacheEnabled = true;
        private static Dictionary<string, WeakReference> _compiledExpressions = new Dictionary<string, WeakReference>();
        private static readonly ReaderWriterLock Rwl = new ReaderWriterLock();

        public static bool CacheEnabled
        {
            get { return _cacheEnabled; }
            set
            {
                _cacheEnabled = value;

                if (!CacheEnabled)
                {
                    // Clears cache
                    _compiledExpressions = new Dictionary<string, WeakReference>();
                }
            }
        }

        /// <summary>
        /// Removed unused entries from cached compiled expression
        /// </summary>
        private static void CleanCache()
        {
            var keysToRemove = new List<string>();

            try
            {
                Rwl.AcquireWriterLock(Timeout.Infinite);
                {
                    // foreach(var de in _compiledExpressions)
                    var __enumerator1 = (_compiledExpressions).GetEnumerator();
                    while (__enumerator1.MoveNext())
                    {
                        var de = __enumerator1.Current;
                        {
                            if (!de.Value.IsAlive)
                            {
                                keysToRemove.Add(de.Key);
                            }
                        }
                    }
                }
                {
                    var __list2 = keysToRemove;
                    var __listCount2 = __list2.Count;
                    for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                    {
                        var key = (string)__list2[__i2];
                        {
                            _compiledExpressions.Remove(key);
                            Trace.TraceInformation("Cache entry released: " + key);
                        }
                    }
                }
            }
            finally
            {
                Rwl.ReleaseReaderLock();
            }
        }

        #endregion

        public static LogicalExpression Compile(string expression, bool nocache)
        {
            LogicalExpression logicalExpression = null;

            if (_cacheEnabled && !nocache)
            {
                try
                {
                    Rwl.AcquireReaderLock(Timeout.Infinite);

                    if (_compiledExpressions.ContainsKey(expression))
                    {
                        Trace.TraceInformation("Expression retrieved from cache: " + expression);
                        var wr = _compiledExpressions[expression];
                        logicalExpression = wr.Target as LogicalExpression;

                        if (wr.IsAlive && logicalExpression != null)
                        {
                            return logicalExpression;
                        }
                    }
                }
                finally
                {
                    Rwl.ReleaseReaderLock();
                }
            }

            if (logicalExpression == null)
            {
                var lexer = new NCalcLexer(new ANTLRStringStream(expression));
                var parser = new NCalcParser(new CommonTokenStream(lexer));

                logicalExpression = parser.ncalcExpression().value;

                if (parser.Errors != null && parser.Errors.Count > 0)
                {
                    throw new EvaluationException(String.Join(Environment.NewLine, parser.Errors.ToArray()));
                }

                if (_cacheEnabled && !nocache)
                {
                    try
                    {
                        Rwl.AcquireWriterLock(Timeout.Infinite);
                        _compiledExpressions[expression] = new WeakReference(logicalExpression);
                    }
                    finally
                    {
                        Rwl.ReleaseWriterLock();
                    }

                    CleanCache();

                    Trace.TraceInformation("Expression added to cache: " + expression);
                }
            }

            return logicalExpression;
        }

        /// <summary>
        /// Pre-compiles the expression in order to check syntax errors.
        /// If errors are detected, the Error property contains the message.
        /// </summary>
        /// <returns>True if the expression syntax is correct, otherwiser False</returns>
        public bool HasErrors()
        {
            try
            {
                if (ParsedExpression == null)
                {
                    ParsedExpression = Compile(OriginalExpression, (Options & EvaluateOptions.NoCache) == EvaluateOptions.NoCache);
                }

                // In case HasErrors() is called multiple times for the same expression
                return ParsedExpression != null && Error != null;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return true;
            }
        }

        public string Error { get; private set; }

        public LogicalExpression ParsedExpression { get; private set; }

        protected Dictionary<string, IEnumerator> ParameterEnumerators;
        protected Dictionary<string, object> ParametersBackup;

        public object Evaluate()
        {
            if (HasErrors())
            {
                throw new EvaluationException(Error);
            }

            if (ParsedExpression == null)
            {
                ParsedExpression = Compile(OriginalExpression, (Options & EvaluateOptions.NoCache) == EvaluateOptions.NoCache);
            }


            var visitor = new EvaluationVisitor(Options);
            visitor.EvaluateFunction += EvaluateFunction;
            visitor.EvaluateParameter += EvaluateParameter;
            visitor.Parameters = Parameters;

            // if array evaluation, execute the same expression multiple times
            if ((Options & EvaluateOptions.IterateParameters) == EvaluateOptions.IterateParameters)
            {
                int size = -1;
                ParametersBackup = new Dictionary<string, object>();
                {
                    // foreach(var key in Parameters.Keys)
                    var __enumerator3 = (Parameters.Keys).GetEnumerator();
                    while (__enumerator3.MoveNext())
                    {
                        var key = (string)__enumerator3.Current;
                        {
                            ParametersBackup.Add(key, Parameters[key]);
                        }
                    }
                }

                ParameterEnumerators = new Dictionary<string, IEnumerator>();
                {
                    // foreach(var parameter in Parameters.Values)
                    var __enumerator4 = (Parameters.Values).GetEnumerator();
                    while (__enumerator4.MoveNext())
                    {
                        var parameter = (object)__enumerator4.Current;
                        {
                            if (parameter is IEnumerable)
                            {
                                int localsize = 0;
                                {
                                    // foreach(var o in (IEnumerable)parameter)
                                    var __enumerator7 = ((IEnumerable)parameter).GetEnumerator();
                                    while (__enumerator7.MoveNext())
                                    {
                                        var o = (object)__enumerator7.Current;
                                        {
                                            localsize++;
                                        }
                                    }
                                }

                                if (size == -1)
                                {
                                    size = localsize;
                                }
                                else if (localsize != size)
                                {
                                    throw new EvaluationException("When IterateParameters option is used, IEnumerable parameters must have the same number of items");
                                }
                            }
                        }
                    }
                }
                {
                    // foreach(var key in Parameters.Keys)
                    var __enumerator5 = (Parameters.Keys).GetEnumerator();
                    while (__enumerator5.MoveNext())
                    {
                        var key = (string)__enumerator5.Current;
                        {
                            var parameter = Parameters[key] as IEnumerable;
                            if (parameter != null)
                            {
                                ParameterEnumerators.Add(key, parameter.GetEnumerator());
                            }
                        }
                    }
                }

                var results = new List<object>();
                for (int i = 0; i < size; i++)
                {
                    {
                        // foreach(var key in ParameterEnumerators.Keys)
                        var __enumerator6 = (ParameterEnumerators.Keys).GetEnumerator();
                        while (__enumerator6.MoveNext())
                        {
                            var key = (string)__enumerator6.Current;
                            {
                                IEnumerator enumerator = ParameterEnumerators[key];
                                enumerator.MoveNext();
                                Parameters[key] = enumerator.Current;
                            }
                        }
                    }

                    ParsedExpression.Accept(visitor);
                    results.Add(visitor.Result);
                }

                return results;
            }

            ParsedExpression.Accept(visitor);
            return visitor.Result;

        }

        public event EvaluateFunctionHandler EvaluateFunction;
        public event EvaluateParameterHandler EvaluateParameter;

        private Dictionary<string, object> _parameters;

        public Dictionary<string, object> Parameters
        {
            get { return _parameters ?? (_parameters = new Dictionary<string, object>()); }
            set { _parameters = value; }
        }

    }
}
