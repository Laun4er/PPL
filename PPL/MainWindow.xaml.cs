﻿using Newtonsoft.Json.Linq;
using PPL.Properties;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        }

        private void CreateStoryboards()
        {
            // Show Panel Storyboard
            showPanelStoryboard = new Storyboard();

            var showWidthAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(showWidthAnimation, "NavigationPanel");
            Storyboard.SetTargetProperty(showWidthAnimation, new PropertyPath(Border.WidthProperty));
            showWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(200, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new CubicEase { EasingMode = EasingMode.EaseOut }));

            showPanelStoryboard.Children.Add(showWidthAnimation);

            // Hide Panel Storyboard
            hidePanelStoryboard = new Storyboard();

            var hideWidthAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(hideWidthAnimation, "NavigationPanel");
            Storyboard.SetTargetProperty(hideWidthAnimation, new PropertyPath(Border.WidthProperty));
            hideWidthAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(10, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new CubicEase { EasingMode = EasingMode.EaseOut }));

            hidePanelStoryboard.Children.Add(hideWidthAnimation);

            // Show Blur Storyboard
            showBlurStoryboard = new Storyboard();

            var showBlurAnimation = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(showBlurAnimation, "BlurFrame");
            Storyboard.SetTargetProperty(showBlurAnimation, new PropertyPath(BlurEffect.RadiusProperty));
            showBlurAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1)), new CubicEase { EasingMode = EasingMode.EaseOut }));

            showBlurStoryboard.Children.Add(showBlurAnimation);

            // Hide Blur Storyboard
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
        }

        private void PinPanel_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.PinnedPanel = false;
            Properties.Settings.Default.Save();
            hidePanelStoryboard.Begin(this);
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
    }
}
