using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace POE
    {
        public static class ActivityLog
        {
            private static readonly Queue<string> _log = new Queue<string>();
            private const int MaxEntries = 10;

            public static void Add(string description)
            {
                if (string.IsNullOrWhiteSpace(description))
                    return;

                string stamp = DateTime.Now.ToString("dd MMM HH:mm");
                string entry = $"[{stamp}] {description}";

                _log.Enqueue(entry);

                while (_log.Count > MaxEntries)
                    _log.Dequeue();
            }

            public static string GetSummary()
            {
                if (_log.Count == 0)
                    return "No actions have been recorded yet.";

                var sb = new StringBuilder();
                sb.AppendLine("Here's a summary of recent actions:");

                int n = 1;
                foreach (var entry in _log)
                    sb.AppendLine($"{n++}. {entry}");

                return sb.ToString().TrimEnd();
            }
        }
    }

