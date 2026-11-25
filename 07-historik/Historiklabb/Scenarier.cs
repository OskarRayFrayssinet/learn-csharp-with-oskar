namespace Historiklabb;

/// <summary>Alla scenarier i Historiklabbet, från lätt till svårt.</summary>
public static class Scenarier
{
    private const string Readme = "# Kalkyl\nEtt litet projekt i Historiklabbet.\n";

    public static IReadOnlyList<Scenario> Alla { get; } = new List<Scenario>
    {
        // -----------------------------------------------------------------
        new()
        {
            Nummer = 1,
            Titel = "Fel meddelande i senaste commit",
            Niva = "Lätt",
            Berattelse = "Din senaste commit lägger till Multiplicera, men du råkade skriva meddelandet \"asdfgh\".",
            Uppdrag = "Ändra meddelandet på den SENASTE commiten till:  Lägg till Multiplicera\n" +
                      "Det ska inte bli någon ny commit, och innehållet ska vara som förut.",
            Kommandon = new[] { "git log --oneline", "git commit --amend -m \"Lägg till Multiplicera\"", "git log --oneline" },
            Bygg = repo => Historik(repo,
                ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme, ["Kalkylator.cs"] = Kalkylator(Addera) }),
                ("Lägg till Subtrahera", "Du", new() { ["Kalkylator.cs"] = Kalkylator(Addera, Subtrahera) }),
                ("asdfgh", "Du", new() { ["Kalkylator.cs"] = Kalkylator(Addera, Subtrahera, Multiplicera) })),
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                Krav.Meddelanden(efter, fel, "Skapa projektet", "Lägg till Subtrahera", "Lägg till Multiplicera");
                Krav.OrordaForsta(fore, efter, 2, fel);
                Krav.SammaFiler(efter.Topp(), fore.Topp()!.Filer, "Senaste commit", fel);
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 2,
            Titel = "Glömd fil i senaste commit",
            Niva = "Lätt",
            Berattelse = "Du committade Multiplicera, men glömde filen Tester.md som hör till.\n" +
                         "Den ligger kvar i mappen men är inte committad (se git status).",
            Uppdrag = "Lägg till Tester.md i den SENASTE commiten, utan att skapa en ny commit\n" +
                      "och utan att ändra meddelandet.",
            Kommandon = new[] { "git status", "git add Tester.md", "git commit --amend --no-edit", "git show --stat" },
            Bygg = repo =>
            {
                Historik(repo,
                    ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme, ["Kalkylator.cs"] = Kalkylator(Addera) }),
                    ("Lägg till Subtrahera", "Du", new() { ["Kalkylator.cs"] = Kalkylator(Addera, Subtrahera) }),
                    ("Lägg till Multiplicera", "Du", new() { ["Kalkylator.cs"] = Kalkylator(Addera, Subtrahera, Multiplicera) }));
                repo.SkrivOcommittad("Tester.md", TesterMd);
            },
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                Krav.Meddelanden(efter, fel, "Skapa projektet", "Lägg till Subtrahera", "Lägg till Multiplicera");
                Krav.OrordaForsta(fore, efter, 2, fel);
                Krav.SammaFiler(efter.Topp(), Krav.Med(fore.Topp()!.Filer, "Tester.md", TesterMd), "Senaste commit", fel);
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 3,
            Titel = "Ångra en gammal commit med revert",
            Niva = "Medel",
            Berattelse = "Commiten \"Snabba upp Medel\" (nummer 3 av 5) införde en bugg: Medel räknar nu med\n" +
                         "heltalsdivision. Commitarna efter den är bra och ska vara kvar.",
            Uppdrag = "Ångra commiten \"Snabba upp Medel\" med git revert.\n" +
                      "Det blir en NY commit som tar bort buggen. Den gamla historiken ska vara orörd.",
            Kommandon = new[] { "git log --oneline", "git show <id>", "git revert <id>  (stäng editorn om den öppnas)", "git log --oneline" },
            Bygg = repo => Historik(repo,
                ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme, ["Statistik.cs"] = Statistik(MedelRatt) }),
                ("Lägg till Storsta", "Du", new() { ["Statistik.cs"] = Statistik(MedelRatt, Storsta) }),
                ("Snabba upp Medel", "Kompisen", new() { ["Statistik.cs"] = Statistik(MedelFel, Storsta) }),
                ("Lägg till Minsta", "Du", new() { ["Statistik.cs"] = Statistik(MedelFel, Storsta, Minsta) }),
                ("Uppdatera README", "Du", new() { ["README.md"] = Readme + "Räknar summa, medel, största och minsta.\n" })),
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                var ny = Krav.EnNyCommit(fore, efter, fel);
                Krav.ArRevert(ny, "Snabba upp Medel", fel);
                Krav.SammaFiler(ny, Krav.Med(fore.Topp()!.Filer, "Statistik.cs", Statistik(MedelRatt, Storsta, Minsta)), "Den nya commiten", fel);
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 4,
            Titel = "Hitta buggen själv och reverta",
            Niva = "Medel",
            Berattelse = "Elever med EXAKT 50 poäng blir underkända, men gränsen ska vara 50 eller mer.\n" +
                         "Någon av de sju commitarna orsakade det. Ingen vet vilken.",
            Uppdrag = "Hitta commiten som ändrade villkoret i ArGodkand, och ångra den med git revert.\n" +
                      "Tips: git log -p Betyg.cs  eller  git blame Betyg.cs",
            Kommandon = new[] { "git log --oneline", "git blame Betyg.cs", "git log -p Betyg.cs", "git revert <id>" },
            Bygg = repo => Historik(repo,
                ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme, ["Betyg.cs"] = Betyg(">=", false, false) }),
                ("Lägg till betygsgränser", "Du", new() { ["Betyg.cs"] = Betyg(">=", true, false) }),
                ("Skriv dokumentation", "Kompisen", new() { ["Dokumentation.md"] = "# Betyg\nGodkänt från 50 poäng.\n" }),
                ("Städa koden", "Kompisen", new() { ["Betyg.cs"] = Betyg(">", true, false) }),
                ("Lägg till ArMVG", "Du", new() { ["Betyg.cs"] = Betyg(">", true, true) }),
                ("Uppdatera README", "Du", new() { ["README.md"] = Readme + "Räknar ut betyg.\n" }),
                ("Fler exempel i dokumentationen", "Du", new() { ["Dokumentation.md"] = "# Betyg\nGodkänt från 50 poäng.\nMVG från 90 poäng.\n" })),
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                var ny = Krav.EnNyCommit(fore, efter, fel);
                Krav.ArRevert(ny, "Städa koden", fel);
                Krav.SammaFiler(ny, Krav.Med(fore.Topp()!.Filer, "Betyg.cs", Betyg(">=", true, true)), "Den nya commiten", fel);
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 5,
            Titel = "Leta med git bisect",
            Niva = "Svår",
            Berattelse = "I installningar.txt ska det stå  Rabatt: 10  men nu står det  Rabatt: 100.\n" +
                         "Historiken har 16 commits som alla heter \"Uppdatering N\". Någon av dem ändrade rabatten.",
            Uppdrag = "Hitta commiten med git bisect (git halverar historiken åt dig), och ångra den med git revert.\n" +
                      "  1. git bisect start\n" +
                      "  2. git bisect bad                 (nuvarande version är fel)\n" +
                      "  3. git bisect good <id för första commit>\n" +
                      "  4. Titta i installningar.txt. Skriv git bisect good eller git bisect bad. Upprepa.\n" +
                      "  5. När git säger \"... is the first bad commit\": skriv upp id:t och kör git bisect reset\n" +
                      "  6. git revert <id>",
            Kommandon = new[] { "git bisect start", "git bisect bad", "git bisect good <id>", "type installningar.txt  (Mac/Linux: cat)", "git bisect reset", "git revert <id>" },
            Bygg = repo =>
            {
                var steg = new List<(string, string, Dictionary<string, string>)>
                {
                    ("Uppdatering 1", "Läraren", new() { ["README.md"] = Readme, ["installningar.txt"] = Installningar("10"), ["logg.txt"] = Logg(1) }),
                };
                for (int i = 2; i <= 16; i++)
                {
                    var filer = new Dictionary<string, string> { ["logg.txt"] = Logg(i) };
                    if (i == 11)
                    {
                        filer = new() { ["installningar.txt"] = Installningar("100") };
                    }
                    steg.Add(($"Uppdatering {i}", i % 2 == 0 ? "Du" : "Kompisen", filer));
                }
                Historik(repo, steg.ToArray());
            },
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                var ny = Krav.EnNyCommit(fore, efter, fel);
                Krav.ArRevert(ny, "Uppdatering 11", fel);
                Krav.SammaFiler(ny, Krav.Med(fore.Topp()!.Filer, "installningar.txt", Installningar("10")), "Den nya commiten", fel);
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 6,
            Titel = "Byt meddelande på en äldre commit",
            Niva = "Medel",
            Berattelse = "Commit nummer 3 av 5 heter \"fixade grejer\". Den lägger till metoden Medel.",
            Uppdrag = "Byt meddelandet på den commiten till:  Lägg till Medel\n" +
                      "Använd git rebase -i HEAD~3, byt pick mot reword (eller r) på rätt rad,\n" +
                      "spara och stäng. Skriv det nya meddelandet, spara och stäng igen.\n" +
                      "Innehållet i alla commits ska vara som förut.",
            Kommandon = new[] { "git log --oneline", "git rebase -i HEAD~3", "(byt pick mot reword, spara, stäng)", "(skriv nytt meddelande, spara, stäng)", "git log --oneline" },
            Bygg = repo => Historik(repo,
                ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme, ["Statistik.cs"] = Statistik() }),
                ("Lägg till Summa", "Du", new() { ["Statistik.cs"] = Statistik(Summa) }),
                ("fixade grejer", "Du", new() { ["Statistik.cs"] = Statistik(Summa, MedelRatt) }),
                ("Lägg till Storsta", "Du", new() { ["Statistik.cs"] = Statistik(Summa, MedelRatt, Storsta) }),
                ("Uppdatera README", "Du", new() { ["README.md"] = Readme + "Räknar summa, medel och största.\n" })),
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                Krav.Meddelanden(efter, fel, "Skapa projektet", "Lägg till Summa", "Lägg till Medel", "Lägg till Storsta", "Uppdatera README");
                Krav.OrordaForsta(fore, efter, 2, fel);
                SammaInnehallVarjeCommit(fore, efter, fel);
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 7,
            Titel = "Slå ihop småcommits",
            Niva = "Medel",
            Berattelse = "Sorteringen blev klar i fyra steg: \"Lägg till sortering\", \"WIP\", \"fix\" och \"fix igen\".\n" +
                         "Det ser rörigt ut i historiken.",
            Uppdrag = "Slå ihop WIP, fix och fix igen med \"Lägg till sortering\", så att historiken blir:\n" +
                      "  Skapa projektet,  Lägg till sortering,  Lägg till README\n" +
                      "Använd git rebase -i HEAD~5 och byt pick mot fixup (eller f) på de tre småcommitarna.\n" +
                      "Slutresultatet (filerna) ska vara precis som förut.",
            Kommandon = new[] { "git log --oneline", "git rebase -i HEAD~5", "(byt pick mot fixup på WIP, fix, fix igen)", "git log --oneline" },
            Bygg = repo => Historik(repo,
                ("Skapa projektet", "Läraren", new() { ["Sortering.cs"] = Sortering(0) }),
                ("Lägg till sortering", "Du", new() { ["Sortering.cs"] = Sortering(1) }),
                ("WIP", "Du", new() { ["Sortering.cs"] = Sortering(2) }),
                ("fix", "Du", new() { ["Sortering.cs"] = Sortering(3) }),
                ("fix igen", "Du", new() { ["Sortering.cs"] = Sortering(4) }),
                ("Lägg till README", "Du", new() { ["README.md"] = Readme + "Sorterar listor.\n" })),
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                Krav.Meddelanden(efter, fel, "Skapa projektet", "Lägg till sortering", "Lägg till README");
                Krav.OrordaForsta(fore, efter, 1, fel);
                var f = fore.Kedja();
                var e = efter.Kedja();
                if (e.Count == 3)
                {
                    Krav.SammaFiler(e[1], f[4].Filer, "Commiten \"Lägg till sortering\"", fel);
                    Krav.SammaFiler(e[2], f[5].Filer, "Commiten \"Lägg till README\"", fel);
                }
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 8,
            Titel = "Ta bort en commit som inte skulle vara med",
            Niva = "Medel",
            Berattelse = "Commiten \"Testkod – ska inte vara med\" lade till filen debug.txt av misstag.\n" +
                         "Commiten efter den är bra.",
            Uppdrag = "Ta bort hela commiten \"Testkod – ska inte vara med\" ur historiken.\n" +
                      "Använd git rebase -i HEAD~3 och byt pick mot drop (eller d) på den raden.\n" +
                      "debug.txt ska inte finnas i någon commit. Allt annat ska vara kvar.",
            Kommandon = new[] { "git log --oneline", "git rebase -i HEAD~3", "(byt pick mot drop)", "git log --oneline --stat" },
            Bygg = repo => Historik(repo,
                ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme, ["Kalkylator.cs"] = Kalkylator(Addera) }),
                ("Lägg till Subtrahera", "Du", new() { ["Kalkylator.cs"] = Kalkylator(Addera, Subtrahera) }),
                ("Testkod – ska inte vara med", "Du", new() { ["debug.txt"] = "x = 5\ny = 7\nTEST TEST TEST\n" }),
                ("Lägg till Multiplicera", "Du", new() { ["Kalkylator.cs"] = Kalkylator(Addera, Subtrahera, Multiplicera) })),
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                Krav.Meddelanden(efter, fel, "Skapa projektet", "Lägg till Subtrahera", "Lägg till Multiplicera");
                Krav.OrordaForsta(fore, efter, 2, fel);
                Krav.SammaFiler(efter.Topp(), Krav.Utan(fore.Topp()!.Filer, "debug.txt"), "Senaste commit", fel);
                if (efter.Kedja().Any(c => c.Filer.ContainsKey("debug.txt")))
                {
                    fel.Add("debug.txt finns fortfarande i någon commit.");
                }
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 9,
            Titel = "Ta bort ett lösenord ur historiken",
            Niva = "Svår",
            Berattelse = "I commit nummer 2, \"Lägg till inställningar\", råkade någon skriva ett riktigt lösenord\n" +
                         "i config.txt:  Losenord=hemligt123\n" +
                         "Även om man tar bort raden i en ny commit finns lösenordet kvar i historiken!",
            Uppdrag = "Ändra commit 2 så att raden blir  Losenord=  (tomt), och lösenordet försvinner\n" +
                      "ur ALLA commits. Meddelandena och allt annat ska vara som förut.\n" +
                      "  1. git rebase -i HEAD~4  och byt pick mot edit på \"Lägg till inställningar\"\n" +
                      "  2. Ändra config.txt\n" +
                      "  3. git add config.txt\n" +
                      "  4. git commit --amend --no-edit\n" +
                      "  5. git rebase --continue",
            Kommandon = new[] { "git rebase -i HEAD~4", "(byt pick mot edit)", "(ändra config.txt)", "git add config.txt", "git commit --amend --no-edit", "git rebase --continue" },
            Bygg = repo => Historik(repo,
                ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme }),
                ("Lägg till inställningar", "Kompisen", new() { ["config.txt"] = Config("hemligt123") }),
                ("Lägg till Addera", "Du", new() { ["Kalkylator.cs"] = Kalkylator(Addera) }),
                ("Lägg till Subtrahera", "Du", new() { ["Kalkylator.cs"] = Kalkylator(Addera, Subtrahera) }),
                ("Uppdatera README", "Du", new() { ["README.md"] = Readme + "Använder config.txt.\n" })),
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                Krav.Meddelanden(efter, fel, "Skapa projektet", "Lägg till inställningar", "Lägg till Addera", "Lägg till Subtrahera", "Uppdatera README");
                Krav.OrordaForsta(fore, efter, 1, fel);
                Krav.SammaFiler(efter.Topp(), Krav.Med(fore.Topp()!.Filer, "config.txt", Config("")), "Senaste commit", fel);
                if (efter.Commits.Any(c => c.Filer.Values.Any(v => v.Contains("hemligt123"))))
                {
                    fel.Add("Lösenordet hemligt123 finns fortfarande i någon commit på en gren.");
                }
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 10,
            Titel = "Återställ en raderad fil",
            Niva = "Medel",
            Berattelse = "Commiten \"Städa bort gamla filer\" raderade Recept.txt. Den behövdes!",
            Uppdrag = "Hämta tillbaka Recept.txt, precis som den såg ut innan den raderades,\n" +
                      "och committa den i en NY commit. Den gamla historiken ska vara orörd.\n" +
                      "Tips: git log --oneline -- Recept.txt  visar commits som rört filen.\n" +
                      "      git restore --source <id> Recept.txt  hämtar filen från en viss commit.",
            Kommandon = new[] { "git log --oneline -- Recept.txt", "git restore --source <id> Recept.txt", "git add Recept.txt", "git commit -m \"Återställ Recept.txt\"" },
            Bygg = repo => Historik(repo,
                ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme, ["Recept.txt"] = "Pannkakor\n" }),
                ("Lägg till ingredienser", "Du", new() { ["Recept.txt"] = ReceptFardigt }),
                ("Lägg till inköpslista", "Du", new() { ["Inkop.txt"] = "Mjöl\nMjölk\nÄgg\n" }),
                ("Städa bort gamla filer", "Kompisen", new() { ["Recept.txt"] = null! }),
                ("Uppdatera README", "Du", new() { ["README.md"] = Readme + "Recept och inköp.\n" })),
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                var ny = Krav.EnNyCommit(fore, efter, fel);
                Krav.SammaFiler(ny, Krav.Med(fore.Topp()!.Filer, "Recept.txt", ReceptFardigt), "Den nya commiten", fel);
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 11,
            Titel = "Plocka en commit från en annan gren",
            Niva = "Medel",
            Berattelse = "På grenen experiment finns tre commits. Två är halvfärdiga experiment,\n" +
                         "men \"Rätta avrundningen\" är en bra buggfix som behövs i main nu.",
            Uppdrag = "Kopiera BARA commiten \"Rätta avrundningen\" till main med git cherry-pick.\n" +
                      "Experimenten ska inte komma med, och grenen experiment ska vara orörd.",
            Kommandon = new[] { "git log --oneline --all --graph", "git log --oneline experiment", "git cherry-pick <id>", "git log --oneline" },
            Bygg = repo =>
            {
                Historik(repo,
                    ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme, ["Pris.cs"] = Pris(false, false) }));
                repo.Kor("switch", "-q", "-c", "experiment");
                Historik(repo,
                    ("Prova ny design", "Kompisen", new() { ["Design.txt"] = "Färg: lila\n" }),
                    ("Rätta avrundningen", "Kompisen", new() { ["Pris.cs"] = Pris(true, false) }),
                    ("Mer experiment", "Kompisen", new() { ["Design.txt"] = "Färg: lila\nTypsnitt: Comic Sans\n" }));
                repo.Kor("switch", "-q", "main");
                Historik(repo,
                    ("Lägg till MedMoms", "Du", new() { ["Pris.cs"] = Pris(false, true) }));
            },
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                var ny = Krav.EnNyCommit(fore, efter, fel);
                if (ny != null && ny.Meddelande != "Rätta avrundningen")
                {
                    fel.Add($"Den nya commiten på main ska heta \"Rätta avrundningen\". Nu heter den \"{ny.Meddelande}\".");
                }
                Krav.SammaFiler(ny, Krav.Med(fore.Topp()!.Filer, "Pris.cs", Pris(true, true)), "Den nya commiten", fel);
                if (efter.Grenar.GetValueOrDefault("experiment") != fore.Grenar["experiment"])
                {
                    fel.Add("Grenen experiment har ändrats. Den skulle vara orörd.");
                }
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 12,
            Titel = "Commits på fel gren",
            Niva = "Svår",
            Berattelse = "Du har gjort två commits på main (\"Påbörja export\" och \"Export till fil\"),\n" +
                         "men de skulle ha legat på en egen gren. Inget är pushat än.",
            Uppdrag = "Flytta de två commitarna till en ny gren som heter export, och flytta tillbaka main\n" +
                      "till \"Lägg till Addera\". Inget arbete får försvinna. Stå på main när du är klar.\n" +
                      "  1. git branch export          (en ny gren som pekar på din senaste commit)\n" +
                      "  2. git reset --hard HEAD~2    (flytta main två commits bakåt)",
            Kommandon = new[] { "git log --oneline", "git branch export", "git reset --hard HEAD~2", "git log --oneline --all --graph" },
            Bygg = repo => Historik(repo,
                ("Skapa projektet", "Läraren", new() { ["README.md"] = Readme }),
                ("Lägg till Addera", "Du", new() { ["Kalkylator.cs"] = Kalkylator(Addera) }),
                ("Påbörja export", "Du", new() { ["Export.cs"] = "// Export\n" }),
                ("Export till fil", "Du", new() { ["Export.cs"] = "// Export\n// Skriver till fil\n" })),
            Kontrollera = (fore, efter) =>
            {
                var fel = new List<string>();
                var f = fore.Kedja();
                if (efter.Grenar.GetValueOrDefault("export") != f[^1].Sha)
                {
                    fel.Add("Grenen export ska finnas och peka på din senaste commit, \"Export till fil\".");
                }
                if (efter.Grenar.GetValueOrDefault("main") != f[1].Sha)
                {
                    fel.Add("main ska peka på \"Lägg till Addera\" (två commits bakåt).");
                }
                return fel;
            },
        },
    };

    public static Scenario? Hitta(int nummer) => Alla.FirstOrDefault(s => s.Nummer == nummer);

    // ---------------------------------------------------------------------
    // Bygga historik
    // ---------------------------------------------------------------------

    /// <summary>
    /// Skapar en commit per steg. Filerna i varje steg skrivs (eller tas bort om innehållet är null).
    /// Repot skapas om det inte redan finns.
    /// </summary>
    private static void Historik(GitRepo repo, params (string Meddelande, string Person, Dictionary<string, string> Filer)[] steg)
    {
        if (!Directory.Exists(Path.Combine(repo.Mapp, ".git")))
        {
            repo.Skapa();
        }
        foreach (var (meddelande, person, filer) in steg)
        {
            foreach (var (fil, innehall) in filer)
            {
                if (innehall == null)
                {
                    repo.TaBort(fil);
                }
                else
                {
                    repo.Skriv(fil, innehall);
                }
            }
            repo.Commit(meddelande, person);
        }
    }

    private static void SammaInnehallVarjeCommit(Ogonblick fore, Ogonblick efter, List<string> fel)
    {
        var f = fore.Kedja();
        var e = efter.Kedja();
        for (int i = 0; i < Math.Min(f.Count, e.Count); i++)
        {
            Krav.SammaFiler(e[i], f[i].Filer, $"Commit {i + 1}", fel);
        }
    }

    public static void TaBortMapp(string mapp)
    {
        if (!Directory.Exists(mapp))
        {
            return;
        }
        foreach (var fil in Directory.EnumerateFiles(mapp, "*", SearchOption.AllDirectories))
        {
            File.SetAttributes(fil, FileAttributes.Normal);
        }
        Directory.Delete(mapp, recursive: true);
    }

    // ---------------------------------------------------------------------
    // Filinnehåll
    // ---------------------------------------------------------------------

    private const string Addera = "    public static int Addera(int a, int b)\n    {\n        return a + b;\n    }\n";
    private const string Subtrahera = "    public static int Subtrahera(int a, int b)\n    {\n        return a - b;\n    }\n";
    private const string Multiplicera = "    public static int Multiplicera(int a, int b)\n    {\n        return a * b;\n    }\n";

    private static string Kalkylator(params string[] metoder) =>
        "namespace Kalkyl;\n\npublic static class Kalkylator\n{\n" + string.Join("\n", metoder) + "}\n";

    private const string Summa = "    public static int Summa(int[] tal)\n    {\n        int summa = 0;\n        foreach (int t in tal)\n        {\n            summa += t;\n        }\n        return summa;\n    }\n";
    private const string MedelRatt = "    public static double Medel(int[] tal)\n    {\n        int summa = 0;\n        foreach (int t in tal)\n        {\n            summa += t;\n        }\n        return (double)summa / tal.Length;\n    }\n";
    private const string MedelFel = "    public static double Medel(int[] tal)\n    {\n        int summa = 0;\n        foreach (int t in tal)\n        {\n            summa += t;\n        }\n        return summa / tal.Length;\n    }\n";
    private const string Storsta = "    public static int Storsta(int[] tal)\n    {\n        int storsta = tal[0];\n        foreach (int t in tal)\n        {\n            if (t > storsta)\n            {\n                storsta = t;\n            }\n        }\n        return storsta;\n    }\n";
    private const string Minsta = "    public static int Minsta(int[] tal)\n    {\n        int minsta = tal[0];\n        foreach (int t in tal)\n        {\n            if (t < minsta)\n            {\n                minsta = t;\n            }\n        }\n        return minsta;\n    }\n";

    private static string Statistik(params string[] metoder) =>
        "namespace Kalkyl;\n\npublic static class Statistik\n{\n" + string.Join("\n", metoder) + "}\n";

    private static string Betyg(string jamforelse, bool betygsgranser, bool mvg) =>
        "namespace Kalkyl;\n\npublic static class Betyg\n{\n" +
        "    public static bool ArGodkand(int poang)\n    {\n" +
        $"        return poang {jamforelse} 50;\n    }}\n" +
        (betygsgranser ? "\n    public static int Maxpoang()\n    {\n        return 100;\n    }\n" : "") +
        (mvg ? "\n    public static bool ArMVG(int poang)\n    {\n        return poang >= 90;\n    }\n" : "") +
        "}\n";

    private static string Installningar(string rabatt) =>
        $"Valuta: SEK\nMoms: 25\nRabatt: {rabatt}\nSpråk: svenska\n";

    private static string Logg(int antal) =>
        string.Join("", Enumerable.Range(1, antal).Select(i => $"Uppdatering {i} klar\n"));

    private static string Sortering(int steg) => steg switch
    {
        0 => "namespace Kalkyl;\n\npublic static class Sortering\n{\n}\n",
        1 => "namespace Kalkyl;\n\npublic static class Sortering\n{\n    public static void Sortera(int[] tal)\n    {\n    }\n}\n",
        2 => "namespace Kalkyl;\n\npublic static class Sortering\n{\n    public static void Sortera(int[] tal)\n    {\n        for (int i = 0; i < tal.Length; i++)\n        {\n        }\n    }\n}\n",
        3 => "namespace Kalkyl;\n\npublic static class Sortering\n{\n    public static void Sortera(int[] tal)\n    {\n        for (int i = 0; i < tal.Length; i++)\n        {\n            for (int j = 0; j < tal.Length - 1; j++)\n            {\n            }\n        }\n    }\n}\n",
        _ => "namespace Kalkyl;\n\npublic static class Sortering\n{\n    public static void Sortera(int[] tal)\n    {\n        for (int i = 0; i < tal.Length; i++)\n        {\n            for (int j = 0; j < tal.Length - 1 - i; j++)\n            {\n                if (tal[j] > tal[j + 1])\n                {\n                    (tal[j], tal[j + 1]) = (tal[j + 1], tal[j]);\n                }\n            }\n        }\n    }\n}\n",
    };

    private static string Config(string losenord) =>
        $"Server=skolan.local\nAnvandare=admin\nLosenord={losenord}\nTimeout=30\n";

    private const string ReceptFardigt = "Pannkakor\n2,5 dl mjöl\n6 dl mjölk\n3 ägg\n1 krm salt\n";

    private const string TesterMd = "# Tester\n- Multiplicera(3, 4) ska bli 12\n- Multiplicera(0, 5) ska bli 0\n";

    private static string Pris(bool rattAvrundning, bool medMoms) =>
        "namespace Kalkyl;\n\npublic static class Pris\n{\n" +
        "    public static double Avrunda(double pris)\n    {\n" +
        (rattAvrundning
            ? "        return Math.Round(pris, MidpointRounding.AwayFromZero);\n"
            : "        return Math.Round(pris);\n") +
        "    }\n" +
        "\n    // Fler metoder nedan\n" +
        (medMoms ? "\n    public static double MedMoms(double pris)\n    {\n        return pris * 1.25;\n    }\n" : "") +
        "}\n";
}
