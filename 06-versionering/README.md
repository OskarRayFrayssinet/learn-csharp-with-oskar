# Övning 06: Versionering

Du är **förvaltare** av biblioteket `Prisraknare`. Andra programmerare
använder det i sina program. Nu kommer det in buggrapporter och önskemål,
och för varje ändring ska du bestämma vilken **version** biblioteket får.

**Läs först:** [Guide: Versionering](../guider/versionering.md)

| Del | Vad du gör |
|---|---|
| **A – Förstå** | Svara på 10 frågor i [`svar.txt`](svar.txt) |
| **B – Ärenden** | Gör fem ändringar i [`Prisraknare/Pris.cs`](Prisraknare/Pris.cs) |
| **C – Versioner** | För varje ärende: höj versionen, skriv i [`CHANGELOG.md`](CHANGELOG.md) och sätt en git-tagg |

Allt rättas automatiskt. Del B och C hänger ihop: varje ärende ger **en**
ny version.

---

## Del A: Frågor

Skriv bokstaven för ditt svar i [`svar.txt`](svar.txt), till exempel `Fråga 1: B`.

**Fråga 1.** Ett bibliotek har version `1.4.2`. Du rättar en bugg utan att ändra hur man använder biblioteket. Vilken blir nästa version?
- A) `1.4.3`
- B) `1.5.0`
- C) `2.0.0`
- D) `1.4.2.1`

**Fråga 2.** Version `2.3.7`. Du lägger till en ny metod. All gammal kod fungerar som förut. Nästa version?
- A) `2.3.8`
- B) `3.0.0`
- C) `2.4.0`
- D) `2.4.7`

**Fråga 3.** Version `1.9.0`. Du tar bort en publik metod som andra använder. Nästa version?
- A) `1.10.0`
- B) `2.0.0`
- C) `1.9.1`
- D) `2.9.0`

**Fråga 4.** Version `3.2.1`. En metod returnerade `int` men returnerar nu `double`. Nästa version?
- A) `3.2.2`
- B) `3.3.0`
- C) `4.0.0`
- D) `4.2.1`

**Fråga 5.** Version `1.2.5`. Du lägger till en ny funktion. Vad händer med sista siffran (PATCH)?
- A) Den blir kvar: `1.3.5`
- B) Den nollställs: `1.3.0`
- C) Den blir 1: `1.3.1`
- D) Den ökar också: `1.3.6`

**Fråga 6.** Vad betyder det att ett bibliotek har version `0.4.0`?
- A) Att det är stabilt och färdigt
- B) Att det är i en tidig utvecklingsfas och vad som helst kan ändras
- C) Att det är gratis
- D) Att det har 4 användare

**Fråga 7.** Version `2.1.0`. Du gör en metod snabbare. Den ger exakt samma svar som förut. Nästa version?
- A) `2.2.0`
- B) `3.0.0`
- C) `2.1.1`
- D) Ingen ny version behövs

**Fråga 8.** Vilken version är högst?
- A) `1.10.0`
- B) `1.9.0`
- C) `1.2.10`
- D) `1.9.9`

**Fråga 9.** Varför är det viktigt att höja MAJOR när en ändring inte är bakåtkompatibel?
- A) För att versionsnumret ska se större ut
- B) För att GitHub kräver det
- C) För att det går snabbare att bygga
- D) För att den som använder biblioteket ska förstå att deras kod kan sluta fungera

**Fråga 10.** Vad är `2.0.0-beta.1`?
- A) En förhandsversion (pre-release) som kommer före `2.0.0`
- B) En version som kommer efter `2.0.0`
- C) En ogiltig version
- D) Samma sak som `2.0.0`

---

## Del B och C: Ärendena

Ta ärendena **i ordning**. Biblioteket är version **1.0.0** när du börjar.

### Arbetsgång för VARJE ärende

