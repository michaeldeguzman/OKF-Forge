---
title: RAG Pipeline Overview
slug: rag-pipeline-overview
type: index
version: 1.0
created: 2026-07-26
summary: Hub node linking every native RAG component built inside ODC, from chunking through hybrid retrieval.
tags: [rag, overview, index]
owner: mdg
status: published
---

This is the entry point into the native RAG pipeline built inside OutSystems
Developer Cloud. Every component here is a real, published Forge contribution,
not a test fixture. The pipeline follows one architectural principle
throughout: native-first, minimizing external dependencies, with honest
documentation of where native ODC is the right call and where to hand off to
external tooling.

The pipeline has two retrieval halves that eventually converge.

On the chunking side, [Chunking Library](chunking-library) provides the
foundational split strategies. [Semantic Chunking](semantic-chunking) builds
on it with embedding-aware split points. [Agentic Chunking](agentic-chunking)
goes further still, using an LLM to extract and group propositions rather
than splitting on structural or semantic boundaries alone.

Separately, [Image Semantic Search](image-semantic-search) extends the same
architectural pattern into multimodal content.

On the lexical side, [BM25 Lexical Search](bm25-lexical-search) provides
scoring independent of any embedding model.

Both halves converge in [Hybrid Search with RRF](hybrid-search-rrf), which
fuses vector and BM25 rankings into one result set. Downstream of that,
[Query Result Cache](query-result-cache) avoids recomputing retrieval for
repeated queries.
