# Guide: Merge-konflikter

## Vad är en konflikt?

Git är väldigt bra på att slå ihop ändringar automatiskt. Om du ändrar
rad 3 och din kompis ändrar rad 10 i samma fil, fixar Git det själv.

Men om ni **båda har ändrat samma rad** (eller rader precis bredvid
varandra) kan Git inte veta vilken version som är rätt. Då stannar Git och
frågar dig. Det är en **merge-konflikt**.

```
              ┌── Du:      "Tisdag: Pannkakor" ──┐
 "Tisdag: Soppa"                                  ├──▶  ??? Git vet inte
              └── Kompis:  "Tisdag: Tacos"     ──┘
```

En konflikt är **inget fel**. Den betyder bara att en människa behöver
bestämma.

---

## När händer det?

| Kommando | Typiskt läge |
|---|---|
| `git merge kompis` | Du slår ihop en annan gren med din |
| `git pull` | Någon annan har pushat ändringar på samma rader som du |
| `git pull upstream main` | Läraren har ändrat i en fil som du också har ändrat |

Git säger till direkt:
```
CONFLICT (content): Merge conflict in meny.txt
Automatic merge failed; fix conflicts and then commit the result.
```

---

## Konfliktmarkeringarna

Git skriver in **båda versionerna** i filen och markerar dem:

```
Måndag: Pasta
<<<<<<< HEAD
Tisdag: Pannkakor
=======
Tisdag: Tacos
>>>>>>> kompis
Onsdag: Fisk
```

| Markering | Betyder |
|---|---|
| `<<<<<<< HEAD` | Här börjar **din** version (den gren du står på) |
| `=======` | Skiljelinjen |
| `>>>>>>> kompis` | Här slutar **den andra** versionen (grenen du slår ihop med) |

Allt **utanför** markeringarna har Git redan löst. Du behöver bara ta hand
om det som står **mellan** `<<<<<<<` och `>>>>>>>`.

---

## Lös en konflikt i fem steg

### 1. Se vilka filer som har konflikter
```powershell
git status
```
```
Unmerged paths:
        both modified:   meny.txt
```

### 2. Öppna filen och bestäm vad som ska gälla
Du har fyra val:

| Val | Exempel på resultat |
|---|---|
| Behåll **din** version | `Tisdag: Pannkakor` |
| Behåll **den andra** versionen | `Tisdag: Tacos` |
| Behåll **båda** | `Tisdag: Pannkakor` och `Tisdag: Tacos` på var sin rad |
| Skriv **något nytt** som kombinerar dem | `Tisdag: Pannkakor med tacofyllning` |

### 3. Ta bort markeringarna
Radera raderna med `<<<<<<<`, `=======` och `>>>>>>>`. Filen ska se ut
precis som du vill ha den:
```
Måndag: Pasta
Tisdag: Tacos
Onsdag: Fisk
```

### 4. Markera filen som löst
```powershell
git add meny.txt
```

### 5. Avsluta mergen
```powershell
git commit --no-edit
```
`--no-edit` använder Gits färdiga meddelande ("Merge branch 'kompis'").
Utan det öppnas en texteditor. Hamnar du i Vim: tryck **Esc**, skriv
`:wq` och tryck **Enter**.

Klart! Kontrollera med `git status` och `git log --oneline --graph`.

---

## Lös konflikter i editorn

### VS Code
VS Code färgar konflikterna och visar knappar ovanför varje block:

| Knapp | Gör |
|---|---|
| **Accept Current Change** | Behåll din version (HEAD) |
| **Accept Incoming Change** | Behåll den andra versionen |
| **Accept Both Changes** | Behåll båda, den ena efter den andra |
| **Compare Changes** | Visa versionerna bredvid varandra |

Under **Source Control** (Ctrl+Shift+G) listas filer med konflikter under
**Merge Changes**. Klicka på **+** när en fil är löst. Det är samma sak som
`git add`.

### Visual Studio
Öppna **View → Git Changes**. Filer med konflikter syns under **Unmerged
Changes**. Dubbelklicka på en fil för att öppna **Merge Editor**. Där kryssar
du i vilka rader som ska vara med, och resultatet syns längst ner. Klicka på
**Accept Merge** och sedan **Commit Merge**.

### Vanlig textredigerare
Det går lika bra att ta bort markeringarna för hand i vilken editor som
helst. Det viktiga är att filen ser rätt ut efteråt.

---

## Särskilda konflikter

### Ändrad hos en, raderad hos den andra (modify/delete)
```
CONFLICT (modify/delete): anteckningar.txt deleted in HEAD and modified in kompis.
```
`git status` visar `deleted by us` eller `deleted by them`. Här finns inga
markeringar. Du väljer bara:

| Du vill | Kommando |
|---|---|
| Behålla filen | `git add anteckningar.txt` |
| Radera filen | `git rm anteckningar.txt` |

### Samma nya fil på båda håll (add/add)
Ni har skapat en fil med samma namn. Den löses precis som en vanlig
konflikt: med markeringar i filen.

### Konflikter i flera filer
`git status` listar alla. Lös dem en i taget och kör `git add` på varje.
Du kan inte avsluta mergen förrän alla är lösta.

---

## Nödutgången: avbryt

Blev det fel, eller har du slagit ihop fel gren? Avbryt hela mergen, så är
allt som innan du började:

```powershell
git merge --abort
```

---

## Konflikter i C#-kod

I kod räcker det inte att filen *ser* rätt ut. Den ska också **fungera**.

1. Läs båda versionerna noga. Vad försökte var och en göra?
2. Ibland är den ena rätt. Ibland behövs **delar av båda**.
3. Efter mergen: **bygg** (Ctrl+Shift+B) och **kör testerna**.

Vanliga misstag:
- En `}` försvann eller kom med två gånger.
- Båda versionerna av en rad blev kvar, så att en variabel deklareras två gånger.
- En variabel har bytt namn i den ena versionen men inte i resten av koden.

---

## Så undviker du konflikter

| Gör så här | Varför |
|---|---|
| Hämta ändringar ofta (`git pull`) | Ju mindre som skiljer, desto färre konflikter |
| Committa och pusha i små steg | Mindre att slå ihop åt gången |
| Prata ihop er om vem som ändrar vad | Ändringar i olika filer krockar aldrig |
| Ändra inte i andras filer i onödan | I kursen ändrar du bara i dina egna filer |
| Formatera inte om hela filer | Tusentals ändrade mellanslag ger konflikter överallt |

---

## Fusklapp

```powershell
git merge kompis               # slå ihop grenen kompis med din gren
git pull --no-rebase upstream main   # hämta och slå ihop lärarens ändringar
git status                     # vilka filer har konflikter?
# ... rätta filerna ...
git add <fil>                  # markera som löst
git commit --no-edit           # avsluta mergen
git merge --abort              # ångra allt och börja om
git log --oneline --graph      # se historiken som ett träd
```
