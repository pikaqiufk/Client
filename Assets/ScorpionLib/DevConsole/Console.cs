using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClientService;
using DataContract;
using DevConsole;
using ScorpionNetLib;
using EventType = UnityEngine.EventType;

namespace DevConsole
{
    [System.Serializable]
    public class Console : MonoBehaviour
    {

        //=============================
        //VARS
        //=============================
        [SerializeField]
        float fadeSpeed = 500f;                         //Pixels/sec at which console fades in & out
        const int TEXT_AREA_OFFSET = 7;                 //Margin for the text at the bottom of the console
        bool helpEnabled = true;                        //Is the help for autocompletion on?
        int numHelpCommandsToShow = 5;                  //Max candidates to show on autocompletion
        float helpWindowWidth = 200;                    //The width of the autocompletion window
        const int WARNING_THRESHOLD = 15000;            //Number of characters in consoleText at which a warning appears
        const int DANGER_THRESHOLD = 16000;             //Number of characters in consoleText at which a danger appears	
        const int AUTOCLEAR_THRESHOLD = 18000;          //At this many characters, a DC_CLEAR will be done automatically


        List<Command> consoleCommands;                  //Whole list of commands available				
        List<string> candidates = new List<string>();   //Commands that match existing text
        int selectedCandidate = 0;                      //Index of candidate selected
        List<string> history = new List<string>();      //A history of texts sent into the console
        int selectedHistory = 0;                        //Current index in the history

        [SerializeField]
        KeyCode consoleKey = KeyCode.Backslash;         //Key to open/close Console
        [SerializeField]
        GUISkin skin;                                   //GUISkin to use
        [SerializeField]
        bool dontDestroyOnLoad;                         //Sets whether it destroys on load or not

        static Console singleton;                       //Singleton
        bool opening;                                   //Can write already?
        bool closed = true;                             //Is the Console closed?
        bool showHelp = true;                           //Should Help Window show?
        bool inHistory = false;                         //Are we browsing the history?
        bool showTimeStamp;                             //Should time stamps be shown?

        float numLinesThreshold;                        //Max numes allowed in the console
        float maxConsoleHeight;                         //Screen.height/3

        float currentConsoleHeight;                     //Current Y position in pixels
        Vector2 consoleScroll = Vector2.zero;
        Vector2 helpWindowScroll = Vector2.zero;

        string consoleText = string.Empty;              //Text in the whole console log
        string inputText = string.Empty;                //Text in the input TextField
        string lastText = string.Empty;                 //Used for history mostly
        int numLines;                                   //Number of '\n' in consoleText

        //=============================
        //AWAKE
        //=============================



        public static void Show()
        {
            singleton.opening = !singleton.opening;
            singleton.StartCoroutine(singleton.FadeInOut(singleton.opening));
        }
        void Awake()
        {
            if (singleton == null)
                singleton = this;
            else
            {
                Debug.LogWarning("There can only be one Console per project");
                Destroy(this);
                return;
            }
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
            consoleCommands = new List<Command>(){
                new Command("HELP",GeneralHelp,"Shows a list of all Commands available"),
                new Command("DC_INFO",ShowInfo, "Shows info about the DevConsole"),
                new Command("DC_CLEAR",Clear, "Clears the console"),
                new Command("DC_CHANGE_KEY",ChangeKey, ChangeKeyHelp),
                new Command("DC_SHOW_DEBUGLOG", ShowLog, "Establishes whether or not to show Unity Debug Log"),
                new Command("DC_SHOW_TIMESTAMP",ShowTimeStamp, "Establishes whether or not to show the time stamp for each command")
            };
            GMCommand.regeditGMCommand();
        }

        void Start()
        {
            //Application.RegisterLogCallback(LogCallback);
        }

