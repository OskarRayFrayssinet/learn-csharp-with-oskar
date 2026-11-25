// =========================================================================
//  MIN DASHBOARD
//  Bygger och testar all din kod och skapar en sida som visar hur långt
//  du har kommit: min-dashboard/index.html. Sidan fungerar utan internet.
//
//  Starta:  dubbelklicka på dashboard.cmd i repots rotmapp
//     eller:  dotnet run --project verktyg/Dashboard
//
//  Val:
//    --utan-test     visa senaste resultatet utan att köra testerna igen
//    --inte-oppna    öppna inte webbläsaren
//    uppslagsbok     skapa bara uppslagsbok.html i repots rotmapp
// =========================================================================

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Dashboard;

Console.OutputEncoding = Encoding.UTF8;
var rot = HittaRepot();
var utmapp = Path.Combine(rot, "min-dashboard");
var mall = File.ReadAllText(Path.Combine(rot, "verktyg", "Dashboard", "mall.html"));
var uppslag = JsonDocument.Parse(File.ReadAllText(Path.Combine(rot, "verktyg", "uppslag.json"))).RootElement;

if (args.Contains("uppslagsbok"))
{
    var sida = SkapaSida(mall, new { uppslag, guideprefix = "", progression = (object?)null });
    File.WriteAllText(Path.Combine(rot, "uppslagsbok.html"), sida, new UTF8Encoding(false));
    Console.WriteLine("Skapade uppslagsbok.html");
    return 0;
}

Directory.CreateDirectory(utmapp);
var trxMapp = Valfri("--trx") ?? Path.Combine(utmapp, "testresultat");
var bygglogg = "";
var byggfel = new List<string>();
var byggt = true;

if (!args.Contains("--utan-test") && Valfri("--trx") == null)
{
    Console.WriteLine("Bygger all kod (det kan ta en minut)...");
    var (kod, utdata) = Kor(rot, "build", "learn-csharp-with-oskar.sln", "--no-incremental", "-nologo", "-v", "q");
    bygglogg = utdata;
    byggt = kod == 0;
    byggfel = utdata.Split('\n')
        .Where(r => r.Contains(": error "))
        .Select(r => r.Trim().Replace(rot + Path.DirectorySeparatorChar, ""))
        .Select(r => r.Contains(" [") ? r[..r.LastIndexOf(" [")] : r)
        .Distinct().Take(15).ToList();

    if (Directory.Exists(trxMapp))
    {
        Directory.Delete(trxMapp, recursive: true);
    }
    if (byggt)
    {
        Console.WriteLine("Kör testerna...");
        Kor(rot, "test", "learn-csharp-with-oskar.sln", "--no-build", "-nologo",
            "--logger", "trx", "--results-directory", trxMapp);
    }
    else
    {
        Console.WriteLine("Koden gick inte att bygga. Felen visas på dashboarden.");
    }
}
else if (Valfri("--bygglogg") is { } loggfil)
{
    bygglogg = File.ReadAllText(loggfil);
}

var tester = Testresultat.Las(trxMapp);
byggt = byggt && tester.Count > 0;
var ovningar = Testresultat.Gruppera(tester);
var tid = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
var historik = UppdateraHistorik(Path.Combine(utmapp, "historik.csv"), tid, tester, byggt);
var kodtips = Kodgranskning.Granska(rot, bygglogg);

var progression = new
{
    elev = Elevnamn(rot),
    tid,
    byggt,
    byggfel,
    godkanda = tester.Count(t => t.Ok),
    totalt = tester.Count,
    ovningar = ovningar.Select(o => new { nr = o.Nr, namn = o.Namn, godkanda = o.Godkanda, totalt = o.Totalt }),
    tester = tester.Select(t => new { namn = t.Namn, nr = t.Nr, ok = t.Ok, meddelande = t.Meddelande }),
    historik = historik.Select(h => new { tid = h.Tid, godkanda = h.Godkanda, totalt = h.Totalt }),
    kodtips = kodtips.Select(k => new { fil = k.Fil, rad = k.Rad, kod = k.Kod, problem = k.Problem, losning = k.Losning }),
};

var html = SkapaSida(mall, new { uppslag, guideprefix = "../", progression });
var indexfil = Path.Combine(utmapp, "index.html");
File.WriteAllText(indexfil, html, new UTF8Encoding(false));

Console.WriteLine();
Console.WriteLine(byggt
    ? $"Klart! {tester.Count(t => t.Ok)} av {tester.Count} tester är gröna. {kodtips.Count} kodtips."
    : "Klart, men koden gick inte att bygga. Se dashboarden.");
Console.WriteLine($"Dashboarden finns här: {indexfil}");

if (!args.Contains("--inte-oppna"))
{
    try
    {
        Process.Start(new ProcessStartInfo(indexfil) { UseShellExecute = true });
    }
    catch
    {
        Console.WriteLine("Öppna filen ovan i din webbläsare.");
    }
}
return 0;

// ---------------------------------------------------------------------------

string? Valfri(string namn)
{
    var i = Array.IndexOf(args, namn);
    return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
}

static string HittaRepot()
{
    var mapp = new DirectoryInfo(AppContext.BaseDirectory);
    while (mapp != null)
    {
        if (File.Exists(Path.Combine(mapp.FullName, "learn-csharp-with-oskar.sln")))
        {
            return mapp.FullName;
        }
        mapp = mapp.Parent;
    }
    throw new DirectoryNotFoundException("Hittade inte repots rotmapp (learn-csharp-with-oskar.sln).");
}

static (int Kod, string Utdata) Kor(string mapp, params string[] argument)
{
    var start = new ProcessStartInfo("dotnet")
    {
        WorkingDirectory = mapp,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
    };
    foreach (var a in argument)
    {
        start.ArgumentList.Add(a);
    }
    start.Environment["DOTNET_NOLOGO"] = "1";
    start.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";

    using var process = Process.Start(start)!;
    var ut = process.StandardOutput.ReadToEndAsync();
    var fel = process.StandardError.ReadToEndAsync();
    process.WaitForExit();
    return (process.ExitCode, ut.Result + fel.Result);
}

static string Elevnamn(string rot)
{
    try
    {
        var start = new ProcessStartInfo("git", "config user.name")
        {
            WorkingDirectory = rot,
            RedirectStandardOutput = true,
        };
        using var process = Process.Start(start)!;
        var namn = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();
        if (namn.Length > 0)
        {
            return namn;
        }
    }
    catch
    {
        // git saknas: använd användarnamnet nedan
    }
    return Environment.UserName;
}

static List<(string Tid, int Godkanda, int Totalt)> UppdateraHistorik(string fil, string tid, List<Test> tester, bool byggt)
{
    var historik = File.Exists(fil)
        ? File.ReadAllLines(fil).Skip(1).Select(r => r.Split(';')).Where(d => d.Length == 3)
            .Select(d => (d[0], int.Parse(d[1]), int.Parse(d[2]))).ToList()
        : new List<(string, int, int)>();

    if (byggt)
    {
        historik.Add((tid, tester.Count(t => t.Ok), tester.Count));
    }
    historik = historik.TakeLast(200).ToList();
    File.WriteAllLines(fil, new[] { "tid;godkanda;totalt" }.Concat(historik.Select(h => $"{h.Item1};{h.Item2};{h.Item3}")));
    return historik;
}

static string SkapaSida(string mall, object data)
{
    // Standardinställningen escapar < och > så att JSON kan ligga tryggt i en <script>-tagg.
    var json = JsonSerializer.Serialize(data);
    return mall.Replace("/*DATA*/", json);
}
