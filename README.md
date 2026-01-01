# WavyDavy
dynamic enemy wave system for a Unity 3D WebGL game

# Project WavyDavy - Development Summary

## Overview
This document outlines the development progress, technical implementation, and asset attribution for the "WavyDavy" Unity project. The game is a 3D survival shooter featuring infinite enemy waves, implemented with performance-optimized systems suitable for WebGL.

## Implemented Features

### 1. Core Systems
*   **Game Manager**: Centralized control of game state and flow.
*   **Wave System (`WaveManager`)**:
    *   Dynamic difficulty progression.
    *   Waves scale in size (starting at 30, 50, 70, then +10 per wave).
    *   Infinite cycling with delay between waves.
*   **Object Pooling (`ObjectPool`)**:
    *   optimized memory management for enemies to prevent garbage collection spikes.
    *   Reuses enemy instances for high performance on WebGL.

### 2. Enemy AI
*   **State Machine Architecture**:
    *   Enemies utilize a flexible state machine (`EnemyStates.cs`) handling `Wander`, `Chase`, and `Attack` behaviors.
*   **Behaviors**:
    *   **Wander**: Random movement when no player is detected.
    *   **Chase**: pathfinding towards the player when in range.
    *   **Attack**: Dealing damage upon contact.

### 3. User Interface (UI)
*   **HUD**: Displays Wave Number, Enemy Count, and dynamic FPS counter.
*   **Controls**: Buttons for managing waves (Stop/Resume, Next Wave, Kill Wave).

## Asset Attribution & Credits

### 3D Models
*   **Enemies**: Custom-created 3D models designed and modeled in **Blender**.
*   **Obstacles/Environment**: High-quality obstacle assets were sourced and downloaded from **Sketchfab**.

## Tech Stack
*   **Engine**: Unity 2022+ (URP)
*   **Language**: C#
*   **Platform Target**: WebGL

