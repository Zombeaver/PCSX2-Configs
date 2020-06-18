#r "nuget: Newtonsoft.Json, 12.0.3"

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

public static async Task CommitFile(string repo, string branch, string file)
{
    var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    var client = new HttpClient();
    client.BaseAddress = new Uri($"https://api.github.com/repos/{repo}/git/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", githubToken);
    Func<object, StringContent> object_tojson = (obj) => new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

    var headRefContents = await client.GetStringAsync($"ref/heads/{branch}");
    var headRefObject = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(headRefContents)).@object;

    var headCommitContents = await client.GetStringAsync(headRefObject.url);
    var headCommit = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(headCommitContents));

    var commitTreeContents = await client.GetStringAsync(headCommit.tree.url);
    var commitTree = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(commitTreeContents));

    var blobResponse = await client.PostAsync("blobs", object_tojson(new { content = file, encoding = "utf-8" }));
    var blobContents = await blobResponse.Content.ReadAsStringAsync();
    var blob = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(blobContents));

    var treeResponse = await client.PostAsync("trees", object_tojson(new
    {
        base_tree = commitTree.sha,
        tree = new object[]
        {
            new
            {
                path = "RemoteIndex.xml",
                mode = "100644",
                type = "blob",
                sha = blob.sha
            }
        }
    }));
    var treeContents = await treeResponse.Content.ReadAsStringAsync();
    var tree = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(treeContents));

    var commitResponse = await client.PostAsync("commits", object_tojson(new 
    { 
        message = "Update Remote Index",
        parents = new string[] { headCommit.sha },
        tree = tree.sha
    }));
    var commitContents = await commitResponse.Content.ReadAsStringAsync();
    var commit = ((dynamic)JsonConvert.DeserializeObject<ExpandoObject>(commitContents));

    var updateHeadResponse = await client.PatchAsync($"refs/heads/{branch}", object_tojson(new { sha = commit.sha }));
    updateHeadResponse.EnsureSuccessStatusCode();
}