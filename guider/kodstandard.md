# Guide: Kodstandard och automatiska tips

## Varför en kodstandard?

Kod läses mycket oftare än den skrivs, av dig själv om en vecka, av
klasskamrater och av läraren. En **kodstandard** är gemensamma regler för
hur koden ska se ut, så att all kod i ett projekt ser likadan ut och blir
lätt att läsa.

Kursen följer de vanliga reglerna för C#, som nästan alla C#-programmerare
använder. Reglerna finns i filen [`.editorconfig`](../.editorconfig) i repots
rot.

---

## Du får tips automatiskt

Du behöver inte komma ihåg alla regler. Datorn säger till:

| Var | Hur det syns |
|---|---|
| **Visual Studio** | Gröna eller gula vågstreck under koden. I **Error List** under fliken **Warnings** (efter bygge) |
| **VS Code** | Vågstreck under koden, och i panelen **Problems** (Ctrl+Shift+M) |
| **GitHub Actions** | Avsnittet **💡 Tips om kodstil** i sammanfattningen, och små notiser direkt i koden |
| **Din dashboard** | Antal tips och vilka sorter |

> **Tipsen påverkar inte rättningen.** Ett test blir aldrig rött på grund
> av kodstil. Men det är bra att träna på att skriva snygg kod från början.

Varje tips har en **kod**, till exempel `IDE0011`. Håll musen över
vågstrecket eller sök på koden i tabellen nedan.

---

## Reglerna

### Namn (IDE1006)

| Vad | Stil | Bra | Dåligt |
|---|---|---|---|
| Klasser | PascalCase | `Sortering`, `Kalkylator` | `sortering`, `kalkylator_klass` |
| Metoder | PascalCase | `SorteraStigande`, `ArJamnt` | `sorteraStigande`, `ar_jamnt` |
| Variabler | camelCase | `summa`, `antalElever` | `Summa`, `antal_elever` |
| Parametrar | camelCase | `int tal`, `string namn` | `int Tal` |
| Konstanter | PascalCase | `const int MaxAntal = 10;` | `const int MAX_ANTAL` |

**PascalCase:** varje ord börjar med stor bokstav: `RaknaUtSumma`.
**camelCase:** första ordet med liten bokstav, resten med stor: `raknaUtSumma`.

Välj också namn som **säger vad saken är**. `summa` är bättre än `s`, och
`antalJamna` är bättre än `x`. Bara loopräknare får heta `i`, `j` och `k`.

### Klamrar alltid (IDE0011)
```csharp
// ❌ Dåligt
if (tal > 0) return true;

// ✅ Bra
if (tal > 0)
{
    return true;
}
```
Klamrarna gör det tydligt vad som hör till `if`-satsen, och förhindrar
buggar när man lägger till en rad till senare.

### Indrag och mellanslag (IDE0055)
- Fyra mellanslag per nivå.
- `{` och `}` på egna rader.
- Mellanslag runt operatorer: `a + b`, inte `a+b`.
- Mellanslag efter `if`, `for` och `while`: `if (x)`, inte `if(x)`.

Du behöver inte göra det för hand. **Formatera hela filen** med:

| Program | Kortkommando |
|---|---|
| Visual Studio | **Ctrl+K, Ctrl+D** |
| VS Code | **Shift+Alt+F** |

### Ingen onödig kod (IDE0059, IDE0051, CS0168, CS0219)
Variabler och metoder som aldrig används gör koden svårare att läsa. Ta
bort dem.

### Egna regler för kursen

| Kod | Regel | Varför |
|---|---|---|
| `STIL01` | Ingen `Console.WriteLine` i övningarnas klassbibliotek | Metoder ska **returnera** svaret. Skriv ut i Provkör i stället |
| `STIL02` | Inga variabelnamn med en bokstav (utom `i`, `j`, `k`) | Namnet ska säga vad variabeln innehåller |
| `STIL03` | Ingen bortkommenterad kod | Git kommer ihåg gammal kod. Ta bort den |
| `STIL04` | Rader högst 120 tecken | Långa rader är svåra att läsa |

---

## Fixa automatiskt

### Glödlampan (Ctrl+.)
Ställ markören på ett vågstreck och tryck **Ctrl+.** Visual Studio och
VS Code föreslår en lösning, till exempel *Add braces* eller *Rename to
'summa'*. Välj den med **Enter**.

### `dotnet format`
Det här kommandot fixar formatering och stil i ett helt projekt på en gång:
```powershell
dotnet format 01-grunder/Grunder
```
Kör `git diff` efteråt för att se vad som ändrades.

---

## Kommentarer

| Gör | Gör inte |
|---|---|
| Förklara **varför** något görs | Upprepa **vad** koden redan säger |
| `// Börja på 1 eftersom 0 inte påverkar summan` | `// lägg till i i summa` |
| Skriv `///` ovanför en metod för att beskriva den | Lämna kvar bortkommenterad kod |
