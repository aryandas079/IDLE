# IDLE Privacy and Safety

## Purpose

IDLE is an experimental accessibility and sensory-comfort project. Privacy and user control are core design requirements.

## Privacy principles

### Local-first processing
Perform analysis locally wherever practical. Do not send content to a cloud service unless a specific feature requires it, the data flow is documented, and the user has appropriate notice and control.

### Data minimization
Collect only what is needed for an enabled feature. Prefer measurements and event metadata over raw screen frames, full webpages, or audio recordings.

### No raw capture retention by default
Do not retain raw screen or audio content by default. If an opt-in debugging or research mode is ever introduced, define its purpose, consent process, retention period, access controls, and deletion path before release.

### Permission transparency
Explain why browser or operating-system permissions are required. Request only the permissions necessary for the enabled feature.

### No unnecessary telemetry
Do not transmit screen content, audio, personal preferences, or inferred health information through analytics, crash reports, or logs without a documented, consent-based design.

### Secrets
Never commit API keys, tokens, credentials, private recordings, or sensitive data. Keep local secrets out of version control.

## Safety principles

- Users remain in control of automatic interventions.
- Provide pause, disable, undo, and recovery controls.
- Unsupported content should remain unchanged.
- A detector must not claim to diagnose or predict a medical episode.
- Do not silently suppress emergency alerts or safety-critical communications.
- Report when a detector or intervention is unavailable.
- Avoid flashing or loud notifications in IDLE's own interface.
- Test interventions for disruption and unintended effects.

## Medical boundaries

IDLE detects defined digital patterns and may adjust supported content. Such detections do not establish that a pattern is medically dangerous to a particular user.

IDLE is not a medical device and does not diagnose, treat, cure, or guarantee prevention of migraine, vertigo, photosensitivity, sensory overload, or any other condition.

Claims about symptom reduction or prevention require separate, appropriate evidence and, where applicable, clinical evaluation.

## Threats to consider

- Excessive browser-extension permissions
- Malicious webpages
- Unintended data transmission
- Exposure of local diagnostic logs
- Compromised dependencies
- Incorrect or disruptive automatic interventions
- Failed restoration of settings
- Insecure handling of optional cloud services

## Review checklist for new features

Before merging a feature, ask:
1. What input does it access?
2. Why is that input necessary?
3. Does any data leave the device?
4. Is raw content retained?
5. Can users disable the feature?
6. Can the action be reversed?
7. What happens if the feature fails?
8. Could it interfere with essential content or alerts?
9. What claims can the test results actually support?
