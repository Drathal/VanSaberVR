using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class JsonMapFile
{
    public List<DifficultyLevel> _difficultyInfo = new List<DifficultyLevel>();
    
    public static Map LoadChart(string file)
    {
        JsonMapFile newChartFile = new JsonMapFile();
        List<DifficultyBeatmapSets> _sets = new List<DifficultyBeatmapSets>();
        List<DifficultyBeatmap> _difficultys = new List<DifficultyBeatmap>();
        List<DatLevelData> _datLevel = new List<DatLevelData>();

        string filePath = Path.GetDirectoryName(file);
        var info = File.ReadAllText(file);
        var fileData = JsonConvert.DeserializeObject<Chart>(info);
        
        foreach (DifficultyData diffData in fileData.difficultyLevels)
        {
            var diffDataInfo = File.ReadAllText(filePath + "/" + diffData.jsonPath);

            var handleNegativeValues = new JsonSerializerSettings
            {
                FloatParseHandling = FloatParseHandling.Decimal
            };

            var returnedDiffData = JsonConvert.DeserializeObject<DifficultyLevel>(diffDataInfo, handleNegativeValues);

            Comparison<NoteData> NoteCompare = (x, y) => x._time.CompareTo(y._time);
            returnedDiffData._notes.Sort(NoteCompare);

            Comparison<ObstacleData> ObsticaleCompare = (x, y) => x._time.CompareTo(y._time);
            returnedDiffData._obstacles.Sort(ObsticaleCompare);

            Comparison<EventData> EventCompare = (x, y) => x.Time.CompareTo(y.Time);
            returnedDiffData._events.Sort(EventCompare);

            DatLevelData _level = new DatLevelData()
            {
                _version = "2.0.0",
                _notes = returnedDiffData._notes,
                _events = returnedDiffData._events,
                _obstacles = returnedDiffData._obstacles
            };

            _datLevel.Add(_level);
            
            newChartFile._difficultyInfo.Add(returnedDiffData);

        }

        for(int i = 0; i < newChartFile._difficultyInfo.Count; i++)
        {
            DifficultyBeatmap _difficulty = new DifficultyBeatmap()
            {
                _difficulty = fileData.difficultyLevels[i].difficulty,
                _beatmapFilename = fileData.difficultyLevels[i].jsonPath,
                _difficultyRank = fileData.difficultyLevels[i].difficultyRank,
                _noteJumpMovementSpeed = newChartFile._difficultyInfo[i]._noteJumpSpeed,
                _noteJumpStartBeatOffset = (int)newChartFile._difficultyInfo[i]._noteJumpStartBeatOffset,
                level = _datLevel[i]
            };
            _difficultys.Add(_difficulty);
        }

        for (int i = 0; i < newChartFile._difficultyInfo.Count; i++)
        {
            string[] _split = fileData.difficultyLevels[i].jsonPath.Split('.');

            DifficultyBeatmapSets _beatmap = new DifficultyBeatmapSets()
            {
                _beatmapCharacteristicName = GetBeatmapCharacteristicName(_split[0]),
                _difficultyBeatmaps = _difficultys
            };

            DifficultyBeatmapSets result = _sets.Find(x => x._beatmapCharacteristicName == _beatmap._beatmapCharacteristicName);

            if (result != null)
            {
                continue;
            }

            _sets.Add(_beatmap);
        }


        Map _song = new Map
        {
            path = file,
            _songAuthorName = fileData.authorName,
            _songName = fileData.songName,
            _beatsPerMinute = fileData.beatsPerMinute,
            _environmentName = fileData.environmentName,
            _coverImageFilename = fileData.coverImagePath,
            _previewDuration = fileData.previewDuration,
            _previewStartTime = fileData.previewStartTime,
            _songFilename = fileData.difficultyLevels[0].audioPath,
            _songSubName = fileData.songSubName,
            _songTimeOffset = fileData.difficultyLevels[0].offset,
            _levelAuthorName = "BeatSaberClone",
            _shuffle = newChartFile._difficultyInfo[0]._shuffle,
            _shufflePeriod = newChartFile._difficultyInfo[0]._shufflePeriod,
            _version = newChartFile._difficultyInfo[0]._version,
            _difficultyBeatmapSets = _sets,
            _levelInfo = _datLevel
        };

        return _song;
    }

    public static string GetBeatmapCharacteristicName(string name)
    {
        string _characteristic = "Standard";

        if (name.Contains("OneSaber"))
        {
            _characteristic = "OneSaber";
        }

        if (name.Contains("NoArrows"))
        {
            _characteristic = "NoArrows";
        }

        return _characteristic;
    }
}

public class DifficultyLevel
{
    public string _version { get; set; }
    public long _beatsPerMinute { get; set; }
    public long _beatsPerBar { get; set; }
    public float _noteJumpSpeed { get; set; }
    public float _shuffle { get; set; }
    public float _shufflePeriod { get; set; }
    public float _time { get; set; }
    public float _noteJumpStartBeatOffset { get; set; }
    public List<uint> _BPMChanges { get; set; }
    public List<EventData> _events { get; set; }
    public List<NoteData> _notes { get; set; }
    public List<ObstacleData> _obstacles { get; set; }
}

public class DifficultyData
{
    public string difficulty { get; set; }
    public int difficultyRank { get; set; }
    public string audioPath { get; set; }
    public string jsonPath { get; set; }
    public float offset { get; set; }
    public float oldOffset { get; set; }
}

public class EventData
{
    [JsonProperty("_time")]
    public float Time { get; set; }

    [JsonProperty("_type")]
    public int Type { get; set; }

    [JsonProperty("_value")]
    public int Value { get; set; }
}

public class NoteData
{
    public float _time { get; set; }
    public float _lineIndex { get; set; }
    public float _lineLayer { get; set; }
    public NoteType _type { get; set; }
    public CutDirection _cutDirection { get; set; }
}

public class ObstacleData
{
    public float _time { get; set; }
    public float _lineIndex { get; set; }
    public float _type { get; set; }
    public float _duration { get; set; }
    public float _width { get; set; }
}

public class Chart
{
    public string songName { get; set; }
    public string songSubName { get; set; }
    public string authorName { get; set; }
    public long beatsPerMinute { get; set; }
    public long previewStartTime { get; set; }
    public long previewDuration { get; set; }
    public string coverImagePath { get; set; }
    public string environmentName { get; set; }
    public List<DifficultyData> difficultyLevels { get; set; }
}