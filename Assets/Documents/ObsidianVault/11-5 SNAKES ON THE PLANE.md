# That's Right Baby 🐫
---
Despite the fact that I'm on the plane to go on vacation, I'm continuing to work for reasons unknown. I'm having a blast though, as I've gotten some good work done already! This has been a surprisingly effective way to kill 4-ish hours or so on the plane.

In any case, I've got a lot of the TileSelection / Move-making process working well, INCLUDING working `RevertState()`s. That's HUGE for me. The part that's left for me as I'm writing this paragraph in particular is to get the ActionPromptState up to snuff. But yes, currently, you can revert from the `TargetSelectionState` by clicking outside of the interaction range (which is painted in its `Enter()`) to get back to the `CharacterSelectionState`. You can exit the `ActionPromptState()` by clicking on the new button labelled 'Cancel'. Thank goodness this has turned out to be a much more managable solution compared to my original one in GridHelperScript.

----
By the way, OMG I CAN DO MATH? Let's see this...
$ax^2+bx+c = 0$
Look at that quadratic fella... Incredible! Thanks, Obsidian!

---
I was thinking about doing an `OnValidate()` overload where I iterate through the names of the children of the ActionPromptPanel (Buttons), and assign them automatically if the button is named appropriately. I've already hooked the buttons up, but I think that would be handy if I have to add more buttons in the future and things like that.
I'd still have to assign the `OnClick()` functionality through the inspector, but I'd have to see if I could automate that in the same place as I detect and assign them.
They were absolutely right when they said that once you start writing custom inspector scripts and other stuff like that that you don't stop LOL. It's just so handy though! I feel like a frontiersman, making my own workflows to survive as my tasks grow larger or more tedious to do by hand. How fun!

Also I didn't think about it as I ran my program to test for the *other* things, but I think that the code that I'd written haphazardly into `ActionPromptScript()` that displayed the buttons correctly. The test Cynthia I have has a healing item equipped, and the Healing Button showed when I pulled the menu up. I only wrote to display the Wait and Cancel Buttons, so I guess that means something's working well, LOL. Checking if that's actually the case.

---
#### Two Unlikely Friends: ActionPromptScript and GridHelperScript

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
Here's literally the current TurnAction enum. This will help me populate the below sections for determining how to how to determine if these requests are valid.

Oh MAN I'm so excited to work on this, because this is the time to write ALL the cool helper functions in `GridHelperScript` that I can.

###### Talk
If there are Talkable units adjacent to the User, this option should show.

###### Interact
Same as Talk but for Interactables.

###### Attack
We can only Attack if there are `CharacterUnitScripts` of the opposite Allegiance within the Actionable and Walkable ranges.

###### Healing
Same for Healing, but only for people of the same Allegiance.

###### Combat Arts
If there are `CharacterUnitScripts` of the opposite Allegiance within the range of any of the User's Combat Arts (not currently implemented).

The neat thing about this is, I don't need to check if they're in the interaction range or anything, I can probably just use the Manhattan Distance method to determine the range.

###### Chest
If there is a Chest unit or something adjacent to the user, this shows.

###### Items
If the Unit has literally anything in their inventory, this should show. No use in showing an empty inventory.

###### Trade
If the Unit is next to a `CharacterUnitScript` of the same Allegiance that has items, show this.

###### Convoy
If the Unit has a designation that allows them to use the Convoy, display this option.
Perhaps there could be like wagon units that let you use the convoy or something???

###### Wait
Waiting is always allowed :] (as far as I know currently).

---

My, that was comprehensive. Yet, it's something that may change as I continue to research how other TRPGs do this stuff. Either way, I'm really glad for what I've done so far. I've taken an abstract process that spans multiple systems, UI, normal backend scripting, scripting that involves gameObjects and stuff like that, and more. I'm far from finished, but this is one small step for man, one massive leap for mankind in my world. Seeing it work (for now) and be good (for now) brings a smile to my face. How nice.

## Oh my god

I think I forgot a state? If I did forget a state, how EXCITING would that be?!

Okay, so I was just thinking about how my pathfinding algorithm may need to be reworked with a depth-first search so I can favor the truly shortest paths to a target tile from the center (I think) instead of following the one the A\* algorithm likes.
You can see the sort of 'pathing artifacts' or potentially unfavorable paths you take when you try the approach I did. Potentially, that's to be expected, but that could ALSO be alleviated by creating a state where *when you select a Movable tile*, **you  can get ANOTHER TileSelection State that shows another Interaction Range (not for movement, just for the equipped battleitem's range) where you could select Tiles to target.**

Reading that through a second time, I think I'm just thinking of the selection process that comes when you go past the ActionPromptState state. So while, yes, I *forgot* a state, I haven't actually conceived its functionality or anything yet, so I didn't really *forget* it at all. What a pain.

In any case, I'll have to implement this. But what to call it? Obviously the more generic the name the better, and it can't be CharacterSelectionState, TargetSelectionState, or ActionPromptState. Perhaps something like...

Wait, nuts. I don't think I thought I'd reach this point as fast as I did, because my memory banks seem to have like NO pre-existing mental notes on the fact that ***I'll literally need quite a few new states to account for the selection process for  the TurnAction values...*** I am a fool. Also I found out just now I can use emojis. 🐪 😳

In any case, that's quite a doozy to take in. However, just like the underlying similarities between the TurnActions and their eligibility to have their buttons displayed, there must then be some generic approach to make states that help.

