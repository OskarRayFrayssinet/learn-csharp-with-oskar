using System.Diagnostics;
using System.Text;

namespace Konfliktlabb;

/// <summary>
/// Kör git-kommandon i en mapp. Används för att bygga upp övningsrepona
/// och för att kontrollera elevens lösning.
/// </summary>
public sealed class GitRepo
{
    public GitRepo(string mapp)
    {
        Mapp = mapp;
    }

    public string Mapp { get; }

    public string Kor(params string[] argument)
    {
        var (kod, utdata) = Forsok(argument);
        if (kod != 0)
        {
            throw new InvalidOperationException($"git {string.Join(' ', argument)} misslyckades:\n{utdata}");
        }
        return utdata;
    }

    public (int Kod, string Utdata) Forsok(params string[] argument)
    {
        var start = new ProcessStartInfo("git")
        {
            WorkingDirectory = Mapp,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
        };
        foreach (var a in argument)
        {
            start.ArgumentList.Add(a);
        }
        start.Environment["GIT_TERMINAL_PROMPT"] = "0";
        start.Environment["GIT_MERGE_AUTOEDIT"] = "no";

        using var process = Process.Start(start)
            ?? throw new InvalidOperationException("Kunde inte starta git. Är git installerat?");
        var ut = process.StandardOutput.ReadToEndAsync();
        var fel = process.StandardError.ReadToEndAsync();
        process.WaitForExit();
        return (process.ExitCode, (ut.Result + fel.Result).Trim());
    }

    /// <summary>Skapar ett nytt repo med inställningar som gör övningarna likadana på alla datorer.</summary>
    public void Skapa()
    {
        Directory.CreateDirectory(Mapp);
        Kor("init", "-q", "-b", "main");
        Kor("config", "user.name", "Du");
        Kor("config", "user.email", "du@konfliktlabbet.local");
        Kor("config", "merge.conflictstyle", "merge");
        Kor("config", "pull.rebase", "false");
        Kor("config", "core.autocrlf", "false");
        Kor("config", "commit.gpgsign", "false");
    }

    public void Skriv(string fil, string innehall)
    {
        var sokvag = Path.Combine(Mapp, fil);
        Directory.CreateDirectory(Path.GetDirectoryName(sokvag)!);
        File.WriteAllText(sokvag, innehall.Replace("\r\n", "\n"), new UTF8Encoding(false));
    }

    public void TaBort(string fil)
    {
        Kor("rm", "-q", fil);
    }

    public void Commit(string meddelande, string person)
    {
        Kor("add", "-A");
        var epost = person.ToLowerInvariant().Replace('ä', 'a').Replace(' ', '.') + "@konfliktlabbet.local";
        Kor("-c", $"user.name={person}", "-c", $"user.email={epost}", "commit", "-q", "-m", meddelande);
    }

    public string Sha(string referens) => Kor("rev-parse", referens);

    public string? LasFil(string fil)
    {
        var sokvag = Path.Combine(Mapp, fil);
        return File.Exists(sokvag) ? File.ReadAllText(sokvag).Replace("\r\n", "\n") : null;
    }

    public bool FinnsIGitMappen(string namn) => File.Exists(Path.Combine(Mapp, ".git", namn));
}
