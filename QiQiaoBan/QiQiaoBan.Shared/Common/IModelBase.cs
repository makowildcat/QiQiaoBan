using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QiQiaoBan.Common
{
    /// <summary>Interface for base class for view models</summary>
    public interface IModelBase
    {
        /// <summary>PropertyChanged event. Raised when any property changes</summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a boolean value specifying if session state data for the object derived from 
        /// this base class has previously been loaded. If it has not, any attempt to 
        /// use LoadState() in the derived class will *initialize* state variables
        /// to default type values
        /// </summary>
        /// <returns>Returns true if state is available for loading, false otherwise</returns>
        bool IsStateAvailable();

        /// <summary>Override in derived class. See example usage</summary>
        /// <example>
        /// public override void SaveState()        
        /// {        
        ///     SetStateItem("Connected", Connected);        
        ///     SetStateItem("ChannelUri", ChannelUri);
        /// }   
        /// </example>  
        void SaveState();

        /// <summary>Override in derived class. See example usage</summary>
        /// <example>
        /// public override void LoadState()
        /// {
        ///     if(!IsStateAvailable()) return;
        ///
        ///     ChannelUri = GetStateItem("ChannelUri") as Uri;
        ///     Connected = (bool) GetStateItem("Connected");
        /// }
        /// </example>
        void LoadState();

        /// <summary>Saves an object to the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="value">The value to save</param>
        void SetStateItem(string key, object value);

        /// <summary>Gets an object from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <returns>Returns the object with the specified key, or null if it doesn't exist in state</returns>
        object GetStateItem(string key);

        /// <summary>Gets an object from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>Returns the object with the specified key, or null if it doesn't exist in state</returns>
        object GetStateItem(string key, object defaultValue);

        /// <summary>Gets an object of type T from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <returns>Returns the object of type T with the specified key, or the default value of T if it doesn't exist in state</returns>
        T GetStateItem<T>(string key);

        /// <summary>Gets an object of type T from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>Returns the object of type T with the specified key, or the default value of T if it doesn't exist in state</returns>
        T GetStateItem<T>(string key, T defaultValue);

        /// <summary>
        /// Saves state for all properties marked with the [AutoState] attribute for the class that inherits from ModelBase. 
        /// </summary>
        void SaveAutoState();

        /// <summary>
        /// Loads state for all properties marked with an [AutoState] attribute
        /// If the state for a property is not available or null, the default supplied 
        /// with via the attribute is used. For example: [AutoState("Hello")] 
        /// </summary>
        void LoadAutoState();

        /// <summary>
        /// Saves state for all properties marked with an attribute that implements IAutoAttribute (e.g. [AutoState]).
        /// </summary>
        void SaveAuto<T>() where T : Attribute, IAutoAttribute;

        /// <summary>
        /// Loads state for all properties marked with an attribute that implements IAutoAttribute (e.g. [AutoState]).
        /// If the state for a property is not available or null, the default supplied 
        /// with via the attribute is used. For example: [AutoState("Hello")] 
        /// </summary>
        void LoadAuto<T>() where T : Attribute, IAutoAttribute;
    }
}