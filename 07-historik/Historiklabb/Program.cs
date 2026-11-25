// =========================================================================
//  HISTORIKLABBET
//  Skapar små färdiga projekt med en historik som du ska rätta till.
//
//  Kör från repots rotmapp:
//    dotnet run --project 07-historik/Historiklabb              (meny)
//    dotnet run --project 07-historik/Historiklabb -- skapa 1
//    dotnet run --project 07-historik/Historiklabb -- kontrollera 1
//
//  Du behöver inte ändra något i det här projektet.
// =========================================================================

using System.Text;
using Historiklabb;

Console.OutputEncoding = Encoding.UTF8;
var labb = new Labb(Labb.HittaOvningsmapp());

if (args.Length > 0)
{
    return KorKommando(args);
}

// Ingen parameter: visa en meny.
while (true)
{
    VisaLista();
    Console.WriteLine();
    Console.WriteLine("Skriv till exempel:  skapa 1   kontrollera 1   visa 1   (eller avsluta)");
    Console.Write("> ");
    var rad = Console.ReadLine();
    if (rad == null || rad.Trim() is "avsluta" or "q" or "")
    {
        return 0;
    }
    KorKommando(rad.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    Console.WriteLine();
    Console.WriteLine("Tryck Enter för att fortsätta...");
    Console.ReadLine();
}

int KorKommando(string[] delar)
{
    var kommando = delar[0].ToLowerInvariant();
    if (kommando == "lista")
    {
        VisaLista();
        return 0;
    }

    if (delar.Length < 2 || !int.TryParse(delar[1], out var nummer) || Scenarier.Hitta(nummer) is not { } scenario)
    {
        Console.WriteLine($"Okänt kommando eller scenario. Scenarier finns från 1 till {Scenarier.Alla.Count}.");
        return 1;
    }

    switch (kommando)
    {
        case "skapa":
            var mapp = labb.Skapa(scenario);
            var relativ = Path.GetRelativePath(Path.GetDirectoryName(labb.Ovningsmapp)!, mapp);
            Console.WriteLine(Labb.Beskrivning(scenario));
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("Scenariot är skapat. Gå till mappen så här (från repots rotmapp):");
            Console.WriteLine();
            Console.WriteLine($"    cd {relativ}");
            Console.WriteLine();
            Console.WriteLine("Börja med git status och git log --oneline --all --graph");
            return 0;

        case "visa":
            Console.WriteLine(Labb.Beskrivning(scenario));
            return 0;

        case "kontrollera":
            var fel = labb.Kontrollera(scenario);
            Console.WriteLine($"Kontrollerar scenario {scenario.Nummer}: {scenario.Titel}");
            Console.WriteLine();
            if (fel.Count == 0)
            {
                Console.WriteLine("  OK  Snyggt! Scenariot är löst.");
                Console.WriteLine();
                Console.WriteLine($"  Ett kvitto har sparats i 07-historik/kvitton/scenario-{scenario.Nummer:00}.json");
                Console.WriteLine("  Committa och pusha det i ditt vanliga repo, så syns det i autorättningen.");
                return 0;
            }
            Console.WriteLine("  Inte klart än:");
            foreach (var f in fel)
            {
                Console.WriteLine($"  -  {f}");
            }
            return 1;

        default:
            Console.WriteLine("Okänt kommando. Använd skapa, kontrollera, visa eller lista.");
            return 1;
    }
}

void VisaLista()
{
    Console.WriteLine("HISTORIKLABBET");
    Console.WriteLine("==============");
    foreach (var s in Scenarier.Alla)
    {
        var klar = labb.ArKlar(s) ? "[klar]" : "[    ]";
        Console.WriteLine($" {klar} {s.Nummer,2}. {s.Titel} ({s.Niva})");
    }
}
