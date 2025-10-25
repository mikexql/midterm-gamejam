[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/N5tu08Ko)
# **Game Title**

## Code of Silence
_(Add some screenshots here showcasing your game)_

## **How to Run**

- System requirements: Windows, Mouse-only
- Unity Version: 6000.2.6f2
- **Executable file** of the game to download _(Ensure it is playable without extra setup.)_

### Gameplay Recording

You should provide a link to your **Full gameplay recording** showing mechanics, UI/UX, and polish. _(A complete playthrough is required.)_ here.

## Chosen Theme

Honor Died on the Beach

## Game Description

_(Briefly describe your game’s premise, setting, and goal. What is the game about? What experience should the player have?)_
“Code of Silence” is a short narrative strategy game inspired by the classic Prisoner’s Dilemma game theory concept — reimagined in a mafia interrogation room.
You and your associate have been captured. The detective offers both of you a deal: snitch, and walk free — or stay silent and risk it all.

Each round, you decide whether to cooperate (stay silent) or betray. The opposing AI makes its own choice based on its trust and heat level, simulating a rational agent.
Your goal is to survive interrogation and walk free — without losing your honor.

The experience explores the tension between loyalty and survival, framed through a cinematic pixel-art aesthetic.

## **Core Mechanic**

_(What is the core mechanic? Use the core mechanics described in class. Then you can describe the main action players perform repeatedly that drives gameplay. Example: "Jumping between platforms to avoid obstacles" or "Controlling a spaceship to dodge and shoot enemies.")_
Strategy

Players repeatedly choose between staying silent or betraying.
Each decision affects three dynamic meters:
Trust – the AI’s belief in your loyalty
Heat – how close the police are to breaking you both
Score – your reputation within the family
Each round forces the player to balance personal gain versus moral integrity, driving both narrative and outcome.

## **Game Procedure & Controls**

_(How does the player navigate the game? What are the controls? Provide a list of inputs and their functions.)_
Left Mouse Click – Advance dialogue, Click on Buttons

## **Core Drive (Tally with Design Principles, Balance, and Intentional Design)**

_(What **motivates** the player to engage with your game? Relate this to the Octalysis Framework or other motivational principles.)_

### **Examples of Core Drives:**

- **Epic Meaning & Purpose**: Does the game make players feel like they are part of something greater?
- **Development & Accomplishment**: Are there clear goals and a sense of progress?
- **Empowerment of Creativity & Feedback**: Can players experiment, make choices, and receive meaningful feedback?
- **Ownership & Possession**: Do players feel invested in their progress, character, or world?
- **Social Influence & Relatedness**: Does the game foster interaction, competition, or cooperation?
- **Scarcity & Impatience**: Does limited access to resources create meaningful decisions?
- **Unpredictability & Curiosity**: Does the game encourage exploration or surprise the player?
- **Loss & Avoidance**: Are players driven by the fear of losing progress or failing?

_(Choose ONE core drive that influenced your design and explain how they shape player engagement.)_

## **Game Balance Efforts (Tally with Rubric)**

_(What balance considerations were made in the game? Did you focus on PvP balance, PvE balance, asymmetry, statistical balance, risk vs. reward, skill balance, or economic balance? Explain how you implemented or adjusted game balance.)_
Betrayal yields higher short-term score but drastically lowers trust and raises heat.
Cooperation maintains trust but slows progress and you risk AI betrayal.
The AI adapts probabilistically based on trust, event context, and previous actions.
In the classic problem, the rational behaviour in the iterated version is often to cooperate.

Example:

- **Risk vs. Reward**: High-risk areas provide more power-ups, rewarding skilled players
- **PvE Balance**: Enemy difficulty scales gradually, ensuring a fair challenge.
- **Asymmetry**: Two players have different abilities but are balanced through unique strengths and weaknesses.

## **Unique Rule (Tally with Rubric)**

_(What unique rule did you introduce? Did you apply an elegant rule, break a conventional rule, or create asymmetry? Describe how this rule impacts gameplay and how it aligns with the rubric.)_

Example:

- **Breaking a Rule**: In a platformer, players cannot jump but must rely on environment-based movement.
- **Asymmetry**: The player controls two characters with different abilities, requiring strategic switching.

## **Game Design Principles Incorporated**

_(Which design principles from class were intentionally applied? List at least one and explain how it enhances the player’s experience.)_
Meaningful Choices: Each round, the player makes a moral decision that alters outcomes and future dialogue.
Feedback Loops: Visual meters (Trust, Heat, Score) provide instant, readable feedback.
Grounding the Player: The pixel-art interrogation room and lighting changes reflect psychological tone shifts.
Player Empathy: Dialogue humanizes both the detective and mafia associate, making betrayal emotionally costly.
Example:

- **Grounding the Player**: The environment provides strong visual cues to help players understand mechanics naturally.
- **Meaningful Choices**: Players must decide between fighting stronger enemies for rewards or avoiding them for safety.
- **Player Empathy**: The story and mechanics are designed to create an emotional connection with the character.

## Code Cleanliness Efforts (Bonus)

_Outline your code architecture here and attempt to organise your code that suits your game. Be concise and clear. Leave this section blank if you are not doing this bonus part._
Modular Architecture:
GameManager: Core round loop, event system, and state machine.
DialogueManager: Handles animated text display and blip feedback.
UIController: Updates sliders, buttons, and feedback colors dynamically.
EventSystem: Supplies fixed narrative events by round.
AIController: Handles trust-based decisions probabilistically.
Data-Driven Design:
Game balance parameters stored in BalanceSO ScriptableObject for easy tuning.
Scalable Dialogue:
Dialogue text and speaker metadata handled by array-based system for modular writing.

## Credits

Please credit any work (art form, ideas, etc) you use in this exam.
AI-generated Sprites: Detective, Main Menu and Game Background, Buttons
Music:
Sprites:
