// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Connect.cs" company="None, it's free for all.">
//   Copyright (c) None, it's free for all. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TrailingSpacesRemover
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    using System.Text;

    using EnvDTE;

    using EnvDTE80;

    using Extensibility;

    using Microsoft.VisualStudio.CommandBars;

    /// <summary>
    ///   The object for implementing an Add-in.
    /// </summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        #region Fields

        /// <summary>
        ///   The add-in instance.
        /// </summary>
        private AddIn addInInstance;

        /// <summary>
        ///   The application object.
        /// </summary>
        private DTE2 applicationObject;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.
        /// </summary>
        /// <param name="commandName">
        /// The name of the command to execute. 
        /// </param>
        /// <param name="executeOption">
        /// Describes how the command should be run. 
        /// </param>
        /// <param name="varIn">
        /// Parameters passed from the caller to the command handler. 
        /// </param>
        /// <param name="varOut">
        /// Parameters passed from the command handler to the caller. 
        /// </param>
        /// <param name="handled">
        /// Informs the caller if the command was handled or not. 
        /// </param>
        [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0042:MethodHasTooManyArgument", Justification = "No choice, required by the Add-In API.")]
        [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0041:MethodHasBooleanParameter", Justification = "No choice, required by the Add-In API.")]
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;

            if (executeOption != vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                return;
            }

            if (commandName == "TrailingSpacesRemover.Connect.TrailingSpacesRemover")
            {
                TextDocument textDocument = (TextDocument)this.applicationObject.ActiveDocument.Object("TextDocument");

                EditPoint editPoint = textDocument.StartPoint.CreateEditPoint();

                editPoint.EndOfDocument();

                string allText = editPoint.GetLines(1, editPoint.Line + 1);

                string[] allLines = allText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                StringBuilder stringBuilder = new StringBuilder();

                for (int lineOffset = 0; lineOffset <= allLines.Length - 1; lineOffset++)
                {
                    if (lineOffset != allLines.Length - 1)
                    {
                        stringBuilder.AppendLine(allLines[lineOffset].TrimEnd());
                    }
                    else
                    {
                        stringBuilder.Append(allLines[lineOffset].TrimEnd());
                    }
                }

                textDocument.Selection.SelectAll();
                editPoint.StartOfDocument();
                textDocument.Selection.Delete();
                editPoint.Insert(stringBuilder.ToString());

                Window window = this.applicationObject.Windows.Item(Constants.vsWindowKindOutput);
                OutputWindow outputWindow = (OutputWindow)window.Object;
                outputWindow.ActivePane.Activate();
                outputWindow.ActivePane.OutputString("Trailing spaces removed." + Environment.NewLine);

                handled = true;
            }
        }

        /// <summary>
        /// Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.
        /// </summary>
        /// <param name="customs">
        /// Array of parameters that are host application specific. 
        /// </param>
        /// <seealso class="IDTExtensibility2"/>
        public void OnAddInsUpdate(ref Array customs)
        {
        }

        /// <summary>
        /// Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.
        /// </summary>
        /// <param name="customs">
        /// Array of parameters that are host application specific. 
        /// </param>
        /// <seealso class="IDTExtensibility2"/>
        public void OnBeginShutdown(ref Array customs)
        {
        }

        /// <summary>
        /// Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.
        /// </summary>
        /// <param name="application">
        /// Root object of the host application. 
        /// </param>
        /// <param name="connectMode">
        /// Describes how the Add-in is being loaded. 
        /// </param>
        /// <param name="addInInst">
        /// Object representing this Add-in. 
        /// </param>
        /// <param name="customs">
        /// The customs. 
        /// </param>
        /// <seealso class="IDTExtensibility2"/>
        [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0042:MethodHasTooManyArgument", Justification = "No choice, required by the Add-In API.")]
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array customs)
        {
            this.applicationObject = (DTE2)application;

            this.addInInstance = (AddIn)addInInst;

            if (connectMode != ext_ConnectMode.ext_cm_UISetup)
            {
                return;
            }

            try
            {
                object[] contextGuids = new object[] { };

                Commands2 commands = (Commands2)this.applicationObject.Commands;

                string toolsMenuName = this.GetToolsMenuName();

                CommandBar menuBarCommandBar = ((CommandBars)this.applicationObject.CommandBars)["MenuBar"];

                CommandBarControl toolsCommandBarControl = menuBarCommandBar.Controls[toolsMenuName];

                CommandBarPopup toolsCommandBarPopup = (CommandBarPopup)toolsCommandBarControl;

                Command command = commands.AddNamedCommand2(
                    this.addInInstance, 
                    "TrailingSpacesRemover", 
                    "Trailing Spaces Remover", 
                    "Executes the command for Trailing Spaces Remover", 
                    false, 
                    1, 
                    ref contextGuids, 
                    (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, 
                    (int)vsCommandStyle.vsCommandStylePictAndText, 
                    vsCommandControlType.vsCommandControlTypeButton);

                if ((command != null) && (toolsCommandBarPopup != null))
                {
                    command.AddControl(toolsCommandBarPopup.CommandBar, 1);
                }
            }
            catch (ArgumentException)
            {
                Console.Out.WriteLine("Trailing Spaces Remove command already exists.");
            }
        }

        /// <summary>
        /// Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.
        /// </summary>
        /// <param name="disconnectMode">
        /// Describes how the Add-in is being unloaded. 
        /// </param>
        /// <param name="customs">
        /// Array of parameters that are host application specific. 
        /// </param>
        /// <seealso class="IDTExtensibility2"/>
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array customs)
        {
        }

        /// <summary>
        /// Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.
        /// </summary>
        /// <param name="customs">
        /// Array of parameters that are host application specific. 
        /// </param>
        /// <seealso class="IDTExtensibility2"/>
        public void OnStartupComplete(ref Array customs)
        {
        }

        /// <summary>
        /// Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated
        /// </summary>
        /// <param name="commandName">
        /// The name of the command to determine state for. 
        /// </param>
        /// <param name="neededText">
        /// Text that is needed for the command. 
        /// </param>
        /// <param name="status">
        /// The state of the command in the user interface. 
        /// </param>
        /// <param name="commandText">
        /// Text requested by the neededText parameter. 
        /// </param>
        /// <seealso class="Exec"/>
        [SuppressMessage("CleanCodersStyleCopRules.CleanCoderAnalyzer", "CC0042:MethodHasTooManyArgument", Justification = "No choice, required by the Add-In API.")]
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
        {
            if (neededText != vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                return;
            }

            if (commandName == "TrailingSpacesRemover.Connect.TrailingSpacesRemover")
            {
                status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Get the Tools menu name, while supporting localisation of Visual Studio in multiple languages.
        /// </summary>
        /// <returns> The tools menu name. </returns>
        private string GetToolsMenuName()
        {
            string toolsMenuName = "Tools";

            ResourceManager resourceManager = new ResourceManager("TrailingSpacesRemover.CommandBar", Assembly.GetExecutingAssembly());

            CultureInfo cultureInfo = new CultureInfo(this.applicationObject.LocaleID);

            string resourceName = string.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");

            if (string.IsNullOrEmpty(resourceManager.GetString(resourceName)) == false)
            {
                toolsMenuName = resourceManager.GetString(resourceName);
            }

            return toolsMenuName;
        }

        #endregion
    }
}