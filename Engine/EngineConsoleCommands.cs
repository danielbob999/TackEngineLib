using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;

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
                        if (overloadArgs != "") {
                            TackConsole.EngineLog(EngineLogType.Message, "     [" + overloadArgs + "]");
                        } else {
                            TackConsole.EngineLog(EngineLogType.Message, "     [No Args]");
                        }
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

        [CommandMethod("renderer.enableFpsCounter", "")]
        public static void RendererEnableFpsCounter(string[] args) {
            Renderer.TackRenderer.SetFpsCounterState(true);
        }

        [CommandMethod("renderer.disableFpsCounter", "")]
        public static void RendererDisableFpsCounter(string[] args) {
            Renderer.TackRenderer.SetFpsCounterState(false);
        }

        [CommandMethod("console.printOperationsOfCommandclass", "commandClassName:string")]
        public static void ConsolePrintOperationsOfCommandclass(string[] args) {
            if (args.Length == 2) {
                TackConsole.EngineLog(EngineLogType.Message, "Operations of Commandclass: " + args[1]);
                foreach (TackCommand command in TackConsole.GetLoadedTackCommands()) {
                    if (command.CommandCallString.StartsWith(args[1])) {
                        TackConsole.EngineLog(EngineLogType.Message, "      {0} {1} ({2} overloads)", command.CommandCallString.Remove(0, (args[1].Length + 1)), command.CommandArgList.FirstOrDefault(), command.CommandArgList.Count - 1);
                    }
                }
            }
        }

        [CommandMethod("tackengine.restartModule", "keepState:bool")]
        public static void TackEngineRestartModuleCommand(string[] args) {
            if (args.Length == 2) {
                if (args[1] != "") {
                    TackGameWindow.RestartModule(args[1], false);
                }
            } else if (args.Length == 3) {
                if (args[1] != "") {
                    if (bool.TryParse(args[2], out bool res)) {
                        TackGameWindow.RestartModule(args[1], res);
                    }
                }
            }
        }

        [CommandMethod("renderer.setVSync", "state:bool")]
        public static void RendererSetVSync(string[] args) {
            if (args.Length == 2) {
                if (bool.TryParse(args[1], out bool result)) {
                    if (result) {
                        TackEngine.currentWindow.VSync = OpenTK.VSyncMode.On;
                    } else {
                        TackEngine.currentWindow.VSync = OpenTK.VSyncMode.Off;
                    }
                }
            }
        }

        [CommandMethod("tackobject.listAll", "")]
        public static void TackObjectListAllObjects(string[] args) {
            TackConsole.EngineLog(EngineLogType.Message, "Loaded TackObjects:\n");
            TackConsole.EngineLog(EngineLogType.Message, string.Format("{0,-20} | {1,-20} | {2,5}", "Name", "IsActive", "Hash"));
            TackConsole.EngineLog(EngineLogType.Message, "--------------------------------------------------------------");

            Objects.TackObject[] tackObjects = Objects.TackObjectManager.GetAllTackObjects();

            for (int i = 0; i < tackObjects.Length; i++) {
                TackConsole.EngineLog(EngineLogType.Message, string.Format("{0,-20} | {1,-20} | {2,5}", tackObjects[i].Name, true, tackObjects[i].GetHash()));
            }

            TackConsole.EngineLog(EngineLogType.Message, "--------------------------------------------------------------");
        }

        [CommandMethod("physicscomponent.setvariable", "tackobjecthash:string variablename:string newvalue:object")]
        public static void PhysicsComponentSetVariable(string[] args) {
            if (args.Length != 4) {
                TackConsole.EngineLog(EngineLogType.Error, "Incorrect number of arguments for command: " + args[0]);
                return;
            }

            TackObject calledObject = TackObject.GetUsingHash(args[1]);

            if (calledObject == null) {
                TackConsole.EngineLog(EngineLogType.Error, "There is no TackObject with hash: " + args[1]);
                return;
            }

            PhysicsComponent calledComponent = calledObject.GetComponent<PhysicsComponent>();

            if (calledComponent.IsNullComponent()) {
                TackConsole.EngineLog(EngineLogType.Error, "There is no PhysicsComponent object connected to TackObject with hash: " + args[1]);
                return;
            }

            bool successfullySet = false;

            switch (args[2]) {
                case "collisionsenabled":
                    if (bool.TryParse(args[3], out bool c1Res)) {
                        calledComponent.CollisionsEnabled = c1Res;
                        successfullySet = true;
                    }
                    break;
                case "allowedtomove":
                    if (bool.TryParse(args[3], out bool c2Res)) {
                        calledComponent.AllowedToMove = c2Res;
                        successfullySet = true;
                    }
                    break;
                case "simulategravity":
                    if (bool.TryParse(args[3], out bool c3Res)) {
                        calledComponent.SimulateGravity = c3Res;
                        successfullySet = true;
                    }
                    break;
                case "weight":
                    if (float.TryParse(args[3], out float c4Res)) {
                        calledComponent.Weight = c4Res;
                        successfullySet = true;
                    }
                    break;
                default:
                    TackConsole.EngineLog(EngineLogType.Error, "There is no variable attached to PhysicsComponent with name: " + args[2]);
                    break;
            }

            if (successfullySet) {
                TackConsole.EngineLog(EngineLogType.Message, "Successfully set physicscomponent." + args[2] + " on TackObject with hash: " + args[1] + ", to value: " + args[3]);
            }
        }

        [CommandMethod("physicscomponent.getvariable", "tackobjecthash:string variablename:string")]
        public static void PhysicsComponentGetVariable(string[] args) {
            if (args.Length != 3) {
                TackConsole.EngineLog(EngineLogType.Error, "Incorrect number of arguments for command: " + args[0]);
                return;
            }

            TackObject calledObject = TackObject.GetUsingHash(args[1]);

            if (calledObject == null) {
                TackConsole.EngineLog(EngineLogType.Error, "There is no TackObject with hash: " + args[1]);
                return;
            }

            PhysicsComponent calledComponent = calledObject.GetComponent<PhysicsComponent>();

            if (calledComponent.IsNullComponent()) {
                TackConsole.EngineLog(EngineLogType.Error, "There is no PhysicsComponent object connected to TackObject with hash: " + args[1]);
                return;
            }

            switch (args[2]) {
                case "collisionsenabled":
                    TackConsole.EngineLog(EngineLogType.Message, "physicscomponent." + args[2] + " on TackObject with hash: " + args[1] + ", has value of: " + calledComponent.CollisionsEnabled.ToString());
                    break;
                case "allowedtomove":
                    TackConsole.EngineLog(EngineLogType.Message, "physicscomponent." + args[2] + " on TackObject with hash: " + args[1] + ", has value of: " + calledComponent.AllowedToMove.ToString());
                    break;
                case "simulategravity":
                    TackConsole.EngineLog(EngineLogType.Message, "physicscomponent." + args[2] + " on TackObject with hash: " + args[1] + ", has value of: " + calledComponent.SimulateGravity.ToString());
                    break;
                case "weight":
                    TackConsole.EngineLog(EngineLogType.Message, "physicscomponent." + args[2] + " on TackObject with hash: " + args[1] + ", has value of: " + calledComponent.Weight.ToString());
                    break;
                default:
                    TackConsole.EngineLog(EngineLogType.Error, "There is no variable attached to PhysicsComponent with name: " + args[2]);
                    break;
            }
        }
    }
}
