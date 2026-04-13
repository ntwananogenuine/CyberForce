using System;
using System.Collections;

namespace cyberforce_poe
{//start of namespace
    public class prompt_and_search
    {//start of class

        //ArrayLists to store answers and words to ignore
        ArrayList answers = new ArrayList();
        ArrayList ignore = new ArrayList();

        //method
        public void aibot(string username)
        {//start of method

            //add answers
            answers.Add("A strong password is at least 8 characters long, with uppercase, numbers, and symbols.");
            answers.Add("I don't have specific info on that yet, but stay cautious online!");
            answers.Add("Hey! I'm doing great, thanks for asking.");

            //add ignore words
            ignore.Add("are");
            ignore.Add("you");
            ignore.Add("hey");
            ignore.Add("me");
            ignore.Add("about");
            ignore.Add("tell");

            //section header
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  |======================================|");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  |        ADVANCED SEARCH MODE          |");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  |======================================|");
            Console.ResetColor();
            Console.WriteLine();

            welcome_and_username.TypeWrite("  CyberForce : Ask me anything, " + username + ". Type 'exit' to quit.", ConsoleColor.Blue);
            Console.WriteLine();

            string ask = string.Empty;

            do
            {
                //divider
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ──────────────────────────────────────");
                Console.ResetColor();

                //prompt the user
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("  " + username + " : ");
                Console.ForegroundColor = ConsoleColor.White;
                ask = Console.ReadLine();
                Console.ResetColor();

                //input validation - empty check
                if (string.IsNullOrWhiteSpace(ask))
                {
                    welcome_and_username.TypeWrite("  CyberForce: You didn't type anything. Could you rephrase?", ConsoleColor.Red);
                    ask = " "; // prevent exit condition triggering
                    continue;
                }

            } while (process(ask));

        }//end of method

        //method to process the input
        private Boolean process(string ask)
        {//start of method
            if (ask.ToLower() == "exit")
            {
                Console.WriteLine();
                welcome_and_username.TypeWrite("  CyberForce : Goodbye! Stay safe online.", ConsoleColor.Yellow);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ══════════════════════════════════════");
                Console.ResetColor();
                System.Environment.Exit(0);
                return false;
            }
            else
            {
                //split the input word by word
                string[] words = ask.ToLower().Split(' ');

                bool matched = false;

                //search through answers using keywords
                foreach (string word in words)
                {
                    if (ignore.Contains(word)) continue;

                    foreach (string answer in answers)
                    {
                        if (answer.ToLower().Contains(word))
                        {
                            welcome_and_username.TypeWrite("  CyberForce : " + answer, ConsoleColor.Green);
                            matched = true;
                            break;
                        }
                    }
                    if (matched) break;
                }

                //input validation - no match
                if (!matched)
                {
                    welcome_and_username.TypeWrite("  CyberForce : I didn't quite understand that. Could you rephrase?", ConsoleColor.Red);
                    welcome_and_username.TypeWrite("  CyberForce : Try asking about: passwords, phishing, or safe browsing.", ConsoleColor.Yellow);
                }

                Console.WriteLine();
                return true;
            }
        }//end of method

    }//end of class
}//end of namespace