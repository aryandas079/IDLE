# Security Policy

## Scope

IDLE is an early-stage open-source project. Its security practices and supported release process may evolve as implementation progresses.

## Reporting a vulnerability

Do not publish credentials, private recordings, personal information, or sensitive exploit details in a public Issue.

Until a private security-reporting channel is configured, contact the repository maintainer through an appropriate private GitHub contact method. Include:
- A concise description
- Affected component and version or commit
- Steps to reproduce
- Potential impact
- A suggested mitigation, if known

Do not include real secrets or sensitive user data in the report.

## Security design priorities

- Minimize browser permissions.
- Keep sensitive processing local wherever practical.
- Avoid retaining raw screen or audio content by default.
- Never commit secrets.
- Validate inputs and handle failures safely.
- Document external services and data flows.
- Keep dependencies updated and review their licences and advisories.
- Make automatic interventions reversible.

## Current limitations

IDLE is an experimental project and must not be assumed to provide a security guarantee or medical protection. Do not use it as the sole protection mechanism for safety-critical systems.