        //=============================
        //GUI
        //=============================
        void OnGUI()
        {
            if (skin != null && GUI.skin != skin)
                GUI.skin = skin;
            if (consoleKey == KeyCode.None)
                return;

            Event current = Event.current;
            GUI.skin.textArea.richText = true;

            //Open/Close Console
            if (current.type == EventType.KeyDown && current.keyCode == consoleKey)
            {
                opening = !opening;
                GUIUtility.keyboardControl = 0;
                StartCoroutine(FadeInOut(opening));
            }

            //Local declarations
            bool moving = !((currentConsoleHeight == maxConsoleHeight) || (currentConsoleHeight == 0));
            float lineHeight = GUI.skin.textArea.lineHeight;
            float height = lineHeight * numLines;
            float scrollHeight = height > currentConsoleHeight ? height : currentConsoleHeight;
            string controlname = gameObject.GetHashCode().ToString();
            if (!closed)
            {
                if (!moving)
                    GUI.FocusControl(controlname);
                //KEYS
                if (current.type == EventType.keyDown)
                {
                    if (inputText != string.Empty)
                    {
                        switch (current.keyCode)
                        {
                            case KeyCode.Return:

                                PrintInput(inputText);
                                break;
                            case KeyCode.Tab:
                                if (candidates.Count != 0)
                                {
                                    inputText = candidates[selectedCandidate];
                                    //showHelp = false;
                                    SetCursorPos(inputText.Length);
                                    candidates.Clear();
                                }
                                break;
                            case KeyCode.Escape:
                                showHelp = false;
                                candidates.Clear();
                                break;
                            case KeyCode.F1:
                                showHelp = true;
                                break;
                        }
                    }
                    else { }
                    switch (current.keyCode)
                    {
                        case KeyCode.UpArrow:
                            if ((inHistory || inputText == string.Empty) && history.Count != 0)
                            {
                                selectedHistory = Mathf.Clamp(selectedHistory + (inHistory ? 1 : 0), 0, history.Count - 1);
                                inputText = history[selectedHistory];
                                showHelp = false;
                                inHistory = true;
                                lastText = inputText;
                            }
                            else if (inputText != string.Empty && !inHistory)
                            {
                                selectedCandidate = Mathf.Clamp(--selectedCandidate, 0, candidates.Count - 1);
                                if (selectedCandidate * lineHeight <= helpWindowScroll.y ||
                                    selectedCandidate * lineHeight > helpWindowScroll.y + lineHeight * (numHelpCommandsToShow - 1))
                                    helpWindowScroll = new Vector2(0, selectedCandidate * lineHeight - 1 * lineHeight);
                            }
                            SetCursorPos(inputText.Length);
                            break;
                        case KeyCode.DownArrow:
                            if ((inHistory || inputText == string.Empty) && history.Count != 0)
                            {
                                selectedHistory = Mathf.Clamp(selectedHistory - (inHistory ? 1 : 0), 0, history.Count - 1);
                                inputText = history[selectedHistory];
                                showHelp = false;
                                inHistory = true;
                                lastText = inputText;
                            }
                            else if (inputText != string.Empty && !inHistory)
                            {
                                selectedCandidate = Mathf.Clamp(++selectedCandidate, 0, candidates.Count - 1);
                                if (selectedCandidate * lineHeight > helpWindowScroll.y + lineHeight * (numHelpCommandsToShow - 2) ||
                                    selectedCandidate * lineHeight < helpWindowScroll.y)
                                    helpWindowScroll = new Vector2(0, selectedCandidate * lineHeight - ((numHelpCommandsToShow - 2) * lineHeight));
                            }
                            SetCursorPos(inputText.Length);
                            break;
                    }
                }

                if (lastText != inputText)
                {
                    inHistory = false;
                    lastText = string.Empty;
                }
                //CONSOLE PAINTING
                GUI.Box(new Rect(0, 0, Screen.width, currentConsoleHeight), new GUIContent());
                GUI.SetNextControlName(controlname);
                inputText = GUI.TextField(new Rect(0, currentConsoleHeight + 0, Screen.width, 100), inputText);
                GUI.skin.textArea.normal.background = null;
                GUI.skin.textArea.hover.background = null;
                consoleScroll = GUI.BeginScrollView(new Rect(0, 0, Screen.width, currentConsoleHeight), consoleScroll,
                                                    new Rect(0, 0, Screen.width - 20, scrollHeight));
                GUI.TextArea(new Rect(0, currentConsoleHeight - 0 - (numLines == 0 ? -0 + lineHeight : height) +
                                      (numLines >= numLinesThreshold - 1 ? lineHeight * (numLines - numLinesThreshold) : 0),
                                      Screen.width, TEXT_AREA_OFFSET + (numLines == 0 ? lineHeight : height)), consoleText);
                GUI.EndScrollView();
                if (inputText == string.Empty)
                    showHelp = true;
            }

            if (!inputText.Trim().Equals(string.Empty))
            {
                if (GUI.Button(new Rect(Screen.width / 2 - 100, currentConsoleHeight + 120, 200, 120), string.Format("发送")))
                {
                    PrintInput(inputText);
                }
            }

            //HELP WINDOW
            if (showHelp && helpEnabled && inputText.Trim() != string.Empty)
            {
                ShowHelp();
                if (candidates.Count != 0)
                {
                    string help = string.Empty;
                    {
                        var __list1 = candidates;
                        var __listCount1 = __list1.Count;
                        for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                        {
                            var s = (string)__list1[__i1];
                            help += (candidates[selectedCandidate] == s ? "<color=yellow>" + s + "</color>" : s) + '\n';
                        }
                    }
                    GUI.skin.textArea.normal.background = GUI.skin.textField.normal.background;
                    GUI.skin.textArea.hover.background = GUI.skin.textField.hover.background;
                    if (candidates.Count > numHelpCommandsToShow)
                    {
                        helpWindowScroll = GUI.BeginScrollView(new Rect(0, currentConsoleHeight - numHelpCommandsToShow * lineHeight - TEXT_AREA_OFFSET,
                                                    helpWindowWidth, 5 + lineHeight * numHelpCommandsToShow), helpWindowScroll,
                                                               new Rect(0, 0, helpWindowWidth - 20, TEXT_AREA_OFFSET + candidates.Count * lineHeight));
                        GUI.TextArea(new Rect(0, 0, helpWindowWidth, TEXT_AREA_OFFSET + candidates.Count * lineHeight), help);
                        GUI.EndScrollView();
                    }
                    else
                        GUI.TextArea(new Rect(0, currentConsoleHeight - TEXT_AREA_OFFSET -
                                          (candidates.Count > numHelpCommandsToShow ? numHelpCommandsToShow * lineHeight :
                                               lineHeight * candidates.Count), helpWindowWidth,
                                          (candidates.Count > numHelpCommandsToShow ? numHelpCommandsToShow * lineHeight :
                                             lineHeight * candidates.Count) + TEXT_AREA_OFFSET), help);
                }
            }

        }

