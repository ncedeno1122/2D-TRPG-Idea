# TileSelectionStates
So here we are. Thinking about this, I understand that (again) the AI *doesn't* need this process in the same way the player does. The player has helpful UI, this and that, all that jazz. What matters most to me about this structure is that I don't need to rush any sequences or anything like that, I only need to let the states know to progress from external messages that help us know that the player or AI has settled on a move or something like that.

So that's one thing I have to achieve, which is ONLY progressing / changing states and all that once we're prompted to from some event / notification.

Now, I have to think about the steps in the process again, to create states from. Then again, though, most of the work *seems* to be done for me in what I did with the `GridHelperScript`. It'll be nice to return that class to its name's implications of mostly being a utility class.

In any case, I have three states again I think:
1. CharacterUnit isn't yet selected (SelectingUnitState)
2. CharacterUnit is selected, TargetTile isn't yet selected (SelectingTargetState)
3. ActionPromptState
	1. Helps the Player decide on their move and all that. This can probably be circumvented for AI somehow.
4. Another state to transition back to 1? Probably not?

### Resuming Work Overnight
Oh yeah baby.
In any case, I've moved forward with my architecture decision of using the TileSelectionStates and all that stuff. You can find the `TileSelectionManager` that maintains three states. This also has to take over the current duties of the `HandleSelectTile(Vector3Int tilePosition)` function in the `GridHelperScript`. Eventually, I will be able to decide the accessibility of all of the functions in that script to be used with the `TileSelectionManager`, but also have `GridHelperScript` focused mostly on helper functions related to pathfinding and getting areas of the main Grid.

That said, the first state is mostly fleshed out.

There are more things that need doing though:

One problem is that there's a *lot* of buttons, and hooking  them up so that they return some int value representing their order seems like it can be done in an easier way to fulfill the `HandleInput(TurnAction ta)` function in the `ActionPromptScript`. This seems like it can be automated.