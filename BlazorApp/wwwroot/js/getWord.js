function addEventListenerForTextClicked() {
    if (iframeDocument) {
        var elements = iframeDocument.querySelectorAll("p");
        elements.forEach(function (element) {
            returnClickedWord(element);
        });
    }
}

function returnClickedWord(element) {
    element.addEventListener("click", function () {
        var s = window.getSelection();
        var range = s.getRangeAt(0);
        var node = s.anchorNode;
        var word_regexp = /^\w*$/;

        // Найти начальную точку
        while ((range.startOffset > 0) && range.toString().match(word_regexp)) {
            range.setStart(node, (range.startOffset - 1));
        }

        if (!range.toString().match(word_regexp)) {
            range.setStart(node, range.startOffset + 1);
        }

        // Найти конечную точку
        while ((range.endOffset < node.length) && range.toString().match(word_regexp)) {
            range.setEnd(node, range.endOffset + 1);
        }

        if (!range.toString().match(word_regexp)) {
            range.setEnd(node, range.endOffset - 1);
        }

        // Получите текст и удалите выделение
        var word = range.toString().trim();
        alert(word);
        window.getSelection().removeAllRanges();
    });
}


function findClickedWordIndex(event, words) {
    var clickedX = event.clientX;
    var textRect = event.target.getBoundingClientRect();
    var wordIndex = 0;
    var cumulativeWidth = 0;

    for (var i = 0; i < words.length; i++) {
        var wordWidth = getTextWidth(words[i], event.target.style.font);
        cumulativeWidth += wordWidth;
        if (cumulativeWidth >= clickedX - textRect.left) {
            wordIndex = i;
            break;
        }
    }

    return wordIndex;
}

function getTextWidth(text, font) {
    var canvas = document.createElement("canvas");
    var context = canvas.getContext("2d");
    context.font = font;
    return context.measureText(text).width;
}