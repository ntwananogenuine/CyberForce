using System;
using System.ComponentModel;
using System.Media;

namespace cyberforce_poe
{//start of namespace
    public class Program
    {//start of class
        public static void Main(string[] args)
        {//start of method

            // create voice greeting class
           
            audioplayer.PlayGreeting();

            // create ascii logo class
            new DisplayLogo();

            // create classes to display welcome screen and ask user for name
            welcome_and_username welcome_and_collect = new welcome_and_username();
            welcome_and_collect.welcome();
            welcome_and_collect.ask_user();

            //get the username
            string collected_name = welcome_and_collect.GetUsername();

            // running the cybersecurity responses
            chats chats_response = new chats();
            chats_response.ResponseSystem(collected_name);

            // prompt and search class to obtain info on the cybersecurity topics
            prompt_and_search search = new prompt_and_search();
            search.aibot(collected_name);

        }//end of method
    }//end of class
}//end of namespace