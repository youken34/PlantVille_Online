using System;
using System.Windows;

namespace week08
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = usernameTextBox.Text; // Retrieve the value from the TextBox

                Interface newWindow = new Interface(username);
                newWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
        }
    }
}