Thinking more about this, there's definitely a sort of generic selectable logic for any list of valid tiles. Think about in Fire Emblem, where this is the case. If you're surrounded by a bunch of enemies and you have a 2-range weapon, you can get a list of them, and focus on them. Hitting left or right, you can select a specific target to focus on.
This is so cool! And it fits in with my current logic, as we set potential Targets and TargetPositions in TargetSelectionState. If we select a Movable tile without a valid target on it, we can eventually reach this sort of selection state and set the Target that way.

That said, I'll need to make some different parent class instead of just looking to Target `CharacterUnitScripts`. In the cases of like Interactable, Chest, and other tiles (ALSO I FORGOT ABOUT KEYS/DOORS) that represent targetable obects that aren't units or characters or anything like that, this is why this is particularly useful.

This seems to be somewhat figured out, but that's not the only one I have to conquer. There's an inventory one I'll have to do for items. There's TWO inventory-like ones I'll have to do for trading. And then another one potentially for the convoy, that'll function the same was as a store's UI logic. If I can create a state for this and tie some termination event to something that we can advance the state to in the `TileSelectionManager`.

As well, there's another state that I had sometime in the past, but need to reimplement. Some state where I can reset back to the `CharacterSelectionState`and clear the CurrentMoveInProgress.

Finally, there's a flow here I have to recognize. It's not just a simple path from one to another, I will need to come up with some way to determine how to advance each state in a more in-depth fashion. As I have it now, I have each state calling their manager's `AdvanceState()` function, which has a current circular and linear flow. Should each state know how to advance depending on the input given? Really, I'm not sure it's the worst idea... When I have to revert this, it also may get messy, so I'm not exactly sure how I would proceed in this aspect.\

But, this may be necessary. Now that there are different paths conditionally to some of these states, it literally isn't a linear path there. Thus, I may need to just do the classic method in which each state decides where it'll go next, and **how to revert accordingly**. This concerns me, but it might be simpler than I think?

Changing this actually destroys the definition of reverting (reversion?) states in my current context. How exciting.

There's something I'm running into in getting rid of my main, concrete RevertState() function.
Because the path is no longer linear, should I keep track of the States somewhere to validate and track accordingly? Like, some sort of Stack that tracks all preceding states and all of that? That's more data, though, and I'd like to avoid unnecessary structures like that if at all possible.
Then, I think, should maintain a TileSelectionState variable representing the previous state in each one? No probably, that would actually lead to an infinitely-reverting process.
If I can't do that, then it seems I'm just going to have to conditionally recognize the state to go back to, which is far from unachievable. It just makes my skin crawl because it was my first idea and I fear being unable to discern the proper state to come from given the data available and all that. Then again, though, I won't know until I try.

##### NEW STATE HAH again....
Also I need to come up with a new state, like `TargetConfirmationState` or something. Basically, it's the state you enter when you click on a valid tile and there's a valid Target on it. You have acquired the target and are confirming it to be so.
This state is also reached when you take the alternate route, which is selecting a movable tile with no target in `TargetSelectionState`, entering `ActionPromptState` and selecting some action that requires a Target, entering the new `SelectTargetFromRange` state where you select from a list of targets, and finally exit that state having selected a target, and thus being about to confirm it. Marvellous. I feel like the Kronk meme, because 'Oh yeah, it's all coming together'.

---
### But what to focus on first?

I'm not entirely sure, again, what takes precidence here. I think these are things that are, again, closely related, so I have to develop them one baby-step at a time. That does excite me though, I love these brave bounds into the unkonwn I feel by attempting them.

As it's going now, I have to conditionally determine if there's a Target already located on the TargetPosition that we get from the `HandleInput(Vector3Int)` function in the `TargetSelectionState`. That is the first deviation from the normal flow, as:
- If the Tile is within Movable range and doesn't have a valid Target on it, `ActionPromptState` to see what we wanna do.
	- `ActionPromptState` should take us to the possibility of going to the `SelectTargetFromRangeState` which will then reach `TargetConfirmationState` upon selecting a valid target.
- If the Tile is within Movable range and **does** have a valid Target on it, go to `TargetConfirmationState`. That's what I'll call it.
- If the Tile is within Actionable range *but outside Movable range* and doesn't have a valid Target on it, do nothing I guess. Maybe even revert?
- If the Tile is within Actionable range *but outside Movable range* and **does** have a valid target, go to the `TargetConfirmationState`.

Well, that covers most cases I care about currently. The only other ones seem to be out-of-interaction-range cases which all simply lead to a reversion. Easy stuff so far, it seems. Time to go program this new state, though, that's exciting. The `SelectTargetFromRangeState` is a mouthful, but it's the state I was describing earlier that I'll need some helper functions for.
As well, this is finally the point at which I can safely retire the `HandleSelectTile(Vector3Int tilePosition)` function from `GridHelperScript`, as it's been surpassed. That goal is achieved, thank goodness!

#### BTW, Raycasting?
Getting into helper functions in `GridHelperScript`, I'm getting increasingly concerned about the efficiency of searching through a list of literally EVERY CharacterUnitScript that exists. While this has its own problems I'll tackle (such as needed some parent or thing greater than it to address when I search for targets), I need to find a way to search for things on given tiles more efficiently.

Instantly, my innate solution is to slap a trigger collider on the TileEntities (that's what I'll call those things OFFICIALLY that have a space and a GameObject on the map) and fire raycasts at them. Or maybe do some other collision-based check or something if my method proves inaccurate. In any case, this seems to be a MUCH better method of finding things on my map instead of searching through literally everything on it. Though, that's something for when we land. I believe that currently we don't have much time left on the flight, so I'm working diligently to cram as much work and notes as I can before I must stop.