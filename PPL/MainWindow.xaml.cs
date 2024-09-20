using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;

namespace PPL
{
    public partial class MainWindow : Window
    {
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

                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "settings.json");

                        File.WriteAllText(filePath, jsonObject.ToString());

                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message} \n \n {ex.InnerException}");
                    }
                }
            });
        }
    }
}
