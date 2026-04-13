using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace cyberforce_poe
{
    public class audioplayer
    {

        public static void PlayGreeting()
        {

            //call the greet method 
            greet();


        }//end of constructor

        private static void greet()
        {//start of greet method

            //auto get the path of the voice record 

            string paths = AppDomain.CurrentDomain.BaseDirectory;

            //then rename the path
            string fullpath = paths.Replace(@"bin\Debug\", "greeting.wav");

            //Load the audio , then Play the audio after instance
            SoundPlayer voice_play = new SoundPlayer(fullpath);
            // load the audio file
            voice_play.Load();
            //then play the audio
            voice_play.Play();

        }// end of greet method
    }
}