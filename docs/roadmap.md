# IDLE Development Roadmap

This roadmap describes planned work for IDLE: Intelligent Detection Layer for Experience. Dates are intentionally omitted until effort and availability are known.

## Phase 1 — Research and specification

### Tasks
- Review existing accessibility settings, standards, and related research.
- Define the minimum viable product (MVP).
- Select the initial supported browser and operating system.
- Document input permissions, data flow, and failure behaviour.
- Create synthetic test cases and evaluation criteria.
- Record third-party dependency and dataset licences.

### Deliverables
- Product requirements
- Architecture document
- Privacy and safety document
- Evaluation plan
- Prioritized GitHub Issues

### Exit criteria
The team can state precisely what the first version observes, changes, cannot observe, and how it will be evaluated.

## Phase 2 — Visual detection baseline

### Tasks
- Implement luminance measurements.
- Detect predefined temporal brightness changes.
- Evaluate frame-difference or motion features.
- Handle missing, invalid, and low-quality inputs.
- Create deterministic tests and synthetic fixtures.

### Deliverables
- Testable visual detectors
- Synthetic fixture set
- Baseline precision, recall, false-positive, and latency measurements

### Exit criteria
Results are reproducible on the documented test set and include known failures.

## Phase 3 — Supported protective interventions

### Tasks
- Build a browser integration for the supported environment.
- Implement reduced-motion or simplified-view controls.
- Add user-visible status, pause, undo, and exclusions.
- Handle navigation, unsupported content, and intervention failures.

### Deliverables
- End-to-end prototype
- Integration tests
- Compatibility notes

### Exit criteria
A predefined test event can trigger a real, observable, reversible action on supported content.

## Phase 4 — Preferences and privacy

### Tasks
- Add configurable thresholds and detector toggles.
- Store preferences locally.
- Minimize operational logs.
- Review extension permissions and data flows.
- Add reset and deletion controls.

### Exit criteria
Users can inspect and change settings, disable protection, and understand what data is processed.

## Phase 5 — Evaluation and optimization

### Tasks
- Measure precision, recall, and false alerts per hour.
- Measure end-to-end intervention latency.
- Measure CPU and memory usage.
- Test normal content as well as challenging synthetic patterns.
- Review compatibility and usability.

### Exit criteria
A reproducible report describes results, environment, limitations, and failure cases.

## Phase 6 — Demonstration and release preparation

### Tasks
- Improve installation and setup documentation.
- Prepare a repeatable demonstration.
- Document current features versus planned features.
- Review licence, security, and contribution files.
- Prepare a technical presentation based on measured results.

### Exit criteria
A new contributor can follow the documented setup, run available tests, and understand the prototype's limitations.

## Future research — Not part of the initial MVP

- Audio stimulus analysis and supported attenuation.
- Broader Windows integration.
- Additional browser support.
- Optional wearable signals.
- Personalization research.
- Machine-learning models if they outperform simpler baselines.
- Research partnerships for usability or clinical questions.

## Prioritization rule

Build one complete, testable vertical slice before adding more modules. Do not describe a feature as implemented until it exists in code; do not describe it as validated without appropriate evidence.
