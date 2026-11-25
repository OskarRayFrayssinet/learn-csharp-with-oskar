# Tester och autorättning

## Vad är ett test?

Ett test är en liten bit kod som **anropar din kod och kontrollerar
svaret**. Programmerare skriver tester för att snabbt kunna se att koden
fungerar, även efter att de har ändrat något.

Så här ser ett av testerna i övning 01 ut:

```csharp
[Fact(DisplayName = "01 · Uppgift 1: StorstAvTva – andra talet störst")]
public void AndraStorst()
{
    Assert.Equal(8, Ovningar.StorstAvTva(3, 8));
}
```

| Del | Betyder |
|---|---|
| `[Fact(...)]` | "Det här är ett test." `DisplayName` är namnet du ser i Test Explorer |
| `Ovningar.StorstAvTva(3, 8)` | Testet anropar **din** metod med 3 och 8 |
| `Assert.Equal(8, ...)` | Kontrollerar att svaret blev **8**. Annars blir testet rött |

Testerna ligger i projekten som slutar på `.Tests`. Du får gärna läsa dem.
Det är ett bra sätt att förstå exakt vad som krävs. Men **ändra inte i
dem**.

---

## Grönt, rött och blått

| Symbol | Betyder |
|---|---|
| ✅ Grön bock | Testet gick igenom. Din kod gav rätt svar |
| ❌ Rött kryss | Testet gick inte igenom. Fel svar, eller så kraschade koden |
| ⓘ Blå/grå ikon | Testet har inte körts än |

Från början är nästan alla tester röda, eftersom metoderna bara innehåller
`throw new NotImplementedException();`. Det är meningen. Ditt jobb är att
göra dem gröna, en i taget.

---

## Två ställen där testerna körs

| | På din dator | På GitHub (autorättning) |
|---|---|---|
| **Var** | Test Explorer i Visual Studio | Fliken **Actions** i din fork |
| **När** | När du klickar **Run All Tests** | Automatiskt, varje gång du pushar |
| **Till för** | Dig, medan du jobbar | Dig *och* läraren. Det är den som räknas |
| **Kräver** | .NET på datorn | Ingenting, det körs på GitHubs servrar |

Det är **samma tester** på båda ställena. Är de gröna hos dig blir de
nästan alltid gröna på GitHub också.

---

## Vad händer när du pushar?

```
  Du kör git push
        │
        ▼
  GitHub tar emot dina commits
        │
        ▼
  GitHub Actions startar en tom dator på internet
        │
        ├── 1. Hämtar din kod
        ├── 2. Installerar .NET
        ├── 3. Bygger allt och kör alla tester
        ├── 4. Gör en tabell med resultatet
        └── 5. Granskar kodstilen och ger tips
        │
        ▼
  ✅ eller ❌ visas vid din commit
```

Instruktionerna för stegen ligger i filen
`.github/workflows/autorattning.yml`. Det tar ungefär en minut.

### Så ser du resultatet
1. Gå till **din fork** på GitHub.
2. Klicka på fliken **Actions**.
3. Klicka på den översta körningen.
4. Scrolla ner till **Resultat för autorättningen**. Där finns en tabell
   med alla tester, ✅ eller ❌, och en kommentar om vad som är fel.

Du ser också en liten ✓ eller ✗ bredvid din senaste commit på forkens
startsida.

### Om körningen är röd
Det betyder att **minst ett** test inte gick igenom. Det är helt normalt
så länge du inte är klar med alla övningar. Titta i tabellen för att se
**vilka** tester som är röda.

Står det *"Koden gick inte att kompilera"* finns det ett kompileringsfel
någonstans. Då kan inga tester köras alls. Bygg i Visual Studio och kolla
**Error List**.

---

## Din dashboard (på din egen dator)

Du kan se hur långt du har kommit utan att pusha och utan internet.
Dubbelklicka på **`dashboard.cmd`** i repomappen, eller kör:

```powershell
dotnet run --project verktyg/Dashboard
```

Verktyget bygger all din kod, kör alla tester och öppnar sidan
`min-dashboard/index.html` i webbläsaren. Där finns:

- **Förloppsstaplar** för varje övning, och totalt
- **Nästa steg**: det första testet som inte är grönt än, med en förklaring
- **Utveckling**: en graf över hur många tester du hade gröna varje gång du körde dashboarden
- **Kodtips**: samma tips som i GitHub Actions
- **Alla tester**, övning för övning
- **Uppslagsbok**: sök efter C#-ord, metoder, felmeddelanden, git- och dotnet-kommandon (tryck **/** för att söka)

Mappen `min-dashboard` finns bara på din dator. Den laddas inte upp till
GitHub. Vill du bara visa senaste resultatet utan att köra testerna igen:
`dotnet run --project verktyg/Dashboard -- --utan-test`

Uppslagsboken finns också som en egen fil: öppna **`uppslagsbok.html`** i
repomappen.

---

## Tips om kodstil

Samtidigt som testerna körs granskas din kod mot kursens
[kodstandard](kodstandard.md). Tipsen syns i sammanfattningen under
**💡 Tips om kodstil**, som små notiser i koden på GitHub, och på din lokala
dashboard. **Tipsen påverkar aldrig rättningen.**

---

## Varför testar man på GitHub också?

- **Läraren ser samma resultat som du**, utan att behöva ladda ner din kod.
- **Det är rättvist.** Alla rättas på exakt samma sätt, på en ren dator.
- **Det är så det fungerar på riktigt.** De flesta företag kör tester
  automatiskt varje gång någon pushar kod. Det kallas *CI*, Continuous
  Integration.

---

## Tester eller Provkör?

| | Provkör | Tester |
|---|---|---|
| Vem väljer värdena? | Du | Läraren |
| Svar | Skrivs ut, du bedömer själv | Rätt eller fel |
| Bäst för | Att förstå och experimentera | Att kontrollera att allt är rätt |

Använd **båda**: provkör medan du jobbar och kör testerna när du tror att
du är klar. Se [Testkör din kod själv](testkora.md).
