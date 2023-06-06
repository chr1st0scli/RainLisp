namespace RainLispConsole
{
    internal static class Resources
    {
        internal const string WELCOME_MESSAGE = @"Available Modes:
----------------
0: Single-line REPL,
1: Multi-line REPL,
2: Code Editor
3: Help
----------------";
        internal const string COMMAND_LINE_HELP = @"
HELP
====
Modes
-----
Single-line REPL provides a Read-Eval-Print-Loop, based on individual line evaluations.

Multi-line REPL provides a Read-Eval-Print-Loop, based on evaluation of several lines at a time.

Code Editor provides whole program evaluation, syntax highlighting, basic code completion and source code file management.

Command Line Arguments
----------------------
[-c <code>]
Evaluates the specified code and exits.
Example: RainLispConsole -c ""(+ 1 2 3)""

[-f <file path>]
Evaluates the contents of the specified file and exits.
Example: RainLispConsole -f somefile.rl";
        internal const string CODE_OPTION = "-c";
        internal const string FILE_OPTION = "-f";
        internal const string MODE_PROMPT = "(0/1/2/3)?";
        internal const string REPL_PROMPT = "Hit Enter to evaluate > ";
        internal const string REPL_MULTILINE_PROMPT = "Ctrl + Z on a new line and hit Enter to evaluate >";
        internal const string TITLE = "Rainλisp";
        internal const string CODE_EDITOR = "Code Editor";
        internal const string OUTPUT = "Output";
        internal const string FILE_EXT = ".rl";
        internal const string EVALUATE = "Ctrl-Enter Evaluate";
        internal const string QUIT = "Ctrl-F4 Quit";
        internal const string NEW = "New";
        internal const string OPEN = "Open";
        internal const string SAVE = "Save";
        internal const string SAVE_AS = "Save As";
        internal const string VIEW_HELP = "View Help";
        internal const string ABOUT = "About";
        internal const string FILE = "File";
        internal const string HELP = "Help";
        internal const string OPEN_FILE = "Open file";
        internal const string SAVE_FILE = "Save file";
        internal const string OK = "Ok";
        internal const string UNTITLED = "Untitled";
        internal const string OVERWRITE_FILE = "The file already exists. Are you sure you want to overwrite it?";
        internal const string LOSE_UNSAVED_CHANGES = "There are unsaved changes. Are you sure you want to continue and lose them?";
        internal const string CONFIRMATION = "Confirmation";
        internal const string ERROR = "Error";
        internal const string YES = "Yes";
        internal const string NO = "No";
        internal const string CURSOR_POS_FORMAT = "Ln: {0}, Col: {1}";
        internal const string HELP_CONTENTS = @"https://github.com/chr1st0scli/RainLisp/blob/master/RainLisp/Docs/contents.md
https://github.com/chr1st0scli/RainLisp/blob/master/RainLisp/Docs/quick-start.md";

        internal const string LOGO = @"

                                           ╓████▄
                                          ╔██▀▀███.
                                          █""    ╙██
                       ,,,w╥«»»»«╥ww,,  ,╓▒»»H***╜▀***H»»w╓,
                  ╓@╙`                 ""╙                   `╠H~»HHH≡~~╖,
            ╓H*""""`╙╙*                                                     ╙╗
            ╙≡╖,                                                         ,╓╝
                      '""""*^*H~~~~~~~─**╜""""**^^*j▄▄▄▄▄▄*^**""""`  `
       ╓▄▄▄▄▄;                                ,████ ██
        ▐█┘  ▐█W             ▀▀              ┌████  ▐█▌          '▀▀
        █▌   ██┘    ╓╥╖,▄  ,▄▄  ,▄▄  ▄▄     ┌████    ██        ,╓▄▄   ╓▄╖╓∞ ,▄▄ ,▄▄
       ▐█▀&█▀╙    ▄▀   █▌   █▌   █▌ƒ""]█M   ┌████     ▐█▌      ▄ ]█╛  ▐█  ▐""  ██╜  ▐█
      ]█▌  ██    █▌   ██   ▐█   ▐█▀  █▌   ┌████`      ███,  ,█▌ █▌    ▀█▄   ▐█'   ██
      ██   ╙█▌  ▐█, ╓╣█░╥ ]█░a ,█▀  ▐█,∞ ╓████`        ███████ ▐█,≡ ▌  '█M  █▌  ,█▀
    ""▀▀▀╙   ╙▀╙` ▀▀╙ ""▀""  ""▀╙  ╙▀   ╙▀'  ▀▀▀▀`          ╘▀▀▀└  ╙▀"" └▀╙*╙'  ██▀*╙└
                                                                          ╓█⌐
    
";

        internal const string INFO = @"
  _____       _      __     _           
 |  __ \     (_)     \ \   (_)          
 | |__) |__ _ _ _ __  \ \   _ ___ _ __  
 |  _  // _` | | '_ \  > \ | / __| '_ \ 
 | | \ \ (_| | | | | |/ ^ \| \__ \ |_) |
 |_|  \_\__,_|_|_| |_/_/ \_\_|___/ .__/ 
                                 | |    
                                 |_|    

https://github.com/chr1st0scli/RainLisp
";
    }
}
