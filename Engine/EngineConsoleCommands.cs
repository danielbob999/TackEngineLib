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
    /// <summary>
    /// The holder class for all TackConsole commands
    /// </summary>
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

        [CommandMethod("renderer.v-sync", "", "state:bool")]
        public static void RendererSetVSync(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(EngineLogType.Message, "Value: " + (TackEngine.currentWindow.VSync == OpenTK.VSyncMode.Off ? "Off" : "On"));
                return;
            }

            if (args.Length == 2) {
                if (bool.TryParse(args[1], out bool result)) {
                    if (result) {
                        TackEngine.currentWindow.VSync = OpenTK.VSyncMode.On;
                        TackConsole.EngineLog(EngineLogType.Message, "Set renderer.v-sync to value: On");
                    } else {
                        TackEngine.currentWindow.VSync = OpenTK.VSyncMode.Off;
                        TackConsole.EngineLog(EngineLogType.Message, "Set renderer-v-sync to value: Off");
                    }
                } else {
                    TackConsole.EngineLog(EngineLogType.Error, "Couldn't convert '{0}' to type: bool", args[1]);
                }

                return;
            }

            TackConsole.EngineLog(EngineLogType.Error, "Incorrect number of args for command: " + args[0]);
        }

        [CommandMethod("renderer.backgroundColour", "", "red:byte green:byte blue:byte")]
        public static void ChangeBackgroundColour(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(EngineLogType.Message, "Value: " + TackEngineLib.Renderer.TackRenderer.BackgroundColour.ToString());
                return;
            }

            if (args.Length == 4) {
                if (byte.TryParse(args[1], out byte r)) {
                    if (byte.TryParse(args[2], out byte g)) {
                        if (byte.TryParse(args[3], out byte b)) {
                            TackEngineLib.Renderer.TackRenderer.BackgroundColour = new Colour4b(r, g, b);
                            TackConsole.EngineLog(EngineLogType.Message, "Set tackrenderer.backgroundColour to value: " + Renderer.TackRenderer.BackgroundColour.ToString());
                        } else {
                            TackConsole.EngineLog(EngineLogType.Error, "Failed to convert '{0}' to type: byte", args[3]);
                            return;
                        }
                    } else {
                        TackConsole.EngineLog(EngineLogType.Error, "Failed to convert '{0}' to type: byte", args[2]);
                        return;
                    }
                } else {
                    TackConsole.EngineLog(EngineLogType.Error, "Failed to convert '{0}' to type: byte", args[1]);
                    return;
                }
                return;
            }

            TackConsole.EngineLog(EngineLogType.Error, "Incorrect number of arguments for command: " + args[0]);
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

        [CommandMethod("tackobject.info", "name:string", "hash:string")]
        public static void TackObjectGetInfo(string[] args) {
            if (args.Length == 2) {
                // Try and get the TackObject by treating args[1] has a name
                TackObject calledObject = TackObject.Get(args[1]);

                // If there is no TackObject with name that is equal to args[1]
                //  - Treat args[1] as a hash and look for TackObject based on hash
                if (calledObject == null) {
                    calledObject = TackObject.GetUsingHash(args[1]);
                }

                // If there is no TackObject with name OR hash equal to args[1], return
                if (calledObject == null) {
                    TackConsole.EngineLog(EngineLogType.Error, "There is no TackObject that has name/hash with value: " + args[1]);
                    return;
                }

                TackConsole.EngineLog(EngineLogType.Message, "TackObject Info");
                TackConsole.EngineLog(EngineLogType.Message, "--------------------------------------");
                TackConsole.EngineLog(EngineLogType.Message, "Name: {0:-20}", calledObject.Name);
                TackConsole.EngineLog(EngineLogType.Message, "Hash: {0:-20}", calledObject.GetHash());
                TackConsole.EngineLog(EngineLogType.Message, "Position: {0:-20}", calledObject.Position.ToString());
                TackConsole.EngineLog(EngineLogType.Message, "Scale: {0:-20}", calledObject.Scale.ToString());
                TackConsole.EngineLog(EngineLogType.Message, "Rotation: {0:-20}", calledObject.Rotation);
                TackConsole.EngineLog(EngineLogType.Message, "Components ({0}):", calledObject.GetComponents().Length);

                TackComponent[] components = calledObject.GetComponents();

                foreach (TackComponent comp in components) {
                    TackConsole.EngineLog(EngineLogType.Message, "          - {0}", comp.GetType().Name);
                }

                return;
            }

            TackConsole.EngineLog(EngineLogType.Error, "Incorrect number of arguments for command: " + args[0]);
        }

        [CommandMethod("physicscomponent.setVariable", "tackobjecthash:string variablename:string newvalue:object")]
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

        [CommandMethod("physicscomponent.getVariable", "tackobjecthash:string variablename:string")]
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

        [CommandMethod("audiomanager.masterVolume", "", "newValue:float")]
        public static void ChangeAudioMasterVolume(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(EngineLogType.Message, "Value: " + Audio.AudioManager.MasterVolume.ToString("0.000"));
                return;
            }

            if (args.Length == 2) {
                if (float.TryParse(args[1], out float res)) {
                    Audio.AudioManager.MasterVolume = res;
                    TackConsole.EngineLog(EngineLogType.Message, "Changed audiomanager.mastervolume to value: " + Audio.AudioManager.MasterVolume);
                    return;
                } else {
                    TackConsole.EngineLog(EngineLogType.Error, "Couldn't convert '{0}' to float", args[1]);
                    return;
                }
            }

            TackConsole.EngineLog(EngineLogType.Error, "Incorrect number of args for command: " + args[1]);
        }
    }
}
