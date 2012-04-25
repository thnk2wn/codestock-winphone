namespace Phone.Common.Diagnostics.Logging
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    public enum LogAutoPersist
    {
        /// <summary>
        /// Never automatically persisted to a file; must explicitly persist
        /// </summary>
        None,

        /// <summary>
        /// Automatically written to storage periodically on a thread as well as when disposed
        /// </summary>
        Periodic,

        /// <summary>
        /// Only automatically written on dispose
        /// </summary>
        Dispose
    }
}
