using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EditModeSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var container = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem) as ListViewItem;
                if (container != null)
                {
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
            //if (e.OriginalSource is ComboBox)
            //{
            //    return;
            //}

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && (e.Key == Key.Down || e.Key == Key.Up))
            {
                var listView = (ListView)sender;

                string focusedElementName = null;
                var focusedElement = Keyboard.FocusedElement as FrameworkElement;
                if (focusedElement != null)
                {
                    focusedElementName = focusedElement.Name;
                    focusedElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }

                listView.SelectedIndex = (listView.SelectedIndex + listView.Items.Count + (e.Key == Key.Down ? 1 : -1)) % listView.Items.Count;
                
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var container = listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem) as ListViewItem;
                    if (container != null)
                    {
                        bool elementHasFocus = false;
                        if (focusedElementName != null)
                        {
                            var element = this.FindElement(container, focusedElementName);
                            if (element != null && element.Focusable && element.Focus())
                            {
                                elementHasFocus = true;
                            }
                        }

                        if (!elementHasFocus)
                        {
                            container.Focus();
                        }

                        listView.UpdateLayout();
                        listView.ScrollIntoView(listView.SelectedItem);
                    }
                }), DispatcherPriority.ApplicationIdle);
            }
        }

        private IInputElement FindElement(DependencyObject container, string name)
        {
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
