![](https://forums.launchbox-app.com/uploads/monthly_2017_11/tumblr_mcoma8r8jv1qbxlono1_500.gif.bb3379452f0a7273e9f944b6aa6bccfd.gif)

These are configuration files to be used in conjunction with the PS2 emulator [PCSX2](https://pcsx2.net/) and the [PCSX2 Configurator](https://github.com/roguesaloon/launchbox-plugin_pcsx2-configurator-next) plugin for [Launchbox](https://www.launchbox-app.com/). These require PCSX2 1.5.0-dev-3400 or higher to function properly. **PCSX2 1.6.0 is recommended.** Do **not** use 1.4.0

The configurations have been tested and created on a mid-range PC (Win 7/64-bit, i7 4770k, GTX 780, 32GB RAM) and are designed to eliminate as many problems and visual errors as possible, while still upscaling (by 4x generally) wherever possible.

An extensive spreadsheet covering the specifics of each config and detailing issues present (if any) can be found [here](https://docs.google.com/spreadsheets/d/1sC19l0htx0Qu2QE1CFta5R35iqPBN332dCHfTg1BGlQ/edit?usp=sharing).

Further details and discussion of the project can be found on the Launchbox forums [here](https://forums.launchbox-app.com/topic/41653-zombeavers-pcsx2-configs-simple-1-click-installs-with-pcsx2-configurator/).

**Performance improvement tips (if necessary)**

As part of the update process from 1.5.0 configs to 1.6.0, I went through and benchmarked all configs with the framelimiter off, and made adjustments as necessary. The end result should be better performance (at the very least, better maximum performance, whether you actually need that or not) for everyone. Some key elements that can be used to improve performance (and generally have been used already where appropriate) are listed below.

Enabling MTVU is often a good starting point for improving performance. It has high compatibility and is an easy go-to. There are some cases where it causes problems in a game that I've noted on the details spreadsheet. This is generally enabled where appropriate and where there was an actual impact on performance in testing.

![](https://forums.launchbox-app.com/uploads/monthly_2017_11/MTVU.png.a49a2314162e67801c2219614e84ed5f.png)

If necessary, you can also try dropping the scaling by 1x at a time until performance improves. It should be noted that some games are more CPU-limited than GPU-limited, in which case reducing the scaling can have little to no impact on performance. It's something to try if performance is poor, however.

![](https://forums.launchbox-app.com/uploads/monthly_2017_11/5a035ed1f1c76_InternalResolutionScale.png.db95882183c15e90451931b1ac3104f6.png)

You can also try disabling Large Framebuffer. This is a parameter that used to be on by default but no longer is. It serves a primarily niche purpose as it only affects a small number of games (FMV flickering with it off) but can often negatively impact performance. I've generally disabled this in cases where appropriate, didn't cause an issue (or the issue could be remedied in an alternate way), and actually did improve performance in testing, but there may be additional cases where disabling it would be beneficial.

![](https://forums.launchbox-app.com/uploads/monthly_2020_05/image.png.7552dede8651d35377ae9785d112f05f.png)

Enabling Allow 8-Bit Textures can also improve performance depending on your hardware and the demands of the game. It basically shifts the demand on the CPU and GPU.

![](https://forums.launchbox-app.com/uploads/monthly_2020_05/image.png.7cd6bdea85dfffae5d48c90f86dc1d65.png)

One additional thing you can try is changing the sync mode in the SPU settings from timestretch to async. I mention this one last because it has the highest probability of causing problems, but compatibility is still quite good overall. Basically what this is doing is changing it so that audio and video no longer have to be 100% synced at all times, which sounds like a bad thing but in practice it’s not. With timestretch there can be times where, if the game is performing poorly, you’ll get this really awful, stuttery, warbled mess for the audio because it’s going out of its way to keep them in sync and it creates a bit of a mess. Async can make these problematic moments much less noticeable. It's useful for games that have occasional and brief dips in performance - if a game is just constantly running at 50% speed though, the only thing it's going to do is make the audio way out of sync from the game.

![](https://forums.launchbox-app.com/uploads/monthly_2017_11/Async.png.ad0d95feecb43c57a5038e2c28e42046.png)

You can also try adjusting the EE Cycle Rate, but I don't recommend doing this unless you really have no other option. This has a significantly higher probability of breaking things, so leave it alone unless absolutely necessary.

My hope is that the vast majority of people won’t have to change anything at all, but I wanted to give som﻿e pointers here in case y﻿our system is struggling.