        //=============================
        //OTHERS
        //=============================
        IEnumerator FadeInOut(bool opening)
        {
            maxConsoleHeight = Screen.height / 3;
            numLinesThreshold = maxConsoleHeight / GUI.skin.textArea.lineHeight;
            closed = false;
            do
            {
                if (opening)
                    currentConsoleHeight = Mathf.Min(currentConsoleHeight + fadeSpeed * Time.deltaTime, maxConsoleHeight);
                else
                    currentConsoleHeight = Mathf.Max(currentConsoleHeight - fadeSpeed * Time.deltaTime, 0);
                if (currentConsoleHeight == 0 || currentConsoleHeight == maxConsoleHeight)
                    opening = !opening;
                yield return null;
            } while (opening == this.opening);
            if (currentConsoleHeight == 0)
                closed = true;
            if (closed)
                inputText = string.Empty;
        }

        void ShowHelp()
        {
            string aux = string.Empty;
            if (candidates.Count != 0 && selectedCandidate >= 0 && candidates.Count > selectedCandidate)
                aux = candidates[selectedCandidate];
            candidates.Clear();
            for (int i = 0; i < consoleCommands.Count; i++)
            {
                if (consoleCommands[i].CommandName.Length < inputText.Length) continue;
                string tempStr = consoleCommands[i].CommandName.Substring(0, inputText.Length);
                //Logger.Error("{0}:{1}", inputText, consoleCommands[i].CommandName);
                if (String.Compare(inputText, tempStr, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    candidates.Add(consoleCommands[i].CommandName);
                }
                //if(consoleCommands[i].CommandName.StartsWith(inputText.ToUpper()))
                //    candidates.Add(consoleCommands[i].CommandName);
            }
            if (aux == string.Empty)
            {
                selectedCandidate = 0;
                return;
            }
            for (int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i] == aux)
                {
                    selectedCandidate = i;
                    return;
                }
            }
            selectedCandidate = 0;
        }

