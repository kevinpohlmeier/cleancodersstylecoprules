﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassNameHasTooManyWord.cs" company="None, it's free for all.">
//   Copyright (c) None, it's free for all. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CleanCodersStyleCopRules.Rule
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using CleanCodersStyleCopRules.Common;

    using StyleCop;
    using StyleCop.CSharp;

    /// <summary>
    /// StyleCop custom rule that validates if a the class name contains too many words.
    /// </summary>
    [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0309:DescriptiveNameTooExplicit", Justification = "It's for a test.")]
    public class ClassNameHasTooManyWord : CustomRuleBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassNameHasTooManyWord"/> class.
        /// </summary>
        public ClassNameHasTooManyWord()
        {
            this.ElementTypes.Add(ElementType.Class);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the rule setting name.
        /// </summary>
        public static new string RuleSettingName
        {
            get
            {
                return MethodBase.GetCurrentMethod().ReflectedType.Name + "Value";
            }
        }

        /// <summary>
        /// Gets the rule name.
        /// </summary>
        public override string RuleName
        {
            get
            {
                return MethodBase.GetCurrentMethod().ReflectedType.Name;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Validate if a the class name contains too many words with an element.
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
        public override bool ValidateElement(CsElement element, CsElement parentElement, CleanCoderAnalyzer context)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(context, "context");

            string[] classNameParts = Utility.SplitStringAtUpperCaseLetter(element.Declaration.Name);

            if (classNameParts.Length > (int)context.AnalyserSetting[RuleSettingName])
            {
                context.AddViolation(element, element.LineNumber, this.RuleName, element.Declaration.Name, classNameParts.Length, context.AnalyserSetting[RuleSettingName]);
            }

            return true;
        }

        #endregion
    }
}