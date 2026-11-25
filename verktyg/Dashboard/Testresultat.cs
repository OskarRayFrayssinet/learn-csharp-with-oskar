using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Dashboard;

public sealed record Test(string Namn, string Nr, bool Ok, string Meddelande);

public sealed record Ovning(string Nr, string Namn, int Godkanda, int Totalt);

/// <summary>Läser testresultat (TRX-filer) och grupperar dem per övning.</summary>
public static class Testresultat
{
    public static readonly Dictionary<string, string> Ovningsnamn = new()
    {
        ["01"] = "Grunder – villkor, loopar och arrayer",
        ["02"] = "Sortera en lista",
        ["03"] = "Filstrukturen i C#",
        ["04"] = "Träning – 40 små uppgifter",
        ["05"] = "Merge-konflikter",
        ["06"] = "Versionering",
        ["07"] = "Rätta gammal historik",
    };

    public static List<Test> Las(string mapp)
    {
        var tester = new List<Test>();
        if (!Directory.Exists(mapp))
        {
            return tester;
        }

        foreach (var fil in Directory.EnumerateFiles(mapp, "*.trx", SearchOption.AllDirectories))
        {
            foreach (var resultat in XDocument.Load(fil).Descendants().Where(e => e.Name.LocalName == "UnitTestResult"))
            {
                var namn = resultat.Attribute("testName")?.Value ?? "?";
                var meddelande = resultat.Descendants().FirstOrDefault(e => e.Name.LocalName == "Message")?.Value ?? "";
                var forsta = meddelande.Trim().Split('\n')[0].Trim();
                if (forsta.Contains("NotImplementedException"))
                {
                    forsta = "Inte påbörjad ännu.";
                }
                var nr = namn.Length >= 2 && char.IsDigit(namn[0]) && char.IsDigit(namn[1]) ? namn[..2] : "??";
                tester.Add(new Test(namn, nr, resultat.Attribute("outcome")?.Value == "Passed", forsta));
            }
        }

        return tester.OrderBy(t => Ordning(t.Namn).Nr)
            .ThenBy(t => Ordning(t.Namn).Del)
            .ThenBy(t => Ordning(t.Namn).Tal)
            .ThenBy(t => t.Namn, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>Sorterar i uppgiftsordning: övning, del (A, B, C), nummer. Regler och liknande sist.</summary>
    public static (string Nr, string Del, int Tal) Ordning(string namn)
    {
        var traff = Regex.Match(namn, @"^(\d+) · (?:([A-Z]) · )?\D*?(\d+)");
        if (!traff.Success)
        {
            return (namn.Length >= 2 ? namn[..2] : namn, "", 999);
        }
        return (traff.Groups[1].Value, traff.Groups[2].Value, int.Parse(traff.Groups[3].Value));
    }

    public static List<Ovning> Gruppera(List<Test> tester) =>
        tester.GroupBy(t => t.Nr)
            .OrderBy(g => g.Key)
            .Select(g => new Ovning(g.Key, Ovningsnamn.GetValueOrDefault(g.Key, $"Övning {g.Key}"),
                g.Count(t => t.Ok), g.Count()))
            .ToList();
}
