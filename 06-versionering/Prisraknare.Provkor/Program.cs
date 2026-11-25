// =========================================================================
//  PROVKÖR – Prisraknare
//
//  Det här programmet ANVÄNDER biblioteket Prisraknare, precis som en
//  annan programmerare som har laddat ner ditt bibliotek skulle göra.
//
//  Så kör du:  högerklicka på Prisraknare.Provkor -> Set as Startup Project -> ▶
//
//  Lägg märke till vad som händer med det här programmet när du gör
//  ändringar i biblioteket. Fortsätter det att fungera, eller inte?
// =========================================================================

using Prisraknare;

Console.WriteLine("=== Prisraknare ===");
Console.WriteLine($"MedMoms(100)     = {Pris.MedMoms(100)}");
Console.WriteLine($"Rabatt(200, 10)  = {Pris.Rabatt(200, 10)}");
