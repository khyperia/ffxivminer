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
                craftCost += ingCost * ingredient.amount;
            }
        }
        marketEntry.craftCost = craftCost / recipe.resultamount;
        return Math.min(marketEntry.craftCost, marketEntry.medianppu);
    }
    return marketEntry.medianppu;
}

for (let entry of globaldata.market) {
    computeCost(entry);
}

function dumpHtml(marketEntry, amount) {
    let generatedHtml = "";
    if (amount == 1) {
        generatedHtml += marketEntry.name;
    }
    else {
        generatedHtml += Number(amount.toFixed(2)) + "x " + marketEntry.name;
    }
    generatedHtml += " - <a href=\"https://universalis.app/market/" + marketEntry.id + "\">open in universalis</a>";
    generatedHtml += "<ul>";
    generatedHtml += "<li>1x costs " + marketEntry.medianppu + " on marketboard</li>";
    if (marketEntry.craftCost) {
        generatedHtml += "<li>1x costs " + Number(marketEntry.craftCost.toFixed(2)) + " to craft (" + Number((marketEntry.medianppu / marketEntry.craftCost).toFixed(2)) + "x ROI)</li>";
    }
    generatedHtml += "<li>sells " + Number(marketEntry.itemsperday.toFixed(2)) + " items/day for a flux of " + Number((marketEntry.medianppu * marketEntry.itemsperday).toFixed(2)) + "</li>";
    if (marketEntry.id in recipesMap) {
        let recipe = recipesMap[marketEntry.id];
        for (let ingredient of recipe.ingredients) {
            generatedHtml += "<li>Ingredient: ";
            if (!(ingredient.id in marketMap)) {
                generatedHtml += ingredient.name + " is not on the marketboard";
            }
            else {
                generatedHtml += dumpHtml(marketMap[ingredient.id], amount * ingredient.amount / recipe.resultamount);
            }
            generatedHtml += "</li>";
        }
    }
    generatedHtml += "</ul>";
    return generatedHtml;
}

function compareMarketEntry(left, right) {
    var leftProfitFlux = (left.medianppu - left.craftCost) * left.itemsperday;
    var rightProfitFlux = (right.medianppu - right.craftCost) * right.itemsperday;
    return rightProfitFlux - leftProfitFlux;
}

function render() {
    let search = document.getElementById("search").value;
    let minlevel = document.getElementById("minlevel").value;
    minlevel = minlevel ? Number(minlevel) : 0;
    let maxlevel = document.getElementById("maxlevel").value;
    maxlevel = maxlevel ? Number(maxlevel) : Number.MAX_VALUE;
    let minitemsday = document.getElementById("minitemsday").value;
    minitemsday = minitemsday ? Number(minitemsday) : 0;
    let maxitemsday = document.getElementById("maxitemsday").value;
    maxitemsday = maxitemsday ? Number(maxitemsday) : Number.MAX_VALUE;
    let mincost = document.getElementById("mincost").value;
    mincost = mincost ? Number(mincost) : 0;
    let minroi = document.getElementById("minroi").value;
    minroi = minroi ? Number(minroi) : 0;

    let displayMarket = [];
    for (let entry of globaldata.market) {
        if (!entry.craftCost) {
            continue;
        }
        if (search) {
            if (!entry.name.toLowerCase().includes(search.toLowerCase())) {
                continue;
            }
        }
        if (entry.id in recipesMap) {
            let recipe = recipesMap[entry.id];
            if (recipe.level < minlevel || recipe.level > maxlevel) {
                continue;
            }
        }
        if (entry.itemsperday < minitemsday || entry.itemsperday > maxitemsday) {
            continue;
        }
        if (entry.medianppu < mincost) {
            continue;
        }
        let roi = entry.medianppu / entry.craftCost;
        if (roi < minroi) {
            continue;
        }
        if (entry.craftCost) {
            displayMarket.push(entry);
        }
    }

    displayMarket.sort(compareMarketEntry);

    let i = 0;
    let generatedHtml = "";
    for (let entry of displayMarket) {
        if (i++ == 100) {
            break;
        }
        generatedHtml += dumpHtml(entry, 1);
    }
    let node = document.getElementById("generated-content");
    node.innerHTML = generatedHtml;
}
