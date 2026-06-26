using POE;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace POE
{
    public partial class MainWindow : Window
    {
        private ChatBot CyberForce;
        private CyberQuiz _quizEngine;   // dedicated quiz engine for the Quiz panel

        public MainWindow()
        {
            InitializeComponent();
            CyberForce = new ChatBot();
            _quizEngine = new CyberQuiz();
        }

        //  Window loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try { new voice_greeting().greet(); } catch { }
        }

        // Home → Username
        private void proceed(object sender, RoutedEventArgs e)
        {
            home_grid.Visibility = Visibility.Hidden;
            username_grid.Visibility = Visibility.Visible;
        }

        // Username submit 
        private void submit_name(object sender, RoutedEventArgs e)
        {
            var username = usernames_input.Text?.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a username.", "Validation",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            username_grid.Visibility = Visibility.Hidden;
            chat_grid.Visibility = Visibility.Visible;

            if (CyberForce.IsKnownUser(username))
            {
                AppendBotMessage(CyberForce.GetPersonalisedOpener());
            }
            else
            {
                CyberForce.SetUserName(username);
                AppendBotMessage($"Hello {username}! How can I assist you today?\n" +
                    "Type 'help' to see all features including tasks, quiz, and more.");
            }

            question.Focus();
        }

        //  SIDEBAR NAVIGATION
        private void ShowPanel(UIElement panel)
        {
            panel_chat.Visibility = Visibility.Hidden;
            panel_tasks.Visibility = Visibility.Hidden;
            panel_quiz.Visibility = Visibility.Hidden;
            panel_log.Visibility = Visibility.Hidden;
            panel.Visibility = Visibility.Visible;
        }

        private void Nav_Chat(object sender, RoutedEventArgs e)
        {
            ShowPanel(panel_chat);
            HighlightNav(btn_chat);
        }

        private void Nav_Tasks(object sender, RoutedEventArgs e)
        {
            ShowPanel(panel_tasks);
            HighlightNav(btn_tasks);
            LoadTaskList();
        }

        private void Nav_Quiz(object sender, RoutedEventArgs e)
        {
            ShowPanel(panel_quiz);
            HighlightNav(btn_quiz);
            if (!_quizEngine.IsActive)
                AppendQuiz("Click START QUIZ to begin the Cybersecurity Mini-Game!");
        }

        private void Nav_Log(object sender, RoutedEventArgs e)
        {
            ShowPanel(panel_log);
            HighlightNav(btn_log);
            RefreshLog_Click(null, null);
        }

        private void HighlightNav(Button active)
        {
            foreach (Button b in new[] { btn_chat, btn_tasks, btn_quiz, btn_log })
                b.Background = new SolidColorBrush(Color.FromRgb(0x1A, 0x3A, 0x5C));

            active.Background = Brushes.Cyan;
            active.Foreground = Brushes.Black;

            foreach (Button b in new[] { btn_chat, btn_tasks, btn_quiz, btn_log })
                if (b != active) b.Foreground = Brushes.White;
        }

        //  CHAT PANEL
        
        private void send(object sender, RoutedEventArgs e) => SendMessage();

        private void SendMessage()
        {
            string userInput = question.Text.Trim();
            if (string.IsNullOrWhiteSpace(userInput)) return;

            AppendUserMessage(userInput);
            string response = CyberForce.ProcessInput(userInput);
            AppendBotMessage(response);
            question.Clear();

            if (chats.Items.Count > 0)
                chats.ScrollIntoView(chats.Items[chats.Items.Count - 1]);
        }

        private void AppendUserMessage(string message)
        {
            var border = new Border
            {
                Background = Brushes.Cyan,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(80, 5, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            border.Child = new TextBlock
            {
                Text = "You: " + message,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 450
            };
            chats.Items.Add(new ListViewItem
            {
                Content = border,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            });
        }

        private void AppendBotMessage(string message)
        {
            var border = new Border
            {
                Background = Brushes.DarkSlateBlue,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 5, 80, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            border.Child = new TextBlock
            {
                Text = "CYBERFORCE: " + message,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 450
            };
            chats.Items.Add(new ListViewItem
            {
                Content = border,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            });
        }

        // Keep for compatibility
        private void SendButton_Click(object sender, RoutedEventArgs e) => SendMessage();

       
        //  TASK PANEL
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = task_title.Text.Trim();
            string desc = task_desc.Text.Trim();
            string reminder = task_reminder.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.", "Validation",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                TaskDatabase.AddTask(title, desc, reminder);
                ActivityLog.Add($"Task added: '{title}'" +
                    (string.IsNullOrWhiteSpace(reminder) ? "" : $" (Reminder: {reminder})"));

                task_title.Clear();
                task_desc.Clear();
                task_reminder.Clear();

                LoadTaskList();
                MessageBox.Show($"Task '{title}' added successfully!", "Task Added",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not add task:\n{ex.Message}",
                                "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTaskList()
        {
            task_list.Items.Clear();
            try
            {
                var tasks = TaskDatabase.GetAllTasks();
                foreach (var t in tasks)
                {
                    string status = t.IsCompleted ? "✔" : "⏳";
                    string reminder = string.IsNullOrWhiteSpace(t.Reminder) ? "" : $"  ⏰ {t.Reminder}";
                    task_list.Items.Add(new ListViewItem
                    {
                        Content = $"[{t.Id}] {status} {t.Title} — {t.Description}{reminder}",
                        Tag = t.Id,
                        Foreground = t.IsCompleted ? Brushes.LightGreen : Brushes.White,
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    });
                }

                if (tasks.Count == 0)
                    task_list.Items.Add(new ListViewItem
                    {
                        Content = "No tasks yet. Add one above!",
                        Foreground = Brushes.LightGray,
                        Background = Brushes.Transparent,
                        BorderThickness = new Thickness(0)
                    });
            }
            catch (Exception ex)
            {
                task_list.Items.Add(new ListViewItem
                {
                    Content = $" Database unavailable: {ex.Message}",
                    Foreground = Brushes.OrangeRed,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0)
                });
            }
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (task_list.SelectedItem is ListViewItem item && item.Tag is int id)
            {
                try
                {
                    TaskDatabase.CompleteTask(id);
                    ActivityLog.Add($"Task #{id} marked as complete");
                    LoadTaskList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a task first.", "No Selection",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (task_list.SelectedItem is ListViewItem item && item.Tag is int id)
            {
                var confirm = MessageBox.Show($"Delete task #{id}?", "Confirm Delete",
                                              MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        TaskDatabase.DeleteTask(id);
                        ActivityLog.Add($"Task #{id} deleted");
                        LoadTaskList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a task first.", "No Selection",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshTasks_Click(object sender, RoutedEventArgs e) => LoadTaskList();


        //  QUIZ PANEL
        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            quiz_log.Items.Clear();
            _quizEngine = new CyberQuiz();
            ActivityLog.Add("Quiz started");
            AppendQuiz(_quizEngine.Start());
        }

        private void QuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            string answer = quiz_answer.Text.Trim();
            if (string.IsNullOrWhiteSpace(answer))
            {
                MessageBox.Show("Please enter your answer (A/B/C/D or T/F).",
                                "No Answer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppendQuiz($"You: {answer}");
            string result = _quizEngine.SubmitAnswer(answer);
            AppendQuiz(result);
            quiz_answer.Clear();

            if (!_quizEngine.IsActive)
                ActivityLog.Add($"Quiz completed — {_quizEngine.CorrectCount}/{_quizEngine.TotalQuestions} correct");
        }

        private void AppendQuiz(string message)
        {
            quiz_log.Items.Add(new ListViewItem
            {
                Content = message,
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(4)
            });

            if (quiz_log.Items.Count > 0)
                quiz_log.ScrollIntoView(quiz_log.Items[quiz_log.Items.Count - 1]);
        }

        //  ACTIVITY LOG PANEL
        private void RefreshLog_Click(object sender, RoutedEventArgs e)
        {
            log_list.Items.Clear();
            string summary = ActivityLog.GetSummary();
            foreach (var line in summary.Split(new[] { '\n', '\r' },
                                               StringSplitOptions.RemoveEmptyEntries))
            {
                log_list.Items.Add(new ListViewItem
                {
                    Content = line,
                    Foreground = Brushes.White,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0)
                });
            }
        }
    }
}