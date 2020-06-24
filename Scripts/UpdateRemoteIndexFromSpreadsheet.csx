#r "nuget: Newtonsoft.Json, 12.0.3"
#r "nuget: ini-parser-netstandard, 2.5.2"

using System;
using System.Xml;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using IniParser.Model;
using IniParser.Parser;

static string GetScriptFolder([CallerFilePath] string path = null) => Path.GetDirectoryName(path);
var googleApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
var spreadsheetRequestUri = $"https://sheets.googleapis.com/v4/spreadsheets/1sC19l0htx0Qu2QE1CFta5R35iqPBN332dCHfTg1BGlQ/values/PS2%20Configs?key={googleApiKey}";
var remoteIndexRequestUri = "https://raw.githubusercontent.com/Zombeaver/PCSX2-Configs/master/RemoteIndex.xml";
var pcsx2DbRequestUri = "https://raw.githubusercontent.com/PCSX2/pcsx2/master/bin/GameIndex.dbf";
var client = new HttpClient();

var spreadsheetContents = await client.GetStringAsync(spreadsheetRequestUri);
var remoteIndexContents = await client.GetStringAsync(remoteIndexRequestUri);
var pcsx2DbContents = await client.GetStringAsync(pcsx2DbRequestUri);

IEnumerable<dynamic> spreadsheetValues = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(spreadsheetContents)).values;
spreadsheetValues = spreadsheetValues.Skip(5).Where(val => val.Count > 0 && !new string[] {val[0], val[1], val[3]}.Any(x => string.IsNullOrWhiteSpace(x)));

XmlDocument remoteIndexXml = new XmlDocument();
remoteIndexXml.LoadXml(remoteIndexContents);

static var IniDataParser = new IniDataParser();
var pcsx2DbString = pcsx2DbContents.Substring(pcsx2DbContents.IndexOf("-- Game List") + "-- Game List".Length).Split("---------------------------------------------");
IEnumerable<IniData> pcsx2DbEntries = pcsx2DbString.Skip(1).Select(entry => Regex.Replace(entry, @"\[patches.*?\[\/patches]", "", RegexOptions.Singleline)).Select(entry => Regex.Replace(entry, @"\/\/.*", ""))
    .Select(entry => IniDataParser.Parse(entry)).Where(entry => entry.Global.Count != 0);

Func<string, string> xml_tolower = (expr) => $"translate({expr}, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')";
foreach(var value in spreadsheetValues) // Iterate through all the entries in the spreadsheet
{
    // Find or Add the Config node of interest (in our RemoteIndex.xml) that matches up with the spreadsheet value
    var configNode = FindOrAddConfigNode(configName: value[1]);

    // If Has Games Id's Already then skip
    // Otherwise add game id's from PCSX2 DB
    if(configNode.SelectSingleNode("GameIds") == null)
    {
        var gameIds = LookupGameIds(gameName: value[3], configName: value[1]);
        AddOrUpdateGameIds(configNode, gameIds);
    }
    
    // Add (or update) Status and Notes values from the spreadsheet
    AddOrUpdateStatusNode(configNode, status: value[0]); 
    AddOrUpdateNotesNode(configNode, notes: value[20]);
}

// Save the updated file locally, overwriting the original
remoteIndexXml.Save($"{Directory.GetParent(GetScriptFolder())}/RemoteIndex.xml");

XmlNode FindOrAddConfigNode(string configName)
{
    // Look for matching Config node in RemoteIndex
    var configNode = remoteIndexXml.SelectSingleNode($"//Config[{xml_tolower("@Name")}=\"{configName.ToLowerInvariant()}\"]");
    if(configNode == null) // Not listed in RemoteIndex so add it
    {
        configNode = remoteIndexXml.DocumentElement.AppendChild(remoteIndexXml.CreateElement("Config"));
        ((XmlElement)configNode).SetAttribute("Name", $"{configName}");
    }
    return configNode;
}

