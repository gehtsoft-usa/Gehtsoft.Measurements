function loadStarted() {
    window.onload = loadCompleted;

    if (highlighterEnabled && !window.location.href.indexOf('mk:') == 0) {
        var head = document.getElementsByTagName('head')[0];

        var script1 = document.createElement('script');
        script1.type = 'text/javascript';
        script1.src = './highlighter/highlight.pack.js';
        head.appendChild(script1);

        var script2 = document.createElement('script');
        script2.type = 'text/javascript';
        script2.src = './highlighter/highlight.cshtml.js';
        head.appendChild(script2);
    }
}

function loadCompleted() {
    if (highlighterEnabled && !window.location.href.indexOf('mk:') == 0) {
        hljs.registerLanguage('cshtml-razor', window.hljsDefineCshtmlRazor);
        x = document.querySelectorAll('pre code');
        for (var i = 0; i < x.length; i++) {
            if (x[i].hasAttribute('class')) {
                hljs.highlightBlock(x[i]);
            }
        }
    }
}


function hidediv(divname) {
    document.getElementById(divname).style.display = 'none';
}

function showdiv(divname) {
    document.getElementById(divname).style.display = 'inline';
}

function syncList() {
    if(window.parent.frames[0]) {
        window.parent.frames[0].selectItem(window.location.href, null);
    }
}

function syncList1(key) {
    if(window.parent.frames[0]) {
        window.parent.frames[0].selectItem(window.location.href, key + '.html');
    }
}