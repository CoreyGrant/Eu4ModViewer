async function getJson(filename, modId) {
    return await fetch("api/File/Json/" + filename + "/" + modId).then(x => x.json());
}

async function queueMod(request) {
    return await fetch("api/Queue/QueueMod", { method: "POST", body: JSON.stringify(request), headers: {"Content-Type": "application/json"} })
        .then(x => x.json()).then(x => x.result);
}

async function modExists(modId) {
    return await fetch("api/Queue/ModExists/" + modId)
        .then(x => x.json()).then(x => <boolean>x.result);
}

async function modQueued(modId) {
    return await fetch("api/Queue/ModQueued/" + modId)
        .then(x => x.json()).then(x => <boolean>x.result);
}

async function queuedMods() {
    return await fetch("api/Queue/QueuedMods")
        .then(x => x.json()).then(x => x.result);
}

async function getModDetails(modId) {
    return await fetch("api/Mod/" + modId)
        .then(x => x.json());
}

async function getPublishedFileDetails(modId) {
    return await fetch("api/Mod/PublishedFileDetails/" + modId)
        .then(x => x.json());
}

async function getMods() {
    return await fetch("api/Mod")
        .then(x => x.json());
}

function getImageLink(filename, folder, modId) {
    return `api/File/Image/${filename}/${folder}/${modId}`;
}

function getIconLink(filename, folder: string|null = null) {
    return folder
        ? `/Icons/${folder}/${filename}.png`
        : `/Icons/${filename}.png`;
}

export {
    getJson,
    getImageLink,
    getIconLink,

    queueMod,
    modExists,
    modQueued,
    queuedMods,
    getPublishedFileDetails,

    getModDetails,
    getMods
};