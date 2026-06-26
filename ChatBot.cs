using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace POE
{
    // ══════════════════════════════════════════════════════════════════════════
    //  ChatBot  —  Part 3 (extends Part 1 & 2)
    //  NEW:  Task Assistant (MySQL), Mini-Game Quiz, NLP Simulation, Activity Log
    // ══════════════════════════════════════════════════════════════════════════
    public class ChatBot
    {
        // ── Part 1 / 2 dependencies ───────────────────────────────────────────
        private respond _keywords;
        private SentimentDetector _sentiment;
        private MemoryStore _memory;

        private bool _awaitingName = true;
        private string _lastTopic = "";

        // ── Part 3 — Task Assistant ───────────────────────────────────────────
        private bool _awaitingTaskTitle = false;
        private bool _awaitingTaskDescription = false;
        private bool _awaitingTaskReminder = false;
        private string _pendingTaskTitle = "";
        private string _pendingTaskDescription = "";
        private bool _dbAvailable = false;

        // ── Part 3 — Quiz ─────────────────────────────────────────────────────
        private CyberQuiz _quiz;

        // ── Constructor ───────────────────────────────────────────────────────
        public ChatBot()
        {
            _keywords = new respond();
            _sentiment = new SentimentDetector();
            _quiz = new CyberQuiz();

            // Restore persisted memory
            try
            {
                var appDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "CyberForce");
                _memory = MemoryStore.Load(appDir);
            }
            catch { _memory = new MemoryStore(); }

            if (!string.IsNullOrWhiteSpace(_memory.UserName))
                _awaitingName = false;

            // Try to initialise the database (non-fatal if MySQL is not present)
            try
            {
                TaskDatabase.Initialise();
                _dbAvailable = true;
            }
            catch
            {
                _dbAvailable = false;
            }
        }

        // ── Public helpers (kept from Part 2) ─────────────────────────────────
        public string GetGreeting() => "Hello! What is your name?";
        public string GetPersonalisedOpener() => _memory.GetPersonalisedOpener();

        public void SetUserName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            _memory.UserName = name.Trim();
            _awaitingName = false;
            PersistMemory();
            SaveUserInfo();
        }

        public bool IsKnownUser(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            return string.Equals(_memory.UserName, name, StringComparison.OrdinalIgnoreCase);
        }

        public void SaveUserInfo()
        {
            try
            {
                var line = $"{DateTime.UtcNow:o}\t{_memory?.UserName ?? ""}\t{_memory?.FavouriteTopic ?? ""}";
                var appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CyberForce");
                if (!Directory.Exists(appDir)) Directory.CreateDirectory(appDir);
                File.AppendAllText(Path.Combine(appDir, "user_names.txt"), line + Environment.NewLine);
            }
            catch { }
        }

        // ── Main input processor ──────────────────────────────────────────────
        public string ProcessInput(string input)
        {
            if (input == null) input = "";
            var lower = input.Trim().ToLowerInvariant();

            // ── Name capture (first time) ──────────────────────────────────────
            if (_awaitingName)
            {
                _memory.UserName = lower;
                _awaitingName = false;
                return $"Nice to meet you {_memory.UserName}! Ask me anything about cybersecurity.";
            }

            // ── Quiz in progress: route all input to the quiz engine ───────────
            if (_quiz.IsActive && _quiz.AwaitingAnswer)
            {
                string quizResult = _quiz.SubmitAnswer(input);
                if (!_quiz.IsActive)
                    ActivityLog.Add($"Quiz completed — {_quiz.CorrectCount}/{_quiz.TotalQuestions} correct");
                return quizResult;
            }

            // ── Multi-step task creation flow ──────────────────────────────────
            if (_awaitingTaskTitle)
            {
                _pendingTaskTitle = input.Trim();
                _awaitingTaskTitle = false;
                _awaitingTaskDescription = true;
                return "Got it! Please enter a description for this task:";
            }

            if (_awaitingTaskDescription)
            {
                _pendingTaskDescription = input.Trim();
                _awaitingTaskDescription = false;
                _awaitingTaskReminder = true;
                return "Would you like to set a reminder? Enter a date or timeframe (e.g. '3 days', '2026-07-01') or type 'none' to skip:";
            }

            if (_awaitingTaskReminder)
            {
                string reminder = lower == "none" ? "" : input.Trim();
                _awaitingTaskReminder = false;

                if (_dbAvailable)
                {
                    try
                    {
                        TaskDatabase.AddTask(_pendingTaskTitle, _pendingTaskDescription, reminder);
                        string reminderText = string.IsNullOrWhiteSpace(reminder) ? "no reminder" : $"reminder set for {reminder}";
                        ActivityLog.Add($"Task added: '{_pendingTaskTitle}' ({reminderText})");
                        return $"Task added: '{_pendingTaskTitle}'. {(string.IsNullOrWhiteSpace(reminder) ? "" : $"I'll remind you in {reminder}.")}";
                    }
                    catch (Exception ex)
                    {
                        return $"Could not save the task to the database: {ex.Message}";
                    }
                }
                else
                {
                    return "⚠ Database is not available. Please ensure MySQL is running and try again.";
                }
            }

            // ═══════════════════════════════════════════════════════════════════
            //  NLP SIMULATION — detect intent from varied phrasing (Task 3)
            // ═══════════════════════════════════════════════════════════════════

            // ── Start quiz ─────────────────────────────────────────────────────
            if (NlpMatch(lower, "quiz", "start quiz", "play quiz", "cybersecurity quiz",
                         "mini game", "mini-game", "test me", "test my knowledge",
                         "knowledge test", "question", "game"))
            {
                ActivityLog.Add("Quiz started");
                return _quiz.Start();
            }

            // ── Show activity log ──────────────────────────────────────────────
            if (NlpMatch(lower, "show activity log", "activity log", "what have you done",
                         "what have you done for me", "recent actions", "history",
                         "show log", "view log", "log"))
            {
                return ActivityLog.GetSummary();
            }

            // ── Add task ───────────────────────────────────────────────────────
            if (NlpMatch(lower, "add task", "new task", "create task", "set task",
                         "add a task", "create a task", "schedule task",
                         "remind me to", "i need to", "add to my tasks"))
            {
                if (!_dbAvailable)
                    return "⚠ Database is not available. Please ensure MySQL is running.";

                _awaitingTaskTitle = true;
                return "Sure! What is the title of your cybersecurity task?";
            }

            // ── View tasks ─────────────────────────────────────────────────────
            if (NlpMatch(lower, "view tasks", "show tasks", "my tasks", "list tasks",
                         "show my tasks", "display tasks", "what tasks", "pending tasks",
                         "all tasks"))
            {
                return GetTaskList();
            }

            // ── Complete task ──────────────────────────────────────────────────
            if (NlpMatch(lower, "complete task", "mark task done", "task done",
                         "finish task", "mark complete", "done task"))
            {
                return "To mark a task as complete, type: complete task <ID>  (e.g. 'complete task 3').\nUse 'view tasks' to see task IDs.";
            }

            // Parse "complete task <N>" exactly
            if (lower.StartsWith("complete task "))
            {
                var parts = lower.Split(' ');
                if (parts.Length >= 3 && int.TryParse(parts[2], out int tid))
                    return CompleteTask(tid);
                return "Please specify a task ID, e.g. 'complete task 3'.";
            }

            // ── Delete task ────────────────────────────────────────────────────
            if (lower.StartsWith("delete task "))
            {
                var parts = lower.Split(' ');
                if (parts.Length >= 3 && int.TryParse(parts[2], out int tid))
                    return DeleteTask(tid);
                return "Please specify a task ID, e.g. 'delete task 3'.";
            }

            // ── Help ───────────────────────────────────────────────────────────
            if (NlpMatch(lower, "help", "what can you do", "commands", "features",
                         "options", "menu"))
            {
                return "I can help with:\n" +
                       "• Cybersecurity topics — phishing, passwords, malware, VPN, privacy, etc.\n" +
                       "• Task Assistant — type 'add task', 'view tasks', 'complete task <id>', 'delete task <id>'\n" +
                       "• Quiz — type 'quiz' to test your cybersecurity knowledge\n" +
                       "• Activity Log — type 'activity log' to see recent actions\n" +
                       "• Type 'tell me more' to expand on the last topic";
            }

            // ── Tell me more ───────────────────────────────────────────────────
            if (NlpMatch(lower, "tell me more", "explain more", "more info",
                         "elaborate", "expand on that"))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                    return $"Here is more about {_lastTopic}: " +
                           "Always stay alert and keep learning about cybersecurity threats. " +
                           "Regular awareness training reduces risk significantly.";
            }

            // ── How are you ───────────────────────────────────────────────────
            if (NlpMatch(lower, "how are you", "how are you doing", "are you okay",
                         "you alright", "how do you feel"))
                return "I am functioning perfectly and ready to help you stay safe online!";

            // ── Purpose ───────────────────────────────────────────────────────
            if (NlpMatch(lower, "purpose", "what is your purpose", "why are you here",
                         "what do you do"))
                return "My purpose is to educate users about cybersecurity awareness.";

            // ═══════════════════════════════════════════════════════════════════
            //  Part 1 / 2 — Sentiment + Keyword responses
            // ═══════════════════════════════════════════════════════════════════
            Sentiment sentResult = _sentiment.Detect(lower);
            string sentResponse = _sentiment.GetSentimentResponse(sentResult);
            string kwResponse = _keywords.GetResponse(lower);

            if (kwResponse != null)
            {
                foreach (string kw in _keywords.GetAllKeywords())
                {
                    if (lower.Contains(kw))
                    {
                        _lastTopic = kw;
                        _memory.FavouriteTopic = kw;
                        break;
                    }
                }

                string personalised = string.IsNullOrEmpty(_memory.FavouriteTopic)
                    ? ""
                    : $"As someone interested in {_memory.FavouriteTopic}, ";

                return $"{sentResponse} {personalised}{kwResponse}".Trim();
            }

            if (!string.IsNullOrEmpty(sentResponse) && sentResult != Sentiment.Neutral)
                return sentResponse;

            // Fallback
            return "I didn't quite understand that. Could you rephrase?\n" +
                   "Try asking about passwords, phishing, malware, or type 'help' for all commands.";
        }

        
        //  NLP Helper — returns true if input contains ANY of the given phrases
      
        private bool NlpMatch(string lower, params string[] phrases)
        {
            foreach (var phrase in phrases)
                if (lower.Contains(phrase)) return true;
            return false;
        }

        
        //  Task helpers
        private string GetTaskList()
        {
            if (!_dbAvailable)
                return "Database is not available. Please ensure MySQL is running.";

            try
            {
                var tasks = TaskDatabase.GetAllTasks();
                if (tasks.Count == 0)
                    return "You have no cybersecurity tasks yet. Type 'add task' to create one.";

                var sb = new StringBuilder("Your cybersecurity tasks:\n");
                foreach (var t in tasks)
                    sb.AppendLine($"[{t.Id}] {t}");

                return sb.ToString().TrimEnd();
            }
            catch (Exception ex)
            {
                return $"Could not retrieve tasks: {ex.Message}";
            }
        }

        private string CompleteTask(int id)
        {
            if (!_dbAvailable)
                return "Database is not available. Please ensure MySQL is running.";

            try
            {
                bool ok = TaskDatabase.CompleteTask(id);
                if (ok)
                {
                    ActivityLog.Add($"Task #{id} marked as complete");
                    return $"Task #{id} has been marked as completed. Well done!";
                }
                return $"No task with ID {id} was found.";
            }
            catch (Exception ex) { return $"Error: {ex.Message}"; }
        }

        private string DeleteTask(int id)
        {
            if (!_dbAvailable)
                return "Database is not available. Please ensure MySQL is running.";

            try
            {
                bool ok = TaskDatabase.DeleteTask(id);
                if (ok)
                {
                    ActivityLog.Add($"Task #{id} deleted");
                    return $"Task #{id} has been deleted.";
                }
                return $"No task with ID {id} was found.";
            }
            catch (Exception ex) { return $"Error: {ex.Message}"; }
        }

        //  Memory persistence
        private void PersistMemory()
        {
            try
            {
                var appDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "CyberForce");
                _memory.Save(appDir);
            }
            catch { }
        }
    }

    
    //  MemoryStore  (unchanged from Part 2)
    
    public class MemoryStore
    {
        private Dictionary<string, string> _data = new Dictionary<string, string>();

        public string UserName { get; set; }
        public string FavouriteTopic { get; set; }

        public void Store(string key, string value) => _data[key] = value;
        public string Recall(string key) => _data.TryGetValue(key, out var v) ? v : null;
        public string GetPersonalisedOpener() => $"Welcome back {UserName}.";

        public void Save(string folder)
        {
            try
            {
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, "memory.txt"), UserName ?? "");
            }
            catch { }
        }

        public static MemoryStore Load(string folder)
        {
            try
            {
                var ms = new MemoryStore();
                var file = Path.Combine(folder, "memory.txt");
                if (File.Exists(file))
                    ms.UserName = File.ReadAllText(file).Trim();
                return ms;
            }
            catch { return new MemoryStore(); }
        }
    }
}