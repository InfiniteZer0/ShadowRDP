using System;
using System.Collections.Generic;
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

namespace ShadowRDP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string serverName = ServerNameTextBox.Text;

            try
            {
                string sessionId = GetActiveSessionId(serverName);
                if (!string.IsNullOrEmpty(sessionId))
                {
                    string arg = $"/shadow:{sessionId} /v:{serverName} /control";

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "mstsc.exe",
                        Arguments = arg,
                        UseShellExecute = false,
                        RedirectStandardOutput = true, 
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(startInfo))
                    {
                        process.WaitForExit();
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();

                        if(!string.IsNullOrEmpty(error))
                        {
                            MessageBox.Show($"Error: {error}");
                        }

                    }
                }
                else
                {
                    MessageBox.Show("No active session found.");
                }
            } catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private string GetActiveSessionId(string serverName)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c qwinsta /server:{serverName}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();

                    string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.ToLower().Contains("active") || line.ToLower().Contains("активно"))
                        {
                            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 2)
                            {
              
                                return parts[2]; 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching session ID: {ex.Message}");
            }

            return null;
        }






    }
}
