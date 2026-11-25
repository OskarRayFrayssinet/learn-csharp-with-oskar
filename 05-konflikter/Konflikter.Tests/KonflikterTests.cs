using Konfliktlabb;

namespace Konflikter.Tests;

// ÄNDRA INTE I DEN HÄR FILEN.

public class DelA_Konfliktlabbet
{
    [Fact(DisplayName = "05 · A · Scenario 1: Samma rad – välj kompisens")]
    public void S01() => KollaKvitto(1);

    [Fact(DisplayName = "05 · A · Scenario 2: Samma rad – behåll din egen")]
    public void S02() => KollaKvitto(2);

    [Fact(DisplayName = "05 · A · Scenario 3: Båda ändringarna behövs")]
    public void S03() => KollaKvitto(3);

    [Fact(DisplayName = "05 · A · Scenario 4: Två konflikter i samma fil")]
    public void S04() => KollaKvitto(4);

    [Fact(DisplayName = "05 · A · Scenario 5: Konflikt i C#-kod")]
    public void S05() => KollaKvitto(5);

    [Fact(DisplayName = "05 · A · Scenario 6: Kombinera två ändringar")]
    public void S06() => KollaKvitto(6);

    [Fact(DisplayName = "05 · A · Scenario 7: Konflikter i två filer")]
    public void S07() => KollaKvitto(7);

    [Fact(DisplayName = "05 · A · Scenario 8: Raderad eller ändrad")]
    public void S08() => KollaKvitto(8);

    [Fact(DisplayName = "05 · A · Scenario 9: Samma nya fil")]
    public void S09() => KollaKvitto(9);

    [Fact(DisplayName = "05 · A · Scenario 10: Avbryt en merge")]
    public void S10() => KollaKvitto(10);

    [Fact(DisplayName = "05 · A · Scenario 11: Hämta från läraren")]
    public void S11() => KollaKvitto(11);

    [Fact(DisplayName = "05 · A · Scenario 12: Tre konflikter i C#")]
    public void S12() => KollaKvitto(12);

    [Fact(DisplayName = "05 · A · Scenario 13: Delat repo – din push nekas")]
    public void S13() => KollaKvitto(13);

    [Fact(DisplayName = "05 · A · Scenario 14: Delat repo – kombinera kod")]
    public void S14() => KollaKvitto(14);

    private static void KollaKvitto(int nummer)
    {
        var scenario = Scenarier.Hitta(nummer)!;
        var labb = new Labb(Labb.HittaOvningsmapp());
        var sokvag = Kvitto.Sokvag(labb.Kvittomapp, scenario);

        Assert.True(File.Exists(sokvag),
            $"Inte gjort ännu. Skapa scenariot med Konfliktlabbet, lös det, kör kontrollera och pusha kvittot.");

        var filer = Kvitto.Las(sokvag);
        var fel = filer.SelectMany(f => Regler.Konfliktmarkeringar(f.Key, f.Value)).ToList();
        fel.AddRange(scenario.KontrolleraInnehall(filer));
        Assert.True(fel.Count == 0, "Kvittot stämmer inte: " + string.Join(" ", fel));
    }
}

public class DelB_KonfliktMedLararen
{
    private static readonly string[] GamlaRubriker = { "Namn:", "Favoritfrukt:", "Det bästa med programmering:" };
    private static readonly string[] NyaRubriker = { "Namn (bara förnamn):", "Favoritfrukt eller favoritgrönsak:", "Det bästa med programmering:" };

    [Fact(DisplayName = "05 · B · 1: Du har fyllt i klassrummet.txt")]
    public void Ifylld()
    {
        var rader = LasRader();
        var fel = Regler.Konfliktmarkeringar("klassrummet.txt", string.Join("\n", rader));
        Assert.True(fel.Count == 0, string.Join(" ", fel));

        for (int i = 0; i < 3; i++)
        {
            var svar = HittaSvar(rader, GamlaRubriker[i]) ?? HittaSvar(rader, NyaRubriker[i]);
            Assert.True(!string.IsNullOrWhiteSpace(svar),
                $"Fyll i ett svar efter \"{GamlaRubriker[i]}\" i 05-konflikter/klassrummet.txt.");
        }
    }

    [Fact(DisplayName = "05 · B · 2: Lärarens ändring är hämtad och konflikten löst")]
    public void LararensAndring()
    {
        var rader = LasRader();
        var fel = Regler.Konfliktmarkeringar("klassrummet.txt", string.Join("\n", rader));
        Assert.True(fel.Count == 0, string.Join(" ", fel));

        Assert.True(HittaSvar(rader, NyaRubriker[0]) != null,
            "Lärarens nya rubriker finns inte i filen än. Vänta tills läraren har gjort sin ändring, " +
            "hämta den med git pull --no-rebase upstream main och lös konflikten.");

        for (int i = 0; i < 3; i++)
        {
            var svar = HittaSvar(rader, NyaRubriker[i]);
            Assert.True(!string.IsNullOrWhiteSpace(svar),
                $"Raden \"{NyaRubriker[i]}\" ska finnas och ha ditt svar efter kolonet.");
        }

        Assert.True(!rader.Any(r => r.StartsWith("Namn:") || r.StartsWith("Favoritfrukt:")),
            "De gamla raderna (Namn: och Favoritfrukt:) ska vara borta. Använd lärarens nya rubriker med dina svar.");
    }

    private static List<string> LasRader()
    {
        var fil = Path.Combine(Labb.HittaOvningsmapp(), "klassrummet.txt");
        return File.ReadAllLines(fil).Select(r => r.TrimEnd()).ToList();
    }

    private static string? HittaSvar(List<string> rader, string rubrik) =>
        rader.FirstOrDefault(r => r.StartsWith(rubrik))?[rubrik.Length..].Trim();
}
