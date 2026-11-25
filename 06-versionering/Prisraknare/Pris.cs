namespace Prisraknare;

/// <summary>
/// Prisraknare – ett litet bibliotek som räknar på priser.
/// Du är den som förvaltar biblioteket. Se README.md för ärendena du ska ta hand om.
/// </summary>
public static class Pris
{
    /// <summary>
    /// Returnerar priset med 25 % moms.
    ///   MedMoms(100)  ->  125
    /// </summary>
    public static double MedMoms(double pris)
    {
        return pris * 1.25;
    }

    /// <summary>
    /// Returnerar priset efter rabatt. Rabatten anges i procent.
    ///   Rabatt(200, 10)  ->  180
    /// </summary>
    public static double Rabatt(double pris, double procent)
    {
        return pris * procent / 100;
    }
}
