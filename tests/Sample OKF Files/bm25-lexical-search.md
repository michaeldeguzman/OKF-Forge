---
title: BM25 Lexical Search
slug: bm25-lexical-search
type: concept
version: 1.0
created: 2026-07-26
summary: Native BM25 scoring engine with a self-written Porter stemmer, independent of any embedding model.
tags: [rag, bm25, lexical, external-logic]
owner: mdg
status: published
---

BM25 Lexical Search provides keyword-based scoring as an alternative and
complement to vector similarity search. The stack is BM25Engine (C# External
Logic) wrapping a self-written Porter stemmer, verified against the original
1980 Porter test vocabulary rather than pulling in a NuGet dependency, feeding
into BM25 Library, which hooks directly into the existing RAG Knowledge Base
app rather than requiring a new one.

A tokenizer bug was found and fixed during development: version 1 omitted
non-alphanumeric characters entirely, which caused token fusion — for
example "ERR-BM25-003" collapsed into "errbm25003". Version 2 replaces
punctuation with spaces as word boundaries instead of dropping it. A full
corpus reindex was required after the fix, and a TokenizerVersion site
property now flags any future mismatch between ingestion-time and
query-time tokenization.

Document frequency and corpus-wide statistics are recomputed via a
timer-driven batch job rather than maintained incrementally, which
eliminates concurrency races and retry double-counting at the cost of a
bounded staleness window — an explicit and accepted tradeoff.

BM25 scoring on its own answers a different question than vector search:
exact term matching rather than semantic similarity. The two are combined in
[Hybrid Search with RRF](hybrid-search-rrf).
