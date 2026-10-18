# IDLE Testing and Evaluation Plan

## Purpose

This document defines how IDLE should be evaluated. The first evaluation concerns engineering behaviour on controlled test cases, not clinical effectiveness.

## Evaluation principles

- Define test cases and labels before measuring results.
- Test ordinary content as well as challenging synthetic patterns.
- Record the software version and test environment.
- Report false positives, missed detections, and failure cases.
- Do not equate synthetic detection accuracy with symptom prevention.
- Do not expose participants to potentially harmful stimuli for testing.

## Test categories

### Visual fixtures
Include:
- Stable scenes
- Gradual brightness changes
- Abrupt brightness changes
- Repeated temporal changes
- Static high-contrast content
- Slow motion
- Rapid motion
- Short transitions
- Missing, corrupted, or invalid frames

Labels should describe measurable stimulus properties, not predict individual medical responses.

### Audio fixtures (when implemented)
Include:
- Silence
- Stable amplitude
- Gradual level increases
- Sudden peaks
- Repeated peaks
- Different sample rates
- Interrupted or invalid input

Use controlled files and safe test levels.

## Metrics

### Precision
`TP / (TP + FP)`

The proportion of positive detections that match the test labels.

### Recall
`TP / (TP + FN)`

The proportion of labelled positive cases detected.

### False alerts per hour
Count false triggers during a defined period of ordinary test content.

### Intervention latency
Measure time from detection to successful intervention. Report median and 95th percentile, plus the measurement method.

### Intervention success rate
Measure whether the requested modification actually occurred on supported content.

### Resource usage
Measure CPU and memory under representative workloads. Record hardware, operating system, browser, and test duration.

### Compatibility
Record supported and unsupported pages, browser versions, permissions, and known failure modes.

## Test layers

1. **Unit tests:** Mathematical features, thresholds, policies, settings, and recovery logic.
2. **Integration tests:** Input-to-detection-to-policy-to-intervention flow.
3. **Performance tests:** Latency, CPU, memory, and sustained operation.
4. **Usability tests:** Whether users understand alerts and can undo changes.
5. **Privacy tests:** Confirm that data flows and storage match documentation.

## Acceptance criteria

Set numerical thresholds before evaluating a release. The thresholds should be appropriate to the defined task and test data; do not choose them after seeing results merely to make the prototype pass.

Every report should include:
- Test dataset or fixture description
- Labels and test methodology
- Software and environment versions
- Metrics and uncertainty where appropriate
- Failure cases
- Known limitations

## Clinical claims

Passing software tests does not establish that IDLE reduces symptoms or prevents medical episodes. Any clinical claim requires a separate study design and evidence appropriate to that claim.
