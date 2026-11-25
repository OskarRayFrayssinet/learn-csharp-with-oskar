"""
Automatisk kodgranskning med tips på svenska.

Läser varningarna från bygget (kodstilsregler i .editorconfig) och gör några
egna kontroller av elevens kod. Skriver:
  - en tabell till GitHub Actions-sammanfattningen ($GITHUB_STEP_SUMMARY, annars skärmen)
  - anteckningar (annotations) som syns i koden på GitHub (stdout)
  - kodtips.json som dashboarden använder

Tipsen påverkar INTE rättningen.

Användning: python3 kodgranskning.py <bygglogg> <kodtips.json>
"""
import json
import os
import re
import sys

GUIDE = "guider/kodstandard.md"

FORKLARING = {
    "IDE0055": ("Indrag eller mellanslag följer inte standarden.",
                "Formatera filen: Ctrl+K, Ctrl+D i Visual Studio eller Shift+Alt+F i VS Code."),
    "IDE0011": ("Klamrar { } saknas efter if, else, for eller while.",
                "Sätt alltid { } runt koden, även om det bara är en rad."),
    "IDE1006": ("Namnet följer inte namnstandarden.",
                "Klasser och metoder: StorBokstav (PascalCase). Variabler och parametrar: litenBokstav (camelCase)."),
    "IDE0059": ("Ett värde sparas i en variabel men används aldrig.",
                "Ta bort tilldelningen eller använd variabeln."),
    "IDE0051": ("En privat metod eller variabel används aldrig.",
                "Ta bort den om den inte behövs."),
    "IDE0052": ("En privat variabel får ett värde men läses aldrig.",
                "Ta bort den om den inte behövs."),
    "CS0168": ("En variabel deklareras men används aldrig.",
               "Ta bort variabeln."),
    "CS0219": ("En variabel får ett värde men används aldrig.",
               "Ta bort variabeln eller använd den."),
    "CS0162": ("Koden kan aldrig köras, till exempel för att den står efter return.",
               "Ta bort koden eller flytta den."),
    "STIL01": ("Console.WriteLine i ett klassbibliotek.",
               "Metoder i biblioteket ska returnera svaret, inte skriva ut det. Skriv ut i Provkör i stället."),
    "STIL02": ("Variabelnamn med en enda bokstav.",
               "Välj ett namn som säger vad variabeln innehåller, t.ex. summa i stället för s. (i, j och k i loopar är okej.)"),
    "STIL03": ("Bortkommenterad kod.",
               "Ta bort gammal kod i stället för att kommentera bort den. Git kommer ihåg den åt dig."),
    "STIL04": ("Raden är längre än 120 tecken.",
               "Dela upp raden på flera rader, eller använd en hjälpvariabel."),
}

VARNING = re.compile(r"^(?P<fil>.+?\.cs)\((?P<rad>\d+),(?P<kol>\d+)\): warning (?P<kod>(IDE|CS)\d+): (?P<text>.*?)(?: \[.*\])?$")
EN_BOKSTAV = re.compile(r"\b(?:int|double|string|bool|char|var|long|float|decimal)\s+([a-zA-Z])\s*(?:=(?![=>])|;)")
BORTKOMMENTERAD = re.compile(r"^\s*//(?!/)\s*.*(;|\{|\})\s*$")


def ar_larar_eller_test(sokvag):
    delar = sokvag.replace("\\", "/").split("/")
    return any(d.endswith(".Tests") or d in ("Konfliktlabb", "bin", "obj", ".github", "labb") for d in delar)


def projekt_for(fil):
    mapp = os.path.dirname(fil)
    while mapp and mapp != os.path.dirname(mapp):
        for namn in os.listdir(mapp):
            if namn.endswith(".csproj"):
                return os.path.join(mapp, namn)
        mapp = os.path.dirname(mapp)
    return None


def ar_bibliotek(fil):
    projekt = projekt_for(fil)
    if projekt is None:
        return False
    with open(projekt, encoding="utf-8-sig") as f:
        return "<OutputType>Exe</OutputType>" not in f.read()


