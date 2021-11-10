# I'm going to the Grand Canyon today
...and, I'm still putting in some work on the drive there.

One of my main objectives today is generating the list of available actions that the `ActionPromptState` will choose from. The only thing is, where do I put this logic, and why?

I think this should probably go in `ActionPromptState` after all. This, and the range associated with the actions after will be relevant an `SelectFromRangeState`, and I don't need to consider them before the state.

I do worry somewhat that the `ActionPromptState` is getting a little chunky, but I guess that's just the way of things. As I refactor and do other stuff, I'm sure I'll realize or accept the true size this class ought to be.

---
### Now I'm back Home (in the AirBnB)
The Grand Canyon was certainly... vast (I can' t JUST say great). It makes me wonder how such a thing was formed, and how one could generate canyon shapes and things like that from some sort of heightmap.

In any case, I'm back home and I'm working to follow up on my earlier tasks. I need to find some way to not only get some collection(s) of things within the Actionable and/or Adjacent ranges around a TargetPosition.

Before, I was worried about how I might maintain a bunch of lists storing TileEntities that were valid for each TurnAction. This approach seemed like caca, and almost certainly was, in hindsight.

Here's the process as I'm seeing it currently:

1. So I have to figure out what exactly I can do to get the TileEntities in the range from the TargetPosition.
2. Then, I have to determine all of the potential actions I could do with each one.
3. Excluding all of the actions I COULDN'T do, I then have to display the ActionPrompt with all the options I can do.
4. Once an action is selected, I need to get a list from all of the TileEntities in the range that I will feed to `SelectTargetFromRangeState` to cycle through and select.

Step 1 is done, which is nice. But now that I have the range, I have to come up with all of the actions and stuff like that that I will have to do. I think some helper function in the `GridHelperScript` could do this. Though, some of the options I have, like Talk, Interact, etc., are forcing me to think of how I might implement and check if a TileEntity is any of those things. It could be through another component or something, or a subclass of TileEntity like my current `CharacterUnitScript`, but I have to find out how to do that before I can pull up the ActionPrompt as well.

One idea I had was to wrap all of these states in a binary-encoded integer with sufficient digits to represent each of the TurnActions except for being able to wait.

	public enum TurnAction  
	{  
	 TALK = 0,  
	 INTERACT = 1,  
	 ATTACK = 2,  
	 HEAL = 3,  
	 COMBAT_ARTS = 4,  
	 CHEST = 5,  
	 ITEMS = 6,  
	 TRADE = 7,  
	 CONVOY = 8,  
	 WAIT = 9,  
	 // TODO: Specify more actions to be done on Tiles  
	}
	
As an example, a PLAYER-allied CharacterUnitScript with items could probably be described as `0b000100110`.
The thing about this system is that there are some actions, like WAIT that always show up. 

This binary value represents the possible actions 
that the current unit can have with any of the other units within the range.

I feel like this is fallible in some sense, but I'm not sure how exactly. I think this is going to need to be a binary string, parsed as something that's not a byte. The reason why is that I may have more TurnActions in the future, and to best support those options I should do it as a string. As well, I can convert with Int32.Parse().

The thing that I worry about is that there are some fields that only pertain to the user, NOT the user's relationship with their target, which is what I made the whole thing for (in the case of the user being able to use ITEMS and access the CONVOY). Then again, I can probably omit those as normal booleans and just make the strings focus on the relationship-relating aspects. 

---

So it's tomorrow now, and I don't want to make a new notes file with this one being so short and having to continue these ideas today, so I'll resume today.

So now I've written a function to return a binary string encoding all the valid relational actions that one User can have with a Target As well, I've created a `Dictionary<TileEntity, string>` that stores all of the valid relational actions.
NOW, I need to run "analytics" and be able to interpret this Dictionary, being able to read and check for certain relational actions, and eventually extract a list of all the TileEntities that have valid interactions.

---

Well, I've gotten it done now, the task of generating the proper buttons based on the valid relational actions in the surrounding range.

As well, I've also fixed an issue with the animation of walking to the tile while exiting the `TargetSelectionState`, regarding the fact that the internal ValidMove and ValidActionable tile lists were no longer defined. I couldn't get the adjacent tiles, so I couldn't populate the A\* open list, so I couldn't get a path, and my character couldn't follow a path that didn't exist. Now, I've got it, though.

Now, I think I'm good to start implementing all of the states past this point... How exciting!

I should probably start with the `SelectTargetFromRangeState`, seeing as that will have to lead up to the probably-simpler `TargetConfirmationState`.
I also can't forget about the more UI-centric states for items, trading, and using the convoy.