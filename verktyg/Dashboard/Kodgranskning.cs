using System.Text.RegularExpressions;

namespace Dashboard;

public sealed record Kodtips(string Fil, int Rad, string Kod, string Problem, string Losning);

/// <summary>
/// Samma kodgranskning som i GitHub Actions (.github/scripts/kodgranskning.py),
/// men på din egen dator.
/// </summary>
public static class Kodgranskning
{
    public static readonly Dictionary<string, (string Problem, string Losning)> Forklaring = new()
    {
        ["IDE0055"] = ("Indrag eller mellanslag följer inte standarden.", "Formatera filen: Ctrl+K, Ctrl+D i Visual Studio eller Shift+Alt+F i VS Code."),
        ["IDE0011"] = ("Klamrar { } saknas efter if, else, for eller while.", "Sätt alltid { } runt koden, även om det bara är en rad."),
        ["IDE1006"] = ("Namnet följer inte namnstandarden.", "Klasser och metoder: StorBokstav (PascalCase). Variabler och parametrar: litenBokstav (camelCase)."),
        ["IDE0059"] = ("Ett värde sparas i en variabel men används aldrig.", "Ta bort tilldelningen eller använd variabeln."),
        ["IDE0051"] = ("En privat metod eller variabel används aldrig.", "Ta bort den om den inte behövs."),
        ["IDE0052"] = ("En privat variabel får ett värde men läses aldrig.", "Ta bort den om den inte behövs."),
        ["CS0168"] = ("En variabel deklareras men används aldrig.", "Ta bort variabeln."),
        ["CS0219"] = ("En variabel får ett värde men används aldrig.", "Ta bort variabeln eller använd den."),
        ["CS0162"] = ("Koden kan aldrig köras, till exempel för att den står efter return.", "Ta bort koden eller flytta den."),
        ["STIL01"] = ("Console.WriteLine i ett klassbibliotek.", "Metoder i biblioteket ska returnera svaret, inte skriva ut det. Skriv ut i Provkör i stället."),
        ["STIL02"] = ("Variabelnamn med en enda bokstav.", "Välj ett namn som säger vad variabeln innehåller, t.ex. summa i stället för s. (i, j och k i loopar är okej.)"),
        ["STIL03"] = ("Bortkommenterad kod.", "Ta bort gammal kod i stället för att kommentera bort den. Git kommer ihåg den åt dig."),
        ["STIL04"] = ("Raden är längre än 120 tecken.", "Dela upp raden på flera rader, eller använd en hjälpvariabel."),
    };

    private static readonly Regex Varning = new(@"^(?<fil>.+?\.cs)\((?<rad>\d+),(?<kol>\d+)\): warning (?<kod>(IDE|CS)\d+): ");
    private static readonly Regex EnBokstav = new(@"\b(?:int|double|string|bool|char|var|long|float|decimal)\s+([a-zA-Z])\s*(?:=(?![=>])|;)");
    private static readonly Regex Bortkommenterad = new(@"^\s*//(?!/)\s*.*(;|\{|\})\s*$");

    private static readonly string[] IntePaElevkod = { "bin", "obj", "labb", ".github", "verktyg", "min-dashboard", "Konfliktlabb", "Historiklabb" };

    public static bool ArElevkod(string relativ)
    {
        var delar = relativ.Replace('\\', '/').Split('/');
        return !delar.Any(d => d.EndsWith(".Tests") || IntePaElevkod.Contains(d));
    }

    public static List<Kodtips> Granska(string rot, string bygglogg)
    {
        var hittade = new HashSet<(string Fil, int Rad, string Kod)>();

        foreach (var rad in bygglogg.Split('\n'))
        {
            var traff = Varning.Match(rad.Trim());
            if (!traff.Success)
            {
                continue;
            }
            var relativ = Path.GetRelativePath(rot, traff.Groups["fil"].Value).Replace('\\', '/');
            if (!relativ.StartsWith("..") && ArElevkod(relativ))
            {
                hittade.Add((relativ, int.Parse(traff.Groups["rad"].Value), traff.Groups["kod"].Value));
            }
        }

        foreach (var fil in Directory.EnumerateFiles(rot, "*.cs", SearchOption.AllDirectories))
        {
            var relativ = Path.GetRelativePath(rot, fil).Replace('\\', '/');
            if (!ArElevkod(relativ))
            {
                continue;
            }
            var bibliotek = ArBibliotek(fil);
            var rader = File.ReadAllLines(fil);
            for (int i = 0; i < rader.Length; i++)
            {
                var kod = rader[i].Split("//")[0];
                if (bibliotek && kod.Contains("Console.Write"))
                {
                    hittade.Add((relativ, i + 1, "STIL01"));
                }
                foreach (Match m in EnBokstav.Matches(kod))
                {
                    if (m.Groups[1].Value is not ("i" or "j" or "k"))
                    {
                        hittade.Add((relativ, i + 1, "STIL02"));
                    }
                }
                if (bibliotek && Bortkommenterad.IsMatch(rader[i]))
                {
                    hittade.Add((relativ, i + 1, "STIL03"));
                }
                if (rader[i].Length > 120)
                {
                    hittade.Add((relativ, i + 1, "STIL04"));
                }
            }
        }

        return hittade.OrderBy(h => h.Fil, StringComparer.Ordinal).ThenBy(h => h.Rad)
            .Select(h =>
            {
                var (problem, losning) = Forklaring.GetValueOrDefault(h.Kod, (h.Kod, "Se meddelandet i Visual Studio."));
                return new Kodtips(h.Fil, h.Rad, h.Kod, problem, losning);
            })
            .ToList();
    }

    private static bool ArBibliotek(string fil)
    {
        var mapp = Path.GetDirectoryName(fil);
        while (mapp != null)
        {
            var projekt = Directory.EnumerateFiles(mapp, "*.csproj").FirstOrDefault();
            if (projekt != null)
            {
                return !File.ReadAllText(projekt).Contains("<OutputType>Exe</OutputType>");
            }
            mapp = Path.GetDirectoryName(mapp);
        }
        return false;
    }
}
