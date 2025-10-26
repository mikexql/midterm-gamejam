[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/N5tu08Ko)
# **Game Title**

## **Code of Silence**
![Dialogue](Pictures/dialogue.png)
![Choice](Pictures/choice.png)
![Round Outcome](Pictures/roundOutcome.png)

## **How to Run**

- System requirements: Windows 64-bit, Mouse and Keyboard Controls
- Unity Version: 6000.2.6f2
- [**Executable file** of the game to download](https://github.com/50033-game-design-and-development/midterm-game-jam-2025-fall-mikexql/releases/tag/v1.0.1)

### Gameplay Recording

https://youtu.be/sYayIp7s4qs

Blinking and Talking (Mafia only) Portraits

## Chosen Theme

### "**Honor Died on the Beach**"

## Game Description

**“Code of Silence”** is a short narrative strategy game inspired by the classic *Prisoner’s Dilemma* game theory concept — **reimagined** in a mafia interrogation room.

You and your associate have been captured by the po-po, and he offers to reduce your sentence if you rat on your buddy. If you rat enough, you can even get your sentence reduced completely.

Each round, you decide whether to cooperate (stay silent) or betray. In the actual game theory scenario, the ideal strategy for both would be to cooperate over multiple rounds. However, in this game, cooperation only will not give the you enough `Score` to win, forcing you to betray, otherwise the game ends and you are still imprisoned. The opposing AI makes its own choice based on its `Trust` level.

There are three endings:
- Witness Deal: You gave enough information about your buddy and your sentence is lifted fully at the cost of betrayal. Occurs when `Score` is maxed out at the end of the game.
- Still Imprisoned: You did not betray your buddy enough to get your sentence fully reduced. Occurs when Score is not maxed out at the end of the game.
- **Secret True Narrative Ending**, Take the Fall: You did not give any information at all about your partner and even volunteer to take the fall for him like a **true stand-up guy**. Occurs if the player never chose `Betrayal`

The experience explores the tension between loyalty and survival, framed through a cinematic pixel-art aesthetic.

## **Core Mechanic**

_(What is the core mechanic? Use the core mechanics described in class. Then you can describe the main action players perform repeatedly that drives gameplay. Example: "Jumping between platforms to avoid obstacles" or "Controlling a spaceship to dodge and shoot enemies.")_

**Strategy**

Players repeatedly choose between staying silent or betraying.
Each decision affects two meters:

`Trust` – the AI’s belief in your loyalty, higher trust means the AI is more likely to choose cooperate and lower trust means he is more likely to betray you.

`Score` – how close you are to fully reducing your sentence. The goal is to max it out so that you can walk.

Each round forces the player to balance personal gain versus moral integrity, driving both narrative and outcome. They also have to consider the meters when making their choice to predict the AI's moves.

## **Game Procedure & Controls**

**Left Mouse Click/Spacebar/Enter** – Skip/Advance dialogue

**Left Mouse Click** - Select Choice

## **Core Drive (Tally with Design Principles, Balance, and Intentional Design)**
_(Choose ONE core drive that influenced your design and explain how they shape player engagement.)_
### **Core Drive: Development & Accomplishment**

There are clear goals in the form of meters on the screen informing the player of their progress. They have to max out the `Score` meter while keeping an eye on the `Trust` meter to predict the AI's move and plan ahead. There are also 3 endings to the game (good and bad) that give player's a sense of accomplishment when they first see each of them.

## **Game Balance Efforts (Tally with Rubric)**

_(What balance considerations were made in the game? Did you focus on PvP balance, PvE balance, asymmetry, statistical balance, risk vs. reward, skill balance, or economic balance? Explain how you implemented or adjusted game balance.)_

I tweaked the outcome tables of the classic Prisoner's Dilemma concept to drive players towards a mixed strategy that necessitates betrayal along with cooperation.

Here are the possible outcomes of each round (`Player Choice/AI Choice`):

`Betrayal/Coop, Coop/Betrayal, Betrayal/Betrayal, Coop/Coop`

`Betrayal/Coop` yields the highest score but drastically lowers trust, but the player needs to be careful not to spam this since `Betrayal/Betrayal` gives a low score and also reduces trust further, and lower trust also means AI `Betrayal` is more likely

`Coop/Coop` improves trust but only gives a lower score and you risk AI betrayal. It is not possible to win just by spamming `Coop/Coop` as it does not give you enough score.

Therefore, the player needs to adopt a mixed strategy by playing both `Betrayal` and `Coop` to keep the AI trust high when betraying to maximise their score to escape.
## **Unique Rule (Tally with Rubric)**

_(What unique rule did you introduce? Did you apply an elegant rule, break a conventional rule, or create asymmetry? Describe how this rule impacts gameplay and how it aligns with the rubric.)_

Example:

- **Breaking a Rule**: In a platformer, players cannot jump but must rely on environment-based movement.
- **Asymmetry**: The player controls two characters with different abilities, requiring strategic switching.

## **Game Design Principles Incorporated**

_(Which design principles from class were intentionally applied? List at least one and explain how it enhances the player’s experience.)_

**Meaningful Choices**: Each round, the player makes a moral decision that builds up towards which ending they get.

**Feedback Loops**: Visual meters (Trust, Score) provide instant, readable feedback after each round.

**Player Empathy**: Dialogue humanizes both the detective and mafia associate, making betrayal emotionally costly.

## Code Cleanliness Efforts (Bonus)

_Outline your code architecture here and attempt to organise your code that suits your game. Be concise and clear. Leave this section blank if you are not doing this bonus part._

### Modular Architecture:

- **GameManager**: Core round loop, event system, and state machine.
- **DialogueManager**: Handles animated text display.
- **UIController**: Updates sliders, buttons, and feedback colors dynamically.
- **EventSystem**: Supplies fixed narrative events by round. (Unused because no time to come up with good variety of balanced events)
- **AIController**: Handles trust-based decisions probabilistically.

### Data-Driven Design:
- Game balance parameters stored in BalanceSO ScriptableObject for easy tuning.

### Scalable Dialogue:
- Dialogue text and speaker metadata handled by array-based system for modular writing.

## Credits

Please credit any work (art form, ideas, etc) you use in this exam.

- AI-generated Sprites: Detective, Main Menu and Game Background, Buttons
- Music:
    - Game Music: https://www.youtube.com/watch?v=cQcq3Gzj7Kw
    - Main Menu Music: https://www.youtube.com/watch?v=3ZCHmli9H1g
- Sprites: Mafia Portrait copied from https://captainskolot.itch.io/mafia-men-portrait-pack-asset-pixelart-pixel-art-sprite-badass-bust-pack-rpg-vi
- Interesting educational Prisoner's Dilemma Game that gave inspiration: https://ncase.me/trust/
