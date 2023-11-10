de_regexp = /^[\wßäöüÄÖÜ]*$/;
cs_regexp = /^[\wčďěňřšťžáéíóúůýČĎĚŇŘŠŤŽÁÉÍÓÚŮÝ]*$/;
it_regexp = /^[\wàèéìòóùÀÈÉÌÒÓÙ]*$/;
es_regexp = /^[\wáéíóúñÑÁÉÍÓÚüÜ]*$/;
lang_regexp = /^\w*$/;

function getSelectedWord(hostElement) {
    return new Promise(function (resolve) {
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

        // Gets a word, removes selection and sends it to C#
        var word = range.toString().trim();
        window.getSelection().removeAllRanges();

        var rangePosition = range.getBoundingClientRect();
        var height = 60;
        var width = rangePosition.width;
        var top = rangePosition.top - height - rzHeader.clientHeight + rzBody.scrollTop;
        var left = rangePosition.left;
        resolve({ word, height, width, top, left });
    });
}