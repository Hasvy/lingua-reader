﻿de_regexp = /^[\wßäöüÄÖÜ]*$/;
cs_regexp = /^[\wčďěňřšťžáéíóúůýČĎĚŇŘŠŤŽÁÉÍÓÚŮÝ]*$/;
it_regexp = /^[\wàèéìòóùÀÈÉÌÒÓÙ]*$/;
es_regexp = /^[\wáéíóúñÑÁÉÍÓÚüÜ]*$/;
ru_regexp = /^[\wwа-яёА-ЯЁ]*$/;
lang_regexp = /^\w*$/;
whitespace_regexp = /\s/;

function getSelectedWord(hostElement) {
    return new Promise(function (resolve) {
        if (span.parentNode) {
            removeSpan();
        }

        var wordRegexp = lang_regexp;
        var selection = getSelectionText(hostElement);
        var node = selection.anchorNode;
        if (node.nodeName !== '#text') {
            return;
        }
        var range = selection.getRangeAt(0);

        //Test if was selected a single word
        var word = range.toString().trim();
        if (whitespace_regexp.test(word)) {
            return;
        }

        // Finds start point of a clicked word
        while ((range.startOffset > 0) && range.toString().match(wordRegexp)) {
            range.setStart(node, (range.startOffset - 1));
        }
        if (!range.toString().match(wordRegexp)) {
            range.setStart(node, range.startOffset + 1);
        }

        // Finds end point of a clicked word
        while ((range.endOffset < node.length) && range.toString().match(wordRegexp)) {
            range.setEnd(node, range.endOffset + 1);
        }
        if (!range.toString().match(wordRegexp)) {
            if (range.endOffset <= node.length) {
                range.setEnd(node, range.endOffset - 1);
            }
        }

        //Gets a word with set ranges
        var word = range.toString().trim();

        //Highlights a word and removes selection
        addSpan(range);
        window.getSelection().removeAllRanges();

        //Gets start position to draw translator window loading (right upon the clicked word)
        var rangePosition = range.getBoundingClientRect();
        var top = rangePosition.top - rzHeader.clientHeight + rzBody.scrollTop - 5;
        var left = rangePosition.left + (rangePosition.width / 2);
        resolve({ word, top, left });
    });
}

function addSpan(range) {
    if (range.startContainer.nodeType === Node.TEXT_NODE && range.endContainer.nodeType === Node.TEXT_NODE) {
        range.surroundContents(span);
    }
}

function removeSpan() {
    var cont = document.createTextNode(span.innerHTML);
    if (span.parentNode != null) {
        span.parentNode.replaceChild(cont, span);
    }
}

function getSelectionText(hostElement) {
    var selection;
    try {
        selection = hostElement.shadowRoot.getSelection();  //For Chrome and others
    } catch (e) {
        selection = document.getSelection();                //For Firefox
    }
    if (!selection || !selection.rangeCount) {
        return;
    }
    return selection;
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

        var diff = rect.top - rect.height;
        if (diff < 0) {
            var changeTop = Math.abs(diff) + rect.top;
            translatorWindow.style.setProperty("top", changeTop + "px");
        }
    }
}