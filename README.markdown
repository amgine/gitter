# Gitter

## Introduction

Gitter was originally developed as a standalone repository management tool for Windows with powerful GUI.

It is written completely in C# and targets .NET FW 4.0 Client Profile.

Application is mainly tested on Win7, but should work on Vista/XP without any problems.

## Repository access options

Currently, gitter supports interaction with repository through comand-line interface only, so you'll need git installation to work with gitter.
Gitter was tested with MSysGit installations.

Get MSysGit: <http://code.google.com/p/msysgit/downloads/list>

Minimum supported git version: 1.7.0.2

## Prerequisites required to build and run gitter

The only thing you need is .NET FW 4.0 Full or Client-Profile:

Get .NET FW 4.0 Client-Profile: <http://www.microsoft.com/download/en/details.aspx?id=24872>

## How to build gitter

If you don't have MS Visual Studio 2010:

1. Ensure that you have .NET FW 4.0 installed (full or client-profile)
2. Clone gitter repository
3. Run 'build.cmd'
4. Binaries can be found in 'output/Release' directory

If you have MSVS2010 installed, you can also open 'gitter.sln' solution file and build it.

## Features

Gitter supports most fetures that you can expect from git GUI application:

### History browser

* Applies topological sort or sorts by commit date
* Can limit displayed commit number
* Can filter displayed references

### Commit tool

* Allows staging/unstaging files and directories
* Can launch merge tool for conflicted files
* Supports --amend

### Branches

Can show, create, rename, delete (including remote tracking branches), checkout

### Tags

Can show, create and delete (annotated, lightweight and signed)

### Synchronization with remotes

* Fetch
* Pull
* Push: knows about --force flag, allows to push multiple branches at once
* Can add, remove and rename remotes
* Shows references for selected remote and allows to remove them

### Submodules

* Can show list of submodules
* Allows some basic operations: add and update

### Other git operations

* Merge: one or multiple branches, knows about --squash, --no-ff and --no-commit options, allows custom merge commit message
* Rebase: detects state, allows to continue, skip or abort conflicting commit
* Revert
* Cherry-pick

### Other features

* Spellchecker included (commit messages, tag messages)
* Can download and show avatars from gravatar.com
* Real-time tracking changes in repository made by other processes (detects all kinds of working directory changes and some basic changes of git directory, like creating/removing branches, tags and remotes, changing configuration, etc)
