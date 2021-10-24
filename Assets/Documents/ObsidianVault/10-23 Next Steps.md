# 10/23 Next Steps

## State Pattern for UI
So I want to continue to try and implement the patterns that I learn in my GMD210 class in this project. The pattern I have to use next is the State pattern, and I don't think that'd be a bad thing for something that has a palapable State, like the *GridHelperScript* or something like that. I remember that I attempted basic state machines and things like that in the ReWorld internships. I'm not really sure about flow-based UI and all that. This seems to have some sort of State.

The main problem that I arrive at is the need to name the different states. I'll need to look into different state-related terminology and all that.

## Refactoring A* path?
There are certain cases using the algorithm I do to find a path at extreme ranges that do not seem to work properly. I tested this with the TestItem10 (I think) that has a healing range of 3.

The issue is that I'm not executing a Depth-First search (I also think) and that I'm not finding the shortest walkable path to the destination.
This is likely a result of the fact that I'm finding a walkable path to the destination and then pruning of the tiles that aren't walkable. As a result, this is correct a lot of the time, but not all of the time in the case of clicking on the furthest tiles away over two sets of rock tiles.

From some of my pathfinding research before, I learned that Depth-First search is not half bad for finding the shortest path first. I think that's going to be key. Is this what I should do instead of A*?
I liked the A* because of its very modular heuristic and all that, perhaps I'm just misusing the algorithm or my path it generates here.

I wonder if I have to choose? I know that in Fire Emblem games, there's an arrow that follows your cursor within the walkable range during the Tile Selection State. This, I think, uses DFS, as it finds the a fairly simple and certainly short path to the target Tile. Perhaps I *should* opt for that one instead.

Overall though, I really do need to think about the archiecture of the whole system that combines with the flow. GridHelperScript is far from a specific enough name.

In fact,

## Architecture of the Tile Selection Prompt

So I'm thinking that however I architect this, I have to do it with respect to the following states that make up the whole Tile Selection / Player Move process:
1. TileEntitySelectionState
	* Used to select TileUnits, shows info for other TileEntities that aren't player-controlled or something like that
2. CharacterMoveSelectionState
	* Displays the whole movable tile BS
	* Used to help the player determine the Tiles they can move a character too
3. MoveCharacterState
	* Used to play the character movement animation and move the character to the target Tile
5. ActionPromptState
	* Prompts the Character for an action once a Target Tile is specified
6. MoveCommitState
	* Commits the move as a Command with its respective information
7. ActionAnimationState
	* Plays any animation associated with the Character's action

This is the order that the whole Move definition process should go under. Also I really need to decide on a name for the whole process so I can be consistent with it.

MoveDefinition Process does have a decent ring to it, though. That said, I have to think about the scope of this process and all that. After all, AI will use this as well! Yet, this process fits into something bigger. I don't want to just call it the GameManager, as that is usually reserved for other functions besides the game flow. This is probably also good to add intermittent states that help with the flow of the game where we apply TIle effects, changing the Player -> Enemy -> Ally phases and all that.

Perhaps GameFlowManager is a bit better for it? I'm not sure.

##### "Listener" States?
I'm not sure if it's a good idea to have blocking states that listen for specific events and all that to wait for specific input from some external event or something like that. These states wouldn't be Monobehaviours, so they might have to implement the Observer pattern so I can notify each state of a message.
The idea I had in mind for this is for the UI prompts and menus and stuff like that. In states like the ActionPromptState that rely on UI input to progress, something like this might be necessary. Then again, maybe not? Perhaps the button panel should only be active during that ActionPromptState, and that all states theoretically could receive input from the panel based on some master function in the GameFlowManager? This doesn't feel like a great approach, but it is certainly one approach I could take to achieve what I want.

I may write on this more, but I'm going to try and work with this idea on my PC. I forgot to say that I'm writing this on my Mac. In any case, time to commit and start cracking away at this idea.