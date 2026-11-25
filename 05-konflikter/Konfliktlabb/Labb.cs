using System.Text;

namespace Konfliktlabb;

/// <summary>Skapar och kontrollerar scenarierna i mappen 05-konflikter/labb.</summary>
public sealed class Labb
{
    private const string Tillstandsfil = "konfliktlabbet.txt";

    public Labb(string ovningsmapp)
    {
        Ovningsmapp = ovningsmapp;
    }

    public string Ovningsmapp { get; }

    public string Labbmapp => Path.Combine(Ovningsmapp, "labb");

    public string Kvittomapp => Path.Combine(Ovningsmapp, "kvitton");

    public string ScenarioMapp(Scenario s) => Path.Combine(Labbmapp, s.Mappnamn);

    public bool ArKlar(Scenario s) => File.Exists(Kvitto.Sokvag(Kvittomapp, s));

    /// <summary>Hittar mappen 05-konflikter genom att gå uppåt från programmets mapp.</summary>
    public static string HittaOvningsmapp()
    {
        var mapp = new DirectoryInfo(AppContext.BaseDirectory);
        while (mapp != null)
        {
            if (mapp.Name == "05-konflikter")
            {
                return mapp.FullName;
            }
            mapp = mapp.Parent;
        }
        throw new DirectoryNotFoundException("Hittade inte mappen 05-konflikter.");
    }

    public string Skapa(Scenario s)
    {
        var mapp = ScenarioMapp(s);
        Scenarier.TaBortMapp(mapp);
        Directory.CreateDirectory(Labbmapp);

        var repo = new GitRepo(mapp);
        var mal = s.Bygg(repo, Labbmapp);
        var start = repo.Sha("HEAD");

        File.WriteAllText(Path.Combine(mapp, ".git", Tillstandsfil),
            $"scenario={s.Nummer}\nstart={start}\nmal={mal ?? ""}\n");

        // Hjälpfiler som inte ska synas i git status.
        var projekt = Path.Combine(Ovningsmapp, "Konfliktlabb", "Konfliktlabb.csproj");
        File.WriteAllText(Path.Combine(mapp, "UPPDRAG.txt"), Beskrivning(s), new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(mapp, "kontrollera.cmd"),
            $"@dotnet run --project \"{projekt}\" -- kontrollera {s.Nummer}\r\n");
        File.WriteAllText(Path.Combine(mapp, "kontrollera.sh"),
            $"#!/bin/sh\ndotnet run --project \"{projekt}\" -- kontrollera {s.Nummer}\n");
        File.AppendAllText(Path.Combine(mapp, ".git", "info", "exclude"),
            "\nUPPDRAG.txt\nkontrollera.cmd\nkontrollera.sh\n");

        return mapp;
    }

    /// <summary>Kontrollerar elevens lösning. Returnerar en lista med fel (tom lista = klart).</summary>
    public List<string> Kontrollera(Scenario s)
    {
        var fel = new List<string>();
        var mapp = ScenarioMapp(s);
        var tillstand = Path.Combine(mapp, ".git", Tillstandsfil);
        if (!File.Exists(tillstand))
        {
            fel.Add($"Scenariot är inte skapat än. Kör: skapa {s.Nummer}");
            return fel;
        }

        var repo = new GitRepo(mapp);
        var varden = File.ReadAllLines(tillstand)
            .Select(r => r.Split('=', 2)).Where(d => d.Length == 2).ToDictionary(d => d[0], d => d[1]);

        var gren = repo.Forsok("symbolic-ref", "--short", "HEAD");
        if (gren.Kod != 0 || gren.Utdata != "main")
        {
            fel.Add("Du står inte på grenen main. Gå tillbaka med: git switch main");
        }

        var pagaendeMerge = repo.FinnsIGitMappen("MERGE_HEAD");
        var status = repo.Kor("status", "--porcelain");
        var filer = s.Filer.ToDictionary(f => f, f => repo.LasFil(f));

        foreach (var (fil, text) in filer)
        {
            fel.AddRange(Regler.Konfliktmarkeringar(fil, text));
        }

        if (s.AvbrytMerge)
        {
            if (!repo.FinnsIGitMappen("ORIG_HEAD"))
            {
                fel.Add("Det verkar som att du inte har startat någon merge. Kör först: git merge experiment");
            }
            if (pagaendeMerge)
            {
                fel.Add("Mergen pågår fortfarande. Avbryt den med: git merge --abort");
            }
            else if (repo.Sha("HEAD") != varden["start"])
            {
                fel.Add("main har ändrats. Uppdraget var att avbryta mergen, inte att slutföra den. Skapa scenariot igen och försök på nytt.");
            }
        }
        else
        {
            if (pagaendeMerge)
            {
                fel.Add("Du är mitt i en merge. Lös konflikterna, kör git add på filerna och avsluta med git commit --no-edit");
            }
            else
            {
                var mal = varden.GetValueOrDefault("mal") ?? "";
                var sammanslagen = mal.Length > 0 && repo.Forsok("merge-base", "--is-ancestor", mal, "HEAD").Kod == 0;
                var foraldrar = repo.Kor("rev-list", "--parents", "-n", "1", "HEAD").Split(' ').Length - 1;
                if (!sammanslagen || foraldrar < 2)
                {
                    fel.Add(s.TipsSammanslagning);
                }
                else if (s.KraverPush)
                {
                    var server = repo.Forsok("ls-remote", "origin", "refs/heads/main");
                    var paServern = server.Utdata.Split('\t')[0];
                    if (server.Kod != 0 || paServern != repo.Sha("HEAD"))
                    {
                        fel.Add("Din lösning finns bara på din dator. Skicka den till det delade repot med: git push");
                    }
                }
            }
        }

        if (status.Length > 0 && !pagaendeMerge)
        {
            fel.Add("Det finns ändringar som inte är committade (se git status). Kör git add och git commit.");
        }

        // Innehållet kontrolleras först när konfliktmarkeringarna är borta.
        if (!fel.Any(f => f.Contains("konfliktmarkering")))
        {
            fel.AddRange(s.KontrolleraInnehall(filer));
        }

        if (fel.Count == 0)
        {
            Kvitto.Skriv(Kvittomapp, s, filer);
        }
        return fel;
    }

    public static string Beskrivning(Scenario s)
    {
        var text = new StringBuilder();
        text.AppendLine($"SCENARIO {s.Nummer}: {s.Titel}  ({s.Niva})");
        text.AppendLine(new string('=', 60));
        text.AppendLine();
        text.AppendLine("VAD SOM HAR HÄNT");
        text.AppendLine(s.Berattelse);
        text.AppendLine();
        text.AppendLine("DITT UPPDRAG");
        text.AppendLine(s.Uppdrag);
        text.AppendLine();
        text.AppendLine("KOMMANDON DU TROLIGEN BEHÖVER");
        foreach (var k in s.Kommandon)
        {
            text.AppendLine($"    {k}");
        }
        text.AppendLine();
        text.AppendLine("KONTROLLERA DIN LÖSNING");
        text.AppendLine("    Windows:    .\\kontrollera.cmd");
        text.AppendLine("    Mac/Linux:  sh kontrollera.sh");
        return text.ToString();
    }
}
