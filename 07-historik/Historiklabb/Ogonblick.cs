namespace Historiklabb;

/// <summary>En commit: id, föräldrar, meddelande och alla filer med innehåll.</summary>
public sealed class Commit
{
    public string Sha { get; set; } = "";
    public List<string> Foraldrar { get; set; } = new();
    public string Meddelande { get; set; } = "";
    public Dictionary<string, string> Filer { get; set; } = new();
}

/// <summary>
/// En ögonblicksbild av ett repo: alla grenar och alla commits som kan nås från dem.
/// Tas en gång när scenariot skapas (före) och en gång när det kontrolleras (efter).
/// </summary>
public sealed class Ogonblick
{
    public Dictionary<string, string> Grenar { get; set; } = new();
    public List<Commit> Commits { get; set; } = new();

    public Commit? Hitta(string sha) => Commits.FirstOrDefault(c => c.Sha == sha);

    /// <summary>Commits på en gren, från första till senaste (bara första föräldern).</summary>
    public List<Commit> Kedja(string gren = "main")
    {
        var kedja = new List<Commit>();
        if (!Grenar.TryGetValue(gren, out var sha))
        {
            return kedja;
        }
        var aktuell = Hitta(sha);
        while (aktuell != null)
        {
            kedja.Add(aktuell);
            aktuell = aktuell.Foraldrar.Count > 0 ? Hitta(aktuell.Foraldrar[0]) : null;
        }
        kedja.Reverse();
        return kedja;
    }

    public List<string> Meddelanden(string gren = "main") => Kedja(gren).Select(c => c.Meddelande).ToList();

    public Commit? Topp(string gren = "main") => Grenar.TryGetValue(gren, out var sha) ? Hitta(sha) : null;

    public static Ogonblick Ta(GitRepo repo)
    {
        var bild = new Ogonblick();
        foreach (var rad in Rader(repo.Kor("for-each-ref", "--format=%(refname:short) %(objectname)", "refs/heads")))
        {
            var delar = rad.Split(' ');
            bild.Grenar[delar[0]] = delar[1];
        }

        foreach (var sha in Rader(repo.Kor("rev-list", "--topo-order", "--branches")))
        {
            // Föräldrar och meddelande med ett tecken emellan som aldrig står i text (\x1f).
            var info = repo.Kor("show", "-s", "--format=%P%x1f%s", sha).Split('\x1f');
            var commit = new Commit
            {
                Sha = sha,
                Foraldrar = info[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList(),
                Meddelande = info.Length > 1 ? info[1].Trim() : "",
            };
            foreach (var fil in Rader(repo.Kor("ls-tree", "-r", "--name-only", sha)))
            {
                commit.Filer[fil] = repo.Kor("show", $"{sha}:{fil}").Replace("\r\n", "\n").TrimEnd('\n');
            }
            bild.Commits.Add(commit);
        }
        return bild;
    }

    private static IEnumerable<string> Rader(string text) =>
        text.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).Where(r => r.Length > 0);
}
