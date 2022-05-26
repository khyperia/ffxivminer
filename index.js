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

let universalisCache = {}
let universalisCacheNeeded = []

function getUniversalis(id) {
    if (id in universalisCache) {
        return universalisCache[id];
    } else if (!universalisCacheNeeded.includes(id)) {
        universalisCacheNeeded.push(id);
        universalisCacheNeeded.sort((a, b) => a - b);
    }
    return null;
}

function parseUniversalis(data) {
    console.log(data);
    if (data.items) {
        for (let item of data.items) {
            universalisCache[item.itemID] = item;
        }
    } else if (data.itemID) {
        universalisCache[data.itemID] = data;
    } else {
        console.log("Could not parse universalis response??");
    }
}

function fetchUniversalisCacheNeeded() {
    let server = document.getElementById("server").value || "Shiva";
    if (universalisCacheNeeded.length > 0) {
        fetch(`https://universalis.app/api/${server}/${universalisCacheNeeded.join()}?listings=0&entries=0`, {mode: 'cors'})
            .then(response => response.json())
            .then(data => {
                parseUniversalis(data);
                item();
            });
    }
    universalisCacheNeeded = [];
}

function price(id) {
    return getUniversalis(id)?.minPrice || NaN;
}

function velocity(id) {
    return getUniversalis(id)?.regularSaleVelocity || NaN;
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
            return " 1x costs " + Number(craftingCost.toFixed(2)) + " to craft [unknown marketboard cost]";
        } else {
            //return " [unknown marketboard cost]";
            return "";
        }
    } else {
        if (isNaN(craftingCost)) {
            return " 1x costs " + Number(cost.toFixed(2)) + " to buy [unknown crafting cost]";
        } else if (craftingCost != 0) {
            return " 1x costs " + Number(cost.toFixed(2)) + " to buy, " + Number(craftingCost.toFixed(2)) + " to craft (" + Number((cost / craftingCost).toFixed(2)) + "x ROI)";
        } else {
            return " 1x costs " + Number(cost.toFixed(2)) + " to buy";
        }
    }
}

function recipeHeader(id, amount) {
    let name = idtoname[id];
    let generatedHtml = "";
    if (amount == 1) {
        generatedHtml += name;
    } else {
        generatedHtml += Number(amount.toFixed(2)) + "x " + name;
    }
    generatedHtml += " - <a href=\"https://universalis.app/market/" + id + "\">open in universalis</a>";
    return generatedHtml;
}

function renderRecipeStep(id, amount) {
    let generatedHtml = recipeHeader(id, amount);
    generatedHtml += "<ul>";
    let cost = price(id);
    if (id in recipesMap) {
        let recipe = recipesMap[id];
        if (amount / recipe.resultamount != 1 && recipe.resultamount != 1) {
            generatedHtml += `<li>craft ${Number((amount / recipe.resultamount).toFixed(2))}x ()</li>`;
        }
        generatedHtml += `<li>crafting level ${recipe.level}</li>`
        let craftingCost = craftingPrice(recipe);
        let priceStr = priceString(cost, craftingCost);
        if (priceStr) {
            generatedHtml += "<li>" + priceStr + "</li>";
        }
        let vel = velocity(id);
        if (vel) {
            generatedHtml += "<li>sells " + Number(vel.toFixed(2)) + " items/day, for a market flux of " + Number((vel * cost).toFixed(2)) + " and profit flux of " + Number((vel * (cost - craftingCost)).toFixed(2)) + "</li>";
        }
        for (let ingredient of recipe.ingredients) {
            generatedHtml += "<li>Ingredient: ";
            let ingAmount = amount * ingredient.amount / recipe.resultamount;
            generatedHtml += renderRecipeStep(ingredient.id, ingAmount);
            generatedHtml += "</li>";
        }
    } else {
        let priceStr = priceString(cost, 0);
        if (priceStr) {
            generatedHtml += "<li>" + priceStr + "</li>";
        }
    }
    generatedHtml += "</ul>";
    return generatedHtml;
}

function showCrystals(recipe) {
    let generatedHtml = "";
    for (let id = 2; id < 20; id++) {
        let cost = price(id);
        let costString = "[no price]";
        if (!isNaN(cost)) {
            costString = cost;
        }
        generatedHtml += `${idtoname[id]} - ${costString} - <a href=\"https://universalis.app/market/${id}\">open in universalis</a><br/>`;
    }
    if (universalisCacheNeeded.length > 0) {
        let button = "<button onclick='fetchUniversalisCacheNeeded()'>Fetch costs from universalis</button> (please don't spam this, it hits universalis' API)<br/>";
        generatedHtml = button + generatedHtml;
    }
    let back = "<a href=\"#\" onclick=\"item()\">Back to item list</a><br/>\n";
    generatedHtml = back + generatedHtml;
    let node = document.getElementById("generated-content");
    node.innerHTML = generatedHtml;
}

function showRecipe(recipe) {
    universalisCacheNeeded = [];
    let generatedHtml = "";
    generatedHtml += renderRecipeStep(recipe.resultid, 1);
    if (universalisCacheNeeded.length > 0) {
        let button = "<button onclick='fetchUniversalisCacheNeeded()'>Fetch costs from universalis</button> (please don't spam this, it hits universalis' API)<br/>";
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