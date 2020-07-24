(function () {
    try {
        eval("async function __foobar__() {}");
        eval("var __arrow_foobar__ = () => {}");
        eval("const __const_foobar__ = undefined;");
        eval("let __let_foobar__ = undefined;");
        return true;
    } catch (e) { return false; }
})(); // Can I run?