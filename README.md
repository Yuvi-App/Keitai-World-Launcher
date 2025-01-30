# Keitai-World-Launcher
Universal Launcher for everything Keitai Emulation

# What it does
The Keitai World Launcher provides seamless access to Games, Apps, Chara-Den, Machi-Chara, and much more from a server. Fully portable and universally compatible, it is designed to work with any configured server or S3 bucket. The launcher centralizes access to Keitai resources, allowing for easy management and updates through a remotely configurable XML list.

# How to Setup a Server
### Client
The client only need to configure the "AppConfig.XML" with a couple address to point to your server. Such as Version Update URL, Gamelist XML, and etc. 

### Server
- VersionUpdate.txt for the current app version
- Gameslist.xml - XML for everything Games
- Machichara.xml - XML for everything MachiChara
- Charaden.xml - XML for everything Charaden

Generating a XML:
The XML file can be generated within the app itself. However, it reads from a master zip file that contains all the content you want to catalog. The format should be as follows:
- ListMaster.zip contains 30 folders.
- Inside each folder, there should be one .jam, one .jar, and one .sp file.
These files are then packed into their corresponding zip archives and XML files, making them ready for use with the client.

SD Card Data:
In the Gamelist.xml, there is a parameter specifying a URL for downloading SD Card data. Place the SD Card game data in a  folder, name it "SVC0000{GAME JAM}". Then compress (zip) the folder. This zipped folder will be used for downloading and running the game.
For example, RockmanDASH-sdcarddata.zip contains a folder named SVC0000rockman5island.jam, which holds 30 game data files. This structure ensures that the client can properly access and use the game data when needed.

### WIP
