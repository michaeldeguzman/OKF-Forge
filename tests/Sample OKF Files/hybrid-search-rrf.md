---
title: Hybrid Search with RRF
slug: hybrid-search-rrf
type: concept
version: 1.0
created: 2026-07-26
summary: Fuses vector search and BM25 rankings into one result set using Reciprocal Rank Fusion.
tags: [rag, hybrid, rrf, retrieval]
owner: mdg
status: published
---

Hybrid Search with RRF is the convergence point of the two retrieval halves
built earlier in the pipeline. It runs a query against [BM25 Lexical
Search](bm25-lexical-search) and against the existing vector similarity
search independently, then fuses the two ranked result lists using
Reciprocal Rank Fusion rather than trying to combine raw scores from two
incompatible scoring systems.

The implementation is split into three private phase actions: RunVectorLeg,
RunBM25Leg, and RunRRFFusion, called by one orchestrating action. Two bugs
were found and fixed during testing: a SQL IN clause construction error in
the BM25 leg where the query terms string wasn't building correctly, and a
rank-order bug in the vector leg where the loop was iterating results in
database Id order rather than true similarity rank order, which was
silently producing incorrect rank contributions in the fusion step. After
both fixes, RRF scores were verified mathematically correct to eight decimal
places across multiple test queries.

This is the retrieval layer an agent or end user actually queries. Its
output feeds into [Query Result Cache](query-result-cache) to avoid
recomputing the same fusion for repeated queries.
