---
title: Chunking Library
slug: chunking-library
type: concept
version: 1.0
created: 2026-07-26
summary: C# External Logic library exposing four fixed chunking strategies for ODC RAG pipelines.
tags: [rag, chunking, external-logic]
owner: mdg
status: published
---

The Chunking Library is a stateless C# External Logic component that sits
between a document extraction layer and a vector store inside ODC. It exposes
four Server Actions, each implementing a distinct splitting strategy:
ChunkByCharacter for fixed-size splits, ChunkRecursively for a recursive
character-based approach, ChunkBySentence for sentence-boundary splitting,
and ChunkMarkdown for heading-aware splitting that protects Markdown
structural elements from being broken mid-section.

ChunkBySentence has one known quirk worth noting: it bundles Markdown
headings together with the following sentence, since headings lack terminal
punctuation. This does not affect ChunkMarkdown, which is heading-aware by
design.

This library is the foundation the rest of the chunking work builds on.
[Semantic Chunking](semantic-chunking) extends these fixed strategies with
embedding-based split points, and [Agentic Chunking](agentic-chunking) goes
further still with LLM-based proposition extraction.
