using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QiQiaoBan.Common
{
    /// <summary>Abstracts the interface for TransientStateHelper and PersistentStateHelper</summary>
    public interface IStateHelper
    {
        /// <summary>Get an object from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <returns>Returns a value from the state store as an object</returns>
        object this[string key] { get; set; }

        /// <summary>The name of the file used to store app state in the local store</summary>
        string StateFileName { get; }

        /// <summary>Get an object of type T from the state store using the specified key</summary>
        /// <typeparam name="T">The type of the value specified by the key</typeparam>
        /// <param name="key">The key for the state store value</param>
        /// <returns>Returns a value from the state store</returns>
        T Get<T>(string key);

        /// <summary>Get a string value from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from the state store</returns>
        string Get(string key, string defaultValue = "");

        /// <summary>Get a double value from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from the state store</returns>
        double Get(string key, double defaultValue = 0);

        /// <summary>Get a bool value from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from the state store</returns>
        bool Get(string key, bool defaultValue = false);

        /// <summary>Returns true if the selected state store contains the specified key, false otherwise</summary>
        /// <param name="key">The key for the state store value</param>
        /// <returns>Returns true if the selected state store contains the specified key, false otherwise</returns>
        bool ContainsKey(string key);

        /// <summary>Get a DateTime value from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="defaultValue">The DateTime to return if the key doesn't exist</param>
        /// <returns>Returns a DateTime from the state store</returns>
        DateTime Get(string key, DateTime defaultValue);

        /// <summary>Save an object to the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="value">The value to save to the state store</param>
        void Set(string key, object value);

        /// <summary>
        /// Creates a new in-memory state store to hold state values that will be loaded 
        /// or saved from the local state store. This should be done before starting the 
        /// process to save state from the state store (no need to do this when loading)
        /// </summary>
        void CreateInMemoryStateStore();

        /// <summary>Saves the temporary in-memory state store to a file in the local store</summary>
        /// <returns>Returns true if the in-memory state store was saved, false otherwise</returns>
        Task<bool> SaveInMemoryStateToStoreAsync();

        /// <summary>Loads app state from a file in the local store</summary>
        /// <returns>Returns true if state was loaded to the in-memory state store, false if there was an error or the state file doesn't exist</returns>
        Task<bool> LoadInMemoryStateFromStoreAsync();
    }
}