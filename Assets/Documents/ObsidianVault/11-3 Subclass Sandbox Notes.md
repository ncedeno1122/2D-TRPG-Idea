### PhaseManagers?
I was thinking a lot about how to specifically implement the *Subclass Sandbox* pattern for my GMD210 class in this project, and I think I know how I'll do it. Since I'm opting for a more data-oriented approach with CharacterUnitScripts (name may  have to change to UnitScript btw) and their CharacterUnit Data as opposed to some inheritance structure. However, I was thinking about the possibility of inheritance with this approach with, conveniently, the next issue I've been thinking about lately: the whole Turn system and all that.

So what I'm thinking starts with the idea of a Turn, which I want to be able to store as data. That data is a collection of TileActions that units perform, in an order. However, this needs more specificity as we imply 'order'. In Fire Emblem games, this is done in 'phases'. There's a Player Phase, and Enemy Phase, and an Ally Phase. There could be more, but I think that would depend on my own architecture and liberty in this project.

Continuing down this road, I need to create the logic for specifying whose Turn it is, depending on their Allegiance most likely. Currently, my Allegiance enum consists of { PLAYER, ENEMY, ALLY } and that's good enough to start. However, if I wanted more of these Allegiances, I'd have to have some sort of Phase data to help manage each Allegiance Units' turns. This sounds familar with some of the ideas I was working with in the last 11-1 State Notes note.

The question is, how might I architect this data?

I think something like this might make sense in terms of storage, anyway:
```
(
	(Turn 1, [Player Phase Moves, Enemy Phase Moves, Ally Phase Moves]),
	(Turn 2, [Player Phase Moves, Enemy Phase Moves, Ally Phase Moves]),
	(Turn 3, [Player Phase Moves, Enemy Phase Moves, Ally Phase Moves]),
	...
)
```

Well, for the `Player Phase Moves`, `Enemy Phase Moves`, `Ally Phase Moves`, and however many more I might need, these all have the potential to be some typed classes that hold this information. I want it to be serializable or encoded somehow. 

### But wait (again)!

If I'm ultimately storing TurnActions that involve things like moving, waiting, moving and performing other actions, I need to first come up with a way to store this, at least at first. These are actually things that would more accurately, perhaps, illustrate the Subclass Sandbox pattern than what I'm attempting right now.

So now I'm creating the Commands representing TurnActions, which were previously called TileActions. I think tying the name to turns instead of the tiles they occur on is slightly more relevant, IDK. Either way, I have realized something about the Tile selection process. I **SHOULDN'T** change the CharacterUnit's position as it navigates to the Tile as I have it doing now. That obviously opens the UI context menu that I haven't gotten out yet, but not changing the position within the CharacterUnitScript is ESSENTIAL. This is what I'm going to pull to get the OriginPosition to describe the TurnActionCommand, whatever it'll be, and the position that the SelectedTile is at the TargetPosition.

## Now it's 5am...
I did it again... But I did rest intermittently from about 11 -> 2, so that much wasn't too bad. In the meantime though, I have to write about what I've done (for my commit too) and what I have left to do as I turn in for a few more hours.

#### What I've Done
So what I've DONE besides a few slight refactors and reorganizations around on many files of code, I've been tackling the Turn Data problem that I've needed to work on for a bit now in the best way I know how - head on in a weird moment of clarity in the night.

One of the first things I worked on was in creating the whole folder of `BattleDataScripts` tonight, which is going to help me encapsulate moves as data with the Command Pattern. I've started with only the `WaitCommand` at the moment. The BattleDataManager is going to be informed with a larger, greater `TurnState` idea I have in mind.

Secondly, I started the ActionPrompt UI. I have to program the valid-actions-defining function that shows only the relevant buttons to the specific character. This has to be informed by the GridHelperScript as well, but I haven't gotten there yet. I have to also create some outgoing event that I send `TurnAction`-enum values to, which can be handled by the future-`TurnState` (to integrate with my `BattleDataScripts` and create Phase and Turn data to store >:]).

#### What's To Do
What I need to remember is the `TurnState` State-Pattern-implementing structure. This is something I'm really excited to work on when I get the ActionPrompt UI things done. This will help take the load off of `GridHelperScript` where I'm currently handling that. Super exciting stuff!

The main thing about this is that these concepts seem somewhat interdependent. I can't really make the `ActionPrompt UI` (that only appears during a state of the Turn selection process) without making the `TurnState` state machine to help manage all of the states that go into the game flow as well. Not to mention, $I want that data real bad$ about the actual Units' actions as commands, which I need to define during runtime with the `ActionPrompt UI` and schedule/sequence/make-sense-of with  the `TurnState` machine. So yeah, this is kinda silly.