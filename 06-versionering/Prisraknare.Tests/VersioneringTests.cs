using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Prisraknare;

namespace Prisraknare.Tests;

// ÄNDRA INTE I DEN HÄR FILEN.
// Testerna använder "reflection" för att hitta metoderna, så att de fungerar
// både före och efter att du har bytt namn på eller lagt till metoder.

public class DelA_Fragor
{
    [Fact(DisplayName = "06 · A · Fråga 1: Buggfix i 1.4.2")]
    public void F01() => Hjalp.KollaSvar(1, "7b598eabcaa511cc");

    [Fact(DisplayName = "06 · A · Fråga 2: Ny metod i 2.3.7")]
    public void F02() => Hjalp.KollaSvar(2, "151968cac454dc13");

    [Fact(DisplayName = "06 · A · Fråga 3: Borttagen metod i 1.9.0")]
    public void F03() => Hjalp.KollaSvar(3, "0541b1e473ae5b70");

    [Fact(DisplayName = "06 · A · Fråga 4: Ny returtyp i 3.2.1")]
    public void F04() => Hjalp.KollaSvar(4, "62d08c585ead16d9");

    [Fact(DisplayName = "06 · A · Fråga 5: Vad händer med PATCH?")]
    public void F05() => Hjalp.KollaSvar(5, "f9122da6a53fa068");

    [Fact(DisplayName = "06 · A · Fråga 6: Version 0.x")]
    public void F06() => Hjalp.KollaSvar(6, "6ba027f7d3c3b079");

    [Fact(DisplayName = "06 · A · Fråga 7: Snabbare utan nytt beteende")]
    public void F07() => Hjalp.KollaSvar(7, "bcdc4c56d73af0ca");

    [Fact(DisplayName = "06 · A · Fråga 8: Vilken version är högst?")]
    public void F08() => Hjalp.KollaSvar(8, "7487f69e31a3c2b0");

    [Fact(DisplayName = "06 · A · Fråga 9: Varför höja MAJOR?")]
    public void F09() => Hjalp.KollaSvar(9, "f7f7e7e2f143bd4c");

    [Fact(DisplayName = "06 · A · Fråga 10: Vad är 2.0.0-beta.1?")]
    public void F10() => Hjalp.KollaSvar(10, "211bb38c0b4078fb");
}

public class DelB_Arenden
{
    [Fact(DisplayName = "06 · B · Ärende 1: Rabatt räknar rätt")]
    public void Arende1()
    {
        var namn = Hjalp.Finns("MedRabatt", 2) ? "MedRabatt" : "Rabatt";
        Hjalp.Lika(180, namn, 200.0, 10.0);
        Hjalp.Lika(60, namn, 80.0, 25.0);
        Hjalp.Lika(100, namn, 100.0, 0.0);
    }

    [Fact(DisplayName = "06 · B · Ärende 2: Avrunda finns och avrundar rätt")]
    public void Arende2()
    {
        Assert.True(Hjalp.Finns("Avrunda", 1), "Metoden public static double Avrunda(double pris) finns inte än.");
        Hjalp.Lika(100, "Avrunda", 99.5);
        Hjalp.Lika(99, "Avrunda", 99.4);
        Hjalp.Lika(3, "Avrunda", 2.5);
        Hjalp.Lika(10, "Avrunda", 10.0);
    }

    [Fact(DisplayName = "06 · B · Ärende 3: MedMoms med valfri momssats")]
    public void Arende3()
    {
        Assert.True(Hjalp.Finns("MedMoms", 2),
            "Metoden public static double MedMoms(double pris, double momsProcent) finns inte än.");
        Hjalp.Lika(106, "MedMoms", 100.0, 6.0);
        Hjalp.Lika(224, "MedMoms", 200.0, 12.0);
        Hjalp.Lika(125, "MedMoms", 100.0, 25.0);
        Assert.True(Hjalp.Finns("MedMoms", 1),
            "Den gamla MedMoms(pris) måste finnas kvar, annars slutar andras kod att fungera.");
        Hjalp.Lika(125, "MedMoms", 100.0);
    }

    [Fact(DisplayName = "06 · B · Ärende 4: Rabatt heter MedRabatt")]
    public void Arende4()
    {
        Assert.True(Hjalp.Finns("MedRabatt", 2), "Metoden MedRabatt(double pris, double procent) finns inte än.");
        Assert.False(Hjalp.Finns("Rabatt", 2), "Den gamla metoden Rabatt finns kvar. Den ska bytas ut mot MedRabatt.");
        Hjalp.Lika(180, "MedRabatt", 200.0, 10.0);
    }

