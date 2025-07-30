# GOAP-AI Unity 6 Example

## About GOAP
This repository is to display the GOAP system and how it can be achieved in Unity 6 using C#. GOAP stands for Goal Orientated Action Planning and is an artificial intelligence system for autonomous agents that allows them to dynamically plan a sequence of actions to satisfy a set goal ([ref1](https://medium.com/@vedantchaudhari/goal-oriented-action-planning-34035ed40d0b), [ref2](https://en.wikipedia.org/wiki/Automated_planning_and_scheduling)).
 
GOAP is used in many systematic and simulation games, such as the Sims (Electronic Arts) or City Skylines (Colossal Order). GOAP seperates tasks and goals into modular steps which can be modified or changed at runtime. This creates an intelligent AI system which appears sentient.

## The Project
I created this project to better understand AI in video games, how they work in large simulator games, and how they can appear 'smarter' than they really are. GOAP is a great way of achieving this, as agents can have goals, fallbacks and success/failure states. This can enhance the gamers experience, especially in games which heavily rely on
AI.

For this example, I used the Store Simulator genre to demonstrate how GOAP can be used. I included simple Store mechanics (checkouts, stock placement, building etc) and created an abstracted GOAP framework - Goals, Tasks and Actions. They can be defined as this:
- Goal: The overarching task the AI will try and achieve (eg. Buy an item)
- Task: A collection of actions which will help complete the main goal (eg. Find product)
- Action: A singular action which will help complete a task (eg. Move to position)

This can be expanded further using composite and atomic tasks. Fallbacks can also be introduced, which allows an AI to dynamically decide an option based on the local environment (eg START -> Buy Goal -> ... -> FAIL -> Wander Store Goal -> SUCCESS -> Retry Buy Goal)
