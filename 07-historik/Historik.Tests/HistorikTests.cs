using Historiklabb;

namespace Historik.Tests;

// ÄNDRA INTE I DEN HÄR FILEN.
// Varje test läser kvittot från Historiklabbet och kontrollerar historiken igen.

public class Historiklabbet
{
    [Fact(DisplayName = "07 · Scenario 1: Fel meddelande i senaste commit")]
    public void S01() => KollaKvitto(1);

    [Fact(DisplayName = "07 · Scenario 2: Glömd fil i senaste commit")]
    public void S02() => KollaKvitto(2);

    [Fact(DisplayName = "07 · Scenario 3: Ångra en gammal commit med revert")]
    public void S03() => KollaKvitto(3);

    [Fact(DisplayName = "07 · Scenario 4: Hitta buggen själv och reverta")]
    public void S04() => KollaKvitto(4);

    [Fact(DisplayName = "07 · Scenario 5: Leta med git bisect")]
    public void S05() => KollaKvitto(5);

    [Fact(DisplayName = "07 · Scenario 6: Byt meddelande på en äldre commit")]
    public void S06() => KollaKvitto(6);

    [Fact(DisplayName = "07 · Scenario 7: Slå ihop småcommits")]
    public void S07() => KollaKvitto(7);

    [Fact(DisplayName = "07 · Scenario 8: Ta bort en commit")]
    public void S08() => KollaKvitto(8);

    [Fact(DisplayName = "07 · Scenario 9: Ta bort ett lösenord ur historiken")]
    public void S09() => KollaKvitto(9);

    [Fact(DisplayName = "07 · Scenario 10: Återställ en raderad fil")]
    public void S10() => KollaKvitto(10);

    [Fact(DisplayName = "07 · Scenario 11: Plocka en commit från en annan gren")]
    public void S11() => KollaKvitto(11);

    [Fact(DisplayName = "07 · Scenario 12: Commits på fel gren")]
    public void S12() => KollaKvitto(12);

    private static void KollaKvitto(int nummer)
    {
        var scenario = Scenarier.Hitta(nummer)!;
        var labb = new Labb(Labb.HittaOvningsmapp());
        var kvitto = Kvitto.Las(Kvitto.Sokvag(labb.Kvittomapp, scenario));

        Assert.True(kvitto != null,
            "Inte gjort ännu. Skapa scenariot med Historiklabbet, lös det, kör kontrollera och pusha kvittot.");
        Assert.True(kvitto!.Scenario == nummer, "Kvittot hör till fel scenario.");

        var fel = scenario.Kontrollera(kvitto.Fore, kvitto.Efter);
        Assert.True(fel.Count == 0, "Kvittot stämmer inte: " + string.Join(" ", fel));
    }
}
