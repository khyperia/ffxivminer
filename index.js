let nametoid = globaldata.nametoid;
let idtoname = {};
for (let name in nametoid) {
    idtoname[nametoid[name]] = name;
}
globaldata.recipes.sort((a, b) => a.resultid - b.resultid);
let recipesMap = {};
for (let recipe of globaldata.recipes) {
    recipe.resultname = idtoname[recipe.resultid];
    for (let ingredient of recipe.ingredients) {
        ingredient.name = idtoname[ingredient.id];
    }
    recipesMap[recipe.resultid] = recipe;
}

function removeItem(arr, value) {
    let index = arr.indexOf(value);
    if (index > -1) {
        arr.splice(index, 1);
    }
}

function tostr(v) {
    return Number(v.toFixed(2));
}

class UniversalisServerCache {
    constructor(server, history) {
        this.server = server;
        this.history = history;
        this.cache = {};
        this.needed = [];
    }

    get(id) {
        if (id in this.cache) {
            return this.cache[id];
        } else if (!this.needed.includes(id)) {
            this.needed.push(id);
            this.needed.sort((a, b) => a - b);
        }
        return null;
    }

    fetchNeeded() {
        return this.needed.length > 0;
    }

    fetch() {
        if (this.needed.length > 0) {
            let url = this.history ?
                `https://universalis.app/api/history/${this.server}/${this.needed.join()}` :
                `https://universalis.app/api/${this.server}/${this.needed.join()}?listings=0&entries=0`;
            fetch(url, {mode: 'cors'})
                .then(response => response.json())
                .then(data => {
                    this.parse(data);
                    item();
                });
        }
    }

    parse(data) {
        console.log(data);
        if (data.items) {
            for (let item of data.items) {
                removeItem(this.needed, item.itemID);
                this.cache[item.itemID] = item;
            }
        } else if (data.itemID) {
            removeItem(this.needed, item.itemID);
            this.cache[data.itemID] = data;
        } else {
            console.log("Could not parse universalis response??");
        }
    }
}

class UniversalisCache {
    constructor(history) {
        this.servers = {};
        this.history = history;
    }

    get(id, server) {
        server = server || document.getElementById("server").value || "Shiva";
        if (!(server in this.servers)) {
            this.servers[server] = new UniversalisServerCache(server, this.history);
        }
        return this.servers[server].get(id);
    }

    fetchNeeded() {
        for (let server in this.servers) {
            if (this.servers[server].fetchNeeded()) {
                return true;
            }
        }
        return false;
    }

    fetch() {
        for (let server in this.servers) {
            this.servers[server].fetch();
        }
    }
}

let universalis = new UniversalisCache(false);
let universalisHistory = new UniversalisCache(true);

function price(id, server) {
    return universalis.get(id, server)?.minPrice || NaN;
}

function priceHQ(id, server) {
    return universalis.get(id, server)?.minPriceHQ || NaN;
}

function velocity(id, server) {
    return universalis.get(id, server)?.regularSaleVelocity || NaN;
}

function velocityHQ(id, server) {
    return universalis.get(id, server)?.hqSaleVelocity || NaN;
}

function velocityHistory(id, server, hq) {
    let hist = universalisHistory.get(id, server);
    if (hist === null) {
        return NaN;
    }
    let minTime = Infinity;
    let maxTime = 0;
    let quantity = 0;
    for (let entry of hist.entries) {
        if (hq && !entry.hq) {
            continue;
        }
        quantity += entry.quantity;
        minTime = Math.min(minTime, entry.timestamp);
        maxTime = Math.max(maxTime, entry.timestamp);
    }
    let seconds = maxTime - minTime;
    let days = seconds / 86400;
    let perDay = quantity / days;
    return perDay;
}

// heck it, who needs runtime complexity
function craftingPrice(recipe, isChild) {
    let sum = 0;
    for (let ingredient of recipe.ingredients) {
        if (ingredient.id in recipesMap) {
            sum += craftingPrice(recipesMap[ingredient.id]) * ingredient.amount;
        } else {
            sum += price(ingredient.id) * ingredient.amount;
        }
    }
    let priceViaCrafting = sum / recipe.resultamount;
    if (!isChild) {
        return sum / recipe.resultamount;
    }
    let priceViaBuying = price(recipe.resultid);
    if (isNaN(priceViaCrafting)) {
        return priceViaBuying;
    } else if (isNaN(priceViaBuying)) {
        return priceViaCrafting;
    } else if (priceViaBuying < priceViaCrafting) {
        return priceViaBuying;
    } else {
        return priceViaCrafting;
    }
}

function priceString(cost, craftingCost) {
    if (isNaN(cost)) {
        if (isNaN(craftingCost)) {
            //return " [unknown marketboard and crafting cost]";
            return "";
        } else if (craftingCost != 0) {
            return " 1x costs " + tostr(craftingCost) + " to craft [unknown marketboard cost]";
        } else {
            //return " [unknown marketboard cost]";
            return "";
        }
    } else {
        if (isNaN(craftingCost)) {
            return " 1x costs " + tostr(cost) + " to buy [unknown crafting cost]";
        } else if (craftingCost != 0) {
            return " 1x costs " + tostr(cost) + " to buy, " + tostr(craftingCost) + " to craft (" + tostr((cost / craftingCost)) + "x ROI)";
        } else {
            return " 1x costs " + tostr(cost) + " to buy";
        }
    }
}

