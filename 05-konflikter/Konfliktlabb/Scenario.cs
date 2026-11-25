namespace Konfliktlabb;

/// <summary>Ett övningsscenario: hur repot byggs upp och hur lösningen kontrolleras.</summary>
public sealed class Scenario
{
    public required int Nummer { get; init; }
    public required string Titel { get; init; }
    public required string Niva { get; init; }

    /// <summary>Vad som har hänt innan eleven tar över.</summary>
    public required string Berattelse { get; init; }

    /// <summary>Vad eleven ska göra och hur resultatet ska se ut.</summary>
    public required string Uppdrag { get; init; }

    /// <summary>Kommandon som eleven troligen behöver.</summary>
    public required string[] Kommandon { get; init; }

    /// <summary>Filerna som kontrolleras och sparas i kvittot.</summary>
    public required string[] Filer { get; init; }

    /// <summary>Bygger upp repot. Returnerar SHA för den commit som ska mergas in (eller null).</summary>
    public required Func<GitRepo, string, string?> Bygg { get; init; }

    /// <summary>Kontrollerar innehållet i filerna. Nyckeln är filnamnet, värdet null betyder att filen saknas.</summary>
    public required Func<IReadOnlyDictionary<string, string?>, List<string>> KontrolleraInnehall { get; init; }

    /// <summary>true om uppdraget är att avbryta mergen i stället för att slutföra den.</summary>
    public bool AvbrytMerge { get; init; }

    /// <summary>true om lösningen också ska pushas till det delade repot (origin).</summary>
    public bool KraverPush { get; init; }

    /// <summary>Tips som visas om eleven inte har slagit ihop rätt commit.</summary>
    public string TipsSammanslagning { get; init; } = "Du har inte gjort någon merge av kompisens gren. Kör: git merge kompis";

    public string Mappnamn => $"scenario-{Nummer:00}";
}

/// <summary>Hjälpmetoder för att jämföra filinnehåll och skriva tydliga fel.</summary>
public static class Regler
{
    public static IEnumerable<string> Rader(string? text) =>
        (text ?? "").Replace("\r\n", "\n").Split('\n').Select(r => r.TrimEnd()).Reverse()
            .SkipWhile(r => r.Length == 0).Reverse();

    /// <summary>Filen ska innehålla exakt de här raderna (mellanslag i slutet av raden ignoreras).</summary>
    public static List<string> ExaktaRader(IReadOnlyDictionary<string, string?> filer, string fil, params string[] forvantat)
    {
        var fel = new List<string>();
        if (!filer.TryGetValue(fil, out var text) || text == null)
        {
            fel.Add($"Filen {fil} saknas.");
            return fel;
        }

        var rader = Rader(text).ToList();
        for (int i = 0; i < Math.Max(rader.Count, forvantat.Length); i++)
        {
            var har = i < rader.Count ? rader[i] : null;
            var ska = i < forvantat.Length ? forvantat[i] : null;
            if (har == ska)
            {
                continue;
            }

            if (ska == null)
            {
                fel.Add($"{fil}: rad {i + 1} ska inte finnas, men innehåller \"{har}\".");
            }
            else if (har == null)
            {
                fel.Add($"{fil}: rad {i + 1} saknas. Den ska vara \"{ska}\".");
            }
            else
            {
                fel.Add($"{fil}: rad {i + 1} ska vara \"{ska}\", men är \"{har}\".");
            }

            // Visa bara första skillnaden, annars blir det för många meddelanden.
            break;
        }
        return fel;
    }

    /// <summary>Jämför kod. Indrag och tomma rader spelar ingen roll.</summary>
    public static List<string> SammaKod(IReadOnlyDictionary<string, string?> filer, string fil, string forvantat)
    {
        var fel = new List<string>();
        if (!filer.TryGetValue(fil, out var text) || text == null)
        {
            fel.Add($"Filen {fil} saknas.");
            return fel;
        }

        var har = Rader(text).Select(r => r.Trim()).Where(r => r.Length > 0).ToList();
        var ska = Rader(forvantat).Select(r => r.Trim()).Where(r => r.Length > 0).ToList();
        for (int i = 0; i < Math.Max(har.Count, ska.Count); i++)
        {
            var h = i < har.Count ? har[i] : "(inget)";
            var s = i < ska.Count ? ska[i] : "(inget)";
            if (h != s)
            {
                fel.Add($"{fil}: kodraden \"{h}\" stämmer inte. Där borde det stå \"{s}\".");
                break;
            }
        }
        return fel;
    }

    /// <summary>Letar efter rader som börjar med konfliktmarkeringar.</summary>
    public static List<string> Konfliktmarkeringar(string fil, string? text)
    {
        var fel = new List<string>();
        int nr = 0;
        foreach (var rad in (text ?? "").Replace("\r\n", "\n").Split('\n'))
        {
            nr++;
            if (rad.StartsWith("<<<<<<<") || rad.StartsWith("=======") || rad.StartsWith(">>>>>>>"))
            {
                fel.Add($"{fil}: rad {nr} är en konfliktmarkering ({rad.Split(' ')[0]}). Ta bort den.");
            }
        }
        return fel;
    }
}
