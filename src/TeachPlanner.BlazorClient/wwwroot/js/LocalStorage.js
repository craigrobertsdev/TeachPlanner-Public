async function loadStateFromLocalStorage() {
    const state = localStorage.getItem("teacherId");

    if (state === null) {
        console.log("No state found in local storage");
        return null;
    }

    return state;
}

async function getTokenFromLocalStorage() {
    return localStorage.getItem("JWT_KEY");
}

async function getAccountSetupStatus() {
    const accountSetupComplete = localStorage.getItem("AccountSetupComplete");
    return accountSetupComplete === "true";
}

async function getAccountSettings() {
    const settingsString = localStorage.getItem("settings");
    console.log(settingsString);
    const settings = JSON.parse(settingsString);
    console.log(settings);
    return settings;
}