string[] LookupGameIds(string gameName, string configName)
{
    Func<string, string> name_transform = (name) => {
        name = Regex.Replace(name, @"\[.*?\]", "");
        name = Regex.Replace(name, @"Special Edition", "", RegexOptions.IgnoreCase);
        name = Regex.Replace(name, @"- part \d - ", "", RegexOptions.IgnoreCase);
        name = Regex.Replace(name, @", the\b", "", RegexOptions.IgnoreCase);
        name = Regex.Replace(name, @"\bthe\b", "", RegexOptions.IgnoreCase);
        name = Regex.Replace(name, @"\b04\b", "4");
        name = Regex.Replace(name, @"X\b", "10");
        name = Regex.Replace(name, @"V\b", "5");
        name = Regex.Replace(name, @"IV\b", "4");
        name = Regex.Replace(name, @"III\b", "3");
        name = Regex.Replace(name, @"II\b", "2");
         
        return name.Replace(":", "").Replace(" - ", " ").ToLowerInvariant().Trim().Replace("  ", " ");
    };

    Func<string, string> strip_numbers = (name) => Regex.Replace(name, @"\b\d", "").Trim().Replace("-", " ").Replace("  ", " ");
    Func<string, bool> has_subtitle = (name) => name.Contains(":") || name.Contains("-") || name.Contains("//");
    Func<string, string> get_subtitle = (name) => name.Split(new string[] {":","-","//"}, StringSplitOptions.None).Last().ToLowerInvariant().Trim();
    Func<string, int?> has_number = (name) => int.TryParse(Regex.Match(name, @"\d", RegexOptions.RightToLeft).Value, out var i) ? (int?) i : null;

    var gameHasSubtitle = has_subtitle(gameName);
    var gameSubtitle = gameHasSubtitle ? get_subtitle(gameName) : null;
    configName = configName.Substring(0, configName.IndexOf("id#")).Trim();
    gameName = name_transform(gameName);
    configName = name_transform(configName);
    
    var gameIds = pcsx2DbEntries.Where(entry => {
        var name = name_transform(entry.Global["Name"]);
        var isMatch = Regex.IsMatch(name, $@"\b{gameName}\b") || Regex.IsMatch(name, $@"\b{configName}\b");
        isMatch = name.Any(char.IsDigit) && !gameName.Any(char.IsDigit) && !configName.Any(char.IsDigit) ? false : isMatch;
        isMatch = gameName.Split(" ").Where(x => !x.Any(char.IsDigit)).Count() == 1 ? name == gameName || name == configName : isMatch;
        isMatch = !gameHasSubtitle && has_subtitle(entry.Global["Name"]) ? false : isMatch;
        isMatch = !isMatch ? gameName.Replace(" ", "") == name.Replace(" ", "") : isMatch;
        isMatch = isMatch && gameHasSubtitle ? (gameSubtitle == get_subtitle(entry.Global["Name"])) : isMatch;
        return isMatch;
    });

    if(!gameIds.Any(id => id.Global["Serial"].Contains("U")))
    {
        var fullGameName = gameName;
        gameName = strip_numbers(fullGameName);
        gameIds = pcsx2DbEntries.Where(entry => {
            var fullName = name_transform(entry.Global["Name"]);
            var name = strip_numbers(fullName);
            var isMatch = Regex.IsMatch(gameName, $@"\b{name}\b") || Regex.IsMatch(name, $@"\b{gameName}\b");
            isMatch = !isMatch ? gameName.Split().All(name.Contains) : isMatch;
            isMatch = isMatch ? has_number(fullGameName) == has_number(fullName) || has_subtitle(entry.Global["Name"]) : isMatch;
            isMatch = has_number(fullName) != null && has_number(fullGameName) != null && has_number(fullGameName) != has_number(fullName) ? false : isMatch;
            return isMatch;
        });
    }

    return gameIds.Select(id => $"{id.Global["Name"]} | {id.Global["Serial"]}").ToArray();
}

void AddOrUpdateGameIds(XmlNode configNode, string[] gameIds)
{
    if(gameIds.Count() == 0) return;
    var gameIdsNode = configNode.SelectSingleNode("GameIds");
    if(gameIdsNode == null)
    {
        gameIdsNode = remoteIndexXml.CreateElement("GameIds");
        configNode.AppendChild(gameIdsNode);
    }

    gameIdsNode.RemoveAll();
    foreach(var gameId in gameIds)
    {
        var gameIdNode = remoteIndexXml.CreateElement("GameId");
        gameIdNode.InnerText = gameId;
        gameIdsNode.AppendChild(gameIdNode);
    }
}

void AddOrUpdateStatusNode(XmlNode configNode, string status)
{
    // Check for exististance of Status node
    var statusNode = configNode.SelectSingleNode("Status");
    if(statusNode == null) // If it doesn't exist add it
    {
        statusNode = remoteIndexXml.CreateElement("Status");
        statusNode.InnerText = status;
        configNode.AppendChild(statusNode);
    } // If it does exist but the value is different then update it
    else if(statusNode.InnerText != status) statusNode.InnerText = status;
}

void AddOrUpdateNotesNode(XmlNode configNode, string notes)
{
    // Check if we have notes to add
    if(string.IsNullOrWhiteSpace(notes)) return;

    // If we do, check for pre-existing Notes node
    var notesNode = configNode.SelectSingleNode("Notes");
    if(notesNode == null) // If we don't have on add it
    {
        notesNode = remoteIndexXml.CreateElement("Notes");
        notesNode.InnerText = notes;
        configNode.AppendChild(notesNode);
    } // Otherwise update it if the value is different
    else if(notesNode.InnerText != notes) notesNode.InnerText = notes;
}