        #region Tools
        void SetCursorPos(int pos)
        {
            TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            te.pos = pos;
            te.selectPos = te.pos;
        }

        public static string ColorToHex(Color color)
        {
            string hex = "0123456789ABCDEF";
            int r = (int)(color.r * 255);
            int g = (int)(color.g * 255);
            int b = (int)(color.b * 255);

            return hex[(int)(Mathf.Floor(r / 16))].ToString() + hex[(int)(Mathf.Round(r % 16))].ToString() +
                hex[(int)(Mathf.Floor(g / 16))].ToString() + hex[(int)(Mathf.Round(g % 16))].ToString() +
                hex[(int)(Mathf.Floor(b / 16))].ToString() + hex[(int)(Mathf.Round(b % 16))].ToString();
        }

        bool StringToBool(string value, out bool result)
        {
            bool bResult = result = false;
            int iResult = 0;

            if (bool.TryParse(value, out bResult))
            {
                result = bResult;
                return true;
            }
            else if (int.TryParse(value, out iResult))
            {
                if (iResult == 1 || iResult == 0)
                {
                    result = iResult == 1 ? true : false;
                    return true;
                }
                else
                    return false;
            }
            else if (value.ToLower().Equals("yes") || value.ToLower().Equals("no"))
            {
                result = value.ToLower().Equals("yes") ? true : false;
                return true;
            }
            else
                return false;

        }
        #endregion
        //=============================
        //PRINTS
        //=============================
        #region Logs
        /// <summary>
        /// Logs a white text to the console.
        /// </summary>
        /// <param name="text">Text to be sent.</param>
        public static void Log(string text)
        {
            singleton.BasePrint(text);
        }
        /// <summary>
        /// Logs a ligh blue text to the console.
        /// </summary>
        /// <param name="text">Text to be sent.</param>
        public static void LogInfo(string text)
        {
            singleton.BasePrint(text, Color.cyan);
        }
        /// <summary>
        /// Logs a yellow text to the console.
        /// </summary>
        /// <param name="text">Text to be sent.</param>
        public static void LogWarning(string text)
        {
            singleton.BasePrint(text, Color.yellow);
        }
        /// <summary>
        /// Logs a red text to the console.
        /// </summary>
        /// <param name="text">Text to be sent.</param>
        public static void LogError(string text)
        {
            singleton.BasePrint(text, Color.red);
        }
        /// <summary>
        /// Logs a text to the console with the specified color.
        /// </summary>
        /// <param name="text">Text to be sent.</param>
        /// <param name="color">Color provided in HTML format.</param>
        public static void Log(string text, string color)
        {
            singleton.BasePrint(text, color);
        }
        /// <summary>
        /// Logs a text to the console with the specified color.
        /// </summary>
        /// <param name="text">Text to be sent.</param>
        /// <param name="color">Color to be used.</param>
        public static void Log(string text, Color color)
        {
            singleton.BasePrint(text, color);
        }
        #endregion
        #region Prints
        void BasePrint(string text)
        {
            BasePrint(text, ColorToHex(Color.white));
        }
        void BasePrint(string text, Color color)
        {
            BasePrint(text, ColorToHex(color));
        }
        delegate void AddText(string t, string c);
        void BasePrint(string text, string color)
        {
            text = "> " + text;
            AddText addText = delegate (string t, string c)
            {
                consoleText += (showTimeStamp ? "[" + System.DateTime.Now.ToShortTimeString() + "]  " : "") + "<color=#" + c + ">" + t + "</color>";
            };
            int numLineJumps = 1;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                    numLineJumps++;
            }
            text += '\n';
            numLines += numLineJumps;
            if (numLines >= numLinesThreshold - 1)
                consoleScroll = new Vector2(0, consoleScroll.y + int.MaxValue);
            addText(text, color);
            if (consoleText.Length >= AUTOCLEAR_THRESHOLD)
            {
                Clear();
                addText("Buffer cleared automatically\n", ColorToHex(Color.yellow));
            }
            else if (consoleText.Length >= DANGER_THRESHOLD)
                addText("Buffer size too large. You should clear the console\n", ColorToHex(Color.red));
            else if (consoleText.Length >= WARNING_THRESHOLD)
                addText("Buffer size too large. You should clear the console\n", ColorToHex(Color.yellow));
        }

        public void PrintInput(string input)
        {

            // var strs = input.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            if (!isGmCommand(input))
            {
                BasePrint(input);
                for (int i = 0; i < consoleCommands.Count; i++)
                {
                    if (input.StartsWith(consoleCommands[i].CommandName))
                    {
                        if (input == consoleCommands[i].CommandName + "?")
                            consoleCommands[i].ShowHelp();
                        else
                            consoleCommands[i].Execute(input.Substring(
                                consoleCommands[i].CommandName.Length + (input.Contains(" ") ? 1 : 0)));
                    }
                }
            }
        }

        bool isGmCommand(string command)
        {
            inputText = string.Empty;
            history.Insert(0, command);
            selectedHistory = 0;


            bool ret = false;
            var gmCommand = command.Trim();
            if ((gmCommand.Substring(0, 2)) == "!!")
            {
                var strs = command.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                BasePrint(command);
                if (NetManager.Instance.Connected)
                    SendGmCommandImpl(strs.GetEnumerator());
                else
                    LogError("need connect to server frist!!");
                ret = true;
            }
            return ret;
        }

        private void SendGmCommandImpl(IEnumerator enumerator)
        {
            if (!enumerator.MoveNext())
            {
                return;
            }
            StartCoroutine(SendGmCommand(enumerator));
        }

        private IEnumerator SendGmCommand(IEnumerator enumerator)
        {
            OutMessage msg = null;
            var command = (string)enumerator.Current;
            if (command != null)
            {
                command = command.Trim();
                string[] strs = command.Split(',');
                eGmCommandType sceneid;
                if (GMCommand.mGms.TryGetValue(strs[0], out sceneid))
                {
                    switch (sceneid)
                    {
                        case eGmCommandType.GMLocal:
                        {
                            if (strs[0] == "!!EnableNewFunctionTip")
                            {
                                GameSetting.Instance.EnableNewFunctionTip = !GameSetting.Instance.EnableNewFunctionTip;
                                yield break;
                            }
                        }
                            break;
                        case eGmCommandType.GMLogic:
                            msg = NetManager.Instance.GMLogic(command);
                            break;
                        case eGmCommandType.GMChat:
                            msg = NetManager.Instance.GMChat(command);
                            break;
                        case eGmCommandType.GMScene:
                            msg = NetManager.Instance.GMScene(command);
                            break;
                        case eGmCommandType.GMTeam:
                            msg = NetManager.Instance.GMTeam(command);
                            break;
                        case eGmCommandType.GMRank:
                            msg = NetManager.Instance.GMRank(command);
                            break;
                        case eGmCommandType.GMAll:
                            {
                                OutMessage msgScene = NetManager.Instance.GMScene(command);
                                yield return msgScene.SendAndWaitUntilDone();
                                if (msgScene.State == MessageState.Reply)
                                {
                                    if (msgScene.ErrorCode == (int)ErrorCodes.OK)
                                    {
                                    }
                                }
                            }
                            {
                                OutMessage msgTeam = NetManager.Instance.GMTeam(command);
                                yield return msgTeam.SendAndWaitUntilDone();
                                if (msgTeam.State == MessageState.Reply)
                                {
                                    if (msgTeam.ErrorCode == (int)ErrorCodes.OK)
                                    {
                                    }
                                }
                            }
                            {
                                OutMessage msgRank = NetManager.Instance.GMRank(command);
                                yield return msgRank.SendAndWaitUntilDone();
                                if (msgRank.State == MessageState.Reply)
                                {
                                    if (msgRank.ErrorCode == (int)ErrorCodes.OK)
                                    {
                                    }
                                }

                            }
                            msg = NetManager.Instance.GMLogic(command);
                            break;
                    }

                    yield return msg.SendAndWaitUntilDone();

                    if (msg.State == MessageState.Timeout)
                    {
                        LogError(string.Format("GMCommand {0} TimeOut", command));
                    }
                    else if (msg.ErrorCode == (int)ErrorCodes.OK)
                    {
                        LogInfo(string.Format("GMCommand {0} Return {1}", command,
                            Enum.GetName(typeof(ErrorCodes), msg.ErrorCode)));
                    }
                    else
                    {
                        LogError(string.Format("GMCommand {0} Return {1}", command,
                            Enum.GetName(typeof(ErrorCodes), msg.ErrorCode)));
                    }
                }
            }
            SendGmCommandImpl(enumerator);
        }

        void LogCallback(string log, string stackTrace, LogType type)
        {
            Color color;
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                color = Color.red;
            else if (type == LogType.Warning)
                color = Color.yellow;
            else// if (type == LogType.Log)
                color = Color.cyan;
            BasePrint(log, color);
            BasePrint(stackTrace, color);
            for (int i = 0; i < stackTrace.Length; i++)
            {
                if (stackTrace[i] == '\n')
                    numLines++;
            }
        }
        #endregion
        //===========================
        //COMMANDS
        //===========================
        #region Manage Commands
        public static void AddCommand(Command c)
        {
            if (!CommandExists(c.CommandName))
                singleton.consoleCommands.Add(c);
        }
        public static void AddCommand(string commandName, Command.NoArgs function)
        {
            if (!CommandExists(commandName))
                singleton.consoleCommands.Add(new Command(commandName, function));
        }
        public static void AddCommand(string commandName, Command.OneStringArg function)
        {
            if (!CommandExists(commandName))
                singleton.consoleCommands.Add(new Command(commandName, function));
        }
        public static void AddCommand(string commandName, Command.NoArgs function, string helpInfo)
        {
            if (!CommandExists(commandName))
                singleton.consoleCommands.Add(new Command(commandName, function, helpInfo));
        }
        public static void AddCommand(string commandName, Command.OneStringArg function, string helpInfo)
        {
            if (!CommandExists(commandName))
                singleton.consoleCommands.Add(new Command(commandName, function, helpInfo));
        }
        public static void AddCommand(string commandName, Command.NoArgs function, Command.NoArgs helpFunction)
        {
            if (!CommandExists(commandName))
                singleton.consoleCommands.Add(new Command(commandName, function, helpFunction));
        }
        public static void AddCommand(string commandName, Command.OneStringArg function, Command.NoArgs helpFunction)
        {
            if (!CommandExists(commandName))
                singleton.consoleCommands.Add(new Command(commandName, function, helpFunction));
        }
        static bool CommandExists(string commandName)
        {
            {
                var __list2 = singleton.consoleCommands;
                var __listCount2 = __list2.Count;
                for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var c = (Command)__list2[__i2];
                    {
                        if (c.CommandName.ToUpper() == commandName.ToUpper())
                        {
                            LogError("The command " + commandName + " already exists");
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static void RemoveCommand(string commandName)
        {
            {
                var __list3 = singleton.consoleCommands;
                var __listCount3 = __list3.Count;
                for (int __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var c = (Command)__list3[__i3];
                    {
                        if (c.CommandName == commandName)
                        {
                            singleton.consoleCommands.Remove(c);
                            Log("Command " + commandName + " removed successfully", Color.green);
                            return;
                        }
                    }
                }
            }
            LogWarning("The command " + commandName + " could not be found");
        }
        #endregion
        #region Predefined Commands
        void GeneralHelp()
        {
            string text = string.Empty;
            for (int i = 0; i < consoleCommands.Count; i++)
            {
                text += consoleCommands[i].CommandName;
                text += '\n';
            }
            LogInfo("List of commands available:\n" + text);
        }
        void Clear()
        {
            singleton.consoleText = string.Empty;
            singleton.numLines = 0;
        }

        void ChangeKey(string key)
        {
            //If not a number
            int n;
            if (!int.TryParse(key, out n))
            {
                try
                {
                    singleton.consoleKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), key, true);
                    Log("Change successful", Color.green);
                }
                catch
                {
                    LogError("The entered value is not a valid KeyCode value");
                }
            }
            else
            {
                string[] keyCodes = System.Enum.GetNames(typeof(KeyCode));
                if (n >= 0 || n < keyCodes.Length)
                {
                    try
                    {
                        singleton.consoleKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodes[n], true);
                        Log("Change successful", Color.green);
                    }
                    catch
                    {
                        LogError("The entered value is not a valid KeyCode value");
                    }
                }
                else
                    LogError("The entered value is not a valid KeyCode value");
            }
        }
        void ChangeKeyHelp()
        {
            string[] keyCodes = System.Enum.GetNames(typeof(KeyCode));
            string help = "\nSPECIAL KEYS 1: ";
            int lineLength = 0;
            for (int i = 0; i < keyCodes.Length; i++)
            {
                string text = string.Empty;
                if (i == 22)
                {
                    text = "\n\nNUMERIC KEYS: ";
                    lineLength = 0;
                }
                else if (i == 32)
                {
                    text = "\n\nSPECIAL KEYS 2: ";
                    lineLength = 0;
                }
                else if (i == 45)
                {
                    text = "\n\nALPHA KEYS: ";
                    lineLength = 0;
                }
                else if (i == 71)
                {
                    text = "\n\nKEYPAD KEYS: ";
                    lineLength = 0;
                }
                else if (i == 89)
                {
                    text = "\n\nSPECIAL KEYS 3: ";
                    lineLength = 0;
                }
                else if (i == 98)
                {
                    text = "\n\nF KEYS: ";
                    lineLength = 0;
                }
                else if (i == 113)
                {
                    text = "\n\nSPECIAL KEYS 4: ";
                    lineLength = 0;
                }
                else if (i == 134)
                {
                    text = "\n\nMOUSE: ";
                    lineLength = 0;
                }
                else if (i == 141)
                {
                    text = "\n\nJOYSTICK KEYS: ";
                    lineLength = 0;
                }
                text += keyCodes[i] + "[" + i + "]" + (i != keyCodes.Length - 1 ? "," : "");
                lineLength += text.Length;
                help += text;
                if (lineLength >= 65)
                {
                    help += '\n';
                    lineLength = 0;
                }
            }
            LogInfo("Command Info: " + help);
        }

        void ShowLog(string value)
        {
            bool result;
            if (StringToBool(value, out result))
            {
                if (result)
                    Application.RegisterLogCallback(LogCallback);
                else
                    Application.RegisterLogCallback(null);
                Log("Change successful", Color.green);
            }
            else
                LogError("The entered value is not a valid boolean value");
        }
        void ShowTimeStamp(string value)
        {
            bool result;
            if (StringToBool(value, out result))
            {
                showTimeStamp = result;
                Log("Change successful", Color.green);
            }
            else
                LogError("The entered value is not a valid boolean value");
        }
        void ShowInfo()
        {
            string text = "DevConsole by CobsTech \n" +
                "Version 1.0\n" +
                "Contact/Support: antoniocogo@gmail.com\n+" +
                "More updates soon";
            Console.LogInfo(text);
        }
        #endregion
    }
}