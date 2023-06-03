using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void SimulateButtonClick(object o, KeyEventArgs r)
        {
            try
            {
                if (r.Key == Key.Enter)
                {
                    Login.Focus(); // Set focus to the button
                    RoutedEventArgs eventArgs = new RoutedEventArgs(Button.ClickEvent);
                    Login.RaiseEvent(eventArgs);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
