# AstroMust

The gitlab repo for the astromust project without any large-file assets such as asset packs or 3D models.
Those are stored in a seperate .unitypackage file that you will have to install manually after cloning this
repo, this is crucial in order to get the project to run successfully.

All external assets will be located in a `ExternalAssets` directory, while the bespoke assets and code that
we create ourselves will be in the `ASTROMUST_MAIN` directory.

## Setup Instructions (New)

TLDR:

- Clone the repo with git, and switch to whichever branch you wish.
- Download the `AstromustExternalAssets.zip` and extract the folders **into the Assets directory of the project folder**.
- Open the project with Unity version `2021.3.21f1`

Link to the ExternalAssets.zip file [is here](https://drive.google.com/file/d/177YslDbAe4Wa6hhovswV9lKmIaBM8Jhr/view?usp=share_link).

### External Assets

After you've cloned, note that any assets under `ExternalAssets` won't be included with a git commit,
and if you add any new assets there you would have to create an updated `.unitypackage` and notify the
team. This should however not be necessary, as most of the assets we'll create ourselves.

---

### Building

You can build for any platform as you normally would, note that when playing in the editor, you may
need to enable "debug controls" so you can use mouse button inputs for looking around.

When working on a feature, create a seperate branch, i.e: `feat/database-backend` or `chore:/fixed-textures`.

Then, simply make a merge request when it's ready for review.
