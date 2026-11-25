# Lär dig C# med Oskar

Övningar i C# som rättas automatiskt. Du skriver koden, pushar till GitHub
och får direkt veta vilka tester som är godkända.

## Börja här

> 📊 **Din dashboard:** dubbelklicka på **`dashboard.cmd`** i repomappen (eller kör
> `dotnet run --project verktyg/Dashboard`). Den bygger och testar din kod och öppnar en
> sida som visar hur långt du har kommit, vad du ska göra härnäst, tips om din kod och en
> **uppslagsbok** med sökfunktion. Allt finns på din dator och fungerar utan internet.

1. **Läs [Så jobbar du i kursen](guider/sa-jobbar-du.md).** Den förklarar
   hur allt hänger ihop och vad du gör varje lektion.
2. **Gör [Kom igång](#kom-igång-görs-en-gång)** längre ner på den här sidan (en gång).
3. **Börja med övning 01.**

Så här ser varje lektion ut:

```
 Hämta nytt  →  Läs  →  Skriv kod  →  Provkör  →  Testa  →  Dashboard  →  Commit  →  Push
 från läraren   uppgift              (▶)        (Test     (framsteg     (spara)    (skicka,
                + guide                          Explorer)  och tips)               rättning)
```

## Användbara kommandon

Kör kommandona i PowerShell i repots rotmapp
(`cd C:\github-repos\learn-csharp-with-oskar`). Behöver du slå upp
något annat, använd **[Uppslagsboken](uppslagsbok.html)**.

### Varje lektion
```powershell
git fetch upstream                              # finns det något nytt från läraren?
git log --oneline HEAD..upstream/main           # vad är nytt? (tomt = inget)
git pull --no-rebase --no-edit upstream main    # hämta det nya
start learn-csharp-with-oskar.sln               # öppna i Visual Studio  (VS Code: code .)
git add .                                       # välj alla ändringar
git commit -m "Löste uppgift 3"                 # spara en version
git push                                        # skicka till din fork, rättningen startar
.\dashboard.cmd                                 # visa din progression (fungerar offline)
```

### Git
| Kommando | Gör |
|---|---|
| `git status` | Vad har ändrats? Vad är nästa steg? **Kör den ofta.** |
| `git log --oneline --graph` | Visa historiken |
| `git diff` | Visa exakt vilka rader du har ändrat |
| `git restore <fil>` | Ångra ändringar i en fil som inte är committade |
| `git commit --amend --no-edit` | Lägg till glömda ändringar i senaste commit (före push) |
| `git merge --abort` | Avbryt en merge med konflikter |
| `git remote -v` | Visa vart `origin` och `upstream` pekar |

### .NET
| Kommando | Gör |
|---|---|
| `dotnet build` | Bygg allt och visa fel |
| `dotnet test` | Kör alla tester |
| `dotnet test --filter "DisplayName~01 ·"` | Kör bara testerna för övning 01 |
| `dotnet run --project 04-traning/Traning.Provkor` | Kör ett Provkör-program |
| `dotnet format 01-grunder/Grunder` | Rätta formatering och kodstil automatiskt |

### Labbarna
| Kommando | Gör |
|---|---|
| `dotnet run --project 05-konflikter/Konfliktlabb` | Öva på merge-konflikter (meny) |
| `dotnet run --project 07-historik/Historiklabb` | Öva på att rätta gammal historik (meny) |
| `.\kontrollera.cmd` | Kontrollera ditt scenario (i scenariots mapp) |

### Visual Studio
| Tangent | Gör |
|---|---|
| **Ctrl+Shift+B** | Bygg |
| **F5** / **Ctrl+F5** | Kör med / utan debugger |
| **Ctrl+R, A** | Kör alla tester |
| **F9** | Brytpunkt |
| **Ctrl+K, Ctrl+D** | Formatera filen |
| **Ctrl+.** | Snabbfix (glödlampan) |

## Övningar

| Nr | Övning | Läs först |
|---|---|---|
| 01 | [Grunder – villkor, loopar och arrayer](01-grunder) | [Variabler](guider/variabler-och-datatyper.md), [Villkor och loopar](guider/villkor-och-loopar.md), [Metoder](guider/metoder.md) |
| 02 | [Sortera en lista](02-sortera-lista) | [Villkor och loopar](guider/villkor-och-loopar.md), [Testkör](guider/testkora.md) |
| 03 | [Filstrukturen i C#](03-filstruktur) | [Filstrukturen](guider/filstruktur.md), [Recept: skapa projekt](guider/skapa-projekt.md) |
| 04 | [Träning – 40 små uppgifter](04-traning) | [Variabler](guider/variabler-och-datatyper.md), [Villkor och loopar](guider/villkor-och-loopar.md) |
| 05 | [Merge-konflikter](05-konflikter) | [Merge-konflikter](guider/merge-konflikter.md), [Git och GitHub](guider/git-och-github.md) |
| 06 | [Versionering](06-versionering) | [Versionering](guider/versionering.md) |
| 07 | [Rätta gammal historik](07-historik) | [Ändra och ångra historik](guider/andra-historik.md) |

## Guider

Alla guider finns i **[guider/](guider/README.md)**. Läs de fem första innan du börjar:

| Guide | Handlar om |
|---|---|
| [Så jobbar du i kursen](guider/sa-jobbar-du.md) | Arbetsgången, reglerna, när något går fel |
| [Git och GitHub](guider/git-och-github.md) | Commit, push, pull, fork och vad allt betyder |
| [Så fungerar C#](guider/sa-fungerar-csharp.md) | Från kod till program, tre sorters fel |
| [Tester och autorättning](guider/tester-och-autorattning.md) | Vad som händer när du pushar |
| [Visual Studio och VS Code](guider/visual-studio-och-vscode.md) | Fönstren, kortkommandon, köra och debugga |
| [Variabler och datatyper](guider/variabler-och-datatyper.md) | `int`, `double`, `string`, att räkna |
| [Villkor, loopar och arrayer](guider/villkor-och-loopar.md) | `if`, `for`, `while`, arrayer |
| [Metoder](guider/metoder.md) | Parametrar och returvärden |
| [Testkör din kod själv](guider/testkora.md) | Provkör, debuggern, läsa testfel |
| [Filstrukturen i C#](guider/filstruktur.md) | Solution, projekt, program eller bibliotek |
| [Recept: skapa projekt](guider/skapa-projekt.md) | Bygga eget program och bibliotek |
| [Kodstandard och automatiska tips](guider/kodstandard.md) | Namn, klamrar, formatering och tipsen du får |
| [Merge-konflikter](guider/merge-konflikter.md) | Varför konflikter uppstår och hur du löser dem |
| [Versionering](guider/versionering.md) | MAJOR.MINOR.PATCH, ändringslogg och taggar |
| [Ändra och ångra historik](guider/andra-historik.md) | amend, revert, rebase -i, cherry-pick, bisect, reflog |

---

## Kom igång (görs en gång)

### 1. Forka repot
Klicka på **Fork** uppe till höger och sedan på **Create fork**.
Kontrollera att det står **ditt-namn / learn-csharp-with-oskar** överst.

### 2. Slå på autorättningen
GitHub stänger av automatiska körningar i nya forkar.

1. Gå till fliken **Actions** i **din fork**.
2. Klicka på den gröna knappen **I understand my workflows, go ahead and enable them**.

Gör du inte det här steget händer ingenting när du pushar.

### 3. Klona din fork
Byt `DITT-NAMN` mot **ditt eget** GitHub-namn. Klona din fork, inte lärarens repo,
annars går det inte att pusha.
```bash
git clone https://github.com/DITT-NAMN/learn-csharp-with-oskar.git
cd learn-csharp-with-oskar
start learn-csharp-with-oskar.sln
```
Den sista raden öppnar övningarna i Visual Studio. Använder du VS Code, skriv `code .` i stället.
Se [Visual Studio och VS Code](guider/visual-studio-och-vscode.md).

### 4. Koppla till lärarens repo
Då kan du hämta nya övningar när de kommer:
```bash
git remote add upstream https://github.com/OskarRayFrayssinet/learn-csharp-with-oskar.git
```

---

## Så jobbar du med en övning

1. Öppna övningens mapp och läs dess README.
2. Skriv din kod i den fil som README:n anger. **Ändra inte i testfilerna.**
   **Provkör** med egna värden: högerklicka på övningens `Provkor`-projekt,
   välj **Set as Startup Project** och tryck ▶. Läs mer i
   [Testkör din kod själv](guider/testkora.md).
   Kör sedan testerna med **Test → Run All Tests**.
3. Spara, committa och pusha:
   ```bash
   git add .
   git commit -m "Löste uppgift 1"
   git push
   ```
4. Gå till fliken **Actions** i din fork och klicka på den senaste körningen.
   Där ser du en tabell med alla tester:
   - ✅ = godkänt
   - ❌ = inte godkänt än

   På forkens startsida syns resultatet också som en grön bock ✓ eller ett rött kryss ✗
   bredvid din senaste commit.

Du kan pusha hur många gånger du vill. Varje push rättas på nytt.

### Testa på din egen dator (valfritt)
Har du .NET 10 SDK installerat kan du köra testerna innan du pushar:
```bash
dotnet test
```

---

## Nya övningar från läraren

Läraren lägger till nya övningar och rättelser i sitt repo. De kommer
**inte** automatiskt till din fork, så du hämtar dem själv.

### Kolla om det finns uppdateringar

Inget av sätten nedan ändrar något i dina filer.

**På GitHub (enklast):** öppna din fork. Ovanför fillistan står något av följande:

| Det står | Betyder |
|---|---|
| *This branch is up to date with …* | Inga uppdateringar |
| *This branch is **3 commits behind** …* | Det finns 3 nya ändringar från läraren |
| *This branch is 2 commits ahead of …* | Bara dina egna ändringar, inget nytt från läraren |
| *… ahead of, 3 commits behind …* | Både och. Det finns uppdateringar att hämta |

Klicka på länken **"3 commits behind"** för att se vad läraren har ändrat.

**I PowerShell:**
```bash
git fetch upstream
git log --oneline HEAD..upstream/main
```
- `git fetch upstream` kollar vad som finns hos läraren, men ändrar ingenting i dina filer.
- `git log …` listar ändringarna som du inte har hämtat än, till exempel:
  ```
  a1b2c3d Ny övning: 04-strängar
  ```
- **Skrivs ingenting ut** finns det inga uppdateringar.

Vill du också se vilka filer som har ändrats:
```bash
git diff --stat HEAD...upstream/main
```

### Hämta uppdateringarna

Committa alltid ditt eget arbete först, och hämta sedan:
```bash
git add .
git commit -m "Mitt arbete"
git pull --no-rebase --no-edit upstream main
git push
```

Står det `CONFLICT`: räck upp handen.