```
1. Läs ärendet
2. Ändra koden i Prisraknare/Pris.cs
3. Kör testerna. Testet för ärendet ska bli grönt
4. Bestäm: är det PATCH, MINOR eller MAJOR?
5. Höj <Version> i Prisraknare/Prisraknare.csproj
6. Skriv den nya versionen ÖVERST i CHANGELOG.md
7. Committa:   git commit -am "Ärende 1: Rätta rabattberäkningen"
8. Tagga:      git tag v1.0.1
9. Pusha:      git push   och   git push --tags
```

Exempel på hur en ny version skrivs i `CHANGELOG.md` (överst, under texten):
```markdown
## [1.0.1] - 2026-11-26
### Fixat
- `Rabatt` räknade ut rabattens storlek i stället för priset efter rabatt.
```
Kategorierna är **Tillagt**, **Ändrat**, **Borttaget** och **Fixat**.

---

### 🐛 Ärende 1: Rabatten blir konstig
> *Från: Kim, som använder Prisraknare i sin webbutik*
>
> När jag anropar `Rabatt(200, 10)` får jag **20**. Men en vara som kostar
> 200 kr med 10 % rabatt borde kosta **180** kr!

**Förväntat resultat:** `Rabatt(200, 10)` → `180`, `Rabatt(80, 25)` → `60`

---

### ✨ Ärende 2: Avrunda till hela kronor
> *Från: Sara, kassasystem*
>
> Vi skulle behöva en metod som avrundar ett pris till hela kronor. Kan ni
> lägga till `Avrunda(pris)`?

**Förväntat resultat:**
`Avrunda(99.5)` → `100`, `Avrunda(99.4)` → `99`, `Avrunda(2.5)` → `3`

**Tips:** `Math.Round(2.5)` ger faktiskt `2` i C#, eftersom den avrundar
till närmaste jämna tal. Använd
`Math.Round(pris, MidpointRounding.AwayFromZero)`.

---

### ✨ Ärende 3: Olika momssatser
> *Från: Ali, bokhandel*
>
> Böcker har 6 % moms, inte 25 %. Kan `MedMoms` ta emot momssatsen?
> **Men:** vår gamla kod använder `MedMoms(pris)` på hundra ställen, så den
> får inte sluta fungera!

**Förväntat resultat:**
`MedMoms(100, 6)` → `106`, `MedMoms(200, 12)` → `224`, och `MedMoms(100)` → `125` som förut

**Tips:** lägg till en **ny** metod med samma namn men två parametrar
(det kallas *överlagring*). Behåll den gamla.

---

### 💥 Ärende 4: Tydligare namn
> *Från: teamet som förvaltar Prisraknare (alltså du)*
>
> `Rabatt` låter som att metoden returnerar rabatten, men den returnerar
> priset **efter** rabatt. Byt namn till `MedRabatt`, så att den matchar `MedMoms`.

**Förväntat resultat:** `MedRabatt(200, 10)` → `180`, och `Rabatt` finns inte längre.

**Lägg märke till:** när du har bytt namn slutar `Prisraknare.Provkor`
att gå att bygga. Det använder ju `Rabatt`! Det är precis så det blir för
alla som använder ditt bibliotek. Rätta Provkör så att den använder
`MedRabatt`. Vilken del av versionen ska höjas när andras kod slutar
fungera?

---

### 🐛 Ärende 5: Negativa priser
> *Från: Kim igen*
>
> Av misstag skrev vi in 150 % rabatt, och då blev priset **−50 kr**. Ett
> pris kan väl inte vara negativt?

**Förväntat resultat:** `MedRabatt(100, 150)` → `0`, `MedRabatt(100, 20)` → `80`

---

## När du är klar
- `Prisraknare.csproj` har den sista versionen.
- `CHANGELOG.md` har sex versioner, med den nyaste överst.
- Du har fem taggar: `git tag` visar dem.

Se vad du har gjort:
```powershell
git log --oneline --decorate
```
Taggarna syns bredvid dina commits, precis som riktiga releaser.

---

## Testerna
Det finns 23 tester: 10 för Del A, 5 för Del B och 8 för Del C.
Testnamnen börjar med `06 ·`.
