![](https://forums.launchbox-app.com/uploads/monthly_2017_11/tumblr_mcoma8r8jv1qbxlono1_500.gif.bb3379452f0a7273e9f944b6aa6bccfd.gif)

These are configuration files to be used in conjunction with the PS2 emulator [PCSX2](https://pcsx2.net/) and the [PCSX2 Configurator](https://github.com/roguesaloon/launchbox-plugin_pcsx2-configurator) plugin for [Launchbox](https://www.launchbox-app.com/). These require **PCSX2 1.5.0 or higher** to function properly. You can find the latest development builds (1.5.0) available [here](https://pcsx2.net/download/development.html).

The configurations have been tested and created on a mid-range PC (Win 7/64-bit, i7 4770k, GTX 780, 32GB RAM) and are designed to eliminate as many problems and visual errors as possible, while still upscaling (by 4x generally) wherever possible.

An extensive spreadsheet covering the specifics of each config and detailing issues present (if any) can be found [here](https://docs.google.com/spreadsheets/d/1sC19l0htx0Qu2QE1CFta5R35iqPBN332dCHfTg1BGlQ/edit?usp=sharing).

Further details and discussion of the project can be found on the Launchbox forums [here](https://forums.launchbox-app.com/topic/41653-zombeavers-pcsx2-configs-simple-1-click-installs-with-pcsx2-configurator/).

**Performance improvement tips (if necessary)**

If a config isn’t performing well on your PC, my first suggestion is to try enabling MTVU in speedhacks (if it isn’t already enabled for that config). Technically this can impact compatibility which is why I usually leave it off when I can get away with it, but generally speaking it causes very few issues so it’s a good place to start.

![](https://forums.launchbox-app.com/uploads/monthly_2017_11/MTVU.png.a49a2314162e67801c2219614e84ed5f.png)

If MTVU alone doesn’t work, try dropping the scaling by 1x at a time until performance improves. Most of the configs use 4x scaling, which is middle-of-the-road. If performance is struggling, dropping that to 3x (or 2x) can make a big difference.

![](https://forums.launchbox-app.com/uploads/monthly_2017_11/5a035ed1f1c76_InternalResolutionScale.png.db95882183c15e90451931b1ac3104f6.png)

One additional thing you can try is changing the sync mode in the SPU settings from timestretch to async. I mention this one last because it has the highest probability of causing problems, but compatibility is still quite good overall. Basically what this is doing is changing it so that audio and video no longer have to be 100% synced at all times, which sounds like a bad thing but in practice it’s not. With timestretch there can be times where, if the game is performing poorly, you’ll get this really awful, stuttery, warbled mess for the audio because it’s going out of its way to keep them in sync and it creates a bit of a mess. Async can make these problematic moments much less noticeable. It's useful for games that have occasional and brief dips in performance - if a game is just constantly running at 50% speed though, the only thing it's going to do is make the audio way out of sync from the game. Again though, I recommend starting with MTVU and reducing the scaling first.

![](https://forums.launchbox-app.com/uploads/monthly_2017_11/Async.png.ad0d95feecb43c57a5038e2c28e42046.png)

You can also try adjusting the EE Cycle Rate, but I don't recommend doing this unless you really have no other option. This has a significantly higher probability of breaking things, so leave it alone unless absolutely necessary.

My hope is that the vast majority of people won’t have to change anything at all, but I wanted to give som﻿e pointers here in case y﻿our system is struggling.
