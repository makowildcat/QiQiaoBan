using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Practices.Unity.Utility;

namespace QiQiaoBan.Common
{
    /// <summary>Base class for view models</summary>
    public abstract class ModelBase : INotifyPropertyChanged, IModelBase
    {
        // Events -------------------------------------------------------------

        /// <summary>PropertyChanged event. Raised when any property changes</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        // Private members ----------------------------------------------------

        private readonly string _stateSavedId;
        private readonly IStateHelper _stateHelper;

        // Methods ------------------------------------------------------------

        /// <summary>Constructor</summary>
        protected ModelBase()
        {
            // This will use the name of the class that inherits from ModelBase to be the
            // key we use to define if state has been saved for this object. So individual
            // state items will have keys like "LocationService.TrackLocation"
            _stateSavedId = this.GetType().Name + ".";

            // Create the state helper that will load/save this model's state
            _stateHelper = IocContainer.Get<IStateHelper>("viewModel", this);
        }

        /// <summary>Constructor</summary>
        /// <param name="stateSaveId">Override the name of the stateSaveId (defaults to type name)</param>
        protected ModelBase(string stateSaveId)
        {
            _stateSavedId = stateSaveId + ".";
            _stateHelper = IocContainer.Get<IStateHelper>("viewModel", this);
        }

        /// <summary>
        /// Gets a boolean value specifying if session state data for the object derived from 
        /// this base class has previously been restored. If it has not, any attempt to 
        /// use LoadState() in the derived class will *initialize* state variables
        /// to default type values
        /// </summary>
        /// <returns>Returns true if state is available for restoring, false otherwise</returns>
        public virtual bool IsStateAvailable()
        {
            return _stateHelper.ContainsKey(_stateSavedId);
        }

        /// <summary>Override in derived class. See example usage</summary>
        /// <example>
        /// public override void SaveState()        
        /// {        
        ///     SaveAutoState();  // Save all settings marked with the [AutoState] attribute      
        /// }   
        /// </example>  
        public abstract void SaveState();

        /// <summary>Override in derived class. See example usage</summary>
        /// <example>
        /// public override void LoadState()
        /// {
        ///     LoadAutoState();  // Load all settings marked with the [AutoState] attribute
        /// }
        /// </example>
        public abstract void LoadState();

        /// <summary>Saves an object to the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="value">The value to save</param>
        public virtual void SetStateItem(string key, object value)
        {
            key = _stateSavedId + key;
            _stateHelper[_stateSavedId] = true;
            _stateHelper[key] = value;
        }

        /// <summary>Gets an object from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <returns>Returns the object with the specified key, or null if it doesn't exist in state</returns>
        public virtual object GetStateItem(string key)
        {
            key = _stateSavedId + key;
            return _stateHelper.ContainsKey(key) ? _stateHelper[key] : null;
        }

        /// <summary>Gets an object from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>Returns the object with the specified key, or null if it doesn't exist in state</returns>
        public virtual object GetStateItem(string key, object defaultValue)
        {
            key = _stateSavedId + key;
            return _stateHelper.ContainsKey(key) ? _stateHelper[key] : defaultValue;
        }

        /// <summary>Gets an object of type T from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <returns>Returns the object of type T with the specified key, or the default value of T if it doesn't exist in state</returns>
        public virtual T GetStateItem<T>(string key)
        {
            key = _stateSavedId + key;
            return _stateHelper.ContainsKey(key) ? (T)_stateHelper[key] : default(T);
        }

        /// <summary>Gets an object of type T from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>Returns the object of type T with the specified key, or the default value of T if it doesn't exist in state</returns>
        public virtual T GetStateItem<T>(string key, T defaultValue)
        {
            key = _stateSavedId + key;
            return _stateHelper.ContainsKey(key) ? (T)_stateHelper[key] : defaultValue;
        }

        /// <summary>
        /// Persists state for all properties marked with the [AutoState] attribute for the class that inherits from ModelBase. 
        /// Properties that are arrays or RelayCommands are not saved. Collections of objects that implement the 
        /// IAutoSaveLoad interface are given the opportunity to save themselves by calls to ToStringRepresentation() for
        /// each element in the collection. Null values are not saved        
        /// </summary>
        public virtual void SaveAutoState()
        {
            SaveAuto<AutoState>();
        }

        /// <summary>
        /// Loads state for all properties marked with the [AutoState] attribute for the class that inherits from ModelBase. 
        /// Properties that are arrays or RelayCommands are not restored. Collections of objects that implement the 
        /// IAutoSaveLoad interface are given the opportunity to restore themselves by calls to FromStringRepresentation() for
        /// each element in the saved collection. Null values are not restored  
        /// </summary>
        public virtual void LoadAutoState()
        {
            LoadAuto<AutoState>();
        }

