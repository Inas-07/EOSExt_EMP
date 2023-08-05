﻿using GameData;
using System.Collections.Generic;
using ExtraObjectiveSetup.BaseClasses;
using System.Text.Json.Serialization;
using LevelGeneration;

namespace ExtraObjectiveSetup.Expedition.IndividualGeneratorGroup
{
    public class ExpeditionIGGroup
    {
        public List<BaseInstanceDefinition> Generators { get; set; } = new();

        [JsonIgnore]
        public List<LG_PowerGenerator_Core> GeneratorInstances { get; set; } = new();

        public bool PlayEndSequenceOnGroupComplete { get; set; } = false;

        public List<List<WardenObjectiveEventData>> EventsOnInsertCell { get; set; } = new() { new() };
    }
}
