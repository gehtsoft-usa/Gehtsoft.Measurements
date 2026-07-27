# Installing the skill

The skill is a folder of Markdown. Installing it means putting
`skills/gehtsoft-measurements/` where your Claude client looks for skills.

## Claude Code

Skills are picked up from two places. Use whichever fits.

**For one project** — the skill applies only when working inside that repository:

```bash
mkdir -p <your-project>/.claude/skills
cp -r skills/gehtsoft-measurements <your-project>/.claude/skills/
```

**For every project on the machine**:

```bash
mkdir -p ~/.claude/skills
cp -r skills/gehtsoft-measurements ~/.claude/skills/
```

On Windows the personal folder is `%USERPROFILE%\.claude\skills`.

Start a new Claude Code session afterwards; the skill list is read at startup. Confirm it is
loaded by asking something the skill covers, for example *"convert 300 yards to metres with
Gehtsoft.Measurements"*, or by typing `/gehtsoft-measurements` to invoke it directly.

## Claude.ai and the desktop app

Upload the skill through **Settings → Capabilities → Skills**. Either point the uploader at the
`skills/gehtsoft-measurements` folder, or zip that folder and upload the archive. Upload the
skill folder itself, not the whole `SKILL/` repository folder.

## Keeping it up to date

The skill describes a specific version of the library. It was written against **1.1.18**, which
is the first version with the span parse API, `UnitEnumValidator`, and the compact JSON
converter. Against 1.1.17 or earlier, the parsing, formatting and validation sections describe
API that does not exist yet.

When the library changes, edit `skills/gehtsoft-measurements/SKILL.md` and copy it over the
installed one again.

## Developing the skill

The skill-creator workflow writes its test runs to a workspace folder next to the skill:

```
skills/gehtsoft-measurements-workspace/
```

**That folder and the eval definitions are deliberately kept out of version control and out of
SonarQube analysis.** They hold generated output, transcripts and throwaway scratch code, none
of which belongs in the repository or in a code-quality report.

- Git: `.gitignore` in the repository root excludes `SKILL/skills/*-workspace/` and
  `SKILL/evals/`.
- SonarQube: the scanner invocation in `sonar.bat` excludes `SKILL/**`. `sonar.bat` is itself
  untracked, so if you recreate it, keep the exclusion:

  ```
  /d:sonar.exclusions="SKILL/**"
  ```

  Nothing under `SKILL/` belongs to an MSBuild project, so the scanner would normally skip it
  anyway. The exclusion is there so that a stray script or a generated `.cs` file in a test
  workspace can never turn up as a finding against the library.
