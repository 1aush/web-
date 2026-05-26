---
name: auto-push-github
description: Use when the user wants to update the project, commit changes, and push to GitHub automatically. Also updates README.md with the latest update time.
---

# Auto Push GitHub Skill

This skill automates the process of committing changes and pushing to GitHub while keeping README.md updated with the latest modification time.

## Trigger Phrases

Use this skill when the user says:
- "更新项目" (update project)
- "提交更改" (commit changes)
- "推送到GitHub" (push to GitHub)
- "同步到GitHub" (sync to GitHub)
- "发布更新" (publish update)
- "自动提交" (auto commit)

## Workflow

When triggered, follow these steps in order:

### 1. Check Git Status

```bash
git status
```

If there are no changes to commit, inform the user and stop.

### 2. Update README.md

Update the "最后更新" (last updated) timestamp in README.md to the current date.

Find and replace the line matching `**最后更新**: YYYY-MM-DD` with the current date in format `YYYY-MM-DD`.

Example: `**最后更新**: 2026-05-26`

### 3. Stage All Changes

```bash
git add -A
```

### 4. Generate Commit Message

Analyze the staged changes to generate a meaningful commit message:
- Use `git diff --cached --stat` to see what files changed
- Use `git diff --cached` to see the actual changes
- Create a concise commit message in Chinese or English that describes the main changes

Format: `[类型] 简短描述`

Types:
- `[功能]` - New features
- `[修复]` - Bug fixes
- `[更新]` - Updates to existing features
- `[文档]` - Documentation changes
- `[重构]` - Code refactoring

### 5. Commit Changes

```bash
git commit -m "生成的提交信息"
```

### 6. Push to GitHub

```bash
git push origin master
```

If the current branch is not `master`, use the current branch name instead.

Get current branch with: `git branch --show-current`

## Error Handling

- If `git push` fails due to remote changes, run `git pull --rebase origin master` first, then retry push
- If merge conflicts occur during pull, inform the user and stop for manual resolution
- If README.md doesn't contain the "最后更新" pattern, add it at the end of the file before the final `---`

## Example Output

After successful execution, report to the user:

```
✅ 项目更新完成！
- 提交信息: [生成的提交信息]
- 已推送到 GitHub
- README.md 已更新: 最后更新 2026-05-26
```