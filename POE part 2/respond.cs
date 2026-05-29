using System;
using System.Collections;
using System.Collections.Generic;

namespace POE2
{
    internal class respond
    {
        private Dictionary<string, ArrayList> responses;
        private Random _random = new Random();

        // parameterless constructor builds internal responses from the answers() list
        public respond()
        {
            BuildResponsesFromAnswers();
        }

        // existing constructor kept for compatibility with earlier code
        public respond(ArrayList reply, ArrayList ignore)
        {
            // call the answers and words methods to populate the lists
            answers(reply);
            words(ignore);
            // also populate internal responses map so other code can use GetResponse
            BuildResponsesFromAnswers();
        }

    
    // helper: build the responses dictionary from the flat answers list
    private void BuildResponsesFromAnswers()
    {
        responses = new Dictionary<string, ArrayList>(StringComparer.OrdinalIgnoreCase);

        var flat = answers(new ArrayList());
        foreach (var obj in flat)
        {
            var s = obj?.ToString() ?? string.Empty;
            var idx = s.IndexOf(' ');
            if (idx <= 0)
                continue;

            var key = s.Substring(0, idx).Trim();
            var content = s.Substring(idx + 1).Trim();

            if (!responses.ContainsKey(key))
                responses[key] = new ArrayList();

            responses[key].Add(content);
        }
    }

    // Return a random response for an input containing a known keyword
    public string GetResponse(string input)
    {
        if (string.IsNullOrEmpty(input) || responses == null)
            return null;

        var lower = input.ToLowerInvariant();
        foreach (var kv in responses)
        {
            if (lower.Contains(kv.Key))
            {
                int index = _random.Next(kv.Value.Count);
                return kv.Value[index].ToString();
            }
        }

        return null;
    }

    public ArrayList GetAllKeywords()
    {
        var keys = new ArrayList();
        if (responses == null)
            BuildResponsesFromAnswers();

        foreach (var k in responses.Keys)
            keys.Add(k);

        return keys;
    }

