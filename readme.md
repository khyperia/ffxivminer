FfxivMiner
===

This is very much some garbage I spewed out for myself and friends, but hey, if you find it interesting, neat I guess.

Deployed version can be found [here](https://khyperia.com/ffxivminer/). The data might be out of date, as I might 
need to run `download.ps1` again and republish the site.

---

How to build and use this project:

1) Run `download.ps1`. The data files it grabs can also be fetched from your local ffxiv install with [SaintCoinach](https://github.com/xivapi/SaintCoinach/releases), but eh, if they're already published on github all nice-like, just download those.
2) Run the C# project (`dotnet run`), which processes the files from step 1 into `data.js`
3) (khyperia only) - run `serverize.bat` to publish `index.html`, `index.js`, and `data.js` to khyperia.com
