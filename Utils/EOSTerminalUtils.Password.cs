﻿using LevelGeneration;
using GameData;
using Localization;
using System.Collections.Generic;
using ExtraObjectiveSetup.BaseClasses.CustomTerminalDefinition;

namespace ExtraObjectiveSetup.Utils
{
    public static partial class EOSTerminalUtils
    {
        public static LG_ComputerTerminal SelectPasswordTerminal(eDimensionIndex dimensionIndex, LG_LayerType layerType, eLocalZoneIndex localIndex, eSeedType seedType, int staticSeed = 1)
        {
            if (seedType == eSeedType.None)
            {
                EOSLogger.Error($"SelectTerminal: unsupported seed type {seedType}");
                return null;
            }

            if (!Builder.CurrentFloor.TryGetZoneByLocalIndex(dimensionIndex, layerType, localIndex, out var zone) || zone == null)
            {
                EOSLogger.Error($"SelectTerminal: Could NOT find zone {(dimensionIndex, layerType, localIndex)}");
                return null;
            }

            if (zone.TerminalsSpawnedInZone.Count <= 0)
            {
                EOSLogger.Error($"SelectTerminal: Could not find any terminals in zone {(dimensionIndex, layerType, localIndex)}");
                return null;
            }

            List<LG_ComputerTerminal> candidateTerminals = new();
            foreach (var terminal in zone.TerminalsSpawnedInZone)
            {
                if (!terminal.HasPasswordPart)
                {
                    candidateTerminals.Add(terminal);
                }
            }

            if (candidateTerminals.Count <= 0)
            {
                EOSLogger.Error($"SelectTerminal: Could not find any terminals without a password part in zone {(dimensionIndex, layerType, localIndex)}, putting the password on random (session) already used terminal.");
                return zone.TerminalsSpawnedInZone[Builder.SessionSeedRandom.Range(0, zone.TerminalsSpawnedInZone.Count, "NO_TAG")];
            }

            switch (seedType)
            {
                case eSeedType.SessionSeed:
                    return candidateTerminals[Builder.SessionSeedRandom.Range(0, candidateTerminals.Count, "NO_TAG")];
                case eSeedType.BuildSeed:
                    return candidateTerminals[Builder.BuildSeedRandom.Range(0, candidateTerminals.Count, "NO_TAG")];
                case eSeedType.StaticSeed:
                    UnityEngine.Random.InitState(staticSeed);
                    return candidateTerminals[UnityEngine.Random.Range(0, candidateTerminals.Count)];
                default:
                    EOSLogger.Error("SelectTerminal: did not have a valid SeedType!!");
                    return null;
            }
        }