    // ── method to store answers ───────────────────────────────────
    public ArrayList answers(ArrayList add_answers)
    {// start of answers method

        // ── greetings ─────────────────────────────────────────────
        add_answers.Add("greeting i'm doing well, thanks for asking! how are you doing today?");
        add_answers.Add("greeting i'm great today, thanks for asking! how can i help you today?");
        add_answers.Add("greeting doing good! hope you are also doing well today?");

        // ── purpose ───────────────────────────────────────────────
        add_answers.Add("purpose my purpose is to educate you on how to stay safe online and guide your cybersecurity questions.");
        add_answers.Add("purpose i help users understand online safety and digital protection.");
        add_answers.Add("purpose i assist with cybersecurity awareness and safety guidance.");

        // ── cybersecurity ─────────────────────────────────────────
        add_answers.Add("cybersecurity cybersecurity is about protecting systems and networks from digital threats.");
        add_answers.Add("cybersecurity it involves protecting devices and online accounts from attacks.");
        add_answers.Add("cybersecurity it focuses on securing digital information and systems.");

        // ── phishing ──────────────────────────────────────────────
        add_answers.Add("phishing phishing is a scam where attackers pretend to be trusted sources to steal information.");
        add_answers.Add("phishing it uses fake messages or websites to trick users into revealing sensitive data.");
        add_answers.Add("phishing attackers use deception to make users believe they are legitimate.");

        // ── firewall ──────────────────────────────────────────────
        add_answers.Add("firewall a firewall controls network traffic based on security rules.");
        add_answers.Add("firewall it helps block unwanted access to your device or network.");
        add_answers.Add("firewall it acts as a protective barrier between trusted and untrusted networks.");

        // ── password ──────────────────────────────────────────────
        add_answers.Add("password a password is used to secure access to your accounts or devices.");
        add_answers.Add("password it should be strong, long and not easy to guess.");
        add_answers.Add("password avoid using personal details when creating a password.");

        // ── hacked ────────────────────────────────────────────────
        add_answers.Add("hacked immediately secure your account and log out of all devices.");
        add_answers.Add("hacked contact support if your account has been compromised.");
        add_answers.Add("hacked enable extra security like two-factor authentication after being hacked.");

        // ── fraud ─────────────────────────────────────────────────
        add_answers.Add("fraud contact your bank immediately if fraud is detected.");
        add_answers.Add("fraud report suspicious financial activity to the authorities.");
        add_answers.Add("fraud monitor your accounts regularly for unusual activity.");

        // ── scam ──────────────────────────────────────────────────
        add_answers.Add("scam be cautious of unsolicited messages asking for personal information.");
        add_answers.Add("scam if something sounds too good to be true online, it probably is.");
        add_answers.Add("scam scammers often impersonate trusted organisations to steal your data.");

        // ── privacy ───────────────────────────────────────────────
        add_answers.Add("privacy review your privacy settings on social media accounts regularly.");
        add_answers.Add("privacy avoid sharing personal information like your ID number in public spaces.");
        add_answers.Add("privacy use a vpn when browsing on public wi-fi to protect your privacy.");

        // ── malware ───────────────────────────────────────────────
        add_answers.Add("malware keep your antivirus software up to date to protect against malware threats.");
        add_answers.Add("malware never download software from untrusted sources as malware is often disguised.");
        add_answers.Add("malware regularly scan your device for malware especially after visiting unfamiliar websites.");

        // ── vpn ───────────────────────────────────────────────────
        add_answers.Add("vpn a vpn helps protect your privacy on public wi-fi.");
        add_answers.Add("vpn it encrypts your internet traffic for added safety.");
        add_answers.Add("vpn it improves security when using public or unsecured networks.");

        // ── wifi ──────────────────────────────────────────────────
        add_answers.Add("wifi avoid using public wi-fi for sensitive activities like banking.");
        add_answers.Add("wifi make sure your home wi-fi uses wpa2 or wpa3 encryption.");
        add_answers.Add("wifi never connect to unknown open wi-fi networks as attackers can intercept your data.");

        // ── 2fa ───────────────────────────────────────────────────
        add_answers.Add("2fa enable two-factor authentication on all important accounts for extra security.");
        add_answers.Add("2fa two-factor authentication makes it much harder for attackers to access your accounts.");
        add_answers.Add("2fa use an authenticator app instead of sms for stronger 2fa protection.");

        // ── malicious chatbot ─────────────────────────────────────
        add_answers.Add("malicious malicious bots often create urgency to trick users.");
        add_answers.Add("malicious fake chatbots may ask for sensitive information.");
        add_answers.Add("malicious be cautious if a bot pressures you for personal data.");

        // ── sentiment: frustrated ─────────────────────────────────
        add_answers.Add("frustrated i understand you're frustrated. let's work through the issue step by step.");
        add_answers.Add("frustrated it's okay to feel frustrated when things aren't working. i'm here to help.");
        add_answers.Add("frustrated take a breath, we'll fix this together.");

        // ── sentiment: confused ───────────────────────────────────
        add_answers.Add("confused that's okay, confusion is normal. i'll explain it clearly for you.");
        add_answers.Add("confused let me break it down step by step so it makes sense.");
        add_answers.Add("confused no worries, i'll help you understand it better.");

        // ── sentiment: worried ────────────────────────────────────
        add_answers.Add("worried it's okay to feel worried. i'm here to help you stay safe online.");
        add_answers.Add("worried don't panic, most cybersecurity issues can be fixed quickly.");
        add_answers.Add("worried i understand your concern. let's make sure your information is safe.");

        // ── sentiment: happy ──────────────────────────────────────
        add_answers.Add("happy that's great to hear! i'm glad things are going well.");
        add_answers.Add("happy awesome! positivity is always good.");
        add_answers.Add("happy i'm happy for you! let me know if you need anything.");

        // ── sentiment: sad ────────────────────────────────────────
        add_answers.Add("sad i'm sorry you're feeling this way. i'm here for you.");
        add_answers.Add("sad that sounds tough, take things one step at a time.");
        add_answers.Add("sad i hope things improve soon. you can talk to me anytime.");

        // ── sentiment: angry ──────────────────────────────────────
        add_answers.Add("angry i understand you're angry. let's try to solve the issue together.");
        add_answers.Add("angry it's okay to feel angry, but i'll help you fix the problem.");
        add_answers.Add("angry take your time, i'm here to help you sort it out.");

        // ── sentiment: curious ────────────────────────────────────
        add_answers.Add("curious great curiosity! staying informed is your best defence online.");
        add_answers.Add("curious i love the enthusiasm! let me share what i know.");
        add_answers.Add("curious asking questions is the first step to staying safe online.");

        return add_answers;

    }// end of answers method

