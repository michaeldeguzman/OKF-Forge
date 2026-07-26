---
title: Query Result Cache
slug: query-result-cache
type: concept
version: 1.0
created: 2026-07-26
summary: Caches hybrid search results to avoid recomputing retrieval for repeated queries.
tags: [rag, cache, performance]
owner: mdg
status: published
---

Query Result Cache sits downstream of Hybrid Search with RRF in the
retrieval pipeline. Rather than recomputing the vector leg, the BM25 leg,
and the RRF fusion for a query that has already been answered recently,
this component stores the fused result set keyed against the query text and
returns the cached result on a repeat lookup.

This is the terminal node in the RAG pipeline graph — nothing downstream of
this component exists yet. Its only expected inbound edge is from Hybrid
Search with RRF, and it produces no outbound links of its own.