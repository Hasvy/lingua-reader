de_regexp = /^[\wßäöüÄÖÜ]*$/;
cs_regexp = /^[\wčďěňřšťžáéíóúůýČĎĚŇŘŠŤŽÁÉÍÓÚŮÝ]*$/;
it_regexp = /^[\wàèéìòóùÀÈÉÌÒÓÙ]*$/;
es_regexp = /^[\wáéíóúñÑÁÉÍÓÚüÜ]*$/;
lang_regexp = /^\w*$/;

function getSelectedWord(hostElement) {
    return new Promise(function (resolve) {
        if (span.parentNode) {
            removeSpan();
        }

        var wordRegexp = lang_regexp;
        var s = hostElement.shadowRoot.getSelection();
        var node = s.anchorNode;
        if (!node) {            //Mb node.nodeName !== '#text'
            return;
        }
        var range = s.getRangeAt(0);

        // Finds a start point of a clicked word
        while ((range.startOffset > 0) && range.toString().match(wordRegexp)) {
            range.setStart(node, (range.startOffset - 1));
        }
        if (!range.toString().match(wordRegexp)) {
            range.setStart(node, range.startOffset + 1);
        }

        // Finds an end point of a clicked word
        while ((range.endOffset < node.length) && range.toString().match(wordRegexp)) {
            range.setEnd(node, range.endOffset + 1);
        }
        if (!range.toString().match(wordRegexp)) {
            range.setEnd(node, range.endOffset - 1);
        }

        // Gets a word, removes selection
        var word = range.toString().trim();
        if (word) {
            addSpan(range);
        }
        window.getSelection().removeAllRanges();

        //Get started position to draw translator window (right upon the clicked word)
        var rangePosition = range.getBoundingClientRect();
        var height = rangePosition.height;
        var width = rangePosition.width;
        var top = rangePosition.top - rzHeader.clientHeight + rzBody.scrollTop - 5;
        var left = rangePosition.left + (width / 2);
        resolve({ word, height, width, top, left });
    });
}

function addSpan(range) {
    range.surroundContents(span);
}

function removeSpan() {
    var cont = document.createTextNode(span.innerHTML);
    span.parentNode.replaceChild(cont, span);
}

function speakWord(word, language) {
    return new Promise(function (resolve) {
        const utterance = new SpeechSynthesisUtterance();
        utterance.text = word;
        utterance.lang = language;
        utterance.onend = () => resolve();
        speechSynthesis.speak(utterance);
    });
}

function setWindowSizeVars() {
    var translatorWindow = document.getElementById("translator-window");
    if (translatorWindow) {
        const rect = translatorWindow.getBoundingClientRect();
        translatorWindow.style.setProperty("--translator-height", Math.ceil(rect.height) + "px");
        translatorWindow.style.setProperty("--translator-width", (Math.ceil(rect.width) / 2) + "px");
    }
}