    // ── method to store ignore words ─────────────────────────────
    private void words(ArrayList ignoring)
    {// start of words method

        ignoring.Add("a"); ignoring.Add("about"); ignoring.Add("above");
        ignoring.Add("across"); ignoring.Add("after"); ignoring.Add("afterwards");
        ignoring.Add("again"); ignoring.Add("against"); ignoring.Add("all");
        ignoring.Add("almost"); ignoring.Add("alone"); ignoring.Add("along");
        ignoring.Add("already"); ignoring.Add("also"); ignoring.Add("although");
        ignoring.Add("always"); ignoring.Add("am"); ignoring.Add("among");
        ignoring.Add("amongst"); ignoring.Add("amount"); ignoring.Add("an");
        ignoring.Add("and"); ignoring.Add("another"); ignoring.Add("any");
        ignoring.Add("anyhow"); ignoring.Add("anyone"); ignoring.Add("anything");
        ignoring.Add("anyway"); ignoring.Add("anywhere"); ignoring.Add("are");
        ignoring.Add("around"); ignoring.Add("as"); ignoring.Add("at");
        ignoring.Add("back"); ignoring.Add("be"); ignoring.Add("became");
        ignoring.Add("because"); ignoring.Add("become"); ignoring.Add("becomes");
        ignoring.Add("becoming"); ignoring.Add("been"); ignoring.Add("before");
        ignoring.Add("beforehand"); ignoring.Add("behind"); ignoring.Add("being");
        ignoring.Add("below"); ignoring.Add("beside"); ignoring.Add("besides");
        ignoring.Add("between"); ignoring.Add("beyond"); ignoring.Add("both");
        ignoring.Add("but"); ignoring.Add("by"); ignoring.Add("can");
        ignoring.Add("cannot"); ignoring.Add("could"); ignoring.Add("did");
        ignoring.Add("do"); ignoring.Add("does"); ignoring.Add("doing");
        ignoring.Add("done"); ignoring.Add("down"); ignoring.Add("during");
        ignoring.Add("each"); ignoring.Add("either"); ignoring.Add("else");
        ignoring.Add("elsewhere"); ignoring.Add("enough"); ignoring.Add("etc");
        ignoring.Add("even"); ignoring.Add("ever"); ignoring.Add("every");
        ignoring.Add("everyone"); ignoring.Add("everything"); ignoring.Add("everywhere");
        ignoring.Add("except"); ignoring.Add("few"); ignoring.Add("first");
        ignoring.Add("for"); ignoring.Add("former"); ignoring.Add("formerly");
        ignoring.Add("from"); ignoring.Add("further"); ignoring.Add("had");
        ignoring.Add("has"); ignoring.Add("have"); ignoring.Add("having");
        ignoring.Add("he"); ignoring.Add("hence"); ignoring.Add("her");
        ignoring.Add("here"); ignoring.Add("hereafter"); ignoring.Add("hereby");
        ignoring.Add("herein"); ignoring.Add("hereupon"); ignoring.Add("hers");
        ignoring.Add("herself"); ignoring.Add("him"); ignoring.Add("himself");
        ignoring.Add("his"); ignoring.Add("how"); ignoring.Add("however");
        ignoring.Add("i"); ignoring.Add("if"); ignoring.Add("in");
        ignoring.Add("indeed"); ignoring.Add("inside"); ignoring.Add("instead");
        ignoring.Add("into"); ignoring.Add("is"); ignoring.Add("it");
        ignoring.Add("its"); ignoring.Add("itself"); ignoring.Add("last");
        ignoring.Add("later"); ignoring.Add("latter"); ignoring.Add("latterly");
        ignoring.Add("least"); ignoring.Add("less"); ignoring.Add("lot");
        ignoring.Add("many"); ignoring.Add("may"); ignoring.Add("me");
        ignoring.Add("meanwhile"); ignoring.Add("might"); ignoring.Add("more");
        ignoring.Add("moreover"); ignoring.Add("most"); ignoring.Add("mostly");
        ignoring.Add("much"); ignoring.Add("must"); ignoring.Add("my");
        ignoring.Add("myself"); ignoring.Add("name"); ignoring.Add("namely");
        ignoring.Add("neither"); ignoring.Add("never"); ignoring.Add("nevertheless");
        ignoring.Add("next"); ignoring.Add("no"); ignoring.Add("nobody");
        ignoring.Add("none"); ignoring.Add("noone"); ignoring.Add("nor");
        ignoring.Add("not"); ignoring.Add("nothing"); ignoring.Add("now");
        ignoring.Add("nowhere"); ignoring.Add("of"); ignoring.Add("off");
        ignoring.Add("often"); ignoring.Add("on"); ignoring.Add("once");
        ignoring.Add("one"); ignoring.Add("only"); ignoring.Add("or");
        ignoring.Add("other"); ignoring.Add("others"); ignoring.Add("otherwise");
        ignoring.Add("ought"); ignoring.Add("our"); ignoring.Add("ours");
        ignoring.Add("ourselves"); ignoring.Add("out"); ignoring.Add("outside");
        ignoring.Add("over"); ignoring.Add("own"); ignoring.Add("part");
        ignoring.Add("per"); ignoring.Add("perhaps"); ignoring.Add("please");
        ignoring.Add("put"); ignoring.Add("rather"); ignoring.Add("re");
        ignoring.Add("same"); ignoring.Add("see"); ignoring.Add("seem");
        ignoring.Add("seemed"); ignoring.Add("seeming"); ignoring.Add("seems");
        ignoring.Add("several"); ignoring.Add("she"); ignoring.Add("should");
        ignoring.Add("show"); ignoring.Add("side"); ignoring.Add("since");
        ignoring.Add("so"); ignoring.Add("some"); ignoring.Add("somehow");
        ignoring.Add("someone"); ignoring.Add("something"); ignoring.Add("sometime");
        ignoring.Add("sometimes"); ignoring.Add("somewhere"); ignoring.Add("still");
        ignoring.Add("such"); ignoring.Add("take"); ignoring.Add("than");
        ignoring.Add("that"); ignoring.Add("the"); ignoring.Add("their");
        ignoring.Add("theirs"); ignoring.Add("them"); ignoring.Add("themselves");
        ignoring.Add("then"); ignoring.Add("thence"); ignoring.Add("there");
        ignoring.Add("thereafter"); ignoring.Add("thereby"); ignoring.Add("therefore");
        ignoring.Add("therein"); ignoring.Add("thereupon"); ignoring.Add("these");
        ignoring.Add("they"); ignoring.Add("this"); ignoring.Add("those");
        ignoring.Add("though"); ignoring.Add("through"); ignoring.Add("throughout");
        ignoring.Add("thru"); ignoring.Add("thus"); ignoring.Add("to");
        ignoring.Add("together"); ignoring.Add("too"); ignoring.Add("toward");
        ignoring.Add("towards"); ignoring.Add("under"); ignoring.Add("unless");
        ignoring.Add("until"); ignoring.Add("up"); ignoring.Add("upon");
        ignoring.Add("us"); ignoring.Add("used"); ignoring.Add("very");
        ignoring.Add("via"); ignoring.Add("was"); ignoring.Add("we");
        ignoring.Add("well"); ignoring.Add("were"); ignoring.Add("what");
        ignoring.Add("whatever"); ignoring.Add("when"); ignoring.Add("whence");
        ignoring.Add("whenever"); ignoring.Add("where"); ignoring.Add("whereafter");
        ignoring.Add("whereas"); ignoring.Add("whereby"); ignoring.Add("wherein");
        ignoring.Add("whereupon"); ignoring.Add("wherever"); ignoring.Add("whether");
        ignoring.Add("which"); ignoring.Add("while"); ignoring.Add("whither");
        ignoring.Add("who"); ignoring.Add("whoever"); ignoring.Add("whole");
        ignoring.Add("whom"); ignoring.Add("whose"); ignoring.Add("why");
        ignoring.Add("will"); ignoring.Add("with"); ignoring.Add("within");
        ignoring.Add("without"); ignoring.Add("would"); ignoring.Add("yes");
        ignoring.Add("yet"); ignoring.Add("you"); ignoring.Add("your");
        ignoring.Add("yours"); ignoring.Add("yourself"); ignoring.Add("yourselves");

    }// end of words method
    
    }

