using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Phone.Common.Diagnostics.Logging;
using Serialization;

namespace Phone.Common.IO
{
    public enum CacheStates
    {
        NotFound,
        Exists,
        Expired
    }

    /// <summary>
    /// 
    /// </summary>
    public class Cache
    {
        public static readonly DateTime NoAbsoluteExpiration = DateTime.MaxValue;
        public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;

        readonly IsolatedStorageFile _myStore = IsolatedStorageFile.GetUserStoreForApplication();

        private object _sync = new object();


        private static Cache _current;
        /// <summary>
        /// Gets the current instance of the cache
        /// </summary>
        /// <value>The current.</value>
        public static Cache Current
        {
            get { return _current ?? (_current = new Cache()); }
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="absoluteExpiration">The absolute expiration.</param>
        /// <param name="slidingExpiration">The sliding expiration.</param>
        public void Add(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            lock (_sync)
            {
                if (Contains(key))
                    Remove(key);

                if (absoluteExpiration == NoAbsoluteExpiration)
                    Add(key, DateTime.UtcNow + slidingExpiration, value);
                if (slidingExpiration == NoSlidingExpiration)
                    Add(key, absoluteExpiration, value);
            }
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="expirationDate">The expiration date.</param>
        /// <param name="value">The value.</param>
        private void Add(string key, DateTime expirationDate, object value)
        {
            lock (_sync)
            {
                if (!_myStore.DirectoryExists(key))
                    _myStore.CreateDirectory(key);
                else
                {
                    string currentFile = GetFileNames(key).FirstOrDefault();
                    if (currentFile != null)
                        _myStore.DeleteFile(string.Format("{0}\\{1}", key, currentFile));
                    if (DeleteDirectory(key))
                        _myStore.CreateDirectory(key);
                }

                string fileName = string.Format("{0}\\{1}.cache", key, expirationDate.ToFileTimeUtc());

                if (_myStore.FileExists(fileName))
                    _myStore.DeleteFile(fileName);

                NormalWrite(fileName, value);
            }
        }

        // slightly more efficient version if info previously retrieved
        public void Add(CacheInfo info, DateTime absoluteExpiration, TimeSpan slidingExpiration, object value)
        {
            lock (_sync)
            {
                if (!info.DirectoryExists)
                {
                    _myStore.CreateDirectory(info.Key);
                }
                else
                {
                    if (info.File != null)
                        _myStore.DeleteFile(info.Filename);
                    
                    // why recreate the directory?
                    try
                    {
                        if (DeleteDirectory(info.Key))
                            _myStore.CreateDirectory(info.Key);
                    }
                    catch (IsolatedStorageException isoEx)
                    {
                        // tombstone / threading most likely
                        LogInstance.LogError("Error with cache add directory recreate for key {0}. Error: {1}", info.Key, isoEx.ToString());
                    }
                }

                var expirationDate = (absoluteExpiration == NoAbsoluteExpiration)
                    ? DateTime.UtcNow + slidingExpiration : absoluteExpiration;

                var fileName = string.Format("{0}\\{1}.cache", info.Key, expirationDate.ToFileTimeUtc());

                if (_myStore.FileExists(fileName))
                    _myStore.DeleteFile(fileName);

                NormalWrite(fileName, value);
            }
        }

        /// <summary>
        /// Gets the state of the given item without actually removing it from cache if there and expired.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheInfo Info(string key)
        {
            lock (_sync)
            {
                var info = new CacheInfo
                {
                    State = CacheStates.NotFound,
                    Key = key,
                    DirectoryExists = _myStore.DirectoryExists(key)
                };

                info.File = (info.DirectoryExists ? GetFileNames(key).FirstOrDefault() : null);

                if (info.DirectoryExists && !string.IsNullOrEmpty(info.File))
                {
                    info.Filename = string.Format(@"{0}\{1}", key, info.File);
                    info.Expiration = DateTime.FromFileTimeUtc(long.Parse(
                        Path.GetFileNameWithoutExtension(info.File)));
                    info.State = (info.Expiration >= DateTime.UtcNow) ? CacheStates.Exists : CacheStates.Expired;
                }
                return info;
            }
        }

        /// <summary>
        /// Determines whether the cache contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string key)
        {
            lock (_sync)
            {
                if (_myStore.DirectoryExists(key) && GetFileNames(key).Any())
                {
                    string currentFile = GetFileNames(key).FirstOrDefault();
                    if (currentFile != null)
                    {
                        var expirationDate =
                            DateTime.FromFileTimeUtc(long.Parse(Path.GetFileNameWithoutExtension(currentFile)));
                        if (expirationDate >= DateTime.UtcNow)
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            lock (_sync)
            {
                if (!Contains(key))
                    throw new AccessViolationException("The key does not exist in the cache");
                string currentFile = GetFileNames(key).FirstOrDefault();
                if (currentFile != null)
                    _myStore.DeleteFile(string.Format("{0}\\{1}", key, currentFile));
                DeleteDirectory(key);
            }
        }

        private bool DeleteDirectory(string key)
        {
            // app settings .cache file is being placed in a cache dir so it's preventing us from deleting dir
            // http://blogs.ugidotnet.org/corrado/archive/2010/10/15/beware-of-.cache-files-on-isolatedstorage.aspx
            if (!_myStore.GetFileNames().Any())
            {
                _myStore.DeleteDirectory(key);
                return true;
            }

            LogInstance.LogInfo("Could not remove directory '{0}'; one or more files exist", key);
            return false;
        }

        /// <summary>
        /// Gets the file names.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private IEnumerable<string> GetFileNames(string key)
        {
            return _myStore.GetFileNames(string.Format("{0}\\*.cache", key));
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            lock (_sync)
            {
                string currentFile = GetFileNames(key).FirstOrDefault();
                if (currentFile != null)
                {
                    var expirationDate =
                        DateTime.FromFileTimeUtc(long.Parse(Path.GetFileNameWithoutExtension(currentFile)));
                    if (expirationDate >= DateTime.UtcNow)
                    {
                        var filename = string.Format(@"{0}\{1}", key, currentFile);
                        return NormalRead<T>(filename, key);
                    }
                    Remove(key);
                }
                return default(T);
            }

        }

        /// <summary>
        /// More efficient way of getting cached item if its info/state has already been checked
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="expiredOkay">If true, any expired data is returned and is not removed from cache (i.e. network error situation)</param>
        /// <returns></returns>
        public T Get<T>(CacheInfo info, bool expiredOkay = false)
        {
            lock (_sync)
            {
                if (info.File != null && info.State != CacheStates.NotFound)
                {
                    if (expiredOkay || info.Expiration >= DateTime.UtcNow)
                    {
                        return NormalRead<T>(info.Filename, info.Key);
                    }
                    
                    Remove(info.Key);
                }
                return default(T);
            }

        }

        #region Serialization

        private T NormalRead<T>(string fileName, string key)
        {
            using (var fileStream = new IsolatedStorageFileStream(fileName, FileMode.Open, _myStore))
            {
                // this is for debugging purposes only when there are deserialization issues:
                //Debug.WriteLine(fileStream.Length);
                //var text = GetFileContents(fileStream);

                // SilverlightSerializer is much faster yo:
                //var s = new DataContractSerializer(typeof(T));
                object value = null;

                try
                {
                    value = SilverlightSerializer.Deserialize(fileStream);
                    //value = s.ReadObject(fileStream);
                }
                catch (SerializationException serEx)
                {
                    HandleReadError(fileStream, serEx, key);
                    return default(T);
                }
                catch (XmlException xmlEx)
                {
                    HandleReadError(fileStream, xmlEx, key);
                    return default(T);
                }
                catch (Exception ex)
                {
                    HandleReadError(fileStream, ex, key);
                    return default(T);
                }

                fileStream.Close();
                return (T)value;
            }
        }

        private void HandleReadError(Stream stream, Exception ex, string key)
        {
            LogInstance.LogException(ex);

            try
            {
                stream.Close();
            }
            catch { }

            // assume the data is bad. in one case empty string / no root element
            Remove(key);
        }

        private string GetFileContents(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                // was empty string in this case
                var contents = reader.ReadToEnd();
                return contents;
            }
        }

        private void NormalWrite(string fileName, object value)
        {
            using (var fileStream = new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, _myStore))
            {
                // DataContractSerializer is too damn slow
                SilverlightSerializer.Serialize(value, fileStream);

                //var s = new DataContractSerializer(value.GetType());
                //s.WriteObject(isolatedStorageFileStream, value);
            }
        }

        #endregion
    }

    public class CacheInfo
    {
        public string Key { get; internal set; }
        public CacheStates State { get; internal set; }
        public string File { get; internal set; }
        public string Filename { get; internal set; }
        public DateTime Expiration { get; internal set; }
        public bool DirectoryExists { get; internal set; }
    }
}