"""
Bygger elevens personliga dashboard (Min progression) från testresultaten.

Användning:
  python3 dashboard.py <testresultat-mapp> <kodtips.json> <historik.csv> <utmapp>

Skriver README.md, historik.csv och sammanfattning.json till utmappen.
Workflowen pushar utmappen till grenen 'dashboard' i elevens fork.
"""
import csv
import datetime
import glob
import json
import os
import re
import sys
import xml.etree.ElementTree as ET

OVNINGAR = {
    "01": "Grunder – villkor, loopar och arrayer",
    "02": "Sortera en lista",
    "03": "Filstrukturen i C#",
    "04": "Träning – 40 små uppgifter",
    "05": "Merge-konflikter",
    "06": "Versionering",
}

STAPEL = 20
GNISTOR = "▁▂▃▄▅▆▇█"


def naturlig(text):
    return [int(d) if d.isdigit() else d for d in re.split(r"(\d+)", text)]


def ordning(namn):
    """Sorterar i uppgiftsordning: övning, del (A, B, C), nummer. Regler sist."""
    traff = re.match(r"(\d+) · (?:([A-Z]) · )?\D*?(\d+)", namn)
    if not traff:
        return (namn[:2], "", 999, namn)
    return (traff.group(1), traff.group(2) or "", int(traff.group(3)), namn)


def las_tester(mapp):
    tester = []
    for fil in glob.glob(os.path.join(mapp, "**", "*.trx"), recursive=True):
        for element in ET.parse(fil).iter():
            if element.tag.endswith("UnitTestResult"):
                meddelande = ""
                for barn in element.iter():
                    if barn.tag.endswith("Message") and barn.text:
                        meddelande = barn.text.strip().splitlines()[0]
                        break
                tester.append({"namn": element.get("testName", "?"),
                               "ok": element.get("outcome") == "Passed",
                               "meddelande": meddelande})
    tester.sort(key=lambda t: ordning(t["namn"]))
    return tester


def stapel(andel):
    fyllda = round(andel * STAPEL)
    return "█" * fyllda + "░" * (STAPEL - fyllda)


def procent(ok, totalt):
    return 0 if totalt == 0 else round(100 * ok / totalt)


def symbol(ok, totalt):
    if totalt and ok == totalt:
        return "🏆"
    if ok == 0:
        return "⬜"
    return "⏳"


def kort_meddelande(text):
    if "NotImplementedException" in text or "Inte påbörjad" in text:
        return "Inte påbörjad ännu"
    return text.replace("|", "\\|")[:160]


