using System.Text;

namespace Konfliktlabb;

/// <summary>
/// Ett kvitto skrivs till 05-konflikter/kvitton när ett scenario är klart.
/// Kvittot committas och pushas, och autorättningen kontrollerar det.
/// </summary>
public static class Kvitto
{
    private const string FilStart = "--- fil: ";
    private const string Saknas = "(saknas)";
    private const string Slut = "--- slut";

    public static string Sokvag(string kvittomapp, Scenario s) =>
        Path.Combine(kvittomapp, $"scenario-{s.Nummer:00}.txt");

    public static void Skriv(string kvittomapp, Scenario s, IReadOnlyDictionary<string, string?> filer)
    {
        var text = new StringBuilder();
        text.Append($"Konfliktlabbet – kvitto för scenario {s.Nummer}: {s.Titel}\n");
        text.Append("Skapat av Konfliktlabb när kontrollen gick igenom. Ändra inte i filen.\n");
        foreach (var (fil, innehall) in filer)
        {
            if (innehall == null)
            {
                text.Append($"{FilStart}{fil} {Saknas}\n");
                continue;
            }
            text.Append($"{FilStart}{fil}\n");
            text.Append(innehall.EndsWith('\n') ? innehall : innehall + "\n");
            text.Append($"{Slut}\n");
        }

        Directory.CreateDirectory(kvittomapp);
        File.WriteAllText(Sokvag(kvittomapp, s), text.ToString(), new UTF8Encoding(false));
    }

    /// <summary>Läser filinnehållet ur ett kvitto.</summary>
    public static Dictionary<string, string?> Las(string sokvag)
    {
        var filer = new Dictionary<string, string?>();
        string? aktuell = null;
        var innehall = new StringBuilder();

        foreach (var rad in File.ReadAllText(sokvag).Replace("\r\n", "\n").Split('\n'))
        {
            if (aktuell == null && rad.StartsWith(FilStart))
            {
                var namn = rad[FilStart.Length..];
                if (namn.EndsWith(" " + Saknas))
                {
                    filer[namn[..^(Saknas.Length + 1)]] = null;
                }
                else
                {
                    aktuell = namn;
                    innehall.Clear();
                }
            }
            else if (aktuell != null && rad == Slut)
            {
                filer[aktuell] = innehall.ToString();
                aktuell = null;
            }
            else if (aktuell != null)
            {
                innehall.Append(rad).Append('\n');
            }
        }
        return filer;
    }
}
