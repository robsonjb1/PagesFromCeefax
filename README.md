[![pfc tests](https://github.com/robsonjb1/pagesfromceefax/actions/workflows/master_pagesfromceefax.yml/badge.svg)](https://github.com/robsonjb1/pagesfromceefax/actions/workflows/master_pagesfromceefax.yml)

## PagesFromCeefax

[https://pagesfromceefax.azurewebsites.net](https://pagesfromceefax.azurewebsites.net)

This is a tribute to Ceefax, the BBC's teletext service.

The site was developed using .NET/C# with Visual Studio 2022 for Mac.

- To run locally you will need to get a key from OpenWeatherMap.org. You can sign up for free, only their "Current weather and forecast" service
is required. You will need to place your key in the appsettings.json file.
- NB: I've not added any of the .mp3 audio files into Github. These can be manually taken from the website if required.

## How it works

This is a simple .NET 6 minimal API that serves an HTML carousel of teletext-style pages.
The content is taken from the BBC RSS news feeds and cached on the server. It is refreshed every 20 minutes.

Even though the browser has the entire carousel in memory, it cycles through each page according to the current time in order to re-create the
'Pages from Ceefax' in-vision TV broadcast service that will be familiar to many. If left running, the browser will periodically request new carousel data.
The 'You are watching...' banner page acts as a useful distraction whilst a new carousel is being retrieved.

I'd welcome any comments and feedback!

## Thanks

Attributions:
- Font based on http://fontstruct.com/fontstructions/show/825647
     (I added in the necessary teletext 3x2 block graphics characters)
- Header artwork was taken from REAL Ceefax pages using the BeebEm BBC Micro emulator (which supports Teletext adapters)
     and used a 'digital' feed of Teletext from some time in 2006.
- Background TV image using licensed stock art imagery purchased from shutterstock.com

- Overall concept inspired by Matt Round's Jellytext (mattround.com)

- The audio was taken from the following YouTube video (attribution unknown)
-   https://www.youtube.com/watch?v=JU8P5G-GM_g