    // Sentiment enum and detector used by ChatBot
    public enum Sentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Happy
    }

    public class SentimentDetector
    {
        private Dictionary<Sentiment, List<string>> _triggers;
        private Random _rand = new Random();

        public SentimentDetector()
        {
            _triggers = new Dictionary<Sentiment, List<string>>()
            {
                { Sentiment.Worried, new List<string> { "worried", "scared", "afraid", "unsafe", "concern", "concerned", "anxious" } },
                { Sentiment.Curious, new List<string> { "curious", "interested", "wondering", "how", "what", "why" } },
                { Sentiment.Frustrated, new List<string> { "confused", "annoyed", "frustrated", "stuck", "problem", "issue" } },
                { Sentiment.Happy, new List<string> { "great", "awesome", "thanks", "thank", "good", "nice", "happy" } }
            };
        }

        public Sentiment Detect(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Sentiment.Neutral;

            var lower = input.ToLowerInvariant();
            var tokens = lower.Split(new[] { ' ', '\t', '\r', '\n', '.', ',', '!', '?', ';', ':', '\'', '"', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            var tokenSet = new HashSet<string>(tokens);

            foreach (var kv in _triggers)
            {
                foreach (var word in kv.Value)
                {
                    if (tokenSet.Contains(word))
                        return kv.Key;
                }
            }

            return Sentiment.Neutral;
        }

        public string GetSentimentResponse(Sentiment sentiment)
        {
            var responses = new List<string>();

            switch (sentiment)
            {
                case Sentiment.Worried:
                    responses.Add("I understand your concern. Cybersecurity can feel overwhelming sometimes.");
                    responses.Add("It's okay to feel worried about online safety.");
                    responses.Add("I understand why that would make you nervous.");
                    responses.Add("Cyber threats can be scary, but learning helps you stay protected.");
                    responses.Add("Don't worry, I'm here to help you understand cybersecurity.");
                    break;
                case Sentiment.Curious:
                    responses.Add("That's a great question!");
                    responses.Add("I like your curiosity about cybersecurity.");
                    responses.Add("Curiosity helps people stay safer online.");
                    responses.Add("I'm happy to explain more about cybersecurity.");
                    responses.Add("That's interesting to ask about.");
                    break;
                case Sentiment.Frustrated:
                    responses.Add("I know cybersecurity can feel confusing sometimes.");
                    responses.Add("Don't worry, I'll explain it more simply.");
                    responses.Add("It can definitely feel frustrating at first.");
                    responses.Add("Cybersecurity has many complex topics, but we can learn step by step.");
                    responses.Add("I understand your frustration. Let's work through it together.");
                    break;
                case Sentiment.Happy:
                    responses.Add("That's great to hear!");
                    responses.Add("I'm glad you are happy!");
                    responses.Add("Awesome! I'm happy this is helping you.");
                    responses.Add("That's wonderful! Would you like to learn more about cybersecurity?");
                    responses.Add("I'm glad you're enjoying the conversation.");
                    responses.Add("Great! Cybersecurity becomes easier when learning is enjoyable.");
                    break;
                default:
                    return string.Empty;
            }

            int index = _rand.Next(responses.Count);
            return responses[index];
        }
    }

} // end namespace POE2
