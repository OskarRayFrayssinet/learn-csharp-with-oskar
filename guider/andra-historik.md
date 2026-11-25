# Guide: Ändra och ångra historik

Git kommer ihåg allt. Men ibland vill man **rätta** historiken: byta ett
meddelande, lägga till en glömd fil, ångra en commit som införde en bugg
eller ta bort något som aldrig skulle ha committats.

---

## Den viktigaste frågan: är det pushat?

```
                      Har du pushat commiten?
                     ┌─────────┴─────────┐
                    NEJ                  JA
                     │                    │
        Du får skriva om historiken   Skriv INTE om historiken.
        amend · rebase -i · reset     Ångra med en ny commit:
                                      git revert
```

När du skriver om historiken får commitarna **nya id**. Om någon annan
redan har hämtat de gamla har ni plötsligt två olika historier, och det
blir trassel. Därför gäller:

> **Skriv bara om historik som bara finns på din dator.**

---

## Snabböversikt

| Jag vill… | Kommando | Skriver om historiken? |
|---|---|---|
| Byta meddelande på senaste commit | `git commit --amend -m "Nytt"` | Ja |
| Lägga till en glömd fil i senaste commit | `git add fil` → `git commit --amend --no-edit` | Ja |
| Ångra en gammal commit (säkert) | `git revert <id>` | **Nej**, gör en ny commit |
| Byta meddelande på en äldre commit | `git rebase -i` → `reword` | Ja |
| Slå ihop flera commits | `git rebase -i` → `fixup` / `squash` | Ja |
| Ta bort en commit helt | `git rebase -i` → `drop` | Ja |
| Ändra innehållet i en äldre commit | `git rebase -i` → `edit` | Ja |
| Hämta en fil från en gammal commit | `git restore --source <id> fil` | Nej |
| Kopiera en commit från en annan gren | `git cherry-pick <id>` | Nej |
| Flytta grenen bakåt | `git reset --soft/--hard HEAD~n` | Ja |
| Hitta commiten som införde en bugg | `git bisect` | Nej |
| Hitta tillbaka till något "försvunnet" | `git reflog` | Nej |

---

## Hitta rätt commit

| Kommando | Visar |
|---|---|
| `git log --oneline` | Alla commits, en per rad |
| `git log --oneline --graph --all` | Alla grenar som ett träd |
| `git show <id>` | Vad en commit ändrade |
| `git log -p fil.cs` | Alla ändringar i en fil, commit för commit |
| `git blame fil.cs` | Vilken commit som senast ändrade varje rad |
| `git log -S "text"` | Commits som lade till eller tog bort en viss text |

Ett **id** är de första tecknen i commitens kod, till exempel `a1b2c3d`.
Det räcker att skriva 7 tecken.

---

## `git commit --amend`: rätta den senaste commiten

```powershell
git commit --amend -m "Bättre meddelande"   # byt meddelande
git add glomd-fil.txt
git commit --amend --no-edit                 # lägg till en glömd fil, behåll meddelandet
```
Senaste commiten **byts ut** mot en ny. Det blir ingen extra commit.

---

## `git revert`: ångra en gammal commit på ett säkert sätt

```powershell
git revert a1b2c3d
```
Git skapar en **ny commit** som gör precis tvärtom mot den gamla. Den gamla
finns kvar i historiken, men dess ändringar är borta. Eftersom inget
skrivs om är det säkert även efter push.

Meddelandet blir `Revert "…"`. Öppnas en editor: spara och stäng.

---

## `git rebase -i`: skriv om flera commits

```powershell
git rebase -i HEAD~4      # de fyra senaste commitarna
```
Git öppnar en lista där **den äldsta står överst**:
```
pick a1b2c3d Lägg till Summa
pick e4f5a6b fixade grejer
pick c7d8e9f Lägg till Storsta
pick 0a1b2c3 Uppdatera README
```
Byt ordet `pick` på de rader du vill ändra:

| Ord | Kort | Gör |
|---|---|---|
| `pick` | `p` | Behåll commiten som den är |
| `reword` | `r` | Behåll innehållet, byt meddelande |
| `edit` | `e` | Stanna här så att du kan ändra innehållet |
| `squash` | `s` | Slå ihop med commiten ovanför, och skriv ett nytt meddelande |
| `fixup` | `f` | Slå ihop med commiten ovanför, och släng det här meddelandet |
| `drop` | `d` | Ta bort commiten helt |

**Spara och stäng** listan, så kör git igenom den uppifrån och ned.

### Med `edit`
Git stannar vid commiten. Då gör du så här:
```powershell
# ändra filerna
git add fil.txt
git commit --amend --no-edit
git rebase --continue
```

### Om något går fel
```powershell
git rebase --abort        # allt blir som innan du började
```
Blir det konflikt under en rebase: lös den som vanligt, `git add`, och
sedan `git rebase --continue`.

---

## `git restore --source`: hämta en fil från förr

```powershell
git log --oneline -- Recept.txt               # vilka commits har rört filen?
git restore --source a1b2c3d Recept.txt       # hämta filen som den var i a1b2c3d
git add Recept.txt
git commit -m "Återställ Recept.txt"
```

---

## `git cherry-pick`: kopiera en enda commit

```powershell
git log --oneline experiment                  # hitta id:t på den andra grenen
git cherry-pick a1b2c3d                       # kopiera commiten till din gren
```

---

## `git reset`: flytta grenen bakåt

| Variant | Commitarna | Ändringarna i filerna |
|---|---|---|
| `git reset --soft HEAD~1` | Tas bort från grenen | Finns kvar, redo att committas igen |
| `git reset --hard HEAD~1` | Tas bort från grenen | **Försvinner** från mappen |

**Commits på fel gren?** Spara dem på en ny gren först, flytta sedan tillbaka:
```powershell
git branch ny-gren          # ny gren som pekar på dina commits
git reset --hard HEAD~2     # flytta tillbaka den gren du står på
```

---

## `git bisect`: hitta buggen genom att halvera

Har du 100 commits och vet att buggen finns nu men inte fanns i början?
`git bisect` hittar rätt commit på ungefär 7 steg.

```powershell
git bisect start
git bisect bad                 # nuvarande version har buggen
git bisect good a1b2c3d        # den här gamla versionen var bra
```
Git checkar ut en commit mitt emellan. Testa och svara:
```powershell
git bisect good                # buggen finns inte här
git bisect bad                 # buggen finns här
```
Till slut säger git: **`<id> is the first bad commit`**. Skriv upp id:t och avsluta:
```powershell
git bisect reset
git revert <id>
```

---

## `git reflog`: livlinan

Har du gjort en `reset --hard` eller en rebase och tappat bort commits?
De finns kvar en tid.
```powershell
git reflog                     # allt som HEAD har pekat på
git branch raddning a1b2c3d    # gör en gren av en "försvunnen" commit
```

---

## Om du redan har pushat omskriven historik

Har du skrivit om commits som redan låg på GitHub vägrar `git push`. Det
är ett skydd. Om du är **helt säker** på att ingen annan har hämtat dem
(till exempel i din egen fork):
```powershell
git push --force-with-lease
```
`--force-with-lease` vägrar om någon annan har pushat under tiden. Använd
aldrig force push mot en gren som andra jobbar på.
