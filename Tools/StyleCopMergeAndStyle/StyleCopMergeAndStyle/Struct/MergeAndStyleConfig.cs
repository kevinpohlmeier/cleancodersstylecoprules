﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeAndStyleConfig.cs" company="None, it's free for all.">
//   Copyright (c) None, it's free for all. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StyleCopMergeAndStyle.Struct
{
    using System.Collections.Generic;

    /// <summary>
    ///   The merge and style config parameters.
    /// </summary>
    public class MergeAndStyleConfig
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the merge file name.
        /// </summary>
        /// <value> The merge file name. </value>
        public string MergeFileName { get; set; }

        /// <summary>
        ///   Gets or sets the list of violation files.
        /// </summary>
        /// <value> The violation files. </value>
        public List<string> ViolationFiles { get; set; }

        #endregion
    }
}