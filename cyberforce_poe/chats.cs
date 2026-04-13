using System;
using System.Globalization;

namespace cyberforce_poe
{//start of namespace
    public class chats
    {//start of class

        //arrays to store questions and answers
        string[] Questions =
        {
            "how are you",
            "what is your purpose",
            "what can i ask you about",
            "password",
            "phishing",
            "safe browsing",
            "browsing",

        };

        string[] Answers =
        {
            "I'm doing great, thank you for asking! Ready to help you stay safe online.",
            "My purpose is to help you learn about cybersecurity and stay safe in the digital world.",
            "You can ask me about password safety, phishing attacks, safe browsing, and more!",
            "A strong password should be at least 8 characters long and include uppercase letters, numbers, and symbols. Never reuse passwords!",
            "Phishing is when attackers trick you into giving personal info via fake emails or websites. Always verify the sender before clicking links!",
            "For safe browsing: use HTTPS websites, avoid suspicious links, keep your browser updated, and use a trusted antivirus.",
            "For safe browsing: use HTTPS websites, avoid suspicious links, keep your browser updated, and use a trusted antivirus.",

        };

        //method called ResponseSystem
        public void ResponseSystem(string username)
        {//start of method

            //section header
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╔══════════════════════════════════════╗");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ║      CyberForce Chat                 ║");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╚══════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            welcome_and_username.TypeWrite("  Cyberforce : Type your question, " + username + ". Type 'exit' to quit.", ConsoleColor.Blue);
            Console.WriteLine();

            //loop that keeps running until user types exit
            while (true)
            {
                //divider before each prompt
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" ─────────────────────────────────────");
                Console.ResetColor();

                //prompt the user
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("  " + username + " : ");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                string userInput = Console.ReadLine();
                Console.ResetColor();

                //input validation - check if empty
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    welcome_and_username.TypeWrite("  Cyberforce : It looks like you didn't type anything. Could you rephrase?", ConsoleColor.Red);
                    continue;
                }

                //check if exit
                if (userInput.ToLower() == "exit")
                {
                    Console.WriteLine();
                    welcome_and_username.TypeWrite("  Cyberforce : Goodbye, " + username + "! Stay safe online. 🔒", ConsoleColor.Yellow);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("");
                    Console.ResetColor();
                    System.Environment.Exit(0);
                    break;
                }

                //search for a matching question
                bool foundAnswer = false;

                for (int i = 0; i < Questions.Length; i++)
                {
                    if (userInput.ToLower().Contains(Questions[i]))
                    {
                        welcome_and_username.TypeWrite("  Cyberforce : " + Answers[i], ConsoleColor.Green);
                        foundAnswer = true;
                        break;
                    }
                }

                //input validation - no match found
                if (!foundAnswer)
                {
                    welcome_and_username.TypeWrite("  Cyberforce : I didn't quite understand that. Could you rephrase?", ConsoleColor.Red);
                    welcome_and_username.TypeWrite("  Cyberforce : Try asking about: passwords, phishing, or safe browsing.", ConsoleColor.Yellow);
                }

                Console.WriteLine();
            }
        }//end of method

    }//end of class
}//end of namespace