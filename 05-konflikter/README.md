# Övning 05: Merge-konflikter

Konflikter händer alla som programmerar tillsammans. Här övar du på att
lösa dem lugnt och metodiskt, tills det känns vardagligt.

**Läs först:** [Guide: Merge-konflikter](../guider/merge-konflikter.md)

| Del | Vad du gör | Rättas |
|---|---|---|
| **A – Konfliktlabbet** | 14 övningsrepon med konflikter, från lätt till svårt, ett par av dem med en robotkompis i ett delat repo | Automatiskt (kvitton) |
| **B – På riktigt med läraren** | Läraren ändrar samma rader som du, och du löser konflikten i ditt eget repo | Automatiskt |

---

## Del A: Konfliktlabbet

Konfliktlabbet är ett litet program som bygger upp små git-repon där en
konflikt väntar på dig. Repona hamnar i mappen `05-konflikter/labb`, som
inte följer med till GitHub. Där kan du göra fel hur många gånger som helst.

### Så gör du

**1. Starta labbet** (från repots rotmapp):
```powershell
dotnet run --project 05-konflikter/Konfliktlabb
```
Du får en meny med alla scenarier. Du kan också skriva kommandona direkt:
```powershell
dotnet run --project 05-konflikter/Konfliktlabb -- skapa 1
```

**2. Läs uppdraget.** Det visas när scenariot skapas och finns också i
filen `UPPDRAG.txt` i scenariots mapp.

**3. Gå till scenariots mapp** i en terminal:
```powershell
cd 05-konflikter\labb\scenario-01
git status
git log --oneline --all --graph
```

**4. Lös konflikten** med git-kommandon och en editor. Öppna gärna
mappen i VS Code (`code .`), som visar konflikter med färger och knappar.

**5. Kontrollera din lösning** (i scenariots mapp):
```powershell
.\kontrollera.cmd
```
Du får veta exakt vad som är fel, eller **OK** om allt stämmer.

**6. Skicka in.** När ett scenario är OK skapas ett kvitto i
`05-konflikter/kvitton`. Gå tillbaka till repots rotmapp och pusha det:
```powershell
cd ..\..\..
git add 05-konflikter/kvitton
git commit -m "Konfliktlabbet: scenario 1"
git push
```

### Gick det snett?
Skapa scenariot igen, så börjar du om från början:
```powershell
dotnet run --project 05-konflikter/Konfliktlabb -- skapa 1
```

### Scenarierna

| Nr | Scenario | Nivå | Du lär dig |
|---|---|---|---|
| 1 | Samma rad – välj kompisens | Lätt | Konfliktmarkeringarna och ett första löst block |
| 2 | Samma rad – behåll din egen | Lätt | Skillnaden mellan HEAD (din) och den andra grenen |
| 3 | Båda ändringarna behövs | Lätt | Att ibland ska båda versionerna vara kvar |
| 4 | Två konflikter i samma fil | Medel | Att lösa flera block i en fil |
| 5 | Konflikt i C#-kod | Medel | Att läsa koden och välja den version som fungerar |
| 6 | Kombinera två ändringar | Medel | Att skriva en ny rad som innehåller båda ändringarna |
| 7 | Konflikter i två filer | Medel | Att använda `git status` för att hitta alla konflikter |
| 8 | Raderad eller ändrad | Medel | `deleted by us` och skillnaden mellan `git add` och `git rm` |
| 9 | Samma nya fil | Medel | Konflikter där filen inte fanns innan (add/add) |
| 10 | Avbryt en merge | Medel | Nödutgången `git merge --abort` |
| 11 | Hämta från läraren | Svår | Konflikt vid `git pull upstream`, precis som i kursen |
| 12 | Tre konflikter i C# | Svår | Att tänka själv i varje block |
| 13 | Delat repo – din push nekas | Svår | `rejected (fetch first)`, sedan pull, lös och pusha igen |
| 14 | Delat repo – kombinera kod | Svår | Samma flöde, men med C#-kod som ska kombineras |

I scenario 13 och 14 delar du repo med **Robotkompisen**, som redan har
pushat innan du börjar. Precis som när man jobbar i grupp: din push nekas,
och du måste hämta, lösa konflikten och pusha igen. Kontrollen ser också
till att din lösning verkligen har pushats till det delade repot.

---

## Del B: På riktigt med läraren

Nu blir det en konflikt i ditt eget repo, precis som det kan bli i
verkligheten.

**1. Fyll i [`klassrummet.txt`](klassrummet.txt)** med dina svar efter kolonen:
```
Namn: Ali
Favoritfrukt: Mango
Det bästa med programmering: När testerna blir gröna
```

**2. Committa och pusha.**

**3. Vänta på läraren.** Läraren ändrar rubrikerna på samma rader i sitt
repo. Du kommer inte att veta exakt hur förrän du hämtar ändringen.

**4. Hämta lärarens ändring:**
```powershell
git pull --no-rebase upstream main
```
Nu blir det konflikt i `klassrummet.txt`.

**5. Lös konflikten.** Använd **lärarens nya rubriker** med **dina svar**
efter kolonen. Ta bort konfliktmarkeringarna. Committa och pusha.

Testet `05 · B · 2` blir grönt först när du har hämtat lärarens ändring och
löst konflikten.

---

## Testerna
Det finns 16 tester: 14 för Konfliktlabbet och 2 för Del B. Testnamnen
börjar med `05 ·`.
