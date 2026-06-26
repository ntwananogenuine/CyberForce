using System;
using System.Collections;
using System.Collections.Generic;

namespace list_view_chats
{
    // Local respond implementation to populate reply/ignore lists used by MainWindow
    public class respond
    {
        private Dictionary<string, ArrayList> responses;
        private Random _random = new Random();

        public respond(ArrayList reply, ArrayList ignore)
        {
            responses = new Dictionary<string, ArrayList>()
            {
                { "password", new ArrayList { "Use strong passwords with symbols and numbers.", "Never reuse passwords across websites.", "Enable two-factor authentication for extra security." } },
                { "phishing", new ArrayList { "Do not click suspicious email links.", "Always verify the sender before opening attachments.", "Phishing attacks often pretend to be trusted companies." } },
                { "privacy", new ArrayList { "Review your social media privacy settings regularly.", "Avoid sharing personal information publicly.", "Use secure websites that start with HTTPS." } },
                { "malware", new ArrayList { "Install antivirus software to protect your device.", "Avoid downloading files from unknown websites.", "Keep your operating system updated." } },
                { "scam", new ArrayList { "Online scams often create urgency to trick victims.", "Never send money to unknown people online.", "Always verify suspicious messages independently." } }
            };

            // populate the reply list with all response strings
            foreach (var kv in responses)
            {
                foreach (var s in kv.Value)
                {
                    reply.Add(s.ToString());
                }
            }

            // populate a basic ignore list if none provided
            var defaultIgnore = new[] { "the", "is", "a", "an", "and", "to", "of", "in", "on", "what", "how", "why", "tell", "me", "more", "explain", "please" };
            foreach (var w in defaultIgnore)
            {
                if (!ignore.Contains(w))
                    ignore.Add(w);
            }
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var lower = input.ToLowerInvariant();

            foreach (var item in responses)
            {
                if (lower.Contains(item.Key))
                {
                    int index = _random.Next(item.Value.Count);
                    return item.Value[index].ToString();
                }
            }

            return null;
        }

        public ArrayList GetAllKeywords()
        {
            var keywords = new ArrayList();
            foreach (var item in responses)
                keywords.Add(item.Key);
            return keywords;
        }
    }
}