        /// <summary>
        /// Persists state for all properties marked with an attribute that implements IAutoAttribute (e.g. [AutoState] or [AutoSetting]).
        /// Properties that are arrays or RelayCommands are not saved. Collections of objects that implement the 
        /// IAutoSaveLoad interface are given the opportunity to save themselves by calls to ToStringRepresentation() for
        /// each element in the collection. Null values are not saved  
        /// </summary>
        public async virtual void SaveAuto<T>() where T : Attribute, IAutoAttribute
        {
            try
            {
                _stateHelper.CreateInMemoryStateStore();  // Create a new temp store to hold our saved state

                var properties = this.GetType().GetPropertiesHierarchical();

                foreach (var pi in properties)
                {
                    if (pi == null) continue;

                    var ca = pi.GetCustomAttribute<T>();
                    if (ca == null) continue;  // Property was not marked with the custom attribute

                    var type = pi.PropertyType;

                    if (type == typeof(RelayCommand)) continue;
                    if (type.IsArray)
                    {
                        Logger.Log("Array properties cannot be auto-saved. Consider converting to a collection");
                        continue;
                    }

                    Logger.Log("Saving state for " + _stateSavedId + pi.Name);

                    var val = pi.GetValue(this);
                    if (val == null)
                    {
                        // The object's null, if it has a default value, save that
                        if (ca.DefaultValue != null) SetStateItem(pi.Name, ca.DefaultValue);
                        else if (ca.SaveNullValues && TypeCanBeNull(type)) SetStateItem(pi.Name, null);  // Or save a null if that's allowed
                        continue;  // Don't save the value
                    }

                    if (type.FullName.ToLower().StartsWith("system.collections"))
                    {
                        // Type is a collection - flatten all elements it into a single string by treating 
                        // it as simple IEnumerable (which all collections should implement)
                        var collection = val as IEnumerable;
                        if (collection == null)
                        {
                            Logger.Log("Cannot save " + pi.Name + ". Type does not implement IEnumerable");
                            continue;
                        }

                        var flattenedCollection = new StringBuilder();
                        foreach (var item in collection)
                        {
                            // Try to get the collection item to stringify itself (it needs to implement IAutoSaveLoad),
                            // otherwise we use the item's ToString() value
                            var canSaveItself = item as IAutoSaveLoad;
                            flattenedCollection.Append(canSaveItself != null ? canSaveItself.ToStringRepresentation() : item.ToString());
                            flattenedCollection.Append(";");
                        }

                        var s = flattenedCollection.ToString();  // If the collection is empty, we save the property as an empty string
                        SetStateItem(pi.Name, s.TrimEnd(new[] { ';' }));  // e.g. "99|52|0|0|101;100|53|5|0|102;101|55|0|0|103;102|53|10|0|104"
                    }
                    else
                    {
                        var canSaveItself = val as IAutoSaveLoad;
                        if (canSaveItself != null)
                        {
                            var flatValue = canSaveItself.ToStringRepresentation();
                            SetStateItem(pi.Name, flatValue);
                        }
                        else
                        {
                            // The type can be serialized by the state helper - save the value
                            SetStateItem(pi.Name, val);
                        }
                    }
                }

                if (!await _stateHelper.SaveInMemoryStateToStoreAsync())
                    Logger.Log("StateHelper was unable to save state to the local store");
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Error saving state for " + _stateSavedId);
            }
        }