    [Fact(DisplayName = "06 · B · Ärende 5: Priset blir aldrig negativt")]
    public void Arende5()
    {
        Assert.True(Hjalp.Finns("MedRabatt", 2), "Gör ärende 4 först (MedRabatt finns inte än).");
        Hjalp.Lika(0, "MedRabatt", 100.0, 150.0);
        Hjalp.Lika(0, "MedRabatt", 100.0, 100.0);
        Hjalp.Lika(80, "MedRabatt", 100.0, 20.0);
    }
}

public class DelC_Versioner
{
    [Fact(DisplayName = "06 · C · 1: Version 1.0.1 finns i CHANGELOG (ärende 1)")]
    public void V1() => Hjalp.KollaVersion("1.0.1", 1, "Fixat");

    [Fact(DisplayName = "06 · C · 2: Version 1.1.0 finns i CHANGELOG (ärende 2)")]
    public void V2() => Hjalp.KollaVersion("1.1.0", 2, "Tillagt");

    [Fact(DisplayName = "06 · C · 3: Version 1.2.0 finns i CHANGELOG (ärende 3)")]
    public void V3() => Hjalp.KollaVersion("1.2.0", 3, "Tillagt");

    [Fact(DisplayName = "06 · C · 4: Version 2.0.0 finns i CHANGELOG (ärende 4)")]
    public void V4() => Hjalp.KollaVersion("2.0.0", 4, "Ändrat", "Borttaget");

    [Fact(DisplayName = "06 · C · 5: Version 2.0.1 finns i CHANGELOG (ärende 5)")]
    public void V5() => Hjalp.KollaVersion("2.0.1", 5, "Fixat");

    [Fact(DisplayName = "06 · C · 6: Versionen i .csproj stämmer med CHANGELOG")]
    public void VersionICsproj()
    {
        var versioner = Hjalp.Versioner();
        var csproj = Path.Combine(Hjalp.Ovningsmapp(), "Prisraknare", "Prisraknare.csproj");
        var version = XDocument.Load(csproj).Descendants("Version").FirstOrDefault()?.Value.Trim();
        Assert.True(versioner.Count > 1, "Gör ärende 1 först och lägg till den nya versionen i CHANGELOG.md.");
        Assert.True(version == versioner[0],
            $"<Version> i Prisraknare.csproj är {version}, men senaste versionen i CHANGELOG.md är {versioner[0]}. De ska vara samma.");
    }

    [Fact(DisplayName = "06 · C · 7: Bara rätta versioner, nyaste först")]
    public void Ordning()
    {
        var versioner = Hjalp.Versioner();
        Assert.True(versioner.Count > 1, "Gör ärende 1 först och lägg till den nya versionen i CHANGELOG.md.");

        var tillatna = new[] { "2.0.1", "2.0.0", "1.2.0", "1.1.0", "1.0.1", "1.0.0" };
        foreach (var v in versioner)
        {
            Assert.True(tillatna.Contains(v),
                $"Version {v} i CHANGELOG.md stämmer inte med något ärende. Tänk på vilken del av versionen som ska höjas.");
        }

        var sorterade = versioner.OrderByDescending(v => new Version(v)).ToList();
        Assert.True(versioner.SequenceEqual(sorterade),
            "Versionerna i CHANGELOG.md ska stå med den nyaste överst: " + string.Join(", ", sorterade));
    }

    [Fact(DisplayName = "06 · C · 8: Git-taggar finns för versionerna")]
    public void Taggar()
    {
        var versioner = Hjalp.Versioner().Where(v => v != "1.0.0").ToList();
        Assert.True(versioner.Count > 0, "Gör ärende 1 först.");

        var taggar = Hjalp.Git("tag", "--list", "v*").Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim()).ToHashSet();
        var saknas = versioner.Where(v => !taggar.Contains("v" + v)).ToList();
        Assert.True(saknas.Count == 0,
            "Taggar saknas för: " + string.Join(", ", saknas.Select(v => "v" + v)) +
            ". Skapa med t.ex. git tag v" + saknas.FirstOrDefault() + " och skicka med git push --tags");
    }
}

internal static class Hjalp
{
    public static string Ovningsmapp()
    {
        var mapp = new DirectoryInfo(AppContext.BaseDirectory);
        while (mapp != null)
        {
            if (File.Exists(Path.Combine(mapp.FullName, "CHANGELOG.md"))
                && Directory.Exists(Path.Combine(mapp.FullName, "Prisraknare")))
            {
                return mapp.FullName;
            }
            mapp = mapp.Parent;
        }
        throw new DirectoryNotFoundException("Hittade inte mappen 06-versionering.");
    }

