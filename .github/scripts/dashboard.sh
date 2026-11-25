#!/usr/bin/env bash
# Bygger elevens dashboard och pushar den till grenen 'dashboard' i elevens fork.
# Grenen innehåller bara dashboarden, så den krockar aldrig med elevens kod.
set -euo pipefail

historik=/tmp/historik.csv
ut=/tmp/dashboard
rm -rf "$ut"
: > "$historik"

# Hämta tidigare historik om dashboarden redan finns.
if git fetch -q origin dashboard 2>/dev/null; then
  git show FETCH_HEAD:historik.csv > "$historik" 2>/dev/null || : > "$historik"
fi

python3 .github/scripts/dashboard.py testresultat kodtips.json "$historik" "$ut"

cd "$ut"
git init -q -b dashboard
git add -A
git -c user.name="github-actions[bot]" \
    -c user.email="41898282+github-actions[bot]@users.noreply.github.com" \
    commit -q -m "Uppdatera dashboard"
git push -q -f "https://x-access-token:${GH_TOKEN}@github.com/${GITHUB_REPOSITORY}.git" dashboard

{
  echo "## 📊 Din dashboard"
  echo ""
  echo "Se din progression här: ${GITHUB_SERVER_URL}/${GITHUB_REPOSITORY}/tree/dashboard"
} >> "$GITHUB_STEP_SUMMARY"
