let nametoid = globaldata.nametoid;
let idtoname = {};
for (let name in nametoid) {
    idtoname[nametoid[name]] = name;
}
let marketMap = {}
for (let entry of globaldata.market) {
    entry.name = idtoname[entry.id];
    marketMap[entry.id] = entry;
}
let recipesMap = {};
for (let recipe of globaldata.recipes) {
    recipe.resultname = idtoname[recipe.resultid];
    for (let ingredient of recipe.ingredients) {
        ingredient.name = idtoname[ingredient.id];
    }
    recipesMap[recipe.resultid] = recipe;
}

function computeCost(marketEntry) {
    if (marketEntry.id in recipesMap) {
        let recipe = recipesMap[marketEntry.id];
        let craftCost = 0;
        for (let ingredient of recipe.ingredients) {
            if (!(ingredient.id in marketMap)) {
                // e.g. "Haddock" is not on marketboard. Assume zero I guess?
            }
            else {
                let ingCost = computeCost(marketMap[ingredient.id]);
                craftCost += ingCost;
            }
        }
        marketEntry.craftCost = craftCost;
        return Math.min(marketEntry.craftCost, marketEntry.medianppu);
    }
    return marketEntry.medianppu;
}

for (let entry of globaldata.market) {
    computeCost(entry);
}

function dumpHtml(marketEntry) {
    document.write(marketEntry.name);
    document.write("<ul>");
    document.write("<li>costs " + marketEntry.medianppu + " on marketboard</li>");
    if (marketEntry.craftCost) {
        document.write("<li>costs " + marketEntry.craftCost + " to craft (" + (marketEntry.medianppu / marketEntry.craftCost).toFixed(2) + "x ROI)</li>");
    }
    document.write("<li>sells " + marketEntry.itemsperday.toFixed(2) + " items/day for a flux of " + (marketEntry.medianppu * marketEntry.itemsperday).toFixed(2) + "</li>");
    if (marketEntry.id in recipesMap) {
        let recipe = recipesMap[marketEntry.id];
        for (let ingredient of recipe.ingredients) {
            document.write("<li>Ingredient: ");
            if (!(ingredient.id in marketMap)) {
                document.write(ingredient.name + " is not on the marketboard");
            }
            else {
                dumpHtml(marketMap[ingredient.id]);
            }
            document.write("</li>");
        }
    }
    document.write("</ul>");
}

function compareMarketEntry(left, right) {
    var leftProfitFlux = (left.medianppu - left.craftCost) * left.itemsperday;
    var rightProfitFlux = (right.medianppu - right.craftCost) * right.itemsperday;
    return rightProfitFlux - leftProfitFlux;
}

let displayMarket = [];
for (let entry of globaldata.market) {
    if (entry.craftCost) {
        displayMarket.push(entry);
    }
}

displayMarket.sort(compareMarketEntry);

let i = 0;
for (let entry of displayMarket) {
    if (i++ == 100) {
        break;
    }
    dumpHtml(entry);
}
