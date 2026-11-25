# Guide: Testkör din kod själv

Testerna säger **om** din kod är rätt. För att förstå **varför** den blir
fel behöver du kunna köra den själv, med egna värden, och titta på vad som
händer. Här är tre sätt, från enklast till mest kraftfullt.

| Sätt | Bäst när du vill | Kortkommando |
|---|---|---|
| 1. Provkör | Prova en metod med egna värden | ▶ / F5 |
| 2. Skriv ut | Se ett värde mitt i en loop | – |
| 3. Debuggern | Stega igenom koden rad för rad | F9, F5, F10 |

---

## 1. Provkör-projektet

Varje övning har ett eget litet program, ett **Provkör-projekt**, som
anropar dina metoder och skriver ut svaren. Det rättas inte, så du får
ändra hur du vill.

| Övning | Provkör-projekt |
|---|---|
| 01 Grunder | `01-grunder/Grunder.Provkor` |
| 02 Sortera en lista | `02-sortera-lista/SorteraLista.Provkor` |
| 04 Träning | `04-traning/Traning.Provkor` |
| 06 Versionering | `06-versionering/Prisraknare.Provkor` |

### Så kör du
1. Högerklicka på Provkör-projektet i **Solution Explorer** och välj
   **Set as Startup Project**. Projektnamnet blir fetstilt.
2. Tryck på **▶ Start** (eller **F5**).

Ett konsolfönster visar resultatet:
```
StorstAvTva(3, 8)              = 8
ArJamnt(7)                     = False
Betyg(85)                      = (inte gjord ännu)
```

### Ändra värdena
Byt talen i `Program.cs` och kör igen. Prova särskilt **kantfall**,
värden där det ofta blir fel:
- `0` och negativa tal
- en tom array: `new int[0]`
- värden precis på en gräns, till exempel `Betyg(90)` och `Betyg(89)`

Det är just sådana värden testerna använder.

> **Varför går ▶ att använda här?** Provkör-projektet är ett **program**
> (`OutputType Exe`), medan projektet med dina uppgifter är ett
> **klassbibliotek**. Läs mer i [guiden om filstrukturen](filstruktur.md).

---

## 2. Skriv ut värden mitt i koden

Lägg en `Console.WriteLine` **inne i din egen metod** för att se vad som
händer i varje varv:

```csharp
public static int SummaUppTill(int n)
{
    int summa = 0;
    for (int i = 1; i <= n; i++)
    {
        summa += i;
        Console.WriteLine($"i = {i}, summa = {summa}");   // tillfällig utskrift
    }
    return summa;
}
```

Kör Provkör, så syns utskriften för varje varv:
```
i = 1, summa = 1
i = 2, summa = 3
i = 3, summa = 6
i = 4, summa = 10
```

Ta bort utskriften när du har hittat felet. Testerna bryr sig inte om
utskrifter, men koden blir lättare att läsa utan dem.

---

## 3. Debuggern: stega igenom koden

Med debuggern kan du **pausa** programmet på en rad och titta på alla
variabler. Det fungerar även i ditt klassbibliotek, så länge du startar
via Provkör.

### Steg för steg
1. Öppna filen med din metod, till exempel `Grunder.cs`.
2. Klicka i **vänstermarginalen** bredvid en rad, eller tryck **F9**.
   En röd prick (**brytpunkt**) dyker upp.
3. Tryck **F5**. Programmet stannar på den röda raden.
4. Håll musen över en variabel för att se dess värde, eller titta i
   fönstret **Locals** längst ner.
5. Tryck **F10** för att köra en rad i taget och se hur värdena ändras.

### Kortkommandon

| Tangent | Gör |
|---|---|
| **F9** | Sätt eller ta bort en brytpunkt |
| **F5** | Starta med debugger, eller fortsätt till nästa brytpunkt |
| **F10** | Kör nästa rad (*step over*) |
| **F11** | Gå in i metoden som anropas på raden (*step into*) |
| **Shift+F5** | Stoppa |
| **Ctrl+F5** | Starta utan debugger (snabbare, stannar inte på brytpunkter) |

### Debugga ett test som blir rött
Du kan också starta debuggern från ett test:
1. Sätt en brytpunkt i din metod.
2. Högerklicka på det röda testet i **Test Explorer** och välj **Debug**.
3. Programmet stannar i din metod, med exakt de värden testet använder.

---

## Läsa ett testfel

Klicka på ett rött test i **Test Explorer**. Längst ner står till exempel:

```
Assert.Equal() Failure: Values differ
Expected: 10
Actual:   6
```

- **Expected** är det testet förväntade sig.
- **Actual** är det din metod gav.

Ta reda på vilka värden testet skickade in. Dubbelklicka på testet så
öppnas testkoden. Prova sedan samma värden i Provkör eller i debuggern.

---

## Ett bra arbetssätt

1. **Skriv** koden för en uppgift.
2. **Provkör** med exemplet från uppgiften. Blir det rätt?
3. **Provkör** med egna kantfall: 0, negativa tal, tom array.
4. Blir något konstigt? **Skriv ut** eller **debugga**.
5. **Kör testerna** när du tror att allt stämmer.
6. **Committa och pusha.**
