using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace QiQiaoBan.Common
{
    /// <summary>
    /// ItemsControl is wrapping UIElement defined in ContentControl. 
    /// But Canvas must be composed of UIElement in order to make SetLeft, SetTop and SetZIndex working
    /// So we bind it directly with the ContentControl
    /// </summary>
    class ItemsControlCanvas : ItemsControl
    {
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
        {
            Binding leftBinding = new Binding() { Path = new PropertyPath("Left"), Mode = BindingMode.TwoWay };
            Binding topBinding = new Binding() { Path = new PropertyPath("Top"), Mode = BindingMode.TwoWay };
            Binding zIndexBinding = new Binding() { Path = new PropertyPath("ZIndex"), Mode = BindingMode.TwoWay };

            FrameworkElement contentControl = (FrameworkElement)element;
            contentControl.SetBinding(Canvas.LeftProperty, leftBinding);
            contentControl.SetBinding(Canvas.TopProperty, topBinding);
            contentControl.SetBinding(Canvas.ZIndexProperty, zIndexBinding);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
