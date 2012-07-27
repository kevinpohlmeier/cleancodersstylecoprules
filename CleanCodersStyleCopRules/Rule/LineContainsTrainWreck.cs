﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineContainsTrainWreck.cs" company="None, it's free for all.">
//   Copyright (c) None, it's free for all. All rights reserved.
// </copyright>
// <summary>
//   StyleCop custom rule that validates if a code line contains a train wreck.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CleanCodersStyleCopRules.Rule
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using CleanCodersStyleCopRules.Common;

    using StyleCop;
    using StyleCop.CSharp;

    /// <summary>
    ///   StyleCop custom rule that validates if a code line contains a train wreck.
    /// </summary>
    public static class LineContainsTrainWreck
    {
        #region Public Properties

        /// <summary>
        /// Gets the rule setting name.
        /// </summary>
        public static string RuleSettingName
        {
            get
            {
                return MethodBase.GetCurrentMethod().ReflectedType.Name + "Value";
            }
        }

        /// <summary>
        ///   Gets the rule name.
        /// </summary>
        public static string RuleName
        {
            get
            {
                return MethodBase.GetCurrentMethod().ReflectedType.Name;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Validate if a code line contains a train wreck.
        /// </summary>
        /// <param name="element">
        /// The current element. 
        /// </param>
        /// <param name="parentElement">
        /// The parent element. 
        /// </param>
        /// <param name="context">
        /// The context, this class. 
        /// </param>
        /// <returns>
        /// Returns true to continue, false to stop visiting the elements in the code document. 
        /// </returns>
        [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0042:MethodHasTooManyArgument", Justification = "It's a delegate for Analyzer.VisitElement.")]
        public static bool Validate(CsElement element, CsElement parentElement, CleanCoderAnalyzer context)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(context, "context");

            DocumentRoot documentRoot = element as DocumentRoot;

            if (documentRoot == null)
            {
                return true;
            }

            List<string> lines = Utility.SplitSourceCodeInLine(documentRoot.Document.SourceCode);

            for (int i = 0; i < lines.Count; i++)
            {
                int currentLineNumber = i + 1;

                string currentLine = lines[i].Trim();

                if (currentLine.StartsWith("//") || currentLine.StartsWith("namespace"))
                {
                    continue;
                }

                Regex wagonRegex = new Regex(@"\(*\)[\.|;]");

                int numberOfWagon = wagonRegex.Matches(currentLine).Count;

                if (numberOfWagon > 3)
                {
                    context.AddViolation(documentRoot, currentLineNumber, RuleName, numberOfWagon, context.AnalyserSetting[RuleSettingName]);
                }
            }

            return true;
        }

        #endregion
    }
}