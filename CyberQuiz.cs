using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace POE
    {
        public class QuizQuestion
        {
            public string Question { get; set; }
            public string[] Options { get; set; }
            public int CorrectIndex { get; set; }
            public string Explanation { get; set; }
            public bool IsTrueFalse { get; set; }
        }

        public class CyberQuiz
        {
            private List<QuizQuestion> _questions;
            private int _currentIndex = 0;
            private int _correctCount = 0;
            private bool _active = false;
            private bool _awaitingAnswer = false;

            public bool IsActive => _active;
            public bool AwaitingAnswer => _awaitingAnswer;
            public int TotalQuestions => _questions.Count;
            public int CorrectCount => _correctCount;

            public CyberQuiz()
            {
                _questions = BuildQuestions();
            }

            public string Start()
            {
                _currentIndex = 0;
                _correctCount = 0;
                _active = true;
                _awaitingAnswer = true;
                return "🎮 Welcome to the Cybersecurity Mini-Game!\n\n" +
                       $"You will answer {TotalQuestions} questions. " +
                       "Type the letter of your answer (A/B/C/D) or T/F for True-False questions.\n\n" +
                       PresentQuestion();
            }

            public string PresentQuestion()
            {
                if (_currentIndex >= _questions.Count)
                    return EndGame();

                var q = _questions[_currentIndex];
                string text = $"Q{_currentIndex + 1}/{TotalQuestions}:\n{q.Question}\n";

                if (q.IsTrueFalse)
                {
                    text += "A) True\nB) False";
                }
                else
                {
                    for (int i = 0; i < q.Options.Length; i++)
                        text += $"{(char)('A' + i)}) {q.Options[i]}\n";
                }

                return text.TrimEnd();
            }

            public string SubmitAnswer(string input)
            {
                if (!_awaitingAnswer)
                    return "Type 'quiz' to start the quiz.";

                var q = _questions[_currentIndex];
                var clean = input.Trim().ToUpperInvariant();

                int given = -1;
                if (clean == "A" || clean == "TRUE" || clean == "T") given = 0;
                else if (clean == "B" || clean == "FALSE" || clean == "F") given = 1;
                else if (clean == "C") given = 2;
                else if (clean == "D") given = 3;

                if (given < 0)
                    return "Please answer with A, B, C or D (or T / F for True-False questions).";

                bool correct = (given == q.CorrectIndex);
                if (correct) _correctCount++;

                string feedback = correct
                    ? $"✅ Correct! {q.Explanation}"
                    : $"❌ Incorrect. The correct answer was " +
                      $"{(char)('A' + q.CorrectIndex)}. {q.Explanation}";

                _currentIndex++;

                if (_currentIndex >= _questions.Count)
                {
                    _awaitingAnswer = false;
                    return feedback + "\n\n" + EndGame();
                }

                return feedback + "\n\n" + PresentQuestion();
            }

            private string EndGame()
            {
                _active = false;
                _awaitingAnswer = false;

                double pct = (double)_correctCount / TotalQuestions * 100;
                string grade;
                if (pct >= 80) grade = "Great job! You're a cybersecurity pro!";
                else if (pct >= 50) grade = "Good effort! Keep learning to stay safe online.";
                else grade = "Keep learning to stay safe online!";

                return $"Quiz Complete!\nScore: {_correctCount}/{TotalQuestions} ({pct:0}%)\n{grade}";
            }

            private List<QuizQuestion> BuildQuestions()
            {
                return new List<QuizQuestion>
            {
                new QuizQuestion {
                    Question     = "What should you do if you receive an email asking for your password?",
                    Options      = new[] { "Reply with your password", "Delete the email",
                                           "Report the email as phishing", "Ignore it" },
                    CorrectIndex = 2,
                    Explanation  = "Reporting phishing emails helps prevent scams and protects others."
                },
                new QuizQuestion {
                    Question     = "Which of the following is the strongest password?",
                    Options      = new[] { "password123", "John1990", "P@ssw0rd!", "Tr!9kY#mX2v$" },
                    CorrectIndex = 3,
                    Explanation  = "Long passwords mixing letters, numbers and symbols are hardest to crack."
                },
                new QuizQuestion {
                    Question     = "What does 'two-factor authentication' (2FA) do?",
                    Options      = new[] { "Doubles your internet speed",
                                           "Requires two forms of identity verification",
                                           "Creates two copies of your password",
                                           "Logs you in twice" },
                    CorrectIndex = 1,
                    Explanation  = "2FA adds a second verification step, greatly improving account security."
                },
                new QuizQuestion {
                    Question     = "A VPN (Virtual Private Network) protects your privacy by:",
                    Options      = new[] { "Speeding up your connection",
                                           "Encrypting your internet traffic",
                                           "Blocking all ads",
                                           "Removing viruses" },
                    CorrectIndex = 1,
                    Explanation  = "VPNs encrypt traffic so attackers cannot read your data on public networks."
                },
                new QuizQuestion {
                    Question     = "Which of these is an example of social engineering?",
                    Options      = new[] { "Using a firewall",
                                           "Tricking someone into revealing their password",
                                           "Encrypting a hard drive",
                                           "Installing antivirus software" },
                    CorrectIndex = 1,
                    Explanation  = "Social engineering manipulates people rather than systems to steal data."
                },
                new QuizQuestion {
                    Question     = "Phishing attacks most commonly arrive through which channel?",
                    Options      = new[] { "SMS only", "Phone calls only",
                                           "Email", "Social media only" },
                    CorrectIndex = 2,
                    Explanation  = "Email is the primary vector for phishing — always verify the sender."
                },
                new QuizQuestion {
                    Question     = "What type of malware encrypts your files and demands payment?",
                    Options      = new[] { "Spyware", "Adware", "Ransomware", "Rootkit" },
                    CorrectIndex = 2,
                    Explanation  = "Ransomware locks or encrypts your files until a ransom is paid."
                },
                new QuizQuestion {
                    Question     = "It is safe to use the same password for all your accounts.",
                    IsTrueFalse  = true,
                    CorrectIndex = 1,
                    Explanation  = "Reusing passwords means one breach exposes all your accounts."
                },
                new QuizQuestion {
                    Question     = "Public Wi-Fi networks are always secure to use for online banking.",
                    IsTrueFalse  = true,
                    CorrectIndex = 1,
                    Explanation  = "Public Wi-Fi is often unencrypted — attackers can intercept your data."
                },
                new QuizQuestion {
                    Question     = "A firewall can help block unauthorised access to your computer.",
                    IsTrueFalse  = true,
                    CorrectIndex = 0,
                    Explanation  = "Firewalls monitor and filter incoming and outgoing network traffic."
                },
                new QuizQuestion {
                    Question     = "Which action best protects you from malware?",
                    Options      = new[] { "Downloading software from any website",
                                           "Keeping antivirus software up to date",
                                           "Disabling your firewall",
                                           "Sharing your login details" },
                    CorrectIndex = 1,
                    Explanation  = "Updated antivirus software can detect and remove the latest malware threats."
                },
                new QuizQuestion {
                    Question     = "HTTPS in a website URL means the connection is encrypted.",
                    IsTrueFalse  = true,
                    CorrectIndex = 0,
                    Explanation  = "HTTPS uses TLS/SSL encryption to secure data in transit between your browser and the server."
                },
            };
            }
        }
    }

