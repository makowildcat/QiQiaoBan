using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace QiQiaoBan.Common
{
    class ItemsControlCanvas : ItemsControl
    {
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
        {
            Binding leftBinding = new Binding() { Path = new PropertyPath("Left"), Mode = BindingMode.TwoWay };
            Binding topBinding = new Binding() { Path = new PropertyPath("Top"), Mode = BindingMode.TwoWay };

            FrameworkElement contentControl = (FrameworkElement)element;
            contentControl.SetBinding(Canvas.LeftProperty, leftBinding);
            contentControl.SetBinding(Canvas.TopProperty, topBinding);

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
