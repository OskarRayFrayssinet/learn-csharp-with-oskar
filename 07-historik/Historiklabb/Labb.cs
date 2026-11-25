using System.Text;
using System.Text.Json;

namespace Historiklabb;

/// <summary>Kvittot som sparas när ett scenario är klart. Autorättningen kontrollerar det igen.</summary>
public sealed class Kvitto
{
    public int Scenario { get; set; }
    public string Titel { get; set; } = "";
    public Ogonblick Fore { get; set; } = new();
    public Ogonblick Efter { get; set; } = new();

    public static string Sokvag(string kvittomapp, Scenario s) => Path.Combine(kvittomapp, $"scenario-{s.Nummer:00}.json");

    public static Kvitto? Las(string sokvag) =>
        File.Exists(sokvag) ? JsonSerializer.Deserialize<Kvitto>(File.ReadAllText(sokvag)) : null;
}

/// <summary>Skapar och kontrollerar scenarierna i mappen 07-historik/labb.</summary>
public sealed class Labb
{
    private const string Forefil = "historiklabbet-fore.json";

    public Labb(string ovningsmapp)
    {
        Ovningsmapp = ovningsmapp;
    }

    public string Ovningsmapp { get; }

    public string Labbmapp => Path.Combine(Ovningsmapp, "labb");

    public string Kvittomapp => Path.Combine(Ovningsmapp, "kvitton");

    public string ScenarioMapp(Scenario s) => Path.Combine(Labbmapp, s.Mappnamn);

    public bool ArKlar(Scenario s) => File.Exists(Kvitto.Sokvag(Kvittomapp, s));

    public static string HittaOvningsmapp()
    {
        var mapp = new DirectoryInfo(AppContext.BaseDirectory);
        while (mapp != null)
        {
            if (mapp.Name == "07-historik")
            {
                return mapp.FullName;
            }
            mapp = mapp.Parent;
        }
        throw new DirectoryNotFoundException("Hittade inte mappen 07-historik.");
    }

    public string Skapa(Scenario s)
    {
        var mapp = ScenarioMapp(s);
        Scenarier.TaBortMapp(mapp);
        Directory.CreateDirectory(mapp);

        var repo = new GitRepo(mapp);
        s.Bygg(repo);
        File.WriteAllText(Path.Combine(mapp, ".git", Forefil), JsonSerializer.Serialize(Ogonblick.Ta(repo)));

        var projekt = Path.Combine(Ovningsmapp, "Historiklabb", "Historiklabb.csproj");
        File.WriteAllText(Path.Combine(mapp, "UPPDRAG.txt"), Beskrivning(s), new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(mapp, "kontrollera.cmd"),
            $"@dotnet run --project \"{projekt}\" -- kontrollera {s.Nummer}\r\n");
        File.WriteAllText(Path.Combine(mapp, "kontrollera.sh"),
            $"#!/bin/sh\ndotnet run --project \"{projekt}\" -- kontrollera {s.Nummer}\n");
        File.AppendAllText(Path.Combine(mapp, ".git", "info", "exclude"), "\nUPPDRAG.txt\nkontrollera.cmd\nkontrollera.sh\n");
        return mapp;
    }

    public List<string> Kontrollera(Scenario s)
    {
        var fel = new List<string>();
        var mapp = ScenarioMapp(s);
        var forefil = Path.Combine(mapp, ".git", Forefil);
        if (!File.Exists(forefil))
        {
            fel.Add($"Scenariot är inte skapat än. Kör: skapa {s.Nummer}");
            return fel;
        }

        var repo = new GitRepo(mapp);
        var git = Path.Combine(mapp, ".git");
        if (Directory.Exists(Path.Combine(git, "rebase-merge")) || Directory.Exists(Path.Combine(git, "rebase-apply")))
        {
            fel.Add("Du är mitt i en rebase. Fortsätt med git rebase --continue, eller börja om med git rebase --abort.");
        }
        if (File.Exists(Path.Combine(git, "MERGE_HEAD")))
        {
            fel.Add("Du är mitt i en merge. Avsluta den med git commit --no-edit, eller avbryt med git merge --abort.");
        }
        if (File.Exists(Path.Combine(git, "CHERRY_PICK_HEAD")))
        {
            fel.Add("Du är mitt i en cherry-pick. Avsluta med git cherry-pick --continue.");
        }
        if (File.Exists(Path.Combine(git, "REVERT_HEAD")))
        {
            fel.Add("Du är mitt i en revert. Avsluta med git revert --continue.");
        }
        if (File.Exists(Path.Combine(git, "BISECT_LOG")))
        {
            fel.Add("git bisect pågår fortfarande. Skriv upp commitens id och avsluta med git bisect reset.");
        }
        if (fel.Count > 0)
        {
            return fel;
        }

        var gren = repo.Forsok("symbolic-ref", "--short", "HEAD");
        if (gren.Kod != 0 || gren.Utdata != "main")
        {
            fel.Add("Du står inte på grenen main. Gå tillbaka med: git switch main");
        }
        if (repo.Kor("status", "--porcelain").Length > 0)
        {
            fel.Add("Det finns ändringar som inte är committade (se git status).");
        }

        var fore = JsonSerializer.Deserialize<Ogonblick>(File.ReadAllText(forefil))!;
        var efter = Ogonblick.Ta(repo);
        fel.AddRange(s.Kontrollera(fore, efter));

        if (fel.Count == 0)
        {
            Directory.CreateDirectory(Kvittomapp);
            var kvitto = new Kvitto { Scenario = s.Nummer, Titel = s.Titel, Fore = fore, Efter = efter };
            File.WriteAllText(Kvitto.Sokvag(Kvittomapp, s),
                JsonSerializer.Serialize(kvitto, new JsonSerializerOptions { WriteIndented = true }), new UTF8Encoding(false));
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
        text.AppendLine("OM GIT ÖPPNAR EN TEXTEDITOR");
        text.AppendLine($"    Git använder {GitRepo.Editor()} i det här scenariot.");
        text.AppendLine("    Gör din ändring, SPARA och STÄNG fönstret (eller fliken), så fortsätter git.");
        text.AppendLine();
        text.AppendLine("KONTROLLERA DIN LÖSNING");
        text.AppendLine("    Windows:    .\\kontrollera.cmd");
        text.AppendLine("    Mac/Linux:  sh kontrollera.sh");
        return text.ToString();
    }
}
