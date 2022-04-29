
(function () {

    // Timing loop
    var currentSeconds = -1;
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    var magazineRequestTime;
    var previousPage = 0;
    var currentPage = 0;

    var pagesInCarousel = 1;
    var transitionSecond = -1;
    var loadingPageDuration = 10;
    var pageDuration = 27;

    var pageTickerNo = 153;
    var pageTicking = true;
    var pageTickerInterval = 0;


    function timer() {
        var now = new Date();

        // Cycle the page ticker
        if (pageTicking) {
            pageTickerInterval--;

            if (pageTickerInterval < 0) {
                pageTickerInterval = 10;
                pageTickerNo++;
                if (Math.floor((Math.random() * 10) == 1)) {
                    pageTickerNo++; // Randomly skip a page, makes the ticker look less uniform
                }
                if (pageTickerNo > 199) {
                    pageTickerNo = 100;
                }
                $('#pageTicker').text(pageTickerNo);
            }
        }

        var s = Math.floor(now.getTime() / 1000);
        if (s != currentSeconds) {
            // Update clock

            currentSeconds = s;

            $('#date').text(days[now.getDay()] + ' ' + ('0' + now.getDate()).slice(-2) + ' ' + months[now.getMonth()]);
            $('#clock').text(('0' + now.getHours()).slice(-2) + ':' + ('0' + now.getMinutes()).slice(-2) + '/' + ('0' + now.getSeconds()).slice(-2));

            // Do we need to refresh the magazine ? (takes place every 30 minutes)

            if (now - magazineRequestTime > (30 * 60 * 1000)) {
                magazineRequestTime = now;
                transitionSecond = (now.getSeconds() + loadingPageDuration) % 60;
              
                // Switch back to loading page
                $('#page' + previousPage).hide();
                previousPage = 0;
                $('#magazineLoading').slideDown(300);

                // Turn on the page ticker
                pageTicking = true;

                // Refresh magazine

                $('#magazineContent').load("/carousel");
            }

            // Change page if we've hit the transition second
            if (now.getSeconds() == transitionSecond) {
                // Turn off the ticker
                pageTicking = false;

                // Move to next page
                currentPage = 1 + (Math.floor(((now.getHours() * 3600) + (now.getMinutes() * 60) + now.getSeconds()) / pageDuration) % pagesInCarousel);
                transitionSecond = (transitionSecond + pageDuration) % 60;

                if ($('#page' + currentPage).length > 0) {
                    if (previousPage == 0) {
                        // Hide loading page
                        $('#magazineLoading').hide();
                    }
                    else {
                        $('#page' + previousPage).hide();
                    }

                    // Slide down page
                    $('#page' + currentPage).slideDown(300);
                    $('#pageTicker').text("152");

                    previousPage = currentPage;
                }
                else {
                    // There's a problem - so force request the magazine content again
                    magazineRequestTime = 0;
                    transitionSecond++;
                }
            }
        }

        requestAnimationFrame(timer);
    }

    // Page initialisation
    $(window).load(function () {
        // Default control values
        $('#page').attr('class', 'view');
        $('#selectedPage').text("P152");
        $('#selectedChannel').text("CEEFAX 1");
        $('#imgPause').attr("style", "display:none");

        // Display loading page
        window.setTimeout(function () { $('#magazineLoading').slideDown(300) }, 500);

        // Mark the time we first requested the magazine
        magazineRequestTime = new Date();
        $('#magazineContent').load("/carousel", function () {
            var magazineReceiveTime = new Date();

            var actualWait = (magazineReceiveTime - magazineRequestTime) / 1000;
            if (actualWait >= loadingPageDuration) {
                // More than 15 seconds have passed, so move to the first page straight away
                transitionSecond = (magazineReceiveTime.getSeconds() + 1) % 60;
            }
            else {
                // Magazine received before 15 seconds have passed, so calculate the correct transition second
                transitionSecond = (magazineRequestTime.getSeconds() + loadingPageDuration) % 60;
            }

            // We now know how many pages are in the carousel

            pagesInCarousel = $('#totalPages').html();
        });

        // Start clock and transition timer
        timer();

    });

})();


// Support for background music
var musicOn = false;

function toggleMusic() {
    if (!musicOn) {
        var now = new Date();

        var minutes = now.getMinutes();
        var hours = now.getHours();
        var seconds = now.getSeconds();

        position = ((hours * 60 * 60) + (minutes * 60) + seconds) % (3 * 60 * 60);
        $('audio')[0].play();
        $('audio')[0].currentTime = position;
        $('#imgPlay').attr("style", "display:none");
        $('#imgPause').attr("style", "display:inline");
    }
    else {
        $('audio')[0].pause();
        $('#imgPlay').attr("style", "display:inline");
        $('#imgPause').attr("style", "display:none");
    }

    musicOn = !musicOn;
}


/* Modernizr 2.6.2 (Custom Build) | MIT & BSD
 * Build: http://modernizr.com/download/#-fontface-backgroundsize-csstransforms-inlinesvg-cssclasses-teststyles-testprop-testallprops-domprefixes-css_pointerevents
 */
