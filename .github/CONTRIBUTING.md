# Contributing Guide

## Welcome

Welcome and thank you for considering contributing to this project!
Please take a moment to read the basics about working on this project and be aware of our [Code-of-Conduct](CODE_OF_CONDUCT.md).

We welcome any type of contribution: 
- **Code**: add code through pull requests including new features or bug fixes (check the [open issues](/../../issues))
- **QA**: add [issue](/../../issues/new), label it as `bug` or comments to other issues
- **Roadmap**: contact us if you have ideas to be considered in our roadmap
- **Exploitation**: write blog posts, tutorials, present the project on public events, ...

If you just want to know how to use the project, see [README](../README.md)

## Is this your first contribution on Github?

If you are not sure how to work with Pull Requests, you can find an introduction here: [How to Contribute to an Open Source Project on GitHub](https://egghead.io/series/how-to-contribute-to-an-open-source-project-on-github).

## Pull Request Guidelines

- If you want to add a new feature, add a [issue](/../../issues/new) (with label `enhancement`) first so that the community has the chance to discuss it and provide helpful input
- Consider the branch [naming converntions](#branch-naming-convention)
- Keep your commit history clean: changes are atomic and the git message format is considered
- Always `rebase` your work on top of the upstream branch to ensure your commit history is clean and linear
- Keep the `.gitignore` file up-to-date
- Feature added?
  - Make sure that the feaure is tested and add a new test if needed
- `dotnet test` needs to be passed
- `Coding Guidelines` have to be considered: We use [StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) as nuget package to check StyleCop rules during build process

## Branch Naming Convention

Base your work on the `master` branch (as the current snapshot) or a current release branch if available.

- `feature-*` for new features
- `fix-*` for bug fixes
- `tests-*` when updating test suite
- `refactor-*` when refactoring code without bbehavior changes
- `docs-*` for [README.md](../README.md) updates (or similar documents)

## Commit Messages

Make sure that you have your own user and email setup correctly in git before
you commit

Take the time to write good commit messages.

Commit message template:

    ---- start ----
    [short line describing main purpose]
    -- empty line --
    [description, why this change is made]
    -- empty line --
    [close/fix #xxxx - representing dedicated github issue ids]
    ---- end ----

## Comments

Please add comments to your code changes.

Use [XML comments](https://docs.microsoft.com/en-us/dotnet/csharp/codedoc) format in your comments.

## Review of the Pull Request

After you have created your Pull Request, a project maintainer reviews it and decides wether it will be merged or rejected.

Try to avoid large Pull Requests as it will take longer to review them.

## Development Setup

Requirements:
- Setup [.NET Framework](https://dotnet.microsoft.com/download) **Version 4.6.1+** or [.NET Core](https://dotnet.microsoft.com/download) **Version 2.0+**

Editor Suggestion:
- [Visual Studio](https://visualstudio.microsoft.com/) **Version 2017+**

Visual Studio Extension Suggestions:
- [Visual Studio Spell Checker](https://marketplace.visualstudio.com/items?itemName=EWoodruff.VisualStudioSpellCheckerVS2017andLater)
- [GhostDoc Community](https://marketplace.visualstudio.com/items?itemName=sergeb.GhostDoc) | [GhostDoc Pro/Enterprise](https://submain.com/products/ghostdoc.aspx)
- [PrestoCoverage](https://marketplace.visualstudio.com/items?itemName=PiotrKula.prestocoverage)

## Project Structure

We use a project stucture based on [David Fowl's](https://gist.github.com/davidfowl/ed7564297c61fe9ab814) suggestion:

```sh
$/
  artifacts/
  build/
  docs/
  lib/
  packages/
  samples/
  src/
  tests/
  .editorconfig
  .gitignore
  .gitattributes
  build.cmd
  build.sh
  coverage.rsp
  GlobalSuppressions.cs
  LICENSE
  NuGet.Config
  README.md
  stylecop.json
  {solution}.sln
```
- ```.github``` - Information and templates for project contribution
- ```src``` - Main projects (the product code)
- ```tests``` - Test projects
- ```docs``` - Documentation stuff, markdown files, help files etc.
- ```samples``` - Sample projects
- ```artifacts``` - Build outputs go here. Doing a build.cmd/build.sh generates artifacts here (nupkgs, dlls, pdbs, etc.)
- ```packages``` - NuGet packages
- ```build``` - Build customizations (custom msbuild files/psake/fake/albacore/etc) scripts
- ```build.bat``` - Execute the build on Windows
- ```build.sh``` - Excute the build on *nix
- ```coverage.rsp``` - Configuration to collection test coverage with [Coverlet](https://github.com/tonerdo/coverlet)
- ```stylecop.json``` - [Configuraiton of StyleCop  Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md) (linked to all projects of solution)
- ```GlobalSuppressions.cs``` - Manage SupressMessage attributes for poject (linked to all projects of solution)
- ```tests.bat``` - Execute tests on Windows
- ```tests.sh``` - Execute tests on *nix

## Development Lifecycle

### Step 1: Fork

Fork the project on GitHub and clone this fork locally.

```sh
$ git clone git@github.com:username/<forkedprojectname>.git
$ cd <forkedprojectname>
$ git remote add upstream https://github.com/<projectname>/<projectname>.git
$ git fetch upstream
```

### Step 2: Branch

Create local branches to hold your changes. 

These should be branched directly off of the `master` or `release-*` branch.

```sh
$ git checkout -b my-branch -t upstream/master
```

Note that your new branch will track changes of the upstream/master now.

### Step 3: Install/Build

To get all required dependencies, run:

```sh
$ dotnet restore
```

Now you are ready to make changes.

### Step 4: Code

If you have made some code changes, be aware of the coding guidelines. We use [StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) as nuget package to check StyleCop rules during build process.

After you have build code, check for new StyleCop issues. They are shown as warnings.
```sh
$ dotnet build
```

Possible rule violations can be found in the [StyleCop Documentation](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/DOCUMENTATION.md)

Some rules can be auto-fixed by Visual Studio
![Visual Studio - Auto fix StyleCop issue](../docs/images/VisualStudio_AutoFixStyleCopIssue.png)

### Step 5: Commit

Keep your changes grouped logically within individual commits. 
This will make the review process easier.

```sh
$ git add changed/files
$ git commit -m "Commit message"
```

### Step 6: Rebase

Use `git rebase` after you have commited a change.

Do NOT use `git merge`.

```sh
$ git fetch upstream
$ git rebase upstream/master
```

If there are conflicts, solve them e.g. by using a diff tool like Meld.

Then continue to rebase

```sh
$ git rebase --continue
```

This ensures that your working branch has the latest changes from the upstream repository
and your history is clean and linear.

In case you want to abort the rabse process:

```sh
$ git rebase --abort
```

### Step 7: Test

New features should also add new tests. See [Testing](../docs/TESTING.md) for more information about testing and coverage reports.

### Step 8: Push

Once you have finished your work prepare your pull request by 
pushing your working branch to your fork on GitHub.

```sh
$ git push origin my-branch
```

### Step 9: Opening the Pull Request

Create a new pull request from your fork on Github.
A pull request template will help you to provide all information needed 
and to check if all requirements are met.

 ### Step 10: Update a Pull Reuest

It is likely that you get some comments to your Pull Request and requests for changes by the reviewer.
This is part of the review process.

In this case, you can still make changes to the existing Pull Request.

Simply make the changes to your local branch, add a new commit and push to your fork.
GitHub will automatically update the pull request.

```sh
$ git add changed/files
$ git commit -m "Commit message"
$ git push origin my-branch
```

Check if a `git rebase` is neccessary before. 
If you have a conflict after a rebase with your remote branch on origin, 
and you are the `ONLY` one working on it, you can overwrite it on origin by 

```sh
$ git push --force origin my-branch
```

### Step 11: Finally

The Pull Requests will be successfully merged if:
- Code reviewer accepted the Pull Request
- CI successfully checked the branch of the Pull Request

## License

By contributing to this project, you agree that your contributions will be licensed under its [LICENSE](/../../blob/master/LICENSE).

## Questions

If you have any questions, add an [issue](/../../issues/new) and label it as `question`
