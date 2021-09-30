const nameCache = {};
const urlCache = {};

function getName(bonusName: string): string|null{
    return nameCache[bonusName] || null;
}
function setName(bonusName: string, mappedBonusName: string): void{
    nameCache[bonusName] = mappedBonusName;
}
function hasName(bonusName: string): boolean{
    return nameCache[bonusName] !== undefined;
}

function getUrl(bonusName: string): string|null{
    return urlCache[bonusName] || null;
}
function setUrl(bonusName: string, bonusUrl: string): void{
    urlCache[bonusName] = bonusUrl;
}
function hasUrl(bonusName: string): boolean{
    return urlCache[bonusName] !== undefined;
}

export { getName, setName, hasName, getUrl, setUrl, hasUrl };