def main():
    trx_mapp, tips_fil, historik_fil, ut = sys.argv[1:5]
    os.makedirs(ut, exist_ok=True)

    tester = las_tester(trx_mapp)
    byggt = len(tester) > 0
    tips = {"antal": 0, "per_kod": {}}
    if os.path.exists(tips_fil):
        with open(tips_fil, encoding="utf-8") as f:
            tips = json.load(f)

    try:
        from zoneinfo import ZoneInfo
        nu = datetime.datetime.now(ZoneInfo("Europe/Stockholm"))
    except Exception:
        nu = datetime.datetime.now(datetime.timezone.utc)
    tid = nu.strftime("%Y-%m-%d %H:%M")

    repo = os.environ.get("GITHUB_REPOSITORY", "elev/learn-csharp-with-oskar")
    server = os.environ.get("GITHUB_SERVER_URL", "https://github.com")
    sha = os.environ.get("GITHUB_SHA", "")[:7]
    korning = f"{server}/{repo}/actions/runs/{os.environ.get('GITHUB_RUN_ID', '')}"
    elev = repo.split("/")[0]

    # Gruppera per övning ("01 · ...").
    grupper = {}
    for t in tester:
        nr = t["namn"][:2] if t["namn"][:2].isdigit() else "??"
        grupper.setdefault(nr, []).append(t)

    ok_totalt = sum(t["ok"] for t in tester)
    totalt = len(tester)

    # Historik
    historik = []
    if os.path.exists(historik_fil):
        with open(historik_fil, encoding="utf-8") as f:
            historik = [r for r in csv.DictReader(f)]
    if byggt:
        historik.append({"tid": tid, "commit": sha, "godkanda": str(ok_totalt), "totalt": str(totalt)})
    historik = historik[-100:]
    with open(os.path.join(ut, "historik.csv"), "w", encoding="utf-8", newline="") as f:
        skrivare = csv.DictWriter(f, fieldnames=["tid", "commit", "godkanda", "totalt"])
        skrivare.writeheader()
        skrivare.writerows(historik)

    r = []
    r.append(f"# 📊 Min progression – {elev}\n")
    r.append(f"Senast uppdaterad **{tid}** · commit `{sha}` · [se hela körningen]({korning})\n")

    if not byggt:
        r.append("> ⚠️ **Koden gick inte att bygga vid senaste pushen**, så inga tester kunde köras.")
        r.append("> Bygg i Visual Studio (Ctrl+Shift+B) och rätta felen i **Error List**.\n")
    else:
        r.append("## Totalt\n")
        r.append("```")
        r.append(f"{stapel(ok_totalt / totalt if totalt else 0)}  {procent(ok_totalt, totalt)} %   ({ok_totalt} av {totalt} tester)")
        r.append("```\n")

        r.append("## Övningar\n")
        r.append("| | Övning | Framsteg | Klara |")
        r.append("|---|---|---|---|")
        for nr in sorted(grupper):
            lista = grupper[nr]
            ok = sum(t["ok"] for t in lista)
            namn = OVNINGAR.get(nr, f"Övning {nr}")
            r.append(f"| {symbol(ok, len(lista))} | **{nr}** {namn} | `{stapel(ok / len(lista))}` | {ok}/{len(lista)} |")
        r.append("")

        nasta = next((t for t in tester if not t["ok"]), None)
        r.append("## 👉 Nästa steg\n")
        if nasta is None:
            r.append("🎉 **Alla tester är gröna!** Fantastiskt jobbat.\n")
        else:
            r.append(f"Jobba vidare med **{nasta['namn']}**")
            r.append(f"> {kort_meddelande(nasta['meddelande'])}\n")

    # Utveckling över tid
    if historik:
        andelar = [int(h["godkanda"]) / max(1, int(h["totalt"])) for h in historik[-30:]]
        gnista = "".join(GNISTOR[min(7, int(a * 7.999))] for a in andelar)
        r.append("## 📈 Utveckling\n")
        r.append(f"Senaste {len(andelar)} pusharna: `{gnista}`\n")
        r.append("| När | Commit | Godkända | Förändring |")
        r.append("|---|---|---|---|")
        senaste = historik[-10:]
        for i in range(len(senaste) - 1, -1, -1):
            h = senaste[i]
            fore = historik[len(historik) - len(senaste) + i - 1] if len(historik) - len(senaste) + i > 0 else None
            diff = int(h["godkanda"]) - int(fore["godkanda"]) if fore else 0
            tecken = f"+{diff}" if diff > 0 else (str(diff) if diff < 0 else "±0")
            r.append(f"| {h['tid']} | `{h['commit']}` | {h['godkanda']}/{h['totalt']} | {tecken} |")
        r.append("")

    # Kodstil
    r.append("## 💡 Kodstil\n")
    if tips.get("antal", 0) == 0:
        r.append("✨ Inga tips. Din kod följer kodstandarden!\n")
    else:
        r.append(f"**{tips['antal']} tips** vid senaste pushen. Tipsen påverkar inte rättningen.")
        r.append(f"Se alla i [körningens sammanfattning]({korning}).\n")
        for kod, antal in sorted(tips.get("per_kod", {}).items(), key=lambda x: -x[1]):
            r.append(f"- `{kod}`: {antal} st")
        r.append("")

    # Alla tester
    if byggt:
        r.append("## Alla tester\n")
        for nr in sorted(grupper):
            lista = grupper[nr]
            ok = sum(t["ok"] for t in lista)
            namn = OVNINGAR.get(nr, f"Övning {nr}")
            r.append(f"<details><summary>{symbol(ok, len(lista))} <b>{nr} {namn}</b> – {ok}/{len(lista)}</summary>\n")
            r.append("| | Test | Kommentar |")
            r.append("|---|---|---|")
            for t in lista:
                kommentar = "" if t["ok"] else kort_meddelande(t["meddelande"])
                r.append(f"| {'✅' if t['ok'] else '❌'} | {t['namn']} | {kommentar} |")
            r.append("\n</details>\n")

    r.append("---")
    r.append("_Den här sidan uppdateras automatiskt varje gång du pushar. Ändra inte i den._")

    with open(os.path.join(ut, "README.md"), "w", encoding="utf-8") as f:
        f.write("\n".join(r) + "\n")

    with open(os.path.join(ut, "sammanfattning.json"), "w", encoding="utf-8") as f:
        json.dump({
            "elev": elev, "uppdaterad": tid, "commit": sha, "byggt": byggt,
            "godkanda": ok_totalt, "totalt": totalt, "kodtips": tips.get("antal", 0),
            "ovningar": {nr: {"godkanda": sum(t["ok"] for t in l), "totalt": len(l)} for nr, l in grupper.items()},
        }, f, ensure_ascii=False, indent=1)


if __name__ == "__main__":
    main()
