using System;
using System.Collections.Generic;
using System.Text;

namespace QiQiaoBan.Common
{
    /// <remarks>
    /// Note that the following rules apply to auto-saving/loading object properties 
    /// decorated with the [AutoState] attribute:
    /// 
    ///     * Your custom object (e.g. a ViewModel) should derive from ModelBase. This 
    ///       provides base methods for the saving/loading of properties to/from local
    ///       state storage. It also defines the abstract methods SaveState() and 
    ///       LoadState(), which you must override in your own class. Normally, 
    ///       these overridden methods will simply contain calls to the base class' 
    ///       SaveState() and LoadState() methods
    /// 
    ///     * Any object wishing to auto-save/load its properties must hook into
    ///       the main Save/Load mechanism that is triggered when the View handles 
    ///       the OnNavigatedTo/OnNavigatedFrom events. When these events are handled, 
    ///       the View should call the ViewModel's SaveState() and LoadState() methods.
    ///       The ViewModel SaveState() and LoadState() methods should then call the
    ///       ModelBase SaveAutoState()/LoadAutoState() methods. If you are using the
    ///       Common.NavigationHelper class in your view, you should place calls to
    ///       ViewModel.SaveState() and ViewModel.LoadState() in the handlers for the
    ///       NavigationHelper.LoadState and NavigationHelper.SaveState events:
    /// 
    ///         public View1()
    ///         {
    ///             this.InitializeComponent();
    ///         
    ///             NavigationCacheMode = NavigationCacheMode.Required;  
    ///         
    ///             _navigationHelper = new NavigationHelper(this);
    ///             _navigationHelper.LoadState += NavigationHelperLoadState;
    ///             _navigationHelper.SaveState += NavigationHelperSaveState;
    ///         }
    ///         
    ///         public void NavigationHelperLoadState(object sender, LoadStateEventArgs e)
    ///         {
    ///             ViewModel.LoadState();  // Load ViewModel state
    ///         }
    ///         
    ///         public void NavigationHelperSaveState(object sender, SaveStateEventArgs e)
    ///         {
    ///             ViewModel.SaveState();  // Save ViewModel state
    ///         }
    /// 
    ///     * You may provide default values as in the following examples: 
    ///         
    ///         [AutoState(DefaultValue: -1)]
    ///         public int MyInt  { get; set; }
    /// 
    ///         [AutoState(DefaultValue: "MyStringProperty")]
    ///         public string MyString { get; set; }
    /// 
    ///     * You may specifiy how you want to deal with null values through the use
    ///       the SaveNullValues and RestoreNullValues parameters. For example:
    /// 
    ///         [AutoState(SaveNullValues = true, RestoreNullValues = true)]
    /// 
    ///     * If a property is a collection of a custom type, that type must implement
    ///       the IAutoSaveLoad interface, which provides two methods 
    ///       ToStringRepresentation() and FromStringRepresentation(). These methods
    ///       are called by ModelBase during the save/load process to restore themselves 
    ///       from a flattened (serialized) string representation of the object
    ///       
    ///     * Properties decorated with [AutoState] will be persisted and restored from 
    ///       local state storage as follows:
    /// 
    ///         + Handled automatically:
    ///             - Fundamental types (string, int, bool, etc.)
    ///             - Collections of fundametal types (e.g. string, int, etc.). 
    ///             - Custom types with properties of fundamental type
    ///             - Custom types with a mix of custom and fundamental properties
    /// 
    ///         + Handled with help from IAutoSaveLoad
    ///             - Generic collections (e.g. List, ObservableCollection) of custom type
    /// 
    ///         + Not handled:
    ///             - RelayCommand types
    ///             - Arrays (implement as a collection)
    /// </remarks> 
    public interface IAutoAttribute
    {
        /// <summary>The default value for a property if the state store does not contain an entry for the property</summary>
        object DefaultValue { get; set; }

        /// <summary>True if you want null values to be saved, false otherwise</summary>
        bool SaveNullValues { get; set; }

        /// <summary>True if you want null values to be restored, false otherwise</summary>
        bool RestoreNullValues { get; set; }
    }
}