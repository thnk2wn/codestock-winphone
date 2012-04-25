// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILog.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
//   Interface defining the logging API.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Phone.Common.Diagnostics.Logging
{
    /// <summary>
    /// Interface defining the logging API.
    /// </summary>
    public interface ILog
    {
        ILog Write(LogLevel level, string msg, params object[] args);

        /// <summary>
        /// Writes diagnostics information about the current state of the device to the log.
        /// </summary>
        /// <returns>
        /// Returns the instance of the log manager - fluent interface style.
        /// </returns>
        ILog WriteDiagnostics();
    }
}
