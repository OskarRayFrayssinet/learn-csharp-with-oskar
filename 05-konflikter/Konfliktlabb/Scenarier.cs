using System.Text.RegularExpressions;

namespace Konfliktlabb;

/// <summary>Alla scenarier i Konfliktlabbet, från lätt till svårt.</summary>
public static class Scenarier
{
    public static IReadOnlyList<Scenario> Alla { get; } = new List<Scenario>
    {
        // -----------------------------------------------------------------
        new()
        {
            Nummer = 1,
            Titel = "Samma rad – välj kompisens",
            Niva = "Lätt",
            Berattelse =
                "Du och din kompis har ändrat matsedeln för tisdag, men på olika sätt.\n" +
                "Du skrev Pannkakor. Kompisen skrev Tacos på grenen 'kompis'.",
            Uppdrag =
                "Slå ihop grenen kompis med main. Kompisen hade rätt: det blir tacos.\n" +
                "meny.txt ska till slut innehålla exakt:\n\n" +
                "    Måndag: Pasta\n    Tisdag: Tacos\n    Onsdag: Fisk",
            Kommandon = new[] { "git merge kompis", "(rätta meny.txt)", "git add meny.txt", "git commit --no-edit" },
            Filer = new[] { "meny.txt" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new() { ["meny.txt"] = "Måndag: Pasta\nTisdag: Soppa\nOnsdag: Fisk\n" },
                kompis: r => r.Skriv("meny.txt", "Måndag: Pasta\nTisdag: Tacos\nOnsdag: Fisk\n"),
                du: r => r.Skriv("meny.txt", "Måndag: Pasta\nTisdag: Pannkakor\nOnsdag: Fisk\n"),
                kompisMeddelande: "Tacos på tisdag", duMeddelande: "Pannkakor på tisdag"),
            KontrolleraInnehall = f => Regler.ExaktaRader(f, "meny.txt", "Måndag: Pasta", "Tisdag: Tacos", "Onsdag: Fisk"),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 2,
            Titel = "Samma rad – behåll din egen",
            Niva = "Lätt",
            Berattelse =
                "Ni har båda ändrat temat i inställningarna.\n" +
                "Du valde mörkt. Kompisen valde blått.",
            Uppdrag =
                "Slå ihop kompis med main. Den här gången ska DIN version gälla.\n" +
                "installningar.txt ska till slut innehålla exakt:\n\n" +
                "    Tema: mörkt\n    Språk: svenska\n    Textstorlek: 12",
            Kommandon = new[] { "git merge kompis", "(rätta installningar.txt)", "git add installningar.txt", "git commit --no-edit" },
            Filer = new[] { "installningar.txt" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new() { ["installningar.txt"] = "Tema: ljust\nSpråk: svenska\nTextstorlek: 12\n" },
                kompis: r => r.Skriv("installningar.txt", "Tema: blått\nSpråk: svenska\nTextstorlek: 12\n"),
                du: r => r.Skriv("installningar.txt", "Tema: mörkt\nSpråk: svenska\nTextstorlek: 12\n"),
                kompisMeddelande: "Blått tema", duMeddelande: "Mörkt tema"),
            KontrolleraInnehall = f => Regler.ExaktaRader(f, "installningar.txt", "Tema: mörkt", "Språk: svenska", "Textstorlek: 12"),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 3,
            Titel = "Båda ändringarna behövs",
            Niva = "Lätt",
            Berattelse =
                "Ni har båda lagt till en ny deltagare sist i listan.\n" +
                "Du lade till Cem. Kompisen lade till Dana.",
            Uppdrag =
                "Slå ihop kompis med main. BÅDA namnen ska vara kvar.\n" +
                "deltagare.txt ska innehålla fyra namn, ett per rad:\n" +
                "Ali och Bea först, sedan Cem och Dana (i valfri ordning).",
            Kommandon = new[] { "git merge kompis", "(behåll båda namnen i deltagare.txt)", "git add deltagare.txt", "git commit --no-edit" },
            Filer = new[] { "deltagare.txt" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new() { ["deltagare.txt"] = "Ali\nBea\n" },
                kompis: r => r.Skriv("deltagare.txt", "Ali\nBea\nDana\n"),
                du: r => r.Skriv("deltagare.txt", "Ali\nBea\nCem\n"),
                kompisMeddelande: "Lägg till Dana", duMeddelande: "Lägg till Cem"),
            KontrolleraInnehall = f =>
            {
                var fel = new List<string>();
                var rader = Regler.Rader(f.GetValueOrDefault("deltagare.txt")).Where(r => r.Length > 0).ToList();
                var ratt = rader.Count == 4 && rader[0] == "Ali" && rader[1] == "Bea"
                    && rader.Skip(2).OrderBy(r => r).SequenceEqual(new[] { "Cem", "Dana" });
                if (!ratt)
                {
                    fel.Add($"deltagare.txt ska innehålla Ali, Bea, Cem och Dana (ett namn per rad). Nu står det: {string.Join(", ", rader)}");
                }
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 4,
            Titel = "Två konflikter i samma fil",
            Niva = "Medel",
            Berattelse =
                "Ni har båda ändrat pannkaksreceptet på två ställen: mjölet och smöret.\n" +
                "Du skrev 'Mjöl: 3 dl' och 'Smör: 1 msk'.\n" +
                "Kompisen skrev 'Mjöl: 2,5 dl' och 'Smör: till stekning, gärna mycket'.",
            Uppdrag =
                "Slå ihop kompis med main. Det blir TVÅ konfliktblock i samma fil.\n" +
                "Mjölet ska komma från kompisen och smöret från dig:\n\n" +
                "    Pannkakor\n    Mjöl: 2,5 dl\n    Mjölk: 4 dl\n    Ägg: 2\n    Salt: en nypa\n    Smör: 1 msk",
            Kommandon = new[] { "git merge kompis", "(lös båda konflikterna i recept.txt)", "git add recept.txt", "git commit --no-edit" },
            Filer = new[] { "recept.txt" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new() { ["recept.txt"] = "Pannkakor\nMjöl: 2 dl\nMjölk: 4 dl\nÄgg: 2\nSalt: en nypa\nSmör: till stekning\n" },
                kompis: r => r.Skriv("recept.txt", "Pannkakor\nMjöl: 2,5 dl\nMjölk: 4 dl\nÄgg: 2\nSalt: en nypa\nSmör: till stekning, gärna mycket\n"),
                du: r => r.Skriv("recept.txt", "Pannkakor\nMjöl: 3 dl\nMjölk: 4 dl\nÄgg: 2\nSalt: en nypa\nSmör: 1 msk\n"),
                kompisMeddelande: "Justera mjöl och smör", duMeddelande: "Mer mjöl, mindre smör"),
            KontrolleraInnehall = f => Regler.ExaktaRader(f, "recept.txt",
                "Pannkakor", "Mjöl: 2,5 dl", "Mjölk: 4 dl", "Ägg: 2", "Salt: en nypa", "Smör: 1 msk"),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 5,
            Titel = "Konflikt i C#-kod – välj den som räknar rätt",
            Niva = "Medel",
            Berattelse =
                "Metoden MedMoms räknade fel (pris * 1.2). Ni försökte båda rätta den.\n" +
                "Du skrev: return pris * 1.25;\n" +
                "Kompisen skrev: return pris + 25;",
            Uppdrag =
                "Slå ihop kompis med main. Momsen är 25 %, så MedMoms(100) ska bli 125\n" +
                "och MedMoms(200) ska bli 250. Behåll den version som räknar rätt för ALLA priser.",
            Kommandon = new[] { "git merge kompis", "(välj rätt rad i Pris.cs)", "git add Pris.cs", "git commit --no-edit" },
            Filer = new[] { "Pris.cs" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new() { ["Pris.cs"] = PrisKod("pris * 1.2") },
                kompis: r => r.Skriv("Pris.cs", PrisKod("pris + 25")),
                du: r => r.Skriv("Pris.cs", PrisKod("pris * 1.25")),
                kompisMeddelande: "Rätta momsen", duMeddelande: "Rätta momsen till 25 %"),
            KontrolleraInnehall = f => Regler.SammaKod(f, "Pris.cs", PrisKod("pris * 1.25")),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 6,
            Titel = "Kombinera två ändringar till en",
            Niva = "Medel",
            Berattelse =
                "Metoden Summa ska räkna 1 + 2 + ... + n.\n" +
                "Du döpte om variabeln x till summa, så att koden blev tydligare.\n" +
                "Kompisen rättade en bugg: loopen gick ett varv för lite (i < n ska vara i <= n).",
            Uppdrag =
                "Slå ihop kompis med main. Ingen av versionerna i konflikten är helt rätt.\n" +
                "Du måste KOMBINERA dem: variabeln ska heta summa OCH loopen ska ha i <= n.",
            Kommandon = new[] { "git merge kompis", "(skriv en rad som kombinerar båda ändringarna)", "git add Summa.cs", "git commit --no-edit" },
            Filer = new[] { "Summa.cs" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new() { ["Summa.cs"] = SummaKod("x", "<") },
                kompis: r => r.Skriv("Summa.cs", SummaKod("x", "<=")),
                du: r => r.Skriv("Summa.cs", SummaKod("summa", "<")),
                kompisMeddelande: "Rätta loopen", duMeddelande: "Byt namn på x till summa"),
            KontrolleraInnehall = f => Regler.SammaKod(f, "Summa.cs", SummaKod("summa", "<=")),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 7,
            Titel = "Konflikter i två filer",
            Niva = "Medel",
            Berattelse =
                "Ni har båda ändrat i lag.txt och poang.txt.\n" +
                "I lag.txt bytte du målvakt till Sam. Kompisen bytte till Kim.\n" +
                "I poang.txt skrev du 'Vinster: 4'. Kompisen skrev 'Vinster: 5'.",
            Uppdrag =
                "Slå ihop kompis med main. Det blir konflikt i BÅDA filerna.\n" +
                "  - lag.txt: din version gäller (Målvakt: Sam)\n" +
                "  - poang.txt: kompisens version gäller (Vinster: 5)\n" +
                "Tips: git status visar vilka filer som har konflikter.",
            Kommandon = new[] { "git merge kompis", "git status", "(lös båda filerna)", "git add lag.txt poang.txt", "git commit --no-edit" },
            Filer = new[] { "lag.txt", "poang.txt" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new()
                {
                    ["lag.txt"] = "Lag: Blixtarna\nMålvakt: Alex\nKapten: Robin\n",
                    ["poang.txt"] = "Matcher: 6\nVinster: 3\nFörluster: 3\n",
                },
                kompis: r =>
                {
                    r.Skriv("lag.txt", "Lag: Blixtarna\nMålvakt: Kim\nKapten: Robin\n");
                    r.Skriv("poang.txt", "Matcher: 6\nVinster: 5\nFörluster: 3\n");
                },
                du: r =>
                {
                    r.Skriv("lag.txt", "Lag: Blixtarna\nMålvakt: Sam\nKapten: Robin\n");
                    r.Skriv("poang.txt", "Matcher: 6\nVinster: 4\nFörluster: 3\n");
                },
                kompisMeddelande: "Ny målvakt och fler vinster", duMeddelande: "Ny målvakt och uppdaterade vinster"),
            KontrolleraInnehall = f =>
                Regler.ExaktaRader(f, "lag.txt", "Lag: Blixtarna", "Målvakt: Sam", "Kapten: Robin")
                    .Concat(Regler.ExaktaRader(f, "poang.txt", "Matcher: 6", "Vinster: 5", "Förluster: 3")).ToList(),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 8,
            Titel = "Raderad hos dig, ändrad hos kompisen",
            Niva = "Medel",
            Berattelse =
                "Du trodde att anteckningar.txt var onödig och raderade den.\n" +
                "Men kompisen har just lagt till en viktig rad i samma fil.",
            Uppdrag =
                "Slå ihop kompis med main. Git kan inte veta om filen ska raderas eller behållas.\n" +
                "Filen behövs! anteckningar.txt ska finnas kvar med kompisens innehåll.\n" +
                "Tips: git status säger 'deleted by us'. Använd git add för att BEHÅLLA filen\n" +
                "(git rm hade raderat den).",
            Kommandon = new[] { "git merge kompis", "git status", "git add anteckningar.txt", "git commit --no-edit" },
            Filer = new[] { "anteckningar.txt" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new() { ["anteckningar.txt"] = "Möte på måndag\nTa med dator\n", ["README.txt"] = "Projektets filer\n" },
                kompis: r => r.Skriv("anteckningar.txt", "Möte på måndag\nTa med dator\nVIKTIGT: Inlämning fredag kl 15\n"),
                du: r => r.TaBort("anteckningar.txt"),
                kompisMeddelande: "Lägg till inlämningsdatum", duMeddelande: "Ta bort gamla anteckningar"),
            KontrolleraInnehall = f => Regler.ExaktaRader(f, "anteckningar.txt",
                "Möte på måndag", "Ta med dator", "VIKTIGT: Inlämning fredag kl 15"),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 9,
            Titel = "Ni skapade samma fil",
            Niva = "Medel",
            Berattelse =
                "Ni skapade båda en ny fil som heter regler.txt, utan att veta om varandra.\n" +
                "Båda skrev 'Regel 1: Var snäll' som första regel, men olika andraregler.",
            Uppdrag =
                "Slå ihop kompis med main. Gör en gemensam lista med alla tre reglerna:\n\n" +
                "    Regel 1: Var snäll\n    Regel 2: Lyssna på varandra\n    Regel 3: Hjälp varandra\n\n" +
                "(Din regel blir nummer 2 och kompisens blir nummer 3.)",
            Kommandon = new[] { "git merge kompis", "(skriv ihop reglerna i regler.txt)", "git add regler.txt", "git commit --no-edit" },
            Filer = new[] { "regler.txt" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new() { ["README.txt"] = "Klassens gemensamma filer\n" },
                kompis: r => r.Skriv("regler.txt", "Regel 1: Var snäll\nRegel 2: Hjälp varandra\n"),
                du: r => r.Skriv("regler.txt", "Regel 1: Var snäll\nRegel 2: Lyssna på varandra\n"),
                kompisMeddelande: "Skapa regler", duMeddelande: "Skapa regler"),
            KontrolleraInnehall = f => Regler.ExaktaRader(f, "regler.txt",
                "Regel 1: Var snäll", "Regel 2: Lyssna på varandra", "Regel 3: Hjälp varandra"),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 10,
            Titel = "Avbryt en merge",
            Niva = "Medel",
            AvbrytMerge = true,
            Berattelse =
                "Grenen 'experiment' innehåller halvfärdiga idéer som inte ska in i main än.\n" +
                "Men du råkar slå ihop den ändå...",
            Uppdrag =
                "1. Kör git merge experiment och se att det blir konflikt.\n" +
                "2. Inse att det var fel gren, och AVBRYT mergen med git merge --abort.\n" +
                "Allt ska vara precis som innan du började: ingen merge, inga ändringar.",
            Kommandon = new[] { "git merge experiment", "git status", "git merge --abort", "git status" },
            Filer = new[] { "plan.txt" },
            Bygg = (repo, _) =>
            {
                repo.Skapa();
                repo.Skriv("plan.txt", "Vecka 1: Planera\nVecka 2: Bygga\nVecka 3: Testa\n");
                repo.Commit("Start", "Läraren");
                repo.Kor("switch", "-q", "-c", "experiment");
                repo.Skriv("plan.txt", "Vecka 1: Skippa planeringen!\nVecka 2: Bygga allt på en gång\nVecka 3: Hoppas på det bästa\n");
                repo.Commit("Vilda idéer", "Kompisen");
                repo.Kor("switch", "-q", "main");
                repo.Skriv("plan.txt", "Vecka 1: Planera noga\nVecka 2: Bygga\nVecka 3: Testa\n");
                repo.Commit("Förtydliga planen", "Du");
                return null;
            },
            KontrolleraInnehall = f => Regler.ExaktaRader(f, "plan.txt", "Vecka 1: Planera noga", "Vecka 2: Bygga", "Vecka 3: Testa"),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 11,
            Titel = "Hämta från läraren (pull från upstream)",
            Niva = "Svår",
            Berattelse =
                "Precis som i kursen finns det en remote som heter 'upstream' (lärarens repo).\n" +
                "Du har svarat på uppgiften i uppgift.txt och committat.\n" +
                "Samtidigt har läraren förtydligat frågan på raden ovanför ditt svar.",
            Uppdrag =
                "Hämta lärarens ändring med git pull --no-rebase upstream main.\n" +
                "Eftersom ni ändrat rader som ligger precis bredvid varandra blir det konflikt.\n" +
                "Behåll lärarens nya fråga OCH ditt svar. uppgift.txt ska bli:\n\n" +
                "    Uppgift 1: Vad är 6 gånger 7? (Räkna i huvudet.)\n    Svar: 42",
            Kommandon = new[] { "git remote -v", "git pull --no-rebase upstream main", "(lös uppgift.txt)", "git add uppgift.txt", "git commit --no-edit" },
            Filer = new[] { "uppgift.txt" },
            TipsSammanslagning = "Du har inte hämtat och slagit ihop lärarens ändring. Kör: git pull --no-rebase upstream main",
            Bygg = (repo, labb) =>
            {
                var server = Path.Combine(labb, "scenario-11-lararens-repo.git");
                var larare = new GitRepo(Path.Combine(labb, ".tmp-scenario-11"));
                TaBortMapp(server);
                TaBortMapp(larare.Mapp);

                new GitRepo(labb).Kor("init", "-q", "--bare", "-b", "main", server);
                larare.Skapa();
                larare.Skriv("uppgift.txt", "Uppgift 1: Vad är 6 gånger 7?\nSvar:\n");
                larare.Commit("Ny uppgift", "Läraren");
                larare.Kor("remote", "add", "origin", server);
                larare.Kor("push", "-q", "origin", "main");

                repo.Skapa();
                repo.Kor("remote", "add", "upstream", server);
                repo.Kor("fetch", "-q", "upstream");
                repo.Kor("reset", "-q", "--hard", "upstream/main");
                repo.Skriv("uppgift.txt", "Uppgift 1: Vad är 6 gånger 7?\nSvar: 42\n");
                repo.Commit("Svar på uppgift 1", "Du");

                larare.Skriv("uppgift.txt", "Uppgift 1: Vad är 6 gånger 7? (Räkna i huvudet.)\nSvar:\n");
                larare.Commit("Förtydliga uppgift 1", "Läraren");
                larare.Kor("push", "-q", "origin", "main");
                var mal = larare.Sha("HEAD");
                TaBortMapp(larare.Mapp);
                return mal;
            },
            KontrolleraInnehall = f => Regler.ExaktaRader(f, "uppgift.txt",
                "Uppgift 1: Vad är 6 gånger 7? (Räkna i huvudet.)", "Svar: 42"),
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 12,
            Titel = "Tre konflikter i C# – tänk själv",
            Niva = "Svår",
            Berattelse =
                "Kalkylator.cs hade buggar i alla tre metoderna. Ni rättade dem båda,\n" +
                "men inte alltid rätt. Det blir tre konfliktblock.",
            Uppdrag =
                "Slå ihop kompis med main. För VARJE konflikt: läs båda versionerna och\n" +
                "behåll den som räknar rätt:\n" +
                "  - Addera(2, 3) ska bli 5\n" +
                "  - Subtrahera(5, 3) ska bli 2\n" +
                "  - Multiplicera(4, 3) ska bli 12\n" +
                "Ibland har du rätt, ibland kompisen.",
            Kommandon = new[] { "git merge kompis", "(lös alla tre konflikterna)", "git add Kalkylator.cs", "git commit --no-edit" },
            Filer = new[] { "Kalkylator.cs" },
            Bygg = (repo, _) => DuOchKompis(repo,
                bas: new() { ["Kalkylator.cs"] = KalkylatorKod("a - b", "a + b", "a + b") },
                kompis: r => r.Skriv("Kalkylator.cs", KalkylatorKod("a * b", "a - b", "b * a")),
                du: r => r.Skriv("Kalkylator.cs", KalkylatorKod("a + b", "b - a", "a * b")),
                kompisMeddelande: "Rätta kalkylatorn", duMeddelande: "Rätta kalkylatorn"),
            KontrolleraInnehall = f =>
            {
                var fel = new List<string>();
                var kod = f.GetValueOrDefault("Kalkylator.cs") ?? "";
                KollaReturn(kod, "Addera", new[] { "a + b", "b + a" }, "Addera(2, 3) ska bli 5", fel);
                KollaReturn(kod, "Subtrahera", new[] { "a - b" }, "Subtrahera(5, 3) ska bli 2", fel);
                KollaReturn(kod, "Multiplicera", new[] { "a * b", "b * a" }, "Multiplicera(4, 3) ska bli 12", fel);
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 13,
            Titel = "Delat repo – din push nekas",
            Niva = "Svår",
            KraverPush = true,
            Berattelse =
                "Du och Robotkompisen delar ett repo på en server (remoten heter origin).\n" +
                "Ni ska båda lägga till en vara sist i inkopslista.txt.\n" +
                "Robotkompisen har REDAN lagt till Bröd och pushat.",
            Uppdrag =
                "1. Lägg till raden Mjölk sist i inkopslista.txt. Committa.\n" +
                "2. Försök pusha med git push. Det nekas (rejected), eftersom servern har något du saknar.\n" +
                "3. Hämta med git pull --no-rebase. Det blir konflikt.\n" +
                "4. Behåll BÅDA varorna, committa och pusha igen.\n" +
                "Listan på servern ska bli: Ägg, Smör, och sedan Bröd och Mjölk (valfri ordning).",
            Kommandon = new[] { "(lägg till Mjölk)", "git commit -am \"Lägg till mjölk\"", "git push", "git pull --no-rebase", "(lös konflikten)", "git add inkopslista.txt", "git commit --no-edit", "git push" },
            Filer = new[] { "inkopslista.txt" },
            TipsSammanslagning = "Du har inte hämtat Robotkompisens ändring. Kör: git pull --no-rebase",
            Bygg = (repo, labb) => DelatRepo(repo, labb, 13,
                bas: "Ägg\nSmör\n",
                robot: "Ägg\nSmör\nBröd\n",
                robotMeddelande: "Lägg till bröd"),
            KontrolleraInnehall = f =>
            {
                var fel = new List<string>();
                var rader = Regler.Rader(f.GetValueOrDefault("inkopslista.txt")).Where(r => r.Length > 0).ToList();
                var ratt = rader.Count == 4 && rader[0] == "Ägg" && rader[1] == "Smör"
                    && rader.Skip(2).OrderBy(r => r).SequenceEqual(new[] { "Bröd", "Mjölk" });
                if (!ratt)
                {
                    fel.Add($"inkopslista.txt ska innehålla Ägg, Smör, Bröd och Mjölk (en vara per rad). Nu står det: {string.Join(", ", rader)}");
                }
                return fel;
            },
        },

        // -----------------------------------------------------------------
        new()
        {
            Nummer = 14,
            Titel = "Delat repo – kombinera med Robotkompisens kod",
            Niva = "Svår",
            KraverPush = true,
            Berattelse =
                "Ni delar repot igen. I Halsning.cs står: return \"Hej \" + namn;\n" +
                "Robotkompisen har lagt till ett utropstecken och pushat: return \"Hej \" + namn + \"!\";",
            Uppdrag =
                "1. Skriv om raden med string interpolation: return $\"Hej {namn}\"; Committa.\n" +
                "2. Pusha. Det nekas. Hämta med git pull --no-rebase. Det blir konflikt.\n" +
                "3. Kombinera: använd interpolation OCH behåll utropstecknet:\n\n" +
                "       return $\"Hej {namn}!\";\n\n" +
                "4. Committa och pusha.",
            Kommandon = new[] { "(ändra raden)", "git commit -am \"Använd interpolation\"", "git push", "git pull --no-rebase", "(kombinera)", "git add Halsning.cs", "git commit --no-edit", "git push" },
            Filer = new[] { "Halsning.cs" },
            TipsSammanslagning = "Du har inte hämtat Robotkompisens ändring. Kör: git pull --no-rebase",
            Bygg = (repo, labb) => DelatRepo(repo, labb, 14,
                bas: HalsningKod("\"Hej \" + namn"),
                robot: HalsningKod("\"Hej \" + namn + \"!\""),
                robotMeddelande: "Lägg till utropstecken"),
            KontrolleraInnehall = f => Regler.SammaKod(f, "Halsning.cs", HalsningKod("$\"Hej {namn}!\"")),
        },
    };

    public static Scenario? Hitta(int nummer) => Alla.FirstOrDefault(s => s.Nummer == nummer);

    // ---------------------------------------------------------------------
    // Hjälpmetoder för att bygga scenarierna
    // ---------------------------------------------------------------------

    private static string DuOchKompis(GitRepo repo, Dictionary<string, string> bas,
        Action<GitRepo> kompis, Action<GitRepo> du, string kompisMeddelande, string duMeddelande)
    {
        repo.Skapa();
        foreach (var (fil, innehall) in bas)
        {
            repo.Skriv(fil, innehall);
        }
        repo.Commit("Start", "Läraren");

        repo.Kor("switch", "-q", "-c", "kompis");
        kompis(repo);
        repo.Commit(kompisMeddelande, "Kompisen");
        var mal = repo.Sha("HEAD");

        repo.Kor("switch", "-q", "main");
        du(repo);
        repo.Commit(duMeddelande, "Du");
        return mal;
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

    /// <summary>
    /// Bygger ett delat repo (en "server") där Robotkompisen redan har pushat en ändring
    /// som eleven inte har hämtat.
    /// </summary>
    private static string DelatRepo(GitRepo repo, string labb, int nummer, string bas, string robot, string robotMeddelande)
    {
        var server = Path.Combine(labb, $"scenario-{nummer:00}-delat-repo.git");
        var robotRepo = new GitRepo(Path.Combine(labb, $".tmp-scenario-{nummer:00}"));
        TaBortMapp(server);
        TaBortMapp(robotRepo.Mapp);

        var fil = nummer == 13 ? "inkopslista.txt" : "Halsning.cs";
        new GitRepo(labb).Kor("init", "-q", "--bare", "-b", "main", server);
        robotRepo.Skapa();
        robotRepo.Skriv(fil, bas);
        robotRepo.Commit("Start", "Läraren");
        robotRepo.Kor("remote", "add", "origin", server);
        robotRepo.Kor("push", "-q", "origin", "main");

        repo.Skapa();
        repo.Kor("remote", "add", "origin", server);
        repo.Kor("fetch", "-q", "origin");
        repo.Kor("reset", "-q", "--hard", "origin/main");
        repo.Kor("branch", "-q", "--set-upstream-to=origin/main");

        robotRepo.Skriv(fil, robot);
        robotRepo.Commit(robotMeddelande, "Robotkompisen");
        robotRepo.Kor("push", "-q", "origin", "main");
        var mal = robotRepo.Sha("HEAD");
        TaBortMapp(robotRepo.Mapp);
        return mal;
    }

    private static string HalsningKod(string uttryck) =>
        "public static class Halsning\n{\n    public static string Halsa(string namn)\n    {\n" +
        $"        return {uttryck};\n" +
        "    }\n}\n";

    private static string PrisKod(string uttryck) =>
        "public static class Pris\n{\n    public static double MedMoms(double pris)\n    {\n" +
        $"        return {uttryck};\n" +
        "    }\n}\n";

    private static string SummaKod(string namn, string jamforelse) =>
        "public static class Rakna\n{\n    public static int Summa(int n)\n    {\n" +
        $"        int {namn} = 0;\n" +
        $"        for (int i = 1; i {jamforelse} n; i++)\n" +
        "        {\n" +
        $"            {namn} += i;\n" +
        "        }\n" +
        $"        return {namn};\n" +
        "    }\n}\n";

    private static string KalkylatorKod(string addera, string subtrahera, string multiplicera) =>
        "public static class Kalkylator\n{\n" +
        "    public static int Addera(int a, int b)\n    {\n" +
        $"        return {addera};\n    }}\n\n" +
        "    public static int Subtrahera(int a, int b)\n    {\n" +
        $"        return {subtrahera};\n    }}\n\n" +
        "    public static int Multiplicera(int a, int b)\n    {\n" +
        $"        return {multiplicera};\n    }}\n" +
        "}\n";

    private static void KollaReturn(string kod, string metod, string[] godkanda, string krav, List<string> fel)
    {
        var traff = Regex.Match(kod, $@"{metod}\s*\(int a, int b\)\s*\{{\s*return\s+([^;]+);");
        if (!traff.Success)
        {
            fel.Add($"Hittade inte metoden {metod} med en return-rad i Kalkylator.cs.");
        }
        else if (!godkanda.Contains(Regex.Replace(traff.Groups[1].Value.Trim(), @"\s+", " ")))
        {
            fel.Add($"{metod} räknar fel: return {traff.Groups[1].Value.Trim()}; ({krav}).");
        }
    }
}
