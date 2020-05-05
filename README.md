<a href="https://www.twitch.tv/thquinn">
<div>
  <img src ="https://i.imgur.com/wYEiqpF.png" />
  <p align="center">An interactive stream overlay for a word game show</p>
  </a>
</div>

# BFGStream

As promised to a tenacious viewer, here's the Unity project for my weekly stream. It's meant as a sample for how to tackle various issues that might arise if you want to make a custom interactive stream overlay. Some of the stuff it does:
* Renders for full-alpha transparency in OBS (use "Game Capture" in OBS and check the transparency box)
* Takes viewer submissions via a bot
* Handles follow, subscription, and cheer events
* Handles channel point reward events to in-game effect

**IMPORTANT:** Before you can run this project, you have to make your own "Assets/StreamingAssets/config.ini" file that provides all the Twitch OAuth keys for the bot account and user account that it uses, plus a Twitch API application ID, plus information about who to treat as hosts. Use the config_example.ini file in that directory to guide you.

I'll probably be adding new features for a while to come. There's plenty of janky stuff going on, so imitate at your own peril... but I do suggest you tune in and watch! It's every Tuesday from 8 PM to 11 PM Eastern. [My Twitch channel is here,](https://www.twitch.tv/thquinn) and all the recorded streams are on [my YouTube channel here.](https://www.youtube.com/playlist?list=PLrM20yT-tpKCXglkVMI58Ha_ZRYU8uC0B)

## Thanks to:
* **Eric Mann-Hielscher** for the countdown music.
* [**YouFulca**](https://wingless-seraph.net/en/index.html) for the lightning round music.
* [**John McDowell**](https://github.com/macdub) for his [Wiktionary parsing](https://github.com/macdub/go-wiktionary-parse).
* and finally, to **Will**: my star, my perfect silence...