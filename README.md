# AmazonEchoAlexaDotnet

This is a complete demo using  Alexa.NET

Within alexa developer center a skill with name Gorilla was created icnluding the following Intents:

## Gorilla Location

This Intent is called when the user try to know the gorilla's location. The request is resolved within the option:

`case "gorillalocation":`

## Gorilla Music

This Intent reproduces a gorilla song located in a external repository

`case "gorillamusic":`

## Gorilla Invitation

This Intent responses an invitation to a meetup

`case "gorillainvitation":`

## Gorilla Calculation

This Intent calculates you age processing the date when you were born

`case "gorillacalculation":`

####NOTES:

When the skill is opened the LaunchRequest return a Welcome message. Welcome to gorilla logic