def elevfiler(rot):
    for mapp, undermappar, filer in os.walk(rot):
        undermappar[:] = [u for u in undermappar if not u.startswith(".") and u not in ("bin", "obj", "labb")]
        for fil in filer:
            sokvag = os.path.join(mapp, fil)
            if fil.endswith(".cs") and not ar_larar_eller_test(os.path.relpath(sokvag, rot)):
                yield sokvag


def egna_kontroller(rot):
    tips = []
    for fil in elevfiler(rot):
        relativ = os.path.relpath(fil, rot).replace("\\", "/")
        bibliotek = ar_bibliotek(fil)
        with open(fil, encoding="utf-8-sig", errors="replace") as f:
            rader = f.read().splitlines()
        for nr, rad in enumerate(rader, start=1):
            kod = rad.split("//")[0]
            if bibliotek and "Console.Write" in kod:
                tips.append((relativ, nr, "STIL01"))
            for namn in EN_BOKSTAV.findall(kod):
                if namn not in ("i", "j", "k"):
                    tips.append((relativ, nr, "STIL02"))
            if bibliotek and BORTKOMMENTERAD.match(rad):
                tips.append((relativ, nr, "STIL03"))
            if len(rad) > 120:
                tips.append((relativ, nr, "STIL04"))
    return tips


def byggvarningar(logg, rot):
    tips = []
    if not os.path.exists(logg):
        return tips
    with open(logg, encoding="utf-8", errors="replace") as f:
        for rad in f:
            traff = VARNING.match(rad.strip())
            if not traff:
                continue
            relativ = os.path.relpath(traff["fil"], rot).replace("\\", "/")
            if relativ.startswith("..") or ar_larar_eller_test(relativ):
                continue
            tips.append((relativ, int(traff["rad"]), traff["kod"]))
    return tips


def main():
    logg = sys.argv[1] if len(sys.argv) > 1 else "testlogg.txt"
    utfil = sys.argv[2] if len(sys.argv) > 2 else "kodtips.json"
    rot = os.getcwd()

    alla = sorted(set(byggvarningar(logg, rot) + egna_kontroller(rot)))

    per_kod = {}
    for _, _, kod in alla:
        per_kod[kod] = per_kod.get(kod, 0) + 1
    with open(utfil, "w", encoding="utf-8") as f:
        json.dump({"antal": len(alla), "per_kod": per_kod,
                   "tips": [{"fil": fil, "rad": rad, "kod": kod} for fil, rad, kod in alla[:200]]},
                  f, ensure_ascii=False, indent=1)

    rader = ["## 💡 Tips om kodstil\n",
             "_Tipsen påverkar **inte** rättningen. De hjälper dig att skriva kod som är lätt att läsa._\n"]
    if not alla:
        rader.append("✨ Inga tips. Din kod följer kodstandarden!")
    else:
        rader.append(f"**{len(alla)} tips** hittades. Läs mer i [guiden om kodstandard]({GUIDE}).\n")
        rader.append("| Fil | Rad | Tips |")
        rader.append("|---|---|---|")
        for fil, rad, kod in alla[:40]:
            rader.append(f"| `{fil}` | {rad} | {FORKLARING.get(kod, (kod, ''))[0]} ({kod}) |")
        if len(alla) > 40:
            rader.append(f"\n…och {len(alla) - 40} till.")
        rader.append("\n### Så fixar du\n")
        for kod in sorted(per_kod):
            problem, losning = FORKLARING.get(kod, (kod, "Se felmeddelandet i Visual Studio."))
            rader.append(f"- **{kod}** ({per_kod[kod]} st): {problem} {losning}")

    sammanfattning = os.environ.get("GITHUB_STEP_SUMMARY")
    if sammanfattning:
        with open(sammanfattning, "a", encoding="utf-8") as f:
            f.write("\n".join(rader) + "\n")
    else:
        print("\n".join(rader))

    # Anteckningar som syns direkt i koden på GitHub (högst 10 per steg).
    for fil, rad, kod in alla[:10]:
        problem, losning = FORKLARING.get(kod, (kod, ""))
        print(f"::notice file={fil},line={rad},title=Kodstil {kod}::{problem} {losning}")


if __name__ == "__main__":
    main()
