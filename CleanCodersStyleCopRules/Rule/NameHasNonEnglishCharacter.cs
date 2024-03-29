﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameHasNonEnglishCharacter.cs" company="None, it's free for all.">
//   Copyright (c) None, it's free for all. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CleanCodersStyleCopRules.Rule
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using CleanCodersStyleCopRules.Common;

    using StyleCop;
    using StyleCop.CSharp;

    /// <summary>
    /// StyleCop custom rule that validates if a name has non english characters in it.
    /// </summary>
    public class NameHasNonEnglishCharacter : CustomRuleBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NameHasNonEnglishCharacter"/> class.
        /// </summary>
        public NameHasNonEnglishCharacter()
        {
            this.ElementTypes.Add(ElementType.Namespace);
            this.ElementTypes.Add(ElementType.Class);
            this.ElementTypes.Add(ElementType.Enum);
            this.ElementTypes.Add(ElementType.Interface);
            this.ElementTypes.Add(ElementType.Struct);

            this.ExpressionTypes.Add(ExpressionType.VariableDeclarator);
        }

        #endregion

        #region Public Properties

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
        /// Validate if a name has non english characters in it.
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
        [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0034:MethodContainsTooManyLine", Justification = "Parent method lunching many child methods.")]
        public override bool ValidateElement(CsElement element, CsElement parentElement, CleanCoderAnalyzer context)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(context, "context");

            if (element.Declaration.Name != null)
            {
                string name = Utility.TrimGenericType(element.Declaration.Name);

                this.ProcessName(element, name, element.LineNumber, context);
            }

            foreach (Delegate delegateObject in element.ChildCodeElements.OfType<Delegate>().ToList())
            {
                this.ParseDelegate(delegateObject, context);
            }

            foreach (Event eventObject in element.ChildCodeElements.OfType<Event>().ToList())
            {
                this.ParseEvent(eventObject, context);
            }

            foreach (Property property in element.ChildCodeElements.OfType<Property>().ToList())
            {
                this.ProcessName(element, property.Declaration.Name, property.LineNumber, context);
            }

            foreach (Constructor constructor in element.ChildCodeElements.OfType<Constructor>().ToList())
            {
                this.ParseConstructor(constructor, context);
            }

            foreach (Method method in element.ChildCodeElements.OfType<Method>().ToList())
            {
                this.ParseMethod(method, context);
            }

            foreach (Struct structObject in element.ChildCodeElements.OfType<Struct>().ToList())
            {
                this.ParseStruct(structObject, context);
            }

            foreach (Enum enumObject in element.ChildCodeElements.OfType<Enum>().ToList())
            {
                this.ParseEnum(enumObject, context);
            }

            return true;
        }

        /// <summary>
        /// Validate if a variable name has non english characters in it an expression.
        /// </summary>
        /// <param name="expression">
        /// The expression. 
        /// </param>
        /// <param name="parentExpression">
        /// The parent expression. 
        /// </param>
        /// <param name="parentStatement">
        /// The parent statement. 
        /// </param>
        /// <param name="parentElement">
        /// The parent element. 
        /// </param>
        /// <param name="context">
        /// The context, this class. 
        /// </param>
        /// <returns>
        /// True if all visited expressions are valid, False otherwise. 
        /// </returns>
        [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0042:MethodHasTooManyArgument", Justification = "It's a delegate.")]
        public override bool ValidateExpression(Expression expression, Expression parentExpression, Statement parentStatement, CsElement parentElement, CleanCoderAnalyzer context)
        {
            if (expression.ExpressionType != ExpressionType.VariableDeclarator)
            {
                return true;
            }

            VariableDeclaratorExpression variableDeclaratorExpression = expression as VariableDeclaratorExpression;

            if (variableDeclaratorExpression == null)
            {
                return true;
            }

            this.ProcessName(parentElement, variableDeclaratorExpression.Identifier.Text, expression.LineNumber, context);

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse a constructor and the names of its parameters.
        /// </summary>
        /// <param name="constructor">
        /// The constructor. 
        /// </param>
        /// <param name="context">
        /// The context. 
        /// </param>
        private void ParseConstructor(Constructor constructor, CleanCoderAnalyzer context)
        {
            this.ProcessName(constructor, constructor.Declaration.Name, constructor.LineNumber, context);

            foreach (Parameter parameter in constructor.Parameters.ToList())
            {
                this.ProcessName(constructor, parameter.Name, parameter.LineNumber, context);
            }
        }

        /// <summary>
        /// Parse a delegate and the names of its parameters.
        /// </summary>
        /// <param name="delegateObject">
        /// The delegate. 
        /// </param>
        /// <param name="context">
        /// The context. 
        /// </param>
        private void ParseDelegate(Delegate delegateObject, CleanCoderAnalyzer context)
        {
            this.ProcessName(delegateObject, delegateObject.Declaration.Name, delegateObject.LineNumber, context);

            foreach (Parameter parameter in delegateObject.Parameters.ToList())
            {
                this.ProcessName(delegateObject, parameter.Name, parameter.LineNumber, context);
            }
        }

        /// <summary>
        /// Parse an enum and the names of its enum items.
        /// </summary>
        /// <param name="enumObject">
        /// The enum. 
        /// </param>
        /// <param name="context">
        /// The context. 
        /// </param>
        private void ParseEnum(Enum enumObject, CleanCoderAnalyzer context)
        {
            this.ProcessName(enumObject, enumObject.Declaration.Name, enumObject.LineNumber, context);

            foreach (EnumItem enumItem in enumObject.ChildCodeElements.OfType<EnumItem>().ToList())
            {
                this.ProcessName(enumObject, enumItem.Declaration.Name, enumItem.LineNumber, context);
            }
        }

        /// <summary>
        /// Parse and event, including its type.
        /// </summary>
        /// <param name="eventObject">
        /// The event. 
        /// </param>
        /// <param name="context">
        /// The context. 
        /// </param>
        private void ParseEvent(Event eventObject, CleanCoderAnalyzer context)
        {
            this.ProcessName(eventObject, eventObject.Declaration.Name, eventObject.LineNumber, context);

            this.ProcessName(eventObject, eventObject.EventHandlerType.Text, eventObject.LineNumber, context);
        }

        /// <summary>
        /// Parse method and the names of its argument.
        /// </summary>
        /// <param name="method">
        /// The method. 
        /// </param>
        /// <param name="context">
        /// The context. 
        /// </param>
        [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0309:DescriptiveNameTooExplicit", Justification = "It's for a test.")]
        private void ParseMethod(Method method, CleanCoderAnalyzer context)
        {
            string methodName = Utility.TrimGenericType(method.Declaration.Name);

            this.ProcessName(method, methodName, method.LineNumber, context);

            foreach (Parameter parameter in method.Parameters.ToList())
            {
                this.ProcessName(method, parameter.Name, parameter.LineNumber, context);
            }
        }

        /// <summary>
        /// Parse a struct and the names of its parameters.
        /// </summary>
        /// <param name="structObject">
        /// The struct. 
        /// </param>
        /// <param name="context">
        /// The context. 
        /// </param>
        private void ParseStruct(Struct structObject, CleanCoderAnalyzer context)
        {
            this.ProcessName(structObject, structObject.Declaration.Name, structObject.LineNumber, context);

            foreach (Constructor constructor in structObject.ChildCodeElements.OfType<Constructor>().ToList())
            {
                this.ParseConstructor(constructor, context);
            }
        }

        /// <summary>
        /// Validate if a name has non english characters in it.
        /// </summary>
        /// <param name="element">
        /// The element. 
        /// </param>
        /// <param name="name">
        /// The variable name. 
        /// </param>
        /// <param name="lineNumber">
        /// The line number where the variable apperas. 
        /// </param>
        /// <param name="context">
        /// The context, this class. 
        /// </param>
        [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0042:MethodHasTooManyArgument", Justification = "It's a delegate for Analyzer.VisitElement.")]
        private void ProcessName(CsElement element, string name, int lineNumber, CleanCoderAnalyzer context)
        {
            Param.AssertNotNull(element, "element");
            Param.AssertNotNull(name, "name");
            Param.AssertNotNull(lineNumber, "lineNumber");
            Param.AssertNotNull(context, "context");

            Regex englishOnlyRegex = new Regex("^[0-9A-Za-z_.]+$");

            if (englishOnlyRegex.IsMatch(name) == false)
            {
                context.AddViolation(element, lineNumber, this.RuleName, name);
            }
        }

        #endregion
    }
}