using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace QiQiaoBan.Common
{
    /// <summary>Provides helper methods to get/set data to the local app data store</summary>
    public class LocalStoreStateHelper : IStateHelper
    {
        private const string _stateFileName = "AppState.xml";
        private readonly string _modelName;
        private Dictionary<string, object> _inMemoryStateStore;

        /// <summary>Get an object from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <returns>Returns a value from the state store as an object (null if the key doesn't exist)</returns>
        public object this[string key]
        {
            get { return _inMemoryStateStore[key]; }
            set { _inMemoryStateStore[key] = value; }
        }

        /// <summary>The name of the file used to store app state in the local store</summary>
        public string StateFileName { get { return "_" + _modelName + "_" + _stateFileName; } }

        /// <summary>Constructor</summary>
        /// <param name="viewModel">The model (must implement IModelBase) that will use LocalStoreStateHelper</param>
        public LocalStoreStateHelper(IModelBase viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException();
            _modelName = viewModel.GetType().Name;
        }

        /// <summary>Get an object of type T from IsolatedStorageSettings using the specified key</summary>
        /// <typeparam name="T">The type of the value specified by the key</typeparam>
        /// <param name="key">The key for the IsolatedStorageSettings value</param>
        /// <returns>Returns a value from IsolatedStorageSettings</returns>
        public T Get<T>(string key)
        {
            // If the setting doesn't exist, create it with a default value
            if (!_inMemoryStateStore.ContainsKey(key)) _inMemoryStateStore[key] = default(T);
            return (T)_inMemoryStateStore[key];
        }

        /// <summary>Get a string value from IsolatedStorageSettings using the specified key</summary>
        /// <param name="key">The key for the IsolatedStorageSettings value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from IsolatedStorageSettings</returns>
        public string Get(string key, string defaultValue = "")
        {
            // If the setting doesn't exist, create it with a default value
            if (!_inMemoryStateStore.ContainsKey(key)) _inMemoryStateStore[key] = defaultValue;
            return _inMemoryStateStore[key] as string;
        }

        /// <summary>Get a double value from IsolatedStorageSettings using the specified key</summary>
        /// <param name="key">The key for the IsolatedStorageSettings value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from IsolatedStorageSettings</returns>
        public double Get(string key, double defaultValue = 0)
        {
            // If the setting doesn't exist, create it with a default value
            if (!_inMemoryStateStore.ContainsKey(key)) _inMemoryStateStore[key] = defaultValue;
            return (double)_inMemoryStateStore[key];
        }

        /// <summary>Get a bool value from IsolatedStorageSettings using the specified key</summary>
        /// <param name="key">The key for the IsolatedStorageSettings value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from IsolatedStorageSettings</returns>
        public bool Get(string key, bool defaultValue = false)
        {
            // If the setting doesn't exist, create it with a default value
            if (!_inMemoryStateStore.ContainsKey(key)) _inMemoryStateStore[key] = defaultValue;
            return (bool)_inMemoryStateStore[key];
        }

        /// <summary>Returns true if the selected state store contains the specified key, false otherwise</summary>
        /// <param name="key">The key for the state store value</param>
        /// <returns>Returns true if the selected state store contains the specified key, false otherwise</returns>
        public bool ContainsKey(string key) { return _inMemoryStateStore.ContainsKey(key); }

        /// <summary>Get a DateTime value from IsolatedStorageSettings using the specified key</summary>
        /// <param name="key">The key for the IsolatedStorageSettings value</param>
        /// <param name="defaultValue">The DateTime to return if the key doesn't exist</param>
        /// <returns>Returns a DateTime from IsolatedStorageSettings</returns>
        public DateTime Get(string key, DateTime defaultValue)
        {
            // If the setting doesn't exist, create it with a default value
            if (!_inMemoryStateStore.ContainsKey(key)) _inMemoryStateStore[key] = defaultValue;
            return (DateTime)_inMemoryStateStore[key];
        }

        /// <summary>Save an object to IsolatedStorageSettings using the specified key</summary>
        /// <param name="key">The key for the IsolatedStorageSettings value</param>
        /// <param name="value">The value to save</param>
        public void Set(string key, object value) { _inMemoryStateStore[key] = value; }

        /// <summary>
        /// Creates a new in-memory state store to hold state values that will be loaded 
        /// or saved from the local state store. This should be done before starting the 
        /// process to save state from the state store (no need to do this when loading)
        /// </summary>
        public void CreateInMemoryStateStore() { _inMemoryStateStore = new Dictionary<string, object>(); }

        /// <summary>Saves the temporary in-memory state store to a file in the local store</summary>
        /// <returns>Returns true if the in-memory state store was saved, false otherwise</returns>
        public async Task<bool> SaveInMemoryStateToStoreAsync()
        {
            try
            {
                var stateStream = new MemoryStream();
                var dataContractSerializer = new DataContractSerializer(typeof(Dictionary<string, object>));

                dataContractSerializer.WriteObject(stateStream, _inMemoryStateStore);

                var stateFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(StateFileName, CreationCollisionOption.ReplaceExisting);
                using (var stream = await stateFile.OpenStreamForWriteAsync())
                {
                    stateStream.Seek(0, SeekOrigin.Begin);
                    await stateStream.CopyToAsync(stream);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Error saving in-memory state store to local store");
            }

            return false;
        }

        /// <summary>Loads app state from a file in the local store</summary>
        /// <returns>Returns true if state was loaded to the in-memory state store, false if there was an error or the state file doesn't exist</returns>
        public async Task<bool> LoadInMemoryStateFromStoreAsync()
        {
            try
            {
                CreateInMemoryStateStore();  // Create a new temp store to hold in-coming state values
                StorageFile stateFile;

                try
                {
                    // Get the input stream for the SessionState file (which may not exist)
                    stateFile = await ApplicationData.Current.LocalFolder.GetFileAsync(StateFileName);
                }
                catch (FileNotFoundException)
                {
                    // Unable to load state because the state file doesn't exist in the local store. 
                    // This is not necessarily an error, the state file will be created when state is next saved
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex, "Error loading state from local store");
                    return false;
                }

                using (var inStream = await stateFile.OpenSequentialReadAsync())
                {
                    var dataContractSerializer = new DataContractSerializer(typeof(Dictionary<string, object>));
                    _inMemoryStateStore = (Dictionary<string, object>)dataContractSerializer.ReadObject(inStream.AsStreamForRead());
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Error loading state from local store");
            }

            return false;
        }
    }
}