        /// <summary>
        /// Loads state for all properties marked with an attribute that implements IAutoAttribute (e.g. [AutoState] or [AutoSetting]).
        /// Properties that are arrays or RelayCommands are not restored. Collections of objects that implement the 
        /// IAutoSaveLoad interface are given the opportunity to restore themselves by calls to FromStringRepresentation() for
        /// each element in the saved collection. Null values are not restored 
        /// </summary>
        public async virtual void LoadAuto<T>() where T : Attribute, IAutoAttribute
        {
            try
            {
                var savedStateExists = await _stateHelper.LoadInMemoryStateFromStoreAsync();  // State may be missing if newly deployed app, etc.
                var properties = this.GetType().GetPropertiesHierarchical();  // Get all the properties for the view model

                foreach (var pi in properties)
                {
                    if (pi == null) continue;

                    var ca = pi.GetCustomAttribute<T>();  // See if the property is marked with [AutoSave]
                    if (ca == null) continue;  // Property was not marked with the [AutoSave] attribute

                    var type = pi.PropertyType;
                    if (type == typeof(RelayCommand)) continue;  // No need to save this type of property
                    if (type.IsArray)
                    {
                        Logger.Log("Unable to restore state for " + type.Name + " in " + _stateSavedId);
                        Logger.Log("Array properties cannot be auto-loaded. Consider using a collection instead");
                        continue;
                    }

                    var val = pi.GetValue(this);  // This can be null (e.g. a null ObservableCollection<T>)
                    var stateVal = savedStateExists ? GetStateItem(pi.Name) : null;  // Get the value from the state store (if it exists)

                    Logger.Log("Loading state for " + _stateSavedId + pi.Name);

                    if (stateVal != null)
                    {
                        var typeIsCollection = type.FullName.ToLower().StartsWith("system.collections");
                        if (typeIsCollection)
                        {
                            // Restore a collection...

                            // First, split the flattened collection of items into individuals rows
                            var rows = stateVal.ToString().Split(new[] { ';' });
                            if (rows.Length == 0) continue;

                            // We now need to create an instance of the collection to receive the state we're going to load
                            var collection = (IList)Activator.CreateInstance(type);

                            // See if we're dealing with a generic collection (e.g. ObservableCollection<string> 
                            // or ObservableCollection<MyType>)
                            var genericArgs = type.GenericTypeArguments;

                            if (genericArgs == null)
                            {
                                Logger.Log("Unable to restore state for " + type.Name + " in " + _stateSavedId);
                                Logger.Log("Can't determine the generic type of the collection");
                                continue;  // Couldn't work out the generic type
                            }

                            if (genericArgs.Length != 1)
                            {
                                Logger.Log("Unable to restore state for " + type.Name + " in " + _stateSavedId);
                                Logger.Log("Too many generic collection types (LoadAuto<T>() only supports collections with a single generic type)");
                                continue;  // We only support collections with a single generic type
                            }

                            var genericCollectionType = genericArgs[0];

                            // Is the generic collection type a fundamental type (string, int, etc.) or a custom type?
                            if (IsFundamentalType(genericCollectionType))
                            {
                                // It's a fundamental type 
                                // Is it a collection of strings?
                                if (genericCollectionType == typeof(string))
                                {
                                    foreach (var sRow in rows)
                                        if (sRow != null) collection.Add(sRow);
                                }
                                else
                                {
                                    // It's a collection of ints, floats, doubles, etc.
                                    foreach (var row in rows)
                                    {
                                        var collectionItem = CastFundamentalTypeValue(row, genericCollectionType);
                                        if (collectionItem == null) continue; // Skip null value (we couldn't cast it)
                                        collection.Add(collectionItem);
                                    }
                                }
                            }
                            else
                            {
                                // It's a custom type. For each row we ask the type to provide a string representation of its value
                                foreach (var row in rows)
                                {
                                    // The custom generic collection type needs to implement IAutoSaveLoad, if not
                                    // the following assignment will fail with an exception
                                    var collectionItem = (IAutoSaveLoad)Activator.CreateInstance(genericCollectionType);
                                    if (collectionItem.FromStringRepresentation(row) == null) continue;  // The object didn't want the item added to the collection
                                    collection.Add(collectionItem);
                                }
                            }

                            pi.SetValue(this, collection);  // Restore the property using the stored state values
                        }
                        else
                        {
                            // Restore a scalar object...

                            if (val == null && !IsFundamentalType(type)) val = Activator.CreateInstance(type);

                            var canRestoreItself = val as IAutoSaveLoad;
                            if (canRestoreItself != null)
                            {
                                // This type wants to restore itself from a string
                                var rehydratedValue = canRestoreItself.FromStringRepresentation(stateVal.ToString());
                                pi.SetValue(this, rehydratedValue);
                            }
                            else pi.SetValue(this, stateVal); // The state helper knows how to restore this type from state
                        }
                    }
                    else
                    {
                        // The value in the state store is null - restore it to the property?
                        if (ca.RestoreNullValues && TypeCanBeNull(type)) pi.SetValue(this, null);  // Restore a null to the property
                        else if (ca.DefaultValue != null) pi.SetValue(this, ca.DefaultValue);  // Restore a default value
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Error restoring state for type " + _stateSavedId);
            }
        }

        /// <summary>Returns true if the type is a fundametal type like a string, int, double, bool, etc.</summary>
        /// <param name="t">The type to inspect</param>
        /// <returns>Returns true if the type is a string, int, double, bool, etc.</returns>
        private static bool IsFundamentalType(Type t)
        {
            // Types with null values can't be fundamental types (except string)
            if (t == null) return false;

            return t == typeof(string) || t.GetTypeInfo().IsValueType;
        }

        /// <summary>Returns true if a type can be given a null value, false otherwise</summary>
        /// <param name="t">The type to test</param>
        /// <returns>Returns true if a type can be given a null value, false otherwise</returns>
        private static bool TypeCanBeNull(Type t)
        {
            return t == null || !t.GetTypeInfo().IsValueType;
        }

        /// <summary>Returns the string value cast to the appropriate type, or null if the cast failed</summary>
        /// <param name="val">Value as a string</param>
        /// <param name="t">The type to cast the string value to</param>
        /// <returns>Returns the string value cast to the appropriate type, or null if the cast failed</returns>
        private static object CastFundamentalTypeValue(string val, Type t)
        {
            try
            {
                if (t == typeof(int)) return int.Parse(val);
                if (t == typeof(double)) return double.Parse(val);
                if (t == typeof(float)) return float.Parse(val);
                if (t == typeof(bool)) return bool.Parse(val);
                if (t == typeof(short)) return short.Parse(val);
            }
            catch (Exception ex)
            {
                Logger.Log("Warning: CastFundamentalTypeValue. Unable to cast \"" + val + "\" to " + t.Name);
            }

            return null;
        }

        /// <summary>Raises the PropertyChanged event</summary>
        /// <param name="propertyName">The name of the property that has changed, null to use the CallerMemberName attribute</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}