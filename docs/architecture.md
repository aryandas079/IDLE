# IDLE System Architecture

## Purpose

This document describes the proposed architecture for IDLE: Intelligent Detection Layer for Experience. It is a target design, not a claim that every module already exists.

## Design goals

- Separate detection from intervention decisions.
- Prefer local processing and minimal data retention.
- Keep every intervention explainable and reversible.
- Allow detectors to be tested independently.
- Handle unavailable inputs and failed actions safely.
- Make platform limitations explicit.

## High-level data flow

```text
Supported input
    |
    v
Input adapter and validation
    |
    v
Feature extraction
    |
    v
Visual / optional audio detectors
    |
    v
Policy engine <--- user preferences and exclusions
    |
    v
Decision: observe / inform / offer / adapt
    |
    v
Supported intervention adapter
    |
    v
Result, minimal local event metadata, optional feedback
```

## Components

### 1. Input adapters

Obtain only the signals needed by enabled features. Prefer webpage metadata and browser APIs over full-screen capture when these provide sufficient information. Handle missing permissions, invalid inputs, and unsupported content explicitly.

### 2. Feature extraction

Convert inputs into measurements such as luminance statistics, temporal differences, motion features, or audio amplitude. Validate measurement quality and minimize the lifetime of sensitive raw input.

### 3. Detection modules

Each detector should have a documented input, output schema, limitations, and independent tests. Detector outputs describe observed patterns; they must not claim to diagnose a condition or predict an episode.

Suggested result fields:
- `detector_id`
- `timestamp`
- `pattern`
- `measurements`
- `rule_match` or appropriately defined confidence
- `input_quality`
- `limitations`

### 4. Policy engine

The policy engine evaluates detector results against user preferences, configured thresholds, exclusions, and safety rules. It selects the least disruptive supported action.

Possible decisions:
- `observe`
- `notify`
- `offer_intervention`
- `apply_intervention`
- `no_action`
- `input_unavailable`

Do not assign arbitrary probabilities to rule-based results.

### 5. Intervention adapters

Apply changes through documented interfaces. Each adapter should state which platforms and content types it supports, verify success when possible, and provide undo or recovery behaviour.

### 6. Preferences and personalization

Store explicit user settings locally by default. Initial personalization should be transparent and rule-based. Do not infer diagnoses from user behaviour.

### 7. Interface

Expose protection status, active detectors, user preferences, intervention explanations, pause/disable controls, and privacy options. The interface itself should avoid unnecessary animation and sound.

### 8. Evaluation

Measure detection quality, false alarms, missed cases, latency, intervention success, resource usage, and compatibility.

## Initial scope

The first implementation should target a narrow set of visual patterns in supported Chromium-based webpages on Windows. Browser-level access does not equal universal access to all rendered pixels, protected media, browser chrome, or other applications.

Audio processing, a native desktop application, and operating-system-wide control should be separate milestones.

## Failure handling

- Missing input: report that the detector is unavailable; do not invent a result.
- Detector error: log minimal diagnostic metadata locally if enabled and disable or restart the affected detector safely.
- Intervention failure: report failure and offer manual controls.
- Unsupported page: leave content unchanged.
- User disables protection: stop the relevant processing and interventions.
- Application exit: do not leave persistent settings in an unknown or altered state.

## Architectural rules

1. No detector directly changes content; it returns a result to the policy engine.
2. No intervention should be irreversible by default.
3. No raw screen or audio data should be retained by default.
4. Every platform-specific capability must be documented.
5. Every claimed capability must have tests or an explicit limitation.