; window.Modernizr = function (a, b, c) { function z(a) { j.cssText = a } function A(a, b) { return z(prefixes.join(a + ";") + (b || "")) } function B(a, b) { return typeof a === b } function C(a, b) { return !!~("" + a).indexOf(b) } function D(a, b) { for (var d in a) { var e = a[d]; if (!C(e, "-") && j[e] !== c) return b == "pfx" ? e : !0 } return !1 } function E(a, b, d) { for (var e in a) { var f = b[a[e]]; if (f !== c) return d === !1 ? a[e] : B(f, "function") ? f.bind(d || b) : f } return !1 } function F(a, b, c) { var d = a.charAt(0).toUpperCase() + a.slice(1), e = (a + " " + n.join(d + " ") + d).split(" "); return B(b, "string") || B(b, "undefined") ? D(e, b) : (e = (a + " " + o.join(d + " ") + d).split(" "), E(e, b, c)) } var d = "2.6.2", e = {}, f = !0, g = b.documentElement, h = "modernizr", i = b.createElement(h), j = i.style, k, l = {}.toString, m = "Webkit Moz O ms", n = m.split(" "), o = m.toLowerCase().split(" "), p = { svg: "http://www.w3.org/2000/svg" }, q = {}, r = {}, s = {}, t = [], u = t.slice, v, w = function (a, c, d, e) { var f, i, j, k, l = b.createElement("div"), m = b.body, n = m || b.createElement("body"); if (parseInt(d, 10)) while (d--) j = b.createElement("div"), j.id = e ? e[d] : h + (d + 1), l.appendChild(j); return f = ["&#173;", '<style id="s', h, '">', a, "</style>"].join(""), l.id = h, (m ? l : n).innerHTML += f, n.appendChild(l), m || (n.style.background = "", n.style.overflow = "hidden", k = g.style.overflow, g.style.overflow = "hidden", g.appendChild(n)), i = c(l, a), m ? l.parentNode.removeChild(l) : (n.parentNode.removeChild(n), g.style.overflow = k), !!i }, x = {}.hasOwnProperty, y; !B(x, "undefined") && !B(x.call, "undefined") ? y = function (a, b) { return x.call(a, b) } : y = function (a, b) { return b in a && B(a.constructor.prototype[b], "undefined") }, Function.prototype.bind || (Function.prototype.bind = function (b) { var c = this; if (typeof c != "function") throw new TypeError; var d = u.call(arguments, 1), e = function () { if (this instanceof e) { var a = function () { }; a.prototype = c.prototype; var f = new a, g = c.apply(f, d.concat(u.call(arguments))); return Object(g) === g ? g : f } return c.apply(b, d.concat(u.call(arguments))) }; return e }), q.backgroundsize = function () { return F("backgroundSize") }, q.csstransforms = function () { return !!F("transform") }, q.fontface = function () { var a; return w('@font-face {font-family:"font";src:url("https://")}', function (c, d) { var e = b.getElementById("smodernizr"), f = e.sheet || e.styleSheet, g = f ? f.cssRules && f.cssRules[0] ? f.cssRules[0].cssText : f.cssText || "" : ""; a = /src/i.test(g) && g.indexOf(d.split(" ")[0]) === 0 }), a }, q.inlinesvg = function () { var a = b.createElement("div"); return a.innerHTML = "<svg/>", (a.firstChild && a.firstChild.namespaceURI) == p.svg }; for (var G in q) y(q, G) && (v = G.toLowerCase(), e[v] = q[G](), t.push((e[v] ? "" : "no-") + v)); return e.addTest = function (a, b) { if (typeof a == "object") for (var d in a) y(a, d) && e.addTest(d, a[d]); else { a = a.toLowerCase(); if (e[a] !== c) return e; b = typeof b == "function" ? b() : b, typeof f != "undefined" && f && (g.className += " " + (b ? "" : "no-") + a), e[a] = b } return e }, z(""), i = k = null, e._version = d, e._domPrefixes = o, e._cssomPrefixes = n, e.testProp = function (a) { return D([a]) }, e.testAllProps = F, e.testStyles = w, g.className = g.className.replace(/(^|\s)no-js(\s|$)/, "$1$2") + (f ? " js " + t.join(" ") : ""), e }(this, this.document), Modernizr.addTest("pointerevents", function () { var a = document.createElement("x"), b = document.documentElement, c = window.getComputedStyle, d; return "pointerEvents" in a.style ? (a.style.pointerEvents = "auto", a.style.pointerEvents = "x", b.appendChild(a), d = c && c(a, "").pointerEvents === "auto", b.removeChild(a), !!d) : !1 });


// http://paulirish.com/2011/requestanimationframe-for-smart-animating/
// http://my.opera.com/emoller/blog/2011/12/20/requestanimationframe-for-smart-er-animating
// requestAnimationFrame polyfill by Erik MÃ¶ller. fixes from Paul Irish and Tino Zijdel
// MIT license
(function () {
    var lastTime = 0;
    var vendors = ['ms', 'moz', 'webkit', 'o'];
    for (var x = 0; x < vendors.length && !window.requestAnimationFrame; ++x) {
        window.requestAnimationFrame = window[vendors[x] + 'RequestAnimationFrame'];
        window.cancelAnimationFrame = window[vendors[x] + 'CancelAnimationFrame']
								   || window[vendors[x] + 'CancelRequestAnimationFrame'];
    }

    if (!window.requestAnimationFrame)
        window.requestAnimationFrame = function (callback, element) {
            var currTime = new Date().getTime();
            var timeToCall = Math.max(0, 16 - (currTime - lastTime));
            var id = window.setTimeout(function () { callback(currTime + timeToCall); },
			  timeToCall);
            lastTime = currTime + timeToCall;
            return id;
        };

    if (!window.cancelAnimationFrame)
        window.cancelAnimationFrame = function (id) {
            clearTimeout(id);
        };
}());
