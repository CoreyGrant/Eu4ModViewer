function ideaGroupHasPolicy(ideaGroupName, policy): boolean {
    if (!policy.allow) {
        return false;
    }
    if (policy.allow.conditions) {
        var conditions = <any[]>policy.allow.conditions;
       
        for (var condition of conditions) {
            if ((<any>condition).value == ideaGroupName) {
                return true;
            }
        }
    }
    if (policy.allow.conditionSets) {
        var conditionSets = <any[]>policy.allow.conditionSets;
        for (var conditionSet of conditionSets) {
            for (var condition of (<any>conditionSet).conditions) {
                if ((<any>condition).value == ideaGroupName) {
                    return true;
                }
            }
        }
    }1
    return false;
}

function policyAllowed(selectedIdeaGroupsIds: number[], policy: any, otherPolicyNames: string[]) : boolean{
    var matches = 0;

    if (otherPolicyNames.includes(policy.name)) {
        return false;
    }

    if (!policy.allow) {
        return false;
    }
    // check list of other policies first
    var limit = policy.allow.policyAllowLimit;
    if (limit) {
        var numberOfDisallowingPolicies = otherPolicyNames.filter(x => limit.policies.includes(x)).length;
        console.log(numberOfDisallowingPolicies, limit.amount);
        if (numberOfDisallowingPolicies >= limit.amount) {
            return false;
        }
    }

    var full = policy.allow.full
    if (full) {
        for (var i = 0; i < full.length; i++) {
            if (selectedIdeaGroupsIds.includes(full[i])) {
                matches++;
            }
        }
    }
    var fullAny = policy.allow.fullAny;
    if (fullAny) {
        for (var i = 0; i < fullAny.length; i++)
        {
            for (var j = 0; j < fullAny[i].ideaGroups; j++) {
                if (selectedIdeaGroupsIds.includes(fullAny[i].ideaGroups[j])) {
                    matches++;
                    break;
                }
            }
        }
    }
    var hiddenTrigger = policy.allow.hiddenTrigger;
    if (hiddenTrigger) {
        if (matches > 0) {
            return true;
        }
    }
    return matches > 1;
}
if (!String.prototype.includes) {
    String.prototype.includes = function (x) {
        return this.indexOf(x) > -1;
    }
}

const displayPercent = val => (parseFloat(val) * 100).toFixed(2) + "%";
const displayTwoDp = val => (parseFloat(val) * 100).toFixed(2);
const displayOneDp = val => (parseFloat(val) * 100).toFixed(1);
const displayZeroDp = val => (parseFloat(val) * 100).toFixed(0);
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
const displayThousand = val => numberWithCommas(parseFloat(val) * 1000);
function displayBonus(bonusValue: string, bonusType: number): string {
    bonusValue = bonusValue.toString();
    var colonialNation = bonusValue.includes("/ColonialNation");
    switch (bonusType) {
        case 0:
            return bonusValue;
        case 1:
            if (colonialNation) {
                var splitValue = bonusValue.split('/');
                return displayPercent(splitValue[0]) + " / Colonial nation";
            }
            if (bonusValue.includes("/")) {
                return bonusValue.split("/").map(displayPercent).join("/");
            }
            return displayPercent(bonusValue);
        case 2:
            if (bonusValue.includes("/")) {
                return bonusValue.split("/").map(displayTwoDp).join("/");
            }
            return displayTwoDp(bonusValue);
        case 3:
            if (bonusValue.includes("/")) {
                return bonusValue.split("/").map(displayOneDp).join("/");
            }
            return displayOneDp(bonusValue);
        case 4:
            if (bonusValue.includes("/")) {
                return bonusValue.split("/").map(displayZeroDp).join("/");
            }
            return displayZeroDp(bonusValue);
        case 5:
            if (bonusValue.includes("/")) {
                return bonusValue.split("/").map(displayThousand).join("/");
            }
            return displayThousand(bonusValue);
        case 6:
            return bonusValue == "yes" ? "Yes" : "No";
        case 7:
        case 8:
        case 9:
            return bonusValue;
        case 10:
            return (parseFloat(bonusValue.split('/')[0]) * 100).toFixed(2) + "% / Colonial nation";
    }
    throw new Error("fuck");
}

function combineBonuses(bonusValue1, bonusValue2, bonusType) {
    var colonialNation = bonusValue1.includes("/ColonialNation");
    switch (bonusType) {
        case 0:
            console.log("none shouldn't be hit");
            return bonusValue1;
        case 1:
            if (colonialNation) {
                var splitValue1 = bonusValue1.split('/');
                var splitValue2 = bonusValue2.split('/');
                return (parseFloat(splitValue1[0].replace("%", "")) + parseFloat(splitValue2[1].replace("%", ""))) + "% / Colonial nation";
            }
            if (bonusValue1.includes("/")) {
                var bonus2Split = bonusValue2.split('/');
                return bonusValue1.split("/")
                    .map((x, i) => (parseFloat(x) + parseFloat(bonus2Split[i])) + "%")
                    .join("/");
            }
            return (parseFloat(bonusValue1) + parseFloat(bonusValue2)) + "%";
        case 2:
            if (bonusValue1.includes("/")) {
                var bonus2Split = bonusValue2.split('/');
                return bonusValue1.split("/")
                    .map((x, i) => (parseFloat(x) + parseFloat(bonus2Split[i]))).join("/");
            }
            return (parseFloat(bonusValue1) + parseFloat(bonusValue2)) + "";
        case 3:
            if (bonusValue1.includes("/")) {
                var bonus2Split = bonusValue2.split('/');
                return bonusValue1.split("/")
                    .map((x, i) => (parseFloat(x) + parseFloat(bonus2Split[i]))).join("/");
            }
            return (parseFloat(bonusValue1) + parseFloat(bonusValue2)) + "";
        case 4:
            if (bonusValue1.includes("/")) {
                var bonus2Split = bonusValue2.split('/');
                return bonusValue1.split("/")
                    .map((x, i) => (parseFloat(x) + parseFloat(bonus2Split[i]))).join("/");
            }
            return (parseFloat(bonusValue1) + parseFloat(bonusValue2)) + "";
        case 5:
            if (bonusValue1.includes("/")) {
                var bonus2Split = bonusValue2.split('/');
                return bonusValue1.split("/")
                    .map((x, i) => (parseFloat(x) + parseFloat(bonus2Split[i]))).join("/");
            }
            return (parseFloat(bonusValue1) + parseFloat(bonusValue2)) + "";
        case 6:
            return bonusValue1 == "yes" && bonusValue2 == "yes";
        case 7:
        case 8:
        case 9:
            return bonusValue1;
        case 10:
            return (parseFloat(bonusValue1.split('%/')[0]) + parseFloat(bonusValue2.split('%/')[0])) + "% / Colonial nation";
    }
    throw new Error("type not found");
}

export default {
    ideaGroupHasPolicy,
    policyAllowed,
    displayBonus,
    combineBonuses
}