using POE2;
using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace POE2
{
    public partial class MainWindow : Window
    {
        private ChatBot CyberForce;
        

        public MainWindow()
        {
            InitializeComponent();

            CyberForce = new ChatBot();
        }

        // Called when the window has finished loading; start the greeting sound here
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vg = new voice_greeting();
                vg.greet();
            }
            catch { }
        }

      

        // Keep existing named handlers (if any) but route to the unified SendMessage
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        // Handler wired from XAML: START AI button
        private void proceed(object sender, RoutedEventArgs e)
        {
            home_grid.Visibility = Visibility.Hidden;
            username_grid.Visibility = Visibility.Visible;
        }

        // Handler wired from XAML: SUBMIT username button
        private void submit_name(object sender, RoutedEventArgs e)
        {
            var username = usernames_input.Text?.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a username.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Use ChatBot public API to store or validate username
            if (CyberForce.IsKnownUser(username))
            {
                // Known returning user
                username_grid.Visibility = Visibility.Hidden;
                chat_grid.Visibility = Visibility.Visible;
                // Use the bot's personalised opener to avoid duplicate phrasing
                AppendBotMessage(CyberForce.GetPersonalisedOpener());
                question.Focus();
                return;
            }

            // New user: set name in bot memory
            CyberForce.SetUserName(username);

            username_grid.Visibility = Visibility.Hidden;
            chat_grid.Visibility = Visibility.Visible;

            // Greet the new user
            AppendBotMessage($"Hello {username}! How are you doing today and how can I assist you?");
            question.Focus();
        }

        // Note: submission requires clicking the login button in the UI

        // Handler wired from XAML: SEND button
        private void send(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            // Use the controls declared in XAML: 'question' and 'chats'
            string userInput = question.Text.Trim();

            if (string.IsNullOrWhiteSpace(userInput))
                return;

            AppendUserMessage(userInput);

            string response = CyberForce.ProcessInput(userInput);

            AppendBotMessage(response);

            question.Clear();

            if (chats.Items.Count > 0)
                chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
        }

        private void AppendUserMessage(string message)
        {
            Border border = new Border
            {
                Background = Brushes.Cyan,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(80, 5, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            TextBlock text = new TextBlock
            {
                Text = "You: " + message,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 450
            };

            border.Child = text;

            var item = new ListViewItem { Content = border, Background = Brushes.Transparent, BorderThickness = new Thickness(0) };
            chats.Items.Add(item);
        }

        private void AppendBotMessage(string message)
        {
            Border border = new Border
            {
                Background = Brushes.DarkSlateBlue,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 5, 80, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            TextBlock text = new TextBlock
            {
                Text = "CYBERFORCE: " + message,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 450
            };

            border.Child = text;

            var item = new ListViewItem { Content = border, Background = Brushes.Transparent, BorderThickness = new Thickness(0) };
            chats.Items.Add(item);
        }
    }
}