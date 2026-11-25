# Övning 07: Rätta gammal historik

I riktiga projekt händer det hela tiden: ett commit-meddelande blev fel, en
fil glömdes, en gammal commit införde en bugg, eller något hemligt råkade
committas. Här övar du på att gå in i ett **färdigt projekt** och rätta
till **äldre commits**.

**Läs först:** [Guide: Ändra och ångra historik](../guider/andra-historik.md)

---

## Historiklabbet

Historiklabbet bygger upp små färdiga projekt med en historik som behöver
rättas. Repona hamnar i `07-historik/labb`, som inte följer med till GitHub.
Där kan du göra fel och börja om hur många gånger som helst.

### Så gör du

**1. Starta labbet** (från repots rotmapp):
```powershell
dotnet run --project 07-historik/Historiklabb
```
Eller direkt: `dotnet run --project 07-historik/Historiklabb -- skapa 1`

**2. Gå till scenariots mapp och titta på historiken:**
```powershell
cd 07-historik\labb\scenario-01
git log --oneline --graph --all
```
Uppdraget visas när scenariot skapas och finns också i `UPPDRAG.txt`.

**3. Rätta historiken** med git-kommandona i uppdraget.

**4. Kontrollera:**
```powershell
.\kontrollera.cmd
```

**5. Skicka in kvittot** från repots rotmapp:
```powershell
git add 07-historik/kvitton
git commit -m "Historiklabbet: scenario 1"
git push
```

### Om git öppnar en texteditor
Kommandon som `git rebase -i` och `git revert` öppnar en texteditor. I
labbet används VS Code om det finns, annars Anteckningar (Windows) eller
nano (Mac/Linux). **Gör din ändring, spara och stäng fönstret eller
fliken**, så fortsätter git.

### Blev det fel?
- Mitt i en rebase: `git rebase --abort` tar dig tillbaka till läget innan.
- Helt fel: skapa scenariot igen så börjar du om:
  `dotnet run --project 07-historik/Historiklabb -- skapa 1`

---

## Scenarierna

| Nr | Scenario | Nivå | Du lär dig |
|---|---|---|---|
| 1 | Fel meddelande i senaste commit | Lätt | `git commit --amend -m` |
| 2 | Glömd fil i senaste commit | Lätt | `git commit --amend --no-edit` |
| 3 | Ångra en gammal commit | Medel | `git revert`: ångra utan att skriva om historiken |
| 4 | Hitta buggen själv och reverta | Medel | `git log -p` och `git blame` för att hitta rätt commit |
| 5 | Leta med git bisect | Svår | `git bisect`: låt git halvera historiken åt dig |
| 6 | Byt meddelande på en äldre commit | Medel | `git rebase -i` med `reword` |
| 7 | Slå ihop småcommits | Medel | `git rebase -i` med `fixup` |
| 8 | Ta bort en commit | Medel | `git rebase -i` med `drop` |
| 9 | Ta bort ett lösenord ur historiken | Svår | `git rebase -i` med `edit` |
| 10 | Återställ en raderad fil | Medel | `git restore --source` |
| 11 | Plocka en commit från en annan gren | Medel | `git cherry-pick` |
| 12 | Commits på fel gren | Svår | `git branch` och `git reset --hard` |

### Vad kontrollen tittar på
Kontrollen jämför historiken **före** och **efter**, till exempel:
- att rätt commits har ändrats, och att resten är **orörda** (samma id)
- att innehållet i filerna är precis som det ska
- att du inte står mitt i en rebase, merge, revert eller bisect
- att allt är committat

---

## Två viktiga regler

1. **Skriv bara om historik som inte är pushad.** `amend`, `rebase -i` och
   `reset` skapar nya commits med nya id. Har någon annan redan hämtat de
   gamla blir det trassel. Är historiken redan pushad: använd `git revert`.
2. **Inget är borta på riktigt.** `git reflog` visar allt du har stått på,
   så du kan nästan alltid hitta tillbaka.

---

## Testerna
Det finns 12 tester, ett per scenario. Testnamnen börjar med `07 ·`.
