---
title: Image Semantic Search
slug: image-semantic-search
type: concept
version: 1.0
created: 2026-07-26
summary: Multimodal embedding search using Cohere Embed v4, sharing architectural patterns with the text chunking stack.
tags: [rag, multimodal, image, embeddings]
owner: mdg
status: published
related: [chunking-library]
---

Image Semantic Search is a separate branch of the pipeline handling
multimodal content rather than text documents. The stack follows the same
three-tier pattern as the rest of the pipeline: MultimodalEmbeddingLibrary
(C# External Logic) handles the HTTP calls to Cohere's Embed v4 API and
returns raw vectors, Image Vector Library (ODC Library) wraps that and
manages storage, and Image Semantic Search (ODC App) exposes ingestion and
query actions.

Cohere Embed v4 was chosen specifically because it produces a shared vector
space for both text and image embeddings, with Matryoshka representation
support for future coarse-to-fine retrieval. Images are stored as native ODC
Binary Data rather than external blob storage, an explicit external-dependency
tradeoff documented at build time.

This component does not depend on Chunking Library directly, since images
aren't chunked, but it follows the same layered architecture pattern
(stateless External Logic for computation, ODC Library for orchestration,
ODC App for the consumer-facing surface) established by the text chunking
components.