        public static void BuildPassword(LG_ComputerTerminal terminal, TerminalPasswordData data)
        {
            if (!data.PasswordProtected) return;

            if(terminal.IsPasswordProtected)
            {
                EOSLogger.Error($"EOSTerminalUtils.BuildPassword: {terminal.PublicName} is already password-protected!");
                return;
            }

            if (!data.GeneratePassword)
            {
                terminal.LockWithPassword(data.Password, data.PasswordHintText);
                return;
            }

            if (data.TerminalZoneSelectionDatas.Count <= 0)
            {
                EOSLogger.Error($"Tried to generate a password for terminal {terminal.PublicName} with no {typeof(TerminalZoneSelectionData).Name}!! This is not allowed.");
                return;
            }
            string codeWord = SerialGenerator.GetCodeWord();
            string hintText = data.PasswordHintText;
            string logPositionText = "<b>[Forgot your password?]</b> Backup security key(s) located in logs on ";
            int passwordPartCount = data.PasswordPartCount;

            if (codeWord.Length % passwordPartCount != 0)
            {
                EOSLogger.Error($"Build() password.Length ({codeWord.Length}) not divisible by passwordParts ({passwordPartCount}). Defaulting to 1.");
                passwordPartCount = 1;
            }

            string[] strArray;
            if (passwordPartCount <= 1)
            {
                strArray = new string[1] { codeWord };
            }
            else
            {
                strArray = codeWord.SplitIntoChunksArray(codeWord.Length / passwordPartCount);
            }

            string str3 = "";
            if (data.ShowPasswordPartPositions)
            {
                for (int i = 0; i < strArray[0].Length; ++i)
                    str3 += "-";
            }

            HashSet<LG_ComputerTerminal> computerTerminalSet = new HashSet<LG_ComputerTerminal>();
            for (int i = 0; i < passwordPartCount; ++i)
            {
                int j = i % data.TerminalZoneSelectionDatas.Count;
                var selectedRange = data.TerminalZoneSelectionDatas[j];
                int selectedIdx = Builder.SessionSeedRandom.Range(0, selectedRange.Count, "NO_TAG");
                var selectedData = selectedRange[selectedIdx];
                LG_ComputerTerminal passwordTerminal;

                if (selectedData.SeedType == eSeedType.None)
                {
                    if (!Builder.CurrentFloor.TryGetZoneByLocalIndex(selectedData.DimensionIndex, selectedData.LayerType, selectedData.LocalIndex, out var passwordZone) || passwordZone == null)
                    {
                        EOSLogger.Error($"BuildPassword: seedType {eSeedType.None} specified but cannot find zone {selectedData.GlobalZoneIndexTuple()}");
                        //continue; // fallback to critical error output
                    }

                    if (passwordZone.TerminalsSpawnedInZone.Count <= 0)
                    {
                        EOSLogger.Error($"BuildPassword: seedType {eSeedType.None} specified but cannot find terminal zone {selectedData.GlobalZoneIndexTuple()}");
                        //continue; // fallback to critical error output
                    }

                    passwordTerminal = passwordZone.TerminalsSpawnedInZone[selectedData.TerminalIndex];
                }
                else
                {
                    passwordTerminal = SelectPasswordTerminal(selectedData.DimensionIndex, selectedData.LayerType, selectedData.LocalIndex, selectedData.SeedType);
                }

                if (passwordTerminal == null)
                {
                    EOSLogger.Error($"BuildPassword: CRITICAL ERROR, could not get a LG_ComputerTerminal for password part ({i + 1}/{passwordPartCount}) for {terminal.PublicName} backup log.");
                    continue;
                }

                string str4 = "";
                string str5;
                if (data.ShowPasswordPartPositions)
                {
                    for (int index3 = 0; index3 < i; ++index3)
                        str4 += str3;
                    str5 = str4 + strArray[i];
                    for (int index4 = i; index4 < passwordPartCount - 1; ++index4)
                        str5 += str3;
                }
                else
                {
                    str5 = strArray[i];
                }

                string str6 = data.ShowPasswordPartPositions ? $"0{i + 1}" : $"0{Builder.SessionSeedRandom.Range(0, 9, "NO_TAG")}";
                TerminalLogFileData passwordLog = new TerminalLogFileData()
                {
                    FileName = $"key{str6}_{LG_TerminalPasswordLinkerJob.GetTerminalNumber(terminal)}{(passwordTerminal.HasPasswordPart ? "_1" : "")}.LOG",
                    FileContent = new LocalizedText()
                    {
                        UntranslatedText = string.Format(Text.Get(passwordPartCount > 1 ? 1431221909 : 2260297836), str5),
                        Id = 0u
                    }
                };
                passwordTerminal.AddLocalLog(passwordLog);
                if (!computerTerminalSet.Contains(passwordTerminal))
                {
                    if (i > 0) logPositionText += ", ";
                    logPositionText = logPositionText + passwordTerminal.PublicName + " in " + passwordTerminal.SpawnNode.m_zone.AliasName;
                }
                computerTerminalSet.Add(passwordTerminal);
                passwordTerminal.HasPasswordPart = true;
            }

            string str7 = logPositionText + ".";
            if (data.ShowPasswordLength)
            {
                terminal.LockWithPassword(codeWord, hintText, str7, "Char[" + codeWord.Length + "]");
            }
            else
            {
                terminal.LockWithPassword(codeWord, hintText, str7);
            }
        }
    }
}
