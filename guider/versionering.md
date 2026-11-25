# Guide: Versionering

## Varför versionsnummer?

När du använder ett bibliotek, till exempel `xunit` i testerna, vill du
veta två saker innan du uppdaterar det:

1. **Har något ändrats?**
2. **Kommer min kod att sluta fungera?**

Versionsnumret svarar på båda frågorna, om det följer en gemensam regel.
Den regel nästan alla använder heter **semantisk versionering**
(*Semantic Versioning*, förkortat **SemVer**).

---

## MAJOR.MINOR.PATCH

Ett versionsnummer har tre delar:

```
        2  .  4  .  1
        │     │     └── PATCH: buggfixar
        │     └──────── MINOR: nya funktioner, gammal kod fungerar fortfarande
        └────────────── MAJOR: ändringar som kan få andras kod att sluta fungera
```

| Du har… | Höj | Exempel från 1.4.2 |
|---|---|---|
| rättat en bugg, och allt används som förut | **PATCH** | `1.4.3` |
| lagt till något nytt, och all gammal kod fungerar | **MINOR** | `1.5.0` |
| ändrat så att andras kod kan sluta fungera | **MAJOR** | `2.0.0` |

### Nollställ delarna till höger
När en del höjs blir alla delar **till höger** om den `0`:

```
1.4.2  ──PATCH──▶  1.4.3
1.4.3  ──MINOR──▶  1.5.0      (PATCH blir 0)
1.5.0  ──MAJOR──▶  2.0.0      (MINOR och PATCH blir 0)
```

### Varje del är ett eget tal
`1.10.0` är **högre** än `1.9.0`, eftersom 10 > 9. Det är inte decimaltal.

---

## Vad räknas som att "andras kod slutar fungera"?

Det som andra kan använda i ditt bibliotek kallas dess **API**: de
publika (`public`) klasserna och metoderna. Ändringar som **bryter** API:t
kräver en ny MAJOR-version.

| Ändring i C# | Bryter API:t? | Höj |
|---|---|---|
| Rätta en bugg inne i en metod | Nej | PATCH |
| Göra en metod snabbare med samma resultat | Nej | PATCH |
| Lägga till en ny `public` metod | Nej | MINOR |
| Lägga till en ny överlagring (samma namn, andra parametrar) | Nej | MINOR |
| **Byta namn** på en `public` metod | **Ja**, anrop med det gamla namnet slutar fungera | MAJOR |
| **Ta bort** en `public` metod | **Ja** | MAJOR |
| **Ändra parametrarna** i en befintlig metod | **Ja** | MAJOR |
| **Ändra returtypen**, t.ex. `int` till `double` | **Ja** | MAJOR |
| Ändra vad metoden **gör** så att den ger andra svar än det som utlovats | **Ja** | MAJOR |

Tumregel: **tänk dig någon som använder ditt bibliotek.** Måste den
personen ändra sin kod efter uppdateringen? Då är det MAJOR.

---

## Specialfall

### Version 0.x.y: tidig utveckling
Så länge MAJOR är `0` lovar man ingenting. Vad som helst kan ändras.
Version `1.0.0` betyder "nu är API:t stabilt".

### Förhandsversioner
Med ett bindestreck kan man släppa en testversion innan den riktiga:
```
2.0.0-alpha.1  <  2.0.0-beta.1  <  2.0.0-rc.1  <  2.0.0
```

---

## Versionen i ett C#-projekt

Versionen skrivs i `.csproj`-filen:
```xml
<PropertyGroup>
  <Version>1.2.0</Version>
</PropertyGroup>
```
Den hamnar sedan i den färdiga `.dll`-filen. När man laddar upp ett
bibliotek till NuGet är det den versionen som visas.

---

## Ändringslogg (CHANGELOG)

En **ändringslogg** är en fil där man skriver vad som har ändrats i varje
version, så att användarna slipper läsa koden. Den vanligaste mallen heter
[Keep a Changelog](https://keepachangelog.com):

```markdown
# Ändringslogg

## [1.1.0] - 2026-11-27
### Tillagt
- `Avrunda(pris)` avrundar till hela kronor.

## [1.0.1] - 2026-11-26
### Fixat
- `Rabatt` gav fel resultat.

## [1.0.0] - 2026-11-25
### Tillagt
- Första versionen.
```

- Den **nyaste versionen står överst**.
- Varje version har ett **datum**.
- Ändringarna sorteras under rubriker:

| Rubrik | Används för | Brukar ge |
|---|---|---|
| **Tillagt** | Nya funktioner | MINOR |
| **Ändrat** | Ändringar i befintliga funktioner | MAJOR (om det bryter API:t) |
| **Borttaget** | Funktioner som tagits bort | MAJOR |
| **Fixat** | Buggfixar | PATCH |

---

## Git-taggar: märk en version i historiken

En **tagg** är en etikett på en commit. Med en tagg per version kan man
alltid hitta exakt den kod som hörde till en viss version.

```powershell
git tag v1.0.1          # sätt taggen på senaste commit
git tag                 # lista alla taggar
git push --tags         # skicka taggarna till GitHub
git log --oneline --decorate   # se taggarna i historiken
```

Det är vanligt att skriva ett `v` före numret: `v1.0.1`.

På GitHub syns taggarna under **Tags** (klicka på grenväljaren eller gå
till `…/tags`). Därifrån kan man också skapa en **Release** med
ändringslogg och filer att ladda ner.

---

## Arbetsgång vid en ny version

```
 Ändra koden → Testa → Välj PATCH/MINOR/MAJOR → Höj <Version>
      → Skriv i CHANGELOG → Commit → Tagga → Push + push --tags
```

---

## Sammanfattning

| Fråga | Svar |
|---|---|
| Buggfix? | PATCH: `1.4.2` → `1.4.3` |
| Ny funktion, gammal kod fungerar? | MINOR: `1.4.2` → `1.5.0` |
| Gammal kod kan sluta fungera? | MAJOR: `1.4.2` → `2.0.0` |
| Var står versionen? | `<Version>` i `.csproj` |
| Var beskrivs ändringarna? | `CHANGELOG.md`, nyaste överst |
| Hur märker man versionen i git? | `git tag v1.4.3` och `git push --tags` |
