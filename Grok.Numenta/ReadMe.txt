# grok-net

This document is the API specification for the Grok .Net Client Library.

A note about releases:

Development of Grok client libraries happens internally and
is then released to github master at the same time as we
release server side code. Github master branch can thus
be considered STABLE.

## Installation

### Using git

Clone the repository into your eclipse Workspace

workspaceDir/$ git clone git@github.com:numenta/grok-net.git

### Using the package

Click 'Downloads' here in github

Click 'grok-net.zip' to download

Unzip that into your Eclipse workspace directory

## Getting Started

From Visual Studio 2012 you can import all projects:

File ->	Open -> Project / Solution

Browse to workspaceDir/grok-net/

Select Grok.Numenta AND HelloGrok (and optionally Grok.Numenta.Tests)
- OR -
Select grok-net.sln

Click Open

Add your API key to HelloGrok\App.Config

Right click on the HelloGrok Project and choose "Set as Start Up Project"

Click Run (or hit F5)

Congratulations!  You're now running Grok!

## Examples

* **HelloGrok** - Get started with Grok.  Create projects, streams, models, and a swarm.
* **HelloGrokPart2** - Extend part 1, promote a model and perform online predictions.

## Getting Help

Visit https://grok.numenta.com/support
or send an email to support@numenta.com.

