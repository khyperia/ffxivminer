$webclient = New-Object System.Net.WebClient
New-Item -Force -Type Directory Data
Invoke-WebRequest 'https://raw.githubusercontent.com/xivapi/ffxiv-datamining/master/csv/Item.csv' -OutFile 'Data\Item.csv'
Invoke-WebRequest 'https://raw.githubusercontent.com/xivapi/ffxiv-datamining/master/csv/Recipe.csv' -OutFile 'Data\Recipe.csv'
Invoke-WebRequest 'https://raw.githubusercontent.com/xivapi/ffxiv-datamining/master/csv/RecipeLevelTable.csv' -OutFile 'Data\RecipeLevelTable.csv'
Invoke-WebRequest 'https://raw.githubusercontent.com/xivapi/ffxiv-datamining/master/csv/World.csv' -OutFile 'Data\World.csv'
Invoke-WebRequest 'https://raw.githubusercontent.com/ffxiv-teamcraft/ffxiv-teamcraft/master/libs/data/src/lib/json/nodes.json' -OutFile 'Data\nodes.json'
