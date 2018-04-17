using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace EditModeSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
         
            this.DataContext = new MainViewModel();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var listView = (ListView)sender;

            // wait for the dispatcher to instantiate the template for the clicked item
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // find the ListViewItem
                var container = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem) as ListViewItem;
                if (container != null)
                {
                    // try to find the element below the cursor that caused the selection
                    VisualTreeHelper.HitTest(
                        container,
                        this.HitTestFilterCallback,
                        result => HitTestResultBehavior.Continue,
                        new PointHitTestParameters(e.GetPosition(container)));

                }
            }), DispatcherPriority.ApplicationIdle);
        }

        private HitTestFilterBehavior HitTestFilterCallback(DependencyObject potentialHitTestTarget)
        {
            if (potentialHitTestTarget is ListViewItem)
            {
                return HitTestFilterBehavior.ContinueSkipSelf;
            }
            else
            {
                // find the first focusable element
                var element = potentialHitTestTarget as IInputElement;
                if (element != null && element.Focusable)
                {
                    if (element.Focus())
                    {
                        return HitTestFilterBehavior.Stop;
                    }
                }
            }

            return HitTestFilterBehavior.Continue;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // select previous/next item on Ctrl + Up/Down
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && (e.Key == Key.Down || e.Key == Key.Up))
            {
                var listView = (ListView)sender;

                // find the currently focused element
                string focusedElementName = null;
                var focusedElement = Keyboard.FocusedElement as FrameworkElement;
                if (focusedElement != null)
                {
                    focusedElementName = focusedElement.Name;

                    // move focus away so that bindings with UpdateSourceTrigger are updated
                    focusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }

                // select the next item
                listView.SelectedIndex = (listView.SelectedIndex + listView.Items.Count + (e.Key == Key.Down ? 1 : -1)) % listView.Items.Count;
                
                // wait for the dispatcher to instantiate the item template for the new element
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    // find the ListViewItem
                    var container = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem) as ListViewItem;
                    if (container != null)
                    {
                        bool elementHasFocus = false;

                        // if an element had the focus in the previously selected item, try to find the element with the same name in the current item
                        if (focusedElementName != null)
                        {
                            var element = this.FindElement(container, focusedElementName);
                            if (element != null && element.Focusable && element.Focus())
                            {
                                elementHasFocus = true;
                            }
                        }

                        // if no element was found, set the focus to the item
                        if (!elementHasFocus)
                        {
                            container.Focus();
                        }
                    }
                }), DispatcherPriority.ApplicationIdle);
            }
        }

        private IInputElement FindElement(DependencyObject container, string name)
        {
            // walk down the visual tree and look for an element with the given name
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(container); i++)
            {
                var child = VisualTreeHelper.GetChild(container, i);
                var element = child as FrameworkElement;
                if (element != null && element.Name == name)
                {
                    return element;
                }

                var result = FindElement(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
