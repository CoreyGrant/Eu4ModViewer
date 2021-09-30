function save(key: string, val: any) {
    window.localStorage.setItem(key, JSON.stringify(val));
}

function load(key): any {
    var val = window.localStorage.getItem(key)
    if (val && val.length) {
        return JSON.parse(val);
    }
    return null;
}

export default {
    save,
    load
}