    // ---- Del A ----------------------------------------------------------

    public static void KollaSvar(int nummer, string ratt)
    {
        string? svar = null;
        foreach (var rad in File.ReadAllLines(Path.Combine(Ovningsmapp(), "svar.txt")))
        {
            var traff = Regex.Match(rad, @"^\s*Fr\S{1,2}ga\s*(\d+)\s*:\s*([A-Da-d])?", RegexOptions.IgnoreCase);
            if (traff.Success && int.Parse(traff.Groups[1].Value) == nummer && traff.Groups[2].Success)
            {
                svar = traff.Groups[2].Value.ToUpperInvariant();
            }
        }
        Assert.True(svar != null, $"Du har inte svarat på fråga {nummer} i svar.txt ännu.");

        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"semver-{nummer}-{svar}")))
            .ToLowerInvariant()[..16];
        Assert.True(hash == ratt, $"Svaret {svar} på fråga {nummer} är inte rätt. Läs guiden om versionering igen.");
    }

    // ---- Del B ----------------------------------------------------------

    private static MethodInfo? Metod(string namn, int antalParametrar) =>
        typeof(Pris).GetMethod(namn, BindingFlags.Public | BindingFlags.Static,
            Enumerable.Repeat(typeof(double), antalParametrar).ToArray());

    public static bool Finns(string namn, int antalParametrar) => Metod(namn, antalParametrar) != null;

    public static void Lika(double forvantat, string namn, params object[] argument)
    {
        var metod = Metod(namn, argument.Length);
        var anrop = $"{namn}({string.Join(", ", argument.Select(a => ((double)a).ToString(System.Globalization.CultureInfo.InvariantCulture)))})";
        Assert.True(metod != null, $"Metoden {namn} med {argument.Length} parameter/parametrar (double) finns inte.");

        double faktiskt;
        try
        {
            faktiskt = (double)metod!.Invoke(null, argument)!;
        }
        catch (TargetInvocationException fel)
        {
            Assert.Fail($"{anrop} kraschade med {fel.InnerException?.GetType().Name}.");
            return;
        }
        Assert.True(Math.Abs(faktiskt - forvantat) < 0.0001,
            $"{anrop} ska bli {forvantat}, men blev {faktiskt.ToString(System.Globalization.CultureInfo.InvariantCulture)}.");
    }

    // ---- Del C ----------------------------------------------------------

    /// <summary>Alla versionsrubriker i CHANGELOG.md, i den ordning de står.</summary>
    public static List<string> Versioner() =>
        Regex.Matches(File.ReadAllText(Path.Combine(Ovningsmapp(), "CHANGELOG.md")),
                @"^##\s+\[?(\d+\.\d+\.\d+)\]?", RegexOptions.Multiline)
            .Select(m => m.Groups[1].Value).ToList();

    private static string? Avsnitt(string version)
    {
        var text = File.ReadAllText(Path.Combine(Ovningsmapp(), "CHANGELOG.md")).Replace("\r\n", "\n");
        var traff = Regex.Match(text, @"^##\s+\[?" + Regex.Escape(version) + @"\]?.*?$(.*?)(?=^##\s|\z)",
            RegexOptions.Multiline | RegexOptions.Singleline);
        return traff.Success ? traff.Groups[1].Value : null;
    }

    public static void KollaVersion(string version, int arende, params string[] kategorier)
    {
        var avsnitt = Avsnitt(version);
        Assert.True(avsnitt != null,
            $"CHANGELOG.md saknar ## [{version}]. Har du gjort ärende {arende}? " +
            "Fundera på om ändringen är en buggfix (PATCH), ny funktion (MINOR) eller bryter befintlig kod (MAJOR).");

        var harKategori = kategorier.Any(k =>
            Regex.IsMatch(avsnitt!, @"^###\s+" + k + @"\s*$\s*^\s*[-*]\s+\S", RegexOptions.Multiline | RegexOptions.IgnoreCase));
        Assert.True(harKategori,
            $"Version {version} hör till ärende {arende}. Under ## [{version}] ska det finnas rubriken " +
            $"### {string.Join(" eller ### ", kategorier)} med minst en punkt (- ...). " +
            "Har du använt versionsnumret till ett annat ärende? Kontrollera om varje ärende är PATCH, MINOR eller MAJOR.");
    }

    public static string Git(params string[] argument)
    {
        var start = new ProcessStartInfo("git")
        {
            WorkingDirectory = Ovningsmapp(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        foreach (var a in argument)
        {
            start.ArgumentList.Add(a);
        }
        using var process = Process.Start(start)!;
        var ut = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return ut;
    }
}
