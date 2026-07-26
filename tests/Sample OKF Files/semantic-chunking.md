---
title: Semantic Chunking
slug: semantic-chunking
type: concept
version: 1.0
created: 2026-07-26
summary: Embedding-aware chunking using Consecutive, Cumulative, and Statistical split patterns.
tags: [rag, chunking, semantic, embeddings]
owner: mdg
status: published
related: [chunking-library]
---

Semantic Chunking is Level 4 in the chunking progression, built on top of
[Chunking Library](chunking-library)'s fixed-size and structural splitting.
Rather than splitting on character count or Markdown headings alone, this
component finds split points based on embedding similarity between adjacent
sentences or passages.

Three patterns are implemented as C# External Logic actions: Consecutive,
which compares each sentence to the one immediately after it; Cumulative,
which compares each sentence to a running average of the current chunk; and
Statistical, which uses a distribution-based threshold to decide where
semantic drift is large enough to warrant a split.

This component does not replace [Chunking Library](chunking-library) — it
sits alongside it as a more computationally expensive alternative for
content where structural boundaries alone don't produce coherent chunks. The
next step in this progression is [Agentic Chunking](agentic-chunking), which
replaces similarity thresholds with LLM judgment entirely.

Chunks produced here are what get embedded and made searchable via vector
similarity, which is the other half of what [Hybrid Search with
RRF](hybrid-search-rrf) fuses together alongside BM25 scoring.
