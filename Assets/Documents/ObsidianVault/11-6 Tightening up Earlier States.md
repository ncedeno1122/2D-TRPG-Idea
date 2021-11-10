What's really exciting is that the almost complete refactoring of `GridHelperScript` is upon me! Instead of maintaining many (or any) private lists of tiles, I'm having to calculate and solve for those lists for my `TileSelectionState` children. That's where I'm at now.

Specifically, I'm getting around to continuing to implement the `TargetSelectionState` logic where we `DetermineNextState(Vector3Int targetTilePosition)`. This is where I need to figure out some of the logic from my last note, [[11-5 SNAKES ON THE PLANE]] (Check it out, I can link to other files!).
I may be overthinking it, but I need a few Lists:
1. The original Movable & Actionable Ranges from the User's starting position.
2. A new Actionable range from the User's target position.

I might have to rethink some of my original considerations about how to tell what state to enter next. For the record, **this is the state that must pinpoint these things.** I just have to think about how...

So for figuring out how to `DetermineNextState(Vector3Int targetTilePosition)`, I need to think about what the player selects as their Target.

Before anything else, we have two pieces of information about any Target Tile: their tile's location, and what's actually on the tile (if anything).

Thus, we have to make some considerations:
- Is there anything on the Tile?
- Is that Tile within the Actionable Range?
- Is that Tile within the Movable Range?
- If there is something on the Tile, do we have any valid interactions with them?
	- Can always trade with units of the same allegiance, but cannot always heal them (if they don't have healing spells, have to program that too :D)

These are a lot of (but perhaps not all of) the factors that I need to take into account. After all, this will determine essentially, whether I'm confirming a move on a target in the `TargetConfirmationState`, prompting for potential actions with the `ActionPromptState`, or just reverting back to the `CharacterSelectionState` if nothing else.

This is essentially it, it's just getting all of these lists and making some new helper functions to get this information. As well, I want to think ahead and at least attempt. Thing is, I have to wrap up for tonight, so I'll leave these thoughts for tomorrow.

BY THE WAY FOR TOMORROW, a potentially easy few helper functions to start with are:
- A function that gets the valid MovableRange (DFS?)
- A function that gets the valid AcitonableRange based on a MovableRange
- A function that takes in two 'out' (if not predefined) or two 'ref' (if using member variables) lists and populates them with the above two functions.

---
#### Before-Bedtime Notes
Also I figured out there are two different ways of thinking that I need to put down on paper before I get stuck between them again:
Firstly, I need to check if a TargetPosition is within the ORIGINAL MoveRange and ActionableRange, all that stuff.
Then, I need to check if there's a Target **on** that TargetLocation, which helps me determine whether I'm about to confirm some EZ action on that target, or whether to pull up the ActionPrompt to figure out what I'll do.

It's **ONLY** then in the `ActionPromptState` that I need to concern myself with things like checking for valid TileActions. After all, if our desired Target is already in range, all we should really have to do is confirm it by performing some action directly upon it. TLDR, we can defer most of the TurnAction-eligibility to when we enter the `ActionPromptState` and/or later states along that path before `TargetConfirmationState`.

---