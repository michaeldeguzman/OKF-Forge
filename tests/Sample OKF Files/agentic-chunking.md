---
title: Agentic Chunking
slug: agentic-chunking
type: concept
version: 1.0
created: 2026-07-26
summary: LLM-based proposition extraction and thematic grouping, replacing similarity thresholds with model judgment.
tags: [rag, chunking, agentic, llm]
owner: mdg
status: published
related: [semantic-chunking]
---

Agentic Chunking is Level 5, the final step in the chunking progression
started by [Chunking Library](chunking-library) and extended by
[Semantic Chunking](semantic-chunking). Rather than measuring similarity
between adjacent passages, this approach uses an LLM to extract individual
propositions from a document and then group them thematically.

Two Forge components implement this: AgenticChunkingHelpers, a C# External
Logic library handling supporting operations, and AgenticChunking, the ODC
App orchestrating the LLM calls and grouping logic.

Testing across real documents confirmed that substance rules — no invented
content, correct pronoun resolution, no duplication, self-contained
propositions — held without exception. Granularity consistently ran finer
than initially predicted: the model tends to split into more propositions
than a human would expect, which was confirmed to be a property of model
judgment rather than a defect in the extraction logic. Infinitive and
purpose phrases are treated as separable facts by the model, which explains
much of this finer granularity.

This is the most computationally expensive chunking strategy in the
pipeline and is best reserved for content where structural or semantic
splitting alone produces incoherent chunks.
