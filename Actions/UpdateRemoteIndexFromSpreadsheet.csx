#r "nuget: Newtonsoft.Json, 12.0.3"

using System;
using System.Xml;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

var googleApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
var spreadsheetRequestUri = $"https://sheets.googleapis.com/v4/spreadsheets/1sC19l0htx0Qu2QE1CFta5R35iqPBN332dCHfTg1BGlQ/values/PS2%20Configs?key={googleApiKey}";
var remoteIndexRequestUri = "https://raw.githubusercontent.com/Zombeaver/PCSX2-Configs/master/RemoteIndex.xml";
var client = new HttpClient();

var spreadsheetContents = await client.GetStringAsync(spreadsheetRequestUri);
var remoteIndexContents = await client.GetStringAsync(remoteIndexRequestUri);

IEnumerable<dynamic> spreadsheetValues = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(spreadsheetContents)).values;
spreadsheetValues = spreadsheetValues.Skip(5).Where(val => val.Count > 0 && !new string[] {val[0], val[1], val[3]}.Any(x => string.IsNullOrWhiteSpace(x)));

XmlDocument remoteIndexXml = new XmlDocument();
remoteIndexXml.LoadXml(remoteIndexContents);

Func<string, string> xml_tolower = (expr) => $"translate({expr}, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')";
foreach(var value in spreadsheetValues)
{
    var configName = (string)value[1];
    var configNode = remoteIndexXml.SelectSingleNode($"//Config[{xml_tolower("@Name")}=\"{configName.ToLowerInvariant()}\"]");
    if(configNode == null) // Not listed in RemoteIndex
    {
        configNode = remoteIndexXml.DocumentElement.AppendChild(remoteIndexXml.CreateElement("Config"));
        ((XmlElement)configNode).SetAttribute("Name", $"{configName}");
    }
    
    if(configNode.ChildNodes.Cast<XmlNode>().Any(x => x.Name == "Status")) continue;
    var statusNode = remoteIndexXml.CreateElement("Status");
    statusNode.InnerText = value[0];
    configNode.AppendChild(statusNode);

    if(configNode.ChildNodes.Cast<XmlNode>().Any(x => x.Name == "Notes") || string.IsNullOrWhiteSpace(value[19])) continue;
    var notesNode = remoteIndexXml.CreateElement("Notes");
    notesNode.InnerText = value[19];
    configNode.AppendChild(notesNode);
}

remoteIndexXml.Save("RemoteIndex.xml");