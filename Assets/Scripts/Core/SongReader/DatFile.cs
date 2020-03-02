using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DatFile
{
    public static Map LoadChart(string file)
    {
        Map newChartFile = new Map();
        
        string filePath = Path.GetDirectoryName(file);
        
        var info = File.ReadAllText(file);
        var fileData = JsonConvert.DeserializeObject<Map>(info);

        newChartFile = fileData;
        newChartFile.path = filePath;

        foreach (DifficultyBeatmapSets _beatmapSet in fileData._difficultyBeatmapSets)
        {
            foreach (DifficultyBeatmap _difficultyBeatmap in _beatmapSet._difficultyBeatmaps)
            {
                var diffDataInfo = File.ReadAllText(filePath + "/" + _difficultyBeatmap._beatmapFilename);
                var returnedDiffData = JsonConvert.DeserializeObject<DatLevelData>(diffDataInfo);

                Comparison<NoteData> NoteCompare = (x, y) => x._time.CompareTo(y._time);
                returnedDiffData._notes.Sort(NoteCompare);

                Comparison<ObstacleData> ObsticaleCompare = (x, y) => x._time.CompareTo(y._time);
                returnedDiffData._obstacles.Sort(ObsticaleCompare);

                Comparison<EventData> EventCompare = (x, y) => x.Time.CompareTo(y.Time);
                returnedDiffData._events.Sort(EventCompare);
                _difficultyBeatmap.level = returnedDiffData;
                newChartFile._levelInfo.Add(returnedDiffData);
            }
        }
        
        return newChartFile;
    }
}

public class DatLevelData
{
    public string _version { get; set; }
    public List<EventData> _events { get; set; }
    public List<NoteData> _notes { get; set; }
    public List<ObstacleData> _obstacles { get; set; }
}

public class DifficultyBeatmap
{
    public string _difficulty { get; set; }
    public int _difficultyRank { get; set; }
    public string _beatmapFilename { get; set; }
    public float _noteJumpMovementSpeed { get; set; }
    public float _noteJumpStartBeatOffset { get; set; }
    public CustomData _customData { get; set; }
    public DatLevelData level { get; set; }
}

public class DifficultyBeatmapSets
{
    public string _beatmapCharacteristicName { get; set; }
    public List<DifficultyBeatmap> _difficultyBeatmaps { get; set; }
}

public class ColorMap
{
    [JsonProperty("b")]
    public double B { get; set; }

    [JsonProperty("g")]
    public double G { get; set; }

    [JsonProperty("r")]
    public double R { get; set; }
}

public class CustomData
{
    [JsonProperty("_colorLeft")]
    public ColorMap ColorLeft { get; set; }

    [JsonProperty("_colorRight")]
    public ColorMap ColorRight { get; set; }

    [JsonProperty("_difficultyLevel")]
    public string DifficultyLevel { get; set; }

    [JsonProperty("_editorOffset")]
    public long EditorOffset { get; set; }

    [JsonProperty("_editorOldOffset")]
    public long EditorOldOffset { get; set; }

    [JsonProperty("_information")]
    public IEnumerable<string> Information { get; set; }

    [JsonProperty("_requirements")]
    public IEnumerable<string> Requirements { get; set; }

    [JsonProperty("_suggestions")]
    public IEnumerable<string> Suggestions { get; set; }

    [JsonProperty("_warnings")]
    public IEnumerable<string> Warnings { get; set; }
}

public class Map
{
    public string path;

    public Sprite icon;
    public AudioClip _clip;

    public List<DatLevelData> _levelInfo = new List<DatLevelData>();
    
    private DifficultyBeatmap _targetedDifficulty;
    public DifficultyBeatmap TargetDifficulty
    {
        get
        {
            return _targetedDifficulty;
        }
        set
        {
            _targetedDifficulty = value;
        }
    }

    private DifficultyBeatmapSets _targetedBeatMapSet;
    public DifficultyBeatmapSets TargetBeatMapSet
    {
        get
        {
            return _targetedBeatMapSet;
        }
        set
        {
            _targetedBeatMapSet = value;
        }
    }

    [JsonProperty("_version")]
    public string _version { get; set; }
    [JsonProperty("_songName")]
    public string _songName { get; set; }
    [JsonProperty("_songSubName")]
    public string _songSubName { get; set; }
    [JsonProperty("_songAuthorName")]
    public string _songAuthorName { get; set; }
    [JsonProperty("_levelAuthorName")]
    public string _levelAuthorName { get; set; }
    [JsonProperty("_beatsPerMinute")]
    public float _beatsPerMinute { get; set; }
    [JsonProperty("_songTimeOffset")]
    public float _songTimeOffset { get; set; }
    [JsonProperty("_shuffle")]
    public float _shuffle { get; set; }
    [JsonProperty("_shufflePeriod")]
    public float _shufflePeriod { get; set; }
    [JsonProperty("_previewStartTime")]
    public float _previewStartTime { get; set; }
    [JsonProperty("_previewDuration")]
    public float _previewDuration { get; set; }
    [JsonProperty("_songFilename")]
    public string _songFilename { get; set; }
    [JsonProperty("_coverImageFilename")]
    public string _coverImageFilename { get; set; }
    [JsonProperty("_environmentName")]
    public string _environmentName { get; set; }
    
    [JsonProperty("_difficultyBeatmapSets")]
    public List<DifficultyBeatmapSets> _difficultyBeatmapSets { get; set; }
}