function recipeHeader(id, amount) {
    let name = idtoname[id];
    let generatedHtml = "";
    if (amount == 1) {
        generatedHtml += name;
    } else {
        generatedHtml += tostr(amount) + "x " + name;
    }
    generatedHtml += " - <a href=\"https://universalis.app/market/" + id + "\">open in universalis</a>";
    return generatedHtml;
}

function renderRecipeStep(id, amount, history) {
    let generatedHtml = recipeHeader(id, amount);
    let cost = price(id);
    if (id in recipesMap) {
        generatedHtml += "<ul>";
        let recipe = recipesMap[id];
        if (amount / recipe.resultamount != 1 && recipe.resultamount != 1) {
            generatedHtml += `<li>craft ${tostr((amount / recipe.resultamount))}x ()</li>`;
        }
        generatedHtml += `<li>crafting level ${recipe.level}</li>`
        let craftingCost = craftingPrice(recipe);
        let priceStr = priceString(cost, craftingCost);
        if (priceStr) {
            generatedHtml += "<li>" + priceStr + "</li>";
        }
        let vel = history ? velocityHistory(id) : velocity(id);
        if (vel) {
            generatedHtml += "<li>sells " + tostr(vel) + " items/day, for a market flux of " + tostr(vel * cost) + " and profit flux of " + tostr(vel * (cost - craftingCost)) + "</li>";
        }
        for (let ingredient of recipe.ingredients) {
            generatedHtml += "<li>Ingredient: ";
            let ingAmount = amount * ingredient.amount / recipe.resultamount;
            generatedHtml += renderRecipeStep(ingredient.id, ingAmount);
            generatedHtml += "</li>";
        }
        generatedHtml += "</ul>";
    } else {
        let priceStr = priceString(cost, 0);
        if (priceStr) {
            generatedHtml += ` - ${priceStr}`;
        }
        let vel = history ? velocityHistory(id) : velocity(id);
        if (vel) {
            generatedHtml += ` - ${tostr(vel)} items/day`;
            if (cost) {
                generatedHtml += `- flux ${tostr(vel * cost)}`;
            }
        }
        generatedHtml += "<br/>";
    }
    return generatedHtml;
}

function showRecipe(recipe) {
    showWrapper(() => {
        return renderRecipeStep(recipe.resultid, 1, false);
    });
}

function showCrystals() {
    showWrapper(() => {
        let generatedHtml = "";
        for (let id = 2; id < 20; id++) {
            generatedHtml += renderRecipeStep(id, 1, true);
        }
        return generatedHtml;
    });
}

function arbitrage() {
    showWrapper(() => {
        let things = [];
        for (let id in idtoname) {
            let name = idtoname[id];
            if (name.startsWith("Classical ")) {
                let server = priceHQ(id);
                let light = priceHQ(id, "Light");
                things.push({
                    id: id,
                    name: name,
                    server: server,
                    light: light,
                    profit: server - light,
                    profitFactor: (server - light) / light,
                });
            }
        }
        let generatedHtml = "";
        things.sort((a, b) => b.profitFactor - a.profitFactor);
        for (let thing of things) {
            generatedHtml += `${thing.name} is ${thing.light} on Light and ${thing.server} on server for profit of ${thing.profit} (${thing.profitFactor}x) with ${velocityHQ(thing.id)} items/day - <a href=\"https://universalis.app/market/${thing.id}\">open in universalis</a><br/>`;
        }
        return generatedHtml;
    });
}

function fetchTheStuff() {
    universalis.fetch();
    universalisHistory.fetch();
}

function showWrapper(action) {
    let generatedHtml = action();
    if (universalis.fetchNeeded() || universalisHistory.fetchNeeded()) {
        let button = "<button onclick='fetchTheStuff()'>Fetch costs from universalis</button> (please don't spam this, it hits universalis' API)<br/>";
        generatedHtml = button + generatedHtml;
    }
    let back = "<a href=\"#\" onclick=\"item()\">Back to item list</a><br/>\n";
    generatedHtml = back + generatedHtml;
    let node = document.getElementById("generated-content");
    node.innerHTML = generatedHtml;
}

function showAllRecipes() {
    let generatedHtml = "";
    generatedHtml += "<a href=\"#crystals\" onclick=\"item()\">Show all crafting crystals</a><br/>\n";
    for (let recipe of globaldata.recipes) {
        generatedHtml += "<a href=\"#" + recipe.resultname + "\" onclick=\"item()\">" + recipe.resultname + "</a><br/>\n";
    }
    let node = document.getElementById("generated-content");
    node.innerHTML = generatedHtml;
}

window.onhashchange = event => item();

function item() {
    let hash = window.location.hash;
    if (hash.startsWith("#")) {
        hash = hash.substring(1);
    }
    hash = decodeURIComponent(hash);
    if (hash == "crystals") {
        showCrystals();
        return;
    }
    if (hash == "arbitrage") {
        arbitrage();
        return;
    }
    for (let recipe of globaldata.recipes) {
        if (recipe.resultname == hash || recipe.resultid == hash) {
            showRecipe(recipe);
            return;
        }
    }
    if (hash != "") {
        for (let recipe of globaldata.recipes) {
            if (recipe.resultname.toLowerCase().includes(hash.toLowerCase())) {
                showRecipe(recipe);
                return;
            }
        }
    }
    showAllRecipes();
}