// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullLoggingService.cs" company="XamlNinja">
//   2011 Richard Griffin and Ollie Riches
// </copyright>
// <summary>
//   Null logging service - several service require an implementation of the ILog interface, they are initialised
//   with this version.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Phone.Common.Diagnostics.Logging
{
    /// <summary>
    /// Null logging service - several services require an implementation of the ILog interface, they are initialised
    /// with this version.
    /// </summary>
    public sealed class NullLoggingService : ILogManager
    {
        /// <summary>
        /// Event fired when log file has been modified - this implementation does nothing.
        /// </summary>
        public event EventHandler LogModified;

        /// <summary>
        /// Writes a formatted message to the log with the arguments- this implementation does nothing.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message">
        /// The message to be written.
        /// </param>
        /// <param name="args">
        /// The message argument.
        /// </param>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILog Write(LogLevel level, string message, params object[] args)
        {
            return this;
        }

        /// <summary>
        /// Writes diagnostics information about the current state of the device to the log - this implementation does nothing.
        /// </summary>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILog WriteDiagnostics()
        {
            return this;
        }

        /// <summary>
        /// Gets the path to the log file - this implementation returns a null.
        /// </summary>
        public string LogPath
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets all the current message in the file - this implementation returns an empty.
        /// </summary>
        public IEnumerable<string> Messages
        {
            get { return Enumerable.Empty<string>(); }
        }

        /// <summary>
        /// Enables the logging service - this implementation does nothing.
        /// </summary>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILogManager Enable()
        {
            return this;
        }

        /// <summary>
        /// Disables the logging service - this implementation does nothing.
        /// </summary>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILogManager Disable()
        {
            return this;
        }

        /// <summary>
        /// Clears the contents of the log file - this implementation does nothing.
        /// </summary>
        /// <returns>
        /// Returns the instance of the logging service - fluent interface style.
        /// </returns>
        public ILogManager Clear()
        {
            return this;
        }

        public string ReadFileText()
        {
            return string.Empty;
        }

        public string LogBuffer()
        {
            return string.Empty;
        }

        public bool DebugOut { get; set; }

        public LogLevel Level { get; set; }

        public LogAutoPersist AutoPersist { get; set; }

        public bool WritePendingToFile()
        {
            return false;
        }

        public string MessageFormat { get; set; }
    }
}