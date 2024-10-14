function setContainerHeights() {
    const containerHeight = setRteContainerMaxHeight();
    initialiseTinyMce(containerHeight);
}

function setRteContainerMaxHeight() {
    const resourceContainer = document.getElementById("resource-cd-container");
    const richTextEditor = document.getElementById("rte-container");
    richTextEditor.style.maxHeight = `${resourceContainer.clientHeight}px`;
    return (+resourceContainer.clientHeight) - 10; // accounts for the padding at the bottom of the resourceContainer
}

function initialiseTinyMce(containerHeight) {
    window.tinymceConf = {
        selector: "textarea",
        plugins: "lists",
        resize: false,
        height: containerHeight,
        promotion: false,
        statusbar: false,
        toolbar: [
            {name: 'history', items: ['undo', 'redo']},
            {name: 'styles', items: ['styles']},
            {name: 'formatting', items: ['bold', 'italic']},
            {name: 'colours', items: ['forecolor', 'backcolor']},
            {name: 'alignment', items: ['alignleft', 'aligncenter', 'alignright', 'alignjustify']},
            {name: 'lists', items: ['bullist', 'numlist']},
            {name: 'indentation', items: ['outdent', 'indent']},
        ],
        license_key: "gpl"
    }

}

function getRTEContent() {
    const html = tinymce.activeEditor.getContent();
    const text = tinymce.activeEditor.getContent({format: "text"});
    const content = {Text: text, Html: html};
    return content;
}

function setRTEContent(content) {
    if (window.tinymce && tinymce.activeEditor) {
        tinymce.activeEditor.setContent(content);
    }
}

function tinyMceReady() {
    return window.tinymce !== undefined;
}
