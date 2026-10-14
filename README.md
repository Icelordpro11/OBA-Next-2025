<div align="center">

<img src="Docs/Nexi%20Banner.png" alt="OBA Next MR — Nexi AI assistant" width="700">

# OBA Next MR — Nexi AI Assistant

**A mixed-reality library experience with an AI-powered virtual assistant for children.**

Built for the Amsterdam Public Library, Nexi helps children explore the library, ask questions, and find relevant books using voice interaction, OpenAI-powered agents, and spatial mapping.

</div>

## Overview

Nexi is an AI conversational assistant integrated into a Unity mixed-reality experience for Meta Quest 3. It combines speech input and output with specialized AI agents to support natural conversations and book discovery.

- **Mixed Reality:** Unity and Meta Quest 3.
- **Voice interaction:** Speech-to-text and text-to-speech.
- **AI conversations:** OpenAI Assistant integration.
- **Specialized agents:** Search, comparison, and book-related queries.
- **Spatial mapping:** Polycam scans and spatial anchors to place virtual content in the real library.

## AI System Flowchart

The diagram below shows how the specialized agents work together to process a spoken request and return a response.

<p align="center">
  <img src="Docs/nexi-agents-flowchart.png" alt="Nexi AI agents flowchart" width="650">
</p>

## AI System Demo

A demonstration of Nexi interacting with users through natural speech, processing queries, and providing relevant book information.

<p align="center">
  <video src="Docs/nexi-ai-system-demo.mp4" controls width="650"></video>
</p>

> If the video does not play in the GitHub README, use the video file link instead: [Nexi AI System Demo](Docs/nexi-ai-system-demo.mp4).

## Main Components

### Assistant Manager

The `Assistants_Manager` Unity singleton coordinates the multi-agent conversation. It sends user input to the primary agent, manages the conversation thread, and receives responses.

<p align="center">
  <img src="Docs/assistant-manager.png" alt="Assistant Manager Unity Inspector setup" width="500">
</p>

### Speech-to-Text System

The speech-to-text component records microphone input, saves the audio as a WAV file, and sends it to OpenAI's transcription service. The returned text is then used to start or continue the conversation.

<p align="center">
  <img src="Docs/speech-to-text.png" alt="Speech-to-text implementation" width="500">
</p>

### Text-to-Speech System

The text-to-speech component sends generated text to OpenAI's audio API, receives MP3 audio, and plays it in Unity. It also supports UI feedback and facial animation triggers during playback.

<p align="center">
  <img src="Docs/text-to-speech.png" alt="Text-to-speech implementation" width="500">
</p>

### OpenAI Agent

The `so_ChatGPTAgent` ScriptableObject handles communication with the OpenAI API. It creates or reuses conversation threads, sends prompts, checks response status, and returns the assistant's reply.

<p align="center">
  <img src="Docs/open-ai-agent.png" alt="OpenAI agent implementation" width="500">
</p>

### Pathfinding and Spatial Mapping

Polycam was used to scan the library floor and create a 3D representation of the environment. The scan was imported into Unity and used with spatial anchors to position virtual objects in the physical space. Passthrough helps blend virtual elements with the real environment.

<p align="center">
  <img src="Docs/pathfinding-spatial-mapping.png" alt="3D spatial scan of the library" width="600">
</p>

## Other Project Media

Additional project media is available in the `Docs` folder:

- [Nexi media](Docs/Nexi-Media.mp4)
- [Nexi popup demonstration](Docs/Nexi-Popup.mp4)
- [Showcase AI](Docs/Showcase-AI.mp4)
- [Showcase testing](Docs/ShowcaseTesting.png)
- [Hand interaction](Docs/hand-interaction.mp4)
- [Meta XR SDK image](Docs/meta-xr-sdk.png)
- [OBA Next MR image](Docs/obanext.png)
- [OBA Next MR video](Docs/obanext.mp4)

## Technologies

- Unity
- C#
- Meta Quest 3
- Mixed Reality and passthrough
- OpenAI APIs
- Speech-to-text and text-to-speech
- Polycam spatial scanning
- Spatial anchors

## My Contribution

I worked on the AI interaction system and its Unity integration, including the multi-agent conversation flow, speech input and output, OpenAI communication, and mixed-reality spatial features.

## Notes

This repository documents selected parts of the project and its architecture. API access may require valid credentials and service availability. **Never commit API keys or other secrets to a public repository.**

---

<div align="center">
  <sub>OBA Next MR · Unity · Mixed Reality · AI</sub>
</div>
