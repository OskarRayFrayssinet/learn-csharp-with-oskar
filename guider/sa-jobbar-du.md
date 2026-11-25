# Så jobbar du i kursen

Den här guiden beskriver **hur det är tänkt att du ska arbeta**, från första
lektionen till sista. Läs den först och kom tillbaka hit när du är osäker på
vad nästa steg är.

---

## Helheten

Det finns **tre platser** där koden finns. Det är viktigt att förstå
skillnaden:

```
 ┌─────────────────────────┐
 │  LÄRARENS REPO          │   Här lägger läraren ut nya övningar.
 │  (på GitHub)            │   Du kan läsa, men inte ändra.
 └───────────┬─────────────┘
             │  1. Fork (en gång)
             │  5. Hämta uppdateringar (ibland)
             ▼
 ┌─────────────────────────┐
 │  DIN FORK               │   Din egen kopia. Här rättas ditt arbete
 │  (på GitHub)            │   automatiskt, och läraren kan se den.
 └───────────┬─────────────┘
       ▲     │  2. Clone (en gång)
       │     ▼
       │ ┌─────────────────────────┐
       │ │  DIN DATOR              │   Här skriver du koden i Visual Studio.
       │ │  (lokal klon)           │
       │ └───────────┬─────────────┘
       │             │  3. Commit (spara en version)
       └─────────────┘  4. Push (skicka upp till din fork)
```

| Plats | Vem äger den | Vad du gör där |
|---|---|---|
| Lärarens repo | Läraren | Ingenting. Den är källan för nya övningar |
| Din fork | Du | Tittar på rättningen under **Actions** |
| Din dator | Du | Skriver kod, provkör, kör tester, committar |

Mer om varför det ser ut så här: [Git och GitHub](git-och-github.md).

---

## Första gången (görs en gång)

Instruktionerna finns i [README-filen i repots rot](../README.md#kom-igång-görs-en-gång):

1. Forka lärarens repo på GitHub.
2. Slå på **Actions** i din fork.
3. Klona **din fork** till datorn.
4. Koppla till lärarens repo med `git remote add upstream …`.

---

## Varje lektion

### I början av lektionen
1. Öppna PowerShell i repomappen: `cd C:\github-repos\learn-csharp-with-oskar`
2. **Kolla om det finns något nytt** från läraren och hämta det i så fall.
   Se [Nya övningar från läraren](../README.md#nya-övningar-från-läraren).
3. Öppna solutionen: `start learn-csharp-with-oskar.sln`

### Under lektionen: jobba i små steg

```
   ┌──────────────┐
   │ 1. Läs       │  uppgiften i övningens README och guiden som hör till
   └──────┬───────┘
          ▼
   ┌──────────────┐
   │ 2. Skriv     │  kod för EN uppgift i taget
   └──────┬───────┘
          ▼
   ┌──────────────┐
   │ 3. Provkör   │  med egna värden (▶ i Provkör-projektet)
   └──────┬───────┘
          ▼
   ┌──────────────┐        nej
   │ 4. Rätt?     │──────────────▶ tillbaka till 2 (använd gärna debuggern)
   └──────┬───────┘
          │ ja
          ▼
   ┌──────────────┐
   │ 5. Testa     │  Test → Run All Tests
   └──────┬───────┘
          ▼
   ┌──────────────┐
   │ 6. Commit    │  spara en version med ett tydligt meddelande
   └──────┬───────┘
          ▼
      nästa uppgift
```

Du behöver inte pusha efter varje uppgift, men **committa** gärna ofta.
Varje commit är en sparpunkt som du kan gå tillbaka till.

### I slutet av lektionen
```powershell
git add .
git commit -m "Uppgift 1–4 i övning 01"
git push
```
Öppna sedan din **dashboard** (länken *Min progression* överst i README-filen
i din fork). Efter ungefär en minut visar den dina nya resultat, vad du ska
göra härnäst och tips om din kod.

> **Pusha alltid innan du går.** Det du inte har pushat finns bara på
> datorn. Läraren kan inte se det, och det rättas inte.

---

## Reglerna

| Gör | Gör inte |
|---|---|
| ✅ Skriv kod i filerna som README:n anger | ❌ Ändra i testfilerna (`*.Tests`) |
| ✅ Ändra fritt i Provkör-projekten | ❌ Ändra i lärarens filer, som `README.md` och `.github` |
| ✅ Committa ofta, med tydliga meddelanden | ❌ Klicka på **Discard commits** på GitHub |
| ✅ Pusha i slutet av varje lektion | ❌ Klona lärarens repo i stället för din fork |
| ✅ Hämta lärarens uppdateringar när det finns några | ❌ Skapa en ny fork om något strular. Fråga i stället |

Att du bara ändrar i **dina** filer gör att lärarens uppdateringar alltid
kan hämtas utan konflikter.

---

## Var finns vad?

| Jag vill… | Titta här |
|---|---|
| …veta vad jag ska göra i en övning | `README.md` i övningens mapp |
| …skriva min kod | Filen som README:n pekar på, t.ex. `Grunder/Grunder.cs` |
| …prova min kod med egna värden | Övningens `Provkor`-projekt |
| …se om mina tester är gröna | **Test Explorer** i Visual Studio |
| …se hur långt jag har kommit | Min **dashboard**: grenen `dashboard` i min fork (länk överst i README) |
| …se min rättning i detalj | Fliken **Actions** i min fork på GitHub |
| …få tips om min kodstil | Vågstrecken i editorn, och dashboarden. Se [Kodstandard](kodstandard.md) |
| …hitta rätt i Visual Studio eller VS Code | [Visual Studio och VS Code](visual-studio-och-vscode.md) |
| …förstå något bättre | [guider/](README.md) |

---

## När något går fel

| Problem | Gör så här |
|---|---|
| Koden går inte att bygga (röda krumelurer) | Läs felmeddelandet i **Error List**. Se [Så fungerar C#](sa-fungerar-csharp.md#tre-sorters-fel) |
| Ett test är rött | Läs *Expected* och *Actual*. Se [Testkör din kod själv](testkora.md#läsa-ett-testfel) |
| ▶ Start ger fel om *class library* | Fel startprojekt. Se [Filstrukturen](filstruktur.md#körbart-program-eller-bibliotek) |
| `git push` ger `403` | Du har klonat lärarens repo. Kör `git remote -v` och visa läraren |
| `CONFLICT` när du hämtar | Räck upp handen |
| Du har fastnat i över 10 minuter | Fråga en klasskamrat eller läraren |

### När du ber om hjälp
Berätta tre saker:
1. **Vad du försökte göra** ("Jag ville pusha uppgift 3")
2. **Vad som hände** (kopiera hela felmeddelandet, eller ta en skärmbild)
3. **Vad du redan har provat**
