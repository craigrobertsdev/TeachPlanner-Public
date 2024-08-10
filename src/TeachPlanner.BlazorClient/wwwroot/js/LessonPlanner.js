function setContainerHeights() {
    setRteContainerMaxHeight();
    setContentDescriptionContainerMaxHeight();
}

function setRteContainerMaxHeight() {
    const resourceContainer = document.getElementById("resource-cd-container");
    const richTextEditor = document.getElementById("rte-container");
    richTextEditor.style.maxHeight = `${resourceContainer.clientHeight}px`;

    const editorContainer = document.getElementsByClassName("rte-editor").item(0);
    // 52 accounts for padding at the bottom of the container as well as the padding of the resourceContainer
    editorContainer.style.maxHeight = `${resourceContainer.clientHeight - 52}px`;
    editorContainer.style.height = `${resourceContainer.clientHeight - 52}px`;
}

function setContentDescriptionContainerMaxHeight() {
    const header = document.getElementById("header");
    const resourceContainer = document.getElementById("resource-cd-container");
    resourceContainer.style.maxHeight = `${resourceContainer.clientHeight}px`;
}
