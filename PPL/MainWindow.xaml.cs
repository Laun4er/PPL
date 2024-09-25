using Newtonsoft.Json.Linq;
using PPL.pages;
using PPL.Pages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace PPL
{
    public partial class MainWindow : Window
    {
        private Storyboard showPanelStoryboard;
        private Storyboard hidePanelStoryboard;
        private Storyboard showBlurStoryboard;
        private Storyboard hideBlurStoryboard;

        private Storyboard isCheckedPin;
        private Storyboard isUncheckedPin;

        private Dictionary<string, Page> pages = new Dictionary<string, Page>();


        public MainWindow()
        {
            InitializeComponent();

            Environment.GetCommandLineArgs().ToList().ForEach(x =>
            {
                if (x.EndsWith("/Upload"))
                {
                    try
                    {
                        JObject jsonObject = new JObject();

                        foreach (SettingsProperty prop in Properties.Settings.Default.Properties)
                        {
                            jsonObject[prop.Name] = Properties.Settings.Default[prop.Name]?.ToString();
                        }

                        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

                        File.WriteAllText(filePath, jsonObject.ToString());

                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message} \n \n {ex.InnerException}");
                    }
                }
            });

            PinPanel.Checked += PinPanel_Checked;
            PinPanel.Unchecked += PinPanel_Unchecked;
            CreateStoryboards();

            if (Properties.Settings.Default.PinnedPanel == true)
            {
                PinPanel.IsChecked = true;
            }
            else
            {
                PinPanel.IsChecked = false;
            }

            pages.Add("Vanilla", new Vanilla());
            pages.Add("ModsServer", new ModsServer());
            pages.Add("Mods", new Mods());
            pages.Add("News", new News());
            pages.Add("Settings", new Settings());
            pages.Add("Profile", new Profile());
            pageFrame.Content = pages["Vanilla"];

            LB1.SelectedIndex = 0;

        }

        private void CreateStoryboards()
        {
            showPanelStoryboard = new Storyboard();
            var showWidthAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(showWidthAnimation, "NavigationPanel");
            Storyboard.SetTargetProperty(showWidthAnimation, new PropertyPath(Border.WidthProperty));
            showWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(200, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new CubicEase { EasingMode = EasingMode.EaseOut }));
            showPanelStoryboard.Children.Add(showWidthAnimation);

            hidePanelStoryboard = new Storyboard();
            var hideWidthAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(hideWidthAnimation, "NavigationPanel");
            Storyboard.SetTargetProperty(hideWidthAnimation, new PropertyPath(Border.WidthProperty));
            hideWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(10, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new CubicEase { EasingMode = EasingMode.EaseOut }));
            hidePanelStoryboard.Children.Add(hideWidthAnimation);


            showBlurStoryboard = new Storyboard();
            var showBlurAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(showBlurAnimation, "BlurFrame");
            Storyboard.SetTargetProperty(showBlurAnimation, new PropertyPath(BlurEffect.RadiusProperty));
            showBlurAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(10, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new CubicEase { EasingMode = EasingMode.EaseOut }));
            showBlurStoryboard.Children.Add(showBlurAnimation);

            hideBlurStoryboard = new Storyboard();
            var hideBlurAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(hideBlurAnimation, "BlurFrame");
            Storyboard.SetTargetProperty(hideBlurAnimation, new PropertyPath(BlurEffect.RadiusProperty));
            hideBlurAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new CubicEase { EasingMode = EasingMode.EaseOut }));
            hideBlurStoryboard.Children.Add(hideBlurAnimation);
        }

        private void PinPanel_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PinnedPanel = true;
            Properties.Settings.Default.Save();
            showPanelStoryboard.Begin(this);
            hideBlurStoryboard.Begin(this);
        }

        private void PinPanel_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PinnedPanel = false;
            Properties.Settings.Default.Save();
            showBlurStoryboard.Begin(this);
        }

        private void NavigationPanel_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (Properties.Settings.Default.PinnedPanel == false)
            {
                showPanelStoryboard.Begin(this);
                showBlurStoryboard.Begin(this);
            }
            else
            {
                showPanelStoryboard.Begin(this);

            }
        }

        private void NavigationPanel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Properties.Settings.Default.PinnedPanel == false)
            {
                hidePanelStoryboard.Begin(this);
                hideBlurStoryboard.Begin(this);
            }
        }
        private void ToolBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void ToolBar1_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                switch (this.WindowState)
                {
                    case WindowState.Maximized:
                        this.WindowState = WindowState.Normal;
                        break;
                    case WindowState.Normal:
                        this.WindowState = WindowState.Maximized;
                        break;
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    this.WindowState = WindowState.Normal;
                    break;
                case WindowState.Normal:
                    this.WindowState = WindowState.Maximized;
                    break;
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private bool isInitialSelection = true;
        private void PageChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialSelection)
            {
                isInitialSelection = false;
                return;
            }

            if (sender is ListBox listBox)
            {
                if (listBox.SelectedItem is ListBoxItem selectedListBoxItem)
                {
                    switch (listBox.Name)
                    {
                        case "ListBox2":
                            LB1.SelectedIndex = -1;
                            break;
                        case "ListBox1":
                            LB2.SelectedIndex = -1;
                            break;
                    }
                    Page pageInstance = pages[selectedListBoxItem.Name];
                    pageFrame.Content = pageInstance;
                }
            }
        }
        public static class SelectorBehavior
        {

            public static readonly DependencyProperty ShouldSelectItemOnMouseUpProperty =
                DependencyProperty.RegisterAttached(
                    "ShouldSelectItemOnMouseUp", typeof(bool), typeof(SelectorBehavior),
                    new PropertyMetadata(default(bool), HandleShouldSelectItemOnMouseUpChange));

            public static void SetShouldSelectItemOnMouseUp(DependencyObject element, bool value)
            {
                element.SetValue(ShouldSelectItemOnMouseUpProperty, value);
            }

            public static bool GetShouldSelectItemOnMouseUp(DependencyObject element)
            {
                return (bool)element.GetValue(ShouldSelectItemOnMouseUpProperty);
            }

            private static void HandleShouldSelectItemOnMouseUpChange(
                DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                if (d is Selector selector)
                {
                    selector.PreviewMouseDown -= HandleSelectPreviewMouseDown;
                    selector.MouseUp -= HandleSelectMouseUp;

                    if (Equals(e.NewValue, true))
                    {
                        selector.PreviewMouseDown += HandleSelectPreviewMouseDown;
                        selector.MouseUp += HandleSelectMouseUp;
                    }
                }
            }

            private static void HandleSelectMouseUp(object sender, MouseButtonEventArgs e)
            {
                var selector = (Selector)sender;

                if (e.ChangedButton == MouseButton.Left && e.OriginalSource is Visual source)
                {
                    var container = selector.ContainerFromElement(source);
                    if (container != null)
                    {
                        var index = selector.ItemContainerGenerator.IndexFromContainer(container);
                        if (index >= 0)
                        {
                            selector.SelectedIndex = index;
                        }
                    }
                }
            }

            private static void HandleSelectPreviewMouseDown(object sender, MouseButtonEventArgs e)
            {
                e.Handled = e.ChangedButton == MouseButton.Left && e.OriginalSource is FrameworkElement originalSource && originalSource.Parent != null;
            }
        }
    }
}
