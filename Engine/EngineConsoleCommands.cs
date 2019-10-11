using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.Engine
{
    internal static class EngineConsoleCommands
    {
        [CommandMethod("help", "", "commandName:string")]
        public static void HelpCommmand(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(EngineLogType.Message, "Commands:");

                foreach (TackCommand command in TackConsole.GetLoadedTackCommands()) {
                    TackConsole.EngineLog(EngineLogType.Message, "     " + command.CommandCallString);
                }

                return;
            }

            if (args.Length == 2) {
                TackCommand com = null;

                foreach (TackCommand command in TackConsole.GetLoadedTackCommands()) {
                    if (args[1] == command.CommandCallString) {
                        com = command;
                    }
                }

                if (com != null) {
                    TackConsole.EngineLog(EngineLogType.Message, com.CommandCallString + ":");

                    foreach (string overloadArgs in com.CommandArgList) {
                        TackConsole.EngineLog(EngineLogType.Message, "     " + overloadArgs);
                    }
                }

                return;
            }

            //TackConsole.EngineLog(EngineLogType.Message, "The TackCommand with call string '" + thisCommandData.GetCallString() + "' has no definition that takes " + args.Length + " arguments");
        }

        [CommandMethod("renderer.printWidth", "")]
        public static void RendererPrintWidthCommand(string[] args) {
            TackConsole.EngineLog(EngineLogType.Message, "Current Window Width: {0}", TackEngine.MainCamera.CameraScreenWidth);
        }

        [CommandMethod("renderer.printHeight", "")]
        public static void RendererPrintHeightCommand(string[] args) {
            TackConsole.EngineLog(EngineLogType.Message, "Current Window Height: {0}", TackEngine.MainCamera.CameraScreenHeight);
        }
    }
}
