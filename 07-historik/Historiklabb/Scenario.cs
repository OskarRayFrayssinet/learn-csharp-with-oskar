namespace Historiklabb;

/// <summary>Ett scenario i Historiklabbet.</summary>
public sealed class Scenario
{
    public required int Nummer { get; init; }
    public required string Titel { get; init; }
    public required string Niva { get; init; }
    public required string Berattelse { get; init; }
    public required string Uppdrag { get; init; }
    public required string[] Kommandon { get; init; }

    /// <summary>Bygger upp repot med sin historik.</summary>
    public required Action<GitRepo> Bygg { get; init; }

    /// <summary>Jämför historiken före och efter. Returnerar en lista med fel (tom = klart).</summary>
    public required Func<Ogonblick, Ogonblick, List<string>> Kontrollera { get; init; }

    public string Mappnamn => $"scenario-{Nummer:00}";
}

/// <summary>Vanliga kontroller som scenarierna använder.</summary>
public static class Krav
{
    public static string Normalisera(string? text) => (text ?? "").Replace("\r\n", "\n").Trim();

    /// <summary>main ska ha exakt de här commit-meddelandena, från första till senaste.</summary>
    public static void Meddelanden(Ogonblick efter, List<string> fel, params string[] forvantat)
    {
        var har = efter.Meddelanden();
        if (!har.SequenceEqual(forvantat))
        {
            fel.Add("Historiken på main ska vara (äldst först): " + Lista(forvantat) +
                    "  Nu är den: " + Lista(har));
        }
    }

    /// <summary>De första commitarna ska vara exakt samma som före (samma id), alltså orörda.</summary>
    public static void OrordaForsta(Ogonblick fore, Ogonblick efter, int antal, List<string> fel)
    {
        var f = fore.Kedja();
        var e = efter.Kedja();
        for (int i = 0; i < antal && i < f.Count; i++)
        {
            if (i >= e.Count || e[i].Sha != f[i].Sha)
            {
                fel.Add($"Commit {i + 1} (\"{f[i].Meddelande}\") skulle inte ändras, men den har skrivits om. " +
                        "Skapa scenariot igen och försök på nytt.");
                return;
            }
        }
    }

    /// <summary>Exakt en ny commit ovanpå den gamla historiken, som ska vara orörd.</summary>
    public static Commit? EnNyCommit(Ogonblick fore, Ogonblick efter, List<string> fel)
    {
        var f = fore.Kedja();
        var e = efter.Kedja();
        if (e.Count != f.Count + 1)
        {
            fel.Add($"Det ska finnas exakt en ny commit ovanpå de {f.Count} gamla. Nu finns {e.Count} commits på main.");
            return null;
        }
        OrordaForsta(fore, efter, f.Count, fel);
        return e[^1];
    }

    /// <summary>Två commits ska ha exakt samma filer med samma innehåll.</summary>
    public static void SammaFiler(Commit? har, Dictionary<string, string> ska, string vad, List<string> fel)
    {
        if (har == null)
        {
            fel.Add($"{vad} saknas.");
            return;
        }
        foreach (var (fil, innehall) in ska)
        {
            if (!har.Filer.TryGetValue(fil, out var h))
            {
                fel.Add($"{vad}: filen {fil} saknas.");
            }
            else if (Normalisera(h) != Normalisera(innehall))
            {
                fel.Add($"{vad}: innehållet i {fil} har ändrats, men det skulle vara som förut.");
            }
        }
        foreach (var fil in har.Filer.Keys.Where(f => !ska.ContainsKey(f)))
        {
            fel.Add($"{vad}: filen {fil} ska inte finnas där.");
        }
    }

    public static Dictionary<string, string> Med(Dictionary<string, string> filer, string fil, string innehall) =>
        new(filer) { [fil] = innehall };

    public static Dictionary<string, string> Utan(Dictionary<string, string> filer, string fil)
    {
        var kopia = new Dictionary<string, string>(filer);
        kopia.Remove(fil);
        return kopia;
    }

    /// <summary>Senaste commit ska vara en revert av commiten med det här meddelandet.</summary>
    public static void ArRevert(Commit? commit, string meddelande, List<string> fel)
    {
        if (commit != null && !commit.Meddelande.StartsWith($"Revert \"{meddelande}\""))
        {
            fel.Add($"Den nya commiten ska vara en revert av \"{meddelande}\" (git revert <id>). " +
                    $"Nu heter den \"{commit.Meddelande}\".");
        }
    }

    private static string Lista(IEnumerable<string> m) =>
        string.Join("  ", m.Select((t, i) => $"{i + 1}) {t}"));
}
