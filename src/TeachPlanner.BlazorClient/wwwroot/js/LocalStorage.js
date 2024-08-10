async function loadStateFromLocalStorage() {
    var state = await localStorage.getItem("teacherId");

    if (state === null) {
        console.log("No state found in local storage");
        return null;
    }

    return state;
}

async function getTokenFromLocalStorage() {
    var token = await localStorage.getItem("JWT_KEY");
    return token;
}

async function getAccountSetupStatus() {
    var accountSetupComplete = await localStorage.getItem("AccountSetupComplete");
    return accountSetupComplete == "true" ? true : false;
}